namespace MobileConsole.UI
{
	public class SliderNodeView : NodeView
	{
		public delegate void Callback(SliderNodeView node, float value);
		public Callback callback;
		public float min;
		public float max;
		public bool wholeNumbers;
		public float value;

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseSliderCell cell = (BaseSliderCell)scrollView.CreateCell("SliderCell");
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			cell.SetConfig(min, max, wholeNumbers);
			cell.SetValue(value);
			cell.OnValueChanged = OnValueChanged;
			return cell;
		}

		public void OnValueChanged(BaseSliderCell cell, float value)
		{
			this.value = value;
			if (callback != null)
			{
				callback(this, value);
			}
		}
	}
}