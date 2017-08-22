namespace MRULib.MRU.Models.Persist
{
    using MRULib.MRU.Interfaces;
    using MRULib.MRU.ViewModels;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements methods to saves/load an entire MRU list to/from XML.
    /// </summary>
    public class MRUEntrySerializer
    {
        /// <summary>
        /// Converts an MRU ViewModel into an equivalent model.
        /// </summary>
        /// <param name="VM"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static MRUList ConvertToModel(IMRUListViewModel VM
                                            , MRUList list = null)
        {
            if (list == null)
              list = new MRUList();
            else
                list.ListOfMRUEntries.Clear();

            if (VM != null)
            {
                list.MaxMruEntryCount = VM.MaxMruEntryCount;

                foreach (var item in VM.Entries.Values)
                {
                    list.ListOfMRUEntries.Add(new MRUEntry(item.PathFileName
                                                         , item.IsPinned
                                                         , item.LastUpdate));
                }
            }

            return list;
        }

        /// <summary>
        /// Converts an MRU model into an equivalent ViewModel.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="VM"></param>
        /// <returns></returns>
        public static IMRUListViewModel ConvertToViewModel(MRUList model
                                                        , IMRUListViewModel VM = null)
        {
            if (VM == null)
                VM = new MRUListViewModel();
            else
                VM.Clear();

            if (model != null)
            {
                VM.ResetMaxMruEntryCount(model.MaxMruEntryCount);

                foreach (var item in model.ListOfMRUEntries)
                    VM.UpdateEntry(new MRUEntryViewModel(item.PathFileName
                                                         , item.LastUpdate
                                                         , item.IsPinned));
            }

            return VM;
        }

        /// <summary>
        /// Save MRU list to XML file in an awaitable fashion.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="VM"></param>
        /// <returns></returns>
        public static Task<bool> SaveAsync(string path, IMRUListViewModel VM)
        {
            return Task.Run(() =>
            {
                try
                {
                    XmlSerializerUtil.Save<MRUList>(path, ConvertToModel(VM));
                    return true;
                }
                catch
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// Save MRU list to XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="VM"></param>
        public static void Save(string path, IMRUListViewModel VM)
        {
            try
            {
                XmlSerializerUtil.Save<MRUList>(path, ConvertToModel(VM));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load MRU list from XML file in an awaitable fashion.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<IMRUListViewModel> LoadAsync(string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    var list = XmlSerializerUtil.Load<MRUList>(path);

                    return ConvertToViewModel(list);
                }
                catch
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// Load MRU list to XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IMRUListViewModel Load(string path)
        {
            try
            {
                var list = XmlSerializerUtil.Load<MRUList>(path);

                return ConvertToViewModel(list);
            }
            catch
            {
                throw;
            }
        }
    }
}
