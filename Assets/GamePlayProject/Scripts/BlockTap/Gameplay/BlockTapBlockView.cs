using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockTap.Gameplay
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class BlockTapBlockView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform spineEffectRoot;
        private const int InvalidPointerId = int.MinValue;

        public event Action<BlockTapBlockView> Clicked;

        public RectTransform RectTransform { get; private set; }
        public int ColorIndex { get; private set; }
        public BlockTapCoordinate Coordinate { get; private set; }
        private int pressedPointerId = InvalidPointerId;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            image = image != null ? image : GetComponent<Image>();
            spineEffectRoot = spineEffectRoot != null ? spineEffectRoot : transform.Find("spine") as RectTransform;
            ResetEffectState();
        }

        public void Setup(int colorIndex, BlockTapCoordinate coordinate, Sprite sprite)
        {
            ColorIndex = colorIndex;
            Coordinate = coordinate;
            image.color = Color.white;
            image.sprite = sprite != null ? sprite : BlockTapRuntimeResources.WhiteSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            ResetPressState();
            ResetEffectState();
            gameObject.name = $"Block_{coordinate.Column}_{coordinate.Row}_{colorIndex}";
        }

        public void SetCoordinate(BlockTapCoordinate coordinate)
        {
            Coordinate = coordinate;
            gameObject.name = $"Block_{coordinate.Column}_{coordinate.Row}_{ColorIndex}";
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            pressedPointerId = eventData.pointerId;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData == null)
            {
                ResetPressState();
                return;
            }

            if (pressedPointerId == eventData.pointerId)
            {
                ResetPressState();
                Clicked?.Invoke(this);
                return;
            }

            ResetPressState();
        }

        public GameObject SpawnDetachedEliminationEffect(Transform parent)
        {
            if (spineEffectRoot == null || parent == null)
            {
                return null;
            }

            PrepareEffectTransform();
            var effectInstance = Instantiate(spineEffectRoot.gameObject, parent, false);
            var effectRect = effectInstance.GetComponent<RectTransform>();
            if (effectRect != null)
            {
                effectRect.anchorMin = new Vector2(0.5f, 0.5f);
                effectRect.anchorMax = new Vector2(0.5f, 0.5f);
                effectRect.pivot = new Vector2(0.5f, 0.5f);
                effectRect.sizeDelta = RectTransform.sizeDelta;
                effectRect.position = RectTransform.position;
                effectRect.localScale = Vector3.one;
                effectRect.localRotation = Quaternion.identity;
            }

            effectInstance.SetActive(false);
            effectInstance.SetActive(true);
            return effectInstance;
        }

        public void ResetEffectState()
        {
            if (spineEffectRoot == null)
            {
                return;
            }

            PrepareEffectTransform();
            spineEffectRoot.gameObject.SetActive(false);
        }

        private void PrepareEffectTransform()
        {
            if (spineEffectRoot == null)
            {
                return;
            }

            spineEffectRoot.anchorMin = new Vector2(0.5f, 0.5f);
            spineEffectRoot.anchorMax = new Vector2(0.5f, 0.5f);
            spineEffectRoot.pivot = new Vector2(0.5f, 0.5f);
            spineEffectRoot.anchoredPosition = Vector2.zero;
            spineEffectRoot.sizeDelta = RectTransform.sizeDelta;
            spineEffectRoot.localScale = Vector3.one;
            spineEffectRoot.localRotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            ResetPressState();
        }

        private void ResetPressState()
        {
            pressedPointerId = InvalidPointerId;
        }
    }
}
