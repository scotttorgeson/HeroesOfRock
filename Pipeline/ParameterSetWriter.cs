using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = GameLib.ParameterSet;

namespace Pipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class ParameterSetWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            //System.Diagnostics.Debugger.Launch();
            output.Write(value.GetCount());
            foreach (KeyValuePair<string, string> pair in value)
            {
                output.Write(pair.Key);
                output.Write(pair.Value);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GameLib.ParameterSetReader, GameLib";
        }
    }
}
