using DailyReportApp.Views;
using Microsoft.Data.SqlClient;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Azure.Core.HttpHeader;

namespace DailyReportApp.ViewModels
{
    public class EmployeeMaintenanceViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private int _employeeId;
        private string _employeeName;

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public int EmployeeId
        {
            get { return _employeeId; }
            set { SetProperty(ref _employeeId, value); }
        }
        public string EmployeeName
        {
            get { return _employeeName; }
            set { SetProperty(ref _employeeName, value); }
        }

        public EmployeeMaintenanceViewModel(IRegionManager regionManager)
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

                using (SqlCommand command = new SqlCommand("usp_register_employee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_employee_id", EmployeeId));
                    command.Parameters.Add(new SqlParameter("@arg_employee_name", EmployeeName));

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

                using (SqlCommand command = new SqlCommand("usp_delete_employee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_employee_id", EmployeeId));

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(MasterList), p);
        }

        private void ShowEmployeeInfo()
        {
            var db = new Database();
            SqlDataReader dr;

            if (EmployeeId == 0) return;

            // 日付・作業内容 等
            db.SQL = "SELECT TOP 1 "
                    + "   * "
                    + " FROM "
                    + "   employees "
                    + " WHERE "
                    + "   state = 0 "
                    + "   AND employee_id =" + EmployeeId.ToString()
                    ;
            dr = db.ReadAsDataReader();
            if (dr == null) return;

            while (dr.Read())
            {
                EmployeeId = (int)dr["employee_id"];
                EmployeeName = dr["employee_name"].ToString();
            }
            dr.Close();

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
            EmployeeId = navigationContext.Parameters.GetValue<int>(nameof(EmployeeId));
            ShowEmployeeInfo();
        }
    }
}
