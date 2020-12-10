using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Mvvm;
using SensorServerApi;

namespace EventManager.Models
{
    public class SensorCollection : BindableBase
    {

        private Sensor currentSensor;
        public Sensor CurrentSensor
        {
            get { return currentSensor; }
            set { SetProperty(ref currentSensor, value); }
        }

        private SensorStatus firstSensorStatus;
        public SensorStatus FirstSensorStatus
        {
            get { return firstSensorStatus; }
            set { SetProperty(ref firstSensorStatus, value); }
        }

        private Event sensorEvents;
        public Event SensorEvents
        {
            get { return sensorEvents; }
            set { SetProperty(ref sensorEvents, value); }
        }

        public SensorCollection()
        {
            SensorEvents = new Event();
        }




    }

    //public class SensorCollection : INotifyPropertyChanged
    //{
    //    private Sensor _currentSensor;
    //    private SensorStatus _firstSensorStatus;
    //    private Event _sensorEvents;

    //    public Sensor CurrentSensor
    //    {
    //        get => _currentSensor;
    //        set
    //        {
    //            if (Equals(value, _currentSensor)) return;
    //            _currentSensor = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public SensorStatus FirstSensorStatus
    //    {
    //        get => _firstSensorStatus;
    //        set
    //        {
    //            if (Equals(value, _firstSensorStatus)) return;
    //            _firstSensorStatus = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public Event SensorEvents
    //    {
    //        get => _sensorEvents;
    //        set
    //        {
    //            if (Equals(value, _sensorEvents)) return;
    //            _sensorEvents = value;
    //            OnPropertyChanged();
    //        }
    //    }


    //    public SensorCollection()
    //    {
    //        SensorEvents = new Event();
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    [NotifyPropertyChangedInvocator]
    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //}
}
