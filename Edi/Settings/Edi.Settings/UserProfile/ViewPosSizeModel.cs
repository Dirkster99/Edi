namespace Edi.Settings.UserProfile
{
    using Edi.Settings.Interfaces;
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
	public class ViewPosSizeModel : IViewPosSizeModel
    {
		#region fields
		private double _X, _Y, _Width, _Height;
		private bool _IsMaximized;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class cosntructor from coordinates of control
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		internal ViewPosSizeModel(double x,
			                      double y,
			                      double width,
			                      double height,
			                      bool isMaximized = false)
		{
			_X = x;
			_Y = y;
			_Width = width;
			_Height = height;
			_IsMaximized = isMaximized;
			DefaultConstruct = false;
		}

        /// <summary>
        /// Standard class constructor
        /// </summary>
        public ViewPosSizeModel()
        {
            _X = 0;
            _Y = 0;
            _Width = 0;
            _Height = 0;
            _IsMaximized = false;
            this.DefaultConstruct = true;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets whether this object was created through the default constructor or not
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
                return _X;
            }

            set
            {
                if (_X != value)
                {
                    _X = value;
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
                return _Y;
            }

            set
            {
                if (_Y != value)
                {
                    _Y = value;
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
                return _Width;
            }

            set
            {
                if (_Width != value)
                {
                    _Width = value;
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
                return _Height;
            }

            set
            {
                if (_Height != value)
                {
                    _Height = value;
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
				return _IsMaximized;
			}

			set
			{
				if (_IsMaximized != value)
				{
					_IsMaximized = value;
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
