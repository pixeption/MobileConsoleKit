using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public abstract class BaseCell : ScrollViewCell
	{
        [SerializeField]
        Image _imgBackground;
		
		public abstract void SetText(string text);
		public abstract void SetHeaderOffset(float offset);
        public virtual void SetBackgroundColor(Color color)
        {
            _imgBackground.color = color;
        }
	}
}