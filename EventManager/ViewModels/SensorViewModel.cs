using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.ViewModels
{
    public class SensorViewModel : BindableBase
    {
        private string fieldName;
        public string PropertyName
        {
            get { return fieldName; }
            set { SetProperty(ref fieldName, value); }
        }

        public SensorViewModel()
        {

        }
    }
}
