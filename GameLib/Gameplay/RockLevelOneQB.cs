//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using BEPUphysics;

//namespace GameLib
//{
//    class MusicTrigger
//    {
//        public bool triggered = false;
//        public BoundingBox bb;
//        public string sound;
//        public bool looped;
//        public int instanceid;

//        public MusicTrigger(BoundingBox bb, string sound, bool looped)
//        {
//            this.bb = bb;
//            this.sound = sound;
//            this.looped = looped;
//            this.instanceid = -1;
//        }
//    }

//    class RockLevelOneQB : Quarterback
//    {  
//        List<MusicTrigger> musicTriggers;

//        Actor guitarActor;
//        CharacterControllerInput characterControllerInput;
//        Actor semiActor;

//        float firstTransition = 168.0f;
//        //float secondTransition = 209.0f;
//        Vector2 endGuitar = new Vector2(205.0f, 25.0f);
//        Vector2 secondTransition = new Vector2(209.0f, 30.0f);

//        const string walkSound_preHighway = "bassloop";
//        //const string walkSound_postHighway = "bassloop";
//        //const string walkSound_final = "bassandbeat";
//        //const string[] walkSounds = { "bassdrumpLOOP", "", "bassloop", "bassandbeat" };

//        enum ControlStage
//        {
//            First,
//            Second,
//            Third,
//            Fourth,
//        }

//        ControlStage controlStage;
//        public override string Name()
//        {
//            return "RockLevelOneQB";
//        }

//        public override void PostLoadInit(ParameterSet Parm)
//        {
//            guitarActor = Stage.ActiveStage.GetQB<ActorQB>().FindActor("GuitarAvatar");
//            characterControllerInput = guitarActor.agents[0] as CharacterControllerInput;
//            semiActor = Stage.ActiveStage.GetQB<ActorQB>().FindActor("Semi");

//            musicTriggers = new List<MusicTrigger>();
//            int i = 0;
//            while (Parm.HasParm("MusicTrigger" + i.ToString()))
//            {
//                string baseString = "MusicTrigger" + i;
//                Vector3 min = Parm.GetVector3(baseString + "Min");
//                Vector3 max = Parm.GetVector3(baseString + "Max");
//                string sound = Parm.GetString(baseString);
//                bool looped = Parm.GetBool(baseString + "Looped");
//                musicTriggers.Add(new MusicTrigger(new BoundingBox(min, max), sound, looped));
//                i++;
//            }

//            characterControllerInput.SetSound("bassloop");
//        }

//        public override void Update(float dt)
//        {
//            if (CameraQB is FreeCamera)
//                return;
//            if (semiActor != null && semiActor.PhysicsObject.Position.Y < -100.0f)
//            {
//                semiActor.MarkForDeath();
//                semiActor = null;
//            }

//            foreach (MusicTrigger trigger in musicTriggers)
//            {
//                if (trigger.triggered == false && trigger.bb.Contains(guitarActor.PhysicsObject.Position) == ContainmentType.Contains)
//                {
//                    trigger.triggered = true;
//                    trigger.instanceid = Stage.ActiveStage.GetQB<AudioQB>().PlaySoundInstance(trigger.sound, trigger.looped, true);
//                    if (trigger.sound == "bassandbeat")
//                        characterControllerInput.SetSound(null);
//                }
//            }

//            switch (controlStage)
//            {
//                case ControlStage.First:
//                    if (guitarActor.PhysicsObject.CharacterController.Body.Position.X > firstTransition)
//                    {
//                        controlStage = ControlStage.Second;
//                        CameraQB = new ChaseCamera(CameraQB.Position, CameraQB.Speed, CameraQB.Pitch, CameraQB.Yaw, CameraQB.ProjectionMatrix, semiActor.PhysicsObject, new Vector3(0.0f, 2.0f, 0.0f), true, 60.0f);
//                        semiActor.PhysicsObject.LinearVelocity = new Vector3(0.0f, 0.0f, 50.0f);
//                        characterControllerInput.MoveDirection = CharacterControllerInput.Direction.Backward;
//                        characterControllerInput.track = firstTransition;
//                    }
//                    break;
//                case ControlStage.Second:
//                    {
//                        semiActor.PhysicsObject.LinearVelocity = new Vector3(0.0f, 0.0f, 50.0f);
//                        if (semiActor.PhysicsObject.Position.Z > -100.0f)
//                        {
//                            CameraQB = new CharacterChaseCamera(Vector3.Zero, 10.0f, 0.0f, 0.0f, Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1280.0f / 720.0f, .1f, 10000), guitarActor.PhysicsObject.CharacterController, new Vector3(0.0f, 3.0f, 0.0f), false, 60);
//                            //characterControllerInput.MoveDirection = CharacterControllerInput.Direction.Backward;
//                            //characterControllerInput.track = firstTransition;
//                            //CameraQB.Yaw = MathHelper.ToRadians(270.0f);
//                            controlStage = ControlStage.Third;
                            
//                        }
//                        break;
//                    }
//                case ControlStage.Third:
//                    if (CameraQB.Yaw > -1.57079649f)
//                        CameraQB.Yaw -= dt;
//                    else
//                    {
//                        if ((guitarActor.PhysicsObject.CharacterController.Body.Position.Z > endGuitar.X && guitarActor.PhysicsObject.CharacterController.Body.Position.Y > endGuitar.Y) == false && musicTriggers[4].triggered == false)
//                        {
//                            musicTriggers[4].triggered = true;
//                            musicTriggers[4].instanceid = Stage.ActiveStage.GetQB<AudioQB>().PlaySoundInstance(musicTriggers[4].sound, musicTriggers[4].looped, true);
//                            Stage.ActiveStage.GetQB<AudioQB>().StopStound(musicTriggers[3].instanceid);
//                            characterControllerInput.SetSound(null);
//                        }
//                    }
//                    semiActor.PhysicsObject.LinearVelocity = new Vector3(0.0f, 0.0f, 12.0f);
//                    if (guitarActor.PhysicsObject.CharacterController.Body.Position.Z > endGuitar.X && guitarActor.PhysicsObject.CharacterController.Body.Position.Y > endGuitar.Y)
//                    {
//                        if ( musicTriggers[4].triggered == true )
//                        {
//                            Stage.ActiveStage.GetQB<AudioQB>().StopStound(musicTriggers[4].instanceid);
//                            musicTriggers[4].triggered = false;


//                            musicTriggers[5].triggered = true;
//                            Stage.ActiveStage.GetQB<AudioQB>().PlaySoundInstance(musicTriggers[5].sound, musicTriggers[5].looped, true);
//                        }
//                    }
//                    if (guitarActor.PhysicsObject.CharacterController.Body.Position.Z > secondTransition.X && guitarActor.PhysicsObject.CharacterController.Body.Position.Y > secondTransition.Y)
//                    {
//                        controlStage = ControlStage.Fourth;
//                        characterControllerInput.MoveDirection = CharacterControllerInput.Direction.Right;
//                        characterControllerInput.track = secondTransition.X;
//                        //CameraQB.Yaw = MathHelper.ToRadians(0.0f);
//                        //characterControllerInput.SetSound(musicTriggers[5].sound);

//                        //Stage.ActiveStage.GetQB<AudioQB>().PlaySoundInstance("Beat4track", true);
//                        //Stage.ActiveStage.GetQB<AudioQB>().StopStound(musicTriggers[4].instanceid);
//                    }
//                    break;
//                case ControlStage.Fourth:
//                    if (CameraQB.Yaw < 0.0f)
//                        CameraQB.Yaw += dt;
//                    if ( semiActor != null && semiActor.PhysicsObject.Position.Z < 300.0f )
//                        semiActor.PhysicsObject.LinearVelocity = new Vector3(0.0f, 0.0f, 50.0f);
//                    break;
//            }
//        }

//        public override void Serialize(ParameterSet parm)
//        {
//            //TODO::
//            //figure out what needs to be updated here
//            int index = 0;
//            foreach (MusicTrigger mt in musicTriggers)
//            {
//                parm.AddParm("MusicTrigger" + index, mt.sound);
//                parm.AddParm("MusicTrigger" + index + "Min", mt.bb.Min.X + " " + mt.bb.Min.Y + " " + mt.bb.Min.Z);
//                parm.AddParm("MusicTrigger" + index + "Max", mt.bb.Max.X + " " + mt.bb.Max.Y + " " + mt.bb.Max.Z);
//                parm.AddParm("MusicTrigger" + index + "Looped", mt.looped.ToString());
//                index++;
//            }
//        }
//    }
//}
