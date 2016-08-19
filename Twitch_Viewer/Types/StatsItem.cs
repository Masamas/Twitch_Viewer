using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        [XmlIgnore]
        public ObservableCollection<ViewTimeData> ViewTimeData { get { return viewTimeData; } set { viewTimeData = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); BindingOperations.EnableCollectionSynchronization(ViewTimeData, lockObjectViewTimeData); } }

        #region GraphProperties
        #region GraphHeight
        public double MondayHeight { get { return 200 * calcDayStats(DayOfWeek.Monday); } }

        public double TuesdayHeight { get { return 200 * calcDayStats(DayOfWeek.Tuesday); } }

        public double WednesdayHeight { get { return 200 * calcDayStats(DayOfWeek.Wednesday); } }

        public double ThursdayHeight { get { return 200 * calcDayStats(DayOfWeek.Thursday); } }

        public double FridayHeight { get { return 200 * calcDayStats(DayOfWeek.Friday); } }

        public double SaturdayHeight { get { return 200 * calcDayStats(DayOfWeek.Saturday); } }

        public double SundayHeight { get { return 200 * calcDayStats(DayOfWeek.Sunday); } }
        #endregion
        #region GraphValues
        public double MondayValue { get { return calcDayStats(DayOfWeek.Monday); } }

        public double TuesdayValue { get { return calcDayStats(DayOfWeek.Tuesday); } }

        public double WednesdayValue { get { return calcDayStats(DayOfWeek.Wednesday); } }

        public double ThursdayValue { get { return calcDayStats(DayOfWeek.Thursday); } }

        public double FridayValue { get { return calcDayStats(DayOfWeek.Friday); } }

        public double SaturdayValue { get { return calcDayStats(DayOfWeek.Saturday); } }

        public double SundayValue { get { return calcDayStats(DayOfWeek.Sunday); } }
        #endregion
        #endregion

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
        }

        private double calcDayStats(DayOfWeek day)
        {
            int dayStats = 0;

            foreach (ViewTimeData data in ViewTimeData)
            {
                if (data.Start.DayOfWeek == day)
                    dayStats++;
            }

            var res = ((double)dayStats / (double)ViewTimeData.Count);

            return res;
        }
    }

    public struct ViewTimeData
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }

        public ViewTimeData(DateTime start, TimeSpan duration)
        {
            this.Start = start;
            this.Duration = duration;
        }
    };
}
