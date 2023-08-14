using DailyReportApp.Views;
using DailyReportApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DailyReportApp.ViewModels
{
	public class MasterMaintenanceMenuViewModel : BindableBase
	{
        private readonly IRegionManager _regionManager;

        public DelegateCommand CancelCommand { get; }

        public MasterMaintenanceMenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CancelCommand = new DelegateCommand(CancelCommandExecute);

        }
        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Dashboard), p);
        }
    }
}
