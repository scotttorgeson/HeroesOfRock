using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using Physics.CylinderCharacter;

namespace GameLib
{
    public class PhysicsObject
    {
        public enum PhysicsType
        {
            Box,
            StaticMesh,
            Character,
            /*SimpleCharacter,*/
            CylinderCharacter,
            TriggerVolume,
            None,
        }

        public static bool ForceStaticMesh = false; // in the editor edit mode, everything uses a static mesh

        ISpaceObject spaceObject;
        public ISpaceObject SpaceObject { get { return spaceObject; } }
        public PhysicsType physicsType { get; private set; }

        public static bool DisableMass = false;

        Vector3 position;
        Matrix rotation;

        private bool collisionEnabled = true;
        public bool CollisionEnabled
        {
            get { return collisionEnabled; }
            set
            {
                if (value != collisionEnabled)
                {
                    if (value)
                    {
                        switch (physicsType)
                        {
                            case PhysicsType.Character:
                                Space.Add(characterController);
                                break;
                            case PhysicsType.CylinderCharacter:
                                Space.Add(cylinderCharController);
                                break;
                            case PhysicsType.None:
                                break;
                            default:
                                Space.Add(spaceObject);
                                break;
                        }
                    }
                    else
                    {
                        switch (physicsType)
                        {
                            case PhysicsType.Character:
                                Space.Remove(characterController);
                                break;
                            case PhysicsType.CylinderCharacter:
                                Space.Remove(cylinderCharController);
                                break;
                            case PhysicsType.None:
                                break;
                            default:
                                Space.Remove(spaceObject);
                                break;
                        }
                    }
                }
            }
        }

        // used for avatars (player characters)...
        CharacterController characterController;
        public CharacterController CharacterController
        {
            get { return characterController; }
            set { characterController = value; }
        }

        /*SimpleCharacterController simpleCharController;
        public SimpleCharacterController SimpleCharController
        {
            get { return simpleCharController; }
            set { simpleCharController = value; }
        }*/

        CylinderCharacterController cylinderCharController;
        public CylinderCharacterController CylinderCharController
        {
            get { return cylinderCharController; }
            set { cylinderCharController = value; }
        }

        readonly Actor actor;

        /*public PhysicsObject(PhysicsType physicsType, Model model, Vector3 position, float mass)
        {
            this.physicsType = physicsType;
            int id = idCounter++;

            if (DisableMass)
                mass = -1.0f;

            switch (physicsType)
            {
                case PhysicsType.Box:                    
                    spaceObject = PhysicsHelpers.ModelToPhysicsBox(model, position, mass);
                    // boxes contain a broadphase entry which is what shows up in ray casts, so we need to make sure its tag is set to the right id
                    // this will need to be done fore anything that is an ibroadphaseentryowner
                    ((IBroadPhaseEntryOwner)spaceObject).Entry.Tag = id;
                    break;
                case PhysicsType.StaticMesh:                    
                    spaceObject = PhysicsHelpers.ModelToStaticMesh(model, new BEPUphysics.MathExtensions.AffineTransform(position));
                    //staticmesh's are not ibroadphaseentryowners...they will turn directly on the raycast, so nothing else to set the tag on
                    break;
                case PhysicsType.None:
                    spaceObject = null;
                    this.position = position;
                    return;
            }

            spaceObject.Tag = id;
            Stage.ActiveStage.GetQB<PhysicsQB>().Space.Add(spaceObject);            
        }*/

        /// <summary>
        /// Create a physics object
        /// </summary>
        /// <param name="actor">Actor that we are attached to</param>
        /// <param name="Parm">Actor's parm file</param>
        /// <param name="model">Actors model (so we can size the collider)</param>
        /// <param name="position">Position (from world parm)</param>
        /// <param name="rotation">Rotation (from world parm)</param>
        public PhysicsObject(Actor actor, ParameterSet Parm, Model model, Vector3 position, Vector3 rotation, Stage stage) // position and rotation come from world parm file, which we dont have access to here
        {
            // **rotation comes in from file as ( pitch, yaw, roll )**

            this.actor = actor;
            this.physicsType = GameLib.PhysicsObject.PhysicsTypeFromString(Parm.GetString("PhysicsType"));
            this.position = position;
            this.rotation = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            
            if (ForceStaticMesh)
                this.physicsType = PhysicsType.StaticMesh;

            switch (this.physicsType)
            {
                case PhysicsType.Box:
                    {
                        float mass;
                        if (DisableMass)
                            mass = -1.0f;
                        else
                            mass = Parm.GetFloat("Mass");
                        BEPUphysics.Entities.Entity entity = PhysicsHelpers.ModelToPhysicsBox(model, position, mass, rotation.X, rotation.Y, rotation.Z);
                        entity.CollisionInformation.OwningActor = actor;
                        spaceObject = entity;
                        break;
                    }
                case PhysicsType.StaticMesh:
                    BEPUphysics.Collidables.StaticMesh mesh = PhysicsHelpers.ModelToStaticMesh(model, new BEPUphysics.MathExtensions.AffineTransform(Quaternion.CreateFromRotationMatrix( this.rotation ), position));
                    mesh.OwningActor = actor;
                    spaceObject = mesh;
                    break;
                case PhysicsType.Character:
                    {
                        float mass = Parm.GetFloat("Mass");
                        float scaleRadius = 1.0f;
                        if (Parm.HasParm("ScaleRadius"))
                            scaleRadius = Parm.GetFloat("ScaleRadius");
                        characterController = PhysicsHelpers.ModelToCharacterController(model, position, mass, scaleRadius);
                        spaceObject = characterController.Body;
                        characterController.Body.CollisionInformation.OwningActor = actor;
                        stage.GetQB<PhysicsQB>().AddToSpace(CharacterController);
                        return;
                    }
                case PhysicsType.CylinderCharacter:
                    {
                        float mass = Parm.GetFloat("Mass");
                        float scaleRadius = 1.0f;
                        if (Parm.HasParm("ScaleRadius"))
                            scaleRadius = Parm.GetFloat("ScaleRadius");
                        if (Parm.HasParm("ColliderDims"))
                        {
                            Vector2 v = Parm.GetVector2("ColliderDims");
                            cylinderCharController = new CylinderCharacterController(position, v.Y, v.X, mass);
                        }
                        else
                            cylinderCharController = PhysicsHelpers.ModelToCylinderCharacterController(model, position, mass, scaleRadius);
                        cylinderCharController.Body.Orientation = Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
                        spaceObject = cylinderCharController.Body;
                        cylinderCharController.Body.CollisionInformation.OwningActor = actor;
                        stage.GetQB<PhysicsQB>().AddToSpace(cylinderCharController);
                        return;
                    }
                case PhysicsType.TriggerVolume:
                    {
                        DetectorVolume detectorVolume = new DetectorVolume(PhysicsHelpers.ModelToTriangleMesh(model, position));
                        detectorVolume.OwningActor = actor;
                        spaceObject = detectorVolume;
                        detectorVolume.EntityBeganTouching += new EntityBeginsTouchingVolumeEventHandler(detectorVolume_EntityBeganTouching);
                        detectorVolume.EntityStoppedTouching += new EntityStopsTouchingVolumeEventHandler(detectorVolume_EntityStoppedTouching);
                        break;
                    }
                case PhysicsType.None:
                    spaceObject = null;
                    this.position = position;
                    this.rotation = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
                    return;
            }

            stage.GetQB<PhysicsQB>().AddToSpace(spaceObject);  
        }

        void detectorVolume_EntityStoppedTouching(DetectorVolume volume, BEPUphysics.Entities.Entity toucher)
        {
            actor.EndCollision((Actor)toucher.CollisionInformation.OwningActor);
        }

        void detectorVolume_EntityBeganTouching(DetectorVolume volume, BEPUphysics.Entities.Entity toucher)
        {
            actor.BeginCollision((Actor)toucher.CollisionInformation.OwningActor);
        }

        public static PhysicsType PhysicsTypeFromString(string typeName)
        {
            switch (typeName)
            {
                case "Box":
                    return PhysicsType.Box;
                case "StaticMesh":
                    return PhysicsType.StaticMesh;
                case "Character":
                    return PhysicsType.Character;
                //case "SimpleCharacter":
                //    return PhysicsType.SimpleCharacter;
                case "CylinderCharacter":
                    return PhysicsType.CylinderCharacter;
                case "None":
                    return PhysicsType.None;
                case "TriggerVolume":
                    return PhysicsType.TriggerVolume;
                default:
                    return PhysicsType.None;
            }
        }

        public void Kill()
        {
            PhysicsQB physicsQB = Stage.ActiveStage.GetQB<PhysicsQB>();
            if (characterController != null && characterController.Space != null)
            {
                physicsQB.RemoveFromSpace(characterController);
                characterController = null;
            }

            if (cylinderCharController != null && cylinderCharController.Space != null)
            {
                physicsQB.RemoveFromSpace(cylinderCharController);
                cylinderCharController = null;
            }

            if (spaceObject != null && spaceObject.Space != null)
            {
                physicsQB.RemoveFromSpace(spaceObject);
                spaceObject = null;
            }
        }

        public ISpace Space
        {
            get { return spaceObject.Space; }
        }

        // lots of accessors to follow. some may be broken for some types, but just about everything should work

        public BEPUphysics.Collidables.MobileCollidables.EntityCollidable CollisionInformation
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).CollisionInformation;
                    case PhysicsType.Character:
                        return characterController.Body.CollisionInformation;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.CollisionInformation;
                    default:
                        return null;
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).Position;
                    case PhysicsType.StaticMesh:
                        return ((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform.Translation;
                    case PhysicsType.Character:
                        return characterController.Body.BufferedStates.InterpolatedStates.Position;
                        //return characterController.Body.Position;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.Position;
                    case PhysicsType.None:
                        return position;
                    case PhysicsType.TriggerVolume:
                        return position;
                    default:
                        return Vector3.Zero;
                }
            }
            set
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        ((BEPUphysics.Entities.Entity)spaceObject).Position = value;
                        break;
                    case PhysicsType.StaticMesh:
                        ((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform = new BEPUphysics.MathExtensions.AffineTransform(Orientation, value);
                        break;
                    case PhysicsType.Character:
                        characterController.Body.Position = value;
                        break;
                    case PhysicsType.CylinderCharacter:
                        cylinderCharController.Body.Position = value;
                        break;
                    case PhysicsType.None:
                        position = value;
                        break;
                }
            }
        }

        public Vector3 LinearVelocity
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).LinearVelocity;
                    case PhysicsType.Character:
                        return characterController.Body.LinearVelocity;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.LinearVelocity;
                    default:
                        return Vector3.Zero;
                }
            }
            set
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        ((BEPUphysics.Entities.Entity)spaceObject).LinearVelocity = value;
                        break;
                    case PhysicsType.Character:
                        characterController.Body.LinearVelocity = value;
                        break;
                    case PhysicsType.CylinderCharacter:
                        cylinderCharController.Body.LinearVelocity = value;
                        break;
                }
            }
        }

        public Quaternion Orientation
        {
            get
            {
                switch (physicsType)
                {                    
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).Orientation;
                    case PhysicsType.StaticMesh:
                        return Quaternion.CreateFromRotationMatrix(((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform.Matrix);
                    case PhysicsType.None:
                        return Quaternion.CreateFromRotationMatrix(rotation);
                    case PhysicsType.Character:
                        return characterController.Body.Orientation;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.Orientation;
                    default:
                        return Quaternion.Identity;
                }
            }
            set
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        ((BEPUphysics.Entities.Entity)spaceObject).Orientation = value;
                        break;
                    case PhysicsType.StaticMesh:
                        ((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform = new BEPUphysics.MathExtensions.AffineTransform(value, Position);
                        break;
                    case PhysicsType.None:
                        rotation = Matrix.CreateFromQuaternion(value);
                        break;
                    case PhysicsType.Character:
                        characterController.Body.Orientation = value;
                        break;
                    case PhysicsType.CylinderCharacter:
                        cylinderCharController.Body.Orientation = value;
                        break;
                }
            }
        }
      
        public BEPUphysics.EntityStateManagement.EntityBufferedStates BufferedStates
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).BufferedStates;
                    case PhysicsType.Character:
                        return characterController.Body.BufferedStates;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.BufferedStates;
                    case PhysicsType.TriggerVolume:
                        
                    default:
                        return null;
                }
            }
        }

        public Matrix TransformMatrix
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).WorldTransform;
                    case PhysicsType.StaticMesh:
                        return ((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform.Matrix;
                    case PhysicsType.None:
                        return rotation * Matrix.CreateTranslation(position);
                    case PhysicsType.Character:
                        return characterController.Body.WorldTransform;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.WorldTransform;
                    case PhysicsType.TriggerVolume:
                        return Matrix.CreateTranslation(position);
                    default:
                        return Matrix.Identity;
                }
            }

            set
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        ((BEPUphysics.Entities.Entity)spaceObject).WorldTransform = value;
                        break;
                    case PhysicsType.StaticMesh:
                        ((BEPUphysics.Collidables.StaticMesh)spaceObject).WorldTransform = new BEPUphysics.MathExtensions.AffineTransform(Quaternion.CreateFromRotationMatrix(value), value.Translation);
                        break;
                    // todo: add PhysicsType.None support
                    case PhysicsType.Character:
                        characterController.Body.WorldTransform = value;
                        break;
                    case PhysicsType.CylinderCharacter:
                        cylinderCharController.Body.WorldTransform = value;
                        break;
                }
            }
        }

        public float Mass
        {
            get
            {
                switch (physicsType)
                {
                    case PhysicsType.Box:
                        return ((BEPUphysics.Entities.Entity)spaceObject).Mass;
                    case PhysicsType.Character:
                        return characterController.Body.Mass;
                    case PhysicsType.CylinderCharacter:
                        return cylinderCharController.Body.Mass;
                    default:
                        return -1.0f;
                }
            }
        }

        public bool IsStatic
        {
            get
            {
                return physicsType == PhysicsType.None || physicsType == PhysicsType.StaticMesh || Mass <= 0.0f;
            }
        }
    }
}
