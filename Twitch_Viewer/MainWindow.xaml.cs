using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TwixelAPI;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainWindowViewModel _viewModel;
        private SettingsViewModel _settingsViewModel;
        private StreamListViewModel _streamListViewModel;
        private DirectoryViewModel _directoryViewModel;
        private ChannelListViewModel _channelListViewModel;

        private Twixel twixel;
        public static Settings settings;

        public static string username;
        public static readonly string workingDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private int gameListOffset = 0;
        private int gameStreamsOffset = 0;
        private int channelsOffset = 0;

        #region Properties
        private static List<string> quality;
        public static List<string> Quality
        {
            get
            {
                return quality;
            }
        }

        private static string selectedQuality;
        public string SelectedQuality
        {
            get { return selectedQuality; }
            set
            {
                selectedQuality = value;
                OnPropertyChanged("SelectedQuality");
            }
        }

        #region Settings
        private string livestreamerArgs = "";
        public string LivestreamerArgs
        {
            get { return livestreamerArgs; }
            set
            {
                livestreamerArgs = value;
            }
        }

        private int refreshInterval = 60;
        public int RefreshInterval
        {
            get { return refreshInterval; }
            set
            {
                refreshInterval = value;
            }
        }
        #endregion

        private double fillHeight;
        public double FillHeight
        {
            get { return fillHeight; }
            set
            {
                fillHeight = value;
                OnPropertyChanged("FillHeight");
            }
        }
        #endregion
        #region lockObjects
        private readonly object lockObjectRefresh = new object();
        private readonly object lockObjectSettings = new object();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            settings = ((App)Application.Current).settings;

            _viewModel = new MainWindowViewModel(this);
            _settingsViewModel = new SettingsViewModel(settings);
            _streamListViewModel = new StreamListViewModel(new StreamList());
            _directoryViewModel = new DirectoryViewModel(new Directory());
            _channelListViewModel = new ChannelListViewModel(new ChannelList());

            quality = new List<string>() { "low", "medium", "high", "source" };

            InitializeComponent();

            base.DataContext = _viewModel;

            statsTab.DataContext = _settingsViewModel;
            streamListTab.DataContext = _streamListViewModel;
            directoryTab.DataContext = _directoryViewModel;
            channelsTab.DataContext = _channelListViewModel;

            ShowDebugSettings();

            //UsernameDialog usernameDialog = new UsernameDialog();
            //usernameDialog.ShowDialog();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            await InitializeWindow();
        }

        private async Task InitializeWindow()
        {
            Twixel.clientID = "mfsqshg9m7s9bw3fxyb6niddken7c39";
            twixel = new Twixel(Twixel.clientID, "http://localhost");

            var tasks = new List<Task>();

            busyIndicatorOnline.Visibility = Visibility.Visible;
            busyIndicatorOffline.Visibility = Visibility.Visible;

            foreach (StreamStatsItem stats in settings.StreamStats)
            {
                if (stats.Saved)
                    tasks.Add(addStreamItem(stats));
            }

            await Task.WhenAll(tasks);

            _streamListViewModel.ItemsOnline.Sort();
            _streamListViewModel.ItemsOffline.Sort(item => item.DisplayName);

            streamListOnline.ItemsSource = _streamListViewModel.ItemsOnline;
            streamListOffline.ItemsSource = _streamListViewModel.ItemsOffline;

            busyIndicatorOnline.Visibility = Visibility.Collapsed;
            busyIndicatorOffline.Visibility = Visibility.Collapsed;
        }

        #region Stream List
        private async void streamListAddButton_Click(object sender, RoutedEventArgs e)
        {
            var link = getStreamLink();

            if (!link.StartsWith("twitch.tv/"))
                return;

            var name = getStreamName();

            var stats = settings.StreamStats.FirstOrDefault(s => s.Name == name);

            if (stats?.Saved == true)
                return;

            if (stats == null)
            {
                StreamStatsItem newStats = new StreamStatsItem(name, true);
                stats = newStats;
                settings.StreamStats.Add(stats);
            }

            await addStreamItem(stats);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as StreamItem;
            var name = item.Name;

            if (_streamListViewModel.ItemsOnline.Contains(item))
                _streamListViewModel.ItemsOnline.Remove(item);
            else if (_streamListViewModel.ItemsOffline.Contains(item))
                _streamListViewModel.ItemsOffline.Remove(item);

            settings.StreamStats.FirstOrDefault(stats => stats.Name == name).Saved = false;
        }

        private void watchButton_Click(object sender, RoutedEventArgs e)
        {
            var link = getStreamLink();
            string args = _settingsViewModel.LivestreamerArgs != null && _settingsViewModel.LivestreamerArgs.Length != 0 ? $"{_settingsViewModel.LivestreamerArgs} {link} {SelectedQuality}" : $"{link} {SelectedQuality}";

            Process p = Process.Start(@"C:\program files (x86)\Livestreamer\livestreamer.exe", args);
        }
        #endregion
        #region Directory
        private async void gameList_Initialized(object sender, EventArgs e)
        {
            await populateGameList();

            gameList.ItemsSource = _directoryViewModel.Games;
        }

        private async void gameItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (GameItem)(sender as Grid).DataContext;

            _directoryViewModel.GameStreams.Clear();

            gameViewBackButton.Visibility = Visibility.Visible;
            directoryHeader.Content = item.FullName;

            gameStreamsOffset = 0;

            //labelDirectoryFilter.Visibility = Visibility.Collapsed;
            //textBoxDirectoryFilter.Visibility = Visibility.Collapsed;
            textBoxDirectoryFilter.Text = "";

            directoryView.Visibility = Visibility.Collapsed;
            gameView.Visibility = Visibility.Visible;

            directoryScrollViewer.ScrollToTop();

            await populateGameStreams(item.FullName);

            streamListDirectory.ItemsSource = _directoryViewModel.GameStreams;
        }

        private void directoryHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gameView.Visibility = Visibility.Collapsed;
            directoryView.Visibility = Visibility.Visible;

            directoryScrollViewer.ScrollToTop();

            textBoxDirectoryFilter.Visibility = Visibility.Visible;
            labelDirectoryFilter.Visibility = Visibility.Visible;

            gameViewBackButton.Visibility = Visibility.Collapsed;
            directoryHeader.Content = "Games";
        }

        private async Task populateGameList(int offset = 0)
        {
            if (busyIndicatorDirectory != null)
                busyIndicatorDirectory.Visibility = Visibility.Visible;

            var list = await DirectoryHandler.getGames(offset);

            if (list == null)
                return;

            if (offset == 0)
            {
                gameListOffset = 0;
                _directoryViewModel.Games.Clear();
            }

            foreach (Game game in list)
            {
                _directoryViewModel.Games.AddIfNew<GameItem>(new GameItem(game.name, DirectoryHandler.getBoxImage(game), game.viewers.Value.ToString()));
            }

            gameListOffset += 25;

            busyIndicatorDirectory.Visibility = Visibility.Collapsed;
        }

        private async Task populateGameStreams(string gameName, int offset = 0)
        {
            if (busyIndicatorGameList != null)
                busyIndicatorGameList.Visibility = Visibility.Visible;

            var list = await StreamItemHelper.getStreams(gameName, offset: gameStreamsOffset);

            foreach (TwixelAPI.Stream stream in list)
            {
                _directoryViewModel.GameStreams.AddIfNew<StreamItem>(new StreamItem(stream.channel.name, stream.channel.displayName, stream.game, StreamItemHelper.getPreview(stream, stream.channel), stream.viewers.Value.ToString()));
            }

            gameStreamsOffset += 25;

            busyIndicatorGameList.Visibility = Visibility.Collapsed;
        }

        #endregion
        #region Channels
        private async void channelsListDirectory_Initialized(object sender, EventArgs e)
        {
            await populateChannelsList();

            channelsListDirectory.ItemsSource = _channelListViewModel.Channels;
        }

        private async void channelsTab_SelectorSelected(object sender, RoutedEventArgs e)
        {
            await populateChannelsList();
        }

        private async Task populateChannelsList(int offset = 0)
        {
            if (busyIndicatorChannelsList != null)
                busyIndicatorChannelsList.Visibility = Visibility.Visible;

            var list = await DirectoryHandler.getChannels(offset);

            if (list == null)
                return;

            if (offset == 0)
            {
                channelsOffset = 0;
                _channelListViewModel.Channels.Clear();
            }

            foreach (TwixelAPI.Stream stream in list)
            {
                _channelListViewModel.Channels.AddIfNew<StreamItem>(new StreamItem(stream.channel.name, stream.channel.displayName, stream.game, StreamItemHelper.getPreview(stream, stream.channel), stream.viewers.Value.ToString()));
            }

            channelsOffset += 25;

            busyIndicatorChannelsList.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region Filter
        private async Task filterGameList(string text)
        {
            if (text.Length != 0)
            {
                var filtered = _directoryViewModel.Games.Where(game => game.FullName.ToLower().StartsWith(text.ToLower()));
                _directoryViewModel.GamesFiltered = new ObservableCollection<GameItem>(filtered);
                gameList.ItemsSource = _directoryViewModel.GamesFiltered;
            }
            else
                gameList.ItemsSource = _directoryViewModel.Games;
        }

        private async Task filterStreamList(string text, ObservableCollection<StreamItem> listToFilter, ItemsControl listToChange)
        {
            if (text.Length != 0)
            {
                var filtered = listToFilter.Where(stream => stream.DisplayName.ToLower().StartsWith(text.ToLower()));
                _channelListViewModel.ChannelsFiltered = new ObservableCollection<StreamItem>(filtered);
                listToChange.ItemsSource = _channelListViewModel.ChannelsFiltered;
            }
            else
                listToChange.ItemsSource = listToFilter;
        }

        private async void textBoxChannelsFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            await filterStreamList(textBox.Text, _channelListViewModel.Channels, channelsListDirectory);
        }

        private async void textBoxDirectoryFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (gameList.IsVisible)
                await filterGameList(textBox.Text);
            else if (streamListDirectory.IsVisible)
                await filterStreamList(textBox.Text, _directoryViewModel.GameStreams, streamListDirectory);
        }
        #endregion
        #region Stats
        private void statsTab_SelectorSelected(object sender, RoutedEventArgs e)
        {
            //settings.ViewTimes = new ObservableCollection<StreamStatsItem>(settings.StreamStats.Where(stats => stats.ViewCount != 0));
            //settings.GameViewTimes = new ObservableCollection<GameStatsItem>(settings.GameStats.Where(stats => stats.ViewCount != 0));

            settings.StreamStats.Sort();
            settings.GameStats.Sort();

            viewTimeStats.ItemsSource = settings.StreamStats.Where(stats => stats.ViewCount != 0);
            gameViewTimeStats.ItemsSource = settings.GameStats.Where(stats => stats.ViewCount != 0);
        }

        [Obsolete("addViewTime in MainWindow is deprecated, use addViewTime in StreamItem instead.", true)]
        private void addViewStats(string name, string game, TimeSpan time)
        {
            var truncatedTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds + (time.Milliseconds > 500 ? 1 : 0));
            var stats = settings.StreamStats.FirstOrDefault(item => item.Name == name);
            var gameStats = settings.GameStats.FirstOrDefault(item => item.Name == game);

            if (settings.DebugStatsLimit)
            {
                stats.ViewTime += truncatedTime;
                stats.ViewCount++;
                gameStats.ViewTime += truncatedTime;
                gameStats.ViewCount++;
                return;
            }

            if (time.TotalSeconds >= 30.0)
            {
                stats.ViewTime += truncatedTime;
                stats.ViewCount++;
                gameStats.ViewTime += truncatedTime;
                gameStats.ViewCount++;
            }
        }

        private void resetStatsButton_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(this, "Willst du wirklich alle Aufzeichnungen löschen?", "Reset", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
                settings.ResetStats();

                viewTimeStats.ItemsSource = settings.StreamStats.Where(item => item.ViewCount != 0);
                gameViewTimeStats.ItemsSource = settings.GameStats.Where(item => item.ViewCount != 0);
            }
        }

        private void deleteStatsButton_Click(object sender, RoutedEventArgs e)
        {
            var stats = (sender as Button).DataContext as StatsItem;

            var res = MessageBox.Show(this, $"Willst du die Stats von {stats.Name} wirklich löschen?", "Stats löschen", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
                if (stats is StreamStatsItem)
                    settings.DeleteStats(stats as StreamStatsItem);
                else
                    settings.DeleteStats(stats as GameStatsItem);

                viewTimeStats.ItemsSource = settings.StreamStats.Where(item => item.ViewCount != 0);
                gameViewTimeStats.ItemsSource = settings.GameStats.Where(item => item.ViewCount != 0);
            }
        }

        private void StatsSortByName(object sender, MouseButtonEventArgs e)
        {
            if (!settings.SortedByName)
            {
                settings.StreamStats.Sort(item => item.Name, false);
                settings.SortedByName = true;
                settings.SortedByViewCount = false;
                settings.SortedByViewTime = false;
                settings.SortedDescending = false;

                NameSortArrow.Source = new BitmapImage(new Uri("/imageResources/UpArrow.png", UriKind.Relative));
                ViewCountSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
                ViewTimeSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
            }
            else if (settings.SortedDescending)
            {
                settings.StreamStats.Sort(item => item.Name, false);
                settings.SortedDescending = false;

                NameSortArrow.Source = new BitmapImage(new Uri("/imageResources/UpArrow.png", UriKind.Relative));
            }
            else
            {
                settings.StreamStats.Sort(item => item.Name, true);
                settings.SortedDescending = true;

                NameSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
            }
        }

        private void StatsSortByViewCount(object sender, MouseButtonEventArgs e)
        {
            if (!settings.SortedByViewCount)
            {
                settings.StreamStats.Sort(item => item.ViewCount, true);
                settings.SortedByName = false;
                settings.SortedByViewCount = true;
                settings.SortedByViewTime = false;
                settings.SortedDescending = true;

                ViewCountSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
                NameSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
                ViewTimeSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
            }
            else if (settings.SortedDescending)
            {
                settings.StreamStats.Sort(item => item.ViewCount, false);
                settings.SortedDescending = false;

                ViewCountSortArrow.Source = new BitmapImage(new Uri("/imageResources/UpArrow.png", UriKind.Relative));
            }
            else
            {
                settings.StreamStats.Sort(item => item.ViewCount, true);
                settings.SortedDescending = true;

                ViewCountSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
            }
        }

        private void StatsSortByViewTime(object sender, MouseButtonEventArgs e)
        {
            if (!settings.SortedByViewTime)
            {
                settings.StreamStats.Sort(item => item.ViewTime, true);
                settings.SortedByName = false;
                settings.SortedByViewCount = false;
                settings.SortedByViewTime = true;
                settings.SortedDescending = true;

                ViewTimeSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
                ViewCountSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
                NameSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrowInactive.png", UriKind.Relative));
            }
            else if (settings.SortedDescending)
            {
                settings.StreamStats.Sort(item => item.ViewTime, false);
                settings.SortedDescending = false;

                ViewTimeSortArrow.Source = new BitmapImage(new Uri("/imageResources/UpArrow.png", UriKind.Relative));
            }
            else
            {
                settings.StreamStats.Sort(item => item.ViewTime, true);
                settings.SortedDescending = true;

                ViewTimeSortArrow.Source = new BitmapImage(new Uri("/imageResources/DownArrow.png", UriKind.Relative));
            }
        }
        #endregion

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as StreamItem;
            var name = item.Name;

            var stats = settings.StreamStats.FirstOrDefault(s => s.Name == name);

            if (stats?.Saved == true)
                return;

            if (stats == null)
            {
                StreamStatsItem newStats = new StreamStatsItem(name, true);
                stats = newStats;
                settings.StreamStats.Add(stats);
            }

            await addStreamItem(stats);
        }

        private void streamItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var streamItem = (StreamItem)(sender as Grid).DataContext;

            streamItem.StartStream(SelectedQuality, _settingsViewModel.LivestreamerArgs, this);
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                //refreshAllItems();
                e.Handled = true;
            }
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight && scrollViewer.VerticalOffset != 0)
            {
                if (directoryView.IsVisible)
                    await populateGameList(gameListOffset);

                else if (gameView.IsVisible)
                    await populateGameStreams((string)directoryHeader.Content, gameStreamsOffset);

                else if (channelsTab.IsVisible)
                    await populateChannelsList(channelsOffset);
            }
        }

        private void general_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private async Task addStreamItem(StreamStatsItem stats)
        {
            TwixelAPI.Stream stream = await StreamItemHelper.getStream(stats.Name);
            TwixelAPI.Channel channel = null;
            if (stream != null)
                channel = stream.channel;
            else
                channel = await StreamItemHelper.getChannel(stats.Name);

            var preview = StreamItemHelper.getPreview(stream, channel);

            string displayName = channel != null ? channel.displayName : stats.Name;

            if (stream != null)
            {
                _streamListViewModel.ItemsOnline.Add(new StreamItem(stats.Name, displayName, stream?.game, preview, stream?.viewers.Value.ToString()));
            }
            else
            {
                _streamListViewModel.ItemsOffline.Add(new StreamItem(stats.Name, displayName, stream?.game, preview, "Offline"));
            }
        }

        private string getStreamLink()
        {
            var link = textBoxStreamLink.Text;

            if (link.StartsWith("http://"))
                link = link.Substring(7);
            else if (link.StartsWith("https://"))
                link = link.Substring(8);

            if (link.StartsWith("www."))
                link = link.Substring(4);

            return link;
        }

        private string getStreamName()
        {
            var input = textBoxStreamLink.Text;

            input = input.Substring(input.LastIndexOf('/') + 1);

            return input;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewModel.FillHeight = this.ActualHeight - tabControl.Items.Count * 40.0 + 4;
        }

        #region Debug Options
        [Conditional("DEBUG")]
        private void ShowDebugSettings()
        {
            DebugSettings.Visibility = Visibility.Visible;
            DebugSettings.DataContext = settings;
        }
        #endregion
    }
}
