using UnityEngine;

namespace MobileConsole.UI
{
	public class GenericNodeView : NodeView
	{
		public delegate void Callback(GenericNodeView node);
		public Callback callback;
		public string icon;
		public Color iconColor = Color.white;

		public override string CellIdentifier()
		{
			return resizable ? "GenericResizableCell" : "GenericCell";
		}

		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseGenericCell cell = (BaseGenericCell)scrollView.CreateCell(CellIdentifier());
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			
			AssetConfig.SpriteInfo spriteInfo = config.GetSpriteInfo(icon);
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

		public override void CellSelected()
		{
			if (callback != null)
			{
				callback(this);
			}
		}

		public override float CellSize(ScrollViewCell cell)
		{
			BaseGenericCell genericCell = (BaseGenericCell)cell;
			float offset = LogConsoleSettings.GetTreeViewOffsetByLevel(level);
			bool hasIcon = !string.IsNullOrEmpty(icon);
			return genericCell.GetPreferHeight(DisplayText, hasIcon, offset);
		}
	}
}