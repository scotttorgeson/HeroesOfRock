using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    class FullscreenQuad
    {
        VertexBuffer vb;
        IndexBuffer ib;

        public FullscreenQuad(GraphicsDevice GraphicsDevice)
        {
            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
            };

            vb = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.None);
            vb.SetData<VertexPositionTexture>(vertices);

            ushort[] indices = { 0, 1, 2, 2, 3, 0 };
            ib = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            ib.SetData<ushort>(indices);
        }

        // set buffers and draw
        public void Draw(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.SetVertexBuffer(vb);
            GraphicsDevice.Indices = ib;
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

        // just set buffers
        public void ReadyBuffers(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.SetVertexBuffer(vb);
            GraphicsDevice.Indices = ib;
        }

        // just draw
        public void JustDraw(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
    }
}
