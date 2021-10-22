namespace MobileConsole.UI
{
	public abstract class NodeView : Node
	{
		public bool resizable = false;
		public string overrideDisplayText;

		protected string DisplayText
		{
			get
			{
				return string.IsNullOrEmpty(overrideDisplayText) ? name : overrideDisplayText;
			}
		}

		public abstract ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex);
		public virtual string CellIdentifier()
		{
			return string.Empty;
		}

		public virtual void CellSelected()
		{
		}

		public virtual float CellSize()
		{
			return 120;
		}

		public virtual float CellSize(ScrollViewCell cell)
		{
			return CellSize();
		}
	}
}