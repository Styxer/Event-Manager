using SensorServerApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EventManager.ValueConverters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusType && value != null)
            {
                var status = (StatusType)value;
                var color = new SolidColorBrush(Colors.Red);
                if(status == StatusType.On || status == StatusType.Connected || status == StatusType.Default)
                {
                    color = new SolidColorBrush(Colors.LightGreen);
                    
                }

                return color;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
