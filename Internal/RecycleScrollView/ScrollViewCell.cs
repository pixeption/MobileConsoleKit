using UnityEngine;
using UnityEngine.EventSystems;

namespace MobileConsole.UI
{
    ///////////////////////////////////////////
    // Scroll View Cell
    ///////////////////////////////////////////
    public class ScrollViewCell : MonoBehaviour, IPointerClickHandler
    {
        public delegate void EventOnCellSelected(ScrollViewCell cell);
        public event EventOnCellSelected OnCellSelected;

        public string identifier;
        
        [System.NonSerialized]
        public ScrollViewCellInfo info;

        public RectTransform rectTransform;

        void Awake()
        {
            rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        }

        public void SetTopPositionAndHeight(float position, float height)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -position);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }


        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (OnCellSelected != null)
            {
                OnCellSelected(this);
            }
        }
    }
}