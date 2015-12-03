using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvokeAsync<T>(this EventHandler<T> evt, object sender, T e) where T : EventArgs
        {
            evt?.BeginInvoke(sender, e, null, null);
        }
    }
}
