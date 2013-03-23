using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using GameLib;

namespace Pipeline
{
    [ContentProcessor(DisplayName = "RModelProcessor")]
    public class RModelProcessor : ModelProcessor
    {
        public enum ModelType
        {
            Normal,
            Animated,
            TriggerVolume,
        }

        [DisplayName("ModelType")]
        [Description("The custom effect applied to the model.")]
        public string CustomEffect
        {
            get { return customEffect; }
            set { customEffect = value; }
        }
        private string customEffect;

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            AnimationProcessor ap = new AnimationProcessor();
            return ap.Process(input, context);
        }
    }
}