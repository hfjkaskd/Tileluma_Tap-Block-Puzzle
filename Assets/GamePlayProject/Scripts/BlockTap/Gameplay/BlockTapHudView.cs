using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlockTap.Gameplay
{
    public class BlockTapHudView : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private RectTransform canvasRoot;
        [SerializeField] private RectTransform hudRoot;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private Transform goalContainer;
        [SerializeField] private Button shuffleButton;
        [SerializeField] private GameObject shuffleAddIcon;
        [SerializeField] private GameObject shuffleCountBg;
        [SerializeField] private TMP_Text shuffleCountLabel;
        [SerializeField] private Button bombButton;
        [SerializeField] private Image bombIconImage;
        [SerializeField] private GameObject bombAddIcon;
        [SerializeField] private GameObject bombCountBg;
        [SerializeField] private TMP_Text bombCountLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Transform resultAnchor;
        [SerializeField] private GameObject propPopup;
        [SerializeField] private TMP_Text propPopupTitleLabel;
        [SerializeField] private TMP_Text propPopupMessageLabel;
        [SerializeField] private Button propPopupRewardButton;
        [SerializeField] private Button propPopupCloseButton;

        [Header("Dynamic Prefabs")]
        [SerializeField] private BlockTapGoalView goalItemPrefab;

        private readonly List<BlockTapGoalView> goalViews = new List<BlockTapGoalView>();

        public RectTransform CanvasRoot => canvasRoot;
        public RectTransform HudRoot => hudRoot;
        public TMP_Text LevelLabel => levelLabel;
        public Button ShuffleButton => shuffleButton;
        public Button BombButton => bombButton;
        public Button RestartButton => restartButton;
        public Button NextButton => nextButton;
        public Transform ResultAnchor => resultAnchor;
        public IReadOnlyList<BlockTapGoalView> GoalViews => goalViews;

        private void Awake()
        {
            EnsureBindings();
        }

        private void Reset()
        {
            EnsureBindings();
            MarkDirtyInEditor();
        }

        private void OnValidate()
        {
            EnsureBindings();
            MarkDirtyInEditor();
        }

        public void RebuildGoals(
            IList<int> colorIndices,
            IReadOnlyDictionary<int, int> remainingByColor,
            IReadOnlyDictionary<int, int> targetByColor,
            Func<int, Sprite> resolveSprite)
        {
            ClearGoals();

            if (goalContainer == null)
            {
                Debug.LogWarning("BlockTapHudView is missing GoalContainer binding.", this);
                return;
            }

            if (goalItemPrefab == null)
            {
                Debug.LogWarning("BlockTapHudView is missing GoalItemPrefab binding.", this);
                return;
            }

            for (var index = 0; index < colorIndices.Count; index++)
            {
                var colorIndex = colorIndices[index];
                var goalView = Instantiate(goalItemPrefab, goalContainer);
                goalView.name = $"Goal_{index}";
                goalView.Setup(colorIndex, resolveSprite(colorIndex), targetByColor[colorIndex]);
                goalView.Refresh(targetByColor[colorIndex] - remainingByColor[colorIndex]);
                goalViews.Add(goalView);
            }
        }

        public void UpdateGoal(int colorIndex, int collected)
        {
            foreach (var goal in goalViews)
            {
                if (goal.ColorIndex != colorIndex)
                {
                    continue;
                }

                goal.Refresh(collected);
                return;
            }
        }

        public void SetLevelText(string text)
        {
            EnsureBindings();
            if (levelLabel != null)
            {
                levelLabel.text = text;
            }
        }

        public void SetMessage(string text)
        {
        }

        public void SetPropCounts(int shuffleCount, int bombCount)
        {
            EnsureBindings();
            SetPropCountState(shuffleCount, shuffleAddIcon, shuffleCountBg, shuffleCountLabel);
            SetPropCountState(bombCount, bombAddIcon, bombCountBg, bombCountLabel);
        }

        public Sprite GetBombIconSprite()
        {
            EnsureBindings();
            if (bombIconImage != null && bombIconImage.sprite != null)
            {
                return bombIconImage.sprite;
            }

            return ResolveIconSprite(bombAddIcon, bombButton);
        }

        public Vector3 GetBombIconPosition()
        {
            EnsureBindings();
            if (bombIconImage != null && bombIconImage.rectTransform != null)
            {
                return bombIconImage.rectTransform.position;
            }

            return bombButton != null ? bombButton.transform.position : transform.position;
        }

        public Vector2 GetBombIconSize()
        {
            EnsureBindings();
            if (bombIconImage != null && bombIconImage.rectTransform != null)
            {
                return bombIconImage.rectTransform.rect.size;
            }

            return Vector2.zero;
        }

        public RectTransform GetBombIconRectTransform()
        {
            EnsureBindings();
            return bombIconImage != null ? bombIconImage.rectTransform : null;
        }

        public void ShowPropPopup(string title, string message)
        {
            EnsureBindings();
            if (propPopupTitleLabel != null)
            {
                propPopupTitleLabel.text = title;
            }

            if (propPopupMessageLabel != null)
            {
                propPopupMessageLabel.text = message;
            }

            if (propPopup != null)
            {
                propPopup.SetActive(true);
            }
        }

        public void HidePropPopup()
        {
            EnsureBindings();
            if (propPopup != null)
            {
                propPopup.SetActive(false);
            }
        }

        private void EnsureBindings()
        {
            canvasRoot = canvasRoot != null ? canvasRoot : GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
            hudRoot = hudRoot != null ? hudRoot : GetComponent<RectTransform>();
            levelLabel = levelLabel != null ? levelLabel : transform.Find("TopBar/LevelLabel")?.GetComponent<TMP_Text>();
            goalContainer = goalContainer != null ? goalContainer : transform.Find("TopBar/GoalContainer");
            shuffleButton = shuffleButton != null ? shuffleButton : transform.Find("BottomBar/ShuffleButton")?.GetComponent<Button>();
            shuffleAddIcon = shuffleAddIcon != null ? shuffleAddIcon : transform.Find("BottomBar/ShuffleButton/AddIcon")?.gameObject;
            shuffleCountBg = shuffleCountBg != null ? shuffleCountBg : transform.Find("BottomBar/ShuffleButton/CountBG")?.gameObject;
            shuffleCountLabel = shuffleCountLabel != null ? shuffleCountLabel : transform.Find("BottomBar/ShuffleButton/CountBG/ShuffleCountLabel")?.GetComponent<TMP_Text>();
            bombButton = bombButton != null ? bombButton : transform.Find("BottomBar/BombButton")?.GetComponent<Button>();
            bombIconImage = bombIconImage != null ? bombIconImage : transform.Find("BottomBar/BombButton/Icon")?.GetComponent<Image>();
            bombAddIcon = bombAddIcon != null ? bombAddIcon : transform.Find("BottomBar/BombButton/AddIcon")?.gameObject;
            bombCountBg = bombCountBg != null ? bombCountBg : transform.Find("BottomBar/BombButton/CountBG")?.gameObject;
            bombCountLabel = bombCountLabel != null ? bombCountLabel : transform.Find("BottomBar/BombButton/CountBG/BombCountLabel")?.GetComponent<TMP_Text>();
            restartButton = restartButton != null ? restartButton : transform.Find("BottomBar/RestartButton")?.GetComponent<Button>();
            nextButton = nextButton != null ? nextButton : transform.Find("BottomBar/NextButton")?.GetComponent<Button>();
            resultAnchor = resultAnchor != null ? resultAnchor : transform.Find("ResultAnchor");
            propPopup = propPopup != null ? propPopup : transform.Find("PropPopup")?.gameObject;
            propPopupTitleLabel = propPopupTitleLabel != null ? propPopupTitleLabel : transform.Find("PropPopup/Panel/TitleLabel")?.GetComponent<TMP_Text>();
            propPopupMessageLabel = propPopupMessageLabel != null ? propPopupMessageLabel : transform.Find("PropPopup/Panel/MessageLabel")?.GetComponent<TMP_Text>();
            propPopupRewardButton = propPopupRewardButton != null ? propPopupRewardButton : transform.Find("PropPopup/Panel/RewardButton")?.GetComponent<Button>();
            propPopupCloseButton = propPopupCloseButton != null ? propPopupCloseButton : transform.Find("PropPopup/Panel/CloseButton")?.GetComponent<Button>();
            if (goalItemPrefab == null)
            {
                var prefabGo = Resources.Load<GameObject>("BlockTap/Prefabs/GoalItem");
                if (prefabGo != null)
                {
                    goalItemPrefab = prefabGo.GetComponent<BlockTapGoalView>();
                }
            }

            if (propPopup != null && !Application.isPlaying)
            {
                propPopup.SetActive(false);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void MarkDirtyInEditor()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }

        private void ClearGoals()
        {
            for (var index = 0; index < goalViews.Count; index++)
            {
                var goalView = goalViews[index];
                if (goalView != null)
                {
                    Destroy(goalView.gameObject);
                }
            }

            goalViews.Clear();
        }

        private static void SetPropCountState(int count, GameObject addIcon, GameObject countBg, TMP_Text countLabel)
        {
            var hasCount = count > 0;
            if (addIcon != null)
            {
                addIcon.SetActive(!hasCount);
            }

            if (countBg != null)
            {
                countBg.SetActive(hasCount);
            }

            if (countLabel != null)
            {
                countLabel.text = $"x{Mathf.Max(0, count)}";
            }
        }

        private static Sprite ResolveIconSprite(GameObject addIcon, Button button)
        {
            if (TryGetImage(addIcon, out var addIconImage) && addIconImage.sprite != null)
            {
                return addIconImage.sprite;
            }

            if (button != null && TryGetImage(button.gameObject, out var buttonImage) && buttonImage.sprite != null)
            {
                return buttonImage.sprite;
            }

            return BlockTapRuntimeResources.WhiteSprite;
        }

        private static bool TryGetIconRectTransform(GameObject target, out RectTransform rectTransform)
        {
            rectTransform = target != null ? target.GetComponent<RectTransform>() : null;
            return rectTransform != null;
        }

        private static bool TryGetImage(GameObject target, out Image image)
        {
            image = target != null ? target.GetComponent<Image>() : null;
            return image != null;
        }
    }
}
