using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyReportApp.ViewModels
{
    public class DailyReportListViewModel : BindableBase, INavigationAware
    {
        private DateOnly _selectedDate;

        public DateOnly SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        public DailyReportListViewModel()
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedDate = navigationContext.Parameters.GetValue<DateOnly>(nameof(SelectedDate));
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
