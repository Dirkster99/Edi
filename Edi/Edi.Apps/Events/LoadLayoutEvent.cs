namespace Edi.Apps.Events
{
    /// <summary>
	/// Class implements a simple PRISM LoadLayout string event
	/// </summary>
	public class LoadLayoutEvent : PubSubEvent<LoadLayoutEventArgs>
	{
		private static readonly EventAggregator _eventAggregator;

	    /// <summary>
		/// Static Class Constructor
		/// </summary>
		static LoadLayoutEvent()
		{
			_eventAggregator = new EventAggregator();
			Instance = _eventAggregator.GetEvent<LoadLayoutEvent>();
		}

		public static LoadLayoutEvent Instance { get; }
	}
}
