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
    public class WorkContentMaintenanceViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private int _workContentId;
        private string _workContentName;

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public int WorkContentId
        {
            get { return _workContentId; }
            set { SetProperty(ref _workContentId, value); }
        }
        public string WorkContentName
        {
            get { return _workContentName; }
            set { SetProperty(ref _workContentName, value); }
        }

        public WorkContentMaintenanceViewModel(IRegionManager regionManager)
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

                using (SqlCommand command = new SqlCommand("usp_register_work_content", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_work_content_id", WorkContentId));
                    command.Parameters.Add(new SqlParameter("@arg_work_content_name", WorkContentName));

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

                using (SqlCommand command = new SqlCommand("usp_delete_work_content", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters to SqlCommand
                    command.Parameters.Add(new SqlParameter("@arg_work_content_id", WorkContentId));

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ShowWorkContentInfo()
        {
            var db = new Database();
            SqlDataReader dr;

            if (WorkContentId == 0) return;

            // 日付・作業内容 等
            db.SQL = "SELECT TOP 1 "
                    + "   * "
                    + " FROM "
                    + "   work_contents "
                    + " WHERE "
                    + "   state = 0 "
                    + "   AND work_content_id =" + WorkContentId.ToString()
                    ;
            dr = db.ReadAsDataReader();
            if (dr == null) return;

            while (dr.Read())
            {
                WorkContentId = (int)dr["work_content_id"];
                WorkContentName = dr["work_content_name"].ToString();
            }
            dr.Close();

        }

        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(MasterListViewModel.CurrentMasterType), MasterType.WorkContents);
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
            WorkContentId = navigationContext.Parameters.GetValue<int>(nameof(WorkContentId));
            ShowWorkContentInfo();
        }
    }
}
