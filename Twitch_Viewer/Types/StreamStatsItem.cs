using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Twitch_Viewer.Types
{
    [Serializable, XmlRoot("StreamStats")]
    public class StreamStatsItem : StatsItem, IComparable
    {
        private bool saved = false;
        public bool Saved { get { return saved; } set { if (saved != value) { saved = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } } }

        public StreamStatsItem() { }

        public StreamStatsItem(string name, bool saved)
        {
            this.name = name;
            this.Saved = saved;
        }

        public StreamStatsItem(string name, TimeSpan viewTime, int viewCount, bool saved = false)
        {
            this.name = name;
            this.Saved = saved;
            this.viewTime = viewTime;
            this.viewCount = viewCount;
        }
    }
}
