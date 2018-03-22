namespace Edi.Apps.Events
{
	using System;
	using Prism.Events;

	/// <summary>
	/// Class implements a simple PRISM LoadLayout string event
	/// 
	/// Sources:
	/// http://www.codeproject.com/Tips/591221/Simple-EventAggregator-in-WPF-PRISM-4
	/// http://stackoverflow.com/questions/11254032/how-to-return-data-from-a-subscribed-method-using-eventaggregator-and-microsoft
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	public class SynchronousEvent<TPayload> : PubSubEvent<TPayload>
	{
		private static readonly EventAggregator EventAggregator;
		private static readonly SynchronousEvent<TPayload> Event;

		/// <summary>
		/// Static class constructor
		/// </summary>
		static SynchronousEvent()
		{
			SynchronousEvent<TPayload>.EventAggregator = new EventAggregator();
			SynchronousEvent<TPayload>.Event = EventAggregator.GetEvent<SynchronousEvent<TPayload>>();
		}

		/// <summary>
		/// Gets static instance of this class type
		/// </summary>
		public static SynchronousEvent<TPayload> Instance
		{
			get { return Event; }
		}

		/// <summary>
		/// Override subscription method to enforce that thread option is the publishers option.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="threadOption"></param>
		/// <param name="keepSubscriberReferenceAlive"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public override SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
		{
			// Don't allow subscribers to use any option other than the PublisherThread option.
			if (threadOption != ThreadOption.PublisherThread)
			{
				throw new InvalidOperationException();
			}

			// Perform the subscription.
			return base.Subscribe(action, threadOption, keepSubscriberReferenceAlive, filter);
		}
	}
}
