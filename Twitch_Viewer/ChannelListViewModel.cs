using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Viewer
{
    public class ChannelListViewModel
    {
        ChannelList _channelList = new ChannelList();

        public ChannelList ChannelList
        {
            get { return _channelList; }
            set
            {
                if (_channelList != value)
                    _channelList = value;
            }
        }

        public ObservableCollection<StreamItem> Channels
        {
            get { return ChannelList.Channels; }
            set
            {
                if (ChannelList.Channels != value)
                    ChannelList.Channels = value;
            }
        }

        public ObservableCollection<StreamItem> ChannelsFiltered
        {
            get { return ChannelList.ChannelsFiltered; }
            set
            {
                if (ChannelList.ChannelsFiltered != value)
                    ChannelList.ChannelsFiltered = value;
            }
        }

        public ChannelListViewModel(ChannelList channelList)
        {
            ChannelList = channelList;
        }
    }
}
