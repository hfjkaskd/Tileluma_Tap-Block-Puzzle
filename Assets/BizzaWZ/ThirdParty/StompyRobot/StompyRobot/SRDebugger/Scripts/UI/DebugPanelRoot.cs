using UnityEngine.UI;

namespace SRDebugger.UI
{
    using Scripts;
    using Services;
    using SRF;
    using SRF.Service;
    using UnityEngine;

    public class DebugPanelRoot : SRMonoBehaviourEx
    {
        [RequiredField] public Canvas Canvas;

        [RequiredField] public CanvasGroup CanvasGroup;

        [RequiredField] public DebuggerTabController TabController;

        protected override void OnEnable()
        {
            base.OnEnable();
            var resolution = new Vector2(1080, 1920) / 2f;
            var widthOrHeight = 1;
            if (Screen.width > Screen.height)
            {
                var tmp = resolution.x;
                resolution.x = resolution.y;
                resolution.y = tmp;
                widthOrHeight = 0;
            }

            if (transform.Find("SR_Canvas") != null)
            {
                var scaler = transform.Find("SR_Canvas").GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.referenceResolution = resolution;
                    scaler.matchWidthOrHeight = widthOrHeight;
                }
            }
        }

        public void Close()
        {
            if (Settings.Instance.UnloadOnClose)
            {
                SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
            }
            else
            {
                SRServiceManager.GetService<IDebugService>().HideDebugPanel();
            }
        }

        public void CloseAndDestroy()
        {
            SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
        }
    }
}
