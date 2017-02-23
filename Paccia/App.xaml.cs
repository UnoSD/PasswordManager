using System;
using System.Windows;
using System.Windows.Threading;
using StructureMap;
using StructureMap.Graph;

namespace Paccia
{
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new Container(SetupContainer);

            var logger = container.GetInstance<Logger>();

            logger.Log(container.WhatDoIHave());
            logger.Log(container.WhatDidIScan());

            var mainWindow = container.GetInstance<MainWindow>();

            MainWindow = mainWindow;

            MainWindow.Show();
        }

        static void SetupContainer(ConfigurationExpression configuration)
        {
            configuration.Scan(ScannerConfiguration);
            configuration.For(typeof(ISerializer<>)).Singleton().Use(typeof(BinarySerializer<>));
        }

        static void ScannerConfiguration(IAssemblyScanner scanner)
        {
            scanner.TheCallingAssembly();
            scanner.SingleImplementationsOfInterface();
            scanner.WithDefaultConventions();
            // Using OnAddedPluginType doesn't make concrete types singletons by default.
            scanner.With(new SingletonConvention());
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
