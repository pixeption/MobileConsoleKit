using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class SimpleImageCell : BaseCell
	{
#pragma warning disable 0649
		[SerializeField]
		LayoutElement _headerSpace;

		[SerializeField]
		Image _img;
#pragma warning restore 0649

		public override void SetText(string name) {}

		public override void SetHeaderOffset(float offset)
		{
			_headerSpace.minWidth = offset;
		}

		public void SetSprite(Sprite sprite)
		{
			_img.sprite = sprite;
			_img.preserveAspect = true;
		}

		public float GetPreferHeight(Sprite sprite, float offset)
		{
			float maxWidth = _img.rectTransform.sizeDelta.x - offset;
			float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
			float spriteHeight = sprite.bounds.size.y * sprite.pixelsPerUnit;
			
			float preferHeight;
			if (spriteWidth > maxWidth)
			{
				preferHeight = (maxWidth / spriteWidth) * spriteHeight;
			}
			else
			{
				preferHeight = spriteHeight;
			}

			return preferHeight;
		}
	}
}