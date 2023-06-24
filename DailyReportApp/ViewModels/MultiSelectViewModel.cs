using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportApp.ViewModels
{
    public class MultiSelectViewModel : BindableBase
    {
        private int _id;
        private string _name;
        private bool _selected;


        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public bool Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
    }
}
