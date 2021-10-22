using UnityEngine;

namespace MobileConsole.UI
{
	public class UIBridge : MonoBehaviour
	{
		public virtual bool toggle
		{
			get { return true; }
			set {}
		}

		public virtual string text
		{
			get { return string.Empty; }
			set {}
		}

		public virtual string input
		{
			get { return string.Empty; }
			set { }
		}
	}
}