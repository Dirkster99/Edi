using Prism.Events;

namespace EdiApp.Events
{
	/// <summary>
	/// Class implements a PRISM tool window registration event
	/// </summary>
	public class RegisterToolWindowEvent : PubSubEvent<RegisterToolWindowEventArgs>
	{
		private static readonly EventAggregator EventAggregator;
		private static readonly RegisterToolWindowEvent Event;

		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static RegisterToolWindowEvent()
		{
			EventAggregator = new EventAggregator();
			Event = EventAggregator.GetEvent<RegisterToolWindowEvent>();
		}

		public static RegisterToolWindowEvent Instance
		{
			get { return Event; }
		}
	}
}
