using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// todo: flag for fading sound in and out over time (zones only)
//       only play sound when certain actors enter trigger? need a way to classify actors. actor tags?

namespace GameLib
{
    class PlaySoundTriggerVolume : TriggerVolume
    {
        string soundName;
        float volume = 1.0f;
        float pitch = 0.0f;
        float pan = 0.0f;
        
        private enum SoundTriggerType
        {
            Zone,
            OneShot,
        }

        SoundTriggerType soundTriggerType = SoundTriggerType.OneShot;

        int soundHandle = -1;

        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            if (worldParm.HasParm("Volume"))
                actorParm.AddParm("Volume", worldParm.GetFloat("Volume"));
            if (worldParm.HasParm("Pitch"))
                actorParm.AddParm("Pitch", worldParm.GetFloat("Pitch"));
            if (worldParm.HasParm("Pan"))
                actorParm.AddParm("Pan", worldParm.GetFloat("Pan"));
            if (worldParm.HasParm("Type"))
                actorParm.AddParm("Type", worldParm.GetString("Type"));

            System.Diagnostics.Debug.Assert(worldParm.HasParm("SoundName"), "PlaySoundTriggerVolume requires a sound name!");
            actorParm.AddParm("SoundName", worldParm.GetString("SoundName"));
        }

        public PlaySoundTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "PlaySoundTriggerVolume";            
        }

        public override void Initialize(Stage stage)
        {
            soundName = actor.Parm.GetString("SoundName");
            if (actor.Parm.HasParm("Volume"))
                volume = actor.Parm.GetFloat("Volume");
            if (actor.Parm.HasParm("Pitch"))
                pitch = actor.Parm.GetFloat("Pitch");
            if (actor.Parm.HasParm("Pan"))
                pan = actor.Parm.GetFloat("Pan");
            if (actor.Parm.HasParm("Type"))
            {
                switch (actor.Parm.GetString("Type"))
                {
                    case "Zone":
                        soundTriggerType = SoundTriggerType.Zone;
                        break;
                    case "OneShot":
                        soundTriggerType = SoundTriggerType.OneShot;
                        break;
                }
            }

            if (soundTriggerType == SoundTriggerType.Zone)
            {
                AudioQB audioQB = stage.GetQB<AudioQB>();
                soundHandle = audioQB.CreateSoundInstance(soundName);
                audioQB.ChangeSoundPan(soundHandle, pan);
                audioQB.ChangeSoundPitch(soundHandle, pitch);
                audioQB.ChangeSoundVolume(soundHandle, volume);
                audioQB.SetSoundLooped(soundHandle, true);

                UsingOnTriggerExit = true;
            }
            else
            {
                UsingOnTriggerExit = false;
            }

            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;            
            

            base.Initialize(stage); // let the trigger volume base class initialize itself
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
            AudioQB audioQB = Stage.ActiveStage.GetQB<AudioQB>();

            switch ( soundTriggerType )
            {
                case SoundTriggerType.OneShot:
                    audioQB.PlaySoundInstance(soundName, false, true, volume, pitch, pan);
                    break;
                case SoundTriggerType.Zone:
                    audioQB.PlaySound(soundHandle);
                    break;
            }
            
        }

        public override void OnTriggerExit(Actor triggeringActor)
        {
            AudioQB audioQB = Stage.ActiveStage.GetQB<AudioQB>();

            switch (soundTriggerType)
            {              
                case SoundTriggerType.Zone:
                    audioQB.StopStound(soundHandle);
                    break;
            }
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("Volume", volume);
            parm.AddParm("Pitch", pitch);
            parm.AddParm("Pan", pan);
            parm.AddParm("SoundName", soundName);
            parm.AddParm("Type", Enum.GetName(typeof(SoundTriggerType), soundTriggerType));

            base.Serialize(ref parm);
        }
    }
}
