using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using EventManager.Helpers;
using EventManager.Models;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Prism.Mvvm;
using SensorServerApi;

namespace EventManager.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {



        #region Binadable Properties
        private ObservableCollection<SensorCollection> sensorsCollection;
        public ObservableCollection<SensorCollection> SensorCollection
        {
            get { return sensorsCollection; }
            set { SetProperty(ref sensorsCollection, value); }
        }

        private SensorCollection selectedSensor;
        public SensorCollection SelectedSensor
        {
            get { return selectedSensor; }
            set { SetProperty(ref selectedSensor, value); }
        }

        private ObservableCollection<SensorStatus> SelectedSensorStatuses;
        public ObservableCollection<SensorStatus> selectedSensorStatuses
        {
            get { return SelectedSensorStatuses; }
            set { SetProperty(ref SelectedSensorStatuses, value); }
        }

        private SensorStatus selectedSensorStatus;
        public SensorStatus SelectedSensorStatus
        {
            get { return selectedSensorStatus; }
            set { SetProperty(ref selectedSensorStatus, value); }
        }

        #endregion

        #region Private properties
        private SensorHelper sensorHelper;
        #endregion

        #region Commands
        public DelegateCommand<object> SelectSensorCommand { get; private set; }
        public DelegateCommand<object> RemoveSensorCommand { get; private set; }
        #endregion

        #region Servoces

        #endregion


        #region Ctor

        public MainWindowViewModel(ICacheService cacheService, ISensorServer sensorServer)
        {


            SensorCollection = new ObservableCollection<SensorCollection>();

            SelectSensorCommand = new DelegateCommand<object>(SelectSensorExecute);
            RemoveSensorCommand = new DelegateCommand<object>(RemoveSensorExecute);


            sensorHelper = new SensorHelper(SensorCollection, cacheService, sensorServer);

            sensorHelper.DeleteSensorEvent += SensorHelper_DeleteSensorEvent;



        }

        #endregion

        #region Private Functions
        private void SensorHelper_DeleteSensorEvent(object sender, SensorCollection e)
        {          
            DeleteSensor(e.CurrentSensor);
        }

        private void DeleteSensor(Sensor sensor)
        {
            if (SelectedSensor != null
              && SelectedSensor.CurrentSensor.Id == sensor.Id)
            {
                SelectedSensor = null;
            }
            var sensorToDelete = sensorsCollection.Where(x => x.CurrentSensor == sensor).FirstOrDefault();
            if (sensorToDelete != null)
            {
                sensorsCollection.Remove(sensorToDelete);

    }
        }

        private void RemoveSensorExecute(object obj)
        {
            Sensor selected = obj as Sensor;
            DeleteSensor(selected);
            //if (selected != null)
            //{
            //    sensorHelper.DeleteEvent(selected);
            //    if (SelectedSensor != null
            //        && SelectedSensor.CurrentSensor.Id == selected.Id) 
            //        SelectedSensor = null;
            //}
        }

        private void SelectSensorExecute(object obj)
        {
            Sensor selected = obj as Sensor;
            if (selected != null)
            {
                foreach (var sensor in SensorCollection)
                {
                    if (sensor.CurrentSensor.Id == selected.Id)
                    {
                        SelectedSensor = sensor;
                        break;
                    }
                }
            }
        }
        #endregion

    }

  
}
