using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportApp.ViewModels
{
    public sealed class DailyReportListViewModel
    {
        public DailyReportListViewModel(int Id, string Contents)
        {
            ReportId = Id;
            WorkContents = Contents;

        }

        public int ReportId { get; }
        public string WorkContents { get; }
    }
}

