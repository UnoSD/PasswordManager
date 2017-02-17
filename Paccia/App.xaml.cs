using System;
using System.Windows;
using System.Windows.Threading;

namespace Paccia
{
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show($"Exception caught: {((Exception)args.ExceptionObject).Message}");
            MessageBox.Show($"Runtime terminating: {args.IsTerminating}");
        }

        void HandleUiUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Exception caught: {e.Exception}");

            e.Handled = true;
        }
    }
}
