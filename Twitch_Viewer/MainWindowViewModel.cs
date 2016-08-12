using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Twitch_Viewer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MainWindow _window;


        #region Properties
        public MainWindow Window
        {
            get { return _window; }
            set { _window = value; }
        }

        public static List<string> Quality
        {
            get { return MainWindow.Quality; }
        }

        public string SelectedQuality
        {
            get { return Window.SelectedQuality; }
            set
            {
                Window.SelectedQuality = value;
                OnPropertyChanged("SelectedQuality");
            }
        }

        #region Settings
        public string LivestreamerArgs
        {
            get { return Window.LivestreamerArgs; }
            set { Window.LivestreamerArgs = value; }
        }

        public string RefreshInterval
        {
            get { return Window.RefreshInterval.ToString(); }
            set
            {
                var tmp = int.Parse(value);
                if (tmp < 5)
                    Window.RefreshInterval = 5;
                else
                    Window.RefreshInterval = tmp;
            }
        }
        #endregion

        public double FillHeight
        {
            get { return Window.FillHeight; }
            set
            {
                Window.FillHeight = value;
                OnPropertyChanged("FillHeight");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(MainWindow window)
        {
            Window = window;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
