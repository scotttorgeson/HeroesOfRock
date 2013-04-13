//#define JAKESCOMP
#define Lerping
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace GameLib
{
    public class AudioQB : Quarterback
    {
        Dictionary<string,SoundEffect> soundsLib = new Dictionary<string,SoundEffect>();
        private ParameterSet Parm;

        private Dictionary<int, SoundEffectInstance> soundInstances = new Dictionary<int, SoundEffectInstance>();
        private int soundEffectId = 0;

        private List<int> killWhenDoneList = new List<int>();

        public AudioListener AudioListener { get; private set; }

        private SoundEffectInstance levelTheme;
        private string levelThemeString;
        private SoundEffectInstance cheeringSFX;
        private SoundEffectInstance booingSFX;

        public static float maxMusicVolume;
        public static float maxSFXVolume;
        private float pitchFluctuation;
        private float pitchFluctuationRate;
        private float pitchFluctuationAmplitude;
        private int prevRockLevel;

        /*public float MaxMusicVolume
        {
            get { return maxMusicVolume; }
            set { maxMusicVolume = value; }
        }

        public float MaxSFXVolume
        {
            get { return maxSFXVolume; }
            set { maxSFXVolume = value; }
        }*/

        private GameLib.Engine.AttackSystem.RockMeter rm; //for changing the sound levels

        public override string Name()
        {
            return "AudioQB";
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            this.Parm = Parm;

        }

        public override void LoadContent()
        {
            // look for sounds to load in
            // Sound0=soundName
            // Sound1=otherSoundname
            // etc...
            // this allows us to precache them in the content manager so they are ready for use when we want to play them

#if !JAKESCOMP
            int index = 0;
            string key;
            while (Parm.HasParm("Sound" + index))
            {
                key = Parm.GetString("Sound" + index);
                if (!soundsLib.ContainsKey(key))
                    soundsLib.Add(key, Stage.Content.Load<SoundEffect>("Audio/" + Parm.GetString("Sound" + index)));
                else
                    soundsLib[key] = Stage.Content.Load<SoundEffect>("Audio/" + Parm.GetString("Sound" + index));
                index++;
            }
            maxMusicVolume = 1.0f;
            if (GlobalGameParms.initialized)
            {
                if (GlobalGameParms.GameParms.HasParm("SoundMaxMusicVol"))
                    maxMusicVolume = GlobalGameParms.GameParms.GetFloat("SoundMaxMusicVol");
            }

            if (Parm.HasParm("SoundTheme"))
            {
                levelThemeString = Parm.GetString("SoundTheme");
                levelTheme = Stage.Content.Load<SoundEffect>("Audio/" + levelThemeString).CreateInstance();
                levelTheme.IsLooped = true;
                levelTheme.Volume = 0.1f;
            }

            cheeringSFX = Stage.Content.Load<SoundEffect>("Audio/cheerHeavyLoop").CreateInstance();
            booingSFX = Stage.Content.Load<SoundEffect>("Audio/booingLoop").CreateInstance();
#endif
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            if(PlayerAgent.Player != null)
                rm = PlayerAgent.Player.GetAgent<GameLib.Engine.AttackSystem.RockMeter>();
            
            if (GlobalGameParms.initialized)
            {
                if(GlobalGameParms.GameParms.HasParm("SoundFluctuationRate"))
                    pitchFluctuationRate = 6.28f * GlobalGameParms.GameParms.GetFloat("SoundFluctuationRate");
                if (GlobalGameParms.GameParms.HasParm("SoundFluctuationAmp"))
                    pitchFluctuationAmplitude = GlobalGameParms.GameParms.GetFloat("SoundFluctuationAmp");
            }
            int musicVol;
            int sfxVol;

            musicVol = 0;
            sfxVol = 0;
            if(!Stage.Editor)
                Stage.SaveGame.getVolumes(out musicVol, out sfxVol);
            
            maxMusicVolume = (float)musicVol / 11;
            maxSFXVolume = (float)sfxVol / 11;

            if (levelTheme != null)
                levelTheme.Volume = maxMusicVolume;
            if (cheeringSFX != null)
                cheeringSFX.Volume = maxSFXVolume;
            if (booingSFX != null)
                booingSFX.Volume = maxSFXVolume;

            base.PostLoadInit(Parm);
        }

        public void ChangeTheme(string themeName)
        {
#if !JAKESCOMP
            levelTheme = Stage.Content.Load<SoundEffect>("Audio/" + themeName).CreateInstance();
            levelTheme.IsLooped = true;
            levelTheme.Volume = 0.1f;

            int musicVol;
            int sfxVol;

            musicVol = 0;
            sfxVol = 0;
            if (!Stage.Editor)
                Stage.SaveGame.getVolumes(out musicVol, out sfxVol);

            maxMusicVolume = (float)musicVol / 11;
            maxSFXVolume = (float)sfxVol / 11;

            if (levelTheme != null)
            {
                levelTheme.Volume = maxMusicVolume;
                PlayTheme(1.0f);
            }
#endif
        }

        public override void  LevelLoaded()
        {
            if (levelTheme != null)
            {
                levelTheme.Play();

                if (rm != null)
                    FluctuateOnRockLevel(1.0f);
                else
                    PlayTheme(1f);
            }

            base.LevelLoaded();
        }

        public override void PauseQB()
        {
            //to-do pause playing sfx
            this.PauseTheme();
            base.PauseQB();
        }

        public override void UnPauseQB()
        {
            //to-do resume paused sfx
            this.ResumeTheme();
            base.UnPauseQB();
        }

        public override void Update(float dt)
        {
            //if (IsPaused) return;

#if !JAKESCOMP
            if (levelTheme != null) {
                if (rm != null)
                    FluctuateOnRockLevel(dt);
                else
                    PlayTheme(dt);
            }
#endif
            // look for any sounds in the kill when done list that are done, and kill them
            for (int i = killWhenDoneList.Count - 1; i >= 0; --i)
            {
                int id = killWhenDoneList[i];
                if (soundInstances[id].State == SoundState.Stopped)
                {
                    KillSound(id); // this will remove it from the kill when done list too
                }
            }
        }

        public void AddSound(string name)
        {
#if !JAKESCOMP
            if (!soundsLib.ContainsKey(name))
                soundsLib.Add(name, Stage.Content.Load<SoundEffect>("Audio/" + name));
            else
                soundsLib[name] = Stage.Content.Load<SoundEffect>("Audio/" + name);
#endif
        }

        private void FluctuateOnRockLevel(float dt)
        {
#if !Lerping
            if (rm.RockLevel == prevRockLevel && rm.RockLevel >= 3) return;
#endif
            float pitch = 0;
            float volume;
            bool booing = false;
            bool cheering = false;
            float cheerVolume = 0;
            float boolVol = 0;

            if (rm.RockLevel < 3) //1 - 2 - Pitch distortion and boos
            {
                pitchFluctuation += pitchFluctuationRate * dt;
                pitch = pitchFluctuationAmplitude*(float)Math.Sin(pitchFluctuation);
                volume = maxMusicVolume * .2f;

                booing = true;
                boolVol = .3f * maxSFXVolume;
            }
            else if (rm.RockLevel < 6) //3 - 5 - Volume lowered
                volume = maxMusicVolume * .4f;
            else if(rm.RockLevel < 10) //6 - 9 - Standard music and volume
                volume = maxMusicVolume * .5f;
            else if(rm.RockLevel < 11) //10 - Higher volume, light cheering
            {
                volume = maxMusicVolume * .8f;

                //play light cheering
                cheering = true;
                cheerVolume = .25f * maxSFXVolume;
            }
            else //11 - FULL VOLUME, EXTREME cheering
            {
                volume = maxMusicVolume;

                //play extreme cheering
                cheering = true;
                cheerVolume = .75f * maxSFXVolume;
            }
#if Lerping
            float currVol = levelTheme.Volume;
            //float currPitch = levelTheme.Pitch;
            float lerpRate = .5f;
            Lerp(volume, ref currVol, lerpRate * dt);
            //Lerp(pitch, ref currPitch, lerpRate * dt);
            levelTheme.Volume = currVol;
            levelTheme.Pitch = pitch;

            currVol = cheeringSFX.Volume;
            Lerp(cheerVolume, ref currVol, lerpRate * dt);
            cheeringSFX.Volume = currVol;


            if (cheering)
            {
                cheeringSFX.Play();
            }
            else
            {
                if (cheeringSFX.Volume == 0)
                    cheeringSFX.Stop();
            }

            currVol = booingSFX.Volume;
            Lerp(boolVol, ref currVol, lerpRate * dt);
            booingSFX.Volume = currVol;

            if (booing)
            {
                booingSFX.Play();
            }
            else
            {
                if (booingSFX.Volume == 0)
                    booingSFX.Stop();
            }
            
              
#else
            levelTheme.Volume = volume;
            levelTheme.Pitch = pitch;
            if (booing)
            {
                booingSFX.Volume = boolVol;
                booingSFX.Play();
            }
            else
                booingSFX.Stop();

            if (cheering)
            {
                cheeringSFX.Volume = cheerVolume;
                cheeringSFX.Play();
            }
            else
                cheeringSFX.Stop();
            
#endif

            

            prevRockLevel = rm.RockLevel;
        
        }

        private void PlayTheme (float dt) {

            float volume = maxMusicVolume;
#if Lerping
            float currVol = levelTheme.Volume;
            float lerpRate = .5f;
            Lerp(volume, ref currVol, lerpRate * dt);
            levelTheme.Volume = currVol;
#else
            levelTheme.Volume = volume;
            levelTheme.Pitch = pitch;
            
#endif
        }

        public void PlaySound(string soundName)
        {
#if !JAKESCOMP
            LoadSound(soundName);
            soundsLib[soundName].Play(maxSFXVolume, 0, 0);
#endif
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="soundName">Name of the sound.</param>
        /// <param name="volume">Ranges from 0.0 to 1.0. Based off master volume</param>
        /// <param name="pitch">-1.0 to 1.0. -1 is down an octave, 1 is up an octave. 0 is normal</param>
        /// <param name="pan">-1 to 1. -1 is full left, 0 centered, 1 full right</param>
        public void PlaySound(string soundName, float volume, float pitch, float pan)
        {
#if !JAKESCOMP
            LoadSound(soundName);
            soundsLib[soundName].Play(volume*maxSFXVolume, pitch, pan);
#endif
        }

        /// <summary>
        /// Creates a sound instance with the specified name, and returns a handle to it.
        /// </summary>
        /// <param name="soundName"></param>
        /// <returns></returns>
        public int CreateSoundInstance(string soundName)
        {
#if !JAKESCOMP
            LoadSound(soundName);
            SoundEffectInstance instance = soundsLib[soundName].CreateInstance();
            int id = soundEffectId++;
            soundInstances.Add(id, instance);
            return id;
#else
            return 0;
#endif
        }

        public int PlaySoundInstance(string soundName, bool isLooped, bool killWhenDone)
        {
#if !JAKESCOMP
            LoadSound(soundName);
            SoundEffectInstance instance = soundsLib[soundName].CreateInstance();
            instance.IsLooped = isLooped;
            int id = soundEffectId++;
            soundInstances.Add(id, instance);
            instance.Play();

            if (killWhenDone)
            {
                killWhenDoneList.Add(id);
            }

            return id;
#else
            return 0;
#endif
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="soundName">Name of the sound.</param>
        /// <param name="isLooped">true for looped false for one shot</param>
        /// <param name="volume">Ranges from 0.0 to 1.0. Based off master volume</param>
        /// <param name="pitch">-1.0 to 1.0. -1 is down an octave, 1 is up an octave. 0 is normal</param>
        /// <param name="pan">-1 to 1. -1 is full left, 0 centered, 1 full right</param>
        public int PlaySoundInstance(string soundName, bool isLooped, bool killWhenDone, float volume, float pitch, float pan)
        {
#if !JAKESCOMP
            LoadSound(soundName);
            SoundEffectInstance instance = soundsLib[soundName].CreateInstance();
            instance.IsLooped = isLooped;
            instance.Volume = volume * maxSFXVolume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            int id = soundEffectId++;
            soundInstances.Add(id, instance);
            instance.Play();

            if (killWhenDone)
            {
                killWhenDoneList.Add(id);
            }
            return id;
#else
            return 0;
#endif

        }

        public void ChangeSoundVolume(int id, float volume)
        {
#if !JAKESCOMP
            soundInstances[id].Volume = volume * maxSFXVolume;
#endif
        }
        public void ChangeSoundPitch(int id, float pitch)
        {
#if !JAKESCOMP
            soundInstances[id].Pitch = pitch;
#endif
        }
        public void ChangeSoundPan(int id, float pan)
        {
#if !JAKESCOMP
            soundInstances[id].Pan = pan;
#endif
        }

        public SoundState GetSoundState(int id)
        {
#if !JAKESCOMP
            return soundInstances[id].State;
#else
            return new SoundState();
#endif
        }

        public void KillSound(int id)
        {
#if !JAKESCOMP
            soundInstances[id].Stop();
            soundInstances.Remove(id);
            killWhenDoneList.Remove(id);
#endif
        }

        public void KillAllSounds()
        {
            foreach (KeyValuePair<int,SoundEffectInstance> soundInstance in soundInstances)
            {
                soundInstance.Value.Stop();
            }

            soundInstances.Clear();
            killWhenDoneList.Clear();

#if !JAKESCOMP
            if (levelTheme != null)
                levelTheme.Stop();
            booingSFX.Stop();
            cheeringSFX.Stop();
            levelTheme = null;
#endif
        }

        public override void KillInstance()
        {
            KillAllSounds();
        }

        public void PlaySound(int id)
        {
#if !JAKESCOMP
            soundInstances[id].Play();
#endif
        }

        public void ResumeSound(int id)
        {
#if !JAKESCOMP
            soundInstances[id].Resume();
#endif
        }

        public void StopStound(int id)
        {
#if !JAKESCOMP
            soundInstances[id].Stop();
#endif
        }

        public void PauseSound(int id)
        {
#if !JAKESCOMP
            soundInstances[id].Pause();
#endif
        }

        public void ResumeTheme()
        {
            if(soundInstances.ContainsKey(-1))
            {
                levelTheme = soundInstances[-1];
                soundInstances.Remove(-1);
                levelTheme.Play();
            }
        }

        public void PauseTheme()
        {
            if (levelTheme != null)
            {
                levelTheme.Pause();
                booingSFX.Pause();
                cheeringSFX.Pause();
                soundInstances.Add(-1, levelTheme);
                levelTheme = null;
            }
        }

        public void SetSoundLooped(int id, bool isLooped)
        {
#if !JAKESCOMP
            soundInstances[id].IsLooped = isLooped;
#endif
        }

        private void Lerp(float goal, ref float curr, float lerpRate)
        {
            if(goal < curr)
            {
                curr -= lerpRate;
                if (curr < goal)
                    curr = goal;
            }
            else
            {
                curr += lerpRate;
                if (curr > goal)
                    curr = goal;
            }
        }

        /*  Create wrapper class for sound effect instance's
         *  that combine them with AudioEmitters
         *  Allow people to set a sounds position
         *  or tie a sound to an actor
         *  sound will follow that actor around as they move
         */
        public void Apply3D(int id, Vector3 position, Vector3 forward)
        {
        }

        public override void Serialize(ParameterSet parm)
        {
            //TODO::
            //figure out what needs to be updated here

            if (levelTheme != null)
                parm.AddParm("SoundTheme", levelThemeString);

            int index = 0;
            foreach( string key in soundsLib.Keys)
            {
                parm.AddParm("Sound" + index++, key);
            }
        }

        public void LoadSound(string soundName)
        {
            if (!soundsLib.ContainsKey(soundName))
            {
                soundsLib[soundName] = Stage.Content.Load<SoundEffect>("Audio/" + soundName);
            }
        }
    }
}
