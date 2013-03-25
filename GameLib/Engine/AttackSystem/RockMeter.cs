/*
 *RockMeter.cs
 *Rock meter is a a combined multiplier and health system for the main character
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Engine.AttackSystem
{
    class RockMeter : Agent
    {
        public int Score
        {
            get
            {
                return score;
            }
        }
        private int score;
        private int goalScore;

        public int RockLevel
        {
            get { return rockLevel; }
        }
        private int rockLevel;
        private int killCount;
        public int KillCount
        {
            get { return killCount; }
        }
        private float rockLevelf;
        private int lastRockLevel;
        
        private bool rockLevelLocked;
        public bool RockLevelLocked
        {
            get { return rockLevelLocked; }
            set 
            { 
                rockLevelLocked = value;
                timeSinceLastMove = 0.0f;
                prevValues.Store(score, rockLevel, highestRockLevel, killStreak);
            }
        }

        

        private PreviousValues prevValues;

        public int HighestRockLevel
        {
            get { return highestRockLevel; }
        }
        private int highestRockLevel;

        public int NumTimesHit
        {
            get { return numTimesHit; }
        }
        private int numTimesHit;

        private int killStreak;
        public int KillStreak
        {
            get { return killStreak; }
        }

        public int HighestKillStreak
        {
            get { return highestKillStreak; }
        }
        private int highestKillStreak;

        public int NumDeaths
        {
            get { return numDeaths; }
        }
        private int numDeaths;

        private float timeSinceLastMove;

        private float RockLevelCooldownTime;

        private const int LowestRockLevelDueToAPS = 1;

        private int startingRockLevel = 4;

        private List<RockTier> tiers;
        private int tierIndex;

        private bool dontKillActor = false; // for the loading level

        public RockMeter(Actor actor) : base(actor)
        {
            actor.RegisterUpdateFunction(Update);
            Name = "RockMeter";
            rockLevelLocked = true;
            prevValues = new PreviousValues(1); //dummy value
        }

        public override void Initialize(Stage stage)
        {
            if (stage.Parm.HasParm("RockMeterKillActor"))
                dontKillActor = !stage.Parm.GetBool("RockMeterKillActor");

            if (actor.Parm.HasParm("StartingRockLevel"))
                startingRockLevel = actor.Parm.GetInt("StartingRockLevel");

            //get params for the rock level decrease speed here
            int count = 0;
            tiers = new List<RockTier>();
            if (GlobalGameParms.initialized)
            {
                while (GlobalGameParms.GameParms.HasParm("RockLevelTier" + count))
                {
                    tiers.Add(new RockTier(GlobalGameParms.GameParms.GetInt("RockLevelTier" + count),
                        GlobalGameParms.GameParms.GetFloat("RockLevelTier" + count + "DecayRate")));
                    count++;
                }
            }
            if (tiers.Count == 0)
                tiers.Add(new RockTier(1, 3.0f));

            actor.RegisterDeathFunction(OnPlayerDeath);

            ResetEverything();
            base.Initialize(stage);
        }

        void OnPlayerDeath()
        {
            numDeaths++;
            ResetToLastValues();
        }

        public void Update(float dt)
        {

            if (score != goalScore)
                Lerp(goalScore, ref score, (int)Math.Ceiling(900 * dt));

            if (rockLevelLocked) //don't cooldown our timer if we are locked
                return;

            timeSinceLastMove += dt;
            if (timeSinceLastMove >= RockLevelCooldownTime)
                RockLevelDownDueToLowAPS(1);
        }

        public void PerformedAttack(float increaseAmount)
        {
            IncreaseRockLevel(increaseAmount);
            timeSinceLastMove = 0;
        }

        /// <summary>
        /// multiplies the value by the health and increases the score by that value
        /// </summary>
        /// <param name="amount">the base amount to increase by</param>
        public int IncreaseScore(int amount)
        {
            goalScore += amount * rockLevel;
            return amount * rockLevel;
        }

        public int IncreaseScoreDueToKill(int amount)
        {
            goalScore += amount * rockLevel;
            killStreak++;
            if (killStreak > highestKillStreak)
                highestKillStreak = killStreak;

            string sound = null;
            switch (killStreak)
            {
                case 5:
                    sound = "DemonSlayerLoud";
                    break;
                case 10:
                    sound = "BloodBathLoud";
                    break;
                case 20:
                    sound = "RampageLoud";
                    break;
                case 30:
                    sound = "SlaughterhouseLoud";
                    break;
                case 50:
                    sound = "BloodOrgyLoud";
                    break;
                case 100:
                    sound = "RockGodLoud";
                    break;
            }

            if (sound != null)
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound(sound, 1, 0, 0);

            return amount * rockLevel;
        }

        /// <summary>
        /// increases the score by the value (without using the health)
        /// </summary>
        /// <param name="amount">the amount to increase by (will not be multiplied)</param>
        public void IncreaseScoreNoMultiplier(int amount)
        {
            goalScore += amount;
        }

        public void IncreaseRockLevel(float amount)
        {
            rockLevelf += amount;
            rockLevel = (int)Math.Floor(rockLevelf);

            //only play the sound when you reach rock meter 11
            if (rockLevel != lastRockLevel)
            {
                lastRockLevel = rockLevel;
                if(rockLevel >= 11)
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("KnobClick_16", 1, 0, 0);
            }

            if (rockLevel > 11)
            {
                rockLevel = 11;
                rockLevelf = 11.0f;
            }

            //check to see if we have gone up a tier
            while(tierIndex < tiers.Count - 1 && rockLevel >= tiers[tierIndex + 1].minLevel)
            {
                tierIndex++;
                RockLevelCooldownTime = tiers[tierIndex].cooldownTime;
            }
        }

        public void ResetEverything()
        {
            score = goalScore = 0;
            rockLevel = startingRockLevel;
            lastRockLevel = rockLevel;
            if (GlobalGameParms.initialized)
            {
                if (GlobalGameParms.GameParms.HasParm("RockLevelStartingValue"))
                    rockLevel = GlobalGameParms.GameParms.GetInt("RockLevelStartingValue");
            }
            rockLevelf = rockLevel;

            tierIndex = 0;
            while (tierIndex < tiers.Count - 1 && rockLevel >= tiers[tierIndex+1].minLevel)
                tierIndex++;
            RockLevelCooldownTime = tiers[tierIndex].cooldownTime;

            highestRockLevel = rockLevel;
            timeSinceLastMove = 0.0f;
            numTimesHit = 0;
            killStreak = 0;
            numDeaths = 0;
            highestKillStreak = 0;
        }

        public void ResetToLastValues()
        {
            score = goalScore = prevValues.Score;
            rockLevel = prevValues.RockLevel;
            
            rockLevelf = rockLevel;

            tierIndex = 0;
            while (tierIndex < tiers.Count - 1 && rockLevel >= tiers[tierIndex + 1].minLevel)
                tierIndex++;
            RockLevelCooldownTime = tiers[tierIndex].cooldownTime;

            highestKillStreak = 0;
            killStreak = 0;
            timeSinceLastMove = 0.0f;
        }

        public void RockLevelDownDueToDamage(float damage)
        {
            numTimesHit++;
            if (rockLevel > highestRockLevel)
                highestRockLevel = rockLevel;
            rockLevelf -= damage;
            rockLevel = (int)Math.Floor(rockLevelf);
            if (rockLevel < 1 && !dontKillActor)
            {
                rockLevel = 0;
                this.actor.NotifyDeathList();
            }

            lastRockLevel = rockLevel;
            
            //check to see if we have gone down a tier
            while (tierIndex > 0 && rockLevel < tiers[tierIndex].minLevel)
            {
                tierIndex--;
                RockLevelCooldownTime = tiers[tierIndex].cooldownTime;
            }
        }

        public void RockLevelDownDueToLowAPS(float amount)
        {
            timeSinceLastMove = 0.0f;
            if (rockLevel <= LowestRockLevelDueToAPS) return;
            if (rockLevel > highestRockLevel)
                highestRockLevel = rockLevel;
            rockLevelf -= amount;
            rockLevel = (int)Math.Floor(rockLevelf);

            if (rockLevel < LowestRockLevelDueToAPS) //you can't die from low APS
            {
                float decimals = Math.Abs(rockLevelf - rockLevel);
                rockLevel = LowestRockLevelDueToAPS;
                rockLevelf = rockLevel;
            }
            
            lastRockLevel = rockLevel;

            //check to see if we have gone down a tier
            while (tierIndex > 0 && rockLevel < tiers[tierIndex].minLevel)
            {
                tierIndex--;
                RockLevelCooldownTime = tiers[tierIndex].cooldownTime;
            }
        }

        public void Lerp(int goalVal, ref int currVal, int lerpAmount)
        {
            if (goalVal < currVal)
            {
                currVal -= lerpAmount;
                if (currVal <= goalVal) currVal = goalVal;
            }
            else
            {
                currVal += lerpAmount;
                if (currVal >= goalVal) currVal = goalVal;
            }
        }
        
        public void AddKill() 
        {
            killCount++;
        }

        struct RockTier
        {
            public int minLevel;
            public float cooldownTime;

            public RockTier(int lvl, float time)
            {
                minLevel = lvl;
                cooldownTime = time;
            }
        }

        struct PreviousValues
        {
            public int Score;
            public int RockLevel;
            public int HighLevel;
            public int NumKills;

            public PreviousValues(int start)
            {
                Score = RockLevel = HighLevel = NumKills = 0;
            }

            public void Store(int s, int rl, int hrl, int k)
            {
                Score = s;
                RockLevel = rl;
                HighLevel = hrl;
                NumKills = k;
            }

        }
    }
}
