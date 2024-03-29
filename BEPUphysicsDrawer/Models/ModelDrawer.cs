﻿using System;
using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.UpdateableSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Collidables.MobileCollidables;

namespace BEPUphysicsDrawer.Models
{
    /// <summary>
    /// Manages and draws models.
    /// </summary>
    public abstract class ModelDrawer
    {
        private readonly Dictionary<object, ModelDisplayObject> displayObjects = new Dictionary<object, ModelDisplayObject>();
        private readonly RasterizerState fillState;

        private readonly List<SelfDrawingModelDisplayObject> selfDrawingDisplayObjects = new List<SelfDrawingModelDisplayObject>();
        private readonly RasterizerState wireframeState;
        protected Texture2D colors;



        private static readonly Dictionary<Type, Type> displayTypes = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, ShapeMeshGetter> shapeMeshGetters = new Dictionary<Type, ShapeMeshGetter>();

        /// <summary>
        /// Gets the map from object types to display object types.
        /// </summary>
        public static Dictionary<Type, Type> DisplayTypes
        {
            get { return displayTypes; }
        }

        /// <summary>
        /// Gets the map from shape object types to methods which can be used to construct the data.
        /// </summary>
        public static Dictionary<Type, ShapeMeshGetter> ShapeMeshGetters
        {
            get { return shapeMeshGetters; }
        }

        static ModelDrawer()
        {
            //Display types are sometimes requested from contexts lacking a convenient reference to a ModelDrawer instance.
            //Having them static simplifies things.
            displayTypes.Add(typeof(FluidVolume), typeof(DisplayFluid));
            displayTypes.Add(typeof(Terrain), typeof(DisplayTerrain));
            displayTypes.Add(typeof(TriangleMesh), typeof(DisplayTriangleMesh));
            displayTypes.Add(typeof(StaticMesh), typeof(DisplayStaticMesh));
            displayTypes.Add(typeof(InstancedMesh), typeof(DisplayInstancedMesh));


            //Entity types are handled through a special case that uses an Entity's Shape to look up one of the ShapeMeshGetters.
            shapeMeshGetters.Add(typeof(ConvexCollidable<BoxShape>), DisplayBox.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<SphereShape>), DisplaySphere.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<CapsuleShape>), DisplayCapsule.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<CylinderShape>), DisplayCylinder.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<ConeShape>), DisplayCone.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<TriangleShape>), DisplayTriangle.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<ConvexHullShape>), DisplayConvexHull.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<MinkowskiSumShape>), DisplayMinkowskiSum.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<WrappedShape>), DisplayWrappedBody.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(ConvexCollidable<TransformableShape>), DisplayTransformable.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(CompoundCollidable), DisplayCompoundBody.GetShapeMeshData);
            shapeMeshGetters.Add(typeof(MobileMeshCollidable), DisplayMobileMesh.GetShapeMeshData);

        }

        protected ModelDrawer(GraphicsDevice device)
        {
            Device = device;

            colors = new Texture2D(Device, 8, 1);
            colors.SetData(new[] 
            { 
                new Color(255, 216, 0),
                new Color(79, 200, 255),
                new Color(255, 0, 0),
                new Color(177, 0, 254),
                new Color(255, 130, 151),
                new Color(254, 106, 0),
                new Color(168, 165, 255),
                new Color(0, 254, 33)
            });


            fillState = new RasterizerState();
            wireframeState = new RasterizerState();
            wireframeState.FillMode = FillMode.WireFrame;
        }



        /// <summary>
        /// Gets the device using this ModelDrawer.
        /// </summary>
        public GraphicsDevice Device { get; private set; }

        /// <summary>
        /// Gets or sets whether or not the model drawer is drawing wireframes.
        /// </summary>
        public bool IsWireframe { get; set; }

        /// <summary>
        /// Constructs a new display object for an object.
        /// </summary>
        /// <param name="objectToDisplay">Object to create a display object for.</param>
        /// <returns>Display object for an object.</returns>
        public ModelDisplayObject GetDisplayObject(object objectToDisplay)
        {
            Type displayType;
            if (!displayObjects.ContainsKey(objectToDisplay))
            {
                if (displayTypes.TryGetValue(objectToDisplay.GetType(), out displayType))
                {
#if !WINDOWS
                    return (ModelDisplayObject)displayType.GetConstructor(
                                                     new Type[] { typeof(ModelDrawer), objectToDisplay.GetType() })
                                                     .Invoke(new object[] { this, objectToDisplay });
#else
                    return (ModelDisplayObject)Activator.CreateInstance(displayType, new[] { this, objectToDisplay });
#endif
                }
                Entity e;
                if ((e = objectToDisplay as Entity) != null)
                {
                    return new DisplayEntityCollidable(this, e.CollisionInformation);
                }
                EntityCollidable entityCollidable;
                if ((entityCollidable = objectToDisplay as EntityCollidable) != null)
                {
                    return new DisplayEntityCollidable(this, entityCollidable);
                }

            }
            return null;
        }


        /// <summary>
        /// Attempts to add an object to the ModelDrawer.
        /// </summary>
        /// <param name="objectToDisplay">Object to be added to the model drawer.</param>
        /// <returns>ModelDisplayObject created for the object.  Null if it couldn't be added.</returns>
        public ModelDisplayObject Add(object objectToDisplay)
        {
            ModelDisplayObject displayObject = GetDisplayObject(objectToDisplay);
            if (displayObject != null)
            {
                Add(displayObject);
                displayObjects.Add(objectToDisplay, displayObject);
                return displayObject;
            }
            return null; //Couldn't add it.
        }

        /// <summary>
        /// Adds the display object to the drawer.
        /// </summary>
        /// <param name="displayObject">Display object to add.</param>
        /// <returns>Whether or not the display object was added.</returns>
        public bool Add(SelfDrawingModelDisplayObject displayObject)
        {
            if (!selfDrawingDisplayObjects.Contains(displayObject))
            {
                selfDrawingDisplayObjects.Add(displayObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a display object directly to the drawer without being linked to a source.
        /// </summary>
        /// <param name="displayObject">Display object to add.</param>
        public abstract void Add(ModelDisplayObject displayObject);

        /// <summary>
        /// Removes an object from the drawer.
        /// </summary>
        /// <param name="objectToRemove">Object to remove.</param>
        /// <returns>Whether or not the object was present.</returns>
        public bool Remove(object objectToRemove)
        {
            ModelDisplayObject displayObject;
            if (displayObjects.TryGetValue(objectToRemove, out displayObject))
            {
                Remove(displayObject);
                displayObjects.Remove(objectToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an object from the drawer.
        /// </summary>
        /// <param name="displayObject">Display object to remove.</param>
        /// <returns>Whether or not the object was present.</returns>
        public bool Remove(SelfDrawingModelDisplayObject displayObject)
        {
            return selfDrawingDisplayObjects.Remove(displayObject);
        }


        /// <summary>
        /// Removes a display object from the drawer.  Only use this if display object was added directly.
        /// </summary>
        /// <param name="displayObject">Object to remove.</param>
        public abstract void Remove(ModelDisplayObject displayObject);

        /// <summary>
        /// Cleans out the model drawer of any existing display objects.
        /// </summary>
        public void Clear()
        {
            displayObjects.Clear();
            selfDrawingDisplayObjects.Clear();
            ClearManagedModels();
        }

        /// <summary>
        /// Cleans out any data contained by derived drawers.
        /// </summary>
        protected abstract void ClearManagedModels();

        /// <summary>
        /// Determines if the object has an associated display object in this drawer.
        /// </summary>
        /// <param name="displayedObject">Object to check for in the drawer.</param>
        /// <returns>Whether or not the object has an associated display object in this drawer.</returns>
        public bool Contains(object displayedObject)
        {
            return displayObjects.ContainsKey(displayedObject);
        }

        /// <summary>
        /// Updates the drawer and its components.
        /// </summary>
        public void Update()
        {
            foreach (SelfDrawingModelDisplayObject displayObject in selfDrawingDisplayObjects)
                displayObject.Update();
            UpdateManagedModels();
        }

        /// <summary>
        /// Updates the drawer's technique.
        /// </summary>
        protected abstract void UpdateManagedModels();

        /// <summary>
        /// Draws the drawer's models.
        /// </summary>
        /// <param name="viewMatrix">View matrix to use to draw the objects.</param>
        /// <param name="projectionMatrix">Projection matrix to use to draw the objects.</param>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            Device.RasterizerState = IsWireframe ? wireframeState : fillState;

            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;

            foreach (SelfDrawingModelDisplayObject displayObject in selfDrawingDisplayObjects)
                displayObject.Draw(viewMatrix, projectionMatrix);
            DrawManagedModels(viewMatrix, projectionMatrix);
        }

        /// <summary>
        /// Draws the models managed by the drawer using the appropriate technique.
        /// </summary>
        /// <param name="viewMatrix">View matrix to use to draw the objects.</param>
        /// <param name="projectionMatrix">Projection matrix to use to draw the objects.</param>
        protected abstract void DrawManagedModels(Matrix viewMatrix, Matrix projectionMatrix);

        public delegate void ShapeMeshGetter(EntityCollidable collidable, List<VertexPositionNormalTexture> vertices, List<ushort> indices);
    }
}