using DailyReportApp.Models;
using DailyReportApp.Views;
using Microsoft.Data.SqlClient;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DailyReportApp.ViewModels
{
    public class MachineMaintenanceViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private int _machineId;
        private string _machineName;

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public int MachineId
        {
            get { return _machineId; }
            set { SetProperty(ref _machineId, value); }
        }
        public string MachineName
        {
            get { return _machineName; }
            set { SetProperty(ref _machineName, value); }
        }

        public MachineMaintenanceViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            RegisterCommand = new DelegateCommand(RegisterCommandExecute);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }

        private void RegisterCommandExecute()
        {
            var db = new Database();
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_register_machine", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_machine_id", MachineId));
                    command.Parameters.Add(new SqlParameter("@arg_machine_name", MachineName));

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteCommandExecute()
        {
            var db = new Database();
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_delete_machine", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_machine_id", MachineId));

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ShowMachineInfo()
        {
            var db = new Database();
            SqlDataReader dr;

            if (MachineId == 0) return;

            // 日付・作業内容 等
            db.SQL = "SELECT TOP 1 "
                    + "   * "
                    + " FROM "
                    + "   machines "
                    + " WHERE "
                    + "   state = 0 "
                    + "   AND machine_id =" + MachineId.ToString()
                    ;
            dr = db.ReadAsDataReader();
            if (dr == null) return;

            while (dr.Read())
            {
                MachineId = (int)dr["machine_id"];
                MachineName = dr["machine_name"].ToString();
            }
            dr.Close();

        }

        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(MasterListViewModel.CurrentMasterType), MasterType.Machines);
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterList), p);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            MachineId = navigationContext.Parameters.GetValue<int>(nameof(MachineId));
            ShowMachineInfo();
        }
    }
}
