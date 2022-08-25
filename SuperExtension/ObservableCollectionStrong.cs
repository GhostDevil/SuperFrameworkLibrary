using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.Collections.Generic
{
    public class ObservableCollectionStrong<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> addItems)
        {
            if (addItems == null)
            {
                throw new ArgumentNullException(nameof(addItems));
            }

            foreach (var item in addItems)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }
        public void RemoveRange(IEnumerable<T> addItems)
        {
            if (addItems == null)
            {
                throw new ArgumentNullException(nameof(addItems));
            }

            foreach (var item in addItems)
            {
                Items.Remove(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }
    }
}
