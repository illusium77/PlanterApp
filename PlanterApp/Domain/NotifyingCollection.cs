using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace PlanterApp.Domain
{
    class NotifyingCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        [NonSerialized]
        private EventHandler _refreshRequired;
        public event EventHandler RefreshRequired
        {
            add
            {
                if (_refreshRequired == null || _refreshRequired.GetInvocationList().Contains(value) == false)
                {
                    _refreshRequired += value;
                }
            }
            remove
            {
                _refreshRequired -= value;
            }
        }

        public NotifyingCollection() : base()
        {
            CollectionChangedEventManager.AddHandler(this, OnCollectionChanged);
        }

        public NotifyingCollection(IEnumerable<T> collection) : base(collection)
        {
            CollectionChangedEventManager.AddHandler(this, OnCollectionChanged);

            foreach (INotifyPropertyChanged item in collection)
            {
                PropertyChangedEventManager.AddHandler(item, OnPropertyChanged, string.Empty);
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    PropertyChangedEventManager.RemoveHandler(item, OnPropertyChanged, string.Empty);
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler(item, OnPropertyChanged, string.Empty);
                }
            }

            RaiseRefreshRequired();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseRefreshRequired();
        }

        private void RaiseRefreshRequired()
        {
            var handler = _refreshRequired;
            if (handler != null)
            {
                Action action = () =>
                {
                    handler(this, EventArgs.Empty);
                };
                Dispatcher.CurrentDispatcher.BeginInvoke(action);
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction = ListSortDirection.Ascending)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        DoSort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        DoSort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        //public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        //{
        //    DoSort(Items.OrderBy(keySelector, comparer));
        //}

        private void DoSort(IEnumerable<T> sortedItems)
        {
            var sorted = sortedItems.ToList();

            if (sorted != null && sorted.Count == this.Count())
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    var sourceIndex = IndexOf(sorted[i]);

                    if (sourceIndex != i)
                    {
                        Move(sourceIndex, i);
                    }
                }
            }
        }
    }
}
