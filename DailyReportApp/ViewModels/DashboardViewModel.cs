﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using DailyReportApp.Views;
using System.Collections.ObjectModel;
using DailyReportApp.Models;

namespace DailyReportApp.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<ComboBoxViewModel> _years = new ObservableCollection<ComboBoxViewModel>();
        private ObservableCollection<ComboBoxViewModel> _months = new ObservableCollection<ComboBoxViewModel>();

        private int _year;
        private int _month;

        public DashboardViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            RegisterReportCommand = new DelegateCommand(RegisterReportCommandExecute);

            SelectedYear = DateTime.Now.Year;
            SelectedMonth = DateTime.Now.Month;

            var span = 10;
            int i = 0;
            for (i = 0; i < span; i++)
            {
                Years.Add(new ComboBoxViewModel(SelectedYear - i, (SelectedYear - i).ToString()));
            }
            for (i = 1; i <= 12; i++)
            {
                Months.Add(new ComboBoxViewModel(i,  i.ToString()));
            }
        }

        public DelegateCommand RegisterReportCommand { get; }

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

        private void RegisterReportCommandExecute()
        {
            // Menu表示
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }
    }
}
