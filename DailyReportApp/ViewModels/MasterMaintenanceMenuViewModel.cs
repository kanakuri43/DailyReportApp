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

        public DelegateCommand WorkContentsCommand { get; }
        public DelegateCommand EmployeesCommand { get; }
        public DelegateCommand MachinesCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public MasterMaintenanceMenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            WorkContentsCommand = new DelegateCommand(WorkContentsCommandExecute);
            EmployeesCommand = new DelegateCommand(EmployeesCommandExecute);
            MachinesCommand = new DelegateCommand(MachinesCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);

        }

        private void WorkContentsCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(MasterListViewModel.CurrentMasterType), MasterType.WorkContents);
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterList), p);
        }
        private void EmployeesCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(MasterListViewModel.CurrentMasterType), MasterType.Employees);
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterList), p);
        }
        private void MachinesCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(MasterListViewModel.CurrentMasterType), MasterType.Machines);
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterList), p);
        }
        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Dashboard), p);
        }
    }
}
