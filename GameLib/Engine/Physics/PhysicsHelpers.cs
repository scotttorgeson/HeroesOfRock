using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using Physics.CylinderCharacter;

namespace GameLib
{
    public class PhysicsHelpers
    {
        /// <summary>
        /// Create a Physics box prefab from a model.
        /// </summary>
        /// <param name="m">model to create from</param>
        /// <param name="position">position in world</param>
        /// <param name="mass">mass of box</param>
        /// <returns></returns>
        public static BEPUphysics.Entities.Entity/*BEPUphysics.Entities.Prefabs.Box*/ ModelToPhysicsBox(Model m, Vector3 position, float mass, float pitch, float yaw, float roll)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(m, out vertices, out indices);
            BoundingBox box = BoundingBox.CreateFromPoints(vertices);
            // calculate dimensions
            Vector3 difference = box.Max - box.Min;
            float length = difference.Z;
            float height = difference.Y;
            float width = difference.X;

            BEPUphysics.Entities.Entity e;
            
            if ( mass == -1.0f)
            {
                e = new BEPUphysics.Entities.Entity(new BEPUphysics.CollisionShapes.ConvexShapes.BoxShape(width, height, length));
            }
            else
            {
                e = new BEPUphysics.Entities.Entity(new BEPUphysics.CollisionShapes.ConvexShapes.BoxShape(width, height, length), mass);                
            }
            e.Orientation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            e.Position = position;            
            return e;
            //return new BEPUphysics.Entities.Prefabs.Box(position, width, height, length, mass);            
        }

        public static BEPUphysics.Collidables.StaticMesh ModelToStaticMesh(Model model, BEPUphysics.MathExtensions.AffineTransform worldTransform)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            var mesh = new BEPUphysics.Collidables.StaticMesh(vertices, indices, worldTransform);
            return mesh;
        }

        public static BEPUphysics.DataStructures.TriangleMesh ModelToTriangleMesh(Model model, Vector3 position)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);

            for (int i = 0; i < vertices.Length; i++ )
            {                
                vertices[i] = vertices[i] + position;
            }

            var mesh = new BEPUphysics.DataStructures.TriangleMesh(new BEPUphysics.DataStructures.StaticMeshData(vertices, indices));
            return mesh;
        }

        public static CharacterController ModelToCharacterController(Model model, Vector3 position, float mass, float scaleRadius)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            BoundingBox box = BoundingBox.CreateFromPoints(vertices);
            // calculate dimensions
            Vector3 difference = box.Max - box.Min;
            float length = difference.Z;
            float height = difference.Y;
            float width = difference.X;

            float radius = Math.Max(width, length) * scaleRadius;

            return new CharacterController(position, height, height / 2, radius, mass);
        }

        public static CylinderCharacterController ModelToCylinderCharacterController(Model model, Vector3 position, float mass, float scaleRadius)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            BoundingBox box = BoundingBox.CreateFromPoints(vertices);
            // calculate dimensions
            Vector3 difference = box.Max - box.Min;
            float length = difference.Z;
            float height = difference.Y;
            float width = difference.X;

            float radius = Math.Max(width, length) * scaleRadius;

            return new CylinderCharacterController(position, height, radius, mass);
        }

        public static SimpleCharacterController ModelToSimpleCharacterController(Model model, Vector3 position, float mass, float supportHeight)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            BoundingBox box = BoundingBox.CreateFromPoints(vertices);
            // calculate dimensions
            Vector3 difference = box.Max - box.Min;
            float length = difference.Z;
            float height = difference.Y;
            float width = difference.X;

            float radius = Math.Max(width, length);

            return new SimpleCharacterController(position, height, radius, supportHeight, mass);
        }

        public static BoundingSphere ModelToBoundingSphere(Model model, Vector3 position)
        {
            Vector3[] vertices;
            int[] indices;
            BEPUphysics.DataStructures.TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            return BoundingSphere.CreateFromPoints(vertices);
        }

        public static Ray MouseClickToRay(Vector2 mousePosition, Matrix projectionMatrix, Matrix viewMatrix, Viewport viewport)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(mousePosition, 0f);
            Vector3 farSource = new Vector3(mousePosition, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

        public static void QuaternionToEuler(ref Quaternion q, out Vector3 v)
        {
            double test = q.X * q.Y + q.Z * q.W;            
            float bank = 0.0f;
            float heading = 0.0f;
            float attitude = 0.0f;

            if ( test > 0.4999 )
            {
                heading = (float)(2.0 * Math.Atan2(q.X, q.W));
                attitude = MathHelper.PiOver2;
                bank = 0.0f;
            }
            else if (test < -0.499)
            {
                heading = (float)(-2.0 * Math.Atan2(q.X, q.W));
                attitude = -MathHelper.PiOver2;
                bank = 0.0f;
            }
            else
            {
                double sqX = q.X * q.X;
                double sqY = q.Y * q.Y;
                double sqZ = q.Z * q.Z;
                heading = (float)Math.Atan2(2.0 * q.Y * q.W - 2.0 * q.X * q.Z, 1.0 - 2.0 * sqY - 2.0 * sqZ);
                attitude = (float)Math.Asin(2.0 * test);
                bank = (float)Math.Atan2(2.0 * q.X * q.W - 2.0 * q.Y * q.Z, 1.0 - 2.0 * sqX - 2.0 * sqZ);
            }

            v = new Vector3(MathHelper.ToDegrees(bank), MathHelper.ToDegrees(heading), MathHelper.ToDegrees(attitude));
        }
    }
}
