using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace EventManager.Helpers
{
    public  class InvokerHelper
    {
        public static void RunSave(Action action)
        {
            if (Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { action?.Invoke(); }));
        }
    }
}
