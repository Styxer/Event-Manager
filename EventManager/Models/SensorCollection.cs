using Prism.Mvvm;
using SensorServerApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Event sensorEvent;
        public Event SensorEvent
        {
            get { return sensorEvent; }
            set { SetProperty(ref sensorEvent, value); }
        }

        public SensorCollection()
        {
            SensorEvent = new Event();
        }




    }
}
