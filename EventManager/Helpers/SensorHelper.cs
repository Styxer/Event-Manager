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
        #region Ctor
        public SensorHelper(ObservableCollection<SensorCollection> SensorCollection
            , ICacheService cacheService, ISensorServer sensorServer, bool _useCache = true, Rate sensorServerRate = Rate.Easy)
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
        #endregion


        #region Private function
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

        private bool ApplayLogicFromIsAlarming(SensorType sensorType)
        {
            bool result = true;

            if(sensorType == SensorType.Video || sensorType == SensorType.Fence || sensorType == SensorType.AccessControl 
                || sensorType == SensorType.FireDetection || sensorType == SensorType.Radar)
            {
                result = false;
            }

            return result;
        }

        private bool IsSpecialSensorAlarming(SensorType sensorType, StatusType statusType)
        {
            bool result = false;

            if (sensorType == SensorType.Video)
            {
                if (statusType == StatusType.Alarm || statusType == StatusType.Disconnected)
                    result = true;
            }
            else if(sensorType == SensorType.Fence)
            {
                if (statusType == StatusType.Alarm || statusType == StatusType.Disconnected || statusType == StatusType.Off)
                    result = true;
            }
            else if (sensorType == SensorType.AccessControl)
            {
                if (statusType == StatusType.Alarm || statusType == StatusType.Disconnected || statusType == StatusType.On)
                    result = true;
            }
            else if (sensorType == SensorType.FireDetection)
            {
                if (statusType != StatusType.Default)
                    result = true;
            }
            else if (sensorType == SensorType.Radar)
            {
                if (statusType == StatusType.Alarm  || statusType == StatusType.Off)
                    result = true;
            }

            return result;
        }

        private bool IsSensorAlarming(Sensor sensor, SensorStatus sensorStatus)
        {
            bool result = false;

            bool readFromAlarming =  ApplayLogicFromIsAlarming(sensor.SensorType);

            if (readFromAlarming)
                result = sensorStatus.IsAlarmStatus;

            else
                result = IsSpecialSensorAlarming(sensor.SensorType, sensorStatus.StatusType);

            return result;
        }


        #endregion


        #region Public function
        public void AddEvent(Sensor result, SensorStatus sensorStatus)
        {
            InvokerHelper.RunSave(async () =>
            {

                SensorCollection currentSensorCollection;

                bool event_exist = false;
                //foreach (var sensor in SensorCollection)
                //{
                //    if (result.Id == sensor.CurrentSensor.Id)
                //    {
                //        event_exist = true;
                //        break;
                //    }
                //}
                Parallel.ForEach(SensorCollection, (sensor, state) =>
                {
                    if (result.Id == sensor.CurrentSensor.Id)
                    {
                        event_exist = true;
                        state.Stop();
                    }
                });

                if (IsSensorAlarming(result, sensorStatus))
                {
                    if (event_exist)
                    {
                        currentSensorCollection = SensorCollection.FirstOrDefault(t => t.CurrentSensor.Id == result.Id);
                        if (currentSensorCollection != null)
                        {
                            bool canUpdate = await cacheService.UpdateEntity(currentSensorCollection.CurrentSensor);
                            if (canUpdate)
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

                        bool canSave = await cacheService.AddEntity(currentSensorCollection.CurrentSensor);
                        if (canSave)
                        {

                            if (sensorStatus.StatusType == StatusType.Alarm)
                                SensorCollection.Add(currentSensorCollection);
                            else
                                SensorCollection.Insert(0, currentSensorCollection);
                            AddSensorEvent?.
                            Invoke(null, currentSensorCollection);
                        }

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
        #endregion

    }
}
