using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    public class TriggerQB : Quarterback
    {
        
        public LinkedList<Actor> triggers = new LinkedList<Actor>();
        public List<textToScreen> ui = new List<textToScreen>();

        public delegate void DrawFunc();
        List<DrawFunc> drawList = new List<DrawFunc>();
        public void RegisterDrawFunction(DrawFunc drawFunc)
        {
            drawList.Add(drawFunc);
        }
        public void clearDrawList()
        {
            drawList.Clear();
        }

        ParameterSet Parm;
        SpriteFont font;

        public override string Name()
        {
            return "TriggerQB";
        }

        public TriggerQB()
        {
        }

        public override void PreLoadInit(ParameterSet parm)
        {
            Parm = parm;
        }

        public override void LoadContent()
        {
            font = Stage.Content.Load<SpriteFont>("DefaultFont");

            ActorQB actorQB = Stage.LoadingStage.GetQB<ActorQB>();
            Vector3 rotation = Vector3.Zero;
            int index = 0;
            while (Parm.HasParm("TriggerVolume" + index))
            {
                ParameterSet actorParm = new ParameterSet();
                string prefix = "TriggerVolume" + index;
                string triggerVolumeType = Parm.GetString(prefix);
                Parm.SetPrefix(prefix);

                actorParm.AddParm("AssetName", Parm.GetPrefix());
                actorParm.AddParm("ModelName", Parm.GetString("ModelName"));
                actorParm.AddParm("Mass", -1.0f);
                actorParm.AddParm("PhysicsType", "TriggerVolume");
                actorParm.AddParm("DontDraw", !TriggersShown);
                TriggerVolume.ParseParmSet(ref actorParm, ref Parm);

                switch (triggerVolumeType)
                {
                    // todo: add other types
                    case "PlaySoundTriggerVolume":
                        actorParm.AddParm("Agents", "PlaySoundTriggerVolume");                        
                        PlaySoundTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "SpawnActorTriggerVolume":
                        actorParm.AddParm("Agents", "SpawnActorTriggerVolume");
                        SpawnActorTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "RotateCameraTriggerVolume":
                        actorParm.AddParm("Agents", "RotateCameraTriggerVolume");
                        RotateCameraTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "HealthTriggerVolume":
                        actorParm.AddParm("Agents", "HealthTriggerVolume");
                        HealthTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "TextToScreenTriggerVolume":
                        actorParm.AddParm("Agents", "TextToScreenTriggerVolume");
                        TextToScreenTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "ButtonPushTriggerVolume":
                        actorParm.AddParm("Agents", "ButtonPushTriggerVolume");
                        ButtonPushTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "AirBurstTriggerVolume":
                        actorParm.AddParm("Agents", "AirBurstTriggerVolume");
                        AirBurstTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "TutorialStopTriggerVolume":
                        actorParm.AddParm("Agents", "TutorialStopTriggerVolume");
                        TutorialStopTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    case "EndLevelTriggerVolume":
                        actorParm.AddParm("Agents", "EndLevelTriggerVolume");
                        EndLevelTriggerVolume.ParseParmSet(ref actorParm, ref Parm);
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "Unknown trigger volume type: " + Parm.GetString(prefix + index));
                        continue;
                }

                if (actorParm != null)
                {
                    Vector3 position = Parm.GetVector3("Position");

                    Actor trigger = new Actor(actorParm, "Trigger", ref position, ref rotation, Stage.Content, Stage.LoadingStage);
                    triggers.AddLast(trigger);
                }

                Parm.ClearPrefix();

                index++;
            }   
        }

        public override void Update(float dt)
        {
#if DEBUG && WINDOWS
            if ( Stage.ActiveStage.GetQB<ControlsQB>().CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V) && Stage.ActiveStage.GetQB<ControlsQB>().LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.V) )
            {
                ShowTriggers(!TriggersShown);
            }
#endif


            LinkedListNode<Actor> node = triggers.First;
            if (node == null)
                return;
            do
            {
                if (!node.Value.IsShutdown)
                {
                    node.Value.Update(dt);
                    

                    // check if this trigger was marked for death, and kill if so
                    if (node.Value.MarkedForDeath) // should be last
                    {
                        LinkedListNode<Actor> tempNode = node;
                        node = node.Previous;
                        triggers.Remove(tempNode);
                        tempNode.Value.Kill();
                    }
                }

                if (node != null)
                    node = node.Next;

            } while (node != null);
        }

        public static bool TriggersShown = false;
        public void ShowTriggers(bool show)
        {
            if (show == TriggersShown)
                return;

            TriggersShown = show;

            foreach (Actor trigger in triggers)
            {
                if ( show )
                    trigger.modelInstance.model.Instances.Add(trigger.modelInstance);
                else
                    trigger.modelInstance.model.Instances.Remove(trigger.modelInstance);
            }
        }

        public override void DrawUI(float dt)
        {
            foreach (textToScreen value in ui)
                Stage.renderer.SpriteBatch.DrawString(font, value.text, value.position, value.color);

            foreach (DrawFunc d in drawList)
            {
                d();
            }

            base.DrawUI(dt);

        }

        public override void Serialize(ParameterSet parm)
        {
            int index = 0;
            foreach (Actor trigger in triggers)
            {
                TriggerVolume triggerVolume = trigger.agents[0] as TriggerVolume; // shouldn't be any other agents attached...
                parm.SetPrefix("TriggerVolume" + index);
                parm.AddParm("", triggerVolume.Name);
                parm.AddParm("Position", trigger.PhysicsObject.Position);
                parm.AddParm("ModelName", trigger.modelInstance.model.Name);
                triggerVolume.Serialize(ref parm); // trigger type specific parameters
                parm.ClearPrefix();
                index++;
            }
        }

        public Actor AddTrigger(ParameterSet parm, Vector3 position)
        {
            Vector3 zero = Vector3.Zero;
            Actor trigger = new Actor(parm, "Trigger", ref position, ref zero, Stage.Content, Stage.ActiveStage);
            triggers.AddLast(trigger);
            return trigger;
        }

        public void removeTrigger(Actor trig)
        {
            triggers.Remove(trig);
        }

        public void AddText(textToScreen value)
        {
            ui.Add(value);
        }
        public void RemoveText(textToScreen value)
        {
            List<textToScreen> temp = new List<textToScreen>(ui);
            foreach (textToScreen v in temp)
            {
                if (v.text == value.text)
                    ui.Remove(v);
            }
        }
    }

    public class textToScreen
    {
        public string text;
        public Vector2 position;
        public Color color;

        public textToScreen(string t, Vector2 p, Color c)
        {
            text = t;
            position = p;
            color = c;
        }
    }
}
