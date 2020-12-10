using DryIoc;
using EventManager.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using SensorServerApi;
using System;
using System.Windows;

namespace EventManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<SensorDetailsWindow>();
            containerRegistry.Register<SensorWindow>();
            containerRegistry.Register<EventAlarm>();
            containerRegistry.Register<Event>();
            containerRegistry.Register<Sensor>();

            containerRegistry.GetContainer().Register<ICacheService, CacheService>(reuse: Reuse.Singleton);


            containerRegistry.GetContainer().Register<ISensorServer, SensorServer>(
                        made: Parameters.Of.Type(typeof(Rate), ifUnresolved: IfUnresolved.ReturnDefault),
                        reuse: Reuse.Singleton);


            
           
          




        }
    }
}
