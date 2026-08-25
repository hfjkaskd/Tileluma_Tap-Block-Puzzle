using DG.Tweening;
using BlockTap.Config;
using UnityEngine;
using UnityEngine.UI;

namespace BlockTap.Gameplay
{
    public class BoardAnimationPlayer
    {
        private readonly BlockTapVisualConfigSO config;
        private readonly RectTransform screenRoot;
        private readonly RectTransform boardRoot;
        private readonly RectTransform effectsRoot;
        private readonly RectTransform flyingContainer;
        private readonly GameObject eliminationEffectPrefab;
        private const float ReadyBombOffsetY = 140f;
        private const float BombFlyDuration = 0.3f;
        private const float BombReadyHoverDuration = 0.55f;
        private const float BombReadyScale = 0.9f;
        private const float BombShockwavePushDuration = 0.14f;
        private const float BombShockwaveReturnDuration = 0.2f;
        private const float BombShockwaveScaleUp = 1.1f;
        private const float BombShockwaveScaleDown = 0.96f;
        private const float BombScreenShakeDuration = 0.6f;
        private const float BombScreenShakeStrength = 25f;
        private const int BombScreenShakeVibrato = 8;
        private const float EliminationEffectLifetime = 1.2f;

        public BoardAnimationPlayer(
            BlockTapVisualConfigSO config,
            RectTransform screenRoot,
            RectTransform boardRoot,
            RectTransform effectsRoot,
            RectTransform flyingContainer,
            GameObject bombEffectPrefab)
        {
            this.config = config;
            this.screenRoot = screenRoot;
            this.boardRoot = boardRoot;
            this.effectsRoot = effectsRoot;
            this.flyingContainer = flyingContainer;
            eliminationEffectPrefab = Resources.Load<GameObject>("BlockTap/Prefabs/Spine");
        }

        public Tween AnimateSpawn(BlockTapBlockView block, Vector2 finalPosition, int column, int row, bool popFromZero = false)
        {
            block.RectTransform.anchoredPosition = finalPosition + Vector2.up * 300f;
            block.transform.localScale = popFromZero ? Vector3.zero : Vector3.one * 0.92f;
            var delay = column * config.spawnColumnDelay + row * config.spawnRowDelay;
            var sequence = DOTween.Sequence();
            if (delay > 0f)
            {
                sequence.AppendInterval(delay);
            }

            sequence.Append(block.RectTransform.DOAnchorPos(finalPosition, config.spawnDuration).SetEase(Ease.OutCubic));
            sequence.Join(block.transform.DOScale(1f, config.spawnDuration).SetEase(Ease.OutBack));
            return sequence;
        }

        public Tween AnimateShuffleSpawn(BlockTapBlockView block, Vector2 finalPosition, int column, int row, bool popFromZero = false)
        {
            return AnimateSpawn(block, finalPosition, column, row, popFromZero);
        }

        public Tween AnimateMove(BlockTapBlockView block, Vector2 finalPosition)
        {
            return block.RectTransform.DOAnchorPos(finalPosition, config.moveDuration).SetEase(Ease.InCubic);
        }

        public void PlayEliminationEffect(BlockTapBlockView block)
        {
            if (block == null || effectsRoot == null)
            {
                return;
            }

            PlayEliminationEffect(block.RectTransform.position, block.RectTransform.rect.size);
        }

        public Tween AnimateRemoval(BlockTapBlockView block, bool isTargetBlock, System.Action onComplete)
        {
            var image = block.GetComponent<Image>();
            var sequence = DOTween.Sequence();
            if (isTargetBlock)
            {
                sequence
                    .Append(block.transform.DOPunchScale(Vector3.one * 0.22f, config.removePunchDuration, 1, 0f))
                    .Join(image.DOFade(0f, config.removePunchDuration));
            }
            else
            {
                sequence
                    .Append(block.transform.DOScale(Vector3.one * 1.2f, config.removePunchDuration).SetEase(Ease.OutSine))
                    .Join(image.DOFade(0f, config.removePunchDuration));
            }

            sequence
                .OnComplete(() =>
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                    block.transform.localScale = Vector3.one;
                    block.ResetEffectState();
                    onComplete?.Invoke();
                });

            return sequence;
        }

        public Tween AnimateCollect(BlockTapBlockView source, BlockTapGoalView goalView, Sprite sprite, float delay, System.Action onComplete)
        {
            var image = source.GetComponent<Image>();
            image.sprite = sprite != null ? sprite : BlockTapRuntimeResources.WhiteSprite;
            image.color = Color.white;
            image.preserveAspect = true;
            var rect = source.RectTransform;
            rect.SetParent(effectsRoot, false);

            return DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() => PlayEliminationEffect(source.RectTransform.position, source.RectTransform.rect.size))
                .Append(rect.DOMove(goalView.RectTransform.position, config.collectFlyDuration).SetEase(Ease.OutSine))
                .Join(rect.DOScale(0.35f, config.collectFlyDuration))
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }

        public void AnimateBombPulse(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            target.DOKill();
            target.localScale = Vector3.one;
        }

        public void StopBombPulse(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            target.DOKill();
            target.localScale = Vector3.one;
        }

        public void AnimateBombBurst(Vector3 screenPosition, Color color)
        {
            AnimateBombScreenShake();
        }

        public RectTransform CreateFlyingBombIcon(Sprite sprite, RectTransform sourceIconRect, Vector2 fallbackSize)
        {
            var icon = CreateFlyingIcon();
            var image = icon.GetComponent<Image>();
            image.sprite = sprite != null ? sprite : BlockTapRuntimeResources.WhiteSprite;
            image.color = Color.white;
            image.preserveAspect = true;

            var rect = icon.GetComponent<RectTransform>();
            rect.SetParent(flyingContainer != null ? flyingContainer : effectsRoot, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = ResolveFlyingIconSize(sourceIconRect, fallbackSize);
            rect.position = sourceIconRect != null ? sourceIconRect.position : rect.position;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            return rect;
        }

        public Tween AnimateBombIconToReady(RectTransform icon)
        {
            if (icon == null)
            {
                return null;
            }

            icon.DOKill();
            icon.transform.DOKill();
            var readyPosition = GetBombReadyScreenPosition();
            return DOTween.Sequence()
                .Append(icon.DOMove(readyPosition, BombFlyDuration).SetEase(Ease.OutCubic))
                .Join(icon.DOScale(BombReadyScale, BombFlyDuration).SetEase(Ease.OutBack))
                .SetTarget(icon)
                .OnComplete(() =>
                {
                    if (icon == null)
                    {
                        return;
                    }

                    icon.DOKill();
                    icon.transform.DOKill();
                    DOTween.Sequence()
                        .Append(icon.DOMoveY(readyPosition.y + 14f, BombReadyHoverDuration).SetEase(Ease.InOutSine))
                        .Append(icon.DOMoveY(readyPosition.y, BombReadyHoverDuration).SetEase(Ease.InOutSine))
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetTarget(icon);
                });
        }

        public Tween AnimateBombIconToTarget(RectTransform icon, Vector3 targetScreenPosition)
        {
            if (icon == null)
            {
                return null;
            }

            icon.DOKill();
            icon.transform.DOKill();
            return DOTween.Sequence()
                .Append(icon.DOMove(targetScreenPosition, BombFlyDuration).SetEase(Ease.InBack))
                .Join(icon.DOScale(0.6f, BombFlyDuration).SetEase(Ease.InBack))
                .SetTarget(icon);
        }

        public void ReleaseFlyingBombIcon(RectTransform icon)
        {
            if (icon == null)
            {
                return;
            }

            icon.DOKill();
            icon.transform.DOKill();
            DOTween.Kill(icon);
            DOTween.Kill(icon.transform);
            Object.Destroy(icon.gameObject);
        }

        public Tween AnimateBombShockwave(BlockTapBlockView block, Vector2 basePosition, Vector2 pushOffset, float delay)
        {
            if (block == null)
            {
                return null;
            }

            var rect = block.RectTransform;
            var blockTransform = block.transform;
            rect.DOKill();
            blockTransform.DOKill();
            blockTransform.localScale = Vector3.one;
            return DOTween.Sequence()
                .AppendInterval(delay)
                .Append(rect.DOAnchorPos(basePosition + pushOffset, BombShockwavePushDuration).SetEase(Ease.OutQuad))
                .Join(blockTransform.DOScale(BombShockwaveScaleUp, BombShockwavePushDuration).SetEase(Ease.OutQuad))
                .Append(rect.DOAnchorPos(basePosition, BombShockwaveReturnDuration).SetEase(Ease.OutBack))
                .Join(blockTransform.DOScale(BombShockwaveScaleDown, BombShockwaveReturnDuration * 0.5f).SetEase(Ease.InQuad))
                .Append(blockTransform.DOScale(1f, BombShockwaveReturnDuration * 0.5f).SetEase(Ease.OutBack));
        }

        public Tween AnimateBoardShiftOut()
        {
            return boardRoot.DOAnchorPosX(-1100f, config.boardSlideDuration).SetEase(Ease.InSine);
        }

        public Tween AnimateBoardShiftIn()
        {
            boardRoot.anchoredPosition = new Vector2(1100f, boardRoot.anchoredPosition.y);
            return boardRoot.DOAnchorPos(Vector2.zero, config.boardSlideDuration).SetEase(Ease.OutSine);
        }

        public Tween AnimateBombScreenShake()
        {
            if (screenRoot == null)
            {
                return null;
            }

            var originalLocalPosition = screenRoot.localPosition;
            var randomDirection = Random.insideUnitCircle.normalized;
            if (randomDirection.sqrMagnitude < 0.001f)
            {
                randomDirection = Vector2.right;
            }

            screenRoot.DOKill();
            return screenRoot.transform
                .DOPunchPosition((Vector3)(randomDirection * BombScreenShakeStrength), BombScreenShakeDuration, BombScreenShakeVibrato, 0.8f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => screenRoot.localPosition = originalLocalPosition);
        }

        private GameObject CreateFlyingIcon()
        {
            var instance = new GameObject("FlyingBombIcon", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            instance.transform.SetParent(flyingContainer != null ? flyingContainer : effectsRoot, false);
            return instance;
        }

        private Vector3 GetBombReadyScreenPosition()
        {
            var worldPosition = boardRoot.TransformPoint(new Vector3(0f, boardRoot.rect.height * 0.5f + ReadyBombOffsetY, 0f));
            return worldPosition;
        }

        private static Vector2 ResolveFlyingIconSize(RectTransform sourceRect, Vector2 fallbackSize)
        {
            if (sourceRect != null)
            {
                return sourceRect.rect.size;
            }

            return fallbackSize.sqrMagnitude > 0f ? fallbackSize : new Vector2(96f, 96f);
        }

        private void PlayEliminationEffect(Vector3 worldPosition, Vector2 size)
        {
            if (effectsRoot == null || eliminationEffectPrefab == null)
            {
                return;
            }

            var instance = Object.Instantiate(eliminationEffectPrefab, effectsRoot);
            var rect = instance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = size;
                rect.position = worldPosition;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
            }

            Object.Destroy(instance, EliminationEffectLifetime);
        }

    }
}

