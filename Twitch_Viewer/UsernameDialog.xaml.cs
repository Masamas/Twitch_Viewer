using System;
using System.Collections.Generic;
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
    /// Interaction logic for UsernameDialog.xaml
    /// </summary>
    public partial class UsernameDialog : Window
    {
        public UsernameDialog()
        {
            InitializeComponent();
        }

        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUserName.Text.Length != 0 && textBoxUserName.Text != "Username")
                MainWindow.username = textBoxUserName.Text;

            this.Close();
        }

        private void textBoxUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxUserName.Text == "Username")
            {
                textBoxUserName.Text = "";
                textBoxUserName.Foreground = Brushes.Black;
            }
        }

        private void textBoxUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxUserName.Text == "")
            {
                textBoxUserName.Text = "Username";
                textBoxUserName.Foreground = Brushes.LightGray;
            }
        }

        private void textBoxUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.username = textBoxUserName.Text;
                this.Close();
            }
        }
    }
}
