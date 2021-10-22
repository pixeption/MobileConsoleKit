using UnityEngine;

namespace MobileConsole.UI
{
	public class BaseView : MonoBehaviour
	{
		public delegate void EventCallback(BaseView view);
		public event EventCallback OnShow;
		public event EventCallback OnClose;

		public virtual void Show()
		{
			gameObject.SetActive(true);
			if (OnShow != null)
			{
				OnShow(this);
			}
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
			if (OnClose != null)
			{
				OnClose(this);
			}
		}
	}
}