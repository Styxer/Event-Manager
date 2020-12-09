using EventManager.Models;
using Prism.Commands;
using Prism.Mvvm;
using SensorServerApi;
using System;
using System.Collections.ObjectModel;

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

        #endregion

        #region Commands
        public DelegateCommand<object> SelectSensorCommand { get; private set; }
        public DelegateCommand<object> RemoveSensorCommand { get; private set; }
        #endregion


        #region Ctor

        public MainWindowViewModel()
        {
           

            SensorCollection = new ObservableCollection<SensorCollection>();

            SelectSensorCommand = new DelegateCommand<object>(SelectSensorExecute);
            RemoveSensorCommand = new DelegateCommand<object>(RemoveSensorExecute);



           
        }

        private void RemoveSensorExecute(object obj)
        {
            Sensor selected = obj as Sensor;
            if (selected != null)
            {
               //TODO
            }
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
