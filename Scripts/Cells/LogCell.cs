using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class LogCell : BaseLogCell
	{
#pragma warning disable 0649
		[SerializeField]
		TextMeshProUGUI _text;

        [SerializeField]
        GameObject _collapseGroup;

        [SerializeField]
        TextMeshProUGUI _textCollapseNumber;

		[SerializeField]
		Image _imgIcon;
#pragma warning restore 0649

		public override void SetText(string text)
		{
			_text.text = text;
		}

		public override void SetCollapseEnable(bool isEnable)
		{
			_collapseGroup.SetActive(isEnable);
		}

		public override void SetCollapseNumber(int number)
		{
			_textCollapseNumber.text = number.ToString();
		}

		public override void SetHeaderOffset(float offset)
		{
			_text.margin = new Vector4(offset, 0, 0, 0);
		}

		public override void SetIcon(Sprite sprite)
		{
			if (sprite != null)
			{
				_imgIcon.sprite = sprite;
				_imgIcon.gameObject.SetActive(true);
			}
			else
			{
				_imgIcon.gameObject.SetActive(false);
			}
		}

		public override void SetIconColor(Color color)
		{
			_imgIcon.color = color;
		}
	}
}