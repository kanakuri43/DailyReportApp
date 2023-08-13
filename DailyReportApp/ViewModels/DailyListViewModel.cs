using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using DailyReportApp.Views;
using System.Collections.ObjectModel;
using DailyReportApp.Models;
using Microsoft.Data.SqlClient;


namespace DailyReportApp.ViewModels
{
    public class DailyListViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<MonthlyReportListViewModel> _reportList = new ObservableCollection<MonthlyReportListViewModel>();
        private DateTime _selectedDate;
        private int _selectedReportId;

        public DelegateCommand RegisterReportCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ReportListDoubleClickCommand { get; }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }
        public ObservableCollection<MonthlyReportListViewModel> ReportList
        {
            get => _reportList;
            set => SetProperty(ref _reportList, value);
        }
        public int SelectedReportId
        {
            get { return _selectedReportId; }
            set { SetProperty(ref _selectedReportId, value); }
        }

        public DailyListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            RegisterReportCommand = new DelegateCommand(RegisterReportCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            ReportListDoubleClickCommand = new DelegateCommand(ReportListDoubleClickCommandExecute);

        }

        private void RegisterReportCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }
        private void ReportListDoubleClickCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(RegisterReportViewModel.ReportId), SelectedReportId);
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedDate = navigationContext.Parameters.GetValue<DateTime>(nameof(SelectedDate));
            ShowMonthlyReport();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void ShowMonthlyReport()
        {
            SqlDataReader dr;

            ReportList.Clear();

            var db = new Database();
            db.SQL = "SELECT "
                    + "   work_date "
                    + "   , work_content_name "
                    + " FROM "
                    + "   uv_daily_reports "
                    + " WHERE "
                    + "   work_date ='" + SelectedDate.ToString("yyyy/MM/dd") + "'"
                    ;
            dr = db.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ReportList.Add(new MonthlyReportListViewModel(DateTime.Parse(dr["work_date"].ToString()), dr["work_content_name"].ToString()));

                }
            }
        }
        private void CancelCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Dashboard), p);
        }
    }
}
