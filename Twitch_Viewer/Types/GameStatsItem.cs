using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Twitch_Viewer.Types
{
    [Serializable, XmlRoot("GameStats")]
    public class GameStatsItem : StatsItem
    {
        public GameStatsItem() { }

        public GameStatsItem(string name)
        {
            this.name = name;
        }

        public GameStatsItem(string name, TimeSpan viewTime, int viewCount)
        {
            this.name = name;
            this.viewTime = viewTime;
            this.viewCount = viewCount;
        }
    }
}
