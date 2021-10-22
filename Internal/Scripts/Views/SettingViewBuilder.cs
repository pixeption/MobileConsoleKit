using System.Collections.Generic;

namespace MobileConsole.UI
{
	public class SettingViewBuilder : ViewBuilder
	{
		const string ClassName = "SettingView";
		readonly List<Command> _commands;

		public SettingViewBuilder(List<Command> commands)
		{
			_commands = commands;
			foreach (var command in _commands)
			{
				Node currentNode = CreateCategories(command.info.categories);
				AddCommandFields(command, currentNode);
			}

			CategoryPlayerPrefs.LoadCategoryStates(_rootNode, ClassName);

			if (LogConsoleSettings.Instance.useCategoryColor)
				NodeColor.AdjustIconColor(_rootNode);

			_rootNode.children.Sort(DefaultCompareNode);
		}

		protected override void OnCategoryToggled(GenericNodeView nodeView)
		{
			base.OnCategoryToggled(nodeView);
			CategoryPlayerPrefs.SaveCategoryState(nodeView, ClassName);
		}
	}
}

