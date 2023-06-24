using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportApp.ViewModels
{
    public sealed class MonthlyReportList
    {
        public MonthlyReportList(DateTime Date, int Weather)
        {
            ReportDate = Date;
            ReportWeather = Weather;

        }

        public DateTime ReportDate { get; }
        public int ReportWeather { get; }
    }
}
