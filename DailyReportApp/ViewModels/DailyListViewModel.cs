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
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace DailyReportApp.ViewModels
{
    public class DailyListViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<DailyReportListViewModel> _reportList = new ObservableCollection<DailyReportListViewModel>();
        private DateTime _selectedDate;
        private int _selectedReportId;

        public DelegateCommand RegisterReportCommand { get; }
        public DelegateCommand PrintReportCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ReportListDoubleClickCommand { get; }
        public DelegateCommand BeforeCommand { get; }
        public DelegateCommand AfterCommand { get; }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }
        public ObservableCollection<DailyReportListViewModel> ReportList
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
            PrintReportCommand = new DelegateCommand(PrintReportCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            ReportListDoubleClickCommand = new DelegateCommand(ReportListDoubleClickCommandExecute);
            BeforeCommand = new DelegateCommand(BeforeCommandExecute);
            AfterCommand = new DelegateCommand(AfterCommandExecute);

        }

        private void RegisterReportCommandExecute()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }
        private void PrintReportCommandExecute()
        {
            ReportPaper reportPaper = new ReportPaper();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(reportPaper);

            // A4縦
            fixedPage.Width = 8.27 * 96;
            fixedPage.Height = 11.69 * 96; 
            PageContent pc = new PageContent();
            ((IAddChild)pc).AddChild(fixedPage);
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.Pages.Add(pc);

            using (Package p = Package.Open(string.Format(@"C:\temp\{0}.xps", SelectedDate.ToString("yyyyMMdd")), FileMode.Create))
            {
                using (XpsDocument d = new XpsDocument(p))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(d);
                    writer.Write(fixedDocument.DocumentPaginator);
                }
            }

            PdfSharp.Xps.XpsConverter.Convert(string.Format(@"C:\temp\{0}.xps", SelectedDate.ToString("yyyyMMdd")), string.Format(@"C:\temp\{0}.pdf", SelectedDate.ToString("yyyyMMdd")), 0);
            File.Delete(string.Format(@"C:\temp\{0}.xps", SelectedDate.ToString("yyyyMMdd")));

        }

        private void ReportListDoubleClickCommandExecute()
        {
            var p = new NavigationParameters();
            p.Add(nameof(RegisterReportViewModel.ReportId), SelectedReportId);
            _regionManager.RequestNavigate("ContentRegion", nameof(RegisterReport), p);

        }

        private void BeforeCommandExecute()
        {
            SelectedDate = SelectedDate.AddDays(-1);
            ShowDailyReportList();

        }

        private void AfterCommandExecute()
        {
            SelectedDate = SelectedDate.AddDays(1);
            ShowDailyReportList();

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedDate = navigationContext.Parameters.GetValue<DateTime>(nameof(SelectedDate));
            ShowDailyReportList();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void ShowDailyReportList()
        {
            SqlDataReader dr;

            ReportList.Clear();

            var db = new Database();
            db.SQL = "SELECT "
                    + "   daily_report_id "
                    + "   , MIN(work_content_name) work_content_name "
                    + " FROM "
                    + "   uv_daily_reports "
                    + " WHERE "
                    + "   work_date ='" + SelectedDate.ToString("yyyy/MM/dd") + "'"
                    + " GROUP BY "
                    + "   daily_report_id "
                    + "   , work_content_id "
                    ;
            dr = db.ReadAsDataReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ReportList.Add(new DailyReportListViewModel((int)dr["daily_report_id"], dr["work_content_name"].ToString()));

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
