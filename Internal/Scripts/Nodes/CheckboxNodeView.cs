namespace MobileConsole.UI
{
	public class CheckboxNodeView : NodeView
	{
		public delegate void Callback(CheckboxNodeView node);
		public Callback callback;
		public bool isOn;

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseCheckboxCell cell = (BaseCheckboxCell)scrollView.CreateCell("CheckboxCell");
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			cell.SetToggle(isOn);
			cell.OnValueChanged = OnValueChanged;
			return cell;
		}

		public void OnValueChanged(BaseCheckboxCell cell, bool value)
		{
			isOn = value;
			if (callback != null)
			{
				callback(this);
			}
		}
	}
}