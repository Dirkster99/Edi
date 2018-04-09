namespace EdiApp.Events
{
    using Prism.Events;

    /// <summary>
    /// Class implements a PRISM tool window registration event
    /// </summary>
    public class RegisterToolWindowEvent : PubSubEvent<RegisterToolWindowEventArgs>
	{
		/// <summary>
		/// Static Class Constructor
		/// </summary>
		static RegisterToolWindowEvent()
		{
			var eventAggregator = new EventAggregator();
			Instance = eventAggregator.GetEvent<RegisterToolWindowEvent>();
		}

		public static RegisterToolWindowEvent Instance { get; }
	}
}
