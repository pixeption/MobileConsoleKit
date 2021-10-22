using MobileConsole.UI;

namespace MobileConsole
{
	public abstract class BaseLogCell : BaseGenericCell
	{
		public abstract void SetCollapseNumber(int number);
		public abstract void SetCollapseEnable(bool isEnable);
	}
}