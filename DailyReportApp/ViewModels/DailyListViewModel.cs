using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyReportApp.ViewModels
{
    public class DailyListViewModel : BindableBase, INavigationAware
    {
        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        public DailyListViewModel()
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedDate = navigationContext.Parameters.GetValue<DateTime>(nameof(SelectedDate));
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
