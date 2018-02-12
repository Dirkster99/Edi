namespace FolderBrowser.ViewModels
{
    using FolderBrowser.Interfaces;
    using FsCore.Collections;
    using System.Collections.Generic;

    internal class SortableObservableDictionaryCollection : SortableObservableCollection<ITreeItemViewModel>
    {
        Dictionary<string, ITreeItemViewModel> _dictionary = null;

        public SortableObservableDictionaryCollection()
        {
            _dictionary = new Dictionary<string, ITreeItemViewModel>();
        }

        public bool AddItem(ITreeItemViewModel item)
        {
            if (string.IsNullOrEmpty(item.ItemName) == true)
                _dictionary.Add(string.Empty, item);
            else
                _dictionary.Add(item.ItemName.ToLower(), item);

            this.Add(item);

            return true;
        }

        public bool RemoveItem(ITreeItemViewModel item)
        {
            _dictionary.Remove(item.ItemName.ToLower());
            this.Remove(item);

            return true;
        }

        public ITreeItemViewModel TryGet(string key)
        {
            ITreeItemViewModel o;

            if (_dictionary.TryGetValue(key.ToLower(), out o))
                return o;

            return null;
        }

        public void RenameItem(ITreeItemViewModel item, string newName)
        {
            _dictionary.Remove(item.ItemName.ToLower());
            item.Rename(newName);
            _dictionary.Add(newName.ToLower(), item);
        }

        public new void Clear()
        {
            _dictionary.Clear();
            base.Clear();
        }
    }
}
