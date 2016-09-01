using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Twitch_Viewer.Types
{
    public class GraphItem : INotifyPropertyChanged
    {
        private StatsItem Stats { get; set; }

        #region GraphProperties
        #region GraphHeight
        public double MondayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Monday); } }

        public double TuesdayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Tuesday); } }

        public double WednesdayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Wednesday); } }

        public double ThursdayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Thursday); } }

        public double FridayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Friday); } }

        public double SaturdayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Saturday); } }

        public double SundayHeight { get { return 200 * Stats.calcDayStats(DayOfWeek.Sunday); } }
        #endregion // GraphHeight
        #region GraphValues
        public double MondayValue { get { return Stats.calcDayStats(DayOfWeek.Monday); } }

        public double TuesdayValue { get { return Stats.calcDayStats(DayOfWeek.Tuesday); } }

        public double WednesdayValue { get { return Stats.calcDayStats(DayOfWeek.Wednesday); } }

        public double ThursdayValue { get { return Stats.calcDayStats(DayOfWeek.Thursday); } }

        public double FridayValue { get { return Stats.calcDayStats(DayOfWeek.Friday); } }

        public double SaturdayValue { get { return Stats.calcDayStats(DayOfWeek.Saturday); } }

        public double SundayValue { get { return Stats.calcDayStats(DayOfWeek.Sunday); } }
        #endregion // GraphValues
        #endregion // GraphProperties

        public event PropertyChangedEventHandler PropertyChanged;

        public GraphItem(StatsItem stats)
        {
            Stats = stats;
        }

        protected void OnPropertyChanged(MethodBase methodBase)
        {
            OnPropertyChanged(methodBase.Name.Substring(4));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyGraphOnStatsAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnPropertyChanged("MondayHeight");
                OnPropertyChanged("TuesdayHeight");
                OnPropertyChanged("WednesdayHeight");
                OnPropertyChanged("ThursdayHeight");
                OnPropertyChanged("FridayHeight");
                OnPropertyChanged("SaturdayHeight");
                OnPropertyChanged("SundayHeight");
                OnPropertyChanged("MondayValue");
                OnPropertyChanged("TuesdayValue");
                OnPropertyChanged("WednesdayValue");
                OnPropertyChanged("ThursdayValue");
                OnPropertyChanged("FridayValue");
                OnPropertyChanged("SaturdayValue");
                OnPropertyChanged("SundayValue");
            }
        }
    }
}
