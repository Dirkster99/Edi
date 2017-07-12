namespace Edi.Core
{
	/// <summary>
	/// This class is used to display categorized (Information, Error etc.) messages to the user.
	/// </summary>
	public class Msg
	{
		#region constructor
		/// <summary>
		/// Standard Constructor
		/// </summary>
		public Msg()
		{
			this.Message = string.Empty;
			this.CategoryOfMsg = MsgCategory.Error;
		}

		/// <summary>
		/// Convinience constructor for constructing a message object from typical dataitems (string + optional category)
		/// </summary>
		/// <param name="strMsg"></param>
		/// <param name="type"></param>
		public Msg(string strMsg, MsgCategory type = MsgCategory.Error)
			: this()
		{
			this.Message = ((strMsg == null ? string.Empty : strMsg).Length == 0 ? "<Unknown Internal Problem>" : strMsg);
			this.CategoryOfMsg = type;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="copyThis"></param>
		public Msg(Msg copyThis)
		{
			if (copyThis == null) return;

			this.CategoryOfMsg = copyThis.CategoryOfMsg;
			this.Message = copyThis.Message;
		}
		#endregion constructor

		#region enum
		/// <summary>
		/// This enumeration can be used to categorize messages.
		/// </summary>
		public enum MsgCategory
		{
			/// <summary>
			/// A message tagged with this enum represents an information (does not have any severity)
			/// </summary>
			Information = 0,

			/// <summary>
			/// A message tagged with this enum represents an error (has severity and should be observed/fixed by the user)
			/// </summary>
			Error = 1,

			/// <summary>
			/// A message tagged with this enum represents a warning (has severity and should be observed by the user)
			/// </summary>
			Warning = 2,

			/// <summary>
			/// A message tagged with this enum represents an internal error
			/// (has severity and should be observed/fixed by the user, the user should at least save his workspace
			/// and re-start the application or re-start the complete operating system)
			/// </summary>
			InternalError = 3,

			/// <summary>
			/// A message tagged with this enum has an unknown severity. Most likely this is caused by an internal
			/// error in the messaging API - but it could also be due to other unknwon problems.
			/// </summary>
			Unknown = 4
		}
		#endregion enum

		#region property
		/// <summary>
		/// Get property with a string representation that can be displayed to the user
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Get <seealso cref="MsgCategory"/> property to tag this message with a category.
		/// </summary>
		public MsgCategory CategoryOfMsg { get; private set; }

		public string MessageType
		{
			get
			{
				switch (this.CategoryOfMsg)
				{
					case MsgCategory.Information:
						return "Information";
					case MsgCategory.Error:
						return "Error";
					case MsgCategory.Warning:
						return "Warning";
					case MsgCategory.InternalError:
						return "Internal Error";
					case MsgCategory.Unknown:
					default:
						return "Unknown Error";
				}
			}
		}
		#endregion property
	}
}
