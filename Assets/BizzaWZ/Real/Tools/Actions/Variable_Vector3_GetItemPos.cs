using System;
using BlockTap.Gameplay;
using UnityEngine;
using UnityEngine.Scripting;

[Obfuz.ObfuzIgnore]
[Preserve]
[GraphElementInfo(Category = "CustomAction", Text = "获取元素位置", SupportTypes = new Type[] {typeof(ActionGraphBase)})]
public class Variable_Vector3_GetItemPos : Variable_Vector3
{
    [GraphVariable("行", true)]
    public FloatWrapper row;
    [GraphVariable("列", true)]
    public FloatWrapper col;

    public override object Clone()
    {
        var clone = new Variable_Vector3_GetItemPos();
        clone.row = (FloatWrapper)row?.Clone();
        clone.col = (FloatWrapper)col?.Clone();
        return clone;
    }

    public override Vector3 GetValue(in ExecuteArgs executeArgs)
    {
        var gameplayController = GameManager.Instance != null
            ? GameManager.Instance.GameplayController
            : UnityEngine.Object.FindObjectOfType<BlockTapGameplayController>();

        if (gameplayController == null)
        {
            return Vector3.zero;
        }

        if (!gameplayController.TryGetClickableBlockWorldPosition(out var pos))
        {
            return Vector3.zero;
        }

        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return pos;
        }

        // BlockTap board elements are UI. Convert their screen position back into a camera-space
        // world point so the existing tutorial pages, which re-project via Camera.main, land on
        // the same screen coordinate without large offsets.
        var screenPos = RectTransformUtility.WorldToScreenPoint(null, pos);
        var cameraDistance = Mathf.Max(mainCamera.nearClipPlane + 1f, 1f);
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cameraDistance));
    }
}
