using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
    public class GenericCell : BaseGenericCell
    {
#pragma warning disable 0649
		[SerializeField]
		HorizontalLayoutGroup _layoutGroup;

        [SerializeField]
        LayoutElement _headerSpace;

        [SerializeField]
        TextMeshProUGUI _text;

		[SerializeField]
		GameObject _iconGroup;

        [SerializeField]
        Image _imgIcon;
#pragma warning restore 0649

		public override void SetText(string name)
        {
            _text.text = name;
        }

        public override void SetHeaderOffset(float offset)
        {
            _headerSpace.minWidth = offset;
        }

		public override void SetIcon(Sprite sprite)
		{
			if (sprite != null)
			{
				_imgIcon.sprite = sprite;
				_iconGroup.SetActive(true);
			}
			else
			{
				_iconGroup.SetActive(false);
			}
		}

		public override void SetIconColor(Color color)
		{
			_imgIcon.color = color;
		}

		public override float GetPreferHeight(string str, bool iconEnable, float offset)
		{
			float textWidth = _text.rectTransform.sizeDelta.x - offset;
			if (!iconEnable)
			{
				textWidth += (_imgIcon.rectTransform.sizeDelta.x + _layoutGroup.spacing);
			}
			Vector2 vec = _text.GetPreferredValues(str, textWidth, 0);
			return vec.y + _layoutGroup.padding.top + _layoutGroup.padding.bottom;
		}
	}
}