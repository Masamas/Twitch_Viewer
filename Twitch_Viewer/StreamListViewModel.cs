using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Viewer
{
    public class StreamListViewModel
    {
        StreamList _streamList = new StreamList();

        public StreamList StreamList
        {
            get { return _streamList; }
            set
            {
                if (_streamList != value)
                    _streamList = value;
            }
        }

        public ObservableCollection<StreamItem> ItemsOnline
        {
            get { return StreamList.ItemsOnline; }
            set
            {
                if (StreamList.ItemsOnline != value)
                    StreamList.ItemsOnline = value;
            }
        }

        public ObservableCollection<StreamItem> ItemsOffline
        {
            get { return StreamList.ItemsOffline; }
            set
            {
                if (StreamList.ItemsOffline != value)
                    StreamList.ItemsOffline = value;
            }
        }

        public StreamListViewModel(StreamList streamList)
        {
            StreamList = streamList;
        }
    }
}
