using Sirenix.OdinInspector;
using System.Collections;
using Obfuz;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// UI Page 基类
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Animation))]
[ObfuzIgnore]
public abstract class UIPageBase : MonoBehaviour, IPoolElement
{
    [SerializeField, FoldoutGroup("设置")]
    private string m_pageId = string.Empty;
    [SerializeField, FoldoutGroup("设置")]
    private E_UILayer m_layer = E_UILayer.BaseLayer;
    [SerializeField, FoldoutGroup("设置")]
    private bool m_multiPages = true;
    [SerializeField, FoldoutGroup("设置")]
    private bool m_cache = false;
    [SerializeField, FoldoutGroup("设置")]
    private bool m_nativeClose = false;
    [SerializeField, FoldoutGroup("设置")]
    private int m_zIndex = 1000;

    public PageId PageId
    {
        get
        {
            return string.IsNullOrEmpty(m_pageId) ? new PageId(GetType().Name) : new PageId(m_pageId);
        }
    }

    public string RawPageId
    {
        get => m_pageId;
        set => m_pageId = value ?? string.Empty;
    }

    public E_UILayer Layer
    {
        get => m_layer;
        set => m_layer = value;
    }

    public bool MultiPages
    {
        get => m_multiPages;
        set => m_multiPages = value;
    }

    public bool Cache
    {
        get => m_cache;
        set => m_cache = value;
    }

    public bool NativeClose
    {
        get => m_nativeClose;
        set => m_nativeClose = value;
    }

    public int ZIndex
    {
        get => m_zIndex;
        set => m_zIndex = value;
    }
    private Animation m_animation;
    public Animation Animation
    {
        get
        {
            if (!m_animation)
                m_animation = GetComponent<Animation>();

            return m_animation;
        }
    }

    public float Alpha
    {
        set
        {
            var canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            canvasGroup.alpha = value;
        }
    }

    private void Awake()
    {
        if (!m_animation)
        {
            m_animation = GetComponent<Animation>();
        }

        OnAwake();
    }

    private RectTransform m_rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (!m_rectTransform)
            {
                m_rectTransform = transform as RectTransform;
            }
            return m_rectTransform;
        }
    }

    public void OpenPage_CallByFramework()
    {
        try
        {
            OnOpen();
            ShowPage();
        }
        catch (System.Exception e)
        {
            Debug.LogError("打开界面内部报错: " + e.ToString() + "堆栈:" + e.StackTrace);
        }
    }

    public void ClosePage_CallByFramework()
    {
        try
        {
            OnHide();
            HidePage();
        }
        catch (System.Exception e)
        {
            Debug.LogError("关闭界面内部报错: " + e.ToString() + "堆栈:" + e.StackTrace);
        }
    }

    protected virtual void AfterShow()
    {
        Bizza.Channel.Analytics.Manager.ShowViewEvent(PageId.ToString());
    }

    protected void ShowPage()
    {
        if (!string.IsNullOrEmpty(m_animOnShow))
        {
            PlayAnimation(m_animOnShow);
        }
        else
        {
            OnShow();
            AfterShow();
        }
        SetListener(true);
    }

    protected void HidePage()
    {
        if (!string.IsNullOrEmpty(m_animOnHide))
        {
            PlayAnimation(m_animOnHide);
        }
        else
        {
            OnClose();
            UIModule.Instance.ClosePage_CallByFramework(this);
        }
        SetListener(false);
    }

    protected virtual void OnAwake() { }

    protected abstract void OnOpen();

    protected virtual void OnShow()
    {
    }

    protected virtual void OnHide()
    {
    }

    protected abstract void OnClose();

    protected virtual void SetListener(bool addOrRemove){}

    #region Animation

    [SerializeField, FoldoutGroup("PageBase")]
    private string m_animOnShow = "";
    [SerializeField, FoldoutGroup("PageBase")]
    private string m_animOnHide = "";

    protected void PlayAnimation(string animation)
    {
        if (!m_animation)
        {
            m_animation = GetComponent<Animation>();
        }

        if (m_animOnHide == animation)
        {
            m_animation.Stop();
        }
        StartCoroutine(PlayAnimationCortious(animation));
    }

    private IEnumerator PlayAnimationCortious(string animation)
    {
        m_animation.Play(animation);
        while (m_animation.IsPlaying(animation))
        {
            yield return null;
        }
        AnimationPlayFinish(animation);
    }
    private void AnimationPlayFinish(string animationName)
    {
        if (animationName == m_animOnHide)
        {
            OnClose();
            UIModule.Instance.ClosePage_CallByFramework(this);
            return;
        }

        if (animationName == m_animOnShow)
        {
            OnShow();
            AfterShow();
            return;
        }

        OnAnimationFinish(animationName);
    }
    protected virtual void OnAnimationFinish(string animationName)
    {

    }

    public void CloseSelf()
    {
        UIModule.Instance.ClosePage(this);
    }

    public void CloseSelf(float delay)
    {
        TransparentBlock.AddBlock(this);
        GameUtils.DelayDo(() =>
        {
            UIModule.Instance.ClosePage(this);
            TransparentBlock.RemoveBlock(this);
        }, delay);

    }

    #endregion

    public virtual void PoolReset()
    {

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_pageId))
        {
            m_pageId = GetType().Name;
        }
    }
#endif
}
[ObfuzIgnore]
public abstract class UIPageBase<A> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a);

    public void OpenPage_CallByFramework(A a)
    {
        OnOpen(a);
        ShowPage();
    }
}
[ObfuzIgnore]
public abstract class UIPageBase<A, B> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b);

    public void OpenPage_CallByFramework(A a, B b)
    {
        OnOpen(a, b);
        ShowPage();
    }
}
[ObfuzIgnore]
public abstract class UIPageBase<A, B, C> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c);

    public void OpenPage_CallByFramework(A a, B b, C c)
    {
        OnOpen(a, b, c);
        ShowPage();
    }
}
[ObfuzIgnore]
public abstract class UIPageBase<A, B, C, D> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c, D d);
    public void OpenPage_CallByFramework(A a, B b, C c, D d)
    {
        OnOpen(a, b, c, d);
        ShowPage();
    }
}
[ObfuzIgnore]
public abstract class UIPageBase<A, B, C, D, E> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c, D d, E e);
    public void OpenPage_CallByFramework(A a, B b, C c, D d, E e)
    {
        OnOpen(a, b, c, d, e);
        ShowPage();
    }
}
