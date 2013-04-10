using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;

namespace GameLib
{
    public class SaveGame
    {
        IAsyncSaveDevice saveDevice;

        public SaveGame()
        {
            levelsUnlocked.Add("Tutorial");
            levelsUnlocked.Add("Level1");
        }

        public void Initialize(Game game)
        {
            // create the save device
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            game.Components.Add(sharedSaveDevice);
            saveDevice = sharedSaveDevice;

            // create event handlers that force the user to choose a new device
            // if they cancel the device selector, or it they disconnect the storage
            // device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Nothing;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Prompt;

            // prompt for a device on the first update we can
            sharedSaveDevice.PromptForDevice();

#if XBOX
            game.Components.Add(new GamerServicesComponent(game));
#endif

            saveDevice.SaveCompleted += new SaveCompletedEventHandler(saveDevice_SaveCompleted);
            saveDevice.LoadCompleted += new LoadCompletedEventHandler(saveDevice_LoadCompleted);
        }

        void saveDevice_LoadCompleted(object sender, FileActionCompletedEventArgs args)
        {
            SaveDataLoaded = true;
            System.Diagnostics.Debug.WriteLine("Load completed!");
        }

        void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Save completed!");
        }

        bool saveGameRequested = false;
        public void SaveGameData()
        {
            saveGameRequested = true;
        }

        public const int HIGH_SCORES_PER_LEVEL = 3;
        public List<string> levelsUnlocked = new List<string>();
        public Dictionary<string, List<int>> highScores = new Dictionary<string, List<int>>();

        public void AddHighScore(string level, int score)
        {
            if (highScores.ContainsKey(level))
            {
                // add score to the end of the list, and sort the list
                highScores[level].Add(score);
                highScores[level].Sort();

                // remove any extra scores from the list
                while ( highScores[level].Count > HIGH_SCORES_PER_LEVEL )
                    highScores[level].RemoveAt( highScores[level].Count - 1);
            }
            else
            {
                // create the entry in the dictionary, then insert the score
                highScores[level] = new List<int>(HIGH_SCORES_PER_LEVEL + 1);
                highScores[level].Add(score);
            }
        }

        public void UnlockLevel(string level)
        {
            if (levelsUnlocked.Contains(level) == false)
                levelsUnlocked.Add(level);
        }

        public List<int> GetHighScores(string level)
        {
            List<int> scores = new List<int>(HIGH_SCORES_PER_LEVEL);

            // add all the scores we have for this level
            if (highScores.ContainsKey(level))
                scores.AddRange(highScores[level]);

            // then fill out the rest with 0
            while (scores.Count < HIGH_SCORES_PER_LEVEL)
                scores.Add(0);

            return scores;
        }

        public int musicVol = 11;
        public int fxVol = 11;
        public bool strumMode = true;
        public bool gore = true;
        public bool moreGore = true;

        public void StoreOptionData(int musicVolume, int fxVolume, bool gorey, bool moreGorey, bool strum)
        {
            musicVol = musicVolume;
            fxVol = fxVolume;
            strumMode = strum;
            gore = gorey;
            moreGore = moreGorey;
        }

        public void getVolumes(out int musicVolume, out int fxVolume)
        {
            musicVolume = musicVol;
            fxVolume = fxVol;
        }

        public void getGores(out bool gorey, out bool moreGorey)
        {
            gorey = gore;
            moreGorey = moreGore;
        }

        public bool getStrumMode()
        {
            return strumMode;
        }

        public void CheckSaveGame()
        {
            if (saveGameRequested)
            {
                if (saveDevice.IsReady)
                {
                    saveDevice.SaveAsync("HeroesOfRock", "HeroesOfRockSave.txt", stream =>
                    {
                        try
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                            {
                                // write all the levels, and the scores for them
                                foreach (string level in levelsUnlocked)
                                {
                                    List<int> highScores = GetHighScores(level);
                                    writer.Write(level);
                                    foreach (int score in highScores)
                                        writer.Write(' ' + score.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    writer.Write(writer.NewLine);
                                }
                                //write in this order, music volume, fx volume, strumMode, gore, moreGore
                                writer.Write(musicVol); writer.Write(writer.NewLine);
                                writer.Write(fxVol); writer.Write(writer.NewLine);
                                writer.Write(strumMode); writer.Write(writer.NewLine);
                                writer.Write(gore); writer.Write(writer.NewLine);
                                writer.Write(moreGore); writer.Write(writer.NewLine);
                            }
                        }
                        catch(Exception e)
                        {
                            // something bad happened while saving
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                        }
                    });
                    saveGameRequested = false;
                }
            }
        }

        public bool SaveDataLoaded { get; private set; }

        public void LoadGameData()
        {
            if (!SaveDataLoaded)
                loadGameRequested = true;
        }
        private bool loadGameRequested = false;

        public void CheckLoadGame()
        {
            if (loadGameRequested && !SaveDataLoaded)
            {
                if (saveDevice.IsReady)
                {
                    if (saveDevice.FileExists("HeroesOfRock", "HeroesOfRockSave.txt"))
                    {
                        saveDevice.LoadAsync("HeroesOfRock", "HeroesOfRockSave.txt", stream =>
                        {
                            try
                            {
                                int otherParmIndex = 0;
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                                {
                                    bool fileOk = true;
                                    while (!reader.EndOfStream && fileOk)
                                    {
                                        string line = reader.ReadLine();
                                        string[] split = line.Split();

                                        if (split.Length > 1) // we should have at least the level name, and 1 score
                                        {
                                            string level = split[0];
                                            UnlockLevel(level);
                                            for (int i = 1; (i < HIGH_SCORES_PER_LEVEL) && (i < split.Length - 1); i++)
                                            {
                                                int score = 0;
                                                if (int.TryParse(split[i], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out score))
                                                    AddHighScore(level, score);
                                                else
                                                {
                                                    // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                    System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                    fileOk = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // get our other data if it isn't an empty line
                                            if (!(split.Length == 1 && split[0] == ""))
                                            {
                                             
                                                switch (otherParmIndex)
                                                {
                                                    case 0: //music volume
                                                        if (!int.TryParse(split[0], out musicVol))
                                                        {
                                                            // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                            System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                            fileOk = false;
                                                        }
                                                        break;
                                                    case 1: //fx volume
                                                        if (!int.TryParse(split[0], out fxVol))
                                                        {
                                                            // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                            System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                            fileOk = false;
                                                        }
                                                        break;
                                                    case 2: //strum mode
                                                        if (!bool.TryParse(split[0], out strumMode))
                                                        {
                                                            // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                            System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                            fileOk = false;
                                                        }
                                                        break;
                                                    case 3: //gore
                                                        if (!bool.TryParse(split[0], out gore))
                                                        {
                                                            // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                            System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                            fileOk = false;
                                                        }
                                                        break;
                                                    case 4: //more gore
                                                        if (!bool.TryParse(split[0], out moreGore))
                                                        {
                                                            // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                                            System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                                            fileOk = false;
                                                        }
                                                        break;
                                                }
                                                otherParmIndex++;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                // something bad happened while loading
#if DEBUG
                                System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                            }
                        });

                        loadGameRequested = false;
                    }
                    else
                    {
                        SaveDataLoaded = true;
                    }
                }
            }
        }
    }
}
