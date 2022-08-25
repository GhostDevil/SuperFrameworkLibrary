using System.Collections.ObjectModel;

namespace System.Collections.Generic
{
    public static class ObservableCollectionEx
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            if (values != null)
            {
                foreach (var i in values)
                {
                    collection.Add(i);
                }
            }
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        }
        public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            if (values != null)
            {
                foreach (var i in values)
                {
                    collection.Remove(i);
                }
            }
           
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        }
    }
}
