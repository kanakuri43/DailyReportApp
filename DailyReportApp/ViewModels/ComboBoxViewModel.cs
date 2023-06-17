using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportApp.ViewModels
{
    public sealed class ComboBoxViewModel
    {
        public ComboBoxViewModel(int Code, String Name)
        {
            SelectedValue = Code;
            DisplayMember = Name;

        }

        public int SelectedValue { get; }
        public string DisplayMember { get; }
    }
}
