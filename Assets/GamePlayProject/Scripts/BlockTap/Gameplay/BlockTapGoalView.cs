using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace BlockTap.Gameplay
{
    public class BlockTapGoalView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private GameObject completeIcon;
        [SerializeField] private TMP_Text progressLabel;

        public RectTransform RectTransform { get; private set; }
        public int ColorIndex { get; private set; }
        private int targetCount;
        private int displayedCollectedCount;

        private void Awake()
        {
            EnsureBindings();
        }

        public void Bind(Image goalIcon, TMP_Text progress, GameObject completeMark)
        {
            icon = goalIcon;
            progressLabel = progress;
            completeIcon = completeMark;
        }

        public void Setup(int colorIndex, Sprite sprite, int targetCount)
        {
            EnsureBindings();
            ColorIndex = colorIndex;
            this.targetCount = targetCount;
            displayedCollectedCount = 0;
            if (icon != null)
            {
                icon.sprite = sprite != null ? sprite : BlockTapRuntimeResources.WhiteSprite;
                icon.color = Color.white;
                icon.preserveAspect = true;
            }

            SetCompletionVisual(false);
            ApplyLabels(0);
        }

        public void Refresh(int collectedCount)
        {
            EnsureBindings();
            ApplyLabels(collectedCount);
        }

        private void EnsureBindings()
        {
            RectTransform = RectTransform != null ? RectTransform : GetComponent<RectTransform>();

            if (icon == null)
            {
                icon = transform.Find("Icon")?.GetComponent<Image>();
                if (icon == null)
                {
                    icon = GetComponentInChildren<Image>();
                    if (icon == GetComponent<Image>())
                    {
                        icon = null;
                    }
                }
            }

            if (progressLabel == null)
            {
                progressLabel = transform.Find("ProgressLabel")?.GetComponent<TMP_Text>();
            }

            if (progressLabel == null)
            {
                progressLabel = GetComponentInChildren<TMP_Text>();
            }

            if (completeIcon == null)
            {
                completeIcon = transform.Find("CompleteIcon")?.gameObject;
            }
        }

        private void ApplyLabels(int collectedCount)
        {
            if (progressLabel != null)
            {
                DOTween.Kill(progressLabel);
                SetCompletionVisual(false);
                var startValue = displayedCollectedCount;
                DOTween.To(
                        () => startValue,
                        value =>
                        {
                            displayedCollectedCount = value;
                            progressLabel.text = $"{displayedCollectedCount}/{targetCount}";
                        },
                        collectedCount,
                        0.5f)
                    .SetEase(Ease.OutSine)
                    .SetTarget(progressLabel)
                    .OnComplete(() => SetCompletionVisual(collectedCount >= targetCount));
                return;
            }

            if (icon == null)
            {
                Debug.LogWarning($"BlockTapGoalView on '{name}' could not find an icon binding.", this);
            }

            if (progressLabel == null)
            {
                Debug.LogWarning($"BlockTapGoalView on '{name}' could not find any goal label bindings.", this);
            }
        }

        private void SetCompletionVisual(bool isComplete)
        {
            if (progressLabel != null)
            {
                progressLabel.gameObject.SetActive(!isComplete);
            }

            if (completeIcon != null)
            {
                completeIcon.SetActive(isComplete);
            }
        }
    }
}
