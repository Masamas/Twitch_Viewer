using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Twitch_Viewer.Types;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for StatsTest.xaml
    /// </summary>
    public partial class StatsTest : Window
    {
        public StatsTest(StreamStatsItem stats)
        {
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 05, 12, 32, 55), new TimeSpan(0, 5, 20)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 05, 12, 55, 12), new TimeSpan(0, 10, 21)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 05, 16, 22, 37), new TimeSpan(0, 45, 40)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 06, 12, 32, 55), new TimeSpan(0, 5, 20)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 07, 12, 32, 55), new TimeSpan(0, 5, 20)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 08, 12, 32, 55), new TimeSpan(0, 5, 20)));
            stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 08, 09, 12, 32, 55), new TimeSpan(0, 5, 20)));

            InitializeComponent();

            this.DataContext = stats;
        }
    }
}
