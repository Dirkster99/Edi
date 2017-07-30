namespace SimpleControls.MRU.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml.Serialization;

    using Model;
    using MsgBox;
    using SimpleControls.Command;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// This enumeration is used to control the behaviour of pinned entries.
    /// </summary>
    public enum MRUSortMethod
    {
        /// <summary>
        /// Pinned entries are sorted and displayed at the beginning of the list or just be bookmarked
        /// and stay wehere they are in the list.
        /// </summary>
        PinnedEntriesFirst = 0,

        /// <summary>
        /// Pinned entries are just be bookmarked and stay wehere they are in the list. This can be useful
        /// for a list of favourites (which stay if pinned) while other unpinned entries are changed as the
        /// user keeps opening new items and thus, changing the MRU list...
        /// </summary>
        UnsortedFavourites = 1
    }

    public class MRUListVM : Base.BaseViewModel
    {
        #region Fields
        private MRUSortMethod mPinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst;

        private ObservableCollection<MRUEntryVM> mListOfMRUEntries;

        private int mMaxMruEntryCount;

        private RelayCommand mRemoveLastEntryCommand;
        private RelayCommand mRemoveFirstEntryCommand;
        #endregion Fields

        #region Constructor
        public MRUListVM()
        {
            this.mMaxMruEntryCount = 45;
            this.mPinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst;
        }

        public MRUListVM(MRUSortMethod pinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst)
          : this()
        {
            this.mPinEntryAtHeadOfList = pinEntryAtHeadOfList;
        }
        #endregion Constructor

        #region Properties
        [XmlAttribute(AttributeName = "MinValidMRUCount")]
        public int MinValidMruEntryCount
        {
            get
            {
                return 5;
            }
        }

        [XmlAttribute(AttributeName = "MaxValidMRUCount")]
        public int MaxValidMruEntryCount
        {
            get
            {
                return 256;
            }
        }

        [XmlAttribute(AttributeName = "MaxMruEntryCount")]
        public int MaxMruEntryCount
        {
            get
            {
                return this.mMaxMruEntryCount;
            }

            set
            {
                if (this.mMaxMruEntryCount != value)
                {
                    if (value < this.MinValidMruEntryCount || value > this.MaxValidMruEntryCount)
                        throw new ArgumentOutOfRangeException("MaxMruEntryCount", value, "Valid values are: value >= 5 and value <= 256");

                    this.mMaxMruEntryCount = value;

                    this.NotifyPropertyChanged(() => this.MaxMruEntryCount);
                }
            }
        }

        /// <summary>
        /// Get/set property to determine whether a pinned entry is shown
        /// 1> at the beginning of the MRU list
        /// or
        /// 2> remains where it currently is.
        /// </summary>
        [XmlAttribute(AttributeName = "SortMethod")]
        public MRUSortMethod PinSortMode
        {
            get
            {
                return this.mPinEntryAtHeadOfList;
            }

            set
            {
                if (this.mPinEntryAtHeadOfList != value)
                {
                    this.mPinEntryAtHeadOfList = value;
                    this.NotifyPropertyChanged(() => this.PinSortMode);
                }
            }
        }

        [XmlArrayItem("MRUList", IsNullable = false)]
        public ObservableCollection<MRUEntryVM> ListOfMRUEntries
        {
            get
            {
                return this.mListOfMRUEntries;
            }

            set
            {
                if (this.mListOfMRUEntries != value)
                {
                    this.mListOfMRUEntries = value;

                    this.NotifyPropertyChanged(() => this.ListOfMRUEntries);
                    ////this.NotifyPropertyChanged(() => this.ListOfUnPinnedMRUEntries);
                    ////this.NotifyPropertyChanged(() => this.ListOfPinnedMRUEntries);
                }
            }
        }
        /***
            [XmlIgnore]
            public ObservableCollection<MRUEntryVM> ListOfPinnedMRUEntries
            {
              get
              {
                if (this.mListOfMRUEntries == null)
                  return null;

                return new ObservableCollection<MRUEntryVM>(this.mListOfMRUEntries.Where(mru => mru.IsPinned == true));
              }
            }

            [XmlIgnore]
            public ObservableCollection<MRUEntryVM> ListOfUnPinnedMRUEntries
            {
              get
              {
                if (this.mListOfMRUEntries == null)
                  return null;

                return new ObservableCollection<MRUEntryVM>(this.mListOfMRUEntries.Where(mru => mru.IsPinned == false));
              }
            }
        ***/
        #region RemoveEntryCommands
        public ICommand RemoveFirstEntryCommand
        {
            get
            {
                if (this.mRemoveFirstEntryCommand == null)
                    this.mRemoveFirstEntryCommand =
                        new RelayCommand(() => this.OnRemoveMRUEntry(Model.MRUList.Spot.First));

                return this.mRemoveFirstEntryCommand;
            }
        }

        public ICommand RemoveLastEntryCommand
        {
            get
            {
                if (this.mRemoveLastEntryCommand == null)
                    this.mRemoveLastEntryCommand = new RelayCommand(() => this.OnRemoveMRUEntry(Model.MRUList.Spot.Last));

                return this.mRemoveLastEntryCommand;
            }
        }

        #endregion RemoveEntryCommands

        private string AppName
        {
            get
            {
                return Application.ResourceAssembly.GetName().Name;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bPinOrUnPinMruEntry"></param>
        /// <param name="mruEntry"></param>
        public bool PinUnpinEntry(bool bPinOrUnPinMruEntry, MRUEntryVM mruEntry)
        {
            try
            {
                if (this.mListOfMRUEntries == null)
                    return false;

                int pinnedMruEntryCount = this.CountPinnedEntries();

                // pin an MRU entry into the next available pinned mode spot
                if (bPinOrUnPinMruEntry == true)
                {
                    MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.IsPinned == false && mru.PathFileName == mruEntry.PathFileName);

                    if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
                        this.mListOfMRUEntries.Remove(e);

                    e.IsPinned = true;

                    if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
                        this.mListOfMRUEntries.Insert(pinnedMruEntryCount, e);

                    pinnedMruEntryCount += 1;
                    //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

                    return true;
                }
                else
                {
                    // unpin an MRU entry into the next available unpinned spot
                    MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.IsPinned == true && mru.PathFileName == mruEntry.PathFileName);

                    if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
                        this.mListOfMRUEntries.Remove(e);

                    e.IsPinned = false;
                    pinnedMruEntryCount -= 1;

                    if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
                        this.mListOfMRUEntries.Insert(pinnedMruEntryCount, e);

                    //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

                    return true;
                }
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(this.AppName + " encountered an error when pinning an entry:" + Environment.NewLine
                                         + Environment.NewLine
                                         + exp.ToString(), "Error when pinning an MRU entry", MsgBoxButtons.OK, MsgBoxImage.Error);
            }

            return false;
        }

        /// <summary>
        /// Standard short-cut method to add a new unpinned entry from a string
        /// </summary>
        /// <param name="newEntry">File name and path file</param>
        public void AddMRUEntry(string newEntry)
        {
            if (newEntry == null || newEntry == string.Empty)
                return;

            this.AddMRUEntry(new MRUEntryVM() { IsPinned = false, PathFileName = newEntry });
        }

        public void AddMRUEntry(MRUEntryVM newEntry)
        {
            if (newEntry == null) return;

            try
            {
                if (this.mListOfMRUEntries == null)
                    this.mListOfMRUEntries = new ObservableCollection<MRUEntryVM>();

                // Remove all entries that point to the path we are about to insert
                MRUEntryVM e = this.mListOfMRUEntries.SingleOrDefault(item => newEntry.PathFileName == item.PathFileName);

                if (e != null)
                {
                    // Do not change an entry that has already been pinned -> its pinned in place :)
                    if (e.IsPinned == true)
                        return;

                    this.mListOfMRUEntries.Remove(e);
                }

                // Remove last entry if list has grown too long
                if (this.MaxMruEntryCount <= this.mListOfMRUEntries.Count)
                    this.mListOfMRUEntries.RemoveAt(this.mListOfMRUEntries.Count - 1);

                // Add model entry in ViewModel collection (First pinned entry or first unpinned entry)
                if (newEntry.IsPinned == true)
                    this.mListOfMRUEntries.Insert(0, new MRUEntryVM(newEntry));
                else
                {
                    this.mListOfMRUEntries.Insert(this.CountPinnedEntries(), new MRUEntryVM(newEntry));
                }
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(exp.ToString(), "An error has occurred", MsgBoxButtons.OK, MsgBoxImage.Error);
            }
            ////finally
            ////{
            ////   this.NotifyPropertyChanged(() => this.ListOfMRUEntries);
            ////}
        }

        public bool RemoveEntry(string fileName)
        {
            try
            {
                if (this.mListOfMRUEntries == null)
                    return false;

                MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.PathFileName == fileName);

                this.mListOfMRUEntries.Remove(e);

                //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

                return true;
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                                         + Environment.NewLine
                                         + exp.ToString(), "Error when pinning an MRU entry", MsgBoxButtons.OK, MsgBoxImage.Error);
            }

            return false;
        }

        public bool RemovePinEntry(MRUEntryVM mruEntry)
        {
            try
            {
                if (this.mListOfMRUEntries == null)
                    return false;

                MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.PathFileName == mruEntry.PathFileName);

                this.mListOfMRUEntries.Remove(e);

                //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

                return true;
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                                         + Environment.NewLine
                                         + exp.ToString(), "Error when pinning an MRU entry", MsgBoxButtons.OK, MsgBoxImage.Error);
            }

            return false;
        }

        public MRUEntryVM FindMRUEntry(string filePathName)
        {
            try
            {
                if (this.mListOfMRUEntries == null)
                    return null;

                return this.mListOfMRUEntries.SingleOrDefault(mru => mru.PathFileName == filePathName);
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                                         + Environment.NewLine
                                         + exp.ToString(), "Error when pinning an MRU entry", MsgBoxButtons.OK, MsgBoxImage.Error);

                return null;
            }
        }

        private void OnRemoveMRUEntry(Model.MRUList.Spot addInSpot = Model.MRUList.Spot.Last)
        {
            if (this.mListOfMRUEntries == null)
                return;

            if (this.mListOfMRUEntries.Count == 0)
                return;

            switch (addInSpot)
            {
                case MRUList.Spot.Last:
                    this.mListOfMRUEntries.RemoveAt(this.mListOfMRUEntries.Count - 1);
                    break;
                case MRUList.Spot.First:
                    this.mListOfMRUEntries.RemoveAt(0);
                    break;

                default:
                    break;
            }

            //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);
        }

        private int CountPinnedEntries()
        {
            if (this.mListOfMRUEntries != null)
                return this.mListOfMRUEntries.Count(mru => mru.IsPinned == true);

            return 0;
        }
        #endregion Methods
    }
}
