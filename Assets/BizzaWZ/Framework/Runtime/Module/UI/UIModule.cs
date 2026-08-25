using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Obfuz;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

 
public class UIModule : BaseGameModule<UIModule>
{
    [SerializeField] private RectTransform m_pageRoot;
    [SerializeField] private UIAnimationConfig m_animationConfig;
    public Transform fxContainer;

    private readonly Dictionary<string, UIPageBase> _prefabsById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, UIPageRuntimeConfig> _configById = new(StringComparer.Ordinal);

    private List<UIPageBase> _pagesOpened;
    private List<UIPageBase> _pagePool;
    private List<RectTransform> _pageLayerRoots;

    private int _openingCount;
    private Canvas _uiCanvas;
    private IUIPageLoader _pageLoader;


    public bool Opening => _openingCount > 0;
    public Camera UICamera => GetUICanvas().GetComponentInChildren<Camera>();
    public Canvas UICanvas => GetUICanvas();

    [ShowInInspector]
    public bool HasPopup => HasPageInLayer((int)E_UILayer.PopupLayer);

    public void SetPageLoader(IUIPageLoader pageLoader)
    {
        _pageLoader = pageLoader;
    }

    public override void InitGameModule()
    {
        _pagesOpened = new List<UIPageBase>();
        _pagePool = new List<UIPageBase>();
        _pageLayerRoots = new List<RectTransform>();
        _pageLoader ??= CreateDefaultPageLoader();

        BuildLayerRoots();

        m_loadingHandlerPool = new Pool<LoadingHandler>(CreateLoadingHandler);
        m_loadingShow = new Dictionary<PageId, LoadingHandler>();
        m_fullScreenHandler = new LoadingHandler();
        m_fullScreenHandler.PoolReset();
    }

    public override void ReleaseGameModule()
    {
        ReleaseAllPage();
    }

    private static IUIPageLoader CreateDefaultPageLoader()
    {
        return new AddressablesUIPageLoader();
        return AssetUtils.LoadAssetType == AssetUtils.E_LoadAssetType.Addressable
            ? new AddressablesUIPageLoader()
            : new ResourcesUIPageLoader();
    }

    public UIAnimationConfig.UIAnimationConfigData GetUIAnimationConfigById(int id)
    {
        foreach (var config in m_animationConfig.Datas)
        {
            if (config.Id == id)
            {
                return config;
            }
        }

        return null;
    }

    private void BuildLayerRoots()
    {
        _pageLayerRoots.Clear();
        int layerCount = Enum.GetValues(typeof(E_UILayer)).Length;
        for (int i = 0; i < layerCount; i++)
        {
            string layerName = ((E_UILayer)i).ToString();
            var layer = m_pageRoot.Find(layerName);
            if (layer != null && layer.TryGetComponent<RectTransform>(out var existing))
            {
                _pageLayerRoots.Add(existing);
                continue;
            }

            var layerRoot = new GameObject(layerName);
            var rt = layerRoot.AddComponent<RectTransform>();
            rt.SetParent(m_pageRoot);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.SetAsLastSibling();
            _pageLayerRoots.Add(rt);
        }
    }

    public int GetLayerByName(string layerName)
    {
        if (string.IsNullOrEmpty(layerName)) return -1;
        return Enum.TryParse<E_UILayer>(layerName, out var layer) ? (int)layer : -1;
    }

    public bool HasPageInLayer(int layerId)
    {
        if (_pagesOpened == null) return false;
        foreach (var page in _pagesOpened)
        {
            var config = GetPageRuntimeConfig(page);
            if ((int)config.Layer == layerId)
            {
                return true;
            }
        }

        return false;
    }

    public T GetPage<T>() where T : UIPageBase
    {
        foreach (var page in _pagesOpened)
        {
            if (page.GetType() == typeof(T))
            {
                return (T)page;
            }
        }

        return default;
    }

    public UIPageBase GetPage(PageId pageId)
    {
        if (pageId.IsEmpty) return null;
        var targetId = ResolveCanonicalPageId(pageId);
        foreach (var page in _pagesOpened)
        {
            if (page.PageId == targetId)
            {
                return page;
            }
        }

        return null;
    }

    private PageId ResolveCanonicalPageId(PageId pageId)
    {
        if (pageId.IsEmpty) return PageId.Empty;
        return _configById.TryGetValue(pageId.Value, out var config) ? config.PageId : pageId;
    }

    public void OpenPageSync(PageId pageId)
    {
        OpenPage(pageId).Forget();
    }

    public async UniTask<UIPageBase> OpenPage(PageId pageId)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null)
        {
            LogLogger.LogError($"打开界面失败: {pageId}");
            return null;
        }

        uiPage.OpenPage_CallByFramework();
        return uiPage;
    }

    public async UniTask<UIPageBase<A>> OpenPage<A>(PageId pageId, A a)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null || !(uiPage is UIPageBase<A> page))
        {
            LogLogger.LogError($"打开界面失败(类型不匹配): {pageId}");
            return null;
        }

        page.OpenPage_CallByFramework(a);
        return page;
    }

    public async UniTask<UIPageBase<A, B>> OpenPage<A, B>(PageId pageId, A a, B b)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null || !(uiPage is UIPageBase<A, B> page))
        {
            LogLogger.LogError($"打开界面失败(类型不匹配): {pageId}");
            return null;
        }

        page.OpenPage_CallByFramework(a, b);
        return page;
    }

    public async UniTask<UIPageBase<A, B, C>> OpenPage<A, B, C>(PageId pageId, A a, B b, C c)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C> page))
        {
            LogLogger.LogError($"打开界面失败(类型不匹配): {pageId}");
            return null;
        }

        page.OpenPage_CallByFramework(a, b, c);
        return page;
    }

    public async UniTask<UIPageBase<A, B, C, D>> OpenPage<A, B, C, D>(PageId pageId, A a, B b, C c, D d)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C, D> page))
        {
            LogLogger.LogError($"打开界面失败(类型不匹配): {pageId}");
            return null;
        }

        page.OpenPage_CallByFramework(a, b, c, d);
        return page;
    }

    public async UniTask<UIPageBase<A, B, C, D, E>> OpenPage<A, B, C, D, E>(PageId pageId, A a, B b, C c, D d, E e)
    {
        var uiPage = await _OpenPage(pageId);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C, D, E> page))
        {
            LogLogger.LogError($"打开界面失败(类型不匹配): {pageId}");
            return null;
        }

        page.OpenPage_CallByFramework(a, b, c, d, e);
        return page;
    }

    private bool LayerMultiPages(E_UILayer layer)
    {
        return layer is not E_UILayer.BaseLayer;
 //        return layer is E_UILayer.PopupLayer or E_UILayer.TopLayer or E_UILayer.SystemLayer or E_UILayer.TeachLayer;
    }

    private bool LayerIgnoreLowLayerManage(E_UILayer layer)
    {
        return layer == E_UILayer.SystemLayer;
    }

    private UIPageRuntimeConfig GetPageRuntimeConfig(UIPageBase page)
    {
        if (_configById.TryGetValue(page.PageId.Value, out var config))
        {
            return config;
        }

        return UIPageRuntimeConfig.FromPage(page);
    }

    private async UniTask<bool> EnsurePageConfigLoaded(PageId pageId)
    {
        if (pageId.IsEmpty) return false;
        if (_configById.ContainsKey(pageId.Value)) return true;

        var prefab = await _pageLoader.LoadPagePrefabAsync(pageId);
        if (prefab == null)
        {
            LogLogger.LogError($"加载页面失败: {pageId}");
            return false;
        }

        RegisterPrefab(pageId, prefab);
        return true;
    }

    private void RegisterPrefab(PageId requestId, UIPageBase prefab)
    {
        var canonicalId = prefab.PageId.IsEmpty ? requestId : prefab.PageId;
        if (canonicalId.IsEmpty)
        {
            canonicalId = new PageId(prefab.name);
        }

        var config = UIPageRuntimeConfig.FromPage(prefab);
        config.PageId = canonicalId;

        _prefabsById[canonicalId.Value] = prefab;
        _configById[canonicalId.Value] = config;

        if (!requestId.IsEmpty && requestId != canonicalId)
        {
            _configById[requestId.Value] = config;
            _prefabsById[requestId.Value] = prefab;
        }
    }

    private void InitPage(UIPageBase pageBaseClone, UIPageRuntimeConfig config)
    {
        pageBaseClone.RectTransform.anchoredPosition = Vector2.zero;
        pageBaseClone.RectTransform.localScale = Vector3.one;
        if (config.ZIndex >= 1000)
        {
            pageBaseClone.RectTransform.SetAsLastSibling();
        }
        else
        {
            var layerRoot = _pageLayerRoots[(int)config.Layer];
            bool min = true;
            for (int i = layerRoot.childCount - 1; i > -1; i--)
            {
                var child = layerRoot.GetChild(i);
                if (!child.TryGetComponent<UIPageBase>(out var childPage) || childPage == pageBaseClone)
                {
                    continue;
                }

                var childConfig = GetPageRuntimeConfig(childPage);
                if (config.ZIndex >= childConfig.ZIndex)
                {
                    pageBaseClone.RectTransform.SetSiblingIndex(i + 1);
                    min = false;
                    break;
                }
            }

            if (min)
            {
                pageBaseClone.RectTransform.SetAsFirstSibling();
            }
        }

        _pagesOpened.Add(pageBaseClone);
        pageBaseClone.gameObject.SetActive(true);
        BizzaEventSystem.Emit(EventDefine.Frame.OpenPage, config.PageId);
    }

    private async UniTask<UIPageBase> _OpenPage(PageId pageId)
    {
        if (!await EnsurePageConfigLoaded(pageId))
        {
            return null;
        }

        var config = _configById[pageId.Value];
        _openingCount++;

        if (!LayerMultiPages(config.Layer) || !config.MultiPages)
        {
            for (int i = _pagesOpened.Count - 1; i > -1; i--)
            {
                var page = _pagesOpened[i];
                var openedConfig = GetPageRuntimeConfig(page);
                if (openedConfig.PageId == config.PageId)
                {
                    _openingCount--;
                    return page;
                }
            }
        }

        UIPageBase pageBaseClone = null;
        for (int i = 0; i < _pagePool.Count; i++)
        {
            var poolConfig = GetPageRuntimeConfig(_pagePool[i]);
            if (poolConfig.PageId == config.PageId)
            {
                pageBaseClone = _pagePool[i];
                _pagePool.RemoveAt(i);
                break;
            }
        }

        if (!pageBaseClone)
        {
            var prefab = _prefabsById[config.PageId.Value];
            int _layerIndex = (int)config.Layer;
            pageBaseClone = Instantiate(prefab, _pageLayerRoots[_layerIndex]);
        }

        if (!LayerMultiPages(config.Layer))
        {
            for (int i = _pagesOpened.Count - 1; i > -1; i--)
            {
                var openedConfig = GetPageRuntimeConfig(_pagesOpened[i]);
                if (openedConfig.Layer == config.Layer)
                {
                    ClosePage(_pagesOpened[i]);
                    i = Mathf.Min(i, _pagesOpened.Count - 1);
                }
                else if ((int)config.Layer < (int)openedConfig.Layer && !LayerIgnoreLowLayerManage(openedConfig.Layer))
                {
                    ClosePage(_pagesOpened[i]);
                    i = Mathf.Min(i, _pagesOpened.Count - 1);
                }
            }
        }

        InitPage(pageBaseClone, config);
        _openingCount--;
        return pageBaseClone;
    }

    public void ClosePage(UIPageBase pageBase)
    {
        if (pageBase == null)
        {
            Debug.LogError("试图关闭一个null的UIPage");
            return;
        }

        if (_pagesOpened.Contains(pageBase))
        {
            _pagesOpened.Remove(pageBase);
        }

        pageBase.ClosePage_CallByFramework();
        BizzaEventSystem.Emit(EventDefine.Frame.ClosePage, pageBase.PageId);
    }

    public void ClosePage(PageId pageId, bool onlyLast = false)
    {
        if (pageId.IsEmpty) return;
        var targetId = ResolveCanonicalPageId(pageId);
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            if (_pagesOpened[i].PageId == targetId)
            {
                ClosePage(_pagesOpened[i]);
                i = Mathf.Min(i, _pagesOpened.Count - 1);
                if (onlyLast) break;
            }
        }
    }

    public void CloseLayerAllPage(int layer)
    {
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            var config = GetPageRuntimeConfig(_pagesOpened[i]);
            if ((int)config.Layer == layer)
            {
                ClosePage(_pagesOpened[i]);
                i = Mathf.Min(i, _pagesOpened.Count - 1);
            }
        }
    }

    public void CloseAndDestroyLayerAllPage(int layer)
    {
        var temp = new List<UIPageBase>();
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            var config = GetPageRuntimeConfig(_pagesOpened[i]);
            if ((int)config.Layer == layer)
            {
                temp.Add(_pagesOpened[i]);
            }
        }

        foreach (var page in temp)
        {
            ClosePage(page);
        }

        var removeIndex = new List<int>();
        for (int i = 0; i < _pagePool.Count; i++)
        {
            var config = GetPageRuntimeConfig(_pagePool[i]);
            if ((int)config.Layer == layer)
            {
                removeIndex.Add(i);
            }
        }

        _pagePool.RemoveByIndexs(removeIndex);
        if (_pageLayerRoots.TryGetItem(layer, out var root))
        {
            root.DestroyAllChildren();
        }
    }

    public void CloseAllPage(PageId reShowPageId = default)
    {
        for (int i = 0; i < _pageLayerRoots.Count; i++)
        {
            CloseLayerAllPage(i);
        }

        if (!reShowPageId.IsEmpty)
        {
            OpenPageSync(reShowPageId);
        }
    }

    public void ClosePage_CallByFramework(UIPageBase uiPage)
    {
        var config = GetPageRuntimeConfig(uiPage);
        uiPage.gameObject.SetActive(false);
        if (!config.Cache)
        {
            Destroy(uiPage.gameObject);
        }
        else
        {
            uiPage.PoolReset();
            _pagePool.Add(uiPage);
        }
    }

    public bool PageIsOpen(PageId pageId)
    {
        if (pageId.IsEmpty) return false;
        var targetId = ResolveCanonicalPageId(pageId);
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            if (_pagesOpened[i].PageId == targetId)
            {
                return true;
            }
        }

        return false;
    }

    public UIPageBase FindOpenedPage(PageId pageId)
    {
        if (pageId.IsEmpty) return null;
        var targetId = ResolveCanonicalPageId(pageId);
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            if (_pagesOpened[i].PageId == targetId)
            {
                return _pagesOpened[i];
            }
        }

        return null;
    }

    public UIPageBase FindLastOpenedPage()
    {
        return _pagesOpened.Count > 0 ? _pagesOpened[^1] : null;
    }

    public void ReleasePage(IEnumerable<PageId> pageIds)
    {
        foreach (var pageId in pageIds)
        {
            ReleasePage(pageId);
        }
    }

    public void ReleasePage(PageId pageId)
    {
        if (pageId.IsEmpty) return;
        var targetId = ResolveCanonicalPageId(pageId);
        for (int i = _pagesOpened.Count - 1; i > -1; i--)
        {
            if (_pagesOpened[i].PageId == targetId)
            {
                Destroy(_pagesOpened[i].gameObject);
                _pagesOpened.RemoveAt(i);
            }
        }

        if (_prefabsById.ContainsKey(pageId.Value))
        {
            _prefabsById.Remove(pageId.Value);
            _configById.Remove(pageId.Value);
        }
        if (_prefabsById.ContainsKey(targetId.Value))
        {
            _prefabsById.Remove(targetId.Value);
            _configById.Remove(targetId.Value);
        }
    }

    public void ReleaseAllPage()
    {
        for (int i = _pagesOpened.Count - 1; i >= 0; i--)
        {
            var page = _pagesOpened[i];
            if (page != null)
            {
                Destroy(page.gameObject);
            }
        }

        _pagesOpened.Clear();
        _pagePool.Clear();
        _prefabsById.Clear();
        _configById.Clear();
    }

    public Canvas GetUICanvas()
    {
        if (!_uiCanvas)
        {
            _uiCanvas = m_pageRoot.GetComponentInParent<Canvas>();
        }

        return _uiCanvas;
    }

    public float GetPixelSize(float sceneLength, bool isVertical = false)
    {
        var screenPositionA = Camera.main.WorldToScreenPoint(Vector3.zero);
        var screenPositionB = Camera.main.WorldToScreenPoint(new Vector3(sceneLength, 0, 0));
        return Vector3.Distance(screenPositionA, screenPositionB);
    }

    #region Loading Module

    private Pool<LoadingHandler> m_loadingHandlerPool;
    private Dictionary<PageId, LoadingHandler> m_loadingShow;
    private LoadingHandler m_fullScreenHandler;

    private LoadingHandler CreateLoadingHandler()
    {
        return new LoadingHandler();
    }

    public void HideLoading(PageId loadingType)
    {
        if (!m_loadingShow.TryGetValue(loadingType, out _))
        {
            return;
        }

        ClosePage(loadingType, true);
        m_loadingShow.Remove(loadingType);
    }

    #endregion

    private void Update()
    {
        if (TeachUtil.IsTeach)
        {
           //  LogLogger.LogVerbose(LogTag.LOG_UI, "处在新手教程中 未能关闭界面");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCurrentView();
        }
    }

    private void ExitCurrentView()
    {
        int count = _pagesOpened.Count;
        if (count == 0) return;
        var page = _pagesOpened[count - 1];
        var config = GetPageRuntimeConfig(page);
        if (config.NativeClose)
        {
            ClosePage(page.PageId);
        }
    }
}

 
public class LoadingHandler : IPoolElement
{
    public PageId PageType { get; set; }
    public float Process { get; set; }

    public void PoolReset()
    {
        Process = 0;
        PageType = PageId.Empty;
    }
}

public interface IUIPageLoader
{
    UniTask<UIPageBase> LoadPagePrefabAsync(PageId pageId);
}

public sealed class ResourcesUIPageLoader : IUIPageLoader
{
    private readonly string _rootPath;

    public ResourcesUIPageLoader(string rootPath = "Prefabs/UI")
    {
        _rootPath = rootPath?.Trim('/') ?? "Prefabs/UI";
    }

    public UniTask<UIPageBase> LoadPagePrefabAsync(PageId pageId)
    {
        if (pageId.IsEmpty)
        {
            return UniTask.FromResult<UIPageBase>(null);
        }

        string path = $"{_rootPath}/{pageId.Value}";
        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            return UniTask.FromResult<UIPageBase>(null);
        }

        return UniTask.FromResult(prefab.GetComponent<UIPageBase>());
    }
}

public sealed class AddressablesUIPageLoader : IUIPageLoader
{
    private readonly string _rootPath;

    public AddressablesUIPageLoader(string rootPath = "UIPanel")
    {
        _rootPath = rootPath?.Trim('/') ?? "UIPanel";
    }

    public async UniTask<UIPageBase> LoadPagePrefabAsync(PageId pageId)
    {
        if (pageId.IsEmpty)
        {
            return null;
        }

        string path = $"{_rootPath}/{pageId.Value}";
        var handle = Addressables.LoadAssetAsync<GameObject>(path);
        await handle;
        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            return null;
        }

        return handle.Result.GetComponent<UIPageBase>();
    }
}

internal struct UIPageRuntimeConfig
{
    public PageId PageId;
    public E_UILayer Layer;
    public bool MultiPages;
    public bool Cache;
    public bool NativeClose;
    public int ZIndex;

    public static UIPageRuntimeConfig FromPage(UIPageBase page)
    {
        return new UIPageRuntimeConfig
        {
            PageId = page.PageId,
            Layer = page.Layer,
            MultiPages = page.MultiPages,
            Cache = page.Cache,
            NativeClose = page.NativeClose,
            ZIndex = page.ZIndex
        };
    }
}
