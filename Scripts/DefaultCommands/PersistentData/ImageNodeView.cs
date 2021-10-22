using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	public class ImageNodeView : NodeView
	{
		public override string CellIdentifier()
		{
			return "SimpleImageCell";
		}

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			SimpleImageCell cell = (SimpleImageCell)scrollView.CreateCell(CellIdentifier());
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			cell.SetSprite((Sprite)data);

			return cell;
		}

		public override float CellSize(ScrollViewCell cell)
		{
			SimpleImageCell imageCell = (SimpleImageCell)cell;
			return imageCell.GetPreferHeight((Sprite)data, LogConsoleSettings.GetTreeViewOffsetByLevel(level));
		}
	}
}