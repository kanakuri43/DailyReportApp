using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using DailyReportApp.Views;


namespace DailyReportApp.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DashboardViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            RegisterReportCommand = new DelegateCommand(RegisterReportCommandExecute);
        }

        public DelegateCommand RegisterReportCommand { get; }

        private void RegisterReportCommandExecute()
        {
            // Menu表示
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }
    }
}
