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
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("randomUserStatsCount", "Adds random stats to the user given above.", "Do!", addRandomUserStats, 10.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingButton("Remove all stats from the user given above.", "Do!", removeUserStats, null));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("userViewTime", "Add View Time to user given above.", "Do!", addViewTimeToUser, TimeSpan.Zero.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingButton("Generate random user time interval", "Generate!", generateRandomUserViewTime, null));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("userViewCount", "Add View Count to user given above.", "Do!", addViewCountToUser, 1.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingSeparator());
            DebugSettingsItems.Add(new DebugSettingTextBox("game", "Game for debugging options:"));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("randomGameStatsCount", "Adds random stats to the game given above.", "Do!", addRandomGameStats, 10.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingButton("Remove all stats from the game given above.", "Do!", removeGameStats, null));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("gameViewTime", "Add View Time to game given above.", "Do!", addViewTimeToGame, TimeSpan.Zero.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingButton("Generate random game time interval", "Generate!", generateRandomGameViewTime, null));
            DebugSettingsItems.Add(new DebugSettingButtonWithValue("gameViewCount", "Add View Count to game given above.", "Do!", addViewCountToGame, 1.ToString(), null));
            DebugSettingsItems.Add(new DebugSettingSeparator());
            DebugSettingsItems.Add(new DebugSettingCheckBox("statsLimit", "Disable 30 second limit on stats ?", true));
            DebugSettingsItems.Add(new DebugSettingButton("Show Debug settings status.", "Show!", showStatus, null));
        }

        public void showStatus(params object[] parameters)
        {
            string message = "".PadRight(85, '*') + "\n";
            message += "Current status:\n\n";
            message += "Description".PadRight(50) + "ItemType".PadRight(25) + "Value\n";
            
            foreach (DebugSettingsItem item in MainWindow.settings.DebugSettings.Values)
            {
                if (item is DebugSettingCheckBox)
                    message += $"{item.Description}".PadRight(50) + $"CheckBoxItem".PadRight(25) + $"{(item as DebugSettingCheckBox).CheckBoxChecked}\n";

                else if (item is DebugSettingTextBox)
                    message += $"{item.Description}".PadRight(50) + $"TextBoxItem".PadRight(25) +  $"{(item as DebugSettingTextBox).TextBoxText}\n";

                else if (item is DebugSettingButtonWithValue)
                    message += $"{item.Description}".PadRight(50) + $"ButtonWithValueItem".PadRight(25) +  $"{(item as DebugSettingButtonWithValue).Value}\n";
            }

            message += "".PadRight(85, '*');

            Trace.WriteLine(message);
        }

        public void addRandomUserStats(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;
            int count = int.Parse((DebugSettings["randomUserStatsCount"] as DebugSettingButtonWithValue).Value);

            var stats = checkStreamStatsAvailable(username);

            if (stats == null)
                return;

            var r = new Random();
            var startDate = new DateTime(2016, 01, 01);
            var dateRange = (DateTime.Today - startDate).Days;

            for (int i = 0; i < count; i++)
            {
                var start = (startDate.AddDays(r.Next(dateRange))).Add(new TimeSpan(r.Next(24), r.Next(60), r.Next(60)));
                var time = new TimeSpan(0, 0, 30).Add(new TimeSpan(r.Next(6), r.Next(60), r.Next(60)));

                stats.ViewTimeData.Add(new Types.ViewTimeData(start, time));
            }
        }

        public void addRandomGameStats(params object[] parameters)
        {
            string game = (DebugSettings["game"] as DebugSettingTextBox).TextBoxText;
            int count = int.Parse((DebugSettings["randomGameStatsCount"] as DebugSettingButtonWithValue).Value);

            var stats = checkGameStatsAvailable(game);

            if (stats == null)
                return;

            var r = new Random();
            var startDate = new DateTime(2016, 01, 01);
            var dateRange = (DateTime.Today - startDate).Days;

            for (int i = 0; i < count; i++)
            {
                var start = (startDate.AddDays(r.Next(dateRange))).Add(new TimeSpan(r.Next(24), r.Next(60), r.Next(60)));
                var time = new TimeSpan(0, 0, 30).Add(new TimeSpan(r.Next(6), r.Next(60), r.Next(60)));

                stats.ViewTimeData.Add(new Types.ViewTimeData(start, time));
            }
        }

        public void removeUserStats(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;

            var stats = MainWindow.settings.StreamStats.SingleOrDefault(item => item.Name == username);

            stats?.ViewTimeData.Clear();
        }

        public void removeGameStats(params object[] parameters)
        {
            string game = (DebugSettings["game"] as DebugSettingTextBox).TextBoxText;

            var stats = MainWindow.settings.GameStats.SingleOrDefault(item => item.Name == game);

            stats?.ViewTimeData.Clear();
        }

        public void addViewTimeToUser(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;

            var stats = checkStreamStatsAvailable(username);

            if (stats == null)
                return;

            var time = TimeSpan.Parse((DebugSettings["userViewTime"] as DebugSettingButtonWithValue).Value);

            stats.ViewTime = stats.ViewTime.Add(time);
        }

        public void addViewTimeToGame(params object[] parameters)
        {
            string game = (DebugSettings["game"] as DebugSettingTextBox).TextBoxText;

            var stats = checkGameStatsAvailable(game);

            if (stats == null)
                return;

            var time = TimeSpan.Parse((DebugSettings["gameViewTime"] as DebugSettingButtonWithValue).Value);

            stats.ViewTime = stats.ViewTime.Add(time);
        }

        public void addViewCountToUser(params object[] parameters)
        {
            string username = (DebugSettings["username"] as DebugSettingTextBox).TextBoxText;
            int value = int.Parse((DebugSettings["userViewCount"] as DebugSettingButtonWithValue).Value);

            var stats = checkStreamStatsAvailable(username);

            if (stats == null)
                return;

            stats.ViewCount += value;
        }

        public void addViewCountToGame(params object[] parameters)
        {
            string game = (DebugSettings["game"] as DebugSettingTextBox).TextBoxText;
            int value = int.Parse((DebugSettings["gameViewCount"] as DebugSettingButtonWithValue).Value);

            var stats = checkGameStatsAvailable(game);

            if (stats == null)
                return;

            stats.ViewCount += value;
        }

        public void generateRandomUserViewTime(params object[] parameters)
        {
            var r = new Random();

            var time = new TimeSpan(r.Next(6), r.Next(60), r.Next(60));

            (DebugSettings["userViewTime"] as DebugSettingButtonWithValue).Value = time.ToString();
        }

        public void generateRandomGameViewTime(params object[] parameters)
        {
            var r = new Random();

            var time = new TimeSpan(r.Next(6), r.Next(60), r.Next(60));

            (DebugSettings["gameViewTime"] as DebugSettingButtonWithValue).Value = time.ToString();
        }

        private Types.StreamStatsItem checkStreamStatsAvailable(string username)
        {
            var stats = MainWindow.settings.StreamStats.SingleOrDefault(item => item.Name == username);

            MessageBoxResult res = MessageBoxResult.None;
            if (stats == null)
                res = MessageBox.Show($"StreamStats nicht gefunden!\nWillst du neue StreamsStats mit dem usernamen: '{username}' hinzufügen?", "Stats nicht gefunden!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.No)
                return null;
            else if (res == MessageBoxResult.Yes)
            {
                stats = new Types.StreamStatsItem(username, false);
                MainWindow.settings.StreamStats.Add(stats);
            }

            return stats;
        }

        private Types.GameStatsItem checkGameStatsAvailable(string game)
        {
            var stats = MainWindow.settings.GameStats.SingleOrDefault(item => item.Name == game);

            MessageBoxResult res = MessageBoxResult.None;
            if (stats == null)
                res = MessageBox.Show($"GameStats nicht gefunden!\nWillst du neue GameStats mit dem namen: '{game}' hinzufügen?", "Stats nicht gefunden!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.No)
                return null;
            else if (res == MessageBoxResult.Yes)
            {
                stats = new Types.GameStatsItem(game);
                MainWindow.settings.GameStats.Add(stats);
            }

            return stats;
        }
    }
}
