using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TRead = GameLib.ParameterSet;

namespace GameLib
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class ParameterSetReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            //System.Diagnostics.Debugger.Launch(); // uncomment this line if you want to debug this while its building assets
            if (existingInstance == null)
                existingInstance = new TRead();
            existingInstance.AddParm("AssetName", System.IO.Path.GetFileNameWithoutExtension(input.AssetName));

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                string value = input.ReadString();
                existingInstance.AddParm(key, value);
            }

            return existingInstance;
        }
    }
}
