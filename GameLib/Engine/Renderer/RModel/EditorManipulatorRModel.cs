using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    class EditorManipulatorRModel : RModel
    {
        public EditorManipulatorRModel(ParameterSet parm)
            : base(parm)
        {

        }

        public override void  LoadContent(ContentManager content, ParameterSet parm, Stage stage)
        {
            // just load the model
            bool initialized = false;
            model = Renderer.Instance.LookupModel("Models/" + Name, out initialized);

 	        base.LoadContent(content, parm, stage);
        }

        public override void DrawInstances(Renderer.DrawType technique)
        {
            // handled by the editor, we do nothing!
        }
    }
}
