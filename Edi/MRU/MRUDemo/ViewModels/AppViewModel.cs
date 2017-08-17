namespace MRU.ViewModels
{
    using MRULib.MRU.Enums;
    using MRULib.MRU.Models.Persist;
    using MRULib.MRU.ViewModels;
    using MRULib.MRU.ViewModels.Base;
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class AppViewModel : MRULib.MRU.ViewModels.Base.BaseViewModel
    {
        #region fields
        private MRUListViewModel _MRUFilelist = null;

        private ICommand _NavigateUriCommand;
        private ICommand _ClearAllItemsCommand;
        private ICommand _RemoveItemsOlderThanThisCommand;

        private string _LoadTestPath;
        private string _SaveTestPath;
        private ICommand _SaveTestCommand;
        private ICommand _LoadTestCommand;
        private bool _IsProcessing;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            this.IsProcessing = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Test.xml";
            _SaveTestPath = _LoadTestPath = path;

            // Replace this with = new MRUListViewModel(); in a real world app
            _MRUFilelist = GenerateTestData.CreateTestData();
        }
        #endregion  constructor

        #region properties
        #region Save/Load properties
        /// <summary>
        /// Gets a command for loading an XML file from the displayed MRU.
        /// </summary>
        public ICommand LoadTestCommand
        {
            get
            {
                if (_LoadTestCommand == null)
                {
                    _LoadTestCommand = new RelayCommand<object>(async (p) =>
                    {
                        if (p is string == false)
                            return;

                        var param = p as string;

                        if (param == null)
                            return;

                        try
                        {
                            this.IsProcessing = true;
                            this.SetMRUList(await MRUEntrySerializer.LoadAsync(param));
                        }
                        catch (System.Exception exp)
                        {
                            MessageBox.Show(exp.StackTrace, exp.Message);
                        }
                        finally
                        {
                            this.IsProcessing = false;
                        }
                    });
                }

                return _LoadTestCommand;
            }
        }

        /// <summary>
        /// Gets a command for saving an XML file from the displayed MRU.
        /// </summary>
        public ICommand SaveTestCommand
        {
            get
            {
                if (_SaveTestCommand == null)
                {
                    _SaveTestCommand = new RelayCommand<object>(async (p) =>
                    {
                        if (p is string == false)
                            return;

                        var param = p as string;

                        if (param == null)
                            return;

                        try
                        {
                            this.IsProcessing = true;
                            var t = await MRUEntrySerializer.SaveAsync(param, this.MRUFileList);
                        }
                        catch (System.Exception exp)
                        {
                            MessageBox.Show(exp.StackTrace, exp.Message);
                        }
                        finally
                        {
                            this.IsProcessing = false;
                        }
                    });
                }

                return _SaveTestCommand;
            }
        }

        /// <summary>
        /// Gets/sets a path for loading the test XML file.
        /// </summary>
        public string LoadTestPath
        {
            get { return _LoadTestPath; }

            set
            {
                if (_LoadTestPath != value)
                {
                    _LoadTestPath = value;
                    RaisePropertyChanged(() => LoadTestPath);
                }
            }
        }

        /// <summary>
        /// Gets/sets a path for saving the test XML file.
        /// </summary>
        public string SaveTestPath
        {
            get { return _SaveTestPath; }

            set
            {
                if (_SaveTestPath != value)
                {
                    _SaveTestPath = value;
                    RaisePropertyChanged(() => SaveTestPath);
                }
            }
        }
        #endregion Save/Load properties

        /// <summary>
        /// Gets a command that will navigate to an indicated location
        /// by Opening a file reference with the application associated 
        /// in Windows.
        /// </summary>
        public ICommand NavigateUriCommand
        {
            get
            {
                if (_NavigateUriCommand == null)
                {
                    _NavigateUriCommand = new RelayCommand<object>((p) =>
                    {
                        if (p is string == false)
                            return;

                        var param = p as string;

                        if (param == null)
                            return;

                        try
                        {
                            MessageBox.Show("Use this custom command binding method to invoke custom actions, such as, file open in your application.\n\n" +
                                            "Right now this will invoke the standard Windows Association handler which will work only if the file actually exists(!).",
                                            "Invoking Custom Command in AppViewModel", MessageBoxButton.OK);

                            MRULib.MRU.Models.FileSystemCommands.OpenInWindows(param);
                        }
                        catch (System.Exception exp)
                        {
                            MessageBox.Show(exp.StackTrace, exp.Message);
                        }
                    });
                }

                return _NavigateUriCommand;
            }
        }

        /// <summary>
        /// Gets a command that removes all recent file items from the collection.
        /// </summary>
        public ICommand ClearAllItemsCommand
        {
            get
            {
                if (_ClearAllItemsCommand == null)
                {
                    _ClearAllItemsCommand = new RelayCommand<object>((p) =>
                    {
                        MRUFileList.Clear();
                    });
                }

                return _ClearAllItemsCommand;
            }
        }

        /// <summary>
        /// Gets a command that removes all items that are older
        /// than a given <see cref="GroupType"/>.
        /// </summary>
        public ICommand RemoveItemsOlderThanThisCommand
        {
            get
            {
                if (_RemoveItemsOlderThanThisCommand == null)
                {
                    _RemoveItemsOlderThanThisCommand = new RelayCommand<object>((p) =>
                    {
                        if (p is GroupType == false)
                            return;

                        var param = (GroupType)p;

                        MRUFileList.RemoveOlderThanThis(param);
                    });
                }

                return _RemoveItemsOlderThanThisCommand;
            }
        }

        /// <summary>
        /// Gets the viewmodel for the Most Recent list of files.
        /// </summary>
        public MRUListViewModel MRUFileList
        {
            get
            {
                return _MRUFilelist;
            }
        }

        /// <summary>
        /// Gets a property that determines when a progress bar should be shown or not.
        /// </summary>
        public bool IsProcessing
        {
            get { return _IsProcessing; }

            private set
            {
                if (_IsProcessing != value)
                {
                    _IsProcessing = value;
                    RaisePropertyChanged(() => IsProcessing);
                }
            }
        }
        #endregion properties

        #region methods
        private void SetMRUList(MRUListViewModel mruFilelist)
        {
            if (mruFilelist == null)
                return;

            this._MRUFilelist = mruFilelist;
            RaisePropertyChanged(() => MRUFileList);
        }
        #endregion methods
    }
}
