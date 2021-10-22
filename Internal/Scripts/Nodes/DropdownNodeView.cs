namespace MobileConsole.UI
{
	public class DropdownNodeView : NodeView
	{
		public delegate void Callback(DropdownNodeView node, int index);
		public Callback callback;
		public string[] options;
		public int index;

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseDropdownCell cell = (BaseDropdownCell)scrollView.CreateCell("DropdownCell");
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			cell.SetOptions(options);
			cell.SetIndex(index);
			cell.OnValueChanged = OnValueChanged;
			return cell;
		}

		public void OnValueChanged(BaseDropdownCell cell, int index)
		{
			this.index = index;
			if (callback != null)
			{
				callback(this, index);
			}
		}
	}
}