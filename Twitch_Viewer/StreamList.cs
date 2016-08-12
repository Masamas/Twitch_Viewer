using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twitch_Viewer
{
    public class StreamList
    {
        private ObservableCollection<StreamItem> _itemsOnline = new ObservableCollection<StreamItem>();
        private ObservableCollection<StreamItem> _itemsOffline = new ObservableCollection<StreamItem>();

        private readonly object _lockObjectOnline = new object();
        private readonly object _lockObjectOffline = new object();

        public ObservableCollection<StreamItem> ItemsOnline
        {
            get { return _itemsOnline; }
            set
            {
                _itemsOnline = value;
                BindingOperations.EnableCollectionSynchronization(_itemsOnline, _lockObjectOnline);
            }
        }

        public ObservableCollection<StreamItem> ItemsOffline
        {
            get { return _itemsOffline; }
            set
            {
                _itemsOffline = value;
                BindingOperations.EnableCollectionSynchronization(_itemsOffline, _lockObjectOffline);
            }
        }
    }
}
