namespace MobileConsole.UI
{
	public abstract class BaseSliderCell : BaseCell
	{
		public delegate void Callback(BaseSliderCell cell, float value);
		public Callback OnValueChanged;

		public abstract void SetValue(float value);
		public abstract void SetConfig(float min, float max, bool wholeNumbers);
		public abstract void UseRelativeSlider(bool useRelative);

		public void NotifyOnValueChanged(float value)
		{
			if (OnValueChanged != null)
			{
				OnValueChanged(this, value);
			}
		}
	}
}