namespace Edi.Apps.Events
{
	using Prism.Events;

	/// <summary>
	/// Class implements a simple PRISM LoadLayout string event
	/// </summary>
	public class LoadLayoutEvent : PubSubEvent<LoadLayoutEventArgs>
	{
		private static readonly EventAggregator _eventAggregator;
		private static readonly LoadLayoutEvent _event;

		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static LoadLayoutEvent()
		{
			_eventAggregator = new EventAggregator();
			_event = _eventAggregator.GetEvent<LoadLayoutEvent>();
		}

		public static LoadLayoutEvent Instance
		{
			get { return _event; }
		}
	}
}
