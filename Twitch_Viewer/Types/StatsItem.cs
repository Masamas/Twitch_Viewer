using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;

namespace Twitch_Viewer.Types
{
    [Serializable, XmlRoot("Stats")]
    public abstract class StatsItem : INotifyPropertyChanged, IComparable
    {
        protected string name;
        protected TimeSpan viewTime;
        protected int viewCount;
        protected ObservableCollection<ViewTimeData> viewTimeData;

        private object lockObjectViewTimeData = new object();

        public string Name { get { return name; } set { name = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        [XmlIgnore]
        public TimeSpan ViewTime { get { return viewTime; } set { viewTime = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        public string ViewTimeString { get { return XmlConvert.ToString(viewTime); } set { viewTime = XmlConvert.ToTimeSpan(value); OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        public int ViewCount { get { return viewCount; } set { viewCount = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }
        public ObservableCollection<ViewTimeData> ViewTimeData { get { return viewTimeData; } set { viewTimeData = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); BindingOperations.EnableCollectionSynchronization(ViewTimeData, lockObjectViewTimeData); } }
        [XmlIgnore]
        public GraphItem Graph { get; set; }

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

        public StatsItem()
        {
            ViewTimeData = new ObservableCollection<ViewTimeData>();
            Graph = new GraphItem(this);
            ViewTimeData.CollectionChanged += Graph.NotifyGraphOnStatsAdded;
        }

        public double calcDayStats(DayOfWeek day)
        {
            int dayStats = 0;

            foreach (ViewTimeData data in ViewTimeData)
            {
                if (data.Start.DayOfWeek == day)
                    dayStats++;
            }

            var res = ((double)dayStats / (double)ViewTimeData.Count);

            if (!double.IsNaN(res))
                return res;
            else
                return 0.0;
        }
    }

    public struct ViewTimeData
    {
        private TimeSpan _duration;

        public DateTime Start { get; set; }
        [XmlIgnore]
        public TimeSpan Duration { get { return _duration; } set { _duration = value; } }
        public string DurationString { get { return XmlConvert.ToString(_duration); } set { _duration = XmlConvert.ToTimeSpan(value); } }

        public ViewTimeData(DateTime start, TimeSpan duration)
        {
            Start = start;
            _duration = duration;
        }
    };
}
