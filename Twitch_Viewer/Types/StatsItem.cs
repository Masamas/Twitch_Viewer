using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Twitch_Viewer
{
    [Serializable, XmlRoot("Stats")]
    public abstract class StatsItem : INotifyPropertyChanged, IComparable
    {
        protected string name;
        public string Name { get { return name; } set { name = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        protected TimeSpan viewTime;
        [XmlIgnore]
        public TimeSpan ViewTime { get { return viewTime; } set { viewTime = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        public string ViewTimeString { get { return XmlConvert.ToString(viewTime); } set { viewTime = XmlConvert.ToTimeSpan(value); OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        protected int viewCount;
        public int ViewCount { get { return viewCount; } set { viewCount = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(MethodBase methodBase)
        {
            OnPropertyChanged(methodBase.Name.Substring(4));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(object obj)
        {
            StatsItem a = this;
            StatsItem b = obj as StatsItem;

            long aViewTimeTicks = a.ViewTime.Ticks;
            long bViewTimeTicks = b.ViewTime.Ticks;

            return aViewTimeTicks > bViewTimeTicks ? -1 : aViewTimeTicks < bViewTimeTicks ? 1 : 0;
        }
    }
}
