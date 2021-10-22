namespace MobileConsole.UI
{
	public class InputNodeView : NodeView
	{
		public delegate void Callback(InputNodeView node);
		public Callback callback;
		public string value;
		public bool isNumeric;
		public bool isReadonly = false;

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseInputCell cell = (BaseInputCell)scrollView.CreateCell("InputCell");
			cell.SetText(DisplayText);
			cell.SetInput(value);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			cell.SetIsNumeric(isNumeric);
			cell.SetReadonly(isReadonly);
			cell.OnValueChanged = OnValueChanged;
			return cell;
		}

		public void OnValueChanged(BaseInputCell cell, string value)
		{
			if (string.IsNullOrEmpty(value) && isNumeric)
			{
				cell.SetInput(value);
				return;
			}
			else
			{
				this.value = value;
				if (callback != null)
				{
					callback(this);
				}
			}
		}
	}
}