using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Twitch_Viewer.Types
{
    public class GameItem : IComparable, INotifyPropertyChanged
    {
        private string name;
        public string Name { get { return name.Length < 20 ? name : name.Remove(20) + "..."; } set { name = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        public string FullName { get { return name; } }
        private string preview;
        public string Preview { get { return preview; } set { preview = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }
        private string viewers;
        public string Viewers { get { return viewers; } set { viewers = value; OnPropertyChanged(MethodInfo.GetCurrentMethod()); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameItem(string name, string preview, string viewers)
        {
            this.name = name;
            this.preview = preview;
            this.viewers = viewers;
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

        public int CompareTo(object obj)
        {
            GameItem a = this;
            GameItem b = obj as GameItem;

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
