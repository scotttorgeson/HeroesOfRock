using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text;

namespace GameLib
{
    class TextToScreenTriggerVolume : TriggerVolume
    {
        Vector2 textPosition;
        string text;
        Color color;

        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            if (worldParm.HasParm("TextPosition"))
                actorParm.AddParm("TextPosition", worldParm.GetVector2("TextPosition"));

            if (worldParm.HasParm("Text"))
                actorParm.AddParm("Text", worldParm.GetString("Text"));
        }

        public TextToScreenTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "TextToScreenTriggerVolume";            
        }

        public override void Initialize(Stage stage)
        {
           
            UsingOnTriggerEnter = true;
            UsingOnTriggerExit = true;
            UsingOnTriggerStay = false;
            color = Color.White;

            if (actor.Parm.HasParm("TextPosition"))
                textPosition = actor.Parm.GetVector2("TextPosition");

            if (actor.Parm.HasParm("Text"))
                text = actor.Parm.GetString("Text");

            base.Initialize(stage); // let the trigger volume base class initialize itself
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
                textToScreen p = new textToScreen(text, textPosition, color);
                Stage.ActiveStage.GetQB<TriggerQB>().AddText(p);
        }

        public override void OnTriggerExit(Actor triggeringActor)
        {
                textToScreen p = new textToScreen(text, textPosition, color);
                Stage.ActiveStage.GetQB<TriggerQB>().RemoveText(p);
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("Text",text);
            parm.AddParm("TextPosition", textPosition);
            base.Serialize(ref parm);
        }
    }
}
