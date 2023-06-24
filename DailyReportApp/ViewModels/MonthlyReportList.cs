using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportApp.ViewModels
{
    public sealed class MonthlyReportList
    {
        public MonthlyReportList(DateTime Date, string Contents)
        {
            WorkDate = Date;
            WorkContents = Contents;

        }

        public DateTime WorkDate { get; }
        public string WorkContents { get; }
    }
}
