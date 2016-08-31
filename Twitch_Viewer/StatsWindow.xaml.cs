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
    public partial class StatsWindow : Window
    {
        public StatsWindow(StreamStatsItem stats)
        {
            //Random rnd = new Random();

            //for (int i = 0; i < 50; i++)
            //    stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 8, rnd.Next(1, 32), rnd.Next(24), rnd.Next(60), rnd.Next(60)), new TimeSpan(rnd.Next(1), rnd.Next(1, 60), rnd.Next(60))));

            stats.ViewTimeData.Sort(item => item.Start);

            InitializeComponent();

            this.DataContext = stats;
        }
    }
}
