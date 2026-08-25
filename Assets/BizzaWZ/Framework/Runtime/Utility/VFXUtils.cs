using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Bizza.Sdk;
using DG.Tweening;
using UnityEngine;

public static class VFXUtils
{

    public static Dictionary<E_ItemType, Transform> itemFlyTarget = new();

    public static GameObject PlayOnce(string name, Vector3 pos, float duration = 2f, Transform parent = null)
    {
        var prefab = AssetUtils.LoadAssetSync<GameObject>("Prefabs/VFX/" + name);
        var inst = PoolUtil.GetGameObject(prefab);
        if (parent != null)
        {
            inst.transform.SetParent(parent);
        }
        inst.SetActive(true);
        inst.transform.position = pos;
        inst.transform.localScale = Vector3.one;
        GameUtils.DelayDo(() =>
        {
            if (inst != null)
            {
                PoolUtil.ReleaseGameObject(inst);
            }
        }, duration);
        return inst;
    }


    public static GameObject PlayFollow(string name, Transform parent, Vector3 localPos = default)
    {
        var prefab = AssetUtils.LoadAssetSync<GameObject>("Prefabs/VFX/" + name);
        var inst = PoolUtil.GetGameObject(prefab);
        inst.transform.SetParent(parent);
        inst.SetActive(true);
        inst.transform.localPosition = localPos;
        inst.transform.localScale = Vector3.one;
        return inst;
    }

    private static Queue<GameObject> vfxGameObjects;
    private const string VFXObjName = "VFX";

    public static void PlayItemCollectFx(E_ItemType itemType, Vector3 startPos, Action onComplete, bool setIcon = false,
        bool bUiPos = false, Transform target = null)
    {
        if (vfxGameObjects == null)
        {
            vfxGameObjects = new Queue<GameObject>();
            for (int i = 0; i < 10; i++)
            {
                var go = new GameObject($"{VFXObjName}_{i}");
                go.AddComponent<RectTransform>();
                go.transform.SetParent(UIModule.Instance.UICanvas.transform);
                vfxGameObjects.Enqueue(go);
            }
        }

        itemFlyTarget.TryGetValue(itemType, out target);
        if (target == null)
        {
            onComplete?.Invoke();
            return;
        }

        var name = "VFX_Get" + itemType;
#if BIZZA_REAL_WITHDRAW
        bool isSingleMode = ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode;
        if (isSingleMode && itemType == E_ItemType.Gold)
        {
            itemType = E_ItemType.Dollar;
        }
        if (itemType == E_ItemType.Dollar || itemType == E_ItemType.WithDrawDanDollar)
        {
            name = "VFX_Get" + E_ItemType.Dollar;
            name += AccountModule.CountryType;
        }
#endif

        var inst = PlayFollow(name, UIModule.Instance.fxContainer);
        var pos = startPos;
        if (bUiPos)
        {
            (inst.transform as RectTransform).anchoredPosition = pos;
        }
        else
        {
            inst.transform.position = pos;
        }

        if (vfxGameObjects == null || vfxGameObjects.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        var _obj = vfxGameObjects.Dequeue();
        _obj.transform.position = target.position;

        var uiParticle = inst.transform.GetChild(0).GetComponent<ParticleImage>();


        if (uiParticle != null)
        {
            if (setIcon)
            {
                var icon = ItemUtils.GetItemIcon(itemType);
                uiParticle.sprite = icon;
            }

            uiParticle.attractorTarget = _obj.transform;
            uiParticle.onFirstParticleFinished.RemoveAllListeners();
            uiParticle.onFirstParticleFinished.AddListener(() =>
            {
                onComplete?.Invoke();
            });
            uiParticle.onLastParticleFinished.RemoveAllListeners();
            uiParticle.onAnyParticleFinished.RemoveAllListeners();
            uiParticle.onFirstParticleFinished.AddListener(() =>
            {
                // var panel = UiManager.Instance.GetPanel<PlayerFramePanel>(UIPanelType.PlayerFramePanel);
                // if (panel != null)
                // {
                //     panel.DoAnim(!bCoin);
                // }
                PlayGetCoinSound(itemType);
            });
            uiParticle.onLastParticleFinished.AddListener(() =>
            {
                if (!vfxGameObjects.Contains(_obj))
                {
                    vfxGameObjects.Enqueue(_obj);
                }
                PoolUtil.ReleaseGameObject(inst);
            });
        }
        else
        {
            inst.transform.DOMove(target.position, 0.6f).OnComplete(() =>
            {
                if (!vfxGameObjects.Contains(_obj))
                {
                    vfxGameObjects.Enqueue(_obj);
                }
                PoolUtil.ReleaseGameObject(inst);
                onComplete?.Invoke();
            });
        }
    }

    public static void PlayGetCoinSound(E_ItemType itemType)
    {
        var isGold = itemType == E_ItemType.Gold;
        var sound = isGold ? "SFX_GetGold" : "SFX_GetDollar";
        SoundManager.Instance.PlaySFX(sound);
    }
}
