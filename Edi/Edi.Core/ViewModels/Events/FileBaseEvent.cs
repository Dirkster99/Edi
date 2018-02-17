namespace Edi.Core.ViewModels.Events
{
	using System;

	/// <summary>
	/// Identifies a document event by a typed collection of enumerators.
	/// Receivers of the <seealso cref="FileBaseEvent"/> need to switch
	/// through this enumeration to determine the type of event that was received.
	/// </summary>
	public enum FileEventType
	{
		/// <summary>
		/// Identifies an event that was probably not fully initialized.
		/// Receiving this type of event can indicate a defect in the software.
		/// </summary>
		Unknown = -1,

		/// <summary>
		/// Request: Close this particular document.
		/// </summary>
		CloseDocument = 0,

		/// <summary>
		/// Request: Adjust the current path to the path of this document.
		/// </summary>
		AdjustCurrentPath = 1
	}

	/// <summary>
	/// Class implements an event object that can be used to comunicate
	/// simple document request, such as Document close, change current path etc.
	/// </summary>
	public class FileBaseEvent : EventArgs
	{
		#region fields
		private FileEventType mTypeOfEvent;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileBaseEvent(FileEventType typeOfEvent)
		{
			mTypeOfEvent = typeOfEvent;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		protected FileBaseEvent()
			: base()
		{
			mTypeOfEvent = FileEventType.Unknown;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets the type of event <seealso cref="FileBaseEvent"/>
		/// that this object represents.
		/// </summary>
		public FileEventType TypeOfEvent
		{
			get { return mTypeOfEvent; }
		}
		#endregion properties

		#region methods
		#endregion methods
	}
}
