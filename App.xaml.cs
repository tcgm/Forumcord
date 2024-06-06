using CefSharp;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Forumcord
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CefSharpInitializer.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Cef.Shutdown();
            base.OnExit(e);
        }
    }

}
