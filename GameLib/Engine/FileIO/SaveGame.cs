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
            Components.Add(new GamerServicesComponent(this));
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
        public Dictionary<string, List<float>> highScores = new Dictionary<string, List<float>>();

        public void AddHighScore(string level, float score)
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
                highScores[level] = new List<float>(HIGH_SCORES_PER_LEVEL + 1);
                highScores[level].Add(score);
            }
        }

        public void UnlockLevel(string level)
        {
            if (levelsUnlocked.Contains(level) == false)
                levelsUnlocked.Add(level);
        }

        public List<float> GetHighScores(string level)
        {
            List<float> scores = new List<float>(HIGH_SCORES_PER_LEVEL);

            // add all the scores we have for this level
            if (highScores.ContainsKey(level))
                scores.AddRange(highScores[level]);

            // then fill out the rest with 0
            while (scores.Count < HIGH_SCORES_PER_LEVEL)
                scores.Add(0f);

            return scores;
        }

        public void CheckSaveGame()
        {
            if (saveGameRequested)
            {
                if (saveDevice.IsReady)
                {
                    saveDevice.SaveAsync("HeroesOfRock", "HeroesOfRockSave.txt", stream =>
                    {
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                        {
                            // write all the levels, and the scores for them
                            foreach (string level in levelsUnlocked)
                            {
                                List<float> highScores = GetHighScores(level);
                                writer.Write(level);
                                foreach (float score in highScores)
                                    writer.Write(' ' + score.ToString());
                                writer.Write(writer.NewLine);
                            }
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
                                            float score = 0.0f;
                                            if (float.TryParse(split[i], out score))
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
                                        // corrupted file, bad news, maybe they pulled the hard drive out mid save?
#if DEBUG
                                        System.Diagnostics.Debug.Assert(false, "File corrupted!!");
#endif
                                        fileOk = false;
                                    }
                                }
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
