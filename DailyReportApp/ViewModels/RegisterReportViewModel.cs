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
    public class RegisterReportViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        //private ObservableCollection<Employee> _employees;
        private ObservableCollection<ComboBoxViewModel> _employees = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<ComboBoxViewModel> _workContents = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<ComboBoxViewModel> _machines = new ObservableCollection<ComboBoxViewModel>();
        private int _reportId;
        private DateTime _reportDate;
        private int _authorId;
        private int _workContentId;
        private float _workingHours;
        private int _machineId;
        private string _notes = "";
        private ObservableCollection<MultiSelectViewModel> _workers;

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public int ReportId
        {
            get { return _reportId; }
            set { SetProperty(ref _reportId, value); }
        }
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
        public ObservableCollection<ComboBoxViewModel> Machines
        {
            get => _machines;
            set => SetProperty(ref _machines, value);
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
        public int MachineId
        {
            get { return _machineId; }
            set { SetProperty(ref _machineId, value); }
        }
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }
        public ObservableCollection<MultiSelectViewModel> Workers
        {
            get { return _workers; }
            set { SetProperty(ref _workers, value); }
        }

        public RegisterReportViewModel(IRegionManager regionManager)
        {

            _regionManager = regionManager;

            RegisterCommand = new DelegateCommand(RegisterCommandExecute);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);

            InitializeScreen();

            SqlDataReader dr;
            var dbEmployees = new Database();
            dbEmployees.SQL = "SELECT "
                    + "  employee_id "
                    + "  , employee_name "
                    + " FROM "
                    + "   employees "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   employee_id ";
            dr = dbEmployees.ReadAsDataReader();
            if (dr != null)
            {
                Workers = new ObservableCollection<MultiSelectViewModel>();
                while (dr.Read())
                {
                    Employees.Add(new ComboBoxViewModel(int.Parse(dr["employee_id"].ToString()), dr["employee_name"].ToString()));
                    Workers.Add(new MultiSelectViewModel() { Id = int.Parse(dr["employee_id"].ToString()), Name = dr["employee_name"].ToString(), Selected = false });

                }
            }

            var dbWorkContents = new Database();
            dbWorkContents.SQL = "SELECT "
                    + "  work_content_id "
                    + "  , work_content_name "
                    + " FROM "
                    + "   work_contents "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   work_content_id ";
            dr = dbWorkContents.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    WorkContents.Add(new ComboBoxViewModel(int.Parse(dr["work_content_id"].ToString()), dr["work_content_name"].ToString()));
                }
            }

            var dbMachines = new Database();
            dbMachines.SQL = "SELECT "
                    + "  machine_id "
                    + "  , machine_name "
                    + " FROM "
                    + "   machines "
                    + " WHERE "
                    + "   state = 0 "
                    + " ORDER BY "
                    + "   machine_id ";
            dr = dbMachines.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    Machines.Add(new ComboBoxViewModel(int.Parse(dr["machine_id"].ToString()), dr["machine_name"].ToString()));
                }
            }
        }

        private void RegisterCommandExecute()
        {
            var db = new Database();
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                if (ReportId != 0)
                {
                    using (SqlCommand command = new SqlCommand("usp_delete_daily_report", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to SqlCommand
                        command.Parameters.Add(new SqlParameter("@arg_daily_report_id", ReportId));

                        // Execute the command
                        command.ExecuteNonQuery();
                    }

                }

                using (SqlCommand command = new SqlCommand("usp_register_daily_report", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_work_date", ReportDate));
                    command.Parameters.Add(new SqlParameter("@arg_author_id", AuthorId));
                    command.Parameters.Add(new SqlParameter("@arg_work_content_id", WorkContentId));
                    command.Parameters.Add(new SqlParameter("@arg_working_hours", WorkingHours));
                    command.Parameters.Add(new SqlParameter("@arg_machine_id", MachineId));
                    command.Parameters.Add(new SqlParameter("@arg_notes", Notes));

                    // Create DataTable for employeeIds
                    DataTable employeeIdsTable = new DataTable();
                    employeeIdsTable.Columns.Add("employee_id", typeof(int));

                    foreach (var worker in Workers)
                    {
                        if (worker.Selected)
                        {
                            employeeIdsTable.Rows.Add(worker.Id);
                        }
                    }

                    // Create SqlParameter for @arg_employee_ids
                    SqlParameter employeeIdsParameter = command.Parameters.AddWithValue("@arg_employee_ids", employeeIdsTable);
                    employeeIdsParameter.SqlDbType = SqlDbType.Structured;
                    employeeIdsParameter.TypeName = "daily_report_worker_ids_table_type";

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
            InitializeScreen();

        }

        private void ShowReportContents()
        {
            var db = new Database();
            SqlDataReader dr;

            if (ReportId == 0) return;

            // 日付・作業内容 等
            db.SQL = "SELECT TOP 1 "
                    + "   * "
                    + " FROM "
                    + "   daily_reports "
                    + " WHERE "
                    + "   daily_report_id =" + ReportId.ToString()
                    ;
            dr = db.ReadAsDataReader();
            if (dr == null) return;

            while (dr.Read())
            {
                ReportDate = DateTime.Parse(dr["work_date"].ToString());
                AuthorId = (int)dr["author_id"];
                WorkContentId = (int)dr["work_content_id"];
                WorkingHours = (float)((double)dr["working_hours"]);    // SQLServerのfloatはC#ではdoubleと認識されるため
                MachineId = (int)dr["machine_id"];
                Notes = dr["notes"].ToString();
            }
            dr.Close();

            // 作業担当者
            db.SQL = "SELECT "
                    + "   * "
                    + " FROM "
                    + "   daily_report_workers "
                    + " WHERE "
                    + "   daily_report_id =" + ReportId.ToString()
                    ;
            dr = db.ReadAsDataReader();
            if (dr == null) return;

            while (dr.Read())
            {
                foreach (var worker in Workers)
                {
                    if (worker.Id == (int)dr["employee_id"])
                    {
                        worker.Selected = true;
                    }
                }
            }
            dr.Close();

        }

        private void DeleteCommandExecute()
        {
            var db = new Database();
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                if (ReportId != 0)
                {
                    using (SqlCommand command = new SqlCommand("usp_delete_daily_report", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to SqlCommand
                        command.Parameters.Add(new SqlParameter("@arg_daily_report_id", ReportId));

                        // Execute the command
                        command.ExecuteNonQuery();
                    }

                }
            }
            InitializeScreen();
        }

        private void InitializeScreen()
        {
            ReportDate = DateTime.Today;
            AuthorId = 0;
            WorkContentId = 0;
            WorkingHours = 0;    
            MachineId = 0;
            Notes = "";

        }

        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            if (ReportId == 0)
            {
                _regionManager.RequestNavigate("ContentRegion", nameof(Dashboard), p);
            }
            else
            {
                p.Add(nameof(DailyListViewModel.SelectedDate), ReportDate);
                _regionManager.RequestNavigate("ContentRegion", nameof(DailyList), p);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ReportId = navigationContext.Parameters.GetValue<int>(nameof(ReportId));
            ShowReportContents();
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
