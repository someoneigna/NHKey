using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace NHkey
{
    public partial class App : Application
    {
        static string LogFile = "errorLog.log";
        public static App Instance { get; protected set; }

        public void SwitchLanguage(string language)
        {
            ResourceDictionary theme = Resources.MergedDictionaries.Where((rd) => rd.Source.ToString().Contains("Theme.xaml")).Single();
            Resources.MergedDictionaries.Clear();

            Resources.MergedDictionaries.Add(theme);

            var newDict = new ResourceDictionary();
            newDict.Source = new Uri("Resources/" + "Lang." + language + ".xaml", UriKind.Relative);

            Resources.MergedDictionaries.Add(newDict);
            
            foreach(Window window in this.Windows)
            {
                window.InvalidateVisual();
            }
        }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

            Instance = this;
        }

        private void DispatcherUnhandledExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NotifyException(e.Exception);
            e.Handled = true;
            Application.Current.Shutdown();
        }

        private static void NotifyException(object exception)
        {
            StreamWriter logStream = null;
            Exception except = (Exception)exception;
            string errorMessage = DateTime.Now.ToLocalTime() + "\ngot error: " + except.Message + "\nfrom: " + except.Source;

            if (File.Exists(LogFile))
                logStream = File.AppendText(LogFile);
            else
                logStream = File.CreateText(LogFile);
            
            
            logStream.WriteLine(errorMessage.Replace('\n', ' '));
            logStream.Close();

            MessageBox.Show(errorMessage, "Unhandled error ocurred", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            NotifyException(e.ExceptionObject);
            Application.Current.Shutdown();
        }
    }
}
