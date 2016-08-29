using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;
using Twitch_Viewer.Types;

namespace Twitch_Viewer
{
    #region ViewTimeDictionary
    //[XmlRoot("ViewTimes")]
    //public class ViewTimeDictionary : Dictionary<string, TimeSpan>, IXmlSerializable
    //{
    //    public XmlSchema GetSchema()
    //    {
    //        return null;
    //    }

    //    public void ReadXml(XmlReader reader)
    //    {
    //        XmlSerializer keySerializer = new XmlSerializer(typeof(string));
    //        XmlSerializer valueSerializer = new XmlSerializer(typeof(int));

    //        if (reader.IsEmptyElement)
    //            return;

    //        reader.Read();
    //        while (reader.NodeType != XmlNodeType.EndElement)
    //        {
    //            reader.ReadStartElement("Stream");
    //            string key = reader.ReadElementContentAsString();
    //            TimeSpan value = XmlConvert.ToTimeSpan(reader.ReadElementContentAsString());
    //            this.Add(key, value);
    //            reader.ReadEndElement();
    //            reader.MoveToContent();
    //        }
    //        reader.ReadEndElement();
    //    }

    //    public void WriteXml(XmlWriter writer)
    //    {
    //        XmlSerializer keySerializer = new XmlSerializer(typeof(string));
    //        XmlSerializer valueSerializer = new XmlSerializer(typeof(int));
    //        foreach (var key in Keys)
    //        {
    //            writer.WriteStartElement("Stream");
    //            writer.WriteElementString("Name", key.ToString());
    //            writer.WriteElementString("Time", XmlConvert.ToString(this[key]));
    //            writer.WriteEndElement();
    //        }
    //    }
    //}
    #endregion

    public enum SortOrder
    {
        NameAscending,
        NameDescending,
        ViewCountAscending,
        ViewCountDescending,
        ViewTimeAscending,
        ViewTimeDescending
    }

    [Serializable, XmlRoot("Settings")]
    public class Settings : INotifyPropertyChanged
    {
        private Stopwatch sw;
        private Timer _timer;

        private string livestreamerArgs = "";
        private int refreshInterval = 60;
        private double zoom = 0.4;
        private int pipXCoord;
        private int pipYCoord;
        private string pipQuality = "high";
        private TimeSpan totalRunTime;

        private ObservableCollection<StreamStatsItem> streamStats = new ObservableCollection<StreamStatsItem>();
        private ObservableCollection<GameStatsItem> gameStats = new ObservableCollection<GameStatsItem>();

        #region Properties
        public string LivestreamerArgs
        {
            get { return livestreamerArgs; }
            set { livestreamerArgs = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        public int RefreshInterval
        {
            get { return refreshInterval; }
            set
            {
                if (value < 5)
                    refreshInterval = 5;
                else
                    refreshInterval = value;

                OnPropertyChanged(MethodBase.GetCurrentMethod());
            }
        }

        [XmlIgnore]
        public List<string> Quality
        {
            get { return MainWindow.Quality; }
        }

        public double Zoom
        {
            get { return zoom; }
            set { zoom = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        public int PiPXCoord
        {
            get { return pipXCoord; }
            set { pipXCoord = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        public int PiPYCoord
        {
            get { return pipYCoord; }
            set { pipYCoord = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        public string PiPQuality
        {
            get { return pipQuality; }
            set { pipQuality = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        [XmlIgnore]
        public TimeSpan TotalRunTime
        {
            get { return totalRunTime + sw.Elapsed; }
            set { totalRunTime = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }
        public string TotalRunTimeString
        {
            get { return XmlConvert.ToString(TotalRunTime); }
            set { totalRunTime = XmlConvert.ToTimeSpan(value); OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        [XmlIgnore]
        public SortOrder SortedBy { get; set; } = SortOrder.ViewTimeDescending;

        public TimeSpan TotalViewTime
        {
            get
            {
                TimeSpan total = new TimeSpan();
                foreach (var stats in streamStats)
                    total += stats.ViewTime;

                return total;
            }
        }

        public int TotalViewCount
        {
            get
            {
                int count = 0;
                foreach (var stats in streamStats)
                    count += stats.ViewCount;

                return count;
            }
        }

        public ObservableCollection<StreamStatsItem> StreamStats
        {
            get { return streamStats; }
            set { streamStats = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }

        public ObservableCollection<GameStatsItem> GameStats
        {
            get { return gameStats; }
            set { gameStats = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public Settings()
        {
            sw = new Stopwatch();
            sw.Start();
            _timer = new Timer(new TimerCallback((s) => this.OnPropertyChanged("TotalRunTime")), null, 1000, 1000);
        }

        public void ResetStats()
        {
            foreach (var stats in streamStats)
                DeleteStats(stats);

            GameStats = new ObservableCollection<GameStatsItem>();
        }

        public void DeleteStats(StreamStatsItem stats)
        {
            if (!stats.Saved)
                StreamStats.Remove(stats);
            else
            {
                stats.ViewCount = 0;
                stats.ViewTime = new TimeSpan(0);
            }
        }

        public void DeleteStats(GameStatsItem stats)
        {
            GameStats.Remove(stats);
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

    public class SettingsHelper
    {
        XmlSerializer SerializerObj;

        private const string savePath = @"config.xml";

        public SettingsHelper()
        {
            SerializerObj = new XmlSerializer(typeof(Settings));
        }

        public bool Save(Settings settings)
        {
            PurgeStats(settings);

            try
            {
                using (TextWriter WriteFileStream = new StreamWriter(savePath, false))
                    SerializerObj.Serialize(WriteFileStream, settings);
            }
            catch (Exception e)
            { return false; }

            return true;
        }

        public Settings Load()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Settings));
            Settings loadedSettings;

            if (!File.Exists(savePath))
                Save(new Settings());

            using (FileStream ReadFileStream = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedSettings = (Settings)SerializerObj.Deserialize(ReadFileStream);
            }

            return loadedSettings;
        }

        private void PurgeStats(Settings settings)
        {
            for (int i = settings.StreamStats.Count - 1; i >= 0; i--)
                if (!settings.StreamStats[i].Saved && settings.StreamStats[i].ViewCount == 0)
                    settings.StreamStats.RemoveAt(i);

            for (int i = settings.GameStats.Count - 1; i >= 0; i--)
                if (settings.GameStats[i].ViewCount == 0)
                    settings.GameStats.RemoveAt(i);
        }
    }
}
