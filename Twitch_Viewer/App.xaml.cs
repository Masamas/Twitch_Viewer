using System;
using System.IO;
using System.Windows;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Settings settings;
        private SettingsHelper settingsHelper;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            settingsHelper = new SettingsHelper();
            settings = settingsHelper.Load();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            settingsHelper.Save(settings);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var now = DateTime.Now;

            string fileName = $"Exceptions/ExceptionStackTrace_{now.Day}_{now.Month}_{now.Year}-{now.Hour}_{now.Minute}_{now.Second}";

            if (!Directory.Exists("Exceptions"))
                Directory.CreateDirectory("Exceptions");

            using (StreamWriter sr = new StreamWriter(fileName))
            {
                sr.WriteLine(e.Exception.Message);
                sr.WriteLine();
                sr.WriteLine(e.Exception.StackTrace);
            }

            MessageBox.Show("An unhandled exception occurred: " + e.Exception.Message, "Exception occurred", MessageBoxButton.OK, MessageBoxImage.Warning);

#if !DEBUG
            e.Handled = true;
#endif
        }
    }
}
