using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EventManager.Models;
using SensorServerApi;

namespace EventManager.Helpers
{
   

    public class SensorHelper
    {
        #region Private Memebers
        private ObservableCollection<SensorCollection> SensorCollection;
        private bool useCache;
        private ISensorServer sensorServer;
        private ICacheService cacheService;
        private Rate sensorServerRate = Rate.Hardcore;
        private int dueDelay = 30;
        #endregion

        #region Public Members
        public event EventHandler<SensorCollection> DeleteSensorEvent;
        public event EventHandler<SensorCollection> UpdateSensorEvent;
        public event EventHandler<SensorCollection> AddSensorEvent;
        private static DispatcherTimer dispatcherTimer; 
        #endregion
        public SensorHelper(ObservableCollection<SensorCollection> SensorCollection, ICacheService cacheService, ISensorServer sensorServer,  bool _useCache = true, Rate sensorServerRate = Rate.Easy)
        {
            this.SensorCollection = SensorCollection;
            this.useCache = _useCache;
            this.sensorServerRate = sensorServerRate;
            this.sensorServer = sensorServer;
            this.cacheService = cacheService;

            dispatcherTimer = new DispatcherTimer();

            //sensorServer = new SensorServer(sensorServerRate);
            sensorServer.StartServer();
            //sensorServer.OnSensorStatusEvent += SensorServer_OnSensorStatusEvent;
            Observable.FromEvent<OnSensorStatus, SensorStatus>(
              h => sensorServer.OnSensorStatusEvent += SensorServer_OnSensorStatusEvent,
              h => sensorServer.OnSensorStatusEvent -= SensorServer_OnSensorStatusEvent)
                 .Subscribe(p => System.Diagnostics.Trace.WriteLine($"Progress {p}"));

            dispatcherTimer.Interval = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 1, milliseconds: 0);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            Observable.FromEvent<DispatcherTimer>(
             h => dispatcherTimer.Tick += dispatcherTimer_Tick,
             h => dispatcherTimer.Tick -= dispatcherTimer_Tick)
                .Subscribe(p => System.Diagnostics.Trace.WriteLine($"Progress {p}"));
            dispatcherTimer.Start();
        }


        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (var sensors in this.SensorCollection)
            {
                DateTime now = DateTime.Now;
                var dueSensors = sensors.SensorEvents?.
                    EventAlarms?.
                    Count(t => (now - t.TimeRecieved).Seconds >= dueDelay);
                if (dueSensors > 0)
                {
                    sensors.SensorEvents.EventAlarms.RemoveAll(t => (now - t.TimeRecieved).Seconds >= dueDelay);
                    bool update = await cacheService.UpdateEntity(sensors.CurrentSensor);
                    if (update)
                    {
                        UpdateSensorEvent?.
                            Invoke(null, sensors);
                        break;
                    }

                }


                if (sensors.SensorEvents != null &&
                    (sensors.SensorEvents.EventAlarms != null &&
                    (sensors.SensorEvents != null && sensors.SensorEvents.EventAlarms.Count == 0)))
                {
                    bool delete = await cacheService.RemoveEntity(sensors.CurrentSensor);
                    if (delete)
                    {
                        SensorCollection.Remove(sensors);
                        DeleteSensorEvent?.
                            Invoke(null, sensors);
                        break;
                    }
                }


            }
        }

        private async void SensorServer_OnSensorStatusEvent(SensorStatus sensorStatus)
        {
            var result = await sensorServer.GetSensorById(sensorStatus.SensorId);
            if (result != null)
            {
                if (sensorStatus.IsAlarmStatus)
                {
                    AddEvent(result, sensorStatus);
                }
            }

        }


        public void AddEvent(Sensor result, SensorStatus sensorStatus)
        {
            InvokerHelper.RunSave(async () =>
            {

                SensorCollection currentSensorCollection;

                bool event_exist = false;
                foreach (var sensor in SensorCollection)
                {
                    if (result.Id == sensor.CurrentSensor.Id)
                    {
                        event_exist = true;
                        break;
                    }
                }

                if (event_exist)
                {
                    currentSensorCollection = SensorCollection.FirstOrDefault(t => t.CurrentSensor.Id == result.Id);
                    if (currentSensorCollection != null)
                    {
                        bool update = await cacheService.UpdateEntity(currentSensorCollection.CurrentSensor);
                        if (update)
                        {
                            currentSensorCollection.SensorEvents.EventAlarms?.
                            Add(new EventAlarm(sensorStatus));

                            UpdateSensorEvent?.
                            Invoke(null, currentSensorCollection);

                        }
                    }
                }
                else
                {
                    currentSensorCollection = new SensorCollection()
                    {
                        CurrentSensor = result,
                        FirstSensorStatus = sensorStatus,
                        SensorEvents = new Event() { EventAlarms = new List<EventAlarm>() { new EventAlarm(sensorStatus) } }
                    };

                    bool save = await cacheService.AddEntity(currentSensorCollection.CurrentSensor);
                    if (save)
                    {
                        if (sensorStatus.StatusType == StatusType.Alarm)
                            SensorCollection.Add(currentSensorCollection);
                        else
                            SensorCollection.Insert(0, currentSensorCollection);
                        AddSensorEvent?.
                        Invoke(null, currentSensorCollection);
                    }

                }

            });
        }

        public async void DeleteEvent(Sensor sensor)
        {
            bool delete = await cacheService.RemoveEntity(sensor);
            if (delete)
            {
                SensorCollection.Where(t => t.CurrentSensor.Id == sensor.Id)
                    .ToList()?.
                    All(i => SensorCollection.Remove(i));
            }

        }

    }
}
