namespace Util
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.IO.MemoryMappedFiles;
	using System.Threading;

	#region File and License Information
	/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-08-01 18:23:33Z</CreationDate>
</File>
*/
	#endregion

	/// <summary>
	/// This class allows restricting the number of executables in execution, to one.
	/// </summary>
	public sealed class SingletonApplicationEnforcer
	{
		#region fields
		static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Action<IEnumerable<string>> processArgsFunc;
		private readonly Action<string> processActivateFunc;
		private readonly string applicationId;

		Thread thread;
		Thread windowActivationThread;
		string argDelimiter = "_;;_";
		#endregion fields

		#region properties
		/// <summary>
		/// Gets or sets the string that is used to join 
		/// the string array of arguments in memory.
		/// </summary>
		/// <value>The arg delimeter.</value>
		public string ArgDelimeter
		{
			get
			{
				return argDelimiter;
			}
			set
			{
				argDelimiter = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SingletonApplicationEnforcer"/> class.
		/// </summary>
		/// <param name="processArgsFunc">A handler for processing command line args 
		/// when they are received from another application instance.</param>
		/// <param name="applicationId">The application id used 
		/// for naming the <seealso cref="EventWaitHandle"/>.</param>
		public SingletonApplicationEnforcer(Action<IEnumerable<string>> processArgsFunc, Action<string> processArgsFunc1,
																				string applicationId = "DisciplesRock")
		{
			if (processArgsFunc == null)
			{
				throw new ArgumentNullException("processArgsFunc");
			}

			if (processArgsFunc == null)
			{
				throw new ArgumentNullException("processArgsFunc1");
			}

			this.processArgsFunc = processArgsFunc;
			this.processActivateFunc = processArgsFunc1;
			this.applicationId = applicationId;
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Determines if this application instance is not the singleton instance.
		/// If this application is not the singleton, then it should exit.
		/// </summary>
		/// <returns><c>true</c> if the application should shutdown, 
		/// otherwise <c>false</c>.</returns>
		public bool ShouldApplicationExit()
		{
			bool createdNew;
			string argsWaitHandleName = "ArgsWaitHandle_" + applicationId;
			string memoryFileName = "ArgFile_" + applicationId;

			EventWaitHandle argsWaitHandle = new EventWaitHandle(
				false, EventResetMode.AutoReset, argsWaitHandleName, out createdNew);

			GC.KeepAlive(argsWaitHandle);


			if (createdNew)
			{
				/* This is the main, or singleton application. 
				 * A thread is created to service the MemoryMappedFile. 
				 * We repeatedly examine this file each time the argsWaitHandle 
				 * is Set by a non-singleton application instance. */
				thread = new Thread(() =>
				{
					try
					{
						using (MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(memoryFileName, 10000))
						{
							while (true)
							{
								argsWaitHandle.WaitOne();
								using (MemoryMappedViewStream stream = file.CreateViewStream())
								{
									var reader = new BinaryReader(stream);
									string args;
									try
									{
										args = reader.ReadString();
									}
									catch (Exception ex)
									{
										logger.Error("Unable to retrieve string. ", ex);
										continue;
									}
									string[] argsSplit = args.Split(new string[] { argDelimiter },
																									StringSplitOptions.RemoveEmptyEntries);
									processArgsFunc(argsSplit);
								}

							}
						}
					}
					catch (Exception ex)
					{
						logger.Error("Unable to monitor memory file. ", ex);
					}
				});

				thread.IsBackground = true;
				thread.Start();
			}
			else
			{
				try
				{
					windowActivationThread = new Thread(() =>
					{
						try
						{
							processActivateFunc(this.applicationId);
						}
						catch (Exception ex)
						{
							logger.Error("Error activating window", ex);
						}
					});

					windowActivationThread.Start();

					/* Non singleton application instance. 
					 * Should exit, after passing command line args to singleton process, 
					 * via the MemoryMappedFile. */
					using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(memoryFileName))
					{
						using (MemoryMappedViewStream stream = mmf.CreateViewStream())
						{
							var writer = new BinaryWriter(stream);
							string[] args = Environment.GetCommandLineArgs();
							string joined = string.Join(argDelimiter, args);
							writer.Write(joined);
						}
					}
					argsWaitHandle.Set();

				}
				catch (Exception ex)
				{
					logger.Error("Error on OpenExisting memory mapped file", ex);
				}
			}

			return !createdNew;
		}
		#endregion properties
	}
}