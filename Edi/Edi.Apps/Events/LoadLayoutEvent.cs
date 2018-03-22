namespace Edi.Apps.Events
{
	using Prism.Events;

	/// <summary>
	/// Class implements a simple PRISM LoadLayout string event
	/// </summary>
	public class LoadLayoutEvent : PubSubEvent<LoadLayoutEventArgs>
	{
		private static readonly EventAggregator EventAggregator;
		private static readonly LoadLayoutEvent Event;

		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static LoadLayoutEvent()
		{
			EventAggregator = new EventAggregator();
			Event = EventAggregator.GetEvent<LoadLayoutEvent>();
		}

		public static LoadLayoutEvent Instance
		{
			get { return Event; }
		}
	}
}
