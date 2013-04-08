
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using GameLib.Engine.AttackSystem;
using BEPUphysics.Collidables.MobileCollidables;
using Microsoft.Xna.Framework.Graphics;


// Agent that attaches to the player's character. Reads input, plays sound when you move, controls the player physics.
// Modified from BEPUphysics CharacterControllerInput. Likely should be renamed to GuitarController or GuitarAvatar or something.

namespace GameLib
{
    /// <summary>
    /// Handles input and movement of a character in the game.
    /// Acts as a simple 'front end' for the bookkeeping and math of the character controller.
    /// </summary>
    public class PlayerAgent : Agent
    {
        public static Actor Player { get; private set; }

        /// <summary>
        /// Owning space of the character.
        /// </summary>
        public Space Space { get; private set; }

        public PlayerState State { get; private set; }

        #region InputAction declarations
        InputAction strum;
        InputAction leftAxisX;
        InputAction jump;
        InputAction guitarJump;
        InputAction A;
        InputAction B;
        InputAction Y;
        InputAction X;
        InputAction leftBumper;
        InputAction rightBumper;
        InputAction triggers;
        #endregion

        #region combat

        Engine.AttackSystem.ComboSystem attacks;
        ButtonInput input;
        Move currentAttack;
        float recoveryTimer;
        const float MAX_SMASH = 10;
        float smash;

        //support for flow
        public Flow flow;
        #endregion

        #region Animation

        float attackAnimationTimer;
        float attackAnimationDuration;
        PlayerAnimationAgent.AnimationTypes animationType;

        PlayerAnimationAgent playerAnimation;

        float jumpTime = 0.80f;

        #endregion

        #region Direction
        /// <summary>
        /// current facing direction of the player
        /// </summary>
        public PlayerDirection facing;

        public PlayerDirection MoveDirection;

        public static Quaternion faceForward = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 3.14f, 0));
        public static Quaternion faceBackward = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 0, 0));
        public static Quaternion faceLeft = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 4.71f, 0));
        public static Quaternion faceRight = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 1.57f, 0));

        public static float track;

        #endregion

        public PlayerAgent(Actor actor)
            : base(actor)
        {
            this.Name = "PlayerAgent";
            actor.RegisterUpdateFunction(Update);
            actor.RegisterDeathFunction(OnPlayerDeath);
            Player = actor;

            respawnRotation = Player.PhysicsObject.Orientation;
            respawnPosition = Player.PhysicsObject.Position;
        }

        public override void Initialize(Stage stage)
        {
            track = 0.0f;
            track = actor.PhysicsObject.Position.Z;

            Space = stage.GetQB<PhysicsQB>().Space;


            //input actions
            ControlsQB controlsQB = stage.GetQB<ControlsQB>();

            strum = controlsQB.GetInputAction("Strum");
            A = controlsQB.GetInputAction("A");
            B = controlsQB.GetInputAction("B");
            Y = controlsQB.GetInputAction("Y");
            X = controlsQB.GetInputAction("X");
            leftBumper = controlsQB.GetInputAction("LeftBumper");
            rightBumper = controlsQB.GetInputAction("RightBumper");
            triggers = controlsQB.GetInputAction("Triggers");
            leftAxisX = controlsQB.GetInputAction("MoveRight");
            jump = controlsQB.GetInputAction("Jump");
            guitarJump = controlsQB.GetInputAction("GuitarJump");

            //set move direction
            //TODO: look this up in a parm
            MoveDirection = PlayerDirection.Right;

            //get attack list
            attacks = new Engine.AttackSystem.ComboSystem("MoveList");

            //set camera
            CameraQB cameraQB = stage.GetQB<CameraQB>();
            PlayerCamera playerCamera = new PlayerCamera(actor.PhysicsObject, CameraQB.DefaultProjectionMatrix);
            playerCamera.Reset();
            cameraQB.JumpToCamera(playerCamera);

            //animations
            playerAnimation = actor.GetAgent<PlayerAnimationAgent>();

            State = PlayerState.Normal;
            facing = PlayerDirection.Right;

            //dashing & lock on range
            if (Player.PhysicsObject.physicsType == PhysicsObject.PhysicsType.Character)
            {
                Player.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.playerGroup;
                if (Player.Parm.HasParm("DashSpeed"))
                    Player.PhysicsObject.CharacterController.DashSpeed = Player.Parm.GetFloat("DashSpeed");
                if (Player.Parm.HasParm("DashTime"))
                    Player.PhysicsObject.CharacterController.DashTime = Player.Parm.GetFloat("DashTime");
            }

            //combat
            input = new ButtonInput();
            smash = 0;

            //support for the flow mechanic
            flow = new Flow();

            //Load the player sounds here
            AudioQB AQB = Stage.ActiveStage.GetQB<AudioQB>();
            int index = 0;
            while (Player.Parm.HasParm("Sound" + index))
            {
                AQB.AddSound(Player.Parm.GetString("Sound" + index));
                index++;
            }
        }

        /// <summary>
        /// Handles the input and movement of the character.
        /// </summary>
        /// <param name="dt">Time since last frame in simulation seconds.</param>
        public void Update(float dt)
        {
            flow.Update(dt);


            if (recoveryTimer > 0.0f) //recovery is set for attacking time before and after, stun duration, and dashing duration
            {
                recoveryTimer -= dt;
                if (State == PlayerState.Attacking)
                    if (currentAttack.Name == "Smash")
                        if (isSmashInput())
                        {
                            //set the logic for repeat smashing attacks here. Increasing the orb size or whatever.
                            smash++;
                            if (smash > MAX_SMASH)
                                smash = MAX_SMASH;
                        }
            }
            else if (State == PlayerState.Attacking)
                finishAttack();
            else if (State == PlayerState.Dashing)
                State = PlayerState.Normal;
            else if (State == PlayerState.Stunned)
                State = PlayerState.Normal;
            else if (State == PlayerState.Jumping)
                State = PlayerState.Normal;
            else //get input
            {
                GamePadType gamePadType = Stage.ActiveStage.GetQB<ControlsQB>().GetGamePadType();

                if (gamePadType == GamePadType.AlternateGuitar || gamePadType == GamePadType.Guitar)
                    updateGuitarWithStrum(dt);
                else
                    updateGamePad(dt);
            }

            //Animations

            if (attackAnimationTimer > 0.0f)
            {
                playerAnimation.PlayAnimation(animationType, attackAnimationDuration);
                attackAnimationTimer -= dt;

                if (attackAnimationTimer <= 0.0f)
                    attackAnimationTimer = 0.0f;
            }
            else if (State == PlayerState.Jumping)
                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Jump, jumpTime);
            else if(actor.PhysicsObject.CharacterController.Dashing)
                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Dash, actor.PhysicsObject.CharacterController.DashTime);
            else if (State==PlayerState.Running)
                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Walk, 0.9f);
            else
                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Idle, 2.25f);

            //set player facing direction
            FaceMovementDirection();
        }
    
        private void updateGamePad(float dt)
        {
            int dir = (int)leftAxisX.value;

            SetFacingDirection(dir);
            if (dir != 0)
                State = PlayerState.Running;
            else
                State = PlayerState.Normal;

            switch (MoveDirection)
            {
                case PlayerDirection.Right:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * Vector2.UnitX;
                    break;
                case PlayerDirection.Left:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * -Vector2.UnitX;
                    break;
                case PlayerDirection.Forward:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * -Vector2.UnitY;
                    break;
                case PlayerDirection.Backward:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * Vector2.UnitY;
                    break;
            }

            if (rightBumper.IsNewAction)
                Dash(1);
            else if (leftBumper.IsNewAction)
                Dash(-1);
            else if (jump.IsNewAction)
            {
                actor.PhysicsObject.CharacterController.Jump();
                State = PlayerState.Jumping;
                recoveryTimer = actor.PhysicsObject.CharacterController.DashTime;
            }
            else
            {
                //get attack from input
                if (B.IsNewAction)
                    input.Blue = true;
                if (Y.IsNewAction)
                    input.Yellow = true;
                if (X.IsNewAction)
                    input.Red = true;

                if (triggers.IsNewAction)
                {
                    input.Red = true;
                    input.Yellow = true;
                    input.Blue = true;
                }

                if (input.isSet())
                {
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                    currentAttack = attacks.getAttack(input);
                    startAttack();
                    input.Reset();
                }
            }
        }

        private void updateGuitarWithStrum(float dt)
        {
            int dir = (int)strum.value;
            //if all the guitar face buttons are not pressed then move
            if (B.value == 0.0f && Y.value == 0.0f && X.value == 0.0f) /* A.value == 0.0f && leftBumper.value == 0.0f && )*/
            {
                SetFacingDirection(dir);

                if (dir != 0)
                    State = PlayerState.Running;
                else
                    State = PlayerState.Normal;

                switch (MoveDirection)
                {
                    case PlayerDirection.Right:
                        this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * Vector2.UnitX;
                        break;
                    case PlayerDirection.Left:
                        this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * -Vector2.UnitX;
                        break;
                    case PlayerDirection.Forward:
                        this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * -Vector2.UnitY;
                        break;
                    case PlayerDirection.Backward:
                        this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = dir * Vector2.UnitY;
                        break;
                }
            }
            else
            {
                State = PlayerState.Normal;
                this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
            }

            //jumping
            if (guitarJump.IsNewAction  || leftBumper.IsNewAction && strum.value != 0.0f || leftBumper.value != 0.0f && strum.IsNewAction)
            {
                State = PlayerState.Jumping;
                actor.PhysicsObject.CharacterController.Jump();
                recoveryTimer = jumpTime;
            }

            //holding strum and pushing dash button
            if (strum.value != 0.0f)
            {
                if (A.IsNewAction)
                    Dash(dir);
            }

            //all strumming actions
            if (strum.IsNewAction)
            {
                if (A.value != 0.0f)
                    Dash(dir);
                else
                {
                    if (B.value != 0.0f)
                        input.Red = true;
                    if (Y.value != 0.0f)
                        input.Yellow = true;
                    if (X.value != 0.0f)
                        input.Blue = true;

                    if (input.isSet())
                    {
                        currentAttack = attacks.getAttack(input);
                        startAttack();
                        input.Reset();
                    }
                }
            }
        }

        private void finishAttack()
        {
            //perform attack
            Stage.ActiveStage.GetQB<PlayerAttackSystemQB>().PerformAttack(Player.PhysicsObject.Position, facing, currentAttack, smash / MAX_SMASH);

            //set player state
            State = PlayerState.Normal;
            recoveryTimer = 0;          
            currentAttack = null;

            

        }

        private void startAttack()
        {
            //set player state
            State = PlayerState.Attacking;
            smash = 0;

            if (currentAttack.Name != "Smash")
            {
                recoveryTimer = currentAttack.TimeBeforeAttack / flow.getRate();
                attackAnimationDuration = recoveryTimer + currentAttack.TimeAfterAttack / flow.getRate();
            }
            else
            {
                recoveryTimer = currentAttack.TimeBeforeAttack;
                attackAnimationDuration = recoveryTimer + currentAttack.TimeAfterAttack;
            }

            flow.DoingAttack(recoveryTimer, true);

            attackAnimationTimer = attackAnimationDuration;
            animationType = currentAttack.AnimationType;
        }

        public void Dash(int dir)
        {
            State = PlayerState.Dashing;
            recoveryTimer = Player.PhysicsObject.CharacterController.DashTime;
            flow.DoingAttack(recoveryTimer, false);

            //play dashing sound
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Dash_16", 1.0f, 0.0f, 0.0f);

            Vector2 dashDirection = Vector2.Zero;
            if (dir == 0) //dash in the direction we are facing
            {
                switch (facing)
                {
                    case PlayerDirection.Right:
                        dashDirection = Vector2.UnitX;
                        break;
                    case PlayerDirection.Left:
                        dashDirection = -Vector2.UnitX;
                        break;
                    case PlayerDirection.Forward:
                        dashDirection = -Vector2.UnitY;
                        break;
                    case PlayerDirection.Backward:
                        dashDirection = Vector2.UnitY;
                        break;

                }
            }
            else //go whichever direction we were given
            {
                switch (MoveDirection)
                {
                    case PlayerDirection.Right:
                        dashDirection = dir * Vector2.UnitX;
                        facing = (dir > 0) ? PlayerDirection.Right : PlayerDirection.Left;
                        break;
                    case PlayerDirection.Left:
                        dashDirection = -dir * Vector2.UnitX;
                        facing = (dir > 0) ? PlayerDirection.Left : PlayerDirection.Right;
                        break;
                    case PlayerDirection.Forward:
                        dashDirection = -dir * Vector2.UnitY;
                        facing = (dir > 0) ? PlayerDirection.Forward : PlayerDirection.Backward;
                        break;
                    case PlayerDirection.Backward:
                        dashDirection = dir * Vector2.UnitY;
                        facing = (dir > 0) ? PlayerDirection.Backward : PlayerDirection.Forward;
                        break;
                }
            }
            actor.PhysicsObject.CharacterController.Dash(dashDirection);
        }

        private bool isSmashInput()
        {
            GamePadType gamePadType = Stage.ActiveStage.GetQB<ControlsQB>().GetGamePadType();

            if (gamePadType == GamePadType.AlternateGuitar || gamePadType == GamePadType.Guitar)
                return X.value != 0.0f && B.value != 0.0f && Y.value != 0.0f && strum.IsNewAction;
            else
                return triggers.IsNewAction;
        }

        public void stun(float stun)
        {
            if (State != PlayerState.Stunned)
            {
                recoveryTimer = stun;
                currentAttack = null;
                State = PlayerState.Stunned;
                Player.PhysicsObject.CharacterController.Body.LinearVelocity = Vector3.Zero;
                Player.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
            }
        }

        public void stunIndefinitely()
        {
            recoveryTimer = float.PositiveInfinity;
            currentAttack = null;
            State = PlayerState.Stunned;
            Player.PhysicsObject.CharacterController.Body.LinearVelocity = Vector3.Zero;
            Player.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
        }

        public void unStun()
        {
            recoveryTimer = 0;
        }

        public float TimeLeftInCurrentAction()
        {
            return recoveryTimer;
        }

        public float TimeForCurrentAttack()
        {
            if (currentAttack == null) return 0;
            return currentAttack.TimeBeforeAttack + currentAttack.TimeAfterAttack;
        }

        public float TimeToFlow()
        {
            //TODO
            //if (currentAttack == null)
            //return 0;

            //return currentAttack.FlowStart + currentAttack.TimeAfterAttack + currentAttack.TimeBeforeAttack;
            return 0.0f;
        }

        public void MoveToNextStopZone()
        {
            State = PlayerState.Running;
            facing = MoveDirection;
            FaceMovementDirection();
            switch (MoveDirection)
            {
                case PlayerDirection.Right:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.UnitX;
                    break;
                case PlayerDirection.Left:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = -Vector2.UnitX;
                    break;
                case PlayerDirection.Forward:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = -Vector2.UnitY;
                    break;
                case PlayerDirection.Backward:
                    this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.UnitY;
                    break;
            }
        }

        public void ReachedStopZone()
        {
            State = PlayerState.Normal;
            this.actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
        }
       
        public void ChangeMoveDirection(PlayerDirection newDirection, bool rotateRight, float timeForRotation)
        {
            CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
            stun(timeForRotation);

            switch (newDirection)
            {
                case PlayerDirection.Left:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, -20.0f);
                    track = actor.PhysicsObject.Position.Z;
                    break;
                case PlayerDirection.Right:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, 20.0f);
                    track = actor.PhysicsObject.Position.Z;
                    break;
                case PlayerDirection.Forward:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(20.0f, 3.0f, 0.0f);
                    track = actor.PhysicsObject.Position.X;
                    break;
                case PlayerDirection.Backward:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(-20.0f, 3.0f, 0.0f);
                    track = actor.PhysicsObject.Position.X;
                    break;
                case PlayerDirection.Up:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, -20.0f);
                    break;
                case PlayerDirection.Down:
                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, 20.0f);
                    break;
            }

            MoveDirection = newDirection;
            facing = MoveDirection;
            
        }

        private void FaceMovementDirection()
        {
            switch (facing)
            {
                case PlayerDirection.Left:
                    actor.PhysicsObject.CharacterController.Body.Orientation = faceLeft;
                    break;
                case PlayerDirection.Right:
                    actor.PhysicsObject.CharacterController.Body.Orientation = faceRight;
                    break;
                case PlayerDirection.Backward:
                    actor.PhysicsObject.CharacterController.Body.Orientation = faceBackward;
                    break;
                case PlayerDirection.Forward:
                    actor.PhysicsObject.CharacterController.Body.Orientation = faceForward;
                    break;
            }
        }

        private void SetFacingDirection(int dir)
        {
            switch (MoveDirection)
            {
                case PlayerDirection.Right:
                    if (dir > 0.0f)
                        facing = PlayerDirection.Right;
                    else if (dir < 0.0f)
                        facing = PlayerDirection.Left;
                    break;
                case PlayerDirection.Left:
                    if (dir > 0.0f)
                        facing = PlayerDirection.Left;
                    else if (dir < 0.0f)
                        facing = PlayerDirection.Right;
                    break;
                case PlayerDirection.Forward:
                    if (dir > 0.0f)
                        facing = PlayerDirection.Forward;
                    else if (dir < 0.0f)
                        facing = PlayerDirection.Backward;
                    break;
                case PlayerDirection.Backward:
                    if (dir > 0.0f)
                        facing = PlayerDirection.Backward;
                    else if (dir < 0.0f)
                        facing = PlayerDirection.Forward;
                    break;
            }
        }

        #region death and respawn
        static Vector3 respawnPosition;
        static Quaternion respawnRotation;
        public static void SetRespawnPosition()
        {
            respawnRotation = Player.PhysicsObject.Orientation;
            respawnPosition = Player.PhysicsObject.Position;

            switch (Player.GetAgent<PlayerAgent>().MoveDirection)
            {
                case PlayerDirection.Left:
                    respawnPosition.X += 10.0f;
                    break;
                case PlayerDirection.Right:
                    respawnPosition.X -= 10.0f;
                    break;
                case PlayerDirection.Backward:
                    respawnPosition.Z -= 10.0f;
                    break;
                case PlayerDirection.Forward:
                    respawnPosition.Z += 10.0f;
                    break;
                case PlayerDirection.Up:
                    respawnPosition.Y -= 10.0f;
                    break;
                case PlayerDirection.Down:
                    respawnPosition.Y += 10.0f;
                    break;
            }
        }

        public void OnPlayerDeath()
        {
            Player.Revive(ref respawnPosition, ref respawnRotation);
        }
        #endregion

    }

    #region enumerations
    public enum PlayerDirection
    {
        Left,
        Right,
        Forward,
        Backward,
        Up,
        Down,
    }

    public enum PlayerState
    {
        Normal,
        Walking,
        Dashing,
        Attacking,
        Stunned,
        AttackDash,
        Jumping,
        Running
    }
    #endregion

    public class DashAttack
    {
        Actor player;
        public Actor Target;
        public float Dist;
        public float AttackRange;
        public float velocity;
        public PlayerDirection Direction;

        public DashAttack(Actor p)
        {
            player = p;
        }

        public bool ReachedTarget()
        {
            float diff;
            switch (Direction)
            {
                case PlayerDirection.Right:
                    diff = Target.PhysicsObject.Position.X - player.PhysicsObject.Position.X;
                    if (Math.Abs(diff) <= AttackRange)
                        return true;
                    else if (diff < 0)
                        return true;
                    break;
                case PlayerDirection.Left:
                    diff = Target.PhysicsObject.Position.X - player.PhysicsObject.Position.X;
                    if (Math.Abs(diff) <= AttackRange)
                        return true;
                    else if (diff > 0)
                        return true;
                    break;
                case PlayerDirection.Backward:
                    diff = Target.PhysicsObject.Position.Z - player.PhysicsObject.Position.Z;
                    if (Math.Abs(diff) <= AttackRange)
                        return true;
                    else if (diff > 0)
                        return true;
                    break;
                case PlayerDirection.Forward:
                    diff = Target.PhysicsObject.Position.Z - player.PhysicsObject.Position.Z;
                    if (Math.Abs(diff) <= AttackRange)
                        return true;
                    else if (diff < 0)
                        return true;
                    break;
            }
            return false;
        }
    }
}