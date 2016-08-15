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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twitch_Viewer.StyleResources
{
    /// <summary>
    /// Interaction logic for TestItem.xaml
    /// </summary>
    public partial class TestItem : UserControl
    {
        public ObservableCollection<StreamItem> OnlineStreamItems
        {
            get { return (ObservableCollection<StreamItem>)GetValue(OnlineStreamItemsProperty); }
            set { SetValue(OnlineStreamItemsProperty, value); }
        }

        public static readonly DependencyProperty OnlineStreamItemsProperty = DependencyProperty.Register("OnlineStreamItems", typeof(ObservableCollection<StreamItem>), typeof(TestItem));

        public ObservableCollection<StreamItem> OfflineStreamItems
        {
            get { return (ObservableCollection<StreamItem>)GetValue(OfflineStreamItemsProperty); }
            set { SetValue(OfflineStreamItemsProperty, value); }
        }

        public static readonly DependencyProperty OfflineStreamItemsProperty = DependencyProperty.Register("OfflineStreamItems", typeof(ObservableCollection<StreamItem>), typeof(TestItem));

        public Visibility IsBusy
        {
            get { return (Visibility)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(Visibility), typeof(TestItem));

        public string StreamLink
        {
            get { return (string)GetValue(StreamLinkProperty); }
            set { SetValue(StreamLinkProperty, value); }
        }

        public static readonly DependencyProperty StreamLinkProperty = DependencyProperty.Register("StreamLink", typeof(string), typeof(TestItem));

        public TestItem()
        {
            InitializeComponent();
        }
    }
}
