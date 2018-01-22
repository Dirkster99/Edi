namespace FileSystemModels.Models
{
  using System;
  using System.Xml.Serialization;

  /// <summary>
  /// The Viewmodel for filter item displayed in list of filters
  /// </summary>
  [Serializable]
  public class FilterItemModel : IComparable
  {
    #region fields
    private string mFilterText;
    private string mFilterDisplayName;
    #endregion fields

    #region constructor
    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="item"></param>
    public FilterItemModel(FilterItemModel item)
      : this()
    {
      if (item == null)
        return;

      this.mFilterDisplayName = item.FilterDisplayName;
      this.mFilterText = item.FilterText;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FilterItemModel(string filter = "")
      : this()
    {
      if (string.IsNullOrEmpty(filter) == false)
      {
        this.mFilterText = filter;
      }
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FilterItemModel(string name, string extensions)
      : this()
    {
      this.FilterDisplayName = name;
      this.FilterText = extensions;
    }

    /// <summary>
    /// Protected statndard class constructor
    /// (Consumers of this class shall use the parameterized version).
    /// </summary>
    protected FilterItemModel()
    {
      this.FilterDisplayName = string.Empty;
      this.FilterText = "*";
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the regular expression based filter string eg: '*.exe'.
    /// </summary>
    [XmlAttribute(AttributeName = "FilterText")]
    public string FilterText
    {
      get
      {
        return this.mFilterText;
      }

      set
      {
        if (this.mFilterText != value)
          this.mFilterText = value;
      }
    }

    /// <summary>
    /// Gets the name for this filter
    /// (human readable for display in tool tip or label).
    /// </summary>
    [XmlAttribute(AttributeName = "FilterDisplayName")]
    public string FilterDisplayName
    {
      get
      {
        return this.mFilterDisplayName;
      }

      set
      {
        if (this.mFilterDisplayName != value)
          this.mFilterDisplayName = value;
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
      return string.Format("{0} ({1})", this.FilterDisplayName, this.FilterText);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return this.FilterDisplayName.GetHashCode() + this.FilterText.GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      var input = obj as FilterItemModel;

      if (input == null)
        return false;

      if (string.Compare(this.FilterText, input.FilterText) == 0)
      {
        if (string.Compare(this.FilterDisplayName, input.FilterDisplayName) == 0)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Implement <seealso cref="IComparable"/> interface method.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
      if (this.Equals(obj) == true)
        return 0;

      return 1;
    }

    #endregion methods
  }
}
