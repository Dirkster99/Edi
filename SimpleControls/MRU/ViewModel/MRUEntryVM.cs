namespace SimpleControls.MRU.ViewModel
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Input;
  using System.Xml.Serialization;
  using Microsoft.Win32;

  using SimpleControls.Command;

  public class MRUEntryVM : Base.BaseViewModel
  {
    #region fields
    private Model.MRUEntry mMRUEntry;
    #endregion fields

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public MRUEntryVM()
    {
      this.mMRUEntry = new Model.MRUEntry();
      this.IsPinned = false;
    }

    /// <summary>
    /// Constructor from model
    /// </summary>
    /// <param name="model"></param>
    public MRUEntryVM(Model.MRUEntry model) : this()
    {
      this.mMRUEntry = new Model.MRUEntry(model);
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource"></param>
    public MRUEntryVM(MRUEntryVM copySource)
      : this()
    {
      this.mMRUEntry = new Model.MRUEntry(copySource.mMRUEntry);
      this.IsPinned = copySource.IsPinned;
    }
    #endregion Constructor

    #region Properties
    [XmlAttribute(AttributeName = "PathFileName")]
    public string PathFileName
    {
      get
      {
        return this.mMRUEntry.PathFileName;
      }

      set
      {
        if (this.mMRUEntry.PathFileName != value)
        {
          this.mMRUEntry.PathFileName = value;
          this.NotifyPropertyChanged(() => this.PathFileName);
          this.NotifyPropertyChanged(() => this.DisplayPathFileName);
        }
      }
    }

    [XmlIgnore]
    public string DisplayPathFileName
    {
      get
      {
        if (this.mMRUEntry == null)
          return string.Empty;

        if (this.mMRUEntry.PathFileName == null)
          return string.Empty;

        int n = 32;
        return (this.mMRUEntry.PathFileName.Length > n ? this.mMRUEntry.PathFileName.Substring(0, 3) +
                                                "... " + this.mMRUEntry.PathFileName.Substring(this.mMRUEntry.PathFileName.Length - n)
                                              : this.mMRUEntry.PathFileName);
      }
    }

    [XmlAttribute(AttributeName = "IsPinned")]
    public bool IsPinned
    {
      get
      {
        return this.mMRUEntry.IsPinned;
      }

      set
      {
        if (this.mMRUEntry.IsPinned != value)
        {
          this.mMRUEntry.IsPinned = value;
          this.NotifyPropertyChanged(() => this.IsPinned);
        }
      }
    }
    #endregion Properties
  }
}
