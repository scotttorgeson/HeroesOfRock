using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    public class Camera
    {
        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix ViewMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the projection matrix of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        public virtual void Update(float dt)
        {

        }
    }
}
