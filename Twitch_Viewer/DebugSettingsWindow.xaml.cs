using System;
using System.Linq;
using System.Windows;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for DebugSettingsWindow.xaml
    /// </summary>
    public partial class DebugSettingsWindow : Window
    {
        public bool DebugStatsLimit { get; set; } = true;

        public DebugSettingsWindow()
        {
            InitializeComponent();
        }
    }
}
