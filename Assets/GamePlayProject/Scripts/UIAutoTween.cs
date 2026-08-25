using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIAutoTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum TweenTrigger
    {
        OnEnable,
        OnClick,
        OnPress,
        OnHover,
        Loop
    }

    public enum TweenEffect
    {
        PunchScale,
        PunchRotate,
        FadeInOut,
        FadeWave,
        JellyBounce,
        LoopScale,
        Jelly,
        HoverPunch,
        HoverWave,
        ClickWave
    }

    [Serializable]
    public class TweenSetting
    {
        public bool enabled = true;
        public TweenEffect tweenEffect = TweenEffect.PunchScale;
        public float duration = 0.3f;
        public float power = 0.5f;
        public bool resumeLoopAfter = true;
    }

    [Header("Tween Settings Per State")]
    public TweenSetting onEnableSetting = new TweenSetting { enabled = false };
    public TweenSetting onClickSetting = new TweenSetting { enabled = false };
    public TweenSetting onPressSetting = new TweenSetting { enabled = false };
    public TweenSetting onHoverSetting = new TweenSetting { enabled = false };
    public TweenSetting loopSetting = new TweenSetting { enabled = false, tweenEffect = TweenEffect.LoopScale, duration = 2f, power = 0.5f };

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private CanvasGroup canvasGroup;
    private Graphic graphic;

    private Tween loopTween;
    private Tween currentTween;

    private bool pointerOver = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        graphic = GetComponent<Graphic>();

        if ((onEnableSetting.tweenEffect == TweenEffect.FadeInOut || onEnableSetting.tweenEffect == TweenEffect.FadeWave ||
             onClickSetting.tweenEffect == TweenEffect.FadeInOut || onClickSetting.tweenEffect == TweenEffect.FadeWave ||
             onPressSetting.tweenEffect == TweenEffect.FadeInOut || onPressSetting.tweenEffect == TweenEffect.FadeWave ||
             onHoverSetting.tweenEffect == TweenEffect.FadeInOut || onHoverSetting.tweenEffect == TweenEffect.FadeWave ||
             loopSetting.tweenEffect == TweenEffect.FadeInOut || loopSetting.tweenEffect == TweenEffect.FadeWave)
            && canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        StopAllTweens();        // 确保所有旧 Tween 被清除
        ResetToOriginal();      // 重置 Transform 和透明度

        if (onEnableSetting.enabled)
            PlayTweenForState(onEnableSetting);

        if (loopSetting.enabled)
            StartLoopTween();
    }


    private void OnDisable()
    {
        StopAllTweens();
    }

    public void ResetState()
    {
        StopAllTweens();
    }

    private void StopAllTweens()
    {
        loopTween?.Kill();
        currentTween?.Kill();
        DOTween.Kill(this);
        ResetToOriginal();
    }

    private void ResetToOriginal()
    {
        rectTransform.localScale = originalScale;
        rectTransform.rotation = Quaternion.identity;
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        if (graphic != null)
            graphic.color = graphic.color.WithAlpha(1f);
    }

    private void StartLoopTween()
    {
        if (!loopSetting.enabled) return;

        loopTween?.Kill();
        loopTween = CreateTween(loopSetting, loop: true);

        if (loopTween != null)
        {
            // 只有 LoopScale 使用 Yoyo，其它使用 Restart 循环
            var loopType = loopSetting.tweenEffect == TweenEffect.LoopScale ? LoopType.Yoyo : LoopType.Restart;
            loopTween.SetLoops(-1, loopType)
                     .SetEase(Ease.InOutSine)
                     .SetId(this);
        }
    }


    private Tween CreateTween(TweenSetting setting, bool loop = false)
    {
        switch (setting.tweenEffect)
        {
            case TweenEffect.PunchScale:
                rectTransform.localScale = originalScale;
                return rectTransform.DOPunchScale(Vector3.one * 0.1f * setting.power, setting.duration);
            case TweenEffect.PunchRotate:
                return rectTransform.DOPunchRotation(new Vector3(0, 0, 30f) * setting.power, setting.duration, (int)(10 * setting.power), setting.power);
            case TweenEffect.FadeInOut:
                if (canvasGroup != null)
                    return DOTween.Sequence()
                        .Append(canvasGroup.DOFade(Mathf.Clamp01(1 - setting.power), 0.5f * setting.duration))
                        .Append(canvasGroup.DOFade(1f, 0.5f * setting.duration));
                else if (graphic != null)
                    return DOTween.Sequence()
                        .Append(graphic.DOFade(Mathf.Clamp01(1 - setting.power), 0.5f * setting.duration))
                        .Append(graphic.DOFade(1f, 0.5f * setting.duration));
                return null;
            case TweenEffect.FadeWave:
                if (canvasGroup != null)
                    return DOTween.Sequence()
                        .Append(canvasGroup.DOFade(1f, 0.5f * setting.duration))
                        .Append(canvasGroup.DOFade(0f, 0.5f * setting.duration));
                else if (graphic != null)
                    return DOTween.Sequence()
                        .Append(graphic.DOFade(1f, 0.5f * setting.duration))
                        .Append(graphic.DOFade(0f, 0.5f * setting.duration));
                return null;
            case TweenEffect.JellyBounce:
                return DOTween.Sequence()
                    .Append(rectTransform.DOScale(new Vector3(
                        originalScale.x + 0.1f * setting.power,
                        originalScale.y - 0.1f * setting.power,
                        1f), 0.33f * setting.duration))
                    .Append(rectTransform.DOScale(new Vector3(
                        originalScale.x - 0.1f * setting.power,
                        originalScale.y + 0.05f * setting.power,
                        1f), 0.33f * setting.duration))
                    .Append(rectTransform.DOScale(originalScale, 0.34f * setting.duration));
            case TweenEffect.Jelly:
                return DOTween.Sequence()
                    .Append(rectTransform.DOScale(new Vector3(1f + 0.15f * setting.power, 0.85f, 1f), setting.duration * 0.33f))
                    .Append(rectTransform.DOScale(new Vector3(0.95f, 1f + 0.05f * setting.power, 1f), setting.duration * 0.33f))
                    .Append(rectTransform.DOScale(originalScale, setting.duration * 0.34f));
            case TweenEffect.LoopScale:
                if (!loop) return null;
                return rectTransform.DOScale(originalScale * (1.05f + 0.05f * setting.power), setting.duration / 2);
            case TweenEffect.HoverPunch:
                return rectTransform.DOPunchScale(Vector3.one * 0.1f * setting.power, setting.duration);
            case TweenEffect.HoverWave:
                return DOTween.Sequence()
                    .Append(rectTransform.DOScale(originalScale * (1f + 0.05f * setting.power), 0.5f * setting.duration))
                    .Append(rectTransform.DOScale(originalScale, 0.5f * setting.duration));
            case TweenEffect.ClickWave:
                return DOTween.Sequence()
                    .Append(rectTransform.DOScale(originalScale * (1f + 0.15f * setting.power), 0.4f * setting.duration))
                    .Append(rectTransform.DOScale(originalScale, 0.6f * setting.duration));
            default:
                return rectTransform.DOPunchScale(Vector3.one * 0.1f * setting.power, setting.duration);
        }
    }

    private void PlayTweenForState(TweenSetting setting)
    {
        if (!setting.enabled) return;

        loopTween?.Kill();
        currentTween?.Kill();

        currentTween = CreateTween(setting);

        if (currentTween == null) return;

        currentTween.SetId(this);

        if (setting.resumeLoopAfter && loopSetting.enabled)
        {
            currentTween.OnComplete(() =>
            {
                if (!pointerOver && !IsPressed())
                    StartLoopTween();
            });
        }
    }

    private bool IsPressed() => false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!onPressSetting.enabled) return;

        loopTween?.Kill();
        currentTween?.Kill();

        currentTween = rectTransform.DOScale(originalScale * 0.9f, 0.1f).SetEase(Ease.OutQuad).SetId(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!onPressSetting.enabled) return;

        currentTween = rectTransform.DOScale(originalScale * 1.05f, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() => rectTransform.DOScale(originalScale, 0.1f)).SetId(this);

        if (onClickSetting.enabled)
        {
            PlayTweenForState(onClickSetting);
        }
        else if (loopSetting.enabled)
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (!pointerOver && !IsPressed())
                    StartLoopTween();
            });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
        if (onHoverSetting.enabled)
        {
            PlayTweenForState(onHoverSetting);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        if (onHoverSetting.enabled)
        {
            currentTween?.Kill();
            rectTransform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad).SetId(this);
            if (loopSetting.enabled)
                StartLoopTween();
        }
    }

    public void TriggerTween(TweenTrigger trigger)
    {
        switch (trigger)
        {
            case TweenTrigger.OnClick:
                if (onClickSetting.enabled)
                    PlayTweenForState(onClickSetting);
                break;
            case TweenTrigger.OnPress:
                if (onPressSetting.enabled)
                    PlayTweenForState(onPressSetting);
                break;
            case TweenTrigger.OnHover:
                if (onHoverSetting.enabled)
                    PlayTweenForState(onHoverSetting);
                break;
            case TweenTrigger.OnEnable:
                if (onEnableSetting.enabled)
                    PlayTweenForState(onEnableSetting);
                break;
            case TweenTrigger.Loop:
                if (loopSetting.enabled)
                    StartLoopTween();
                break;
        }
    }
}

public static class ColorExtensions
{
    public static Color WithAlpha(this Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }
}
