using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CaptivePortalAssistant.Models
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;

        public ExtendedObservableCollection()
        {
        }

        public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            RegisterItemPropertyChanged(Items);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                UnRegisterItemPropertyChanged(e.OldItems);
            if (e.NewItems != null)
                RegisterItemPropertyChanged(e.NewItems);
            base.OnCollectionChanged(e);
        }

        protected override void ClearItems()
        {
            UnRegisterItemPropertyChanged(Items);
            base.ClearItems();
        }

        private void RegisterItemPropertyChanged(IEnumerable items)
        {
            foreach (T item in items)
            {
                if (item != null)
                    item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void UnRegisterItemPropertyChanged(IEnumerable items)
        {
            foreach (T item in items)
            {
                if (item != null)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(sender, e);
        }
    }
}
