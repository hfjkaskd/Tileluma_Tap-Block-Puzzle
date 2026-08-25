#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class OneToOneVisualRestorer
{
    // Visual QA refresh: approved royal-blue, gold, cream and deep-yellow panel palette.
    private const string ServiceSelectPrefab = "Assets/BizzaWZ/Real/UI/ServiceSelectPanel/ServiceSelectPanel.prefab";
    private const string ServicePanelPrefab = "Assets/BizzaWZ/Real/UI/ServicePanel/ServicePanel.prefab";
    private const string RealGamePanelPrefab = "Assets/BizzaWZ/Real/UI/GamePanel/RealGamePanel.prefab";
    private const string GetRewardPanelPrefab = "Assets/BizzaWZ/Real/UI/GetRewardPanel/GetRewardPanel.prefab";
    private const string LosePanelPrefab = "Assets/BizzaGame/LosePanel/LosePanel.prefab";
    private const string SlotFaqPanelPrefab = "Assets/BizzaWZ/Real/UI/SlotsPanel/SlotFQAPanel/SlotFQAPanel.prefab";
    private const string SlotPanelPrefab = "Assets/BizzaWZ/Real/UI/SlotsPanel/SlotPanel/SlotPanel.prefab";
    private const string DailyMissionPanelPrefab = "Assets/BizzaWZ/Real/UI/DailyMissionPanel/DailyMissionPanel.prefab";
    private const string StarRatingPanelPrefab = "Assets/BizzaWZ/Real/UI/StarRatingPopup/StarRatingPopup.prefab";
    private const string WithdrawalConfirmPanelPrefab = "Assets/BizzaWZ/Real/UI/UIWithdrawalConfirmPanel/InfoConfirm.prefab";
    private const string BadgePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/ServicePanel/QuickReply_NumberBadge.png";
    private const string CardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/bg_InPanel.png";
    private const string ArrowPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/Icon_Back.png";
    private const string CommonFramePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Common_Frame.png";
    private const string CommonTitlePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Common_Title.png";
    private const string GreenButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Btn_Green.png";
    private const string ClaimButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/GamePanel/Btn_Currency.png";
    private const string PrimaryButtonPath = "Assets/GamePlayProject/Art/J_JuNei/XCXX_JN_BG_2.png";
    private const string SimpleGreenButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Btn_Normael.png";
    private const string BombIconPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Prop/Icon_Prop_2.png";
    private const string SwapIconPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Prop/Icon_Prop_1.png";
    private const string SettingIconPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/GamePanel/Icon_Setting.png";
    private const string GameplayBackgroundPath = "Assets/GamePlayProject/Art/J_JuNei/XCXX_JN_BG_13.png";
    private const string RewardStarPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/StarRatingPopup/light.png";
    private const string CountBadgePath = "Assets/GamePlayProject/Art/J_JuNei/XCXX_JN_BG_15.png";
    private const string AmountCardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/bg_AmountPanel_BlueDeepYellow.png";
    private const string RewardValuePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/GetRewardPanel/Bg_RewardMax.png";
    private const string SecondaryRibbonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_TitleRibbon.png";
    private const string SecondaryCrestPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_Crest.png";
    private const string SecondaryBlueButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_BlueButton_v1.png";
    private const string BlueButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SettingPanel/Bg_PauseContinue.png";
    private const string SettingsGreenButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SettingPanel/Bg_PauseBack.png";
    private const string DailyTaskCardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/DailyTask/Task_Elementbg.png";
    private const string DailyTaskCardTargetV2Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/DailyTask/Task_Elementbg_Target_v2.png";
    private const string SettingsGoldCirclePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/GamePanel/Bg_PropAddTip.png";
    private const string SettingsMusicTargetPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Settings_Music_Target.png";
    private const string SettingsSoundTargetPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Settings_Sound_Target.png";
    private const string SettingsVibrateTargetPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Settings_Vibrate_Target.png";
    private const string ButtonLabelMaterialPath = "Assets/GamePlayProject/Fonts/Materials/MainFont_Simple_Outline_Black.mat";
    private const string BrownTitleMaterialPath = "Assets/GamePlayProject/Fonts/Materials/MainFont_Simple_Outline_Brown.mat";
    private const string LightBrownTitleMaterialPath = "Assets/GamePlayProject/Fonts/Materials/MainFont_Simple_Outline_LightBrown.mat";
    private const string DailyTasksCloseTargetPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/DailyTasks_Close_Target.png";
    private const string ItemCloseTargetPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Item_Close_Target.png";
    private const string SecondaryPanelFramePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_Frame_RoyalBlueGold_v3.png";
    private const string SecondaryContentCardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_ContentCard_Gold_v2.png";
    private const string SecondaryContentCardTargetV4Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_ContentCard_Gold_v4_Softened.png";
    private const string SecondaryContentCardSoftPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_ContentCard_Gold_v3_Soft.png";
    private const string SecondaryHeaderCompositeV2Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_HeaderComposite_v2.png";
    private const string SecondaryHeaderCompositeV3Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_HeaderComposite_v3.png";
    private const string SecondaryHeaderCompositeV4Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_HeaderComposite_v4.png";
    private const string SecondaryHeaderCompositeV5Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Secondary_HeaderComposite_v5.png";
    private const string SettingsBlueButtonTargetV2Path = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/Settings_BlueButton_Target_v2.png";
    private const string RewardLaurelGlowPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/LevelComplete_LaurelGlow_v1.png";
    private const string RewardLevelLeavesPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/LevelComplete_LevelLeaves_v1.png";
    private const string RewardFramePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/LevelComplete_Frame_v1.png";
    private const string RewardCoinPilePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SecondaryTheme/LevelComplete_CoinPile_v1.png";
    private const string RewardCashStackPath = "Assets/BizzaWZ/Real/GameAssets/WzTexture_Money/StackMoney_BR.png";
    private const string RealWithdrawFramePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/bg_WithdrawFrame.png";
    private const string RealWithdrawSurfacePath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/bg_RealWithdraw.png";
    private const string RealWithdrawTargetGradientPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/Withdraw_TargetGradient.png";
    private const string RealWithdrawTargetCardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/Withdraw_TargetCard.png";
    private const string RealWithdrawRuntimeCardPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/Withdraw_TargetCard_Runtime.png";
    private const string RealWithdrawTargetButtonPath = "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/Withdraw_TargetButton.png";
    private const string RealWithdrawApprovedReferencePath = "CurrentUI_OneToOne_Audit_20260821/26_ApprovedSecondaryReferences/14_RealWithdrawPanel.png";
    private const string PagBankChannelPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/Icons/Channel_2.png";
    private const string PixChannelPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/Icons/Channel_12.png";

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 01")]
    public static void ApplyBatch01()
    {
        ConfigureGeneratedSprite(BadgePath);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplyServiceSelectVisuals();
        ApplyServicePanelVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 01 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 02")]
    public static void ApplyBatch02()
    {
        ApplyRealGameAlignment();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 02 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 03")]
    public static void ApplyBatch03()
    {
        ApplyRealGameAlignment();
        ApplyGetRewardVisuals();
        ApplyLoseVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 03 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 04")]
    public static void ApplyBatch04()
    {
        ApplySlotFaqVisuals();
        ApplySlotPanelVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 04 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 05")]
    public static void ApplyBatch05()
    {
        ApplyGetRewardVisuals();
        ApplyDailyMissionButtonVisuals();
        ApplyStarRatingTitleVisuals();
        ApplyWithdrawalConfirmTitleVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 05 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 06")]
    public static void ApplyBatch06()
    {
        ApplyGetRewardVisuals();
        ApplyLoseVisuals();
        ApplySlotFaqVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 06 applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 07")]
    public static void ApplyBatch07()
    {
        ApplyGetRewardVisuals();
        ApplySlotFaqVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 07 applied successfully.");
    }

    public static void ApplyBatch08()
    {
        ConfigureGeneratedSprite(SecondaryRibbonPath);
        ConfigureGeneratedSprite(SecondaryCrestPath);
        ConfigureGeneratedSprite(SettingsMusicTargetPath);
        ConfigureGeneratedSprite(SettingsSoundTargetPath);
        ConfigureGeneratedSprite(SettingsVibrateTargetPath);
        ConfigureGeneratedSprite(DailyTasksCloseTargetPath);
        ConfigureGeneratedSprite(ItemCloseTargetPath);
        ConfigureGeneratedSprite(SecondaryPanelFramePath);
        ConfigureGeneratedSprite(SecondaryContentCardPath);
        ConfigureGeneratedSprite(RewardLaurelGlowPath);
        ConfigureGeneratedSprite(RewardLevelLeavesPath);
        ConfigureGeneratedSprite(RewardFramePath);
        ConfigureGeneratedSprite(RewardCoinPilePath);
        ConfigureGeneratedSprite(SecondaryBlueButtonPath);
        ConfigureGeneratedSprite(DailyTaskCardPath);
        ConfigureSpriteBorder(RewardFramePath, new Vector4(224f, 170f, 196f, 164f));
        ConfigureSpriteBorder(SecondaryPanelFramePath, new Vector4(125f, 125f, 125f, 125f));
        ConfigureSpriteBorder(DailyTaskCardPath, new Vector4(42f, 42f, 42f, 42f));
        ConfigureSpriteBorder(RealWithdrawFramePath, new Vector4(110f, 110f, 110f, 110f));
        ConfigureSpriteBorder(PrimaryButtonPath, new Vector4(42f, 30f, 42f, 30f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Item");
        ApplySecondaryModalTheme("Assets/BizzaWZ/MenuSystem/Common/CommonConfirmTipsPanel/CommonConfirmTipsPanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Notice");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/DailyMissionPanel/DailyMissionPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "Daily Mission");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/DailyWithdrawPanel/DailyWithdrawPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "Withdraw");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/ExchangeRatePanel/ExchangeRatePanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "EXCHANGE RATE");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/FAQPanel/FAQPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "FAQ");
        ApplySecondaryModalTheme(GetRewardPanelPrefab, "BG", "BG/ApprovedTitle", "LEVEL COMPLETE");
        ApplySecondaryModalTheme(LosePanelPrefab, "Window/Window", "Window/Window/ApprovedTitle", "TRY AGAIN");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/NewbieGiftPage/NewbieGiftPage.prefab", "Panel/BG (2)", "Panel/BG (2)/Text (TMP)", "New Player Reward");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Settings");
        ApplySecondaryModalTheme(ServiceSelectPrefab, "Content/bg_Content", "Content/bg_Content/Text (TMP)", "Quick Reply");
        ApplySecondaryModalTheme(SlotFaqPanelPrefab, "Content/bg2 (1)", "Content/ApprovedTitle", "SLOT REWARDS");
        ApplySecondaryModalTheme(SlotPanelPrefab, "Content/SlotMachineGroup/Buttom", "Content/ApprovedTitle", "BONUS SPIN");
        ApplySecondaryModalTheme(StarRatingPanelPrefab, "Content", "Content/Title", "RATE US");
        ApplySecondaryModalTheme("Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab", "BG/BG", "BG/BG/Text (TMP)", "DAILY TASKS");
        ApplySecondaryModalTheme(WithdrawalConfirmPanelPrefab, "BG", "Title/Text (TMP)", "CONFIRM WITHDRAWAL");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/WithdrawFillPanel/WithdrawFillPanel.prefab", "Root/FillRoot/BG", "Root/FillRoot/Title", "ACCOUNT DETAILS");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/WithdrawHistory/WithdrawHistory.prefab", "BG (2)", "BG (2)/Text (TMP)", "TRANSACTION HISTORY");

        ApplySettingsMeasuredCalibration();
        ApplyDailyTasksMeasuredCalibration();
        ApplyItemMeasuredCalibration();
        ApplyGetRewardVisuals();

        AdjustSecondaryHeaderCover(ServiceSelectPrefab, "Content/bg_Content", 40f, 250f, 118f);
        AdjustSecondaryHeaderCover("Assets/BizzaWZ/Real/UI/WithdrawFillPanel/WithdrawFillPanel.prefab", "Root/FillRoot/BG", 40f, 310f, 178f);
        SetPrefabObjectActive(SlotFaqPanelPrefab, "Content/bg2", false);

        RemoveSecondaryThemeChildren("Assets/BizzaWZ/Real/UI/FakeWithdrawPanel/FakeWithdrawPanel.prefab", "ButtomGroup");
        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/FakeWithdrawPanel/FakeWithdrawPanel.prefab", "ButtomGroup/bg", "ButtomGroup/Title", "Withdraw");
        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab", "Title", "Title/Title", "Sacar");
        ApplyRealWithdrawMeasuredCalibration();
        ApplyWithdrawLevelPaletteCalibration();
        ApplySecondaryFullPageHeader(ServicePanelPrefab, "Title", "Title/Title", "SUPPORT");
        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/WithdrawDanPanel/WithdrawDanPanel.prefab", "Title", "Title/Text", "My Tier");

        // Re-run the common button pass after every measured layout pass. Some
        // legacy buttons are larger than the shared filter before calibration,
        // then become eligible once their approved size has been applied.
        ApplyApprovedButtonThemeToPrefab("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab");
        ApplyApprovedButtonThemeToPrefab(LosePanelPrefab);
        ApplyApprovedButtonThemeToPrefab("Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab");
        ApplySettingsSpecificButtonTheme();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 08 approved secondary theme applied successfully.");
    }

    public static void ApplyBatch09StructureLocked()
    {
        // The production prefabs are restored from the pre-reskin backup before
        // this method is invoked. Business hierarchy, controller references and
        // button hitboxes stay intact; only the Daily Tasks presentation rects
        // measured from the current target comparison are calibrated below.
        ConfigureGeneratedSprite(SecondaryRibbonPath);
        ConfigureGeneratedSprite(SecondaryCrestPath);
        ConfigureGeneratedSprite(SecondaryPanelFramePath);
        ConfigureGeneratedSprite(SecondaryContentCardPath);
        ConfigureGeneratedSprite(SecondaryBlueButtonPath);
        ConfigureGeneratedSprite(DailyTaskCardPath);
        ConfigureGeneratedSprite(SettingsMusicTargetPath);
        ConfigureGeneratedSprite(SettingsSoundTargetPath);
        ConfigureGeneratedSprite(SettingsVibrateTargetPath);
        ConfigureGeneratedSprite(ItemCloseTargetPath);
        ConfigureSpriteBorder(SecondaryPanelFramePath, new Vector4(125f, 125f, 125f, 125f));
        ConfigureSpriteBorder(DailyTaskCardPath, new Vector4(42f, 42f, 42f, 42f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Item");
        ApplySecondaryModalTheme("Assets/BizzaWZ/MenuSystem/Common/CommonConfirmTipsPanel/CommonConfirmTipsPanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Notice");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/DailyMissionPanel/DailyMissionPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "Daily Mission");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/DailyWithdrawPanel/DailyWithdrawPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "Withdraw");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/ExchangeRatePanel/ExchangeRatePanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "EXCHANGE RATE");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/FAQPanel/FAQPanel.prefab", "BG (2)", "BG (2)/Text (TMP)", "FAQ");
        ApplySecondaryModalTheme(GetRewardPanelPrefab, "BG", "BG/ApprovedTitle", "LEVEL COMPLETE");
        ApplySecondaryModalTheme(LosePanelPrefab, "Window/Window", "Window/Window/ApprovedTitle", "TRY AGAIN");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/NewbieGiftPage/NewbieGiftPage.prefab", "Panel/BG (2)", "Panel/BG (2)/Text (TMP)", "New Player Reward");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)", "BG (1)/Text (TMP)", "Settings");
        ApplySecondaryModalTheme(ServiceSelectPrefab, "Content/bg_Content", "Content/bg_Content/Text (TMP)", "Quick Reply");
        ApplySecondaryModalTheme(SlotFaqPanelPrefab, "Content/bg2 (1)", "Content/ApprovedTitle", "SLOT REWARDS");
        ApplySecondaryModalTheme(SlotPanelPrefab, "Content/SlotMachineGroup/Buttom", "Content/ApprovedTitle", "BONUS SPIN");
        ApplySecondaryModalTheme(StarRatingPanelPrefab, "Content", "Content/Title", "RATE US");
        ApplySecondaryModalTheme("Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab", "BG/BG", "BG/BG/Text (TMP)", "DAILY TASKS");
        ApplySecondaryModalTheme(WithdrawalConfirmPanelPrefab, "BG", "Title/Text (TMP)", "CONFIRM WITHDRAWAL");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/WithdrawFillPanel/WithdrawFillPanel.prefab", "Root/FillRoot/BG", "Root/FillRoot/Title", "ACCOUNT DETAILS");
        ApplySecondaryModalTheme("Assets/BizzaWZ/Real/UI/WithdrawHistory/WithdrawHistory.prefab", "BG (2)", "BG (2)/Text (TMP)", "TRANSACTION HISTORY");

        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/FakeWithdrawPanel/FakeWithdrawPanel.prefab", "ButtomGroup/bg", "ButtomGroup/Title", "Withdraw");
        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab", "Title", "Title/Title", "Sacar");
        ApplyRealWithdrawContentOwnedHeader();
        ApplySecondaryFullPageHeader(ServicePanelPrefab, "Title", "Title/Title", "SUPPORT");
        ApplySecondaryFullPageHeader("Assets/BizzaWZ/Real/UI/WithdrawDanPanel/WithdrawDanPanel.prefab", "Title", "Title/Text", "My Tier");

        // These are sprite-only swaps on the existing authored rectangles.
        ApplyStructureLockedCard("Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab", "BG (1)", "Image (2)");
        ApplyStructureLockedCard("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)", "Image (2)");
        ApplyStructureLockedImage("Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab", "Content/CloseBtn/Image (3)", ItemCloseTargetPath);
        ApplyStructureLockedImage("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/MusicGroup/Button/MusicOn/Image (2)", SettingsMusicTargetPath);
        ApplyStructureLockedImageTint("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/MusicGroup/Button/MusicOff/Image (2)", SettingsMusicTargetPath, new Color32(150, 160, 175, 255));
        ApplyStructureLockedImage("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/SoundGroup/Button (1)/SoundOn/Image (2)", SettingsSoundTargetPath);
        ApplyStructureLockedImageTint("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/SoundGroup/Button (1)/SoundOff/Image (2)", SettingsSoundTargetPath, new Color32(150, 160, 175, 255));
        ApplyStructureLockedImage("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/LibGroup/Button (2)/LibOn/Image (2)", SettingsVibrateTargetPath);
        ApplyStructureLockedImageTint("Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab", "BG (1)/LibGroup/Button (2)/LIboff/Image (2)", SettingsVibrateTargetPath, new Color32(150, 160, 175, 255));
        ApplySettingsSpecificButtonTheme();
        ApplyDailyTasksStructureLockedCalibration();
        HideAddPropAdVisuals();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 09 structure-locked secondary theme applied successfully.");
    }

    [MenuItem("Tools/Block Tap/One-to-One Restore/Apply Batch 10 Original-Node Calibration")]
    public static void ApplyBatch10OriginalNodeCalibration()
    {
        ConfigureGeneratedSprite(SettingsMusicTargetPath);
        ConfigureGeneratedSprite(SettingsSoundTargetPath);
        ConfigureGeneratedSprite(SettingsVibrateTargetPath);
        ConfigureGeneratedSprite(DailyTasksCloseTargetPath);
        ConfigureGeneratedSprite(ItemCloseTargetPath);
        ConfigureGeneratedSprite(SecondaryContentCardPath);
        ConfigureGeneratedSprite(SecondaryBlueButtonPath);
        ConfigureSpriteBorder(SecondaryContentCardPath, new Vector4(64f, 64f, 64f, 64f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsOriginalNodeCalibration();
        ApplyDailyTasksOriginalNodeCalibration();
        ApplyItemOriginalNodeCalibration();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 10 original-node calibration applied successfully.");
    }

    [MenuItem("Tools/Block Tap/Apply Batch 11 Target-Current Correction")]
    public static void ApplyBatch11TargetCurrentCorrection()
    {
        ConfigureGeneratedSprite(SecondaryPanelFramePath);
        ConfigureGeneratedSprite(SecondaryHeaderCompositeV2Path);
        ConfigureGeneratedSprite(SecondaryRibbonPath);
        ConfigureGeneratedSprite(SecondaryCrestPath);
        ConfigureGeneratedSprite(SettingsBlueButtonTargetV2Path);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsTargetCurrentCorrection();
        ApplyDailyTasksTargetCurrentCorrection();
        ApplyItemTargetCurrentCorrection();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 11 target-current correction applied successfully.");
    }

    private static void ApplySettingsTargetCurrentCorrection()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            ConfigureImage(frame.GetComponent<Image>(), RequireSprite(SecondaryPanelFramePath), Image.Type.Simple);

            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2");
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v3");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildByActiveState(frame, "Image (2)", false);
            }
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.gameObject.SetActive(true);
            header.anchoredPosition = new Vector2(0f, 520f);
            header.sizeDelta = new Vector2(790f, 330f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV2Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 475f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 52f);

            ConfigureOriginalButton(frame, "GamePauseGroup/BackButton", RequireSprite(GreenButtonPath), new Vector2(410f, 190f));
            ConfigureOriginalButton(frame, "GamePauseGroup/ContinueBtn", RequireSprite(SettingsBlueButtonTargetV2Path), new Vector2(410f, 190f));

            RectTransform closeIcon = frame.Find("CloseButton/Image (1)") as RectTransform;
            if (closeIcon != null)
            {
                closeIcon.sizeDelta = new Vector2(130f, 128f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            RectTransform close = frame.Find("CloseButton") as RectTransform;
            if (close != null)
            {
                close.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksTargetCurrentCorrection()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.sizeDelta = new Vector2(1076f, 1700f);
            ConfigureImage(frame.GetComponent<Image>(), RequireSprite(SecondaryPanelFramePath), Image.Type.Simple);

            RectTransform ribbon = RequireRect(frame.Find("Image (6)"), prefabPath + "/BG/BG/Image (6)");
            ribbon.gameObject.SetActive(true);
            ribbon.anchoredPosition = new Vector2(0f, 735f);
            ribbon.sizeDelta = new Vector2(920f, 300f);
            ConfigureImage(ribbon.GetComponent<Image>(), RequireSprite(SecondaryRibbonPath), Image.Type.Simple);

            RectTransform crest = RequireRect(frame.Find("Image (3)"), prefabPath + "/BG/BG/Image (3)");
            crest.gameObject.SetActive(true);
            crest.anchoredPosition = new Vector2(0f, 880f);
            crest.sizeDelta = new Vector2(560f, 280f);
            ConfigureImage(crest.GetComponent<Image>(), RequireSprite(SecondaryCrestPath), Image.Type.Simple);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 725f);
            title.localScale = Vector3.one;
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 65f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 235f);
            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(833f, 240f);
            }

            ribbon.SetAsLastSibling();
            crest.SetAsLastSibling();
            title.SetAsLastSibling();

            ApplyDailyTaskElementMeasuredCalibration();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemTargetCurrentCorrection()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1080f, 1120f);
            ConfigureImage(frame.GetComponent<Image>(), RequireSprite(SecondaryPanelFramePath), Image.Type.Simple);

            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2");
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v3");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            }
            if (headerTransform == null)
            {
                headerTransform = FindDirectChildByActiveState(frame, "Image (2)", false);
            }
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.gameObject.SetActive(true);
            header.anchoredPosition = new Vector2(0f, 506f);
            header.sizeDelta = new Vector2(860f, 330f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV2Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 438f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.sizeDelta = new Vector2(380f, 380f);

            RectTransform adButton = root.transform.Find("Content/BtnGroup/ButtonAnim/AdBtn") as RectTransform;
            if (adButton != null)
            {
                adButton.gameObject.SetActive(false);
            }

            RectTransform reward = RequireRect(root.transform.Find("Content/BtnGroup/ButtonAnim/Pop"), prefabPath + "/Content/BtnGroup/ButtonAnim/Pop");
            reward.anchoredPosition = new Vector2(248f, 175f);
            reward.localScale = new Vector3(0.85f, 0.85f, 1f);

            RectTransform closeIcon = root.transform.Find("Content/CloseBtn/Image (3)") as RectTransform;
            if (closeIcon != null)
            {
                closeIcon.sizeDelta = new Vector2(120f, 120f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 12 Pixel Geometry and Soft Cards")]
    public static void ApplyBatch12PixelGeometryAndSoftCards()
    {
        ApplyBatch11TargetCurrentCorrection();
        ConfigureGeneratedSprite(SecondaryContentCardSoftPath);
        ConfigureSpriteBorder(SecondaryContentCardSoftPath, new Vector4(64f, 64f, 64f, 64f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsBatch12Geometry();
        ApplyDailyTasksBatch12Geometry();
        ApplyItemBatch12Geometry();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 12 pixel geometry and soft-card correction applied successfully.");
    }

    private static void ApplySettingsBatch12Geometry()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1060f, 1150f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 40f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 535f);
            header.sizeDelta = new Vector2(790f, 390f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 464f);

            RectTransform contentCard = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg"),
                prefabPath + "/BG (1)/Image (2) [settings content card]");
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(SecondaryContentCardSoftPath), Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = Color.white;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch12Geometry()
    {
        const string pagePath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject page = PrefabUtility.LoadPrefabContents(pagePath);
        try
        {
            RectTransform frame = RequireRect(page.transform.Find("BG/BG"), pagePath + "/BG/BG");
            frame.sizeDelta = new Vector2(1048f, 1650f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -80f);

            RectTransform ribbon = RequireRect(frame.Find("Image (6)"), pagePath + "/BG/BG/Image (6)");
            ribbon.anchoredPosition = new Vector2(0f, 705f);
            ribbon.sizeDelta = new Vector2(1000f, 360f);

            RectTransform crest = RequireRect(frame.Find("Image (3)"), pagePath + "/BG/BG/Image (3)");
            crest.anchoredPosition = new Vector2(0f, 850f);
            crest.sizeDelta = new Vector2(520f, 260f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), pagePath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 710f);

            PrefabUtility.SaveAsPrefabAsset(page, pagePath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(page);
        }

        const string elementPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab";
        GameObject element = PrefabUtility.LoadPrefabContents(elementPath);
        try
        {
            Image card = element.GetComponent<Image>();
            ConfigureImage(card, RequireSprite(SecondaryContentCardSoftPath), Image.Type.Sliced);
            card.color = Color.white;
            PrefabUtility.SaveAsPrefabAsset(element, elementPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(element);
        }
    }

    private static void ApplyItemBatch12Geometry()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1025f, 1120f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 478f);
            header.sizeDelta = new Vector2(820f, 330f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 410f);

            RectTransform contentCard = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v2"),
                prefabPath + "/BG (1)/Image (2) [item content card]");
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(SecondaryContentCardSoftPath), Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = Color.white;

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.sizeDelta = new Vector2(360f, 360f);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 13 Geometry Recovery and Header Match")]
    public static void ApplyBatch13GeometryRecoveryAndHeaderMatch()
    {
        // Re-establish the last stable original-node setup first. Batch 12
        // intentionally experimented with a softer sliced card, but its border
        // metrics compressed several runtime cards. Batch 13 keeps the improved
        // outer-frame geometry while restoring the source card sprites.
        ApplyBatch11TargetCurrentCorrection();
        ConfigureGeneratedSprite(SecondaryHeaderCompositeV3Path);
        ConfigureGeneratedSprite(DailyTaskCardPath);
        ConfigureGeneratedSprite(SecondaryContentCardPath);
        ConfigureSpriteBorder(DailyTaskCardPath, new Vector4(42f, 42f, 42f, 42f));
        ConfigureSpriteBorder(SecondaryContentCardPath, new Vector4(64f, 64f, 64f, 64f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsBatch13Geometry();
        ApplyDailyTasksBatch13Geometry();
        ApplyItemBatch13Geometry();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 13 geometry recovery and header match applied successfully.");
    }

    private static void ApplySettingsBatch13Geometry()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1060f, 1150f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 75f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 535f);
            header.sizeDelta = new Vector2(790f, 390f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV3Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 493f);

            Transform contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v3_Soft");
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg");
            }
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg_Target_v2");
            }
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildByApproximateSize(frame, "Image (2)", new Vector2(875f, 346f));
            }
            RectTransform contentCard = RequireRect(contentTransform, prefabPath + "/BG (1)/Image (2) [settings content card]");
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(DailyTaskCardPath), Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = Color.white;

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            RectTransform close = frame.Find("CloseButton") as RectTransform;
            if (close != null)
            {
                close.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch13Geometry()
    {
        const string pagePath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject page = PrefabUtility.LoadPrefabContents(pagePath);
        try
        {
            RectTransform frame = RequireRect(page.transform.Find("BG/BG"), pagePath + "/BG/BG");
            frame.sizeDelta = new Vector2(1048f, 1650f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -80f);

            RectTransform ribbon = RequireRect(frame.Find("Image (6)"), pagePath + "/BG/BG/Image (6)");
            ribbon.gameObject.SetActive(true);
            ribbon.anchoredPosition = new Vector2(0f, 735f);
            ribbon.sizeDelta = new Vector2(920f, 300f);
            ConfigureImage(ribbon.GetComponent<Image>(), RequireSprite(SecondaryRibbonPath), Image.Type.Simple);
            ribbon.GetComponent<Image>().preserveAspect = false;

            RectTransform crest = RequireRect(frame.Find("Image (3)"), pagePath + "/BG/BG/Image (3)");
            crest.gameObject.SetActive(true);
            crest.anchoredPosition = new Vector2(0f, 880f);
            crest.sizeDelta = new Vector2(560f, 280f);
            ConfigureImage(crest.GetComponent<Image>(), RequireSprite(SecondaryCrestPath), Image.Type.Simple);
            crest.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), pagePath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 725f);

            ribbon.SetAsLastSibling();
            crest.SetAsLastSibling();
            title.SetAsLastSibling();

            PrefabUtility.SaveAsPrefabAsset(page, pagePath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(page);
        }

        const string elementPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab";
        GameObject element = PrefabUtility.LoadPrefabContents(elementPath);
        try
        {
            Image card = element.GetComponent<Image>();
            ConfigureImage(card, RequireSprite(DailyTaskCardPath), Image.Type.Sliced);
            card.color = Color.white;
            PrefabUtility.SaveAsPrefabAsset(element, elementPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(element);
        }

        ApplyDailyTaskElementMeasuredCalibration();
    }

    private static void ApplyItemBatch13Geometry()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1025f, 1120f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v2"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 478f);
            header.sizeDelta = new Vector2(820f, 330f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV3Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 425f);

            Transform contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v3_Soft");
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v2");
            }
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v4_Softened");
            }
            if (contentTransform == null)
            {
                contentTransform = FindDirectChildByApproximateSize(frame, "Image (2)", new Vector2(860f, 500f));
            }
            RectTransform contentCard = RequireRect(contentTransform, prefabPath + "/BG (1)/Image (2) [item content card]");
            contentCard.sizeDelta = new Vector2(860f, 500f);
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(SecondaryContentCardPath), Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = new Color32(255, 242, 210, 255);

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.sizeDelta = new Vector2(350f, 350f);

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 14 Header, Border and Card Match")]
    public static void ApplyBatch14HeaderBorderAndCardMatch()
    {
        ApplyBatch13GeometryRecoveryAndHeaderMatch();
        ConfigureGeneratedSprite(SecondaryHeaderCompositeV4Path);
        ConfigureGeneratedSprite(DailyTaskCardTargetV2Path);
        ConfigureGeneratedSprite(SecondaryContentCardTargetV4Path);
        ConfigureGeneratedSprite(ItemCloseTargetPath);
        ConfigureSpriteBorder(DailyTaskCardTargetV2Path, new Vector4(24f, 24f, 24f, 24f));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsBatch14Match();
        ApplyDailyTasksBatch14Match();
        ApplyItemBatch14Match();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 14 header, border and card match applied successfully.");
    }

    private static void ApplySettingsBatch14Match()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v3")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 524f);
            header.sizeDelta = new Vector2(760f, 390f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV4Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 473f);

            Transform contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg_Target_v2")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v3_Soft")
                ?? FindDirectChildByApproximateSize(frame, "Image (2)", new Vector2(875f, 346f));
            RectTransform contentCard = RequireRect(
                contentTransform,
                prefabPath + "/BG (1)/Image (2) [settings content card]");
            contentCard.sizeDelta = new Vector2(875f, 346f);
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(DailyTaskCardTargetV2Path), Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = Color.white;

            RectTransform closeIcon = frame.Find("CloseButton/Image (1)") as RectTransform;
            if (closeIcon != null)
            {
                ConfigureImage(closeIcon.GetComponent<Image>(), RequireSprite(ItemCloseTargetPath), Image.Type.Simple);
                closeIcon.sizeDelta = new Vector2(138f, 138f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            RectTransform close = frame.Find("CloseButton") as RectTransform;
            if (close != null)
            {
                close.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch14Match()
    {
        const string pagePath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject page = PrefabUtility.LoadPrefabContents(pagePath);
        try
        {
            RectTransform frame = RequireRect(page.transform.Find("BG/BG"), pagePath + "/BG/BG");
            frame.sizeDelta = new Vector2(1076f, 1650f);

            RectTransform ribbon = RequireRect(frame.Find("Image (6)"), pagePath + "/BG/BG/Image (6)");
            ribbon.anchoredPosition = new Vector2(0f, 735f);
            ribbon.sizeDelta = new Vector2(920f, 300f);
            Image ribbonImage = ribbon.GetComponent<Image>();
            ConfigureImage(ribbonImage, RequireSprite(SecondaryRibbonPath), Image.Type.Simple);
            ribbonImage.color = Color.white;
            ribbonImage.preserveAspect = false;

            RectTransform crest = RequireRect(frame.Find("Image (3)"), pagePath + "/BG/BG/Image (3)");
            crest.anchoredPosition = new Vector2(0f, 865f);
            crest.sizeDelta = new Vector2(650f, 325f);
            Image crestImage = crest.GetComponent<Image>();
            ConfigureImage(crestImage, RequireSprite(SecondaryCrestPath), Image.Type.Simple);
            crestImage.color = Color.white;
            crestImage.preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), pagePath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 737f);

            RectTransform taskRoot = RequireRect(page.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), pagePath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(885f, 250f);
                layout.spacing = new Vector2(0f, 10f);
            }

            ribbon.SetAsLastSibling();
            crest.SetAsLastSibling();
            title.SetAsLastSibling();

            PrefabUtility.SaveAsPrefabAsset(page, pagePath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(page);
        }

        const string elementPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab";
        GameObject element = PrefabUtility.LoadPrefabContents(elementPath);
        try
        {
            Image card = element.GetComponent<Image>();
            ConfigureImage(card, RequireSprite(DailyTaskCardTargetV2Path), Image.Type.Sliced);
            card.color = Color.white;
            PrefabUtility.SaveAsPrefabAsset(element, elementPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(element);
        }

        ApplyDailyTaskElementMeasuredCalibration();
    }

    private static void ApplyItemBatch14Match()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v3")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV4Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 439f);

            Transform contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v2")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v4_Softened")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v3_Soft")
                ?? FindDirectChildByApproximateSize(frame, "Image (2)", new Vector2(860f, 500f));
            RectTransform contentCard = RequireRect(
                contentTransform,
                prefabPath + "/BG (1)/Image (2) [item content card]");
            contentCard.sizeDelta = new Vector2(860f, 520f);
            ConfigureImage(contentCard.GetComponent<Image>(), RequireSprite(SecondaryContentCardTargetV4Path), Image.Type.Simple);
            contentCard.GetComponent<Image>().color = Color.white;
            contentCard.GetComponent<Image>().preserveAspect = false;

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.sizeDelta = new Vector2(330f, 330f);

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 15 Current Pixel Correction")]
    public static void ApplyBatch15CurrentPixelCorrection()
    {
        // Batch 15 only adjusts artwork geometry measured from the current
        // PlayMode comparison. It intentionally keeps every source prefab node,
        // binding and interaction from the Batch 14 baseline.
        ApplyBatch14HeaderBorderAndCardMatch();
        ConfigureGeneratedSprite(SecondaryHeaderCompositeV5Path);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ApplySettingsBatch15Correction();
        ApplyDailyTasksBatch15Correction();
        ApplyItemBatch15Correction();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 15 current pixel correction applied successfully.");
    }

    [MenuItem("Tools/Block Tap/Apply Batch 16 Approved Effect Geometry")]
    public static void ApplyBatch16ApprovedEffectGeometry()
    {
        // Start from the verified Batch 15 asset assignments, then correct only
        // the remaining per-prefab geometry measured against the four approved
        // secondary-UI effect images. No gameplay hierarchy or bindings change.
        ApplyBatch15CurrentPixelCorrection();
        ApplySettingsBatch16Correction();
        ApplyDailyTasksBatch16Correction();
        ApplyItemBatch16Correction();
        // Batch 14 intentionally removes the older full-page decoration from
        // RealWithdraw. Recreate exactly one approved visual header before the
        // measured pass so the calibration is idempotent and cannot leave a
        // duplicated legacy title treatment behind.
        ApplySecondaryFullPageHeader(
            "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab",
            "Title",
            "Title/Title",
            "Sacar");
        ApplyRealWithdrawMeasuredCalibration();
        ApplyWithdrawLevelPaletteCalibration();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 16 approved-effect geometry applied successfully.");
    }

    [MenuItem("Tools/Block Tap/Apply Batch 17 Approved Pixel Match")]
    public static void ApplyBatch17ApprovedPixelMatch()
    {
        // Batch 17 is deliberately narrow. The Batch 16 comparison proved that
        // the four approved screens already share the correct source hierarchy;
        // only measured title/card geometry and withdrawal surface assignments
        // remain. Do not rescale any modal root or replace controller bindings.
        ApplyBatch16ApprovedEffectGeometry();
        ApplySettingsBatch17PixelMatch();
        ApplyDailyTasksBatch17PixelMatch();
        ApplyItemBatch17PixelMatch();
        ApplyRealWithdrawBatch17PixelMatch();
        ApplyWithdrawWayBatch17PixelMatch();
        ApplyWithdrawLevelBatch17PixelMatch();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 17 approved pixel match applied successfully.");
    }

    [MenuItem("Tools/Block Tap/Apply Batch 18 Structure Regression Match")]
    public static void ApplyBatch18StructureRegressionMatch()
    {
        // Batch 17 exposed several geometry regressions that were hidden by the
        // earlier art-only comparison. Keep the approved blue/gold assignments,
        // but restore the measured content footprint of the runtime prefabs.
        ApplyBatch17ApprovedPixelMatch();
        ApplySettingsBatch18StructureMatch();
        ApplyDailyTasksBatch18StructureMatch();
        ApplyItemBatch18StructureMatch();
        ApplyRealWithdrawBatch18StructureMatch();
        ApplyWithdrawLevelBatch18StructureMatch();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 18 structure regression match applied successfully.");
    }

    private static void ApplySettingsBatch18StructureMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1105f, 1150f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 501f);
            header.sizeDelta = new Vector2(815f, 350f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 466f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);

            ConfigureOriginalButton(frame, "GamePauseGroup/BackButton", RequireSprite(GreenButtonPath), new Vector2(415f, 200f));
            ConfigureOriginalButton(frame, "GamePauseGroup/ContinueBtn", RequireSprite(SettingsBlueButtonTargetV2Path), new Vector2(415f, 200f));

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch18StructureMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -67f);
            frame.sizeDelta = new Vector2(1190f, 1650f);

            RectTransform header = RequireRect(frame.Find("Image (3)"), prefabPath + "/BG/BG/Image (3)");
            header.anchoredPosition = new Vector2(0f, 759f);
            header.sizeDelta = new Vector2(980f, 500f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 723f);
            title.sizeDelta = new Vector2(800f, 160f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 90f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 226f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(815f, 250f);
                layout.spacing = new Vector2(0f, 10f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemBatch18StructureMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 103f);
            frame.sizeDelta = new Vector2(1025f, 1115f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 475f);
            header.sizeDelta = new Vector2(831f, 365f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 424f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 78f);

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawBatch18StructureMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            RectTransform crest = RequireRect(header.Find("ApprovedSecondaryCrest"), prefabPath + "/Title/ApprovedSecondaryCrest");
            crest.anchoredPosition = new Vector2(0f, 0f);

            TMP_Text approvedTitle = RequireRect(header.Find("ApprovedSecondaryTitle"), prefabPath + "/Title/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.anchoredPosition = new Vector2(0f, -55f);
            approvedTitle.fontSizeMax = 72f;

            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            withdrawFrame.anchoredPosition = new Vector2(withdrawFrame.anchoredPosition.x, -10f);
            withdrawFrame.sizeDelta = new Vector2(1068f, 1265f);

            RectTransform amountFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CoinInfoFrame"), prefabPath + "/CoinInfo/CoinInfoFrame");
            amountFrame.anchoredPosition = new Vector2(0f, -150f);
            amountFrame.sizeDelta = new Vector2(900f, 185f);
            Image amountImage = amountFrame.GetComponent<Image>();
            amountImage.enabled = true;
            ConfigureImage(amountImage, RequireSprite(SecondaryContentCardPath), Image.Type.Sliced);
            amountImage.color = Color.white;
            amountImage.preserveAspect = false;

            RectTransform balanceHint = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/BlanaceHint"), prefabPath + "/WithdrawInfo/BlanaceHint");
            balanceHint.anchoredPosition = new Vector2(balanceHint.anchoredPosition.x, 320f);

            RectTransform hint = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/_Hint"), prefabPath + "/WithdrawInfo/_Hint");
            hint.anchoredPosition = new Vector2(hint.anchoredPosition.x, 20f);

            RectTransform button = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn/Btn"), prefabPath + "/WithdrawInfo/WithdrawBtn/Btn");
            button.sizeDelta = new Vector2(490f, 188f);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                RectTransform methodRoot = controller.withdrawWayRoot as RectTransform;
                GridLayoutGroup methodLayout = methodRoot != null ? methodRoot.GetComponent<GridLayoutGroup>() : null;
                if (methodRoot != null)
                {
                    methodRoot.anchoredPosition = new Vector2(methodRoot.anchoredPosition.x - 7f, methodRoot.anchoredPosition.y);
                }
                if (methodLayout != null)
                {
                    methodLayout.cellSize = new Vector2(420f, 230f);
                    methodLayout.spacing = new Vector2(20f, methodLayout.spacing.y);
                }

                RectTransform levelRoot = controller.withdrawLevelRoot as RectTransform;
                GridLayoutGroup levelLayout = levelRoot != null ? levelRoot.GetComponent<GridLayoutGroup>() : null;
                if (levelRoot != null)
                {
                    levelRoot.anchoredPosition = new Vector2(-8f, -37f);
                }
                if (levelLayout != null)
                {
                    levelLayout.cellSize = new Vector2(412f, 218f);
                    levelLayout.spacing = new Vector2(10f, 1f);
                }
            }

            RectTransform completeHint = root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawLevel/ProgressInfo/CompleteHint") as RectTransform;
            if (completeHint != null)
            {
                completeHint.anchoredPosition = new Vector2(completeHint.anchoredPosition.x, -80f);
            }
            RectTransform progressHint = root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawLevel/ProgressInfo/progressHint") as RectTransform;
            if (progressHint != null)
            {
                progressHint.anchoredPosition = new Vector2(progressHint.anchoredPosition.x, -125f);
            }
            RectTransform progressBar = root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawLevel/ProgressInfo/Progress") as RectTransform;
            if (progressBar != null)
            {
                progressBar.anchoredPosition = new Vector2(progressBar.anchoredPosition.x, -20f);
            }

            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelBatch18StructureMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            foreach (string childName in new[] { "bg", "Shadow", "SelectShadow" })
            {
                RectTransform child = root.transform.Find(childName) as RectTransform;
                if (child != null)
                {
                    child.sizeDelta = new Vector2(840f, 440f);
                }
            }

            Image shadow = root.transform.Find("Shadow")?.GetComponent<Image>();
            if (shadow != null)
            {
                shadow.color = new Color32(160, 139, 108, 175);
            }
            Image selectedShadow = root.transform.Find("SelectShadow")?.GetComponent<Image>();
            if (selectedShadow != null)
            {
                selectedShadow.color = new Color32(188, 158, 106, 135);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 19 Measured Pixel Convergence")]
    public static void ApplyBatch19MeasuredPixelConvergence()
    {
        // Batch 19 is derived only from the archived Batch 18 PlayMode captures.
        // Keep the existing hierarchy and controller bindings; converge the
        // remaining measured rect, duplicate-background and spacing deltas.
        ApplyBatch18StructureRegressionMatch();
        ApplySettingsBatch19MeasuredMatch();
        ApplyDailyTasksBatch19MeasuredMatch();
        ApplyItemBatch19MeasuredMatch();
        ApplyRealWithdrawBatch19MeasuredMatch();
        ApplyWithdrawLevelBatch19MeasuredMatch();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 19 measured pixel convergence applied successfully.");
    }

    private static void ApplySettingsBatch19MeasuredMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1087f, 1150f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 501f);
            header.sizeDelta = new Vector2(765f, 350f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 466f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 60f);

            ConfigureOriginalButton(frame, "GamePauseGroup/BackButton", RequireSprite(GreenButtonPath), new Vector2(423f, 193f));
            ConfigureOriginalButton(frame, "GamePauseGroup/ContinueBtn", RequireSprite(SettingsBlueButtonTargetV2Path), new Vector2(423f, 193f));

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch19MeasuredMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -72f);
            frame.sizeDelta = new Vector2(1159f, 1650f);

            RectTransform header = RequireRect(frame.Find("Image (3)"), prefabPath + "/BG/BG/Image (3)");
            header.anchoredPosition = new Vector2(0f, 755f);
            header.sizeDelta = new Vector2(900f, 500f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 719f);
            title.sizeDelta = new Vector2(780f, 160f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 86f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(842f, 245f);
                layout.spacing = new Vector2(0f, 15f);
            }

            RectTransform closeIcon = root.transform.Find("CloseBtn/Image (2)") as RectTransform;
            if (closeIcon != null)
            {
                closeIcon.sizeDelta = new Vector2(135f, 135f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemBatch19MeasuredMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 108f);
            frame.sizeDelta = new Vector2(1025f, 1100f);

            RectTransform propIcon = root.transform.Find("Content/PropIcon") as RectTransform;
            if (propIcon != null)
            {
                propIcon.sizeDelta = new Vector2(340f, 340f);
            }

            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawBatch19MeasuredMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            TMP_Text approvedTitle = RequireRect(header.Find("ApprovedSecondaryTitle"), prefabPath + "/Title/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.anchoredPosition = new Vector2(0f, -75f);
            approvedTitle.fontSizeMax = 78f;

            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            withdrawFrame.sizeDelta = new Vector2(1068f, 1295f);

            RectTransform amountFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CoinInfoFrame"), prefabPath + "/CoinInfo/CoinInfoFrame");
            amountFrame.anchoredPosition = new Vector2(0f, -150f);
            amountFrame.sizeDelta = new Vector2(925f, 175f);
            ConfigureImage(amountFrame.GetComponent<Image>(), RequireSprite(SecondaryContentCardTargetV4Path), Image.Type.Sliced);

            // The amount label already sits over CoinInfoFrame. Its old Image was
            // a second cream/blue plate, producing the duplicate base seen in the
            // Batch 18 capture. Keep the label and disable only that duplicate art.
            RectTransform realCurrent = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/RealCurrent"), prefabPath + "/CoinInfo/RealCurrent");
            Image duplicateAmountPlate = realCurrent.GetComponent<Image>();
            if (duplicateAmountPlate != null)
            {
                duplicateAmountPlate.enabled = false;
            }

            RectTransform button = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn/Btn"), prefabPath + "/WithdrawInfo/WithdrawBtn/Btn");
            button.sizeDelta = new Vector2(485f, 178f);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                RectTransform levelRoot = controller.withdrawLevelRoot as RectTransform;
                if (levelRoot != null)
                {
                    levelRoot.anchoredPosition = new Vector2(-8f, -44f);
                }
                GridLayoutGroup levelLayout = levelRoot != null ? levelRoot.GetComponent<GridLayoutGroup>() : null;
                if (levelLayout != null)
                {
                    levelLayout.cellSize = new Vector2(402f, 222f);
                    levelLayout.spacing = new Vector2(30f, 3f);
                }
            }

            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelBatch19MeasuredMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            foreach (string childName in new[] { "bg", "Shadow", "SelectShadow" })
            {
                RectTransform child = root.transform.Find(childName) as RectTransform;
                if (child != null)
                {
                    child.sizeDelta = new Vector2(820f, 445f);
                }
            }

            Image shadow = root.transform.Find("Shadow")?.GetComponent<Image>();
            if (shadow != null)
            {
                shadow.color = new Color32(184, 141, 92, 155);
            }
            Image selectedShadow = root.transform.Find("SelectShadow")?.GetComponent<Image>();
            if (selectedShadow != null)
            {
                selectedShadow.color = new Color32(201, 166, 104, 125);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    [MenuItem("Tools/Block Tap/Apply Batch 20 Current PlayMode Pixel Match")]
    public static void ApplyBatch20CurrentPlayModePixelMatch()
    {
        // Batch 20 is measured from the archived Batch 19 PlayMode captures in
        // 38_Batch19MeasuredPixelConvergence_PlayMode.  Only original visual
        // nodes are adjusted; hierarchy, bindings and runtime logic are kept.
        ApplyBatch19MeasuredPixelConvergence();
        ApplySettingsBatch20PixelMatch();
        ApplyDailyTasksBatch20PixelMatch();
        ApplyDailyTaskElementBatch20PixelMatch();
        ApplyItemBatch20PixelMatch();
        ApplyRealWithdrawBatch20PixelMatch();
        ApplyWithdrawLevelBatch20PixelMatch();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 20 current PlayMode pixel match applied successfully.");
    }

    [MenuItem("Tools/Block Tap/Apply Batch 21 Withdraw Correct-Aspect Match")]
    public static void ApplyBatch21WithdrawCorrectAspectMatch()
    {
        // Withdraw-only correction measured from the current correct-aspect
        // PlayMode comparison. Do not re-run the shared secondary pass here:
        // Settings, Daily Tasks and Item have their own calibrated geometry.
        ComposeApprovedWithdrawGradient();
        ComposeApprovedWithdrawChannelCards();
        ComposeApprovedWithdrawRuntimeCard();
        ComposeApprovedWithdrawButton();
        ConfigureGeneratedSprite(RealWithdrawTargetGradientPath);
        ConfigureGeneratedSprite(RealWithdrawTargetCardPath);
        ConfigureGeneratedSprite(RealWithdrawTargetButtonPath);
        ConfigureSpriteBorder(RealWithdrawTargetCardPath, new Vector4(24f, 24f, 24f, 24f));
        AssetDatabase.ImportAsset(
            RealWithdrawTargetCardPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(RealWithdrawRuntimeCardPath) == null)
        {
            if (!AssetDatabase.CopyAsset(RealWithdrawTargetCardPath, RealWithdrawRuntimeCardPath))
            {
                throw new InvalidOperationException("Unable to create the runtime withdrawal card sprite.");
            }
        }
        ConfigureGeneratedSprite(RealWithdrawRuntimeCardPath);
        ConfigureSpriteBorder(RealWithdrawRuntimeCardPath, new Vector4(24f, 24f, 24f, 24f));
        AssetDatabase.ImportAsset(
            RealWithdrawRuntimeCardPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        ApplyRealWithdrawBatch21CorrectAspectMatch();
        ApplyWithdrawWayBatch21CorrectAspectMatch();
        ApplyWithdrawLevelBatch21CorrectAspectMatch();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[OneToOneVisualRestorer] Batch 21 Withdraw correct-aspect match applied successfully.");
    }

    private static void ApplyRealWithdrawBatch21CorrectAspectMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image pageBackground = root.transform.Find("BG")?.GetComponent<Image>();
            if (pageBackground != null)
            {
                ConfigureImage(pageBackground, RequireSprite(RealWithdrawTargetGradientPath), Image.Type.Simple);
                pageBackground.color = Color.white;
            }

            // The approved effect uses the royal-blue outer rail with a gold
            // inner rail. Keep the already matched content transforms intact;
            // only the visual frame is enlarged to the target silhouette.
            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            // Match the approved 841x1870 capture: the frame starts below the
            // blue page header instead of extending behind it, while its lower
            // edge remains aligned with the first withdrawal-level row.
            withdrawFrame.anchoredPosition = new Vector2(withdrawFrame.anchoredPosition.x, -34f);
            withdrawFrame.sizeDelta = new Vector2(1028f, 1220f);
            ConfigureImage(withdrawFrame.GetComponent<Image>(), RequireSprite(SecondaryPanelFramePath), Image.Type.Simple);
            withdrawFrame.GetComponent<Image>().color = new Color32(255, 236, 195, 255);

            Mask viewportMask = root.transform.Find("Content/Scroll View/Viewport")?.GetComponent<Mask>();
            if (viewportMask != null)
            {
                viewportMask.enabled = false;
            }

            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            RectTransform ribbon = RequireRect(header.Find("ApprovedSecondaryRibbon"), prefabPath + "/Title/ApprovedSecondaryRibbon");
            ribbon.anchoredPosition = new Vector2(0f, -88f);
            ribbon.sizeDelta = new Vector2(625f, 313f);
            ConfigureImage(ribbon.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV3Path), Image.Type.Simple);
            ribbon.GetComponent<Image>().preserveAspect = false;

            RectTransform separateCrest = RequireRect(header.Find("ApprovedSecondaryCrest"), prefabPath + "/Title/ApprovedSecondaryCrest");
            separateCrest.gameObject.SetActive(false);

            TMP_Text approvedTitle = RequireRect(header.Find("ApprovedSecondaryTitle"), prefabPath + "/Title/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.anchoredPosition = new Vector2(0f, -105f);
            approvedTitle.rectTransform.sizeDelta = new Vector2(560f, 118f);
            approvedTitle.fontSizeMin = 28f;
            approvedTitle.fontSizeMax = 74f;
            approvedTitle.enableAutoSizing = false;
            approvedTitle.fontSize = 74f;
            approvedTitle.alignment = TextAlignmentOptions.Center;
            approvedTitle.fontSharedMaterial = RequireMaterial(BrownTitleMaterialPath);
            approvedTitle.color = new Color32(255, 205, 48, 255);

            // The old full-width top strip is a second header treatment. It
            // creates the flat blue band visible behind the approved ribbon.
            Image obsoleteTopStrip = header.Find("Image (1)")?.GetComponent<Image>();
            if (obsoleteTopStrip != null)
            {
                obsoleteTopStrip.enabled = false;
            }

            RectTransform withdrawButton = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn/Btn"), prefabPath + "/WithdrawInfo/WithdrawBtn/Btn");
            withdrawButton.anchoredPosition = new Vector2(0f, 0f);
            withdrawButton.sizeDelta = new Vector2(494f, 179f);
            Sprite approvedGreenButton = RequireSprite(RealWithdrawTargetButtonPath);
            ConfigureImage(withdrawButton.GetComponent<Image>(), approvedGreenButton, Image.Type.Simple);

            TMP_Text withdrawButtonLabel = withdrawButton.Find("Text (TMP)")?.GetComponent<TMP_Text>();
            if (withdrawButtonLabel != null)
            {
                withdrawButtonLabel.fontSharedMaterial = RequireMaterial(ButtonLabelMaterialPath);
                withdrawButtonLabel.enableAutoSizing = false;
                withdrawButtonLabel.fontSize = 72f;
            }

            RectTransform coinInfo = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo"), prefabPath + "/WithdrawInfo/CoinInfo");
            coinInfo.anchoredPosition = new Vector2(0f, 380f);

            RectTransform amountFrame = RequireRect(coinInfo.Find("CoinInfoFrame"), prefabPath + "/WithdrawInfo/CoinInfo/CoinInfoFrame");
            amountFrame.anchoredPosition = new Vector2(2f, -99f);

            RectTransform withdrawMode = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawMode"), prefabPath + "/WithdrawInfo/WithdrawMode");
            withdrawMode.anchoredPosition = new Vector2(withdrawMode.anchoredPosition.x, 15f);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                controller.normalSprite = approvedGreenButton;
                controller.canWithdrawSprite = approvedGreenButton;
                ConfigureImage(controller.btn_Image, approvedGreenButton, Image.Type.Simple);

                RectTransform methodTemplate = controller.withdrawWayItem != null
                    ? controller.withdrawWayItem.transform as RectTransform
                    : null;
                if (methodTemplate != null)
                {
                    Image methodBackground = methodTemplate.Find("Frame")?.GetComponent<Image>();
                    if (methodBackground != null)
                    {
                        ConfigureImage(methodBackground, RequireSprite(RealWithdrawRuntimeCardPath), Image.Type.Simple);
                        methodBackground.color = Color.white;
                    }
                }

                // The panel contains two authored prefab instances before the
                // runtime list expands. They can retain old per-instance sprite
                // overrides even when WithdrawWay.prefab itself is corrected,
                // so normalize every existing card as well as the template.
                RectTransform existingMethodRoot = controller.withdrawWayRoot as RectTransform;
                if (existingMethodRoot != null)
                {
                    foreach (WithdrawWay existingMethod in existingMethodRoot.GetComponentsInChildren<WithdrawWay>(true))
                    {
                        Image existingBackground = existingMethod.transform.Find("Frame")?.GetComponent<Image>();
                        if (existingBackground != null)
                        {
                            ConfigureImage(existingBackground, RequireSprite(RealWithdrawRuntimeCardPath), Image.Type.Simple);
                            existingBackground.color = Color.white;
                        }
                    }
                }

                RectTransform methodRoot = controller.withdrawWayRoot as RectTransform;
                GridLayoutGroup methodLayout = methodRoot != null ? methodRoot.GetComponent<GridLayoutGroup>() : null;
                if (methodRoot != null)
                {
                    methodRoot.anchoredPosition = new Vector2(9f, methodRoot.anchoredPosition.y);
                }
                if (methodLayout != null)
                {
                    methodLayout.padding.left = 0;
                    methodLayout.padding.right = 0;
                    methodLayout.cellSize = new Vector2(427f, 230f);
                    methodLayout.spacing = new Vector2(30f, methodLayout.spacing.y);
                }

                RectTransform levelRoot = controller.withdrawLevelRoot as RectTransform;
                GridLayoutGroup levelLayout = levelRoot != null ? levelRoot.GetComponent<GridLayoutGroup>() : null;
                if (levelRoot != null)
                {
                    levelRoot.anchoredPosition = new Vector2(6f, -41f);
                }
                if (levelLayout != null)
                {
                    levelLayout.cellSize = new Vector2(412f, 210f);
                    levelLayout.spacing = new Vector2(14f, 12f);
                }
            }

            RectTransform progressInfo = root.transform.Find("Content/Scroll View/Viewport/Content/ProgressInfo") as RectTransform;
            if (progressInfo != null)
            {
                progressInfo.anchoredPosition = new Vector2(progressInfo.anchoredPosition.x, 140f);
                RectTransform completeHint = progressInfo.Find("CompleteHint") as RectTransform;
                if (completeHint != null)
                {
                    completeHint.anchoredPosition = new Vector2(completeHint.anchoredPosition.x, -55f);
                }
                RectTransform progressHint = progressInfo.Find("progressHint") as RectTransform;
                if (progressHint != null)
                {
                    progressHint.anchoredPosition = new Vector2(progressHint.anchoredPosition.x, -95f);
                    progressHint.gameObject.SetActive(true);
                }
            }

            ribbon.SetAsLastSibling();
            approvedTitle.transform.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawWayBatch21CorrectAspectMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawWay.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image card = root.transform.Find("Frame")?.GetComponent<Image>();
            if (card != null)
            {
                ConfigureImage(card, RequireSprite(RealWithdrawRuntimeCardPath), Image.Type.Simple);
                card.color = Color.white;
            }

            RectTransform selected = root.transform.Find("Select") as RectTransform;
            if (selected != null)
            {
                selected.anchorMin = Vector2.one;
                selected.anchorMax = Vector2.one;
                selected.anchoredPosition = new Vector2(-42.6f, -41.9f);
                selected.sizeDelta = new Vector2(68f, 68f);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelBatch21CorrectAspectMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image card = root.transform.Find("bg")?.GetComponent<Image>();
            if (card != null)
            {
                ConfigureImage(card, RequireSprite(RealWithdrawRuntimeCardPath), Image.Type.Simple);
                card.color = Color.white;
            }

            // Keep the unavailable/selected state overlays, but reduce their
            // opacity so the approved blue/gold card edge and dark labels stay
            // readable instead of being uniformly greyed out.
            Image shadow = root.transform.Find("Shadow")?.GetComponent<Image>();
            if (shadow != null)
            {
                shadow.color = new Color32(135, 88, 28, 90);
            }
            Image selectedShadow = root.transform.Find("SelectShadow")?.GetComponent<Image>();
            if (selectedShadow != null)
            {
                selectedShadow.color = new Color32(160, 108, 45, 72);
            }

            WithdrawLevelItem levelItem = root.GetComponent<WithdrawLevelItem>();
            if (levelItem != null)
            {
                Color approvedBrown = new Color32(112, 57, 28, 255);
                if (levelItem.levelTxt != null)
                {
                    levelItem.levelTxt.color = approvedBrown;
                }
                if (levelItem.balanceText != null)
                {
                    levelItem.balanceText.color = approvedBrown;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 68f);
            frame.sizeDelta = new Vector2(1064f, 1134f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 508f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 473f);

            RectTransform contentCard = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Task_Elementbg_Target_v2"),
                prefabPath + "/BG (1)/Image (2) [content card]");
            contentCard.anchoredPosition = new Vector2(2.7f, 96f);
            contentCard.sizeDelta = new Vector2(872f, 340f);

            foreach (string groupPath in new[] { "MusicGroup", "SoundGroup", "LibGroup" })
            {
                RectTransform group = frame.Find(groupPath) as RectTransform;
                if (group != null)
                {
                    group.anchoredPosition = new Vector2(group.anchoredPosition.x, 96f);
                }
            }

            RectTransform restart = RequireRect(frame.Find("GamePauseGroup/BackButton"), prefabPath + "/GamePauseGroup/BackButton");
            restart.anchoredPosition = new Vector2(211f, -103f);
            restart.sizeDelta = new Vector2(419f, 193f);
            RectTransform continueButton = RequireRect(frame.Find("GamePauseGroup/ContinueBtn"), prefabPath + "/GamePauseGroup/ContinueBtn");
            continueButton.anchoredPosition = new Vector2(659f, -103f);
            continueButton.sizeDelta = new Vector2(419f, 193f);

            RectTransform version = frame.Find("VersionLog") as RectTransform;
            if (version != null)
            {
                version.anchoredPosition = new Vector2(version.anchoredPosition.x, 95f);
            }

            RectTransform closeIcon = frame.Find("CloseButton/Image (1)") as RectTransform;
            if (closeIcon != null)
            {
                ConfigureImage(closeIcon.GetComponent<Image>(), RequireSprite(DailyTasksCloseTargetPath), Image.Type.Simple);
                closeIcon.sizeDelta = new Vector2(135f, 135f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -75f);
            frame.sizeDelta = new Vector2(1124f, 1644f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 730f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 106f);

            RectTransform closeIcon = root.transform.Find("CloseBtn/Image (2)") as RectTransform;
            if (closeIcon != null)
            {
                ConfigureImage(closeIcon.GetComponent<Image>(), RequireSprite(DailyTasksCloseTargetPath), Image.Type.Simple);
                closeIcon.anchoredPosition = new Vector2(4f, closeIcon.anchoredPosition.y);
                closeIcon.sizeDelta = new Vector2(130f, 130f);
            }

            RectTransform header = RequireRect(frame.Find("Image (3)"), prefabPath + "/BG/BG/Image (3)");
            header.SetAsLastSibling();
            title.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTaskElementBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform bar = root.transform.Find("BarBG") as RectTransform;
            if (bar != null)
            {
                bar.sizeDelta = new Vector2(504f, 56.33f);
            }

            RectTransform state = root.transform.Find("State") as RectTransform;
            if (state != null)
            {
                state.anchoredPosition = new Vector2(-136f, state.anchoredPosition.y);
                state.sizeDelta = new Vector2(192f, 192f);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 104f);
            frame.sizeDelta = new Vector2(1022f, 1107f);

            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 479f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 428f);

            RectTransform contentCard = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v4_Softened"),
                prefabPath + "/BG (1)/Image (2) [content card]");
            contentCard.anchoredPosition = new Vector2(0f, 72f);
            contentCard.sizeDelta = new Vector2(840f, 442f);

            RectTransform propIcon = root.transform.Find("Content/PropIcon") as RectTransform;
            if (propIcon != null)
            {
                propIcon.sizeDelta = new Vector2(332f, 332f);
            }

            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            withdrawFrame.anchoredPosition = new Vector2(withdrawFrame.anchoredPosition.x, 5f);
            withdrawFrame.sizeDelta = new Vector2(1052f, 1338f);

            RectTransform amountFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CoinInfoFrame"), prefabPath + "/CoinInfo/CoinInfoFrame");
            amountFrame.anchoredPosition = new Vector2(4f, -99f);
            amountFrame.sizeDelta = new Vector2(905f, 209f);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                RectTransform levelRoot = controller.withdrawLevelRoot as RectTransform;
                if (levelRoot != null)
                {
                    levelRoot.anchoredPosition = new Vector2(6f, -44f);
                }
                GridLayoutGroup levelLayout = levelRoot != null ? levelRoot.GetComponent<GridLayoutGroup>() : null;
                if (levelLayout != null)
                {
                    levelLayout.cellSize = new Vector2(412f, 226f);
                    levelLayout.spacing = new Vector2(14f, 4f);
                }
            }

            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelBatch20PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            foreach (string childName in new[] { "bg", "Shadow", "SelectShadow" })
            {
                RectTransform child = root.transform.Find(childName) as RectTransform;
                if (child != null)
                {
                    child.sizeDelta = new Vector2(832f, 453f);
                }
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 70f);

            ConfigureOriginalButton(frame, "GamePauseGroup/BackButton", RequireSprite(GreenButtonPath), new Vector2(425f, 200f));
            ConfigureOriginalButton(frame, "GamePauseGroup/ContinueBtn", RequireSprite(SettingsBlueButtonTargetV2Path), new Vector2(425f, 200f));
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            // The approved frame is 24 comparison pixels taller at the top while
            // sharing the same bottom edge as Batch 16.
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -80f);
            frame.sizeDelta = new Vector2(1076f, 1650f);

            RectTransform header = RequireRect(frame.Find("Image (3)"), prefabPath + "/BG/BG/Image (3)");
            header.anchoredPosition = new Vector2(0f, 766f);
            header.sizeDelta = new Vector2(950f, 500f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 730f);
            title.sizeDelta = new Vector2(800f, 160f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 100f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 198f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(895f, 250f);
                layout.spacing = new Vector2(0f, 10f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 431f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 72f);
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            RectTransform ribbon = RequireRect(header.Find("ApprovedSecondaryRibbon"), prefabPath + "/Title/ApprovedSecondaryRibbon");
            ribbon.anchoredPosition = new Vector2(0f, -112f);
            ribbon.sizeDelta = new Vector2(800f, 232f);

            RectTransform crest = RequireRect(header.Find("ApprovedSecondaryCrest"), prefabPath + "/Title/ApprovedSecondaryCrest");
            crest.anchoredPosition = new Vector2(0f, -10f);
            crest.sizeDelta = new Vector2(420f, 210f);

            TMP_Text approvedTitle = RequireRect(header.Find("ApprovedSecondaryTitle"), prefabPath + "/Title/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.anchoredPosition = new Vector2(0f, -68f);
            approvedTitle.rectTransform.sizeDelta = new Vector2(620f, 120f);
            approvedTitle.fontSizeMax = 80f;

            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            withdrawFrame.anchoredPosition = new Vector2(withdrawFrame.anchoredPosition.x, -40f);
            withdrawFrame.sizeDelta = new Vector2(1068f, 1265f);

            RectTransform amountFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CoinInfoFrame"), prefabPath + "/CoinInfo/CoinInfoFrame");
            amountFrame.anchoredPosition = new Vector2(0f, -54f);
            amountFrame.sizeDelta = new Vector2(1010f, 330f);
            Image amountImage = amountFrame.GetComponent<Image>();
            amountImage.enabled = true;
            ConfigureImage(amountImage, RequireSprite(SecondaryContentCardTargetV4Path), Image.Type.Simple);
            amountImage.color = Color.white;
            amountImage.preserveAspect = false;

            RectTransform withdrawButtonGroup = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn"), prefabPath + "/WithdrawBtn");
            withdrawButtonGroup.anchoredPosition = new Vector2(withdrawButtonGroup.anchoredPosition.x, -375f);
            withdrawButtonGroup.sizeDelta = new Vector2(withdrawButtonGroup.sizeDelta.x, 210f);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                RectTransform methodRoot = controller.withdrawWayRoot as RectTransform;
                GridLayoutGroup methodLayout = methodRoot != null ? methodRoot.GetComponent<GridLayoutGroup>() : null;
                if (methodLayout != null)
                {
                    methodLayout.cellSize = new Vector2(430f, 230f);
                    methodLayout.spacing = new Vector2(20f, methodLayout.spacing.y);
                }

                RectTransform levelRoot = controller.withdrawLevelRoot as RectTransform;
                GridLayoutGroup levelLayout = levelRoot != null ? levelRoot.GetComponent<GridLayoutGroup>() : null;
                if (levelRoot != null)
                {
                    levelRoot.anchoredPosition = new Vector2(6f, -37f);
                }
                if (levelLayout != null)
                {
                    levelLayout.cellSize = new Vector2(412f, 218f);
                    levelLayout.spacing = new Vector2(10f, 15f);
                }

                if (controller.progressObj != null)
                {
                    RectTransform progress = controller.progressObj.transform as RectTransform;
                    if (progress != null)
                    {
                        progress.anchoredPosition = new Vector2(progress.anchoredPosition.x, progress.anchoredPosition.y - 62f);
                    }
                }
            }

            ribbon.SetAsLastSibling();
            crest.SetAsLastSibling();
            approvedTitle.transform.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawWayBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawWay.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform rootRect = RequireRect(root.transform, prefabPath);
            rootRect.sizeDelta = new Vector2(430f, 230f);
            RectTransform payIcon = RequireRect(root.transform.Find("Frame"), prefabPath + "/Frame");
            payIcon.anchorMin = Vector2.zero;
            payIcon.anchorMax = Vector2.one;
            payIcon.anchoredPosition = Vector2.zero;
            payIcon.sizeDelta = Vector2.zero;
            payIcon.GetComponent<Image>().preserveAspect = false;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelBatch17PixelMatch()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        Sprite cardSprite = RequireSprite(AmountCardPath);
        Sprite flatSurface = RequireSprite(RealWithdrawSurfacePath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform rootRect = RequireRect(root.transform, prefabPath);
            rootRect.sizeDelta = new Vector2(412f, 218f);

            Image background = root.transform.Find("bg")?.GetComponent<Image>();
            if (background != null)
            {
                ConfigureImage(background, cardSprite, Image.Type.Simple);
                background.color = Color.white;
                background.preserveAspect = false;
            }

            foreach (string overlayName in new[] { "Shadow", "SelectShadow" })
            {
                RectTransform overlayRect = root.transform.Find(overlayName) as RectTransform;
                Image overlay = overlayRect != null ? overlayRect.GetComponent<Image>() : null;
                if (overlayRect != null && overlay != null)
                {
                    overlayRect.sizeDelta = new Vector2(760f, 360f);
                    ConfigureImage(overlay, flatSurface, Image.Type.Simple);
                    overlay.color = overlayName == "Shadow"
                        ? new Color32(132, 116, 96, 205)
                        : new Color32(164, 140, 102, 150);
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsBatch16Correction()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");

            // Approved effect keeps the same width, but the crest/ribbon is
            // vertically tighter than the Batch 15 runtime rendering.
            header.anchoredPosition = new Vector2(0f, 505f);
            header.sizeDelta = new Vector2(760f, 350f);
            header.SetAsLastSibling();

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 473f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);
            title.SetAsLastSibling();

            RectTransform close = frame.Find("CloseButton") as RectTransform;
            if (close != null)
            {
                close.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch16Correction()
    {
        const string pagePath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject page = PrefabUtility.LoadPrefabContents(pagePath);
        try
        {
            RectTransform frame = RequireRect(page.transform.Find("BG/BG"), pagePath + "/BG/BG");
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -95f);
            frame.sizeDelta = new Vector2(1048f, 1620f);

            RectTransform header = RequireRect(frame.Find("Image (3)"), pagePath + "/BG/BG/Image (3)");
            header.gameObject.SetActive(true);
            header.anchoredPosition = new Vector2(0f, 790f);
            header.sizeDelta = new Vector2(830f, 520f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV5Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), pagePath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 712f);
            title.sizeDelta = new Vector2(760f, 150f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 80f);

            RectTransform taskRoot = RequireRect(page.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), pagePath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(885f, 250f);
                layout.spacing = new Vector2(0f, 10f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(page, pagePath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(page);
        }
    }

    private static void ApplyItemBatch16Correction()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            RectTransform header = RequireRect(
                FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4"),
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 480f);
            header.sizeDelta = new Vector2(820f, 355f);
            header.SetAsLastSibling();

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 439f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);
            title.SetAsLastSibling();

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsBatch15Correction()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 505f);
            header.sizeDelta = new Vector2(760f, 390f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV5Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 473f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);

            RectTransform close = frame.Find("CloseButton") as RectTransform;
            if (close != null)
            {
                close.anchoredPosition = new Vector2(-100f, close.anchoredPosition.y);
                close.SetAsLastSibling();
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            if (close != null)
            {
                close.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksBatch15Correction()
    {
        const string pagePath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        GameObject page = PrefabUtility.LoadPrefabContents(pagePath);
        try
        {
            RectTransform frame = RequireRect(page.transform.Find("BG/BG"), pagePath + "/BG/BG");
            frame.sizeDelta = new Vector2(1048f, 1650f);

            // The standalone ribbon node is clipped by this prefab's runtime
            // hierarchy. Reuse the already-visible crest node for the complete
            // header artwork so the approved ribbon and crest render together.
            RectTransform ribbon = RequireRect(frame.Find("Image (6)"), pagePath + "/BG/BG/Image (6)");
            ribbon.gameObject.SetActive(false);

            RectTransform header = RequireRect(frame.Find("Image (3)"), pagePath + "/BG/BG/Image (3)");
            header.gameObject.SetActive(true);
            header.anchoredPosition = new Vector2(0f, 849f);
            header.sizeDelta = new Vector2(900f, 425f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV5Path), Image.Type.Simple);
            header.GetComponent<Image>().color = Color.white;
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), pagePath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 735f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 65f);

            RectTransform taskRoot = RequireRect(page.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), pagePath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(833f, 240f);
                layout.spacing = new Vector2(0f, 10f);
            }

            header.SetAsLastSibling();
            title.SetAsLastSibling();

            PrefabUtility.SaveAsPrefabAsset(page, pagePath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(page);
        }
    }

    private static void ApplyItemBatch15Correction()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            Transform headerTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v4")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_HeaderComposite_v5");
            RectTransform header = RequireRect(
                headerTransform,
                prefabPath + "/BG (1)/Image (2) [composite header]");
            header.anchoredPosition = new Vector2(0f, 468f);
            header.sizeDelta = new Vector2(820f, 330f);
            ConfigureImage(header.GetComponent<Image>(), RequireSprite(SecondaryHeaderCompositeV5Path), Image.Type.Simple);
            header.GetComponent<Image>().preserveAspect = false;

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 439f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 64f);

            Transform contentTransform = FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v4_Softened")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v2")
                ?? FindDirectChildBySpriteName(frame, "Image (2)", "Secondary_ContentCard_Gold_v3_Soft")
                ?? FindDirectChildByApproximateSize(frame, "Image (2)", new Vector2(860f, 520f));
            RectTransform contentCard = RequireRect(
                contentTransform,
                prefabPath + "/BG (1)/Image (2) [item content card]");
            contentCard.sizeDelta = new Vector2(833f, 435f);
            contentCard.GetComponent<Image>().color = Color.white;

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.sizeDelta = new Vector2(380f, 380f);

            RectTransform reward = RequireRect(root.transform.Find("Content/BtnGroup/ButtonAnim/Pop"), prefabPath + "/Content/BtnGroup/ButtonAnim/Pop");
            reward.localScale = new Vector3(0.9f, 0.9f, 1f);

            header.SetAsLastSibling();
            title.SetAsLastSibling();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsOriginalNodeCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        Sprite cardSprite = RequireSprite(DailyTaskCardPath);
        Sprite musicTarget = RequireSprite(SettingsMusicTargetPath);
        Sprite soundTarget = RequireSprite(SettingsSoundTargetPath);
        Sprite vibrateTarget = RequireSprite(SettingsVibrateTargetPath);
        Sprite greenButton = RequireSprite(GreenButtonPath);
        Sprite blueButton = RequireSprite(SecondaryBlueButtonPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1090f, 1205f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 76f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 490f);
            title.sizeDelta = new Vector2(700f, 120f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 60f);

            RectTransform contentCard = RequireRect(FindDirectChild(frame, "Image (2)", true), prefabPath + "/BG (1)/Image (2) [active]");
            contentCard.anchoredPosition = new Vector2(contentCard.anchoredPosition.x, 89f);
            contentCard.sizeDelta = new Vector2(862f, 346f);
            ConfigureImage(contentCard.GetComponent<Image>(), cardSprite, Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = Color.white;

            ApplySettingsOriginalStateIcon(frame, "MusicGroup/Button/MusicOn/Image (2)", musicTarget);
            ApplySettingsOriginalStateIcon(frame, "MusicGroup/Button/MusicOff/Image (2)", musicTarget, new Color32(150, 160, 175, 255));
            ApplySettingsOriginalStateIcon(frame, "SoundGroup/Button (1)/SoundOn/Image (2)", soundTarget);
            ApplySettingsOriginalStateIcon(frame, "SoundGroup/Button (1)/SoundOff/Image (2)", soundTarget, new Color32(150, 160, 175, 255));
            ApplySettingsOriginalStateIcon(frame, "LibGroup/Button (2)/LibOn/Image (2)", vibrateTarget);
            ApplySettingsOriginalStateIcon(frame, "LibGroup/Button (2)/LIboff/Image (2)", vibrateTarget, new Color32(150, 160, 175, 255));

            foreach (string groupPath in new[] { "MusicGroup", "SoundGroup", "LibGroup" })
            {
                RectTransform group = RequireRect(frame.Find(groupPath), prefabPath + "/BG (1)/" + groupPath);
                group.anchoredPosition = new Vector2(group.anchoredPosition.x, 89f);
            }

            ConfigureOriginalButton(frame, "GamePauseGroup/BackButton", greenButton, new Vector2(410f, 175f));
            ConfigureOriginalButton(frame, "GamePauseGroup/ContinueBtn", blueButton, new Vector2(410f, 175f));

            RectTransform version = RequireRect(frame.Find("VersionLog"), prefabPath + "/BG (1)/VersionLog");
            version.anchoredPosition = new Vector2(version.anchoredPosition.x, 102f);
            RectTransform close = RequireRect(frame.Find("CloseButton"), prefabPath + "/BG (1)/CloseButton");
            close.anchoredPosition = new Vector2(-130f, -50f);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsOriginalStateIcon(RectTransform frame, string path, Sprite sprite, Color32? tint = null)
    {
        RectTransform iconRect = RequireRect(frame.Find(path), frame.name + "/" + path);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(176f, 176f);
        Image icon = iconRect.GetComponent<Image>();
        ConfigureImage(icon, sprite, Image.Type.Simple);
        icon.preserveAspect = true;
        icon.color = tint ?? Color.white;
    }

    private static void ConfigureOriginalButton(RectTransform frame, string path, Sprite sprite, Vector2 size)
    {
        RectTransform button = RequireRect(frame.Find(path), frame.name + "/" + path);
        button.anchoredPosition = new Vector2(button.anchoredPosition.x, -118f);
        button.sizeDelta = size;
        Image image = button.GetComponent<Image>();
        ConfigureImage(image, sprite, Image.Type.Simple);
        image.color = Color.white;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 28f;
            label.fontSizeMax = 58f;
            label.enableWordWrapping = false;
            label.color = Color.white;
            label.fontSharedMaterial = RequireMaterial(ButtonLabelMaterialPath);
        }
    }

    private static void ApplyDailyTasksOriginalNodeCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        Sprite closeTarget = RequireSprite(DailyTasksCloseTargetPath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.sizeDelta = new Vector2(1076f, 1636f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -66f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG/BG/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 705f);
            title.sizeDelta = new Vector2(760f, 140f);
            title.localScale = new Vector3(1f, 1.15f, 1f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 80f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 210f);
            RectTransform bottom = RequireRect(root.transform.Find("Content/DailyTask/Bottom"), prefabPath + "/Content/DailyTask/Bottom");
            bottom.anchoredPosition = new Vector2(bottom.anchoredPosition.x, -187f);
            bottom.sizeDelta = new Vector2(bottom.sizeDelta.x, 86f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                throw new InvalidOperationException("Daily task root has no GridLayoutGroup: " + prefabPath);
            }
            layout.cellSize = new Vector2(833f, 225f);
            layout.spacing = new Vector2(0f, 20f);

            RectTransform close = RequireRect(root.transform.Find("CloseBtn"), prefabPath + "/CloseBtn");
            close.anchoredPosition = new Vector2(414f, 704f);
            Image closeBackground = close.GetComponent<Image>();
            if (closeBackground != null)
            {
                closeBackground.color = new Color32(255, 255, 255, 0);
            }
            RectTransform closeIcon = RequireRect(close.Find("Image (2)"), prefabPath + "/CloseBtn/Image (2)");
            closeIcon.sizeDelta = new Vector2(122f, 122f);
            ConfigureImage(closeIcon.GetComponent<Image>(), closeTarget, Image.Type.Simple);
            closeIcon.GetComponent<Image>().preserveAspect = true;

            ApplyDailyTaskElementMeasuredCalibration();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyItemOriginalNodeCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        Sprite cardSprite = RequireSprite(SecondaryContentCardPath);
        Sprite closeTarget = RequireSprite(ItemCloseTargetPath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1080f, 1200f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 105f);

            RectTransform title = RequireRect(frame.Find("Text (TMP)"), prefabPath + "/BG (1)/Text (TMP)");
            title.anchoredPosition = new Vector2(0f, 428f);
            title.sizeDelta = new Vector2(650f, 120f);
            ConfigureOriginalTitle(title.GetComponent<TMP_Text>(), 72f);

            RectTransform contentCard = RequireRect(FindDirectChild(frame, "Image (2)", true), prefabPath + "/BG (1)/Image (2) [active]");
            contentCard.anchoredPosition = new Vector2(contentCard.anchoredPosition.x, 69f);
            contentCard.sizeDelta = new Vector2(825f, 500f);
            ConfigureImage(contentCard.GetComponent<Image>(), cardSprite, Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = new Color32(255, 242, 210, 255);

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.anchoredPosition = new Vector2(propIcon.anchoredPosition.x, 163f);
            propIcon.sizeDelta = new Vector2(360f, 360f);
            RectTransform propName = RequireRect(root.transform.Find("Content/PropName"), prefabPath + "/Content/PropName");
            propName.anchoredPosition = new Vector2(propName.anchoredPosition.x, -95f);
            RectTransform reward = RequireRect(root.transform.Find("Content/BtnGroup/ButtonAnim/Pop"), prefabPath + "/Content/BtnGroup/ButtonAnim/Pop");
            reward.anchoredPosition = new Vector2(reward.anchoredPosition.x, 189f);
            RectTransform limitation = RequireRect(root.transform.Find("Content/Limination"), prefabPath + "/Content/Limination");
            limitation.anchoredPosition = new Vector2(limitation.anchoredPosition.x, -492f);

            RectTransform close = RequireRect(root.transform.Find("Content/CloseBtn"), prefabPath + "/Content/CloseBtn");
            close.anchoredPosition = new Vector2(397f, 614f);
            Image closeBackground = close.GetComponent<Image>();
            if (closeBackground != null)
            {
                closeBackground.color = new Color32(255, 255, 255, 0);
            }
            RectTransform closeIcon = RequireRect(close.Find("Image (3)"), prefabPath + "/Content/CloseBtn/Image (3)");
            closeIcon.sizeDelta = new Vector2(110f, 110f);
            ConfigureImage(closeIcon.GetComponent<Image>(), closeTarget, Image.Type.Simple);
            closeIcon.GetComponent<Image>().preserveAspect = true;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureOriginalTitle(TMP_Text title, float maxSize)
    {
        if (title == null)
        {
            return;
        }
        title.alignment = TextAlignmentOptions.Center;
        title.enableAutoSizing = true;
        title.fontSizeMin = 25f;
        title.fontSizeMax = maxSize;
        title.enableWordWrapping = false;
        title.overflowMode = TextOverflowModes.Ellipsis;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color32(255, 205, 48, 255);
        title.fontSharedMaterial = RequireMaterial(ButtonLabelMaterialPath);
        title.raycastTarget = false;
    }

    private static void ApplyStructureLockedCard(string prefabPath, string framePath, string childName)
    {
        Sprite cardSprite = RequireSprite(DailyTaskCardPath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find(framePath), prefabPath + "/" + framePath);
            Transform child = FindDirectChild(frame, childName, true);
            Image image = child != null ? child.GetComponent<Image>() : null;
            if (image != null)
            {
                ConfigureImage(image, cardSprite, Image.Type.Sliced);
                image.color = Color.white;
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyStructureLockedImage(string prefabPath, string objectPath, string spritePath)
    {
        Sprite sprite = RequireSprite(spritePath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image image = root.transform.Find(objectPath)?.GetComponent<Image>();
            if (image != null)
            {
                ConfigureImage(image, sprite, Image.Type.Simple);
                image.preserveAspect = true;
                image.color = Color.white;
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyStructureLockedImageTint(string prefabPath, string objectPath, string spritePath, Color32 tint)
    {
        Sprite sprite = RequireSprite(spritePath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image image = root.transform.Find(objectPath)?.GetComponent<Image>();
            if (image != null)
            {
                ConfigureImage(image, sprite, Image.Type.Simple);
                image.preserveAspect = true;
                image.color = tint;
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksStructureLockedCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        Sprite closeTarget = RequireSprite(DailyTasksCloseTargetPath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.sizeDelta = new Vector2(1076f, 1636f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -66f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 210f);

            RectTransform bottom = RequireRect(root.transform.Find("Content/DailyTask/Bottom"), prefabPath + "/Content/DailyTask/Bottom");
            bottom.anchoredPosition = new Vector2(bottom.anchoredPosition.x, -187f);
            bottom.sizeDelta = new Vector2(bottom.sizeDelta.x, 86f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.cellSize = new Vector2(833f, 225f);
                layout.spacing = new Vector2(0f, 20f);
            }

            RectTransform close = RequireRect(root.transform.Find("CloseBtn"), prefabPath + "/CloseBtn");
            close.anchoredPosition = new Vector2(414f, 704f);
            Image closeBackground = close.GetComponent<Image>();
            if (closeBackground != null)
            {
                closeBackground.color = new Color32(255, 255, 255, 0);
            }
            RectTransform closeIcon = RequireRect(close.Find("Image (2)"), prefabPath + "/CloseBtn/Image (2)");
            Image closeImage = closeIcon.GetComponent<Image>();
            ConfigureImage(closeImage, closeTarget, Image.Type.Simple);
            closeImage.preserveAspect = true;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }

        GameObject element = PrefabUtility.LoadPrefabContents("Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab");
        try
        {
            Image card = element.GetComponent<Image>();
            if (card != null)
            {
                card.color = Color.white;
            }
            PrefabUtility.SaveAsPrefabAsset(element, "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(element);
        }
    }

    private static void HideAddPropAdVisuals()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Transform ad = root.transform.Find("Content/BtnGroup/ButtonAnim/AdBtn");
            if (ad != null)
            {
                Image background = ad.GetComponent<Image>();
                if (background != null)
                {
                    Color color = background.color;
                    background.color = new Color(color.r, color.g, color.b, 0f);
                }
                foreach (Graphic graphic in ad.GetComponentsInChildren<Graphic>(true))
                {
                    if (graphic.transform == ad)
                    {
                        continue;
                    }
                    Color color = graphic.color;
                    graphic.color = new Color(color.r, color.g, color.b, 0f);
                }
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawContentOwnedHeader()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            foreach (string childName in new[] { "ApprovedSecondaryRibbon", "ApprovedSecondaryCrest" })
            {
                Transform child = header.Find(childName);
                if (child != null)
                {
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }
            TMP_Text title = header.Find("ApprovedSecondaryTitle")?.GetComponent<TMP_Text>();
            if (title != null)
            {
                title.rectTransform.anchoredPosition = new Vector2(0f, -110f);
                title.rectTransform.sizeDelta = new Vector2(520f, 100f);
                title.fontSizeMin = 28f;
                title.fontSizeMax = 62f;
                title.transform.SetAsLastSibling();
            }
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void AdjustSecondaryHeaderCover(string prefabPath, string framePath, float widthInset, float height, float centerInset)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find(framePath), prefabPath + "/" + framePath);
            Transform coverTransform = frame.Find("ApprovedSecondaryHeaderCover");
            // The current approved frame is a continuous cream surface and no
            // longer needs the legacy title-well cover. Some structure-locked
            // prefabs therefore intentionally do not contain this optional
            // helper; absence must not abort the rest of the batch.
            if (coverTransform == null)
            {
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                return;
            }
            RectTransform cover = RequireRect(coverTransform, prefabPath + "/ApprovedSecondaryHeaderCover");
            float frameWidth = Mathf.Abs(frame.rect.width);
            float frameHeight = Mathf.Abs(frame.rect.height);
            if (frameWidth < 10f)
            {
                frameWidth = Mathf.Abs(frame.sizeDelta.x);
            }
            if (frameHeight < 10f)
            {
                frameHeight = Mathf.Abs(frame.sizeDelta.y);
            }
            cover.anchoredPosition = new Vector2(0f, frameHeight * 0.5f - centerInset);
            // Some legacy full-screen prefabs use stretched negative sizeDelta
            // values. A fixed lower bound prevents their old blue title well
            // from showing around the approved ribbon.
            cover.sizeDelta = new Vector2(Mathf.Max(900f, frameWidth - widthInset), height);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void SetPrefabObjectActive(string prefabPath, string objectPath, bool active)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Transform target = root.transform.Find(objectPath);
            if (target != null)
            {
                target.gameObject.SetActive(active);
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void RemoveSecondaryThemeChildren(string prefabPath, string parentPath)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Transform parent = root.transform.Find(parentPath);
            if (parent != null)
            {
                foreach (string childName in new[] { "ApprovedSecondaryRibbon", "ApprovedSecondaryCrest", "ApprovedSecondaryTitle", "ApprovedSecondaryHeaderCover" })
                {
                    Transform child = parent.Find(childName);
                    if (child != null)
                    {
                        UnityEngine.Object.DestroyImmediate(child.gameObject);
                    }
                }
            }
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySecondaryModalTheme(string prefabPath, string framePath, string oldTitlePath, string titleValue)
    {
        // The approved effect has one continuous warm panel.  The earlier
        // Common_Frame asset contained a second blue title well and required a
        // rectangular cover, which produced the visible extra backing plate.
        Sprite frameSprite = RequireSprite(SecondaryPanelFramePath);
        Sprite ribbonSprite = RequireSprite(SecondaryRibbonPath);
        Sprite crestSprite = RequireSprite(SecondaryCrestPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find(framePath), prefabPath + "/" + framePath);
            ConfigurePanelImage(frame.gameObject, frameSprite);

            float frameWidth = Mathf.Abs(frame.rect.width);
            float frameHeight = Mathf.Abs(frame.rect.height);
            if (frameWidth < 10f)
            {
                frameWidth = Mathf.Abs(frame.sizeDelta.x);
            }
            if (frameHeight < 10f)
            {
                frameHeight = Mathf.Abs(frame.sizeDelta.y);
            }
            frameWidth = Mathf.Max(760f, frameWidth);
            frameHeight = Mathf.Max(760f, frameHeight);

            // The approved renders use a lower, narrower header than the first
            // approximation.  These values are measured after normalizing the
            // approved images to the 1080 px runtime capture width.
            float ribbonWidth = Mathf.Clamp(frameWidth * 0.76f, 620f, 840f);
            float ribbonHeight = ribbonWidth * 0.3334f;
            float crestWidth = Mathf.Clamp(frameWidth * 0.40f, 320f, 440f);
            float crestHeight = crestWidth * 0.5f;
            float ribbonY = frameHeight * 0.5f - 145f;
            float crestY = frameHeight * 0.5f - 20f;

            Transform obsoleteCover = frame.Find("ApprovedSecondaryHeaderCover");
            if (obsoleteCover != null)
            {
                obsoleteCover.gameObject.SetActive(false);
            }
            EnsureDecorativeImage(frame, "ApprovedSecondaryRibbon", ribbonSprite, new Vector2(0f, ribbonY), new Vector2(ribbonWidth, ribbonHeight));
            EnsureDecorativeImage(frame, "ApprovedSecondaryCrest", crestSprite, new Vector2(0f, crestY), new Vector2(crestWidth, crestHeight));

            Transform oldTitleTransform = root.transform.Find(oldTitlePath);
            TMP_Text source = oldTitleTransform != null ? oldTitleTransform.GetComponent<TMP_Text>() : null;
            if (source == null)
            {
                source = root.GetComponentInChildren<TMP_Text>(true);
            }
            if (source == null)
            {
                throw new InvalidOperationException("No TMP source text found for secondary title: " + prefabPath);
            }

            HideTextGraphic(oldTitleTransform);
            TMP_Text title = EnsurePlainTMP(frame, source, "ApprovedSecondaryTitle", titleValue);
            title.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            title.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            title.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            title.rectTransform.anchoredPosition = new Vector2(0f, ribbonY + 5f);
            title.rectTransform.sizeDelta = new Vector2(ribbonWidth * 0.72f, 128f);
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 25f;
            title.fontSizeMax = 60f;
            title.enableWordWrapping = false;
            title.overflowMode = TextOverflowModes.Ellipsis;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color32(255, 205, 48, 255);
            title.raycastTarget = false;

            ApplyApprovedButtonTheme(root);
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySecondaryFullPageHeader(string prefabPath, string headerPath, string oldTitlePath, string titleValue)
    {
        Sprite ribbonSprite = RequireSprite(SecondaryRibbonPath);
        Sprite crestSprite = RequireSprite(SecondaryCrestPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find(headerPath), prefabPath + "/" + headerPath);
            Transform oldTitleTransform = root.transform.Find(oldTitlePath);
            TMP_Text source = oldTitleTransform != null ? oldTitleTransform.GetComponent<TMP_Text>() : null;
            if (source == null)
            {
                source = root.GetComponentInChildren<TMP_Text>(true);
            }
            if (source == null)
            {
                throw new InvalidOperationException("No TMP source text found for full-page secondary title: " + prefabPath);
            }

            HideTextGraphic(oldTitleTransform);
            EnsureDecorativeImage(header, "ApprovedSecondaryRibbon", ribbonSprite, new Vector2(0f, -48f), new Vector2(560f, 187f));
            EnsureDecorativeImage(header, "ApprovedSecondaryCrest", crestSprite, new Vector2(0f, 10f), new Vector2(260f, 130f));
            TMP_Text title = EnsurePlainTMP(header, source, "ApprovedSecondaryTitle", titleValue);
            title.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            title.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            title.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            title.rectTransform.anchoredPosition = new Vector2(0f, -43f);
            title.rectTransform.sizeDelta = new Vector2(430f, 86f);
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 24f;
            title.fontSizeMax = 52f;
            title.enableWordWrapping = false;
            title.overflowMode = TextOverflowModes.Ellipsis;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color32(255, 205, 48, 255);
            title.raycastTarget = false;

            ApplyApprovedButtonTheme(root);
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void EnsureHeaderCover(RectTransform frame, float frameWidth, float frameHeight)
    {
        Transform existing = frame.Find("ApprovedSecondaryHeaderCover");
        GameObject coverObject = existing != null
            ? existing.gameObject
            : new GameObject("ApprovedSecondaryHeaderCover", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            coverObject.transform.SetParent(frame, false);
        }

        RectTransform rect = coverObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, frameHeight * 0.5f - 118f);
        rect.sizeDelta = new Vector2(Mathf.Max(520f, frameWidth - 150f), 190f);

        Image image = coverObject.GetComponent<Image>();
        image.sprite = null;
        image.color = new Color32(255, 244, 216, 255);
        image.raycastTarget = false;
        coverObject.transform.SetAsLastSibling();
    }

    private static void ApplyApprovedButtonTheme(GameObject root)
    {
        Sprite greenButton = RequireSprite(PrimaryButtonPath);
        Sprite blueButton = RequireSprite(SecondaryBlueButtonPath);
        Material buttonLabelMaterial = RequireMaterial(ButtonLabelMaterialPath);

        HashSet<GameObject> candidates = new HashSet<GameObject>();
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
        {
            candidates.Add(button.gameObject);
        }

        // This project uses BizzaButton for most production UI controls.  It is
        // a pointer-handler MonoBehaviour rather than a Unity UI Button, so the
        // earlier pass silently missed Settings, Withdraw and several result
        // buttons even though their labels matched the approved button list.
        foreach (BizzaButton button in root.GetComponentsInChildren<BizzaButton>(true))
        {
            candidates.Add(button.gameObject);
        }

        foreach (GameObject candidate in candidates)
        {
            Image image = candidate.GetComponent<Image>();
            RectTransform rect = candidate.transform as RectTransform;
            TMP_Text label = candidate.GetComponentInChildren<TMP_Text>(true);
            if (image == null || rect == null || label == null)
            {
                continue;
            }

            // Prefab contents are not attached to a live Canvas, so rect.width
            // can still report the pre-layout value.  The authored sizeDelta is
            // the reliable measurement for these fixed-anchor buttons.
            float width = Mathf.Max(Mathf.Abs(rect.rect.width), Mathf.Abs(rect.sizeDelta.x));
            float height = Mathf.Max(Mathf.Abs(rect.rect.height), Mathf.Abs(rect.sizeDelta.y));
            if (width < 240f || width > 760f || height < 70f || height > 240f)
            {
                continue;
            }

            string value = (label.text ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            bool secondary = value.Contains("continue") || value == "home" || value == "back";
            bool primary = value.Contains("withdraw") || value.Contains("retirar") || value.Contains("confirm") ||
                           value.Contains("claim") || value.Contains("retry") || value.Contains("restart") ||
                           value.Contains("spin") || value == "play" || value == "ok" || value.Contains("send");
            if (!primary && !secondary)
            {
                continue;
            }

            // These button sprites already contain their full bevel, trim and
            // highlight.  Slicing removed most of that approved detail.
            ConfigureImage(image, secondary ? blueButton : greenButton, Image.Type.Simple);
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 24f;
            label.fontSizeMax = 58f;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.color = Color.white;
            label.fontSharedMaterial = buttonLabelMaterial;
        }
    }

    private static void ApplyApprovedButtonThemeToPrefab(string prefabPath)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            ApplyApprovedButtonTheme(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsSpecificButtonTheme()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        Sprite greenButton = RequireSprite(SettingsGreenButtonPath);
        Sprite blueButton = RequireSprite(BlueButtonPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            ConfigureSettingsButton(frame, "GamePauseGroup/BackButton", greenButton, new Color(0.9f, 1f, 0.75f, 1f));
            ConfigureSettingsButton(frame, "GamePauseGroup/ContinueBtn", blueButton, new Color(0.18f, 0.62f, 0.95f, 1f));
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureSettingsButton(RectTransform frame, string path, Sprite sprite, Color tint)
    {
        RectTransform button = RequireRect(frame.Find(path), frame.name + "/" + path);
        Image image = button.GetComponent<Image>();
        ConfigureImage(image, sprite, Image.Type.Simple);
        image.color = tint;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
        {
            return;
        }

        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 28f;
        label.fontSizeMax = 58f;
        label.enableWordWrapping = false;
        label.color = Color.white;
        label.fontSharedMaterial = RequireMaterial(ButtonLabelMaterialPath);
    }

    private static void ApplySettingsMeasuredCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/SettingPanel/PausePanel.prefab";
        Sprite cardSprite = RequireSprite(DailyTaskCardPath);
        Sprite goldCircle = RequireSprite(SettingsGoldCirclePath);
        Sprite musicTarget = RequireSprite(SettingsMusicTargetPath);
        Sprite soundTarget = RequireSprite(SettingsSoundTargetPath);
        Sprite vibrateTarget = RequireSprite(SettingsVibrateTargetPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            // Extend only the lower edge to match the approved effect. Header
            // children are compensated below so the crest/ribbon do not move.
            frame.sizeDelta = new Vector2(1090f, 1205f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 76f);

            SetSecondaryHeaderGeometry(frame, 880f, 400f, 461f, 589f, 490f);
            RequireRect(frame.Find("ApprovedSecondaryRibbon"), prefabPath + "/ApprovedSecondaryRibbon").sizeDelta = new Vector2(880f, 288f);
            RequireRect(frame.Find("ApprovedSecondaryCrest"), prefabPath + "/ApprovedSecondaryCrest").sizeDelta = new Vector2(400f, 190f);

            RectTransform contentCard = RequireRect(FindDirectChild(frame, "Image (2)", true), prefabPath + "/BG (1)/Image (2) [active]");
            contentCard.anchoredPosition = new Vector2(contentCard.anchoredPosition.x, 89f);
            contentCard.sizeDelta = new Vector2(862f, 346f);
            ConfigureImage(contentCard.GetComponent<Image>(), cardSprite, Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = new Color32(255, 230, 150, 255);

            foreach (string groupPath in new[] { "MusicGroup", "SoundGroup", "LibGroup" })
            {
                RectTransform group = RequireRect(frame.Find(groupPath), prefabPath + "/BG (1)/" + groupPath);
                group.anchoredPosition = new Vector2(group.anchoredPosition.x, 89f);
                EnsureDecorativeImage(group, "ApprovedGoldCircle", goldCircle, Vector2.zero, new Vector2(176f, 176f));
                Transform circle = group.Find("ApprovedGoldCircle");
                if (circle != null)
                {
                    circle.SetAsFirstSibling();
                }
            }

            ApplySettingsStateIcon(frame, "MusicGroup/Button/MusicOn", musicTarget);
            ApplySettingsStateIcon(frame, "MusicGroup/Button/MusicOff", musicTarget, new Color32(150, 160, 175, 255));
            ApplySettingsStateIcon(frame, "SoundGroup/Button (1)/SoundOn", soundTarget);
            ApplySettingsStateIcon(frame, "SoundGroup/Button (1)/SoundOff", soundTarget, new Color32(150, 160, 175, 255));
            ApplySettingsStateIcon(frame, "LibGroup/Button (2)/LibOn", vibrateTarget);
            ApplySettingsStateIcon(frame, "LibGroup/Button (2)/LIboff", vibrateTarget, new Color32(150, 160, 175, 255));

            foreach (string buttonPath in new[] { "GamePauseGroup/BackButton", "GamePauseGroup/ContinueBtn" })
            {
                RectTransform button = RequireRect(frame.Find(buttonPath), prefabPath + "/BG (1)/" + buttonPath);
                button.anchoredPosition = new Vector2(button.anchoredPosition.x, -118f);
                button.sizeDelta = new Vector2(410f, 175f);
            }

            RectTransform version = RequireRect(frame.Find("VersionLog"), prefabPath + "/BG (1)/VersionLog");
            version.anchoredPosition = new Vector2(version.anchoredPosition.x, 102f);

            RectTransform close = RequireRect(frame.Find("CloseButton"), prefabPath + "/BG (1)/CloseButton");
            // The approved close control sits on the frame's upper-right corner,
            // not beside the currency bar above the modal.
            close.anchoredPosition = new Vector2(-130f, -50f);

            TMP_Text approvedTitle = RequireRect(frame.Find("ApprovedSecondaryTitle"), prefabPath + "/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.fontSizeMax = 60f;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTasksMeasuredCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab";
        Sprite closeTarget = RequireSprite(DailyTasksCloseTargetPath);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG/BG"), prefabPath + "/BG/BG");
            frame.sizeDelta = new Vector2(1076f, 1636f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, -66f);

            EnsureHeaderCover(frame, 1076f, 1636f);
            // Daily Tasks deliberately has the widest banner in the approved set.
            // Header geometry must be applied after the cover so the cover cannot
            // obscure the ribbon, crest, or title.
            SetSecondaryHeaderGeometry(frame, 950f, 650f, 670f, 855f, 705f);
            RectTransform approvedRibbon = RequireRect(frame.Find("ApprovedSecondaryRibbon"), prefabPath + "/ApprovedSecondaryRibbon");
            approvedRibbon.sizeDelta = new Vector2(950f, 421f);
            approvedRibbon.GetComponent<Image>().preserveAspect = false;
            RectTransform approvedCrest = RequireRect(frame.Find("ApprovedSecondaryCrest"), prefabPath + "/ApprovedSecondaryCrest");
            approvedCrest.sizeDelta = new Vector2(650f, 260f);
            approvedCrest.GetComponent<Image>().preserveAspect = false;
            TMP_Text approvedTitle = RequireRect(frame.Find("ApprovedSecondaryTitle"), prefabPath + "/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.sizeDelta = new Vector2(760f, 140f);
            approvedTitle.fontSizeMax = 80f;
            approvedTitle.rectTransform.localScale = new Vector3(1f, 1.35f, 1f);

            RectTransform content = RequireRect(root.transform.Find("Content"), prefabPath + "/Content");
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 210f);

            RectTransform bottom = RequireRect(root.transform.Find("Content/DailyTask/Bottom"), prefabPath + "/Content/DailyTask/Bottom");
            bottom.anchoredPosition = new Vector2(bottom.anchoredPosition.x, -187f);
            bottom.sizeDelta = new Vector2(bottom.sizeDelta.x, 86f);

            RectTransform taskRoot = RequireRect(root.transform.Find("Content/DailyTask/Bottom/Scroll View/Viewport/Content"), prefabPath + "/dailyTaskRoot");
            GridLayoutGroup layout = taskRoot.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                throw new InvalidOperationException("Daily task root has no GridLayoutGroup: " + prefabPath);
            }
            layout.cellSize = new Vector2(833f, 225f);
            layout.spacing = new Vector2(0f, 20f);

            RectTransform close = RequireRect(root.transform.Find("CloseBtn"), prefabPath + "/CloseBtn");
            close.anchoredPosition = new Vector2(414f, 704f);
            Image closeBackground = close.GetComponent<Image>();
            if (closeBackground != null)
            {
                closeBackground.color = new Color32(255, 255, 255, 0);
            }
            RectTransform closeIcon = RequireRect(close.Find("Image (2)"), prefabPath + "/CloseBtn/Image (2)");
            closeIcon.sizeDelta = new Vector2(122f, 122f);
            Image closeImage = closeIcon.GetComponent<Image>();
            ConfigureImage(closeImage, closeTarget, Image.Type.Simple);
            closeImage.preserveAspect = true;

            ApplyDailyTaskElementMeasuredCalibration();

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySettingsStateIcon(RectTransform frame, string statePath, Sprite targetSprite, Color32? tint = null)
    {
        RectTransform state = RequireRect(frame.Find(statePath), frame.name + "/" + statePath);
        Image stateBackground = state.GetComponent<Image>();
        if (stateBackground != null)
        {
            stateBackground.color = new Color32(255, 255, 255, 0);
        }

        RectTransform iconRect = RequireRect(state.Find("Image (2)"), frame.name + "/" + statePath + "/Image (2)");
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(226f, 226f);
        Image icon = iconRect.GetComponent<Image>();
        ConfigureImage(icon, targetSprite, Image.Type.Simple);
        icon.preserveAspect = true;
        icon.color = tint ?? new Color32(255, 255, 255, 255);
    }

    private static void ApplyItemMeasuredCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/AddPropPanel/AddPropPanel.prefab";
        Sprite cardSprite = RequireSprite(SecondaryContentCardPath);
        Sprite goldCircle = RequireSprite(SettingsGoldCirclePath);
        Sprite closeTarget = RequireSprite(ItemCloseTargetPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform frame = RequireRect(root.transform.Find("BG (1)"), prefabPath + "/BG (1)");
            frame.sizeDelta = new Vector2(1080f, 1200f);
            frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, 105f);
            SetSecondaryHeaderGeometry(frame, 864f, 450f, 408f, 530f, 428f);
            RectTransform approvedCrestRect = RequireRect(frame.Find("ApprovedSecondaryCrest"), prefabPath + "/ApprovedSecondaryCrest");
            approvedCrestRect.sizeDelta = new Vector2(450f, 210f);
            approvedCrestRect.GetComponent<Image>().preserveAspect = false;
            TMP_Text approvedTitle = RequireRect(frame.Find("ApprovedSecondaryTitle"), prefabPath + "/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.fontSizeMax = 72f;

            RectTransform contentCard = RequireRect(FindDirectChild(frame, "Image (2)", true), prefabPath + "/BG (1)/Image (2) [active]");
            contentCard.anchoredPosition = new Vector2(contentCard.anchoredPosition.x, 69f);
            contentCard.sizeDelta = new Vector2(825f, 500f);
            ConfigureImage(contentCard.GetComponent<Image>(), cardSprite, Image.Type.Sliced);
            contentCard.GetComponent<Image>().color = new Color32(255, 242, 210, 255);

            RectTransform propIcon = RequireRect(root.transform.Find("Content/PropIcon"), prefabPath + "/Content/PropIcon");
            propIcon.anchoredPosition = new Vector2(propIcon.anchoredPosition.x, 163f);
            propIcon.sizeDelta = new Vector2(360f, 360f);
            RectTransform propName = RequireRect(root.transform.Find("Content/PropName"), prefabPath + "/Content/PropName");
            propName.anchoredPosition = new Vector2(propName.anchoredPosition.x, -95f);
            TMP_Text propNameText = propName.GetComponent<TMP_Text>();
            if (propNameText != null)
            {
                propNameText.color = new Color32(75, 38, 24, 255);
            }
            RectTransform reward = RequireRect(root.transform.Find("Content/BtnGroup/ButtonAnim/Pop"), prefabPath + "/Content/BtnGroup/ButtonAnim/Pop");
            reward.anchoredPosition = new Vector2(reward.anchoredPosition.x, 189f);
            RectTransform limitation = RequireRect(root.transform.Find("Content/Limination"), prefabPath + "/Content/Limination");
            limitation.anchoredPosition = new Vector2(limitation.anchoredPosition.x, -492f);

            RectTransform close = RequireRect(root.transform.Find("Content/CloseBtn"), prefabPath + "/Content/CloseBtn");
            close.anchoredPosition = new Vector2(397f, 614f);
            EnsureDecorativeImage(close, "ApprovedGoldCircle", goldCircle, Vector2.zero, new Vector2(170f, 170f));
            Transform circle = close.Find("ApprovedGoldCircle");
            if (circle != null)
            {
                circle.gameObject.SetActive(false);
            }
            Image closeBackground = close.GetComponent<Image>();
            if (closeBackground != null)
            {
                closeBackground.color = new Color32(255, 255, 255, 0);
            }
            RectTransform closeIcon = RequireRect(close.Find("Image (3)"), prefabPath + "/Content/CloseBtn/Image (3)");
            closeIcon.sizeDelta = new Vector2(110f, 110f);
            Image closeImage = closeIcon.GetComponent<Image>();
            ConfigureImage(closeImage, closeTarget, Image.Type.Simple);
            closeImage.preserveAspect = true;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealWithdrawMeasuredCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/RealWithdrawPanel.prefab";
        Sprite frameSprite = RequireSprite(SecondaryPanelFramePath);
        Sprite amountCardSprite = RequireSprite(AmountCardPath);
        Sprite goldCardSprite = RequireSprite(SecondaryContentCardPath);
        Sprite primaryButtonSprite = RequireSprite(PrimaryButtonPath);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RectTransform header = RequireRect(root.transform.Find("Title"), prefabPath + "/Title");
            RectTransform ribbon = RequireRect(header.Find("ApprovedSecondaryRibbon"), prefabPath + "/Title/ApprovedSecondaryRibbon");
            // Keep the full-page badge inside the safe top area. The previous
            // measured pass placed the ribbon/title too high and cropped them
            // at the top edge of the 1080x2400 runtime capture.
            ribbon.anchoredPosition = new Vector2(0f, -132f);
            ribbon.sizeDelta = new Vector2(800f, 267f);
            ribbon.GetComponent<Image>().preserveAspect = true;

            RectTransform crest = RequireRect(header.Find("ApprovedSecondaryCrest"), prefabPath + "/Title/ApprovedSecondaryCrest");
            crest.anchoredPosition = new Vector2(0f, -14f);
            crest.sizeDelta = new Vector2(440f, 220f);
            crest.GetComponent<Image>().preserveAspect = true;

            TMP_Text approvedTitle = RequireRect(header.Find("ApprovedSecondaryTitle"), prefabPath + "/Title/ApprovedSecondaryTitle").GetComponent<TMP_Text>();
            approvedTitle.rectTransform.anchoredPosition = new Vector2(0f, -120f);
            approvedTitle.rectTransform.sizeDelta = new Vector2(600f, 110f);
            approvedTitle.fontSizeMin = 28f;
            approvedTitle.fontSizeMax = 70f;
            approvedTitle.alignment = TextAlignmentOptions.Center;
            approvedTitle.fontSharedMaterial = RequireMaterial(ButtonLabelMaterialPath);

            RectTransform withdrawFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/bg"), prefabPath + "/WithdrawInfo/bg");
            withdrawFrame.anchoredPosition = new Vector2(withdrawFrame.anchoredPosition.x, -42f);
            withdrawFrame.sizeDelta = new Vector2(1068f, 1240f);
            ConfigureImage(withdrawFrame.GetComponent<Image>(), frameSprite, Image.Type.Sliced);
            withdrawFrame.GetComponent<Image>().color = Color.white;
            Transform obsoleteWarmFloor = withdrawFrame.Find("ApprovedWithdrawWarmFloor");
            if (obsoleteWarmFloor != null)
            {
                UnityEngine.Object.DestroyImmediate(obsoleteWarmFloor.gameObject);
            }

            RectTransform amountFrame = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/CoinInfo/CoinInfoFrame"), prefabPath + "/CoinInfo/CoinInfoFrame");
            ConfigureImage(amountFrame.GetComponent<Image>(), goldCardSprite, Image.Type.Sliced);
            amountFrame.GetComponent<Image>().color = new Color32(255, 242, 210, 255);

            Image pageBackground = root.transform.Find("BG")?.GetComponent<Image>();
            if (pageBackground != null)
            {
                pageBackground.color = new Color32(33, 153, 225, 255);
            }

            Image topBackground = root.transform.Find("Title/Image (1)")?.GetComponent<Image>();
            if (topBackground != null)
            {
                topBackground.color = new Color32(170, 205, 245, 255);
            }

            RectTransform withdrawButton = RequireRect(root.transform.Find("Content/Scroll View/Viewport/Content/WithdrawInfo/WithdrawBtn/Btn"), prefabPath + "/WithdrawBtn/Btn");
            ConfigureImage(withdrawButton.GetComponent<Image>(), primaryButtonSprite, Image.Type.Simple);

            RealWithdrawPanel controller = root.GetComponent<RealWithdrawPanel>();
            if (controller != null)
            {
                controller.normalSprite = primaryButtonSprite;
                controller.canWithdrawSprite = primaryButtonSprite;
                ConfigureImage(controller.btn_Image, primaryButtonSprite, Image.Type.Simple);

                RectTransform methodTemplate = controller.withdrawWayItem != null
                    ? controller.withdrawWayItem.transform as RectTransform
                    : null;
                if (methodTemplate != null)
                {
                    methodTemplate.sizeDelta = new Vector2(430f, 205f);
                    Image methodBackground = methodTemplate.GetComponent<Image>();
                    if (methodBackground != null)
                    {
                        ConfigureImage(methodBackground, amountCardSprite, Image.Type.Sliced);
                        methodBackground.color = Color.white;
                    }
                    if (controller.withdrawWayItem.payIcon != null)
                    {
                        controller.withdrawWayItem.payIcon.preserveAspect = true;
                    }
                }
            }

            ribbon.SetAsLastSibling();
            crest.SetAsLastSibling();
            approvedTitle.transform.SetAsLastSibling();
            BringCloseControlsForward(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawLevelPaletteCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/Real/UI/RealWithdrawalPanl/WithdrawLevelItem.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image shadow = root.transform.Find("Shadow")?.GetComponent<Image>();
            if (shadow != null)
            {
                shadow.color = new Color32(160, 108, 45, 92);
            }

            Image selectShadow = root.transform.Find("SelectShadow")?.GetComponent<Image>();
            if (selectShadow != null)
            {
                selectShadow.color = new Color32(160, 108, 45, 72);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyTaskElementMeasuredCalibration()
    {
        const string prefabPath = "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Image card = root.GetComponent<Image>();
            if (card == null)
            {
                throw new InvalidOperationException("Daily task element root has no Image: " + prefabPath);
            }
            // Keep the source card's own yellow/cream balance. A warm tint was
            // multiplying away too much blue and made PlayMode substantially
            // more orange than the approved target.
            card.color = Color.white;

            TMP_Text description = root.transform.Find("txtDesc")?.GetComponent<TMP_Text>();
            if (description != null)
            {
                description.color = new Color32(112, 48, 24, 255);
                description.enableAutoSizing = false;
                description.fontSize = 38f;
            }

            RectTransform progress = root.transform.Find("BarBG") as RectTransform;
            if (progress != null)
            {
                Image progressImage = progress.GetComponent<Image>();
                if (progressImage != null)
                {
                    progressImage.color = new Color32(30, 150, 240, 255);
                }

                TMP_Text progressText = progress.Find("Text (TMP)")?.GetComponent<TMP_Text>();
                if (progressText != null)
                {
                    progressText.enableAutoSizing = false;
                    progressText.fontSize = 30f;
                }
            }

            RectTransform rewardState = root.transform.Find("State") as RectTransform;
            if (rewardState != null)
            {
                rewardState.anchoredPosition = new Vector2(-131f, rewardState.anchoredPosition.y);
                rewardState.sizeDelta = new Vector2(200f, 200f);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static Transform FindDirectChild(Transform parent, string childName, bool preferActive)
    {
        Transform fallback = null;
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (!string.Equals(child.name, childName, StringComparison.Ordinal))
            {
                continue;
            }

            if (fallback == null)
            {
                fallback = child;
            }
            if (!preferActive || child.gameObject.activeSelf)
            {
                return child;
            }
        }
        return fallback;
    }

    private static Transform FindDirectChildByActiveState(Transform parent, string childName, bool activeSelf)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (string.Equals(child.name, childName, StringComparison.Ordinal) &&
                child.gameObject.activeSelf == activeSelf)
            {
                return child;
            }
        }
        return null;
    }

    private static Transform FindDirectChildBySpriteName(Transform parent, string childName, string spriteName)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (!string.Equals(child.name, childName, StringComparison.Ordinal))
            {
                continue;
            }

            Image image = child.GetComponent<Image>();
            if (image != null && image.sprite != null &&
                string.Equals(image.sprite.name, spriteName, StringComparison.Ordinal))
            {
                return child;
            }
        }
        return null;
    }

    private static Transform FindDirectChildByApproximateSize(
        Transform parent,
        string childName,
        Vector2 expectedSize)
    {
        Transform best = null;
        float bestDistance = float.MaxValue;
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (!string.Equals(child.name, childName, StringComparison.Ordinal) ||
                child.GetComponent<Image>() == null ||
                !(child is RectTransform rect))
            {
                continue;
            }

            float distance = Vector2.Distance(rect.sizeDelta, expectedSize);
            if (distance < bestDistance)
            {
                best = child;
                bestDistance = distance;
            }
        }
        return bestDistance <= 80f ? best : null;
    }

    private static void SetSecondaryHeaderGeometry(
        RectTransform frame,
        float ribbonWidth,
        float crestWidth,
        float ribbonY,
        float crestY,
        float titleY)
    {
        RectTransform ribbon = RequireRect(frame.Find("ApprovedSecondaryRibbon"), frame.name + "/ApprovedSecondaryRibbon");
        ribbon.anchoredPosition = new Vector2(0f, ribbonY);
        ribbon.sizeDelta = new Vector2(ribbonWidth, ribbonWidth * 0.3334f);

        RectTransform crest = RequireRect(frame.Find("ApprovedSecondaryCrest"), frame.name + "/ApprovedSecondaryCrest");
        crest.anchoredPosition = new Vector2(0f, crestY);
        crest.sizeDelta = new Vector2(crestWidth, crestWidth * 0.5f);

        RectTransform title = RequireRect(frame.Find("ApprovedSecondaryTitle"), frame.name + "/ApprovedSecondaryTitle");
        title.anchoredPosition = new Vector2(0f, titleY);
        title.sizeDelta = new Vector2(ribbonWidth * 0.72f, 118f);

        ribbon.SetAsLastSibling();
        crest.SetAsLastSibling();
        title.SetAsLastSibling();
    }

    private static void BringCloseControlsForward(GameObject root)
    {
        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            string objectName = image.gameObject.name.ToLowerInvariant();
            string spriteName = image.sprite != null ? image.sprite.name.ToLowerInvariant() : string.Empty;
            if (objectName.Contains("close") || spriteName.Contains("close"))
            {
                image.transform.SetAsLastSibling();
            }
        }
    }

    private static void HideTextGraphic(Transform textTransform)
    {
        if (textTransform == null)
        {
            return;
        }

        TMP_Text tmp = textTransform.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            Color color = tmp.color;
            tmp.color = new Color(color.r, color.g, color.b, 0f);
        }

        Text text = textTransform.GetComponent<Text>();
        if (text != null)
        {
            Color color = text.color;
            text.color = new Color(color.r, color.g, color.b, 0f);
        }
    }

    private static void ApplySlotFaqVisuals()
    {
        Sprite frameSprite = RequireSprite(CommonFramePath);
        Sprite titleSprite = RequireSprite(CommonTitlePath);
        Sprite greenButtonSprite = RequireSprite(PrimaryButtonPath);
        Sprite gameplayBackground = RequireSprite(GameplayBackgroundPath);

        GameObject root = PrefabUtility.LoadPrefabContents(SlotFaqPanelPrefab);
        try
        {
            ConfigureImage(root.transform.Find("bg")?.GetComponent<Image>(), gameplayBackground, Image.Type.Simple);

            RectTransform content = RequireRect(root.transform.Find("Content"), "SlotFAQ/Content");
            RectTransform titleBar = RequireRect(content.Find("bg2"), "SlotFAQ title bar");
            titleBar.anchoredPosition = new Vector2(0f, 850f);
            titleBar.sizeDelta = new Vector2(856f, 210f);
            ConfigureImage(titleBar.GetComponent<Image>(), titleSprite, Image.Type.Sliced);

            TMP_Text textSource = content.Find("des")?.GetComponent<TMP_Text>();
            TMP_Text title = EnsurePlainTMP(content, textSource, "ApprovedTitle", "SLOT REWARDS");
            title.rectTransform.anchoredPosition = new Vector2(0f, 850f);
            title.rectTransform.sizeDelta = new Vector2(740f, 120f);
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 40f;
            title.fontSizeMax = 68f;
            title.color = Color.white;

            RectTransform rewardFrame = RequireRect(content.Find("bg2 (1)"), "SlotFAQ reward frame");
            rewardFrame.anchoredPosition = new Vector2(0f, 170f);
            rewardFrame.sizeDelta = new Vector2(946f, 1180f);
            ConfigureImage(rewardFrame.GetComponent<Image>(), frameSprite, Image.Type.Sliced);
            SetActiveIfFound(root.transform, "Content/SlotMachineGroup", false);

            if (textSource != null)
            {
                textSource.rectTransform.anchoredPosition = new Vector2(0f, -245f);
                textSource.rectTransform.sizeDelta = new Vector2(760f, 150f);
                textSource.alignment = TextAlignmentOptions.Center;
                textSource.enableAutoSizing = true;
                textSource.fontSizeMin = 22f;
                textSource.fontSizeMax = 30f;
                textSource.color = new Color32(92, 54, 54, 255);
            }

            RectTransform okButton = RequireRect(content.Find("Btn"), "SlotFAQ OK button");
            okButton.anchoredPosition = new Vector2(0f, -650f);
            okButton.sizeDelta = new Vector2(520f, 160f);
            ConfigureImage(okButton.GetComponent<Image>(), greenButtonSprite, Image.Type.Sliced);
            Transform oldOkGraphic = okButton.Find("Image (1)");
            if (oldOkGraphic != null)
            {
                oldOkGraphic.gameObject.SetActive(false);
            }
            TMP_Text okText = EnsurePlainTMP(okButton, textSource, "ApprovedLabel", "OK");
            StretchLabel(okText.rectTransform, new Vector2(380f, 100f));
            okText.alignment = TextAlignmentOptions.Center;
            okText.enableAutoSizing = true;
            okText.fontSizeMin = 38f;
            okText.fontSizeMax = 62f;
            okText.color = Color.white;

            ReplaceSlotElementSprites(root);
            PrefabUtility.SaveAsPrefabAsset(root, SlotFaqPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplySlotPanelVisuals()
    {
        Sprite frameSprite = RequireSprite(CommonFramePath);
        Sprite greenButtonSprite = RequireSprite(PrimaryButtonPath);
        Sprite gameplayBackground = RequireSprite(GameplayBackgroundPath);

        GameObject root = PrefabUtility.LoadPrefabContents(SlotPanelPrefab);
        try
        {
            ConfigureImage(root.transform.Find("bg")?.GetComponent<Image>(), gameplayBackground, Image.Type.Simple);

            RectTransform machine = RequireRect(root.transform.Find("Content/SlotMachineGroup/Buttom"), "SlotPanel machine frame");
            machine.anchoredPosition = new Vector2(0f, -34f);
            machine.sizeDelta = new Vector2(946f, 1500f);
            ConfigureImage(machine.GetComponent<Image>(), frameSprite, Image.Type.Sliced);

            RectTransform spinButton = RequireRect(root.transform.Find("Content/SlotMachineGroup/Btn"), "SlotPanel spin button");
            spinButton.anchoredPosition = new Vector2(0f, -210f);
            spinButton.sizeDelta = new Vector2(600f, 170f);
            ConfigureImage(spinButton.GetComponent<Image>(), greenButtonSprite, Image.Type.Sliced);

            TMP_Text hint = root.transform.Find("Content/Hint")?.GetComponent<TMP_Text>();
            TMP_Text title = EnsurePlainTMP(RequireRect(root.transform.Find("Content"), "SlotPanel/Content"), hint, "ApprovedTitle", "BONUS SPIN");
            title.rectTransform.anchoredPosition = new Vector2(0f, 525f);
            title.rectTransform.sizeDelta = new Vector2(760f, 120f);
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 42f;
            title.fontSizeMax = 72f;
            title.color = Color.white;

            if (hint != null)
            {
                hint.rectTransform.anchoredPosition = new Vector2(0f, -490f);
                hint.alignment = TextAlignmentOptions.Center;
                hint.color = new Color32(92, 54, 54, 255);
            }

            ReplaceSlotElementSprites(root);
            PrefabUtility.SaveAsPrefabAsset(root, SlotPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ReplaceSlotElementSprites(GameObject root)
    {
        // Slot screens must keep the actual reel symbols.  A previous visual
        // pass replaced these with the match-game colour blocks, which made
        // the reward table describe a different game.
        Sprite[] slotSymbols =
        {
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement1.png"),
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement2.png"),
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement3.png"),
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement4.png"),
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement5.png"),
            RequireSprite("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement6.png")
        };

        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.sprite == null)
            {
                continue;
            }

            string spriteName = image.sprite.name;
            if (spriteName.StartsWith("Icon_SlotElement", StringComparison.Ordinal))
            {
                string suffix = spriteName.Substring("Icon_SlotElement".Length);
                if (int.TryParse(suffix, out int index) && index >= 1 && index <= slotSymbols.Length)
                {
                    image.sprite = slotSymbols[index - 1];
                    image.preserveAspect = true;
                    image.color = Color.white;
                }
            }
        }
    }

    private static void ApplyGetRewardVisuals()
    {
        Sprite frameSprite = RequireSprite(RewardFramePath);
        Sprite greenButtonSprite = RequireSprite(PrimaryButtonPath);
        Sprite blueButtonSprite = RequireSprite(BlueButtonPath);
        Sprite rewardStarSprite = RequireSprite(RewardStarPath);
        Sprite rewardValueSprite = RequireSprite(BlueButtonPath);
        Sprite rewardCardSprite = RequireSprite(CardPath);
        Sprite ribbonSprite = RequireSprite(SecondaryRibbonPath);
        Sprite crestSprite = RequireSprite(SecondaryCrestPath);
        Sprite levelPlateSprite = RequireSprite(CommonTitlePath);
        Sprite rewardLaurelGlowSprite = RequireSprite(RewardLaurelGlowPath);
        Sprite rewardLevelLeavesSprite = RequireSprite(RewardLevelLeavesPath);
        Sprite rewardCoinPileSprite = RequireSprite(RewardCoinPilePath);
        Sprite rewardCashStackSprite = RequireSprite(RewardCashStackPath);
        Material buttonLabelMaterial = RequireMaterial(ButtonLabelMaterialPath);

        GameObject root = PrefabUtility.LoadPrefabContents(GetRewardPanelPrefab);
        try
        {
            GetRewardPanel controller = root.GetComponent<GetRewardPanel>();
            if (controller == null)
            {
                throw new InvalidOperationException("GetRewardPanel component is missing from its prefab root.");
            }

            RectTransform panel = RequireRect(root.transform.Find("BG"), "GetRewardPanel/BG");
            // Match the approved vertical composition: the frame sits lower and
            // reaches almost to the bottom, while the header remains stationary.
            panel.anchoredPosition = new Vector2(0f, -50f);
            panel.sizeDelta = new Vector2(1000f, 1682f);
            EnsureBackgroundImage(panel, "ApprovedFrame", frameSprite);
            Image unusedPanelImage = panel.GetComponent<Image>();
            if (unusedPanelImage != null)
            {
                unusedPanelImage.enabled = false;
            }

            Transform titleBar = FindChildBySprite(panel, "Common_Title");
            if (titleBar != null)
            {
                // The ribbon is the only title base in the approved effect.
                titleBar.gameObject.SetActive(false);
            }

            TMP_Text title = panel.Find("Text (TMP)")?.GetComponent<TMP_Text>();
            if (title != null)
            {
                TMP_Text fixedTitle = EnsurePlainTMP(panel, title, "ApprovedTitle", "LEVEL COMPLETE");
                fixedTitle.rectTransform.anchoredPosition = new Vector2(0f, 770f);
                fixedTitle.rectTransform.sizeDelta = new Vector2(760f, 130f);
                fixedTitle.alignment = TextAlignmentOptions.Center;
                fixedTitle.enableAutoSizing = true;
                fixedTitle.fontSizeMin = 42f;
                fixedTitle.fontSizeMax = 68f;
                fixedTitle.color = Color.white;
                title.color = new Color(title.color.r, title.color.g, title.color.b, 0f);
            }
            Transform legacyApprovedTitle = panel.Find("ApprovedTitle");
            if (legacyApprovedTitle != null)
            {
                legacyApprovedTitle.gameObject.SetActive(false);
            }

            EnsureHeaderCover(panel, 1000f, 1760f);
            RectTransform headerCover = RequireRect(panel.Find("ApprovedSecondaryHeaderCover"), "GetRewardPanel/ApprovedSecondaryHeaderCover");
            headerCover.gameObject.SetActive(false);
            EnsureDecorativeImage(panel, "ApprovedSecondaryRibbon", ribbonSprite, new Vector2(0f, 820f), new Vector2(970f, 340f));
            RequireRect(panel.Find("ApprovedSecondaryRibbon"), "GetRewardPanel/ApprovedSecondaryRibbon").GetComponent<Image>().preserveAspect = true;
            EnsureDecorativeImage(panel, "ApprovedSecondaryCrest", crestSprite, new Vector2(0f, 984f), new Vector2(640f, 300f));
            RectTransform approvedSecondaryTitle = RequireRect(panel.Find("ApprovedSecondaryTitle"), "GetRewardPanel/ApprovedSecondaryTitle");
            approvedSecondaryTitle.anchoredPosition = new Vector2(0f, 840f);
            approvedSecondaryTitle.sizeDelta = new Vector2(840f, 140f);
            TMP_Text approvedSecondaryTitleText = approvedSecondaryTitle.GetComponent<TMP_Text>();
            approvedSecondaryTitleText.text = "LEVEL COMPLETE";
            approvedSecondaryTitleText.fontSizeMax = 82f;
            approvedSecondaryTitle.SetAsLastSibling();

            EnsureDecorativeImage(panel, "ApprovedRewardLaurelGlow", rewardLaurelGlowSprite, new Vector2(0f, 505f), new Vector2(840f, 500f));
            EnsureDecorativeImage(panel, "ApprovedRewardStar", rewardStarSprite, new Vector2(0f, 505f), new Vector2(370f, 370f));
            Transform rewardStar = panel.Find("ApprovedRewardStar");
            Transform rewardLaurelGlow = panel.Find("ApprovedRewardLaurelGlow");
            Transform contentLayer = panel.Find("Content");
            if (contentLayer != null)
            {
                int contentIndex = contentLayer.GetSiblingIndex();
                if (rewardLaurelGlow != null)
                {
                    rewardLaurelGlow.SetSiblingIndex(contentIndex);
                    contentIndex++;
                }
                if (rewardStar != null)
                {
                    rewardStar.SetSiblingIndex(contentIndex);
                }
            }

            RectTransform levelRect = controller.LevelObj != null ? controller.LevelObj.transform as RectTransform : null;
            EnsureDecorativeImage(panel, "ApprovedLevelLeaves", rewardLevelLeavesSprite, new Vector2(0f, 223f), new Vector2(700f, 200f));
            RectTransform levelLeaves = RequireRect(panel.Find("ApprovedLevelLeaves"), "GetRewardPanel/ApprovedLevelLeaves");
            levelLeaves.GetComponent<Image>().preserveAspect = false;
            levelLeaves.SetSiblingIndex(Mathf.Min(1, panel.childCount - 1));
            if (levelRect != null)
            {
                levelRect.gameObject.SetActive(true);
                levelRect.anchoredPosition = new Vector2(0f, 208f);
                levelRect.sizeDelta = new Vector2(640f, 180f);
                ConfigureImage(levelRect.GetComponent<Image>(), levelPlateSprite, Image.Type.Sliced);
                levelRect.GetComponent<Image>().color = new Color(0.52f, 0.76f, 1f, 1f);
                TMP_Text levelText = controller.levelTxt != null ? controller.levelTxt : levelRect.GetComponentInChildren<TMP_Text>(true);
                if (levelText != null)
                {
                    levelText.rectTransform.anchoredPosition = Vector2.zero;
                    levelText.rectTransform.sizeDelta = new Vector2(450f, 125f);
                    levelText.alignment = TextAlignmentOptions.Center;
                    levelText.enableAutoSizing = true;
                    levelText.fontSizeMin = 34f;
                    levelText.fontSizeMax = 66f;
                    levelText.color = Color.white;
                    levelText.fontSharedMaterial = buttonLabelMaterial;
                }
            }

            EnsureDecorativeImage(panel, "ApprovedRewardCard", rewardCardSprite, new Vector2(0f, -117f), new Vector2(800f, 488f));
            Transform rewardCard = panel.Find("ApprovedRewardCard");
            ConfigureImage(rewardCard.GetComponent<Image>(), rewardCardSprite, Image.Type.Sliced);
            rewardCard.GetComponent<Image>().raycastTarget = false;
            if (rewardCard != null && contentLayer != null)
            {
                rewardCard.SetSiblingIndex(contentLayer.GetSiblingIndex());
            }
            EnsureSolidImage(panel, "ApprovedRewardDivider", new Vector2(-6f, -180f), new Vector2(3f, 366f), new Color(0.89f, 0.70f, 0.35f, 0.62f));
            Transform rewardDivider = panel.Find("ApprovedRewardDivider");
            if (rewardDivider != null && contentLayer != null)
            {
                rewardDivider.SetSiblingIndex(contentLayer.GetSiblingIndex());
            }

            RectTransform rewards = root.transform.Find("BG/Content/Rewards") as RectTransform;
            if (rewards != null)
            {
                rewards.anchoredPosition = new Vector2(0f, -45f);
                SetActiveIfFound(root.transform, "BG/Content/Rewards/Reward_2", true);
                SetActiveIfFound(root.transform, "BG/Content/Rewards/Reward_2/Rewards/Reward_2", true);
                foreach (Image rewardImage in rewards.GetComponentsInChildren<Image>(true))
                {
                    if (rewardImage.sprite == null ||
                        (rewardImage.sprite.name != "XCXX_HDJL_BG_4" &&
                         rewardImage.sprite.name != "Bg_RewardMax" &&
                         rewardImage.sprite.name != "Secondary_BlueButton_v1" &&
                         rewardImage.sprite.name != rewardValueSprite.name))
                    {
                        continue;
                    }

                    ConfigureImage(rewardImage, rewardValueSprite, Image.Type.Simple);
                    rewardImage.color = new Color(0.18f, 0.58f, 0.92f, 1f);
                    RectTransform rewardValueRect = rewardImage.rectTransform;
                    if (rewardImage.name == "Image_Title_1")
                    {
                        rewardValueRect.anchoredPosition = new Vector2(49f, -230f);
                    }
                    else if (rewardImage.name == "Image_Title_2")
                    {
                        rewardValueRect.anchoredPosition = new Vector2(-72f, -230f);
                    }
                    else
                    {
                        rewardValueRect.anchoredPosition = new Vector2(rewardValueRect.anchoredPosition.x, -230f);
                    }
                    TMP_Text rewardText = rewardImage.GetComponentInChildren<TMP_Text>(true);
                    if (rewardText != null)
                    {
                        rewardText.color = Color.white;
                        rewardText.alignment = TextAlignmentOptions.Center;
                        rewardText.fontSharedMaterial = buttonLabelMaterial;
                    }
                }

                RectTransform coinReward = root.transform.Find("BG/Content/Rewards/Reward_1/Rewards/Reward_1/Image_Coin_1") as RectTransform;
                if (coinReward != null)
                {
                    coinReward.sizeDelta = new Vector2(360f, 310f);
                    Image legacyCoinImage = coinReward.GetComponent<Image>();
                    if (legacyCoinImage != null)
                    {
                        legacyCoinImage.enabled = false;
                    }

                    // Keep the existing animated reward node and replace only
                    // its artwork with the approved dimensional star-coin pile.
                    // Older composed coin children are retained but hidden so
                    // applying this batch remains non-destructive and repeatable.
                    foreach (Transform child in coinReward)
                    {
                        if (child.name.StartsWith("ApprovedStarCoin_", StringComparison.Ordinal))
                        {
                            child.gameObject.SetActive(false);
                        }
                    }

                    EnsureDecorativeImage(coinReward, "ApprovedCoinPile", rewardCoinPileSprite, new Vector2(42f, 14f), new Vector2(400f, 322f));
                    RectTransform approvedCoinPile = RequireRect(coinReward.Find("ApprovedCoinPile"), GetRewardPanelPrefab + "/ApprovedCoinPile");
                    approvedCoinPile.localEulerAngles = Vector3.zero;
                    approvedCoinPile.GetComponent<Image>().preserveAspect = true;
                }
                RectTransform cashReward = root.transform.Find("BG/Content/Rewards/Reward_2/Rewards/Reward_2/Image_Coin_2") as RectTransform;
                if (cashReward != null)
                {
                    cashReward.sizeDelta = new Vector2(400f, 380f);
                    Image legacyCashImage = cashReward.GetComponent<Image>();
                    if (legacyCashImage != null)
                    {
                        legacyCashImage.enabled = false;
                    }
                    EnsureDecorativeImage(cashReward, "ApprovedCashStack", rewardCashStackSprite, new Vector2(-76f, -16f), new Vector2(400f, 380f));
                }
            }

            RectTransform claimButton = root.transform.Find("BG/Content/ButtonAnim/Button") as RectTransform;
            if (claimButton != null)
            {
                claimButton.anchoredPosition = new Vector2(0f, -318f);
                claimButton.sizeDelta = new Vector2(590f, 190f);
                ConfigureImage(claimButton.GetComponent<Image>(), greenButtonSprite, Image.Type.Simple);

                TMP_Text claimText = claimButton.GetComponentInChildren<TMP_Text>(true);
                if (claimText != null)
                {
                    claimText.text = "CLAIM";
                    claimText.rectTransform.anchoredPosition = Vector2.zero;
                    claimText.rectTransform.sizeDelta = new Vector2(440f, 110f);
                    claimText.alignment = TextAlignmentOptions.Center;
                    claimText.enableAutoSizing = true;
                    claimText.fontSizeMin = 42f;
                    claimText.fontSizeMax = 80f;
                    claimText.fontSharedMaterial = buttonLabelMaterial;
                }

                Transform adIcon = claimButton.Find("Image (2)");
                if (adIcon != null)
                {
                    adIcon.gameObject.SetActive(false);
                }
            }

            SetActiveIfFound(root.transform, "BG/Content/Fake_WithdrawProgress", false);
            SetActiveIfFound(root.transform, "BG/Content/Real_WithdrawProgress", false);
            SetActiveIfFound(root.transform, "BG/Content/WathAdProgress", false);

            RectTransform noThank = root.transform.Find("BG/Content/DelayShow/NoThank") as RectTransform;
            if (noThank != null)
            {
                noThank.anchorMin = new Vector2(0.5f, 0.5f);
                noThank.anchorMax = new Vector2(0.5f, 0.5f);
                noThank.pivot = new Vector2(0.5f, 0.5f);
                noThank.anchoredPosition = new Vector2(0f, -223f);
                noThank.sizeDelta = new Vector2(378f, 108f);
                TMP_Text legacyContinue = noThank.Find("Text (TMP)")?.GetComponent<TMP_Text>();
                TMP_Text continueSource = legacyContinue != null
                    ? legacyContinue
                    : noThank.GetComponentInChildren<TMP_Text>(true);
                if (legacyContinue != null)
                {
                    // The controller rewrites this rich-text label at runtime,
                    // including an inline cash sprite. Keep it inactive and use
                    // the approved plain CONTINUE label so the two never overlap.
                    legacyContinue.gameObject.SetActive(false);
                }
                if (continueSource != null)
                {
                    TMP_Text approvedContinue = EnsurePlainTMP(noThank, continueSource, "ApprovedContinueLabel", "CONTINUE");
                    approvedContinue.rectTransform.anchoredPosition = Vector2.zero;
                    approvedContinue.rectTransform.sizeDelta = new Vector2(330f, 88f);
                    approvedContinue.alignment = TextAlignmentOptions.Center;
                    approvedContinue.enableAutoSizing = true;
                    approvedContinue.fontSizeMin = 28f;
                    approvedContinue.fontSizeMax = 48f;
                    approvedContinue.fontStyle = FontStyles.Bold;
                    approvedContinue.color = Color.white;
                    approvedContinue.fontSharedMaterial = buttonLabelMaterial;
                    approvedContinue.transform.SetAsLastSibling();
                }
                EnsureDecorativeImage(noThank, "ApprovedContinueBackground", blueButtonSprite, Vector2.zero, new Vector2(378f, 108f));
                Transform continueBackground = noThank.Find("ApprovedContinueBackground");
                if (continueBackground != null)
                {
                    continueBackground.GetComponent<Image>().color = new Color(0.18f, 0.58f, 0.92f, 1f);
                    continueBackground.GetComponent<Image>().preserveAspect = false;
                    continueBackground.SetAsFirstSibling();
                }

                Image legacyButtonBackground = noThank.GetComponent<Image>();
                if (legacyButtonBackground != null)
                {
                    legacyButtonBackground.enabled = false;
                }
            }

            foreach (ParticleSystem particle in root.GetComponentsInChildren<ParticleSystem>(true))
            {
                particle.gameObject.SetActive(false);
            }

            PrefabUtility.SaveAsPrefabAsset(root, GetRewardPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyDailyMissionButtonVisuals()
    {
        Sprite simpleGreenButton = RequireSprite(SimpleGreenButtonPath);

        GameObject root = PrefabUtility.LoadPrefabContents(DailyMissionPanelPrefab);
        try
        {
            RectTransform goButton = RequireRect(root.transform.Find("Content/GoBtn"), "DailyMissionPanel/Content/GoBtn");
            Transform buttonGraphic = FindChildBySprite(goButton, "Btn_GetReward");
            if (buttonGraphic == null)
            {
                throw new InvalidOperationException("DailyMissionPanel simple button graphic is missing.");
            }

            RectTransform graphicRect = RequireRect(buttonGraphic, "DailyMissionPanel GoBtn graphic");
            graphicRect.anchoredPosition = new Vector2(9.72f, 0f);
            graphicRect.sizeDelta = new Vector2(474.99f, 170.41f);
            ConfigureImage(buttonGraphic.GetComponent<Image>(), simpleGreenButton, Image.Type.Sliced);

            TMP_Text label = goButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;
                label.enableAutoSizing = true;
                label.fontSizeMin = 34f;
                label.fontSizeMax = 62f;
            }

            PrefabUtility.SaveAsPrefabAsset(root, DailyMissionPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyStarRatingTitleVisuals()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(StarRatingPanelPrefab);
        try
        {
            TMP_Text title = root.transform.Find("Content/Title")?.GetComponent<TMP_Text>();
            if (title == null)
            {
                throw new InvalidOperationException("StarRatingPopup title is missing.");
            }

            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 34f;
            title.fontSizeMax = 48f;
            title.enableWordWrapping = false;
            title.overflowMode = TextOverflowModes.Ellipsis;
            PrefabUtility.SaveAsPrefabAsset(root, StarRatingPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyWithdrawalConfirmTitleVisuals()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(WithdrawalConfirmPanelPrefab);
        try
        {
            TMP_Text title = root.transform.Find("Title/Text (TMP)")?.GetComponent<TMP_Text>();
            if (title == null)
            {
                throw new InvalidOperationException("Withdrawal confirmation title is missing.");
            }

            title.rectTransform.anchoredPosition = Vector2.zero;
            title.rectTransform.sizeDelta = new Vector2(720f, 130f);
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 22f;
            title.fontSizeMax = 50f;
            title.enableWordWrapping = false;
            title.overflowMode = TextOverflowModes.Ellipsis;
            PrefabUtility.SaveAsPrefabAsset(root, WithdrawalConfirmPanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyLoseVisuals()
    {
        Sprite frameSprite = RequireSprite(CommonFramePath);
        Sprite titleSprite = RequireSprite(CommonTitlePath);
        Sprite cardSprite = RequireSprite(CardPath);
        Sprite greenButtonSprite = RequireSprite(PrimaryButtonPath);
        Sprite bombSprite = RequireSprite(BombIconPath);
        Sprite swapSprite = RequireSprite(SwapIconPath);
        Sprite countBadgeSprite = RequireSprite(CountBadgePath);

        GameObject root = PrefabUtility.LoadPrefabContents(LosePanelPrefab);
        try
        {
            RectTransform panel = RequireRect(root.transform.Find("Window/Window"), "LosePanel/Window/Window");
            panel.anchoredPosition = Vector2.zero;
            panel.sizeDelta = new Vector2(946f, 1500f);
            ConfigurePanelImage(panel.gameObject, frameSprite);

            RectTransform titleBar = RequireRect(panel.Find("Tittle"), "LosePanel title");
            titleBar.anchoredPosition = new Vector2(0f, 585f);
            titleBar.sizeDelta = new Vector2(856f, 210f);
            ConfigureImage(titleBar.GetComponent<Image>(), titleSprite, Image.Type.Sliced);

            TMP_Text title = titleBar.GetComponentInChildren<TMP_Text>(true);
            if (title != null)
            {
                TMP_Text fixedTitle = EnsurePlainTMP(panel, title, "ApprovedTitle", "TRY AGAIN");
                fixedTitle.rectTransform.anchoredPosition = new Vector2(0f, 585f);
                fixedTitle.rectTransform.sizeDelta = new Vector2(730f, 120f);
                fixedTitle.alignment = TextAlignmentOptions.Center;
                fixedTitle.enableAutoSizing = true;
                fixedTitle.fontSizeMin = 38f;
                fixedTitle.fontSizeMax = 70f;
                fixedTitle.color = new Color32(255, 205, 48, 255);
                title.color = new Color(title.color.r, title.color.g, title.color.b, 0f);
            }

            TMP_Text subtitle = EnsurePlainTMP(panel, title, "ApprovedSubtitle", "You are almost there!");
            subtitle.rectTransform.anchoredPosition = new Vector2(0f, 390f);
            subtitle.rectTransform.sizeDelta = new Vector2(760f, 90f);
            subtitle.alignment = TextAlignmentOptions.Center;
            subtitle.enableAutoSizing = true;
            subtitle.fontSizeMin = 28f;
            subtitle.fontSizeMax = 48f;
            subtitle.color = new Color32(92, 54, 54, 255);

            RectTransform retry = RequireRect(panel.Find("RestartBtn"), "LosePanel retry button");
            retry.anchoredPosition = new Vector2(0f, 205f);
            retry.sizeDelta = new Vector2(620f, 190f);
            ConfigureImage(retry.GetComponent<Image>(), greenButtonSprite, Image.Type.Sliced);
            TMP_Text retryText = retry.GetComponentInChildren<TMP_Text>(true);
            if (retryText != null)
            {
                TMP_Text fixedRetryText = EnsurePlainTMP(panel, retryText, "ApprovedRetryLabel", "RETRY");
                fixedRetryText.rectTransform.anchoredPosition = new Vector2(0f, 205f);
                fixedRetryText.rectTransform.sizeDelta = new Vector2(500f, 120f);
                fixedRetryText.alignment = TextAlignmentOptions.Center;
                fixedRetryText.enableAutoSizing = true;
                fixedRetryText.fontSizeMin = 42f;
                fixedRetryText.fontSizeMax = 72f;
                fixedRetryText.color = Color.white;
                retryText.color = new Color(retryText.color.r, retryText.color.g, retryText.color.b, 0f);
            }

            TMP_Text boosterLabel = EnsurePlainTMP(panel, title, "ApprovedBoosterLabel", "Use a booster?");
            boosterLabel.rectTransform.anchoredPosition = new Vector2(0f, 45f);
            boosterLabel.rectTransform.sizeDelta = new Vector2(600f, 80f);
            boosterLabel.alignment = TextAlignmentOptions.Center;
            boosterLabel.enableAutoSizing = true;
            boosterLabel.fontSizeMin = 26f;
            boosterLabel.fontSizeMax = 42f;
            boosterLabel.color = new Color32(92, 54, 54, 255);

            RectTransform artCard = RequireRect(panel.Find("Image"), "LosePanel art card");
            artCard.anchoredPosition = new Vector2(0f, -255f);
            artCard.sizeDelta = new Vector2(760f, 430f);
            ConfigureImage(artCard.GetComponent<Image>(), cardSprite, Image.Type.Sliced);
            EnsureDecorativeImage(artCard, "ApprovedBomb", bombSprite, new Vector2(-190f, 20f), new Vector2(270f, 270f));
            EnsureDecorativeImage(artCard, "ApprovedSwap", swapSprite, new Vector2(190f, 20f), new Vector2(270f, 270f));
            EnsureDecorativeImage(artCard, "ApprovedBombCountBg", countBadgeSprite, new Vector2(-85f, -95f), new Vector2(78f, 78f));
            EnsureDecorativeImage(artCard, "ApprovedSwapCountBg", countBadgeSprite, new Vector2(295f, -95f), new Vector2(78f, 78f));
            ConfigureCountLabel(artCard, title, "ApprovedBombCount", new Vector2(-85f, -95f));
            ConfigureCountLabel(artCard, title, "ApprovedSwapCount", new Vector2(295f, -95f));

            PrefabUtility.SaveAsPrefabAsset(root, LosePanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyRealGameAlignment()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(RealGamePanelPrefab);
        try
        {
            SetAnchoredX(root.transform.Find("Content/CurrencyBar"), 0f);
            SetAnchoredX(root.transform.Find("Content/CurrencyBar/CurrentGroup"), 34f);
            SetAnchoredX(root.transform.Find("Content/CurrencyBar/PauseButton"), 434f);
            root.transform.Find("Content/CurrencyBar/PauseButton")?.SetAsLastSibling();
            SetAnchoredX(root.transform.Find("Content/SlotEnter"), 62f);
            EnsureCornerImage(
                RequireRect(root.transform.Find("Content"), "RealGamePanel/Content"),
                "ApprovedPauseVisual",
                RequireSprite(SettingIconPath),
                new Vector2(1f, 1f),
                new Vector2(-62f, -93f),
                new Vector2(104f, 104f));
            PrefabUtility.SaveAsPrefabAsset(root, RealGamePanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void SetAnchoredX(Transform target, float x)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null)
        {
            throw new InvalidOperationException("Required RectTransform is missing.");
        }

        rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
    }

    private static void ComposeApprovedWithdrawChannelCards()
    {
        // WithdrawWay intentionally uses one image for both the payment logo
        // and its card. The untouched same-name channel art already contains
        // the approved bevel, logo scale and padding, so restore those exact
        // bytes instead of reconstructing a flatter approximation.
        RestoreApprovedWithdrawChannelCard(PagBankChannelPath, new RectInt(34, 30, 214, 76), 1.00f, -4f);
        RestoreApprovedWithdrawChannelCard(PixChannelPath, new RectInt(24, 20, 226, 92), 1.00f, -8f);
    }

    private static void RestoreApprovedWithdrawChannelCard(
        string channelAssetPath,
        RectInt sourceLogoRect,
        float logoScale,
        float logoOffsetX)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string assetAbsolutePath = Path.Combine(projectRoot, channelAssetPath.Replace('/', Path.DirectorySeparatorChar));
        string backupPath = Path.Combine(
            projectRoot,
            "CurrentUI_OneToOne_Audit_20260821",
            "AssetBackups",
            "RealWithdraw",
            Path.GetFileName(channelAssetPath));
        if (!File.Exists(backupPath))
        {
            throw new FileNotFoundException("Approved withdrawal channel backup is missing.", backupPath);
        }

        string fillAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawTargetCardPath.Replace('/', Path.DirectorySeparatorChar));
        Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D fill = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D output = null;
        try
        {
            if (!source.LoadImage(File.ReadAllBytes(backupPath), false)
                || !fill.LoadImage(File.ReadAllBytes(fillAbsolutePath), false))
            {
                throw new InvalidOperationException("Unable to decode approved withdrawal channel art.");
            }

            output = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[source.width * source.height];
            Color legacyBackground = source.GetPixel(20, source.height / 2);
            const int border = 11;
            float targetLogoWidth = sourceLogoRect.width * logoScale;
            float targetLogoHeight = sourceLogoRect.height * logoScale;
            float targetLogoX = sourceLogoRect.x
                + (sourceLogoRect.width - targetLogoWidth) * 0.5f
                + logoOffsetX;
            float targetLogoY = sourceLogoRect.y
                + (sourceLogoRect.height - targetLogoHeight) * 0.5f;
            for (int y = 0; y < source.height; y++)
            {
                for (int x = 0; x < source.width; x++)
                {
                    bool isBorder = x < border || x >= source.width - border
                        || y < border || y >= source.height - border;
                    Color sampled = source.GetPixel(x, y);
                    Color result = isBorder
                        ? sampled
                        : fill.GetPixelBilinear(
                            (x + 0.5f) / source.width,
                            (y + 0.5f) / source.height);

                    bool isLogo = !isBorder
                        && x >= targetLogoX && x < targetLogoX + targetLogoWidth
                        && y >= targetLogoY && y < targetLogoY + targetLogoHeight;
                    if (isLogo)
                    {
                        float sourceX = sourceLogoRect.x
                            + ((x + 0.5f - targetLogoX) / targetLogoWidth) * sourceLogoRect.width;
                        float sourceY = sourceLogoRect.y
                            + ((y + 0.5f - targetLogoY) / targetLogoHeight) * sourceLogoRect.height;
                        sampled = source.GetPixelBilinear(
                            sourceX / source.width,
                            sourceY / source.height);
                        float distance = Mathf.Max(
                            Mathf.Abs(sampled.r - legacyBackground.r),
                            Mathf.Abs(sampled.g - legacyBackground.g),
                            Mathf.Abs(sampled.b - legacyBackground.b));
                        float alpha = Mathf.Clamp01((distance - 0.008f) / 0.10f) * sampled.a;
                        if (alpha > 0.001f)
                        {
                            Color foreground = new Color(
                                Mathf.Clamp01((sampled.r - legacyBackground.r * (1f - alpha)) / alpha),
                                Mathf.Clamp01((sampled.g - legacyBackground.g * (1f - alpha)) / alpha),
                                Mathf.Clamp01((sampled.b - legacyBackground.b * (1f - alpha)) / alpha),
                                1f);
                            result = Color.Lerp(result, foreground, alpha);
                        }
                    }
                    pixels[y * source.width + x] = result;
                }
            }
            output.SetPixels(pixels);
            output.Apply(false, false);
            File.WriteAllBytes(assetAbsolutePath, output.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(source);
            UnityEngine.Object.DestroyImmediate(fill);
            if (output != null)
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }
        AssetDatabase.ImportAsset(
            channelAssetPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        ConfigureGeneratedSprite(channelAssetPath);
    }

    private static void ComposeApprovedWithdrawRuntimeCard()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string backupPath = Path.Combine(
            projectRoot,
            "CurrentUI_OneToOne_Audit_20260821",
            "AssetBackups",
            "RealWithdraw",
            Path.GetFileName(PagBankChannelPath));
        string fillAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawTargetCardPath.Replace('/', Path.DirectorySeparatorChar));
        string outputAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawRuntimeCardPath.Replace('/', Path.DirectorySeparatorChar));

        Texture2D borderedCard = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D fillCard = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D output = null;
        try
        {
            if (!borderedCard.LoadImage(File.ReadAllBytes(backupPath), false)
                || !fillCard.LoadImage(File.ReadAllBytes(fillAbsolutePath), false))
            {
                throw new InvalidOperationException("Unable to decode approved withdrawal card art.");
            }

            output = new Texture2D(borderedCard.width, borderedCard.height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[borderedCard.width * borderedCard.height];
            const int border = 11;
            for (int y = 0; y < borderedCard.height; y++)
            {
                for (int x = 0; x < borderedCard.width; x++)
                {
                    bool isBorder = x < border || x >= borderedCard.width - border
                        || y < border || y >= borderedCard.height - border;
                    pixels[y * borderedCard.width + x] = isBorder
                        ? borderedCard.GetPixel(x, y)
                        : fillCard.GetPixelBilinear(
                            (x + 0.5f) / borderedCard.width,
                            (y + 0.5f) / borderedCard.height);
                }
            }
            output.SetPixels(pixels);
            output.Apply(false, false);
            File.WriteAllBytes(outputAbsolutePath, output.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(borderedCard);
            UnityEngine.Object.DestroyImmediate(fillCard);
            if (output != null)
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }

        AssetDatabase.ImportAsset(
            RealWithdrawRuntimeCardPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        ConfigureGeneratedSprite(RealWithdrawRuntimeCardPath);
    }

    private static void ComposeApprovedWithdrawChannelCard(
        string channelAssetPath,
        RectInt sourceLogoRect,
        float logoScale,
        float logoOffsetX)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string sourceAbsolutePath = Path.Combine(projectRoot, channelAssetPath.Replace('/', Path.DirectorySeparatorChar));
        string cardAbsolutePath = Path.Combine(projectRoot, RealWithdrawTargetCardPath.Replace('/', Path.DirectorySeparatorChar));
        string backupDirectory = Path.Combine(
            projectRoot,
            "CurrentUI_OneToOne_Audit_20260821",
            "AssetBackups",
            "RealWithdraw");
        Directory.CreateDirectory(backupDirectory);
        string backupPath = Path.Combine(backupDirectory, Path.GetFileName(channelAssetPath));
        if (!File.Exists(backupPath))
        {
            File.Copy(sourceAbsolutePath, backupPath, false);
        }

        Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D card = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D output = null;
        try
        {
            if (!source.LoadImage(File.ReadAllBytes(backupPath), false)
                || !card.LoadImage(File.ReadAllBytes(cardAbsolutePath), false))
            {
                throw new InvalidOperationException("Unable to decode withdrawal channel source art: " + channelAssetPath);
            }

            output = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[source.width * source.height];
            Color legacyBackground = source.GetPixel(
                Mathf.Clamp(20, 0, source.width - 1),
                Mathf.Clamp(source.height / 2, 0, source.height - 1));

            float targetLogoWidth = sourceLogoRect.width * logoScale;
            float targetLogoHeight = sourceLogoRect.height * logoScale;
            float targetLogoX = sourceLogoRect.x + (sourceLogoRect.width - targetLogoWidth) * 0.5f + logoOffsetX;
            float targetLogoY = sourceLogoRect.y + (sourceLogoRect.height - targetLogoHeight) * 0.5f;

            for (int y = 0; y < source.height; y++)
            {
                for (int x = 0; x < source.width; x++)
                {
                    Color result = card.GetPixelBilinear(
                        (x + 0.5f) / source.width,
                        (y + 0.5f) / source.height);

                    if (x >= targetLogoX && x < targetLogoX + targetLogoWidth
                        && y >= targetLogoY && y < targetLogoY + targetLogoHeight)
                    {
                        float sourceX = sourceLogoRect.x
                            + ((x + 0.5f - targetLogoX) / targetLogoWidth) * sourceLogoRect.width;
                        float sourceY = sourceLogoRect.y
                            + ((y + 0.5f - targetLogoY) / targetLogoHeight) * sourceLogoRect.height;
                        Color sampled = source.GetPixelBilinear(
                            sourceX / source.width,
                            sourceY / source.height);
                        float distance = Mathf.Max(
                            Mathf.Abs(sampled.r - legacyBackground.r),
                            Mathf.Abs(sampled.g - legacyBackground.g),
                            Mathf.Abs(sampled.b - legacyBackground.b));
                        float alpha = Mathf.Clamp01((distance - 0.008f) / 0.10f) * sampled.a;
                        if (alpha > 0.001f)
                        {
                            Color foreground = new Color(
                                Mathf.Clamp01((sampled.r - legacyBackground.r * (1f - alpha)) / alpha),
                                Mathf.Clamp01((sampled.g - legacyBackground.g * (1f - alpha)) / alpha),
                                Mathf.Clamp01((sampled.b - legacyBackground.b * (1f - alpha)) / alpha),
                                1f);
                            result = Color.Lerp(result, foreground, alpha);
                        }
                    }

                    pixels[y * source.width + x] = result;
                }
            }

            output.SetPixels(pixels);
            output.Apply(false, false);
            File.WriteAllBytes(sourceAbsolutePath, output.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(source);
            UnityEngine.Object.DestroyImmediate(card);
            if (output != null)
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }

        AssetDatabase.ImportAsset(
            channelAssetPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        ConfigureGeneratedSprite(channelAssetPath);
    }

    private static void ComposeApprovedWithdrawGradient()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string referenceAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawApprovedReferencePath.Replace('/', Path.DirectorySeparatorChar));
        string outputAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawTargetGradientPath.Replace('/', Path.DirectorySeparatorChar));

        Texture2D reference = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D output = null;
        try
        {
            if (!reference.LoadImage(File.ReadAllBytes(referenceAbsolutePath), false))
            {
                throw new InvalidOperationException("Unable to decode approved withdrawal reference.");
            }

            const int outputWidth = 64;
            output = new Texture2D(outputWidth, reference.height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[outputWidth * reference.height];
            const int sampleWidth = 7;
            for (int y = 0; y < reference.height; y++)
            {
                Color rowColor = Color.clear;
                int sampleCount = 0;
                for (int x = 0; x < sampleWidth; x++)
                {
                    rowColor += reference.GetPixel(x, y);
                    rowColor += reference.GetPixel(reference.width - 1 - x, y);
                    sampleCount += 2;
                }
                rowColor /= sampleCount;
                rowColor.a = 1f;
                for (int x = 0; x < outputWidth; x++)
                {
                    pixels[y * outputWidth + x] = rowColor;
                }
            }

            output.SetPixels(pixels);
            output.Apply(false, false);
            File.WriteAllBytes(outputAbsolutePath, output.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(reference);
            if (output != null)
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }

        AssetDatabase.ImportAsset(
            RealWithdrawTargetGradientPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        ConfigureGeneratedSprite(RealWithdrawTargetGradientPath);
    }

    private static void ComposeApprovedWithdrawButton()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string sourceAbsolutePath = Path.Combine(
            projectRoot,
            PrimaryButtonPath.Replace('/', Path.DirectorySeparatorChar));
        string outputAbsolutePath = Path.Combine(
            projectRoot,
            RealWithdrawTargetButtonPath.Replace('/', Path.DirectorySeparatorChar));

        Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D output = null;
        try
        {
            if (!source.LoadImage(File.ReadAllBytes(sourceAbsolutePath), false))
            {
                throw new InvalidOperationException("Unable to decode withdrawal button source art.");
            }

            output = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            Color[] pixels = source.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                if (color.a > 0.001f && color.g > color.r * 1.3f && color.g > color.b * 1.6f)
                {
                    color.r = Mathf.Clamp01(color.r + color.g * 0.38f);
                    color.g = Mathf.Clamp01(color.g * 0.94f);
                    color.b = Mathf.Clamp01(color.b * 0.12f);
                    pixels[i] = color;
                }
            }
            output.SetPixels(pixels);
            output.Apply(false, false);
            File.WriteAllBytes(outputAbsolutePath, output.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(source);
            if (output != null)
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }

        AssetDatabase.ImportAsset(
            RealWithdrawTargetButtonPath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        ConfigureGeneratedSprite(RealWithdrawTargetButtonPath);
    }

    private static void ConfigureGeneratedSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            throw new InvalidOperationException("Texture importer not found for " + path);
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 100f;
        importer.SaveAndReimport();
    }

    private static void ConfigureSpriteBorder(string path, Vector4 border)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            throw new InvalidOperationException("Texture importer not found for " + path);
        }

        importer.spriteBorder = border;
        importer.SaveAndReimport();
    }

    private static void ApplyServiceSelectVisuals()
    {
        Sprite badgeSprite = RequireSprite(BadgePath);
        Sprite cardSprite = RequireSprite(CardPath);
        Sprite arrowSprite = RequireSprite(ArrowPath);

        GameObject root = PrefabUtility.LoadPrefabContents(ServiceSelectPrefab);
        try
        {
            ServiceSelectPanel page = root.GetComponent<ServiceSelectPanel>();
            if (page == null)
            {
                throw new InvalidOperationException("ServiceSelectPanel component is missing.");
            }

            List<BizzaButton> rows = new List<BizzaButton>(page.defaultButtons ?? Array.Empty<BizzaButton>());
            if (page.customButton != null)
            {
                rows.Add(page.customButton);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                BizzaButton button = rows[i];
                if (button == null)
                {
                    continue;
                }

                RectTransform rowRect = button.transform as RectTransform;
                Image rowImage = button.GetComponent<Image>();
                TMP_Text sourceText = i < page.defaultTexts.Length
                    ? page.defaultTexts[i]
                    : button.GetComponentInChildren<TMP_Text>(true);

                if (rowRect != null)
                {
                    float height = Mathf.Max(82f, rowRect.sizeDelta.y);
                    rowRect.sizeDelta = new Vector2(762f, height);
                }

                if (rowImage != null)
                {
                    rowImage.sprite = cardSprite;
                    rowImage.type = Image.Type.Sliced;
                    rowImage.color = Color.white;
                }

                if (sourceText != null)
                {
                    RectTransform textRect = sourceText.rectTransform;
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.pivot = new Vector2(0.5f, 0.5f);
                    textRect.offsetMin = new Vector2(96f, 8f);
                    textRect.offsetMax = new Vector2(-96f, -8f);
                    sourceText.alignment = TextAlignmentOptions.MidlineLeft;
                    sourceText.enableAutoSizing = true;
                    sourceText.fontSizeMin = 18f;
                    sourceText.fontSizeMax = 30f;
                    sourceText.fontSize = 30f;
                    sourceText.color = new Color32(90, 53, 43, 255);
                    sourceText.enableWordWrapping = true;
                }

                string badgeText = i < page.defaultButtons.Length ? (i + 1).ToString() : "☎";
                EnsureBadge(button.transform, badgeSprite, sourceText, badgeText);
                EnsureArrow(button.transform, arrowSprite);
            }

            PrefabUtility.SaveAsPrefabAsset(root, ServiceSelectPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ApplyServicePanelVisuals()
    {
        Sprite cardSprite = RequireSprite(CardPath);
        GameObject root = PrefabUtility.LoadPrefabContents(ServicePanelPrefab);
        try
        {
            Transform inputBackground = root.transform.Find("Content/InputNode/bg");
            if (inputBackground != null)
            {
                Image image = inputBackground.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = cardSprite;
                    image.type = Image.Type.Sliced;
                    image.color = Color.white;
                }
            }

            Transform inputNode = root.transform.Find("Content/InputNode");
            if (inputNode is RectTransform inputRect)
            {
                inputRect.sizeDelta = new Vector2(inputRect.sizeDelta.x, 300f);
            }

            PrefabUtility.SaveAsPrefabAsset(root, ServicePanelPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void EnsureBadge(Transform parent, Sprite sprite, TMP_Text fontSource, string value)
    {
        Transform existing = parent.Find("ApprovedNumberBadge");
        GameObject badgeObject = existing != null ? existing.gameObject : new GameObject("ApprovedNumberBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            badgeObject.transform.SetParent(parent, false);
        }

        RectTransform badgeRect = badgeObject.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0f, 0.5f);
        badgeRect.anchorMax = new Vector2(0f, 0.5f);
        badgeRect.pivot = new Vector2(0.5f, 0.5f);
        badgeRect.anchoredPosition = new Vector2(48f, 0f);
        badgeRect.sizeDelta = new Vector2(58f, 58f);

        Image image = badgeObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        Transform labelTransform = badgeObject.transform.Find("Label");
        GameObject labelObject = labelTransform != null ? labelTransform.gameObject : new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        if (labelTransform == null)
        {
            labelObject.transform.SetParent(badgeObject.transform, false);
        }

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.text = value;
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = value == "☎" ? 28f : 34f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.raycastTarget = false;
        if (fontSource != null)
        {
            label.font = fontSource.font;
            label.fontSharedMaterial = fontSource.fontSharedMaterial;
        }

        badgeObject.transform.SetAsLastSibling();
    }

    private static void EnsureArrow(Transform parent, Sprite sprite)
    {
        Transform existing = parent.Find("ApprovedArrow");
        GameObject arrowObject = existing != null ? existing.gameObject : new GameObject("ApprovedArrow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            arrowObject.transform.SetParent(parent, false);
        }

        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1f, 0.5f);
        arrowRect.anchorMax = new Vector2(1f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.anchoredPosition = new Vector2(-48f, 0f);
        arrowRect.sizeDelta = new Vector2(56f, 56f);
        arrowRect.localEulerAngles = new Vector3(0f, 0f, 180f);

        Image image = arrowObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        arrowObject.transform.SetAsLastSibling();
    }

    private static RectTransform RequireRect(Transform target, string description)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null)
        {
            throw new InvalidOperationException("Required RectTransform is missing: " + description);
        }
        return rect;
    }

    private static void ConfigurePanelImage(GameObject target, Sprite sprite)
    {
        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            image = target.AddComponent<Image>();
        }
        ConfigureImage(image, sprite, Image.Type.Sliced);
        image.raycastTarget = false;
    }

    private static void EnsureBackgroundImage(RectTransform parent, string name, Sprite sprite)
    {
        Transform existing = parent.Find(name);
        GameObject backgroundObject = existing != null
            ? existing.gameObject
            : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            backgroundObject.transform.SetParent(parent, false);
        }

        RectTransform rect = backgroundObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = parent.sizeDelta;

        Image image = backgroundObject.GetComponent<Image>();
        ConfigureImage(image, sprite, Image.Type.Sliced);
        image.raycastTarget = false;
        backgroundObject.transform.SetAsFirstSibling();
    }

    private static void ConfigureImage(Image image, Sprite sprite, Image.Type type)
    {
        if (image == null)
        {
            throw new InvalidOperationException("Required Image component is missing.");
        }
        image.sprite = sprite;
        image.type = type;
        image.color = Color.white;
        image.preserveAspect = false;
    }

    private static Transform FindChildBySprite(Transform parent, string spriteName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Image image = child.GetComponent<Image>();
            if (image != null && image.sprite != null && image.sprite.name == spriteName)
            {
                return child;
            }
        }
        return null;
    }

    private static void SetActiveIfFound(Transform root, string path, bool active)
    {
        Transform target = root.Find(path);
        if (target != null)
        {
            target.gameObject.SetActive(active);
        }
    }

    private static Text EnsureClonedText(Transform parent, Text source, string name, string value)
    {
        Transform existing = parent.Find(name);
        GameObject textObject;
        if (existing != null)
        {
            textObject = existing.gameObject;
        }
        else if (source != null)
        {
            textObject = UnityEngine.Object.Instantiate(source.gameObject, parent, false);
            textObject.name = name;
        }
        else
        {
            textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
        }

        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.raycastTarget = false;
        text.enabled = true;
        textObject.transform.SetAsLastSibling();
        return text;
    }

    private static TMP_Text EnsureClonedTMP(Transform parent, TMP_Text source, string name, string value)
    {
        Transform existing = parent.Find(name);
        GameObject textObject;
        if (existing != null)
        {
            textObject = existing.gameObject;
        }
        else
        {
            textObject = UnityEngine.Object.Instantiate(source.gameObject, parent, false);
            textObject.name = name;
        }

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.raycastTarget = false;
        text.enabled = true;
        textObject.transform.SetAsLastSibling();
        return text;
    }

    private static Text EnsurePlainText(Transform parent, Text source, string name, string value)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(existing.gameObject);
        }

        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);
        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.font = source.font;
        text.fontStyle = source.fontStyle;
        text.fontSize = source.fontSize;
        text.lineSpacing = source.lineSpacing;
        text.supportRichText = source.supportRichText;
        text.color = source.color;
        text.material = source.material;
        text.raycastTarget = false;
        text.enabled = true;
        textObject.transform.SetAsLastSibling();
        return text;
    }

    private static TMP_Text EnsurePlainTMP(Transform parent, TMP_Text source, string name, string value)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(existing.gameObject);
        }

        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = source.font;
        text.fontSharedMaterial = source.fontSharedMaterial;
        text.fontStyle = source.fontStyle;
        text.fontSize = source.fontSize;
        text.color = source.color;
        text.raycastTarget = false;
        text.enabled = true;
        textObject.transform.SetAsLastSibling();
        return text;
    }

    private static void ConfigureCountLabel(Transform parent, TMP_Text source, string name, Vector2 position)
    {
        TMP_Text label = EnsurePlainTMP(parent, source, name, "1");
        label.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        label.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        label.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        label.rectTransform.anchoredPosition = position;
        label.rectTransform.sizeDelta = new Vector2(68f, 68f);
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 28f;
        label.fontSizeMax = 52f;
        label.color = Color.white;
        label.transform.SetAsLastSibling();
    }

    private static void EnsureDecorativeImage(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        Transform existing = parent.Find(name);
        GameObject imageObject = existing != null
            ? existing.gameObject
            : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            imageObject.transform.SetParent(parent, false);
        }

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        imageObject.transform.SetAsLastSibling();
    }

    private static void EnsureSolidImage(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        Transform existing = parent.Find(name);
        GameObject imageObject = existing != null
            ? existing.gameObject
            : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            imageObject.transform.SetParent(parent, false);
        }

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = imageObject.GetComponent<Image>();
        image.sprite = null;
        image.color = color;
        image.raycastTarget = false;
        imageObject.transform.SetAsLastSibling();
    }

    private static void EnsureCornerImage(RectTransform parent, string name, Sprite sprite, Vector2 anchor, Vector2 position, Vector2 size)
    {
        Transform existing = parent.Find(name);
        GameObject imageObject = existing != null
            ? existing.gameObject
            : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (existing == null)
        {
            imageObject.transform.SetParent(parent, false);
        }

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        imageObject.transform.SetAsLastSibling();
    }

    private static void StretchLabel(RectTransform rect, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }

    private static Sprite RequireSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            throw new InvalidOperationException("Sprite not found: " + path);
        }
        return sprite;
    }

    private static Material RequireMaterial(string path)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            throw new InvalidOperationException("Material not found: " + path);
        }

        return material;
    }
}
#endif
