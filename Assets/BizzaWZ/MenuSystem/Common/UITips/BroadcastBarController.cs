using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BroadcastBarController : BaseSingleton<BroadcastBarController>
{
    public RectTransform barTransform;

    public GameObject textObj;
    public TextMeshProUGUI textComponent;

    public GameObject entryObj;

    public GameObject entryAObj;
    public Image entryAImage;
    public TMP_Text entryAText;

    public GameObject entryBObj;
    public Image entryBImage;
    public TMP_Text entryBText;

    public float stayDuration = 1.5f;
    public float animDuration = 0.5f;
    public float minInterval = 0.3f; // 300ms 内禁止重复触发

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private Sequence currentTween;
    private float txtlastShowTime = -10f;
    private float itemlastShowTime = -10f;


    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        InitPositions();
        barTransform.anchoredPosition = hiddenPos;
        gameObject.SetActive(false);
    }

    void InitPositions()
    {
        float barHeight = barTransform.rect.height;
        hiddenPos = new Vector2(0, barHeight);
        visiblePos = new Vector2(0, 0);
    }

    public void ShowMessage(string message)
    {
        if (Time.time - txtlastShowTime < minInterval)
            return;

        textObj.SetActive(true);
        entryObj.SetActive(false);

        txtlastShowTime = Time.time;

        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        InitPositions(); // 确保分辨率变化时位置正确
        textComponent.text = message;
        gameObject.SetActive(true);

        barTransform.anchoredPosition = hiddenPos;
        CanvasGroup canvasGroup = GetOrAddCanvasGroup();
        canvasGroup.alpha = 0;

        currentTween = DOTween.Sequence().SetTarget(gameObject);
        currentTween.Append(barTransform.DOAnchorPos(visiblePos, animDuration).SetEase(Ease.OutCubic));
        currentTween.Join(canvasGroup.DOFade(1f, animDuration));
        currentTween.AppendInterval(stayDuration);
        currentTween.Append(barTransform.DOAnchorPos(hiddenPos, animDuration).SetEase(Ease.InCubic));
        currentTween.Join(canvasGroup.DOFade(0f, animDuration));
        currentTween.OnComplete(() => gameObject.SetActive(false)).SetTarget(gameObject);
    }

    public void ShowMessage(ItemEntry a, ItemEntry b)
    {
        if (Time.time - itemlastShowTime < minInterval)
            return;

        // 第一个是金币
        if (a.Type != E_ItemType.Gold)
        {
            var c = new ItemEntry();
            c = a;
            a = b;
            b = c;
        }
        itemlastShowTime = Time.time;

        textObj.SetActive(false);
        entryObj.SetActive(true);

        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        InitPositions(); // 确保分辨率变化时位置正确
        entryAObj.SetActive(false);
        if (!a.Equals(default(ItemEntry)))
        {
            entryAObj.gameObject.SetActive(true);
            // entryAImage.sprite = ItemUtils.GetItemIcon(a);
            entryAText.text = ItemUtils.GetItemText(a);
        }

        bool singleCurrencyMode = false;
#if BIZZA_REAL_WITHDRAW
        singleCurrencyMode = ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode;
#endif
        if (singleCurrencyMode)
        {
            entryAImage.sprite = entryBImage.sprite;
            entryBObj.SetActive(false);
        }
        else
        {
            entryBObj.SetActive(false);
            if (!b.Equals(default(ItemEntry)))
            {
                entryBObj.gameObject.SetActive(true);
                // entryBImage.sprite = ItemUtils.GetItemIcon(b);
                entryBText.text = ItemUtils.GetItemText(b);
            }
        }

        gameObject.SetActive(true);

        barTransform.anchoredPosition = hiddenPos;
        CanvasGroup canvasGroup = GetOrAddCanvasGroup();
        canvasGroup.alpha = 0;

        currentTween = DOTween.Sequence().SetTarget(gameObject);
        currentTween.Append(barTransform.DOAnchorPos(visiblePos, animDuration).SetEase(Ease.OutCubic));
        currentTween.Join(canvasGroup.DOFade(1f, animDuration));
        currentTween.AppendInterval(stayDuration);
        currentTween.Append(barTransform.DOAnchorPos(hiddenPos, animDuration).SetEase(Ease.InCubic));
        currentTween.Join(canvasGroup.DOFade(0f, animDuration));
        currentTween.OnComplete(() => gameObject.SetActive(false)).SetTarget(gameObject);
    }

    private CanvasGroup GetOrAddCanvasGroup()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

}
