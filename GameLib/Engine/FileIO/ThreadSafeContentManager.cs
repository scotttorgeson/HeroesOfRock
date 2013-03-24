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
#if DEBUG
            if (Stage.ActiveStage != null)
                System.Diagnostics.Debug.WriteLine(Stage.ActiveStage.Time + " Loading: " + assetName);
            else if ( Stage.LoadingStage != null )
                System.Diagnostics.Debug.WriteLine(Stage.LoadingStage.Time + " Loading: " + assetName);
#endif

            lock (locker)
            {
                return base.Load<T>(assetName);
            }
        }
    }
}
