using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlockTap.Validation
{

internal sealed class BlockTapCompleteRuntimeCapture : MonoBehaviour
{
	private string outputPath;

	private string panelAddress;

	private GameObject gameplayPage;

	private AccountModule.OceanShineWithdrawalPageResponse withdrawalResponse;

	private const string EditorCaptureRequestFile = ".codex-current-ui-capture.request";

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Install()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		string argument = GetArgument(commandLineArgs, "-completeCaptureOutput");
		string requestedPanel = GetArgument(commandLineArgs, "-completeCapturePanel");
#if UNITY_EDITOR
		if (string.IsNullOrWhiteSpace(argument))
		{
			string projectRoot = Directory.GetParent(Application.dataPath).FullName;
			string requestPath = Path.Combine(projectRoot, EditorCaptureRequestFile);
			if (File.Exists(requestPath))
			{
				string[] requestLines = File.ReadAllLines(requestPath);
				if (requestLines.Length > 0)
				{
					argument = requestLines[0].Trim();
				}
				if (requestLines.Length > 1)
				{
					requestedPanel = requestLines[1].Trim();
				}
			}
		}
#endif
		if (!string.IsNullOrWhiteSpace(argument))
		{
			InitializeAllSaveData();
			SeedSaveData();
			GameObject obj = new GameObject("BlockTapCompleteRuntimeCapture");
			UnityEngine.Object.DontDestroyOnLoad(obj);
			BlockTapCompleteRuntimeCapture blockTapCompleteRuntimeCapture = obj.AddComponent<BlockTapCompleteRuntimeCapture>();
			blockTapCompleteRuntimeCapture.outputPath = Path.GetFullPath(argument);
			blockTapCompleteRuntimeCapture.panelAddress = requestedPanel ?? "Gameplay";
		}
	}

	private IEnumerator Start()
	{
		// The desktop player window cannot always grow to 2400 px on a 2160 px
		// monitor. Render the reference aspect ratio at half size and use Unity's
		// supersize capture so the file is always exactly 1080 x 2400.
		Screen.SetResolution(540, 1200, FullScreenMode.Windowed);
		yield return null;
		yield return null;
		yield return new WaitForSecondsRealtime(1.25f);
		LoadTablesForRuntimeCapture();
		yield return new WaitForSecondsRealtime(0.5f);
		float accountWaitUntil = Time.realtimeSinceStartup + 8f;
		while (BaseGameModule<AccountModule>.Instance == null && Time.realtimeSinceStartup < accountWaitUntil)
		{
			yield return null;
		}
		SeedAccountData();
		UnityEngine.Random.InitState(20260814);
		if (!string.Equals(SceneManager.GetActiveScene().name, "GamePlay", StringComparison.OrdinalIgnoreCase))
		{
			AsyncOperation loadGame = SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
			while (loadGame != null && !loadGame.isDone)
			{
				yield return null;
			}
			yield return new WaitForSecondsRealtime(2f);
		}
		// The normal boot flow can hydrate persisted values after the
		// BeforeSceneLoad hook. Reapply the complete in-memory fixture once the
		// gameplay scene is ready so each independent capture starts identically.
		SeedSaveData();
		SeedAccountData();
		Canvas canvas = FindTargetCanvas();
		if (canvas == null)
		{
			Debug.LogError("[BlockTapCompleteRuntimeCapture] No root canvas found.");
			yield return FinishCapture();
			yield break;
		}
		gameplayPage = GameObject.Find("GameRoot");
		GameObject realGameUi = null;
		yield return LoadPanel("UIPanel/RealGamePanel", canvas, "RealGamePanel_CompleteRuntime", delegate(GameObject result)
		{
			realGameUi = result;
		});
		if (realGameUi != null)
		{
			SafeOpen(realGameUi.GetComponent<UIPageBase>());
			CompleteRealGameUi(realGameUi);
		}
		GameObject targetPage = realGameUi != null ? realGameUi : gameplayPage;
		if (!string.Equals(panelAddress, "Gameplay", StringComparison.OrdinalIgnoreCase) && !string.Equals(panelAddress, "UIPanel/RealGamePanel", StringComparison.OrdinalIgnoreCase))
		{
			targetPage = null;
			yield return LoadPanel(panelAddress, canvas, "CapturedPanel_CompleteRuntime", delegate(GameObject result)
			{
				targetPage = result;
			});
			if (targetPage != null)
			{
				try
				{
					ConfigureAndOpen(targetPage, panelAddress);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[BlockTapCompleteRuntimeCapture] Configure warning for " + panelAddress + ": " + ex.GetBaseException().Message);
				}
			}
		}
		yield return null;
		Canvas.ForceUpdateCanvases();
		yield return new WaitForSecondsRealtime(2f);
		SuppressUnexpectedOverlays(targetPage);
		ReapplyDeterministicContent(targetPage);
		FreezeTransientVisuals();
		Canvas.ForceUpdateCanvases();
		yield return FinishCapture();
	}

	private void ReapplyDeterministicContent(GameObject targetPage)
	{
		if (targetPage == null)
		{
			return;
		}
		DailyMissionPanel dailyMission = targetPage.GetComponent<DailyMissionPanel>();
		if (dailyMission != null)
		{
			CompleteDailyMission(dailyMission);
			dailyMission.enabled = false;
		}
		DailyWithdrawPanel dailyWithdraw = targetPage.GetComponent<DailyWithdrawPanel>();
		if (dailyWithdraw != null)
		{
			CompleteDailyWithdraw(dailyWithdraw);
		}
		UIDailyTaskPage dailyTasks = targetPage.GetComponent<UIDailyTaskPage>();
		if (dailyTasks != null)
		{
			CompleteDailyTasks(dailyTasks);
		}
		UITeachMaskPage teachMask = targetPage.GetComponent<UITeachMaskPage>();
		if (teachMask != null)
		{
			StabilizeTeachMask(teachMask);
		}
		UITeachFingerMovePage teachFinger = targetPage.GetComponent<UITeachFingerMovePage>();
		if (teachFinger != null)
		{
			StabilizeTeachFinger(teachFinger);
		}
	}

	private static void FreezeTransientVisuals()
	{
		foreach (Animator animator in UnityEngine.Object.FindObjectsOfType<Animator>(includeInactive: true))
		{
			try
			{
				// Sample the current state at its completed pose. This preserves the
				// state selected by the real runtime while removing frame-time drift
				// between two independent validation launches.
				for (int layer = 0; layer < animator.layerCount; layer++)
				{
					AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layer);
					if (state.fullPathHash != 0)
					{
						animator.Play(state.fullPathHash, layer, 1f);
					}
				}
				animator.Update(0f);
				animator.speed = 0f;
			}
			catch
			{
			}
		}

		foreach (Animation animation in UnityEngine.Object.FindObjectsOfType<Animation>(includeInactive: true))
		{
			try
			{
				foreach (AnimationState state in animation)
				{
					if (!state.enabled)
					{
						continue;
					}
					state.time = Mathf.Max(0f, state.length - 0.001f);
					state.speed = 0f;
				}
				animation.Sample();
			}
			catch
			{
			}
		}

		foreach (ParticleSystem particle in UnityEngine.Object.FindObjectsOfType<ParticleSystem>(includeInactive: true))
		{
			try
			{
				particle.useAutoRandomSeed = false;
				particle.randomSeed = StableVisualSeed(particle.name);
				particle.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
				particle.Simulate(0.75f, withChildren: false, restart: true, fixedTimeStep: true);
				particle.Pause(withChildren: false);
			}
			catch
			{
			}
		}

		// Keep the validation assembly independent from the optional Spine
		// compile reference. Freeze the current runtime pose; never reset to the
		// setup pose because the slot machine and tutorial hand are Spine UIs.
		foreach (MonoBehaviour component in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true))
		{
			if (component == null || component.GetType().Name != "SkeletonGraphic")
			{
				continue;
			}
			try
			{
				Type type = component.GetType();
				FieldInfo timeScale = type.GetField("timeScale", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (timeScale != null)
				{
					timeScale.SetValue(component, 0f);
				}
				object animationState = type.GetProperty("AnimationState", BindingFlags.Instance | BindingFlags.Public)?.GetValue(component);
				if (animationState != null)
				{
					MethodInfo getCurrent = animationState.GetType().GetMethod("GetCurrent", new Type[] { typeof(int) });
					for (int trackIndex = 0; trackIndex < 4; trackIndex++)
					{
						object track = getCurrent?.Invoke(animationState, new object[] { trackIndex });
						if (track == null)
						{
							continue;
						}
						PropertyInfo animationEnd = track.GetType().GetProperty("AnimationEnd", BindingFlags.Instance | BindingFlags.Public);
						PropertyInfo trackTime = track.GetType().GetProperty("TrackTime", BindingFlags.Instance | BindingFlags.Public);
						if (animationEnd != null && trackTime != null && trackTime.CanWrite)
						{
							float end = Convert.ToSingle(animationEnd.GetValue(track));
							trackTime.SetValue(track, Mathf.Max(0f, end - 0.001f));
						}
					}
				}
				type.GetMethod("Update", new Type[] { typeof(float) })?.Invoke(component, new object[] { 0f });
				type.GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.Public)?.Invoke(component, null);
				type.GetMethod("UpdateMesh", new Type[] { typeof(bool) })?.Invoke(component, new object[] { false });
				// SkeletonGraphic.OnDisable clears its CanvasRenderer. Keep the component
				// enabled and freeze only timeScale, otherwise tutorial hands disappear
				// from the captured frame after a correct runtime initialization.
			}
			catch
			{
			}
		}
	}

	private static uint StableVisualSeed(string value)
	{
		unchecked
		{
			uint hash = 2166136261u;
			foreach (char character in value)
			{
				hash = (hash ^ character) * 16777619u;
			}
			return hash == 0u ? 1u : hash;
		}
	}

	private IEnumerator FinishCapture()
	{
		DumpUi(Path.ChangeExtension(outputPath, ".ui.tsv"));
		string directoryName = Path.GetDirectoryName(outputPath);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		ScreenCapture.CaptureScreenshot(outputPath, 2);
		float timeoutAt = Time.realtimeSinceStartup + 12f;
		while (!File.Exists(outputPath) && Time.realtimeSinceStartup < timeoutAt)
		{
			yield return new WaitForSecondsRealtime(0.25f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit((!File.Exists(outputPath)) ? 2 : 0);
#endif
	}

	private void ConfigureAndOpen(GameObject target, string address)
	{
		UIPageBase component = target.GetComponent<UIPageBase>();
		if (!(component == null))
		{
			if (component is AddPropPanel)
			{
				// The approved Item reference uses the round clear-colour bomb.
				// Capture the same item type so the runtime icon and copy can be
				// compared directly against the frozen effect image.
				SafeOpen(component, E_ItemType.GameProp_ClearArea);
			}
			else if (component is CommonConfirmTipsPanel)
			{
				SafeOpen(component, new CommonConfirmTipsPanel.Args
				{
					isLanguage = false,
					des = "Your progress has been saved successfully.",
					onFinish = null
				});
			}
			else if (component is DailyMissionPanel page)
			{
				CompleteDailyMission(page);
			}
			else if (component is DailyWithdrawPanel page2)
			{
				CompleteDailyWithdraw(page2);
			}
			else if (component is ExchangeRatePanel)
			{
				SafeOpen(component, new ExchangeRateInfo
				{
					beforeBlance = 100.0,
					beforeClash = 2.0,
					nowBlance = 100.0,
					nowClash = 5.0,
					beforeRate = 0.02,
					nowRate = 0.05
				});
			}
			else if (component is FakeWithdrawPanel)
			{
				SafeOpen(component);
			}
			else if (component is SlotPanel)
			{
				SaveDataUtils.GameData.playerPassLv = 5;
				SafeOpen(component);
			}
			else if (component is GetRewardPanel)
			{
				SaveDataUtils.GameData.playerSelectedLv = 6;
				SafeOpen(component, new ItemEntry
				{
					Type = E_ItemType.Gold,
					Count = 1000f
				}, new ItemEntry
				{
					Type = E_ItemType.Dollar,
					Count = 16.2f
				}, DoubleGetRewardPanel.E_UseScene.WinPanel, null);
			}
			else if (component is LosePanel)
			{
				SafeOpen(component);
			}
			else if (component is ServiceSelectPanel)
			{
				SafeOpen(component, (ServicePanel)null);
			}
			else if (component is NewbieGiftPage)
			{
				SafeOpen(component);
			}
			else if (component is RealWithdrawPanel page3)
			{
				CompleteRealWithdraw(page3);
			}
			else if (component is ServicePanel page4)
			{
				CompleteService(page4);
			}
			else if (component is UIWithdrawalPanel page5)
			{
				CompleteWithdrawalForm(page5);
			}
			else if (component is UIWithdrawalConfirmPanel page6)
			{
				CompleteWithdrawalConfirm(page6);
			}
			else if (component is WithdrawHistory page7)
			{
				CompleteWithdrawHistory(page7);
			}
			else if (component is WithdrawDanPanel)
			{
				SafeOpen(component);
			}
			else if (component is UIDailyTaskPage page8)
			{
				CompleteDailyTasks(page8);
			}
			else if (component is UITeachTipsPage page9)
			{
				CompleteTeachTip(page9);
			}
			else if (component is UITeachMaskPage page10)
			{
				CompleteTeachMask(page10);
			}
			else if (component is UITeachMaskFocusPage page11)
			{
				CompleteTeachFocus(page11);
			}
			else if (component is UITeachFingerMovePage page12)
			{
				CompleteTeachFinger(page12);
			}
			else
			{
				SafeOpen(component);
			}
			NormalizeVisibleText(target, address);
			if (component is RealWithdrawPanel realWithdraw)
			{
				CompleteRealWithdrawText(realWithdraw);
			}
			if (component is GetRewardPanel getReward)
			{
				CompleteGetRewardText(getReward);
			}
		}
	}

	private static void CompleteGetRewardText(GetRewardPanel page)
	{
		SetTextAt(page.transform, "BG/Image (2)/Text (TMP) (1)", "Level 5");
		SetTextAt(page.transform, "BG/Content/Rewards/Reward_2/Rewards/Reward_2/Image_Title_2/Text (TMP)", "$16.2");
	}

	private void CompleteDailyMission(DailyMissionPanel page)
	{
		page.hintsTxt.text = "Watch 10 videos today and withdraw R$0.20";
		page.refreshTimeTxt.text = "Refreshes daily in 08:00:00";
		page.adsCountTxt.text = "3/10";
		page.GoObj.SetActive(value: true);
		page.WithdrawObj.SetActive(value: false);
		page.ClaimedObj.SetActive(value: false);
		page.claimedHint.gameObject.SetActive(value: false);
	}

	private static void CompleteRealGameUi(GameObject realGameUi)
	{
		SetTextAt(realGameUi.transform, "Content/CurrencyBar/CurrentGroup/GoldGroup/CoinBox/GoldText", "0,72");
		SetTextAt(realGameUi.transform, "Content/CurrencyBar/CurrentGroup/GoldGroup/ButtonView/Text (TMP)", "R$0,02");
		SetTextAt(realGameUi.transform, "Content/CurrencyBar/CurrentGroup/GoldGroup/ButtonView/Text (TMP) (1)", "↔");
		SetTextAt(realGameUi.transform, "Content/CurrencyBar/CurrentGroup/DollarGroup/DollarBox/DollarText", "R$46,60");
		SetTextAt(realGameUi.transform, "Content/SlotEnter/Content/Progress/Text (TMP)", "3/5");

		Transform slotIcon = realGameUi.transform.Find("Content/SlotEnter/Content/Icon");
		if (slotIcon is RectTransform slotIconRect)
		{
			slotIconRect.anchoredPosition += new Vector2(-100f, 0f);
		}
		Transform slotEnter = realGameUi.transform.Find("Content/SlotEnter");
		if (slotEnter != null)
		{
			// Avoid a hard compile-time dependency on the Spine assembly in the
			// standalone validation project.  The real component is still located
			// and adjusted at runtime by its concrete type name.
			foreach (MonoBehaviour skeleton in slotEnter.GetComponentsInChildren<MonoBehaviour>(includeInactive: true))
			{
				if (skeleton.GetType().Name != "SkeletonGraphic")
				{
					continue;
				}
				if (skeleton.transform is RectTransform scoreRect)
				{
					scoreRect.anchoredPosition += new Vector2(100f, 0f);
				}
			}
		}

		foreach (TMP_Text label in UnityEngine.Object.FindObjectsOfType<TMP_Text>(includeInactive: true))
		{
			if (label.name == "LevelLabel" && label.gameObject.activeInHierarchy && label.text.Trim() != "Level")
			{
				label.text = "4";
			}
		}
	}

	private static void CompleteRealWithdrawText(RealWithdrawPanel page)
	{
		SetTextAt(page.transform, "Title/Title", "Sacar");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CurrentCount/MyBalance", "Meu Saldo");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawMode/Title", "Selecionar forma de saque");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn/Btn/Text (TMP)", "Retirar");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawInfo/_Hint", "Quanto maior o nível, maior o valor de troca!");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawLevel/ProgressInfo/CompleteHint", "Concluído: Valor a Receber:<color=#618d2a>0,10</color>");
		SetTextAt(page.transform, "Content/Scroll View/Viewport/Content/WithdrawLevel/ProgressInfo/progressHint", "100=R$2,50");
	}

	private static void SetTextAt(Transform root, string path, string value)
	{
		Transform child = root.Find(path);
		TMP_Text label = child != null ? child.GetComponent<TMP_Text>() : null;
		if (label != null)
		{
			label.text = value;
		}
	}

	private static void CompleteDailyWithdraw(DailyWithdrawPanel page)
	{
		page.balanceTxt.text = "100.00";
		page.clashTxt.text = "46.60";
		page.withdrawalTxt.text = "R$46.60";
		LayoutRebuilder.ForceRebuildLayoutImmediate(page.root);
	}

	private void CompleteRealWithdraw(RealWithdrawPanel page)
	{
		page.fingerObj.SetActive(value: false);
		page.canWithdrawFingerHint.SetActive(value: false);
		page.balanceTxt.text = "3,83";
		page.clashTxt.text = "≈R$0,10";
		page.rateTxt.text = "100 ≈ R$2,50";
		page.hintTxt.text = "PagBank: Você pode sacar R$0,10.";
		page.passLevelText.text = "Passe do Nivel:14";
		page.levelObj.SetActive(value: true);
		foreach (Transform item in page.withdrawLevelRoot)
		{
			item.gameObject.SetActive(value: false);
		}
		string[] levels = { "Nível 1", "Passe do Nivel 20", "Passe do Nivel 80", "Passe do Nivel 250", "Passe do Nivel 500", "Passe do Nivel 1000" };
		string[] rates = { "1.0X", "1.2X", "1.4X", "1.6X", "1.8X", "2.0X" };
		string[] balances = { "R$0,10", "R$0,11", "R$0,13", "R$0,15", "R$0,17", "R$0,19" };
		for (int i = 0; i < levels.Length; i++)
		{
			WithdrawLevelItem levelItem = UnityEngine.Object.Instantiate(page.withdrawLevelItem, page.withdrawLevelRoot, false);
			levelItem.name = "RuntimeWithdrawLevel_" + (i + 1);
			levelItem.gameObject.SetActive(value: true);
			levelItem.levelTxt.text = levels[i];
			levelItem.rateText.text = rates[i];
			levelItem.balanceText.text = balances[i];
			levelItem.shadowObj.SetActive(i > 0);
			levelItem.selectedObj.SetActive(value: false);
			levelItem.selectObj.SetActive(i == 0);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(page.withdrawLevelRoot as RectTransform);
		List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform> os_Wwf = EnsureWithdrawalResponse().Os_Wwf;
		if (os_Wwf == null || os_Wwf.Count == 0)
		{
			return;
		}
		page.withdrawWayRoot.gameObject.SetActive(value: true);
		List<string> withdrawalVisualDiagnostics = new List<string>();
		foreach (Transform item in page.withdrawWayRoot)
		{
			item.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < os_Wwf.Count; i++)
		{
			// Use the real prefab item so the capture contains the same frame,
			// selection mark, button hierarchy, and logo sizing as gameplay.
			WithdrawWay method = UnityEngine.Object.Instantiate(page.withdrawWayItem, page.withdrawWayRoot, false);
			method.name = "RuntimeWithdrawalMethod_" + (i + 1);
			method.gameObject.SetActive(value: true);
			method.Init(delegate { }, os_Wwf[i]);
			Image runtimeFrame = method.transform.Find("Frame")?.GetComponent<Image>();
			withdrawalVisualDiagnostics.Add(runtimeFrame == null
				? i + "|FrameMissing"
				: i + "|" + (runtimeFrame.sprite != null ? runtimeFrame.sprite.name : "SpriteMissing")
					+ "|" + runtimeFrame.color);
			if (i == 0)
			{
				method.OnSelect();
			}
		}
		#if UNITY_EDITOR
		File.WriteAllLines(
			Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".codex-withdraw-runtime-visual.txt"),
			withdrawalVisualDiagnostics);
		#endif
		LayoutRebuilder.ForceRebuildLayoutImmediate(page.withdrawWayRoot as RectTransform);
		Debug.Log("[BlockTapCompleteRuntimeCapture] Withdrawal methods populated=" + os_Wwf.Count + ", hierarchy children=" + page.withdrawWayRoot.childCount);
	}

	private void SuppressUnexpectedOverlays(GameObject targetPage)
	{
		// GamePlay can restore persisted popups while the requested validation
		// prefab is loading. They must not leak into another panel's screenshot.
		// Keep only the authored gameplay background and the requested page.
		foreach (UIPageBase page in UnityEngine.Object.FindObjectsOfType<UIPageBase>(includeInactive: true))
		{
			if (page == null || page.gameObject == targetPage ||
				string.Equals(page.gameObject.name, "RealGamePanel_CompleteRuntime", StringComparison.Ordinal))
			{
				continue;
			}

			page.gameObject.SetActive(value: false);
		}

		foreach (CommonConfirmTipsPanel popup in UnityEngine.Object.FindObjectsOfType<CommonConfirmTipsPanel>(includeInactive: true))
		{
			if (targetPage == null || popup.gameObject != targetPage)
			{
				popup.gameObject.SetActive(value: false);
			}
		}

		foreach (LoadingPanel loading in UnityEngine.Object.FindObjectsOfType<LoadingPanel>(includeInactive: true))
		{
			if (targetPage == null || loading.gameObject != targetPage)
			{
				loading.gameObject.SetActive(value: false);
			}
		}
	}

	private static void CompleteService(ServicePanel page)
	{
		for (int num = page.contentRoot.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(page.contentRoot.GetChild(num).gameObject);
		}
		page.chatElements.Clear();
		page.inputText.Text = string.Empty;
		page.selectQuestionText.text = "Choose a topic or type a message";
		InvokePrivate(page, "SwitchDefaultInputState", false);
		InvokePrivate(page, "RefreshSendState", false);
		ChatInfo[] array = new ChatInfo[6]
		{
			new ChatInfo(Spokesperson.Issue, "How can we help you? Choose a topic or start a chat.", "10:20"),
			new ChatInfo(Spokesperson.Issue, "GAMEPLAY — Questions about levels, boosters or controls.", "10:21"),
			new ChatInfo(Spokesperson.Issue, "REWARDS — Help with coins, bonuses and special offers.", "10:22"),
			new ChatInfo(Spokesperson.Issue, "WITHDRAW — Questions about cash out and withdrawal.", "10:23"),
			new ChatInfo(Spokesperson.Player, "What is the minimum amount to withdraw?", "10:24"),
			new ChatInfo(Spokesperson.Issue, "The minimum withdrawal amount is R$50.00. Complete tasks to reach it.", "10:25")
		};
		foreach (ChatInfo chatInfo in array)
		{
			page.CreateElements(chatInfo, 0, isRecord: false);
		}
		TMP_Text[] componentsInChildren = page.GetComponentsInChildren<TMP_Text>(includeInactive: true);
		foreach (TMP_Text tMP_Text in componentsInChildren)
		{
			if (string.Equals(tMP_Text.text, "Feedback", StringComparison.OrdinalIgnoreCase))
			{
				tMP_Text.text = "SUPPORT";
			}
			else if (string.Equals(tMP_Text.name, "Placeholder", StringComparison.OrdinalIgnoreCase))
			{
				tMP_Text.text = "Type your message...";
			}
		}
	}

	private static void CompleteWithdrawHistory(WithdrawHistory page)
	{
		List<AccountModule.OceanShineWithdrawalRecord> data = new List<AccountModule.OceanShineWithdrawalRecord>
		{
			NewWithdrawRecord(50.0, "PagBank", "15/05/2025 10:45", 3, string.Empty),
			NewWithdrawRecord(30.0, "PIX", "12/05/2025 14:10", 1, string.Empty),
			NewWithdrawRecord(20.0, "PagBank", "10/05/2025 09:20", 2, "Account verification required")
		};
		FailHttpResponse<List<AccountModule.OceanShineWithdrawalRecord>> failHttpResponse = default(FailHttpResponse<List<AccountModule.OceanShineWithdrawalRecord>>);
		failHttpResponse.success = true;
		failHttpResponse.data = data;
		failHttpResponse.message = string.Empty;
		failHttpResponse.errorCode = "200";
		FailHttpResponse<List<AccountModule.OceanShineWithdrawalRecord>> failHttpResponse2 = failHttpResponse;
		InvokePrivate(page, "Refresh", failHttpResponse2);
		TMP_Text[] componentsInChildren = page.GetComponentsInChildren<TMP_Text>(includeInactive: true);
		foreach (TMP_Text tMP_Text in componentsInChildren)
		{
			if (string.Equals(tMP_Text.text, "Transactions", StringComparison.OrdinalIgnoreCase))
			{
				tMP_Text.text = "TRANSACTION HISTORY";
			}
			else if (tMP_Text.text != null && tMP_Text.text.StartsWith("$ ", StringComparison.Ordinal))
			{
				tMP_Text.text = "R$ " + tMP_Text.text.Substring(2);
			}
		}
	}

	private static AccountModule.OceanShineWithdrawalRecord NewWithdrawRecord(double amount, string payment, string date, int status, string failure)
	{
		return new AccountModule.OceanShineWithdrawalRecord
		{
			Os_Cp = "123.456.789-00",
			Os_Crc = "BRL",
			Os_Crcs = "R$",
			Os_Dat = date,
			Os_Id = DateTime.UtcNow.Ticks,
			Os_Prc = amount,
			Os_Pym = payment,
			Os_Ra = "player@example.com",
			Os_Re = "player@example.com",
			Os_Rn = "Player Name",
			Os_Sts = status,
			Os_Tsm = failure
		};
	}

	private void CompleteWithdrawalForm(UIWithdrawalPanel page)
	{
		List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform> os_Wwf = EnsureWithdrawalResponse().Os_Wwf;
		AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform withdrawalPlatform = os_Wwf.First((AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform item) => item.Os_Cn == UIWithdrawalPanel.pagBankInfo);
		SafeOpen(page, withdrawalPlatform, os_Wwf, E_WithdrawType.Real, null, false);
		page.CPFNumberInput.Text = "123.456.789-00";
		page.accountNameInput.Text = "Player Name";
		page.paypalMailInput.Text = "player@example.com";
	}

	private void CompleteWithdrawalConfirm(UIWithdrawalConfirmPanel page)
	{
		AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform data = EnsureWithdrawalResponse().Os_Wwf.First((AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform item) => item.Os_Cn == UIWithdrawalPanel.pagBankInfo);
		WithDrawInfo withDrawInfo = new WithDrawInfo(null, E_WithdrawType.Real, E_PayeeAccountType.Pagbank, PayeeAccountType.CpfCnpj, "123.456.789-00", "Player Name", "player@example.com", string.Empty, data);
		SafeOpen(page, withDrawInfo);
	}

	private static void CompleteDailyTasks(UIDailyTaskPage page)
	{
		page.DailyRootObj.SetActive(value: true);
		page.PlayTimeRootObj.SetActive(value: false);
		if (page.titleText != null)
		{
			page.titleText.text = "DAILY TASKS";
		}
		for (int num = page.dailyTaskRoot.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(page.dailyTaskRoot.GetChild(num).gameObject);
		}
		string[] array = new string[5] { "Clear 20 tiles", "Use a booster", "Complete a level", "Create a match of 5", "Play for 10 minutes" };
		int[] array2 = new int[5] { 0, 0, 0, 0, 3 };
		int[] array3 = new int[5] { 20, 1, 1, 5, 10 };
		for (int i = 0; i < array.Length; i++)
		{
			UIDailyTaskElement uIDailyTaskElement = UnityEngine.Object.Instantiate(page.dailyTaskElement, page.dailyTaskRoot, worldPositionStays: false);
			uIDailyTaskElement.name = "RuntimeTask_" + (i + 1);
			uIDailyTaskElement.gameObject.SetActive(value: true);
			uIDailyTaskElement.descTxt.text = array[i];
			uIDailyTaskElement.progressTxt.text = array2[i] + "/" + array3[i];
			uIDailyTaskElement.progressBar.fillAmount = (float)array2[i] / (float)array3[i];
			uIDailyTaskElement.objRewarded.SetActive(value: false);
			uIDailyTaskElement.objRewardedMask.SetActive(value: false);
			uIDailyTaskElement.btnReward.gameObject.SetActive(value: false);
			uIDailyTaskElement.btnAds.gameObject.SetActive(value: false);
			uIDailyTaskElement.btnGoto.gameObject.SetActive(value: true);
			TMP_Text componentInChildren = uIDailyTaskElement.btnGoto.GetComponentInChildren<TMP_Text>(includeInactive: true);
			if (componentInChildren != null)
			{
				componentInChildren.text = ((i == 4) ? "PLAY" : "GO");
			}
			for (int j = 0; j < uIDailyTaskElement.stateObjs.Length; j++)
			{
				uIDailyTaskElement.stateObjs[j].SetActive(j == 1);
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(page.dailyTaskRoot);
		foreach (RectTransform redPoint in page.dailyTaskRoot.GetComponentsInChildren<RectTransform>(includeInactive: true))
		{
			if (redPoint.name == "RedPoint" && redPoint.parent != null && redPoint.parent.name == "UIRedPoint")
			{
				redPoint.anchoredPosition = new Vector2(0f, -5f);
			}
		}
	}

	private void CompleteTeachTip(UITeachTipsPage page)
	{
		SafeOpen(page, new UITeachTipsPage.InitParam
		{
			content = "",
			posIdx = -1,
			block = true,
			alpha = 145f,
			heightValue = 0.67f
		});
		page.content.text = "Tap a group of matching blocks to clear it.";
	}

	private void CompleteTeachMask(UITeachMaskPage page)
	{
		GameObject gameObject = FindGameplayTarget();
		SafeOpen(page, new UITeachMaskPage.InitParam
		{
			target = gameObject,
			type = UITeachMaskPage.Type.Pos,
			worldPos = ((gameObject != null) ? gameObject.transform.position : Vector3.zero),
			width = 230f,
			height = 230f,
			block = true,
			shape = UITeachMaskPage.Shape.Rect,
			alpha = 175f,
			retryCount = 1,
			retryInterval = 0.1f,
			clickCD = 0.5f,
			bgBlock = false,
			showHand = true
		});
		StabilizeTeachMask(page);
	}

	private static void StabilizeTeachMask(UITeachMaskPage page)
	{
		RectTransform root = page.transform as RectTransform;
		if (root == null)
		{
			return;
		}
		// MaskParent is bottom-left anchored. Position it explicitly inside the
		// board instead of converting coordinates between the gameplay canvas and
		// popup canvas, which placed the cut-out partly outside the screen.
		page.maskParent.gameObject.SetActive(true);
		page.maskParent.anchoredPosition = new Vector2(root.rect.width * 0.5f, root.rect.height * 0.385f);
		page.maskParent.sizeDelta = new Vector2(250f, 250f);
		page.finger.gameObject.SetActive(true);
	}

	private void CompleteTeachFocus(UITeachMaskFocusPage page)
	{
		GameObject gameObject = FindGameplayTarget();
		if (!(gameObject == null))
		{
			RectTransform rectTransform = page.transform as RectTransform;
			page.maskParent.anchoredPosition = new Vector2(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.38f);
			page.maskParent.sizeDelta = new Vector2(250f, 250f);
			Color color = page.block.color;
			color.a = 35f / 51f;
			page.block.color = color;
			page.enabled = false;
			Debug.Log("[BlockTapCompleteRuntimeCapture] Teach focus canvas position=" + page.maskParent.anchoredPosition.ToString() + ", size=" + page.maskParent.sizeDelta.ToString() + ", target=" + gameObject.name);
		}
	}

	private void CompleteTeachFinger(UITeachFingerMovePage page)
	{
		GameObject gameObject = FindGameplayTarget();
		Vector3 vector = ((gameObject != null) ? gameObject.transform.position : new Vector3(-180f, 0f, 0f));
		SafeOpen(page, new UITeachFingerMovePage.MoveArgs
		{
			worldStartPos = vector,
			worldEndPos = vector + new Vector3(220f, 0f, 0f),
			duration = 1.2f,
			onClose = null
		});
		StabilizeTeachFinger(page);
	}

	private static void StabilizeTeachFinger(UITeachFingerMovePage page)
	{
		// Freeze the looping gesture on an actual board cell for a complete,
		// deterministic runtime screenshot. The hierarchy and prefab stay intact.
		page.StopAllCoroutines();
		page.finger.gameObject.SetActive(true);
		page.finger.anchoredPosition = new Vector2(-120f, -120f);
	}

	private GameObject FindGameplayTarget()
	{
		Canvas canvas = FindTargetCanvas();
		if (canvas == null)
		{
			return null;
		}
		RectTransform[] source = (from rect in canvas.GetComponentsInChildren<RectTransform>(includeInactive: true)
			where rect.name.StartsWith("Block_", StringComparison.OrdinalIgnoreCase)
			select rect).ToArray();
		RectTransform rectTransform = source.FirstOrDefault((RectTransform rect) => rect.name.StartsWith("Block_3_3_", StringComparison.OrdinalIgnoreCase));
		if (rectTransform != null)
		{
			return rectTransform.gameObject;
		}
		return source.FirstOrDefault()?.gameObject ?? gameplayPage;
	}

	private static void NormalizeVisibleText(GameObject target, string address)
	{
		string displayTitle = GetDisplayTitle(address);
		TMP_Text[] componentsInChildren = target.GetComponentsInChildren<TMP_Text>(includeInactive: true);
		foreach (TMP_Text tMP_Text in componentsInChildren)
		{
			string text = ((tMP_Text.text == null) ? string.Empty : tMP_Text.text.Trim());
			if (text.EndsWith("_Title", StringComparison.OrdinalIgnoreCase) || text.EndsWith("_Tittle", StringComparison.OrdinalIgnoreCase) || text == "Task_Title" || text == "Setting_Title")
			{
				tMP_Text.text = displayTitle;
			}
			else if (text.StartsWith("Btn_", StringComparison.OrdinalIgnoreCase))
			{
				tMP_Text.text = text.Substring(4).Replace('_', ' ').ToUpperInvariant();
			}
			else if (text == "New Text")
			{
				tMP_Text.text = string.Empty;
			}
		}
	}

	private static string GetDisplayTitle(string address)
	{
		string text = (address.Contains('/') ? address.Substring(address.LastIndexOf('/') + 1) : address);
		return text switch
		{
			"AddPropPanel" => "BOOSTER", 
			"CommonConfirmTipsPanel" => "NOTICE", 
			"DailyMissionPanel" => "DAILY MISSION", 
			"DailyWithdrawPanel" => "DAILY WITHDRAW", 
			"ExchangeRatePanel" => "EXCHANGE RATE", 
			"FakeWithdrawPanel" => "WITHDRAW REWARDS", 
			"FAQPanel" => "FAQ", 
			"GetRewardPanel" => "Congratulations", 
			"LosePanel" => "TRY AGAIN", 
			"NewbieGiftPage" => "NEW PLAYER GIFT", 
			"PausePanel" => "SETTINGS", 
			"RealWithdrawPanel" => "WITHDRAW", 
			"ServicePanel" => "SUPPORT", 
			"ServiceSelectPanel" => "CHOOSE A QUESTION", 
			"SlotFAQPanel" => "SLOT RULES", 
			"SlotPanel" => "SLOTS", 
			"StarRatingPopup" => "RATE US", 
			"UI_DailyTaskPage" => "DAILY TASKS", 
			"UIWithdrawalConfirmPanel" => "CONFIRM WITHDRAWAL", 
			"UIWithdrawalPanel" => "WITHDRAWAL DETAILS", 
			"WithdrawDanPanel" => "WITHDRAW LEVELS", 
			"WithdrawHistory" => "TRANSACTION HISTORY", 
			_ => text.Replace("Panel", string.Empty).Replace("Page", string.Empty).ToUpperInvariant(), 
		};
	}

	private void SeedAccountData()
	{
		AccountModule.CountryType = AccountModule.E_CountryType.BR;
		LanguageUtils.SelectedLanguage = "en-US";
		if (!(BaseGameModule<AccountModule>.Instance == null))
		{
			BaseGameModule<AccountModule>.Instance.Os_Current_Uso = new AccountModule.UserInfo
			{
				Os_Bac = 100.0,
				Os_Baci = 10000,
				Os_Con = 72,
				Os_Crc = "BRL",
				Os_Crcs = "R$",
				Os_Cty = "BR",
				Os_Ewl = 46.6,
				Os_Img = 1,
				Os_Lev = 4,
				Os_Lgd = 3,
				Os_Mny = 46.6,
				Os_Ncm = false,
				Os_Nnm = "Runtime QA",
				Os_Rol = 0.72,
				Os_Rts = 4,
				Os_Tra = 0.05
			};
			withdrawalResponse = new AccountModule.OceanShineWithdrawalPageResponse
			{
				Os_Uso = BaseGameModule<AccountModule>.Instance.Os_Current_Uso,
				Os_Wwf = new List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform>
				{
					new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform
					{
						Os_Cn = UIWithdrawalPanel.pagBankInfo,
						Os_Me = "Bank Transfer",
						Os_Mid = 1,
						Os_Mlt = 50.0,
						Os_Et = "Withdraw to your bank account",
						Os_Id = 1
					},
					new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform
					{
						Os_Cn = UIWithdrawalPanel.pixInfo,
						Os_Me = "PIX / E-Wallet",
						Os_Mid = 2,
						Os_Mlt = 50.0,
						Os_Et = "Fast electronic transfer",
						Os_Id = 2
					}
				},
				Os_Wr = new List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio>
				{
					new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio
					{
						Os_Ady = 0,
						Os_Ben = 0,
						Os_End = 5,
						Os_Wro = 0.02
					},
					new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio
					{
						Os_Ady = 0,
						Os_Ben = 5,
						Os_End = 15,
						Os_Wro = 0.03
					},
					new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio
					{
						Os_Ady = 0,
						Os_Ben = 15,
						Os_End = 100,
						Os_Wro = 0.05
					}
				}
			};
			typeof(AccountModule).GetField("withdrawalPageResponse", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(BaseGameModule<AccountModule>.Instance, withdrawalResponse);
		}
	}

	private AccountModule.OceanShineWithdrawalPageResponse EnsureWithdrawalResponse()
	{
		if (withdrawalResponse != null)
		{
			return withdrawalResponse;
		}
		withdrawalResponse = new AccountModule.OceanShineWithdrawalPageResponse
		{
			Os_Uso = ((BaseGameModule<AccountModule>.Instance != null) ? BaseGameModule<AccountModule>.Instance.Os_Current_Uso : null),
			Os_Wwf = new List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform>
			{
				new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform
				{
					Os_Cn = UIWithdrawalPanel.pagBankInfo,
					Os_Me = "Bank Transfer",
					Os_Mid = 1,
					Os_Mlt = 50.0,
					Os_Et = "Withdraw to your bank account",
					Os_Id = 1
				},
				new AccountModule.OceanShineWithdrawalPageResponse.WithdrawalPlatform
				{
					Os_Cn = UIWithdrawalPanel.pixInfo,
					Os_Me = "PIX / E-Wallet",
					Os_Mid = 2,
					Os_Mlt = 50.0,
					Os_Et = "Fast electronic transfer",
					Os_Id = 2
				}
			},
			Os_Wr = new List<AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio>()
		};
		return withdrawalResponse;
	}

	private static void LoadTablesForRuntimeCapture()
	{
		try
		{
			TableUtils.LoadLanguageTable().Forget();
			TableUtils.LoadTableConfigs().Forget();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[BlockTapCompleteRuntimeCapture] Table load warning: " + ex.GetBaseException().Message);
		}
	}

	private static void SeedSaveData()
	{
		if (SaveDataUtils.GameData != null)
		{
			SetMember(SaveDataUtils.GameData, "playerSelectedLv", 5);
			SetMember(SaveDataUtils.GameData, "playerUnlockedLv", 5);
			SetMember(SaveDataUtils.GameData, "playerpassLevel", 5);
			SetMember(SaveDataUtils.GameData, "playerPassLv", 6);
			SetMember(SaveDataUtils.GameData, "userLookDailyAdCount", 3);
			SetMember(SaveDataUtils.GameData, "userLookDailyAdCountMax", 10);
			SetMember(SaveDataUtils.GameData, "fakeWithdrawPanelFirstWithdraw", false);
			SetMember(SaveDataUtils.GameData, "customTutorialEnd", true);
			SaveDataUtils.GameData.chatInfos.Clear();
		}
		if (SaveDataUtils.TaskData != null)
		{
			SaveDataUtils.TaskData.taskStartDay = (int)TimeUtil.GetCurrentTime(ETimeUnit.DAYS);
		}
		if (SaveDataUtils.ItemData != null)
		{
			SetItemCount(E_ItemType.Gold, 72f);
			SetItemCount(E_ItemType.Dollar, 46.6f);
			SetItemCount(E_ItemType.GameProp_Back, 1f);
			SetItemCount(E_ItemType.GameProp_ClearArea, 1f);
			SetItemCount(E_ItemType.GameProp_Refresh, 1f);
		}
	}

	private static void SetMember(object target, string name, object value)
	{
		if (target == null)
		{
			return;
		}
		Type type = target.GetType();
		FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(target, ConvertValue(value, field.FieldType));
			return;
		}
		PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && property.CanWrite)
		{
			property.SetValue(target, ConvertValue(value, property.PropertyType));
		}
	}

	private static object ConvertValue(object value, Type targetType)
	{
		if (value == null || targetType.IsInstanceOfType(value))
		{
			return value;
		}
		return Convert.ChangeType(value, targetType);
	}

	private static void SafeOpen(UIPageBase page, params object[] args)
	{
		if (page == null)
		{
			return;
		}
		try
		{
			MethodInfo methodInfo = page.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((MethodInfo candidate) => candidate.Name == "OpenPage_CallByFramework" && candidate.GetParameters().Length == args.Length);
			if (methodInfo == null)
			{
				Debug.LogWarning("[BlockTapCompleteRuntimeCapture] No OpenPage method for " + page.GetType().Name + " with " + args.Length + " args.");
			}
			else
			{
				methodInfo.Invoke(page, args);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[BlockTapCompleteRuntimeCapture] Open warning for " + page.GetType().Name + ": " + ex.GetBaseException().Message);
		}
	}

	private static void InvokePrivate(object target, string methodName, params object[] args)
	{
		try
		{
			target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault((MethodInfo candidate) => candidate.Name == methodName && candidate.GetParameters().Length == args.Length)?.Invoke(target, args);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[BlockTapCompleteRuntimeCapture] " + methodName + " warning: " + ex.GetBaseException().Message);
		}
	}

	private static IEnumerator LoadPanel(string address, Canvas targetCanvas, string objectName, Action<GameObject> completed)
	{
		GameObject panelPrefab = null;
#if UNITY_EDITOR
		// PlayMode visual validation must render the prefab that is currently in
		// the project. Loading the address here can return an older built bundle,
		// which makes the comparison screenshot ignore recent prefab/art changes.
		if (string.Equals(address, "UIPanel/RealWithdrawPanel", StringComparison.OrdinalIgnoreCase))
		{
			panelPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
				"Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab");
			if (panelPrefab == null)
			{
				Debug.LogError("[BlockTapCompleteRuntimeCapture] Failed to load the current RealWithdrawPanel prefab directly.");
				completed(null);
				yield break;
			}
		}
		else if (string.Equals(address, "UIPanel/SlotFAQPanel", StringComparison.OrdinalIgnoreCase))
		{
			panelPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
				"Assets/BizzaWZ/Real/UI/SlotsPanel/SlotFQAPanel/SlotFQAPanel.prefab");
			if (panelPrefab == null)
			{
				Debug.LogError("[BlockTapCompleteRuntimeCapture] Failed to load the current SlotFAQPanel prefab directly.");
				completed(null);
				yield break;
			}
		}
#endif
		if (panelPrefab != null)
		{
			GameObject currentPrefabInstance;
			try
			{
				currentPrefabInstance = UnityEngine.Object.Instantiate(panelPrefab, targetCanvas.transform, worldPositionStays: false);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				completed(null);
				yield break;
			}
			PrepareLoadedPanel(currentPrefabInstance, objectName);
			completed(currentPrefabInstance);
			yield break;
		}

		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
		yield return handle;
		if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
		{
			Debug.LogError("[BlockTapCompleteRuntimeCapture] Failed to load " + address);
			completed(null);
			yield break;
		}
		GameObject gameObject;
		try
		{
			gameObject = UnityEngine.Object.Instantiate(handle.Result, targetCanvas.transform, worldPositionStays: false);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			completed(null);
			yield break;
		}
		PrepareLoadedPanel(gameObject, objectName);
		completed(gameObject);
	}

	private static void PrepareLoadedPanel(GameObject gameObject, string objectName)
	{
		gameObject.name = objectName;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		if (component != null)
		{
			component.localScale = Vector3.one;
			component.localRotation = Quaternion.identity;
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
			component.SetAsLastSibling();
		}
	}

	private static Canvas FindTargetCanvas()
	{
		Canvas result = null;
		Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
		foreach (Canvas canvas in array)
		{
			if (canvas.isRootCanvas)
			{
				result = canvas;
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					return canvas;
				}
			}
		}
		return result;
	}

	private static void InitializeAllSaveData()
	{
		FieldInfo[] fields = typeof(SaveDataUtils).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			Type fieldType = fieldInfo.FieldType;
			if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(DataStrategy<>))
			{
				continue;
			}
			try
			{
				object value = fieldInfo.GetValue(null);
				if (value != null)
				{
					PropertyInfo property = fieldType.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
					if (!(property == null) && property.GetValue(value) == null)
					{
						Type obj = fieldType.GetGenericArguments()[0];
						object obj2 = Activator.CreateInstance(obj);
						obj.GetMethod("InitData", BindingFlags.Instance | BindingFlags.Public)?.Invoke(obj2, null);
						fieldType.GetField("<Data>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(value, obj2);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[BlockTapCompleteRuntimeCapture] Save data init skipped for " + fieldInfo.Name + ": " + ex.GetBaseException().Message);
			}
		}
	}

	private static void SetItemCount(E_ItemType itemType, float count)
	{
		SaveDataUtils.ItemData.itemMap[itemType] = new ItemEntry
		{
			Type = itemType,
			Count = count
		};
	}

	private static string GetArgument(string[] args, string name)
	{
		for (int i = 0; i + 1 < args.Length; i++)
		{
			if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
			{
				return args[i + 1];
			}
		}
		return null;
	}

	private static void DumpUi(string path)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Path\tActive\tAnchoredPosition\tSizeDelta\tSprite\tText");
		RectTransform[] array = UnityEngine.Object.FindObjectsOfType<RectTransform>(includeInactive: true);
		foreach (RectTransform rectTransform in array)
		{
			Image component = rectTransform.GetComponent<Image>();
			TMP_Text component2 = rectTransform.GetComponent<TMP_Text>();
			if (!(component == null) || !(component2 == null))
			{
				stringBuilder.Append(GetPath(rectTransform.transform)).Append('\t').Append(rectTransform.gameObject.activeInHierarchy)
					.Append('\t')
					.Append(rectTransform.anchoredPosition)
					.Append('\t')
					.Append(rectTransform.sizeDelta)
					.Append('\t')
					.Append((component != null && component.sprite != null) ? component.sprite.name : string.Empty)
					.Append('\t')
					.Append((component2 != null) ? component2.text.Replace("\t", " ").Replace("\n", " ") : string.Empty)
					.AppendLine();
			}
		}
		File.WriteAllText(path, stringBuilder.ToString());
	}

	private static string GetPath(Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}
}
}
