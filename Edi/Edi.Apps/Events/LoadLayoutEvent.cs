namespace Edi.Apps.Events
{
	using Prism.Events;

	/// <summary>
	/// Class implements a simple PRISM LoadLayout string event
	/// </summary>
	public class LoadLayoutEvent : PubSubEvent<LoadLayoutEventArgs>
	{
		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static LoadLayoutEvent()
		{
			var eventAggregator = new EventAggregator();
			Instance = eventAggregator.GetEvent<LoadLayoutEvent>();
		}

		public static LoadLayoutEvent Instance { get; }
	}
}
