namespace UnitCombobox.ViewModels
{
    using System.Collections.ObjectModel;
    using UnitComboLib.Models;
    using UnitComboLib.Models.Unit;
    using UnitComboLib.Models.Unit.Screen;
    using UnitComboLib.ViewModels;

    public class AppViewModel : BaseViewModel
    {
        #region constructor
        public AppViewModel()
        {
            this.SizeUnitLabel = new UnitViewModel(this.GenerateScreenUnitList(), new ScreenConverter(),
                                                   1,    // Default Unit 0 Percent, 1 ScreeFontPoints
                                                   12   // Default Value
                                                   );
        }
        #endregion constructor

        #region properties
        public UnitViewModel SizeUnitLabel { get; private set; }
        #endregion properties

        #region methods
        private ObservableCollection<ListItem> GenerateScreenUnitList()
        {
            ObservableCollection<ListItem> unitList = new ObservableCollection<ListItem>();

            var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
            var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

            unitList.Add(new ListItem(Itemkey.ScreenPercent, UnitComboLib.Local.Strings.Percent_String, UnitComboLib.Local.Strings.Percent_String_Short, percentDefaults));
            unitList.Add(new ListItem(Itemkey.ScreenFontPoints, UnitComboLib.Local.Strings.Point_String, UnitComboLib.Local.Strings.Point_String_Short, pointsDefaults));

            return unitList;
        }
        #endregion methods
    }
}
