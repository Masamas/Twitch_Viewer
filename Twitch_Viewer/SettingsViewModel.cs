using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch_Viewer
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private Timer _timer;
        Settings _settings = new Settings();

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings != value)
                    _settings = value;
            }
        }

        public string LivestreamerArgs
        {
            get { return Settings.LivestreamerArgs; }
            set
            {
                if (Settings.LivestreamerArgs != value)
                {
                    Settings.LivestreamerArgs = value;
                    OnPropertyChanged(MethodBase.GetCurrentMethod());
                }
            }
        }

        public int RefreshInterval
        {
            get { return Settings.RefreshInterval; }
            set
            {
                if (Settings.RefreshInterval != value)
                {
                    Settings.RefreshInterval = value;
                    OnPropertyChanged(MethodBase.GetCurrentMethod());
                }
            }
        }

        public TimeSpan TotalRunTime
        {
            get { return Settings.TotalRunTime; }
            set
            {
                if (Settings.TotalRunTime != value)
                {
                    Settings.TotalRunTime = value;
                    OnPropertyChanged(MethodBase.GetCurrentMethod());
                }
            }
        }

        public TimeSpan TotalViewTime
        {
            get { return Settings.TotalViewTime; }
        }

        public int TotalViewCount
        {
            get { return Settings.TotalViewCount; }
        }

        public ObservableCollection<StreamStatsItem> StreamStats
        {
            get { return Settings.StreamStats; }
            set
            {
                if (Settings.StreamStats != value)
                    Settings.StreamStats = value;
            }
        }

        public ObservableCollection<GameStatsItem> GameStats
        {
            get { return Settings.GameStats; }
            set
            {
                if (Settings.GameStats != value)
                    Settings.GameStats = value;
            }
        }

        #region DebugProperties
        public bool DebugStatsLimit { get; set; } = true;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;
            _timer = new Timer(new TimerCallback((s) => this.OnPropertyChanged("TotalRunTime")), null, 1000, 1000);
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
    }
}
