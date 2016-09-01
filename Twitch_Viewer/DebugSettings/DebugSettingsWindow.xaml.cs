using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for DebugSettingsWindow.xaml
    /// </summary>
    public partial class DebugSettingsWindow : Window
    {
        public ObservableCollection<DebugSettingsItem> DebugSettingsItems { get; } = new ObservableCollection<DebugSettingsItem>();
        public Dictionary<string, DebugSettingsItem> DebugSettings { get { return MainWindow.settings.DebugSettings; } }

        public DebugSettingsWindow()
        {
            InitializeComponent();

            DebugSettingsItems.Add(new DebugSettingTextBox("username", "Username for debugging options:"));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("randomStatsCount", "Adds random stats to the user given above.", "Do!", addRandomStats, 10.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingButton("Remove all stats from the user given above.", "Do!", removeStats, null));
            DebugSettingsItems.Add(new DebugSettingButton("Generate random time interval", "Generate!", generateRandomViewTime, null));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("viewTime", "Add View Time to user given above.", "Do!", addViewTimeToUser, TimeSpan.Zero.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingSeparator());
            DebugSettingsItems.Add(new DebugSettingCheckBox("statsLimit", "Disable 30 second limit on stats ?", true));
            DebugSettingsItems.Add(new DebugSettingButton("Show Debug settings status.", "Show!", showStatus, null));
        }

        public void showStatus(params object[] parameters)
        {
            string message = "".PadRight(85, '*') + "\n";
            message += "Current status:\n\n";
            message += "ItemType".PadRight(25) + "Description".PadRight(50) + "Value\n";
            
            foreach (DebugSettingsItem item in MainWindow.settings.DebugSettings.Values)
            {
                if (item is DebugSettingCheckBox)
                    message += $"CheckBoxItem".PadRight(25) +  $"{item.Description}".PadRight(50) + $"{(item as DebugSettingCheckBox).CheckBoxChecked}\n";

                else if (item is DebugSettingTextBox)
                    message += $"TextBoxItem".PadRight(25) +  $"{item.Description}".PadRight(50) +  $"{(item as DebugSettingTextBox).TextBoxText}\n";

                else if (item is DebugSettingButtonWithValue)
                    message += $"ButtonWithValueItem".PadRight(25) +  $"{item.Description}".PadRight(50) + $"{(item as DebugSettingButtonWithValue).Value}\n";
            }

            message += "".PadRight(85, '*');

            Trace.WriteLine(message);
        }

        public void addRandomStats(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;
            int count = int.Parse((DebugSettings["randomStatsCount"] as DebugSettingButtonWithValue).Value);

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
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;

            var stats = MainWindow.settings.StreamStats.Single(item => item.Name == username);

            stats.ViewTimeData.Clear();
        }

        public void addViewTimeToUser(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;

            var stats = MainWindow.settings.StreamStats.Single(item => item.Name == username);

            var time = TimeSpan.Parse((DebugSettings["viewTime"] as DebugSettingButtonWithValue).Value);

            stats.ViewTime = stats.ViewTime.Add(time);
        }

        public void generateRandomViewTime(params object[] parameters)
        {
            var r = new Random();

            var time = new TimeSpan(r.Next(6), r.Next(60), r.Next(60));

            (DebugSettings["viewTime"] as DebugSettingButtonWithValue).Value = time.ToString();
        }
    }
}
