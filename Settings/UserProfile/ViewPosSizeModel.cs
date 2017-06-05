namespace Settings.UserProfile
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
			this.mX = 0;
			this.mY = 0;
			this.mWidth = 0;
			this.mHeight = 0;
			this.mIsMaximized = false;
			this.DefaultConstruct = true;
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
			this.mX = x;
			this.mY = y;
			this.mWidth = width;
			this.mHeight = height;
			this.mIsMaximized = isMaximized;
			this.DefaultConstruct = false;
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
				return this.mX;
			}

			set
			{
				if (this.mX != value)
				{
					this.mX = value;
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
				return this.mY;
			}

			set
			{
				if (this.mY != value)
				{
					this.mY = value;
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
				return this.mWidth;
			}

			set
			{
				if (this.mWidth != value)
				{
					this.mWidth = value;
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
				return this.mHeight;
			}

			set
			{
				if (this.mHeight != value)
				{
					this.mHeight = value;
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
				return this.mIsMaximized;
			}

			set
			{
				if (this.mIsMaximized != value)
				{
					this.mIsMaximized = value;
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
			if (this.X < SystemParameters_VirtualScreenLeft)
				this.X = SystemParameters_VirtualScreenLeft;

			if (this.Y < SystemParameters_VirtualScreenTop)
				this.Y = SystemParameters_VirtualScreenTop;
		}
		#endregion methods
	}
}
