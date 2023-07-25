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
    public class DashboardViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<ComboBoxViewModel> _years = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<ComboBoxViewModel> _months = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<MonthlyReportListViewModel> _reportList = new ObservableCollection<MonthlyReportListViewModel>();
        private int _year;
        private int _month;

        public DashboardViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            RegisterReportCommand = new DelegateCommand(RegisterReportCommandExecute);
            YearMonthSelectionChanged = new DelegateCommand(YearMonthSelectionChangedExecute);
            ReportListDoubleClickCommand = new DelegateCommand(ReportListDoubleClickCommandExecute);

            SelectedYear = DateTime.Now.Year;
            SelectedMonth = DateTime.Now.Month;

            var span = 10;
            int i;
            for (i = 0; i < span; i++)
            {
                Years.Add(new ComboBoxViewModel(SelectedYear - i, (SelectedYear - i).ToString()));
            }
            for (i = 1; i <= 12; i++)
            {
                Months.Add(new ComboBoxViewModel(i,  i.ToString()));
            }

            ShowMonthlyReport();
        }

        public DelegateCommand RegisterReportCommand { get; }
        public DelegateCommand YearMonthSelectionChanged { get; }
        public DelegateCommand ReportListDoubleClickCommand { get; }

        public ObservableCollection<ComboBoxViewModel> Years
        {
            get => _years;
            set => SetProperty(ref _years, value);
        }
        public ObservableCollection<ComboBoxViewModel> Months
        {
            get => _months;
            set => SetProperty(ref _months, value);
        }

        public int SelectedYear
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }
        public int SelectedMonth
        {
            get { return _month; }
            set { SetProperty(ref _month, value); }
        }
        public ObservableCollection<MonthlyReportListViewModel> ReportList
        {
            get => _reportList;
            set => SetProperty(ref _reportList, value);
        }

        private void RegisterReportCommandExecute()
        {
            // Menu表示
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }
        private void ReportListDoubleClickCommandExecute()
        {
            // Menu表示
            var p = new NavigationParameters();
            p.Add(nameof(DailyListViewModel.SelectedDate), DateOnly.Parse("2023/01/01"));
            _regionManager.RequestNavigate("ContentRegion", nameof(DailyList), p);

        }

        private void YearMonthSelectionChangedExecute()
        {
            ShowMonthlyReport();
        }

        private void ShowMonthlyReport()
        {
            SqlDataReader dr;

            ReportList.Clear();

            var dbEmployees = new Database();
            dbEmployees.SQL = "SELECT "
                    + "   work_date "
                    + "   , MIN(work_content_name) work_content_name "
                    + " FROM "
                    + "   uv_daily_reports "
                    + " WHERE "
                    + "   YEAR(work_date) =" + SelectedYear.ToString()
                    + "   AND MONTH(work_date) =" + SelectedMonth.ToString()
                    + " GROUP BY "
                    + "   work_date "
                    + " ORDER BY "
                    + "   work_date ";
            dr = dbEmployees.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ReportList.Add(new MonthlyReportListViewModel(DateTime.Parse(dr["work_date"].ToString()), dr["work_content_name"].ToString()));

                }
            }


        }
    }
}
