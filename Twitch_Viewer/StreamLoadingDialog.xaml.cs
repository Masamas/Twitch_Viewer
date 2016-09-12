using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for StreamLoadingDialog.xaml
    /// </summary>
    public partial class StreamLoadingDialog : Window, INotifyPropertyChanged
    {
        public string StreamName { get; set; }

        private string statusText = "Loading stream...";
        public string StatusText { get { return statusText; } set { statusText = value; OnPropertyChanged("StatusText"); } }

        private string detailsText = "";
        public string DetailsText { get { return detailsText; } set { detailsText = value; OnPropertyChanged("DetailsText"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public StreamLoadingDialog()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public StreamLoadingDialog(Window owner, string name)
            : this()
        {
            this.Owner = owner;
            this.Loaded += (wSender, wArgs) => this.Owner = null;

            StreamName = name;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateStatusText(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StatusText = outLine.Data;
            DetailsText += outLine.Data + "\n";
        }

        private void DetailsButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var children = this.LogicalChildren;

            if (children.Current == null)
                children.MoveNext();

            if (!DetailsView.IsVisible)
            {
                ((Grid)children.Current).Height = 350;
                DetailsView.Visibility = Visibility.Visible;
                detailsButton.Source = new BitmapImage(new Uri("/imageResources/UpArrow.png", UriKind.Relative));
            }
            else
            {
                ((Grid)children.Current).Height = 175;
                DetailsView.Visibility = Visibility.Collapsed;
                detailsButton.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
            }
        }
    }
}