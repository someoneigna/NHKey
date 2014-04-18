using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace NHkey
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        static string LogFile = "errorLog.log";

        public App()
        {

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;
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
