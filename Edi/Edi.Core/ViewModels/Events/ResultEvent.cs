﻿namespace Edi.Core.ViewModels.Events
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Stores information about the result of an asynchron task processing.
	/// If an error occurs, Error is set to true and an exception may be stored in InnerException.
	/// </summary>
	public class ResultEvent : EventArgs
	{
		#region Fields
		private readonly bool mCancel;
		private readonly bool mError;
		private readonly ApplicationException mInnerException;
		private readonly string mMessage;
		private readonly Dictionary<string, object> mObjColl;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Convinience constructor to produce simple message for communicating when
		/// batch run was abborted incompletely (bCancel can be set to true or bError
		/// can be set to true).
		/// </summary>
		/// <param name="sMessage"></param>
		/// <param name="bError"></param>
		/// <param name="bCancel"></param>
		/// <param name="objColl"></param>
		/// <param name="innerException"></param>
		public ResultEvent(string sMessage,
											 bool bError,
											 bool bCancel,
											 Dictionary<string, object> objColl = null,
											 ApplicationException innerException = null)
		{
			mMessage = sMessage;
			mError = bError;
			mCancel = bCancel;
			mInnerException = innerException;
			mObjColl = (objColl == null ? null : new Dictionary<string, object>(objColl));
		}

		/// <summary>
		/// Convinience constructor to produce simple message at the end of a batch run
		/// (Cancel not clicked and no error to be reported).
		/// </summary>
		/// <param name="message"></param>
		public ResultEvent(string message)
		{
			mMessage = message;
			mError = false;
			mCancel = false;
			mInnerException = null;
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Get an error message if processing task aborted with errors
		/// </summary>
		public bool Error
		{
			get { return mError; }
		}

		/// <summary>
		/// Get property to determine whether processing was cancelled or not.
		/// </summary>
		public bool Cancel
		{
			get { return mCancel; }
		}

		/// <summary>
		/// Get property to determine whether there is an innerException to
		/// document an abortion with errors.
		/// </summary>
		public ApplicationException InnerException
		{
			get { return mInnerException; }
		}

		/// <summary>
		/// Get current message describing the current step being proceesed
		/// 
		/// </summary>
		public string Message
		{
			get { return mMessage; }
		}

		/// <summary>
		/// Dictionary of mResult objects from executing process
		/// </summary>
		public Dictionary<string, object> ResultObjects
		{
			get
			{
				return (mObjColl == null
										? new Dictionary<string, object>()
										: new Dictionary<string, object>(mObjColl));
			}
		}
		#endregion Properties
	}
}
