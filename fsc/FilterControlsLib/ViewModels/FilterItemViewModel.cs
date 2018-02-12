namespace FilterControlsLib.ViewModels
{
    using FilterControlsLib.Interfaces;

    /// <summary>
    /// The Viewmodel for filter item displayed in a list of filters.
    /// </summary>
    internal class FilterItemViewModel : Base.ViewModelBase, IFilterItemViewModel
    {
        #region fields
        private readonly FileSystemModels.Models.FilterItemModel mFilterItemModel;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public FilterItemViewModel(string filter = "")
          : this()
        {
            if (string.IsNullOrEmpty(filter) == false)
                this.FilterText = filter;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public FilterItemViewModel(string name, string extensions)
          : this()
        {
            this.FilterDisplayName = name;
            this.FilterText = extensions;
        }

        /// <summary>
        /// Construct viewmodel from model
        /// </summary>
        /// <param name="filterItemModel"></param>
        public FilterItemViewModel(FileSystemModels.Models.FilterItemModel filterItemModel)
        : this()
        {
            if (filterItemModel == null)
                return;

            this.mFilterItemModel = new FileSystemModels.Models.FilterItemModel(filterItemModel);
        }

        /// <summary>
        /// Protected statndard class constructor
        /// (Consumers of this class shall use the parameterized version).
        /// </summary>
        protected FilterItemViewModel()
        {
            this.mFilterItemModel = new FileSystemModels.Models.FilterItemModel();
            //// this.FilterDisplayName = string.Empty;
            //// this.FilterText = "*";
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the regular expression based filter string eg: '*.exe'.
        /// </summary>
        public string FilterText
        {
            get
            {
                return this.mFilterItemModel.FilterText;
            }

            set
            {
                if (this.mFilterItemModel.FilterText != value)
                {
                    this.mFilterItemModel.FilterText = value;
                    this.RaisePropertyChanged(() => this.FilterText);
                }
            }
        }

        /// <summary>
        /// Gets the name for this filter
        /// (human readable for display in tool tip or label).
        /// </summary>
        public string FilterDisplayName
        {
            get
            {
                return this.mFilterItemModel.FilterDisplayName;
            }

            set
            {
                if (this.mFilterItemModel.FilterDisplayName != value)
                {
                    this.mFilterItemModel.FilterDisplayName = value;
                    this.RaisePropertyChanged(() => this.FilterDisplayName);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Standard method to display contents of this class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FilterText;
        }
        #endregion methods
    }
}
