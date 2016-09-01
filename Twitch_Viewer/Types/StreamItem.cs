using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Viewer.Types
{
    public class StreamItem : IComparable , INotifyPropertyChanged
    {
        private const string heartImageBlack = "/Twitch_Viewer;component/imageResources/HeartBlack.png";
        private const string heartImageRed = "/Twitch_Viewer;component/imageResources/HeartBorder.png";

        EventHook hook;

        private string name;
        public string Name { get { return name; } set { name = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string displayName;
        public string DisplayName { get { return displayName; } set { displayName = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string curGame;
        public string CurGame { get { return curGame; } set { curGame = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string preview;
        public string Preview { get { return preview; } set { preview = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
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

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public StreamItem(string name, string displayName, string curGame, string preview, string viewers)
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
            DateTime start = DateTime.Now;

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
            p.Exited += (pSender, pArgs) => addViewStats(start, stopwatch.Elapsed);
            p.Exited += (pSender, pArgs) => p.Dispose();
            p.Start();
            p.BeginOutputReadLine();

            loadingDialog.Closing += (dSender, dArgs) =>
            {
                if (p?.HasExited != true)
                    p?.Kill();
            };

            loadingDialog.Show();
        }

        public void StartOverlayStream(string livestreamerArgs, MainWindow startWindow)
        {
            if (MainWindow.settings.ArgumentsArray[0])
                livestreamerArgs += "--player-args \"--no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top {filename}\"";
            else if (MainWindow.settings.ArgumentsArray[1])
                livestreamerArgs += "--player-args=\"--no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top {filename}\"";
            else if (MainWindow.settings.ArgumentsArray[2])
                livestreamerArgs += "--player=\"\"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe\" --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[3])
                livestreamerArgs += "--player \"\"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe\" --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[4])
                livestreamerArgs += "--player \"\\\"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe\\\" --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[5])
                livestreamerArgs += "--player=\"'C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe' --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[6])
                livestreamerArgs += "--player \"'C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe' --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[7])
                livestreamerArgs += "--player=\"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";
            else if (MainWindow.settings.ArgumentsArray[8])
                livestreamerArgs += "--player \"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe --no-video-deco --no-embedded-video --qt-start-minimized --qt-notification=0 --video-on-top\"";

            //var oldCount = VlcWindowManager.GetVlcHandles().Count;

            StartStream(MainWindow.settings.PiPQuality, livestreamerArgs, startWindow);

            hook = new EventHook(OnWindowCreate, EventHook.EVENT_OBJECT_CREATE);

            //VlcWindowManager.TryMoveNewVlcWindows(oldCount);
        }

        public async void OnWindowCreate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                var title = builder.ToString();

                if (title == "VLC (VLC Video Output)")
                {
                    int width = (int)SystemParameters.PrimaryScreenWidth / 3;
                    int height = (int)(width * 0.5625);
                    int xPos = (int)SystemParameters.PrimaryScreenWidth - width;
                    int yPos = (int)SystemParameters.PrimaryScreenHeight - height;

                    await Task.Delay(100);
                    MoveWindow(hWnd, xPos, yPos, width, height + 1, true);
                    hook.Stop();
                    hook = null;
                }
            }
        }

        private void addViewStats(DateTime start, TimeSpan duration)
        {
            var truncatedTime = new TimeSpan(duration.Days, duration.Hours, duration.Minutes, duration.Seconds + (duration.Milliseconds > 500 ? 1 : 0));

            if (MainWindow._debugSettings?.DebugStatsLimit == true)
            {
                StreamStats.ViewTimeData.Add(new ViewTimeData(start, truncatedTime));
                StreamStats.ViewTime += truncatedTime;
                StreamStats.ViewCount++;
                if (GameStats != null)
                {
                    GameStats.ViewTime += truncatedTime;
                    GameStats.ViewCount++;
                }
                return;
            }

            if (duration.TotalSeconds >= 30.0)
            {
                StreamStats.ViewTimeData.Add(new ViewTimeData(start, truncatedTime));
                StreamStats.ViewTime += truncatedTime;
                StreamStats.ViewCount++;
                if (GameStats != null)
                {
                    GameStats.ViewTime += truncatedTime;
                    GameStats.ViewCount++;
                }
            }
        }

        public void registerStatsItem()
        {
            if (StreamStats != null)
                StreamStats.PropertyChanged += ItemStateChanged;

            OnPropertyChanged("HeartImage");
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
