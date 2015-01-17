using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Waf.Foundation;
using System.Windows;
using PlanterApp.Applications.Controllers;
using System;
using System.Waf.Applications;
using System.Windows.Threading;

namespace PlanterApp
{
    public partial class App : Application//, IDisposable
    {
        private AggregateCatalog _catalog;
        private CompositionContainer _container;
        private ApplicationController _controller;
        //private bool _disposed;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            _catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(Model).Assembly));
            // Add the WafApplication assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            _container = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(_container);
            _container.Compose(batch);

            _controller = _container.GetExportedValue<ApplicationController>();
            _controller.Initialize();
            _controller.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _controller.OnShutdown();
            _container.Dispose();
            _catalog.Dispose();

            base.OnExit(e);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            if (!isTerminating)
            {
                MessageBox.Show("Unexpected error: " + e.ToString(), ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
