using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class DebugQB : Quarterback
    {
        SpriteFont font;
        Vector2 fpsPosition;
        

        double avgFps = 0.0;
        double sumFps = 0.0;
        double sumCount = 0.0;
        double nextUpdateTime = 1.0;
        double sumTime = 0.0;

        Actor guitarAvatar;

#if WINDOWS
        static System.Diagnostics.PerformanceCounter cpuCounter;
        static System.Diagnostics.PerformanceCounter ramCounter;
        static float cpu;
        static float ram;
        Vector2 ramPosition;
#endif
        Vector2 cpuPosition;

#if TUNE_DEPTH_BIAS
        Vector2 depthBiasPosition;
#endif

        public override string Name()
        {
            return "DebugQB";
        }

        public DebugQB()
        {
            fpsPosition = new Vector2(Renderer.ScreenWidth - 200, Renderer.ScreenHeight - 80);

            cpuPosition = new Vector2(Renderer.ScreenWidth - 200, 60);
#if WINDOWS
            ramPosition = new Vector2(Renderer.ScreenWidth - 200, 40);

            if (cpuCounter == null)
            {
                cpuCounter = new System.Diagnostics.PerformanceCounter(
                    "Process",
                    "% Processor Time",
                    System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            }

            if (ramCounter == null)
            {
                ramCounter = new System.Diagnostics.PerformanceCounter(
                    "Process",
                    "Working Set",
                    System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            }
#endif // WINDOWS

#if TUNE_DEPTH_BIAS
            depthBiasPosition = new Vector2(20, 200);
#endif
        }

        public override void LoadContent()
        {
            font = Stage.Content.Load<SpriteFont>("DefaultFont");            
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            guitarAvatar = Stage.LoadingStage.GetQB<ActorQB>().FindActor("GuitarAvatar");
        }

        public override void PauseQB()
        {
            //unpausable
        }

        public override void Update(float dt)
        {
            sumTime += dt;
            sumFps += 1.0 / dt;
            sumCount++;

            if (sumTime > nextUpdateTime)
            {
                sumTime = 0.0;
                avgFps = sumFps / sumCount;
                sumFps = 0.0;
                sumCount = 0.0;

#if WINDOWS
                ram = ramCounter.NextValue();
                cpu = cpuCounter.NextValue();
#endif // WINDOWS
            }
        }

        public override void DrawUI(float dt)
        {
#if DEBUG
            // debug menu info
            Renderer.Instance.SpriteBatch.DrawString(font, avgFps.ToString("F"), fpsPosition, Color.White);

#if WINDOWS
            Renderer.Instance.SpriteBatch.DrawString(font, "RAM: " + (ram / 1024f / 1024f).ToString("F") + " MB", ramPosition, Color.White);
            Renderer.Instance.SpriteBatch.DrawString(font, "CPU: " + (cpu) + " %", cpuPosition, Color.White);
#endif
            //Renderer.Instance.SpriteBatch.DrawString(font, CameraQB.ToString(), new Vector2(20, 40), Color.White);
            if (guitarAvatar != null && guitarAvatar.PhysicsObject != null)
            {
                Renderer.Instance.SpriteBatch.DrawString(font, guitarAvatar.PhysicsObject.Position.ToString(), new Vector2(20, 20), Color.White);
                Renderer.Instance.SpriteBatch.DrawString(font, "Flow: " + guitarAvatar.GetAgent<PlayerAgent>().flow.flow_level, cpuPosition + new Vector2(0, 60), Color.White);
            }
#endif
#if TUNE_DEPTH_BIAS
            Renderer.Instance.SpriteBatch.DrawString(font, "Depth Bias:" + Sun.DepthBias.ToString(), depthBiasPosition, Color.Yellow);
#endif
        }

        public void drawCharacterAction(float dt, string phrase)
        {
            // debug menu info
            Renderer.Instance.SpriteBatch.DrawString(font, phrase, new Vector2(500, 500), Color.White);
        }

        public override void Serialize(ParameterSet parm)
        {
            //TODO::
            //figure out what needs to be updated here

        }
    }
}
