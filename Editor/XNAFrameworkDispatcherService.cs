using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Editor
{
    class XNAFrameworkDispatcherService
    {
        private System.Threading.Timer timer;
        

        public XNAFrameworkDispatcherService()
        {
            timer = new Timer(Callback, null, 1000, 1000);
            FrameworkDispatcher.Update();
        }

        public void Callback(object state)
        {
            FrameworkDispatcher.Update();
        }

        public void Kill()
        {
            timer.Dispose();
        }
    }
}
