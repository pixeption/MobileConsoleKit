namespace MobileConsole.UI
{
	public abstract class BaseDropdownCell : BaseCell
	{
		public delegate void Callback(BaseDropdownCell cell, int index);
		public Callback OnValueChanged;

		public abstract void SetOptions(string[] options);
		public abstract void SetIndex(int index);

		public void NotifyOnValueChanged(int index)
		{
			if (OnValueChanged != null)
			{
				OnValueChanged(this, index);
			}
		}
	}
}