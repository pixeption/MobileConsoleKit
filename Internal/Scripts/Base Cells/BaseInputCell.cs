namespace MobileConsole.UI
{
	public abstract class BaseInputCell : BaseCell
	{
		public delegate void Callback(BaseInputCell cell, string value);
		public Callback OnValueChanged;

		public abstract void SetIsNumeric(bool isNumeric);
		public abstract void SetReadonly(bool isReadonly);
		public abstract void SetInput(string text);

		public void NotifyOnValueChanged(string value)
		{
			if (OnValueChanged != null)
			{
				OnValueChanged(this, value);
			}
		}
	}
}