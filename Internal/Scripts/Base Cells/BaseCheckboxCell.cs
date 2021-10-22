namespace MobileConsole.UI
{
	public abstract class BaseCheckboxCell : BaseCell
	{
		public delegate void Callback(BaseCheckboxCell cell, bool value);
		public Callback OnValueChanged;

		public abstract void SetToggle(bool value);
		public void NotifyOnValueChanged(bool value)
		{
			if (OnValueChanged != null)
			{
				OnValueChanged(this, value);
			}
		}
	}
}