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

            containerRegistry.GetContainer().Register<ICacheService, CacheService>(reuse: Reuse.Singleton);
            //  containerRegistry.GetContainer().Register<IIdEntity, IdEntity>();
            //containerRegistry.GetContainer().Register<ISensorServer, SensorServer >(reuse: Reuse.Singleton);

            //containerRegistry.GetContainer().Register<ISensorServer, SensorServer>(
            //            Made.Of(() => new SensorServer(Rate.Hardcore)),
            //            Reuse.Singleton);

            containerRegistry.GetContainer().Register<ISensorServer, SensorServer>(
                        made: Parameters.Of.Type(typeof(Rate), ifUnresolved: IfUnresolved.ReturnDefault),
                        reuse: Reuse.Singleton);
                        

            // containerRegistry.RegisterInstance<Register>("ab).As<ISensorServer>();

            //Container.Resolve<Func<Rate, ISensorServer>>();


        }
    }
}
