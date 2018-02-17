namespace EdiApp.Events
{
    /// <summary>
	/// Class implements a PRISM tool window registration event
	/// </summary>
	public class RegisterToolWindowEvent : PubSubEvent<RegisterToolWindowEventArgs>
	{
		private static readonly EventAggregator _eventAggregator;

	    /// <summary>
		/// Static Class Constructor
		/// </summary>
		static RegisterToolWindowEvent()
		{
			_eventAggregator = new EventAggregator();
			Instance = _eventAggregator.GetEvent<RegisterToolWindowEvent>();
		}

		public static RegisterToolWindowEvent Instance { get; }
	}
}
