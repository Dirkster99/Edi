namespace MRU
{
    using MRULib.MRU.ViewModels;
    using System;

    internal static class GenerateTestData
    {
        internal static MRUListViewModel CreateTestData()
        {
            var retList = new MRUListViewModel();

            var now = DateTime.Now;

            retList.UpdateEntry(new MRUEntryViewModel(@"C:tmp\t0_now.txt", false));

            // This should be shown yesterday if update and grouping work correctly
            retList.UpdateEntry(new MRUEntryViewModel(@"C:tmp\t15_yesterday.txt", now.Add(new TimeSpan(-20, 0, 0, 0)), false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:\tmp\t15_yesterday.txt", now.Add(new TimeSpan(-1, 0, 0, 0)), false));

            retList.UpdateEntry(new MRUEntryViewModel(@"f:tmp\t1_today.txt", now.Add(new TimeSpan(-1, 0,  0, 0)), true ));
            retList.UpdateEntry(new MRUEntryViewModel(@"f:tmp\Readme.txt"  , now.Add(new TimeSpan(-1, 0, 0, 0)), true));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t2_today.txt", now.Add(new TimeSpan(0, -1,  0, 0)), false ));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t3_today.txt", now.Add(new TimeSpan(0, -10, 0, 0)), false ));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t4_today.txt", now.Add(new TimeSpan(0,  0, -1, 0)), false ));

            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t5_today.txt", now.Add(new TimeSpan(0,-20,  0, 0)), false ));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t5_today.txt", now.Add(new TimeSpan(0, -20, 0, 0)), false));

            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\t6_today.txt", now.Add(new TimeSpan(0,-44,  0, 0)), false ));

            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t7_today.txt", now.Add(new TimeSpan( -30, 0, 0, 0) ),true ));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t8_today.txt", now.Add(new TimeSpan( -25, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t9_today.txt", now.Add(new TimeSpan( -20, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t10_today.txt", now.Add(new TimeSpan(-10, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t11_today.txt", now.Add(new TimeSpan( -5, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t12_today.txt", now.Add(new TimeSpan( -4, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t13_today.txt", now.Add(new TimeSpan( -3, 0, 0, 0) ),false));
            retList.UpdateEntry(new MRUEntryViewModel(@"c:tmp\directory1\directory2\directory3\t14_today.txt", now.Add(new TimeSpan( -2, 0, 0, 0) ),false));
            return retList;
        }
    }
}
