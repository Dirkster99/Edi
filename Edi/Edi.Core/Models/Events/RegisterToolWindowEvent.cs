namespace EdiApp.Events
{
	using Prism.Events;

	/// <summary>
	/// Class implements a PRISM tool window registration event
	/// </summary>
	public class RegisterToolWindowEvent : PubSubEvent<RegisterToolWindowEventArgs>
	{
		private static readonly EventAggregator _eventAggregator;
		private static readonly RegisterToolWindowEvent _event;

		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static RegisterToolWindowEvent()
		{
			_eventAggregator = new EventAggregator();
			_event = _eventAggregator.GetEvent<RegisterToolWindowEvent>();
		}

		public static RegisterToolWindowEvent Instance
		{
			get { return _event; }
		}
	}
}
