using DailyReportApp.Models;
using DailyReportApp.Views;
using Microsoft.Data.SqlClient;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DailyReportApp.ViewModels
{
	public class MasterListViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<ComboBoxViewModel> _flexMasterList = new ObservableCollection<ComboBoxViewModel>();
        private int _selectedId;
        private int _masterType;

        public DelegateCommand MouseDoubleClickCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public ObservableCollection<ComboBoxViewModel> FlexMastertList
        {
            get => _flexMasterList;
            set => SetProperty(ref _flexMasterList, value);
        }
        public int SelectedId
        {
            get { return _selectedId; }
            set { SetProperty(ref _selectedId, value); }
        }
        public int CurrentMasterType
        {
            get { return _masterType; }
            set { SetProperty(ref _masterType, value); }
        }

        public MasterListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            MouseDoubleClickCommand = new DelegateCommand(MouseDoubleClickCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);

            ShowMasterList();
        }

        private void ShowMasterList()
        {
            SqlDataReader dr;

            FlexMastertList.Clear();

            var db = new Database();
            switch (CurrentMasterType)
            {
                case (int)MasterType.Employees:
                    db.SQL = "SELECT "
                            + "   employee_id "
                            + "   , employee_name "
                            + " FROM "
                            + "   employees "
                            + " WHERE "
                            + "   state = 0 "
                            + " ORDER BY "
                            + "   employee_id ";
                    break;
                case (int)MasterType.WorkContents:
                    db.SQL = "SELECT "
                            + "   work_content_id "
                            + "   , work_content_name "
                            + " FROM "
                            + "   work_contents "
                            + " WHERE "
                            + "   state = 0 "
                            + " ORDER BY "
                            + "   work_content_id ";
                    break;
                case (int)MasterType.Machines:
                    db.SQL = "SELECT "
                            + "   machine_id "
                            + "   , machine_name "
                            + " FROM "
                            + "   machines "
                            + " WHERE "
                            + "   state = 0 "
                            + " ORDER BY "
                            + "   machine_id ";
                    break;
            }
            dr = db.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    FlexMastertList.Add(new ComboBoxViewModel((int)dr[0], dr[1].ToString()));
                }
            }
        }

        private void MouseDoubleClickCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(EmployeeMaintenanceViewModel.EmployeeId), SelectedId);
            _regionManager.RequestNavigate("ContentRegion", nameof(EmployeeMaintenance), p);
        }
        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterMaintenanceMenu), p);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CurrentMasterType = navigationContext.Parameters.GetValue<int>(nameof(CurrentMasterType));
            ShowMasterList();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
