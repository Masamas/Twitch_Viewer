using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twitch_Viewer
{
    public class ChannelList
    {
        private ObservableCollection<StreamItem> _channels = new ObservableCollection<StreamItem>();
        private ObservableCollection<StreamItem> _channelsFiltered = new ObservableCollection<StreamItem>();

        private readonly object _lockObjectChannels = new object();
        private readonly object _lockObjectStreamsFiltered = new object();

        public ObservableCollection<StreamItem> Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                BindingOperations.EnableCollectionSynchronization(_channels, _lockObjectChannels);
            }
        }

        public ObservableCollection<StreamItem> ChannelsFiltered
        {
            get { return _channelsFiltered; }
            set
            {
                _channelsFiltered = value;
                BindingOperations.EnableCollectionSynchronization(_channelsFiltered, _lockObjectStreamsFiltered);
            }
        }
    }
}
