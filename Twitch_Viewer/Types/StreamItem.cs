﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Twitch_Viewer.Types
{
    public class StreamItem : IComparable , INotifyPropertyChanged
    {
        private const string heartImageBlack = "/Twitch_Viewer;component/imageResources/HeartBlack.png";
        private const string heartImageRed = "/Twitch_Viewer;component/imageResources/HeartBorder.png";

        private string name;
        public string Name { get { return name; } set { name = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string displayName;
        public string DisplayName { get { return displayName; } set { displayName = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string curGame;
        public string CurGame { get { return curGame; } set { curGame = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private BitmapImage preview;
        public BitmapImage Preview { get { return preview; } set { preview = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string viewers;
        public string Viewers { get { return viewers; } set { viewers = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        public StreamStatsItem StreamStats { get { return MainWindow.settings.StreamStats.FirstOrDefault(stats => stats.Name == this.name); } }
        public GameStatsItem GameStats { get { return MainWindow.settings.GameStats.FirstOrDefault(stats => stats.Name == this.curGame); } }

        public string HeartImage
        {
            get
            {
                if (StreamStats == null)
                    return heartImageBlack;

                return StreamStats.Saved ? heartImageRed : heartImageBlack;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StreamItem(string name, string displayName, string curGame, BitmapImage preview, string viewers)
        {
            this.name = name;
            this.displayName = displayName;
            this.curGame = curGame;
            this.preview = preview;
            this.viewers = viewers;

            if (StreamStats != null)
                StreamStats.PropertyChanged += ItemStateChanged;
        }

        public void StartStream(string quality, string livestreamerArgs, MainWindow startWindow)
        {
            var link = "twitch.tv/" + Name;

            if (StreamStats == null)
                MainWindow.settings.StreamStats.Add(new StreamStatsItem(Name, false));
            if (GameStats == null && curGame != null)
                MainWindow.settings.GameStats.Add(new GameStatsItem(curGame));

            Stopwatch stopwatch = new Stopwatch();

            string args = livestreamerArgs != null && livestreamerArgs.Length != 0 ? $"{livestreamerArgs} {link} {quality}" : $"{link} {quality}";

            StreamLoadingDialog loadingDialog = new StreamLoadingDialog(startWindow, DisplayName != null ? DisplayName : Name);

            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\program files (x86)\Livestreamer\livestreamer.exe", args);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            p.StartInfo = psi;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += new DataReceivedEventHandler(loadingDialog.UpdateStatusText);
            stopwatch.Start();
            p.Exited += (pSender, pArgs) => stopwatch.Stop();
            p.Exited += (pSender, pArgs) => addViewStats(stopwatch.Elapsed);
            p.Start();
            p.BeginOutputReadLine();

            //loadingDialog.Initialized += (wSender, wArgs) => loadingDialog.Owner = null;
            loadingDialog.Closing += (dSender, dArgs) =>
            {
                if (p?.HasExited != true)
                    p.Kill();

                p.Dispose();
            };
            loadingDialog.Show();
        }

        private void addViewStats(TimeSpan time)
        {
            var truncatedTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds + (time.Milliseconds > 500 ? 1 : 0));

            if (MainWindow._debugSettings.DebugStatsLimit)
            {
                StreamStats.ViewTime += truncatedTime;
                StreamStats.ViewCount++;
                if (GameStats != null)
                {
                    GameStats.ViewTime += truncatedTime;
                    GameStats.ViewCount++;
                }
                return;
            }

            if (time.TotalSeconds >= 30.0)
            {
                StreamStats.ViewTime += truncatedTime;
                StreamStats.ViewCount++;
                if (GameStats != null)
                {
                    GameStats.ViewTime += truncatedTime;
                    GameStats.ViewCount++;
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(MethodBase methodBase)
        {
            OnPropertyChanged(methodBase.Name.Substring(4));
        }

        private void ItemStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Saved")
                OnPropertyChanged("HeartImage");
        }

        public int CompareTo(object obj)
        {
            StreamItem a = this;
            StreamItem b = obj as StreamItem;

            long aViewers;
            long bViewers;

            long.TryParse(a.Viewers, out aViewers);
            long.TryParse(b.Viewers, out bViewers);

            if (a.Viewers == "Offline" && b.Viewers == "Offline")
                return 0;

            if (a.Viewers != "Offline" && b.Viewers == "Offline")
                return -1;

            if (a.Viewers == "Offline" && b.Viewers != "Offline")
                return 1;

            return aViewers > bViewers ? -1 : aViewers < bViewers ? 1 : 0;
            //return long.Parse(a.Viewers) > long.Parse(b.Viewers) ? -1 : long.Parse(a.Viewers) < long.Parse(b.Viewers) ? 1 : 0;
        }
    }
}
