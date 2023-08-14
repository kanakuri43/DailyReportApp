using DailyReportApp.Models;
using DailyReportApp.Views;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System.Windows;

namespace DailyReportApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Dashboard>();
            containerRegistry.RegisterForNavigation<RegisterReport>();
            containerRegistry.RegisterForNavigation<FindReport>();
            containerRegistry.RegisterForNavigation<DailyList>();
            containerRegistry.RegisterForNavigation<MasterMaintenanceMenu>();
            containerRegistry.RegisterForNavigation<MasterList>();

        }

    }
}
