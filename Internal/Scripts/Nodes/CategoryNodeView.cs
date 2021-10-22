using UnityEngine;

namespace MobileConsole.UI
{
	public class CategoryNodeView : GenericNodeView
	{
		public string iconClose;

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			string cellId = resizable ? "GenericResizableCell" : "GenericCell";
			BaseGenericCell cell = (BaseGenericCell)scrollView.CreateCell(cellId);
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));

			AssetConfig.SpriteInfo spriteInfo = config.GetSpriteInfo((isExpanded || string.IsNullOrEmpty(iconClose)) ? icon : iconClose);
			if (spriteInfo != null)
			{
				cell.SetIcon(spriteInfo.sprite);
				cell.SetIconColor(iconColor != Color.white ? iconColor : spriteInfo.color);
			}
			else
			{
				cell.SetIcon(null);
			}

			return cell;
		}
	}
}