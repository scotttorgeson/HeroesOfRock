using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class ThreadSafeContentManager : ContentManager
    {
        static object locker = new object();

        public ThreadSafeContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public ThreadSafeContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        public override T Load<T>(string assetName)
        {
            lock (locker)
            {
                return base.Load<T>(assetName);
            }
        }
    }
}
