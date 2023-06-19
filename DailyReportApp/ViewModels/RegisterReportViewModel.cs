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
using System.Diagnostics;

namespace DailyReportApp.ViewModels
{
    public class RegisterReportViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        //private ObservableCollection<Employee> _employees;
        private ObservableCollection<ComboBoxViewModel> _employees = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<ComboBoxViewModel> _workContents = new ObservableCollection<ComboBoxViewModel>();
        private DateTime _reportDate;
        private int _authorId;
        private int _workContentId;
        private float _workingHours;



        public RegisterReportViewModel(IRegionManager regionManager)
        {

            _regionManager = regionManager;

            RegisterCommand = new DelegateCommand(RegisterCommandExecute);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);

            ReportDate = DateTime.Today;

            SqlDataReader dr;

            var dbEmp = new Database();
            dbEmp.SQL = "SELECT "
                    + "  employee_id "
                    + "  , employee_name "
                    + " FROM "
                    + "   employees "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   employee_id ";
            dr = dbEmp.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    Employees.Add(new ComboBoxViewModel(int.Parse(dr["employee_id"].ToString()), dr["employee_name"].ToString()));
                }
            }



            var dbWC = new Database();
            dbWC.SQL = "SELECT "
                    + "  work_content_id "
                    + "  , work_content_name "
                    + " FROM "
                    + "   work_contents "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   work_content_id ";
            dr = dbWC.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    WorkContents.Add(new ComboBoxViewModel(int.Parse(dr["work_content_id"].ToString()), dr["work_content_name"].ToString()));
                }
            }


        }
        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DateTime ReportDate
        {
            get { return _reportDate; }
            set { SetProperty(ref _reportDate, value); }
        }

        public ObservableCollection<ComboBoxViewModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }
        public ObservableCollection<ComboBoxViewModel> WorkContents
        {
            get => _workContents;
            set => SetProperty(ref _workContents, value);
        }

        public int AuthorId
        {
            get { return _authorId; }
            set { SetProperty(ref _authorId, value); }
        }
        public int WorkContentId
        {
            get { return _workContentId; }
            set { SetProperty(ref _workContentId, value); }
        }
        public float WorkingHours
        {
            get { return _workingHours; }
            set { SetProperty(ref _workingHours, value); }
        }

        private void RegisterCommandExecute()
        {
            string connectionString = @"Data Source=192.168.3.11;Initial Catalog=daily_report_db;User ID=sa;Password=Sapassword1;Encrypt=false"; // your connection string here
            DateTime workDate = DateTime.Now;
            int machineId = 1;
            string notes = "Test note";
            int[] employeeIds = new int[] { 3, 5, 7 }; // the assignee IDs

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_register_daily_report", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_work_date", ReportDate));
                    command.Parameters.Add(new SqlParameter("@arg_author_id", AuthorId));
                    command.Parameters.Add(new SqlParameter("@arg_work_content_id", WorkContentId));
                    command.Parameters.Add(new SqlParameter("@arg_working_hours", WorkingHours));
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
