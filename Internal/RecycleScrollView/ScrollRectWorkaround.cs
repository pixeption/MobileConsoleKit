using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileConsole.UI
{
    [RequireComponent(typeof(Scrollbar))]
    public class ScrollRectWorkaround : MonoBehaviour, IBeginDragHandler, IDragHandler
    {

        enum UpdateValue
        {
            ScrollRect,
            ScrollBar,
            None
        }

		[SerializeField]
        Scrollbar m_scrollBar;

        [SerializeField]
        ScrollRect m_scrollRect;

        Bounds m_ContentBounds;
        Bounds m_ViewBounds;

        UpdateValue thisFrameUpdate = UpdateValue.None;
        UpdateValue nextFrameUpdate = UpdateValue.None;

        // Use this for initialization
        public void Start()
        {
            m_scrollRect.onValueChanged.AddListener(UpdateScrollbar);
            m_scrollBar.onValueChanged.AddListener(UpdateScrollRect);
            StartCoroutine(InitializeScroll());
        }

        void OnEnable()
        {
            if (m_scrollRect != null && m_scrollBar != null)
                UpdateScrollbar(m_scrollRect.normalizedPosition);
        }

        IEnumerator InitializeScroll()
        {
            yield return new WaitForEndOfFrame();
            UpdateScrollbar(m_scrollRect.normalizedPosition);
            thisFrameUpdate = UpdateValue.None;
            UpdateScrollRect(m_scrollBar.value);
        }

        void LateUpdate()
        {
            thisFrameUpdate = UpdateValue.None;

            if (nextFrameUpdate != UpdateValue.None)
            {
                thisFrameUpdate = nextFrameUpdate;
                nextFrameUpdate = UpdateValue.None;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            nextFrameUpdate = UpdateValue.ScrollRect;
        }

        public void OnDrag(PointerEventData eventData)
        {
            nextFrameUpdate = UpdateValue.ScrollRect;
        }

        //ScrollBar Updates ScrollRect
        public void UpdateScrollRect(float value)
        {
            switch (thisFrameUpdate)
            {
                case UpdateValue.None:
                case UpdateValue.ScrollRect:
                    thisFrameUpdate = UpdateValue.ScrollRect;
                    break;
                default:
                    return;
            }

            if (m_scrollRect.horizontal)
            {
                m_scrollRect.normalizedPosition = new Vector2(value, m_scrollRect.normalizedPosition.y);
            }
            else if (m_scrollRect.vertical)
            {
                m_scrollRect.normalizedPosition = new Vector2(m_scrollRect.normalizedPosition.x, value);
            }
        }


        //Scroll Rect Updates ScrollBar
        public void UpdateScrollbar(Vector2 normalizedPosition)
        {
            switch (thisFrameUpdate)
            {
                case UpdateValue.None:
                case UpdateValue.ScrollBar:
                    thisFrameUpdate = UpdateValue.ScrollBar;
                    break;
                default:
                    return;
            }

            ForceUpdateScrollBar(normalizedPosition);
        }

		public void ForceUpdateScrollBar(Vector2 normalizedPosition)
		{
			//setting offset to zero temporarily.
			UpdateBounds();
			Vector2 offset = Vector2.zero;

			if (m_scrollRect.horizontal)
			{
				if (m_ContentBounds.size.x > 0)
					m_scrollBar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x);
				else
					m_scrollBar.size = 1;

				m_scrollBar.value = normalizedPosition.x;
			}

			else if (m_scrollRect.vertical)
			{
				if (m_ContentBounds.size.y > 0)
					m_scrollBar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y);
				else
					m_scrollBar.size = 1;

				m_scrollBar.value = normalizedPosition.y;
			}
		}

        readonly Vector3[] m_Corners = new Vector3[4];
        void UpdateBounds()
        {
            if (m_scrollRect.content == null)
            {
                m_ContentBounds = new Bounds();
                return;
            }

            //UpdateContentBounds
            m_scrollRect.content.GetWorldCorners(m_Corners);

            RectTransform ViewRect = null;
            ViewRect = m_scrollRect.viewport;

            if (ViewRect == null)
                ViewRect = (RectTransform)m_scrollRect.transform;

            var viewWorldToLocalMatrix = ViewRect.worldToLocalMatrix;
            m_ContentBounds = InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);

            //UpdateViewBounds
            m_ViewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
        }

        internal static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }
    }
}