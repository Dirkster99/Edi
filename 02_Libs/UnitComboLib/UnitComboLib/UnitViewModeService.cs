namespace UnitComboLib
{
    using System.Collections.Generic;
    using UnitComboLib.ViewModels;

    public static class UnitViewModeService
    {
        public static IUnitViewModel CreateInstance(
            IList<UnitComboLib.Models.ListItem> list,
            UnitComboLib.Models.Unit.Converter unitConverter,
            int defaultIndex = 0,
            double defaultValue = 100,
            string maxStringLengthValue = "#####")
        {
            var ret = new UnitViewModel(list, unitConverter,
                                        defaultIndex, defaultValue);

            ret.MaxStringLengthValue = maxStringLengthValue;

            return ret;
        }
    }
}
