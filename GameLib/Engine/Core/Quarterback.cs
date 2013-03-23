using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public abstract class Quarterback
    {
        bool paused = false;
        public bool IsPaused
        {
            get { return paused; }
            protected set { paused = value; }
        }


        public virtual void KillInstance() { }
        public virtual void Update(float dt) { }
        
        // used to draw anything that is part of the ui
        public virtual void DrawUI(float dt) { }

        /*  Initialization ordering:
         * PreLoadInit()
         * LoadContent()
         * PostLoadInit()
         * LevelLoaded()
         */
        public virtual void PreLoadInit(ParameterSet Parm) { }
        public virtual void LoadContent() { }
        public virtual void PostLoadInit(ParameterSet Parm) { }
        public virtual void LevelLoaded() { }

        public virtual void PauseQB() { paused = true; }
        public virtual void UnPauseQB() { paused = false; }
        // todo:
        // qb's overload it and write out any data they want to be saved to the file to the parameter set
        // actor qb adds all its actor stuff by calling actor.SerializeStage()
        // etc.
        public virtual void Serialize(ParameterSet parm) { }

        public abstract string Name();
    }
}
