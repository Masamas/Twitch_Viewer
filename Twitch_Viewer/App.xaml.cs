using System.Windows;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Settings settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SettingsHelper settingsHelper = new SettingsHelper();
            settings = settingsHelper.Load();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SettingsHelper settingsHelper = new SettingsHelper();
            settingsHelper.Save(settings);
        }
    }
}
