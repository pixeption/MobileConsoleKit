namespace MobileConsole
{
	[ExecutableCommand(name = "System/Persistent Data Inspector")]
	public class PersistentDataCommand : Command
	{
		PersistentDataViewBuilder _viewBuilder;
		public override void Execute()
		{
			// Lazy init, reduce pressure on launch
			if (_viewBuilder == null)
				_viewBuilder = new PersistentDataViewBuilder();

			info.shouldCloseAfterExecuted = false;
			LogConsole.PushSubView(_viewBuilder);
		}
	}
}
