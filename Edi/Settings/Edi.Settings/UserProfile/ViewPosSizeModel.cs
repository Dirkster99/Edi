namespace Edi.Settings.UserProfile
{
	using System;
	using System.Xml.Serialization;

	/// <summary>
	/// Simple wrapper class for allowing windows to persist their
	/// position, height, and width between user sessions in Properties.Default...
	/// 
	/// Todo: The storing of Positions must be extended to store collections of
	/// window positions rather than just one window (because it works only for
	/// one window otherwise ...)
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "ControlPos", IsNullable = true)]
	public class ViewPosSizeModel
	{
		#region fields
		private double mX, mY, mWidth, mHeight;
		private bool mIsMaximized;
		#endregion fields

		#region constructors
		/// <summary>
		/// Standard class constructor
		/// </summary>
		public ViewPosSizeModel()
		{
			mX = 0;
			mY = 0;
			mWidth = 0;
			mHeight = 0;
			mIsMaximized = false;
			DefaultConstruct = true;
		}

		/// <summary>
		/// Class cosntructor from coordinates of control
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public ViewPosSizeModel(double x,
														double y,
														double width,
														double height,
														bool isMaximized = false)
		{
			mX = x;
			mY = y;
			mWidth = width;
			mHeight = height;
			mIsMaximized = isMaximized;
			DefaultConstruct = false;
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Get whetehr this object was created through the default constructor or not
		/// (default data values can be easily overwritten by actual data).
		/// </summary>
		[XmlIgnore]
		public bool DefaultConstruct { get; private set; }

		/// <summary>
		/// Get/set X coordinate of control
		/// </summary>
		[XmlAttribute(AttributeName = "X")]
		public double X
		{
			get
			{
				return mX;
			}

			set
			{
				if (mX != value)
				{
					mX = value;
				}
			}
		}

		/// <summary>
		/// Get/set Y coordinate of control
		/// </summary>
		[XmlAttribute(AttributeName = "Y")]
		public double Y
		{
			get
			{
				return mY;
			}

			set
			{
				if (mY != value)
				{
					mY = value;
				}
			}
		}

		/// <summary>
		/// Get/set width of control
		/// </summary>
		[XmlAttribute(AttributeName = "Width")]
		public double Width
		{
			get
			{
				return mWidth;
			}

			set
			{
				if (mWidth != value)
				{
					mWidth = value;
				}
			}
		}

		/// <summary>
		/// Get/set height of control
		/// </summary>
		[XmlAttribute(AttributeName = "Height")]
		public double Height
		{
			get
			{
				return mHeight;
			}

			set
			{
				if (mHeight != value)
				{
					mHeight = value;
				}
			}
		}

		/// <summary>
		/// Get/set whether view is amximized or not
		/// </summary>
		[XmlAttribute(AttributeName = "IsMaximized")]
		public bool IsMaximized
		{
			get
			{
				return mIsMaximized;
			}

			set
			{
				if (mIsMaximized != value)
				{
					mIsMaximized = value;
				}
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Convinience function to set the position of a view to a valid position
		/// </summary>
		public void SetValidPos(double SystemParameters_VirtualScreenLeft,
														double SystemParameters_VirtualScreenTop)
		{
			// Restore the position with a valid position
			if (X < SystemParameters_VirtualScreenLeft)
				X = SystemParameters_VirtualScreenLeft;

			if (Y < SystemParameters_VirtualScreenTop)
				Y = SystemParameters_VirtualScreenTop;
		}
		#endregion methods
	}
}
