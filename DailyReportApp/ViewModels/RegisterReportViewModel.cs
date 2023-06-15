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
    public class RegisterReportViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private ObservableCollection<Employee> _employees;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }


        public RegisterReportViewModel(IRegionManager regionManager)
        {

            _regionManager = regionManager;

            RegisterCommand = new DelegateCommand(RegisterCommandExecute);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }
        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private void RegisterCommandExecute()
        {
            string connectionString = @"Data Source=192.168.3.11;Initial Catalog=DailyReportDB;User ID=sa;Password=Sapassword1;Encrypt=false"; // your connection string here
            string operation = "INSERT"; // or "UPDATE"
            int? reportId = null; // set this if operation is "UPDATE"
            DateTime workDate = DateTime.Now;
            int authorId = 7;
            int workContentId = 4;
            decimal workingHours = 3;
            int machineId = 1;
            string notes = "Test note";
            int[] employeeIds = new int[] { 1, 3, 5, 7 }; // the assignee IDs

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_InsertOrUpdateDailyReport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_operation", operation));
                    command.Parameters.Add(new SqlParameter("@arg_daily_report_id", reportId));
                    command.Parameters.Add(new SqlParameter("@arg_work_date", DateTime.Parse("2023/05/01")));
                    command.Parameters.Add(new SqlParameter("@arg_author_id", authorId));
                    command.Parameters.Add(new SqlParameter("@arg_work_content_id", workContentId));
                    command.Parameters.Add(new SqlParameter("@arg_working_hours", workingHours));
                    command.Parameters.Add(new SqlParameter("@arg_machine_id", machineId));
                    command.Parameters.Add(new SqlParameter("@arg_notes", notes));

                    // Create DataTable for employeeIds
                    DataTable employeeIdsTable = new DataTable();
                    employeeIdsTable.Columns.Add("employee_id", typeof(int));

                    foreach (var id in employeeIds)
                    {
                        employeeIdsTable.Rows.Add(id);
                    }

                    // Create SqlParameter for @arg_employee_ids
                    SqlParameter employeeIdsParameter = command.Parameters.AddWithValue("@arg_employee_ids", employeeIdsTable);
                    employeeIdsParameter.SqlDbType = SqlDbType.Structured;
                    employeeIdsParameter.TypeName = "daily_report_worker_ids_table_type";

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }

        }

        private void DeleteCommandExecute()
        {

        }

        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Dashboard), p);

        }

    }
}
