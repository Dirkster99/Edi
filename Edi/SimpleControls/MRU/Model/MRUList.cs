namespace SimpleControls.MRU.Model
{
  using System;
  using System.Collections.Generic;
  using System.Xml.Serialization;

  public class MRUList
  {
    #region constructor
    public MRUList()
    {
      this.Entries = new List<MRUEntry>();
    }

    public MRUList(MRUList copySource)
    {
      if (copySource == null) return;

      this.Entries = new List<MRUEntry>(copySource.Entries);
    }
    #endregion constructor

    internal enum Spot
    {
      First = 0,
      Last = 1
    }

    #region properties
    [XmlArray(ElementName = "Entries", Namespace = "MRUList")]
    [XmlArrayItem(ElementName = "Entry", Namespace = "MRUList")]
    public List<MRUEntry> Entries { get; set; }
    #endregion properties

    #region AddRemoveEntries
    internal bool AddEntry(MRUEntry emp,
                           MRUList.Spot addInSpot = MRUList.Spot.Last)
    {
      if (emp == null)
        return false;

      if (this.Entries == null)
        this.Entries = new List<MRUEntry>();

      switch (addInSpot)
      {
        case Spot.First:
          this.Entries.Insert(0, new MRUEntry(emp));
          return true;

        case Spot.Last: 
          this.Entries.Add(new MRUEntry(emp));
          return true;

        default:
          throw new NotImplementedException(addInSpot.ToString());
      }
    }

    internal void RemoveEntry(Spot addInSpot)
    {
      if (this.Entries == null) return;

      if (this.Entries.Count == 0) return;

      switch (addInSpot)
      {
        case Spot.First:
          this.Entries.RemoveAt(0);
          break;
        case Spot.Last:
          this.Entries.RemoveAt(this.Entries.Count - 1);
          break;
        default:
          break;
      }
    }
    #endregion AddRemoveEntries

    #region AddRemovePinnedEntries
    internal void AddPinedEntry(MRUEntry emp)
    {
      if (emp == null)
        return;

      if (this.Entries == null)
        this.Entries = new List<MRUEntry>();

      this.Entries.Add(new MRUEntry(emp));
    }
    #endregion AddRemovePinnedEntries

    internal void RemoveMruPath(string p)
    {
      if (this.Entries != null && p != null)
        this.Entries.RemoveAll(item => p == item.PathFileName);      
    }
  }
}
