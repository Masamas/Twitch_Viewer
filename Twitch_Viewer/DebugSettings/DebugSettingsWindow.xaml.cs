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
            DebugSettingsItems.Add(new DebugSettingButton("Show Debug settings status.", "Show!", showStatus, null));
        }

        public void showStatus(object parameters)
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
    }
}
