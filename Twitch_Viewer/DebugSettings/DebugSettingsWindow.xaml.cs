using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for DebugSettingsWindow.xaml
    /// </summary>
    public partial class DebugSettingsWindow : Window
    {
        public ObservableCollection<DebugSettingsItem> DebugSettingsItems { get; } = new ObservableCollection<DebugSettingsItem>();

        public DebugSettingsWindow()
        {
            InitializeComponent();

            DebugSettingsItems.Add(new DebugSettingCheckBox("statsLimit", "Disable 30 second limit on stats ?", true));
            DebugSettingsItems.Add(new DebugSettingTextBox("randomStatsUsername", "User to add Random Stats to:"));
            DebugSettingsItems.Add(new DebugSettingTextBox("randomStatsCount", "Number of Random Stats to add:"));
            DebugSettingsItems.Add(new DebugSettingButton("Adds random stats to the user given above.", "Do!", addRandomStats, null));
            DebugSettingsItems.Add(new DebugSettingButton("Remove all stats from the user given above.", "Do!", removeStats, null));
            DebugSettingsItems.Add(new DebugSettingButton("Show Debug settings status.", "Show!", showStatus, null));
        }

        public void showStatus(params object[] parameters)
        {
            string message = "";

            foreach (DebugSettingsItem item in MainWindow.settings.DebugSettings.Values)
            {
                if (item is DebugSettingCheckBox)
                    message += $"CheckBoxItem {item.Description} = {(item as DebugSettingCheckBox).CheckBoxChecked}\n";

                else if (item is DebugSettingTextBox)
                    message += $"TextBoxItem {item.Description} = {(item as DebugSettingTextBox).TextBoxText}\n";
            }

            MessageBox.Show("Current status:\n" + message, "Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void addRandomStats(params object[] parameters)
        {
            string username = (MainWindow.settings.DebugSettings["randomStatsUsername"] as DebugSettingTextBox).TextBoxText;
            int count = int.Parse((MainWindow.settings.DebugSettings["randomStatsCount"] as DebugSettingTextBox).TextBoxText);

            var stats = MainWindow.settings.StreamStats.Single(item => item.Name == username);

            var r = new Random();
            var startDate = new DateTime(2016, 01, 01);
            var dateRange = (DateTime.Today - startDate).Days;

            for (int i = 0; i < count; i++)
            {
                var start = (startDate.AddDays(r.Next(dateRange))).Add(new TimeSpan(r.Next(24), r.Next(60), r.Next(60)));
                var time = new TimeSpan(0, 0, 30).Add(new TimeSpan(r.Next(8), r.Next(60), r.Next(60)));

                stats.ViewTimeData.Add(new Types.ViewTimeData(start, time));
            }
        }

        public void removeStats(params object[] parameters)
        {
            string username = (MainWindow.settings.DebugSettings["randomStatsUsername"] as DebugSettingTextBox).TextBoxText;

            var stats = MainWindow.settings.StreamStats.Single(item => item.Name == username);

            stats.ViewTimeData.Clear();
        }
    }
}
