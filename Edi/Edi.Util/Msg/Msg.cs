namespace Edi.Util.Msg
{
	using Edi.Util.Local;

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
			Message = string.Empty;
			CategoryOfMsg = MsgCategory.Error;
		}

		/// <summary>
		/// Convinience constructor for constructing a message object from typical dataitems (string + optional category)
		/// </summary>
		/// <param name="strMsg"></param>
		/// <param name="type"></param>
		public Msg(string strMsg, MsgCategory type = MsgCategory.Error)
			: this()
		{
			Message = ((strMsg ?? string.Empty).Length == 0 ? Strings.STR_Category_Unknown_Internal_Error : strMsg);
			CategoryOfMsg = type;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="copyThis"></param>
		public Msg(Msg copyThis)
		{
			if (copyThis == null) return;

			CategoryOfMsg = copyThis.CategoryOfMsg;
			Message = copyThis.Message;
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
		public string Message { get; }

		/// <summary>
		/// Get <seealso cref="MsgCategory"/> property to tag this message with a category.
		/// </summary>
		public MsgCategory CategoryOfMsg { get; }

		public string MessageType
		{
			get
			{
				switch (CategoryOfMsg)
				{
					case MsgCategory.Information:
						return Strings.STR_Category_Information;

					case MsgCategory.Error:
						return Strings.STR_Category_Error;

					case MsgCategory.Warning:
						return Strings.STR_Category_Warning;

					case MsgCategory.InternalError:
						return Strings.STR_Category_InternalError;

				    default:
						return Strings.STR_Category_UnknownError;
				}
			}
		}
		#endregion property
	}
}
