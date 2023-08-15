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
	public class MasterListViewModel : BindableBase
	{
        private readonly IRegionManager _regionManager;

        private ObservableCollection<ComboBoxViewModel> _flexMasterList = new ObservableCollection<ComboBoxViewModel>();
        private int _selectedId;

        public DelegateCommand MouseDoubleClickCommand { get; }

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

        public MasterListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            MouseDoubleClickCommand = new DelegateCommand(MouseDoubleClickCommandExecute);

            ShowMasterList();
        }

        private void ShowMasterList()
        {
            SqlDataReader dr;

            FlexMastertList.Clear();

            var db = new Database();
            db.SQL = "SELECT "
                    + "   employee_id "
                    + "   , employee_name "
                    + " FROM "
                    + "   employees "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   employee_id ";
            dr = db.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    FlexMastertList.Add(new ComboBoxViewModel((int)dr["employee_id"], dr["employee_name"].ToString()));

                }
            }
        }

        private void MouseDoubleClickCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(EmployeeMaintenanceViewModel.EmployeeId), SelectedId);
            _regionManager.RequestNavigate("ContentRegion", nameof(EmployeeMaintenance), p);
        }
    }
}
