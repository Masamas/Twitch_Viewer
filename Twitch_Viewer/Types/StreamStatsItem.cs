﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Twitch_Viewer
{
    [Serializable, XmlRoot("StreamStats")]
    public class StreamStatsItem : StatsItem, IComparable
    {
        private bool saved = false;
        public bool Saved { get { return saved; } set { saved = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }

        public StreamStatsItem() { }

        public StreamStatsItem(string name, bool saved)
        {
            this.name = name;
            this.saved = saved;
        }

        public StreamStatsItem(string name, TimeSpan viewTime, int viewCount, bool saved = false)
        {
            this.name = name;
            this.saved = saved;
            this.viewTime = viewTime;
            this.viewCount = viewCount;
        }
    }
}
