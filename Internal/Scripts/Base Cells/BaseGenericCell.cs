using UnityEngine;

namespace MobileConsole.UI
{
	public abstract class BaseGenericCell : BaseCell
	{
		public abstract void SetIcon(Sprite sprite);
		public abstract void SetIconColor(Color color);
		public virtual float GetPreferHeight(string str, bool iconEnable, float offset) { return 0; }
	}
}