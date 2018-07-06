namespace MiniUML.Model.ViewModels.Document
{
    using MiniUML.Framework;

    public class ExportDocumentWindowViewModel : BaseViewModel
    {
        #region fields
        private double _Resolution;
        private bool _TransparentBackground;
        private bool _EnableTransparentBackground;
        #endregion fields

        #region Ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="enableTransparentBackground"></param>
        /// <param name="transparentBackground"></param>
        public ExportDocumentWindowViewModel(
            double resolution = 96,
            bool enableTransparentBackground = true,
            bool transparentBackground = true
            )
            : this()
        {
            prop_Resolution = resolution;
            prop_EnableTransparentBackground = enableTransparentBackground;
            prop_TransparentBackground = transparentBackground;
        }

        protected ExportDocumentWindowViewModel()
        {

        }
        #endregion Ctors

        #region properties
        public double prop_Resolution
        {
            get
            {
                return _Resolution;
            }

            set
            {
                if (_Resolution != value)
                {
                    _Resolution = value;
                    NotifyPropertyChanged(() => prop_Resolution);
                }
            }
        }

        public bool prop_TransparentBackground
        {
            get
            {
                return _TransparentBackground;
            }

            set
            {
                if (_TransparentBackground != value)
                {
                    _TransparentBackground = value;
                    NotifyPropertyChanged(() => prop_TransparentBackground);
                }
            }
        }

        public bool prop_EnableTransparentBackground
        {
            get
            {
                return _EnableTransparentBackground;
            }

            set
            {
                if (_EnableTransparentBackground != value)
                {
                    _EnableTransparentBackground = value;
                    NotifyPropertyChanged(() => prop_EnableTransparentBackground);
                }
            }
        }
        #endregion properties
    }
}
