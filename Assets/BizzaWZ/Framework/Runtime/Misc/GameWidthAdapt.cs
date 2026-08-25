using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWidthAdapt : MonoBehaviour
{
    public float minRatio;
    public float maxRatio;
    public float minFov;
    public float maxFov;

    public Vector3 minPos;
    public Vector3 maxPos;

    public Vector3 minRot;
    public Vector3 maxRot;

    private void OnEnable()
    {
        BizzaEventSystem.On(EventDefine.Game.RefreshCameraAdaptive, RefreshCameraAdaptive);
    }

    private void OnDisable()
    {
        BizzaEventSystem.Off(EventDefine.Game.RefreshCameraAdaptive, RefreshCameraAdaptive);
    }

#if !UNITY_EDITOR
    void Start()
    {
        RefreshCameraAdaptive();
    }
#else
    void Update()
    {
        RefreshCameraAdaptive();
    }
#endif

    private void RefreshCameraAdaptive()
    {
        var ratio = (float)Screen.width / Screen.height;
        var percent = Mathf.Clamp01((ratio - minRatio) / (maxRatio - minRatio));
        var fov = minFov + (maxFov - minFov) * percent;
        Camera.main.orthographicSize = fov;

        Camera.main.transform.localPosition = minPos + (maxPos - minPos) * percent;
        Camera.main.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(minRot), Quaternion.Euler(maxRot), percent);
    }
}
