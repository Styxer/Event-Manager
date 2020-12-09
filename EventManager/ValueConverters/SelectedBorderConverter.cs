using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EventManager.Models;
using SensorServerApi;

namespace EventManager.ValueConverters
{
    public class SelectedBorderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sensorCollection = values[0] as SensorCollection;
            var sensor = values[1] as Sensor;
            if (sensorCollection != null && sensor != null)
            {
                if (sensorCollection.CurrentSensor.Id == sensor.Id)
                    return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }
    }
}
