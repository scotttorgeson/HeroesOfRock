//using BEPUphysics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using System;
//using System.Diagnostics;
//using GameLib.Engine.AttackSystem;
//using BEPUphysics.Collidables.MobileCollidables;
//using Microsoft.Xna.Framework.Graphics;


//// Agent that attaches to the player's character. Reads input, plays sound when you move, controls the player physics.
//// Modified from BEPUphysics CharacterControllerInput. Likely should be renamed to GuitarController or GuitarAvatar or something.

//namespace GameLib
//{
//    /// <summary>
//    /// Handles input and movement of a character in the game.
//    /// Acts as a simple 'front end' for the bookkeeping and math of the character controller.
//    /// </summary>
//    public class CharacterControllerInput : Agent
//    {
//        public static Actor Player { get; private set; }

//        /// <summary>
//        /// Owning space of the character.
//        /// </summary>
//        public Space Space { get; private set; }

//        //input
//        InputAction moveRight;
//        InputAction jump;
//        InputAction attack;
//        InputAction strum;
//        InputAction Green;
//        InputAction Red;
//        InputAction Yellow;
//        InputAction Blue;
//        InputAction Orange;
//        InputAction rightBumper;
//        InputAction triggers;

//        //switching control schemes
//        InputAction One;
//        InputAction Two;
//        InputAction Three;
//        InputAction Four;

//        //attack logic control
//        bool shouldDash;
//        bool shouldMove;
//        bool stunned;
//        Move currentAttack;
//        bool attacking;
//        float dashDelay;
//        float recover;

//        float lockOnRange;
//        const float DASH_DELAY_TIME = 0.1f;
//        const float SLOW_DELTA = 50;

//        float mashSmashCount;
//        const float MASH_DAMAGE_DELTA = 0.5f;
//        float damage;

//        //public Flow flowMechanic;
//        private float dashFlowWindow;

//        Engine.AttackSystem.ComboSystem guitarCombo;

//        public PlayerDirection facing;
//        private DashAttack dashAttack;
//        private bool jumpDash = false;

//        public static Quaternion faceForward = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 0, 0));
//        public static Quaternion faceBackward = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 3.14f, 0));
//        public static Quaternion faceLeft = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 4.71f, 0));
//        public static Quaternion faceRight = GameLib.Engine.AI.AIQB.EulerToQuaternion(new Vector3(0, 1.57f, 0));

//        public static float track;

//        PlayerAnimationAgent playerAnimation;

//        private PlayerState state;


//        public PlayerDirection MovePlayerDirection;
//        private float motionDuration;

//        public CharacterControllerInput(Actor actor)
//            : base(actor)
//        {
//            this.Name = "CharacterControllerInput";
//            actor.RegisterUpdateFunction(Update);
//            actor.RegisterDeathFunction(OnPlayerDeath);
//            Player = actor;

//            respawnRotation = Player.PhysicsObject.Orientation;
//            respawnPosition = Player.PhysicsObject.Position;
//        }

//        static Vector3 respawnPosition;
//        static Quaternion respawnRotation;
//        public static void SetRespawnPosition()
//        {
//            respawnRotation = Player.PhysicsObject.Orientation;
//            respawnPosition = Player.PhysicsObject.Position;

//            switch (Player.GetAgent<CharacterControllerInput>().MovePlayerDirection)
//            {
//                case PlayerDirection.Left:
//                    respawnPosition.X += 10.0f;
//                    break;
//                case PlayerDirection.Right:
//                    respawnPosition.X -= 10.0f;
//                    break;
//                case PlayerDirection.Backward:
//                    respawnPosition.Z -= 10.0f;
//                    break;
//                case PlayerDirection.Forward:
//                    respawnPosition.Z += 10.0f;
//                    break;
//                case PlayerDirection.Up:
//                    respawnPosition.Y -= 10.0f;
//                    break;
//                case PlayerDirection.Down:
//                    respawnPosition.Y += 10.0f;
//                    break;
//            }
//        }

//        public void OnPlayerDeath()
//        {
//            Player.Revive(ref respawnPosition, ref respawnRotation);
//        }

//        public override void Initialize(Stage stage)
//        {
//            track = 0.0f;

//            Space = stage.GetQB<PhysicsQB>().Space;

//            if (Player.PhysicsObject.physicsType == PhysicsObject.PhysicsType.Character)
//            {
//                Player.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.playerGroup;
//                if (Player.Parm.HasParm("DashSpeed"))
//                    Player.PhysicsObject.CharacterController.DashSpeed = Player.Parm.GetFloat("DashSpeed");
//                if (Player.Parm.HasParm("DashTime"))
//                    Player.PhysicsObject.CharacterController.DashTime = Player.Parm.GetFloat("DashTime");
//            }
//            lockOnRange = 1.0f;
//            if (Player.Parm.HasParm("LockOnRange"))
//                lockOnRange = Player.Parm.GetFloat("LockOnRange");
//            dashFlowWindow = .5f;
//            if (Player.Parm.HasParm("DashFlowWindow"))
//                dashFlowWindow = Player.Parm.GetFloat("DashFlowWindow");

//            dashAttack = new DashAttack(actor);

//            ControlsQB controlsQB = stage.GetQB<ControlsQB>();
//            moveRight = controlsQB.GetInputAction("GuitarMoveRight");
//            jump = controlsQB.GetInputAction("Jump");
//            attack = controlsQB.GetInputAction("GuitarAttack");

//            //guitar
//            strum = controlsQB.GetInputAction("Strum");
//            Green = controlsQB.GetInputAction("A");
//            Red = controlsQB.GetInputAction("B");
//            Yellow = controlsQB.GetInputAction("Y");
//            Blue = controlsQB.GetInputAction("X");
//            Orange = controlsQB.GetInputAction("LeftBumper");
//            rightBumper = controlsQB.GetInputAction("RightBumper");
//            triggers = controlsQB.GetInputAction("Triggers");

//            //switch control scheme
//            One = controlsQB.GetInputAction("One");
//            Two = controlsQB.GetInputAction("Two");
//            Three = controlsQB.GetInputAction("Three");
//            Four = controlsQB.GetInputAction("Four");

//            //CameraQB = new CharacterChaseCamera(Vector3.Zero, 10.0f, 0.1f, 0.0f, Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1280.0f / 720.0f, .1f, 10000), actor.PhysicsObject.CharacterController, new Vector3(0.0f, 10.0f, 0.0f), false, 45);
//            //Stage.ActiveStage.GetQB<CameraQB>().SetAvatar(actor);
//            MovePlayerDirection = PlayerDirection.Right;

//            guitarCombo = new Engine.AttackSystem.ComboSystem("MoveList");

//            recover = 0.0f;
//            dashDelay = 0.0f;
//            attacking = false;
//            currentAttack = null;
//            stunned = false;

//            facing = PlayerDirection.Right;
//            shouldMove = true;
//            shouldDash = false;

//            CameraQB cameraQB = stage.GetQB<CameraQB>();
//            PlayerCamera playerCamera = new PlayerCamera(actor.PhysicsObject, CameraQB.DefaultProjectionMatrix);
//            playerCamera.Reset();
//            cameraQB.JumpToCamera(playerCamera);

//            playerAnimation = actor.GetAgent<PlayerAnimationAgent>();
//            guitarBone = actor.model.AnimatedModel.FindBone("L_Finger02_Joint", true);

//            //particleEmitter = new Engine.Particles.ParticleEmitter(stage.GetQB<Engine.Particles.ParticleQB>(), null, actor.PhysicsObject.Position, false, 20, 40, .1f, .1f, Vector2.One, Vector2.One * 2.0f, Vector3.Zero, Vector3.Zero, 2 * Vector3.One, Stage.Content.Load<Texture2D>("ParticleFX/blood2"));
//            //stage.GetQB<Engine.Particles.ParticleQB>().AddParticleEmitter(particleEmitter);
//            // flowMechanic = new Flow(this.actor);
//            damage = 0.0f;

//            track = actor.PhysicsObject.Position.Z;
//        }

//        Bone guitarBone;
//        private Engine.Particles.ParticleEmitter particleEmitter;

//        float attackAnimationDuration = 0.7f;
//        float attackAnimationTimer;
//        PlayerAnimationAgent.AnimationTypes attackAnimationType;

//        /// <summary>
//        /// Handles the input and movement of the character.
//        /// </summary>
//        /// <param name="dt">Time since last frame in simulation seconds.</param>
//        /// <param name="previousKeyboardInput">The last frame's keyboard state.</param>
//        /// <param name="keyboardInput">The current frame's keyboard state.</param>
//        /// <param name="previousGamePadInput">The last frame's gamepad state.</param>
//        /// <param name="gamePadInput">The current frame's keyboard state.</param>
//        public void Update(float dt)
//        {
//            //particleEmitter.emitterPos = (actor.PhysicsObject.TransformMatrix + guitarBone.AbsoluteTransform).Translation;

//            myflow.Update(dt);

//            Vector2 movement = Vector2.Zero;

//            if (dashDelay > 0)
//                dashDelay -= dt;

//            //flowMechanic.updatePeriod(dt);            

//            //when recovering normal actions aren't available
//            if (recover > 0)
//            {
//                recover -= dt;
//                //decelerate the speed of the character when they start an attack
//                if (!actor.PhysicsObject.CharacterController.Dashing && actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed > 0)
//                {
//                    //using a temp because the speed value cannot be negative
//                    float temp = actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed - dt * SLOW_DELTA;
//                    if (temp < 0)
//                        temp = 0;
//                    actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed = temp;
//                }

//                if (state == PlayerState.AttackDash)
//                {
//                    actor.PhysicsObject.CharacterController.ContinueDash();
//                    if (dashAttack.ReachedTarget())
//                    {
//                        actor.PhysicsObject.CharacterController.FinishDash();
//                        state = PlayerState.Normal;
//                    }
//                }
//                else if (currentAttack != null)
//                {
//                    if (currentAttack.Name == "Smash")
//                    {
//                        if (isSmashingAttackInput())
//                        {
//                            mashSmashCount++;
//                            damage += currentAttack.Damage * MASH_DAMAGE_DELTA / mashSmashCount;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (attacking)
//                    finishAttack();
//                if (state == PlayerState.Dashing || state == PlayerState.AttackDash)
//                {
//                    state = PlayerState.Normal;
//                    //if(flowMechanic.isFlowing()) flowMechanic.setTimers(0,dashFlowWindow);
//                }
//                else
//                {

//                    if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed > 0 && motionDuration <= 0.0f)
//                    {
//                        //using a temp because the speed value cannot be negative
//                        float temp = actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed - dt * SLOW_DELTA;
//                        if (temp < 0)
//                            temp = 0;
//                        actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed = temp;
//                    }

//                    stunned = false;

//                    //get the control scheme - eventually these will be preset and we won't search these keys ever again!
//                    GamePadType gamePadType = Stage.ActiveStage.GetQB<ControlsQB>().GetGamePadType();
//                    ControlsQB.controlSchemEnum controlScheme = Stage.ActiveStage.GetQB<ControlsQB>().controlScheme;

//                    //HACKED IN
//                    if (gamePadType == GamePadType.Unknown)
//                    {
//                        if (controlScheme == ControlsQB.controlSchemEnum.NULL)
//                            controlScheme = ControlsQB.controlSchemEnum.GamePad;

//                    }

//                    if (One.value != 0.0f)
//                        controlScheme = ControlsQB.controlSchemEnum.Guitar_buttonAttack_strumMove;
//                    if (Two.value != 0.0f)
//                        controlScheme = ControlsQB.controlSchemEnum.Guitar_buttonStrumAttacks_strumMove;
//                    if (Three.value != 0.0f)
//                        controlScheme = ControlsQB.controlSchemEnum.Guitar_buttonStrumAll;
//                    if (Four.value != 0.0f)
//                        controlScheme = ControlsQB.controlSchemEnum.GamePad;

//                    Stage.ActiveStage.GetQB<ControlsQB>().controlScheme = controlScheme;

//                    //get attack and movement input
//                    int motion = 0;
//                    switch (controlScheme)
//                    {
//                        case ControlsQB.controlSchemEnum.Guitar_buttonAttack_strumMove:
//                            controlScheme_buttonAttack_strumMove(dt, ref currentAttack, ref motion);
//                            break;
//                        case ControlsQB.controlSchemEnum.Guitar_buttonStrumAttacks_strumMove:
//                            controlScheme_buttonStrumAttack_strumMove(dt, ref currentAttack, ref motion);
//                            break;
//                        case ControlsQB.controlSchemEnum.Guitar_buttonStrumAll:
//                            controlScheme_buttonStrumAll(dt, ref currentAttack, ref motion);
//                            break;
//                        case ControlsQB.controlSchemEnum.GamePad:
//                            controlScheme_GamePad(dt, ref currentAttack, ref motion);
//                            break;
//                        default:
//                            //TODO: use this switch as the only switch
//                            switch (gamePadType)
//                            {
//                                case GamePadType.GamePad:
//                                    controlScheme_GamePad(dt, ref currentAttack, ref motion);
//                                    break;
//                                case GamePadType.Guitar:
//                                case GamePadType.AlternateGuitar:
//                                    controlScheme_buttonStrumAttack_strumMove(dt, ref currentAttack, ref motion);
//                                    break;
//                                default:
//                                    break;
//                            }
//                            break;
//                    }

//                    movement = motionAndPlayerDirectionHandler(motion);

//                    //update the attack loop
//                    if (currentAttack != null)
//                    {
//                        //flowMechanic.updateFlow(currentAttack);
//                        startAttack();
//                        //moveCharacter(movement);
//                    }
//                    else if (shouldMove)
//                        moveCharacter(movement);
//                    if (shouldDash)
//                    {
//                        jumpDash = false;
//                        Dash(motion);
//                    }
//                }
//            }

//            if (attackAnimationTimer > 0.0f)
//            {
//                playerAnimation.PlayAnimation(attackAnimationType, attackAnimationDuration);
//                attackAnimationTimer -= dt;

//                if (attackAnimationTimer <= 0.0f)
//                    attackAnimationTimer = 0.0f;
//            }
//            else if (actor.PhysicsObject.CharacterController.Dashing)
//            {
//                if (jumpDash)
//                    playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Jump, actor.PhysicsObject.CharacterController.DashingTime);
//                else
//                    playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Dash, actor.PhysicsObject.CharacterController.DashingTime);
//            }
//            else if (movement.LengthSquared() > 0.1f)
//            {
//                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Walk, 0.9f);
//            }
//            else
//            {
//                playerAnimation.PlayAnimation(PlayerAnimationAgent.AnimationTypes.Idle, 2.25f);
//            }
//        }

//        public void stun(float stun)
//        {
//            if (!stunned)
//            {
//                recover = stun;
//                currentAttack = null;
//                attacking = false;
//                stunned = true;
//                Player.PhysicsObject.CharacterController.Body.LinearVelocity = Vector3.Zero;
//            }
//        }

//        public void stunIndefinitely()
//        {
//            recover = float.PositiveInfinity;
//            currentAttack = null;
//            attacking = false;
//            stunned = true;
//            Player.PhysicsObject.CharacterController.Body.LinearVelocity = Vector3.Zero;
//        }

//        public void unStun()
//        {
//            recover = 0;
//        }

//        public float TimeLeftInCurrentAction()
//        {
//            return recover;
//        }

//        public float TimeForCurrentAttack()
//        {
//            if (currentAttack == null) return 0;

//            /* REMOVE
//            if (currentAttack.AnimationSecond != PlayerAnimationAgent.AnimationTypes.None)
//            {
//                return currentAttack.TimeBeforeSecond + currentAttack.TimeBeforeAttack + currentAttack.TimeAfterAttack;
//            }
//            */

//            return currentAttack.TimeBeforeAttack + currentAttack.TimeAfterAttack;
//        }

//        public float TimeToFlow()
//        {
//            if (currentAttack == null)
//                return 0;

//            return currentAttack.FlowStart + currentAttack.TimeAfterAttack + currentAttack.TimeBeforeAttack;
//        }

//        private void finishAttack()
//        {
//            //make the character move a little towards attacking PlayerDirection
//            this.actor.PhysicsObject.CharacterController.Body.LinearVelocity = Vector3.Zero;

//            float forceValue = currentAttack.Movement * 10;

//            Vector3 force = Vector3.Zero;
//            switch (facing)
//            {
//                case PlayerDirection.Left:
//                    force.X = -forceValue;
//                    break;
//                case PlayerDirection.Right:
//                    force.X = forceValue;
//                    break;
//                case PlayerDirection.Forward:
//                    force.Z = forceValue;
//                    break;
//                case PlayerDirection.Backward:
//                    force.Z = -forceValue;
//                    break;
//            }

//            //this.actor.PhysicsObject.CharacterController.Body.ApplyLinearImpulse(ref force);

//            Stage.ActiveStage.GetQB<PlayerAttackSystemQB>().PerformAttack(actor.PhysicsObject.Position, facing, currentAttack);

//            /*
//            if (currentAttack.AnimationSecond != PlayerAnimationAgent.AnimationTypes.None) //double attack case
//            {
//                attackAnimationDuration = currentAttack.TimeBeforeAttack + currentAttack.TimeAfterAttack;
//                playerAnimation.PlayAnimation(currentAttack.AnimationType, attackAnimationDuration);
//                attacking = true;
//                recover = currentAttack.TimeBeforeAttack;
//                currentAttack.AnimationSecond = PlayerAnimationAgent.AnimationTypes.None;
//                attackAnimationTimer = attackAnimationDuration;
//                attackAnimationType = currentAttack.AnimationType;
//            }
//            */
//            //flowMechanic.setTimers(currentAttack.FlowStart + currentAttack.TimeAfterAttack, currentAttack.FlowDuration);
//            attacking = false;
//            //recover = currentAttack.TimeAfterAttack;
//            currentAttack = null;
//            damage = 0.0f;

//            AudioQB aqb = Stage.ActiveStage.GetQB<AudioQB>();
//            aqb.PlaySoundInstance("SuperPunchMMA", false, true);
//        }

//        public MyFlow myflow = new MyFlow();

//        private void startAttack()
//        {
//            mashSmashCount = 0;
//            damage = currentAttack.Damage;

//            myflow.DidAttack(currentAttack.TimeBeforeAttack);

//            //check if we are flowing
//            float timeBefore = currentAttack.TimeBeforeAttack / myflow.attack_speed;

//            //set the appropriate timers
//            attackAnimationDuration = timeBefore + (currentAttack.TimeAfterAttack / myflow.attack_speed);
//            recover = timeBefore;

//            //reset the animation type to ensure it starts from the initial position
//            attackAnimationType = currentAttack.AnimationType;
//            playerAnimation.resetAnimation(attackAnimationType);
//            playerAnimation.PlayAnimation(attackAnimationType, attackAnimationDuration);


//            /* remove this code soon!
//            //check if we are flowing
//            timeBefore = currentAttack.TimeBeforeSecond;
//            if (flowMechanic.isFlowing())
//                timeBefore *= flowMechanic.getDelta();

//            //set the appropriate timers
//            attackAnimationDuration = timeBefore;
//            recover = timeBefore;

//            //reset the animation type to ensure it starts from the initial position
//            attackAnimationType = currentAttack.AnimationSecond;
//            playerAnimation.resetAnimation(attackAnimationType);
//            playerAnimation.PlayAnimation(attackAnimationType, timeBefore);
//            */



//            //set the animation timer and attacking to true
//            attackAnimationTimer = attackAnimationDuration;
//            attacking = true;
//        }

//        private Vector2 motionAndPlayerDirectionHandler(int motion)
//        {
//            if (actor.PhysicsObject.CharacterController.Dashing) return Vector2.Zero;
//            Vector3 relativeRight = Vector3.Zero;
//            Vector2 movement = Vector2.Zero;
//            //position = actor.PhysicsObject.CharacterController.Body.Position;

//            switch (MovePlayerDirection)
//            {
//                case PlayerDirection.Right:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Right;
//                    movement = motion * Vector2.UnitX;
//                    if (motion == 1)
//                        facing = PlayerDirection.Right;
//                    else if (motion == -1)
//                        facing = PlayerDirection.Left;
//                    //position.Z = track;
//                    break;
//                case PlayerDirection.Left:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Left;
//                    movement = -motion * Vector2.UnitX;
//                    if (motion == 1)
//                        facing = PlayerDirection.Left;
//                    else if (motion == -1)
//                        facing = PlayerDirection.Right;
//                    //position.Z = track;
//                    break;
//                case PlayerDirection.Up:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Up;
//                    movement = motion * new Vector2(relativeRight.X, relativeRight.Z);
//                    if (motion == 1)
//                        facing = PlayerDirection.Up;
//                    else if (motion == -1)
//                        facing = PlayerDirection.Down;
//                    break;
//                case PlayerDirection.Down:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Down;
//                    movement = -motion * new Vector2(relativeRight.X, relativeRight.Z);
//                    if (motion == 1)
//                        facing = PlayerDirection.Up;
//                    else if (motion == -1)
//                        facing = PlayerDirection.Down;
//                    break;
//                case PlayerDirection.Forward:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Forward;
//                    movement = -motion * Vector2.UnitY;
//                    if (motion == -1)
//                        facing = PlayerDirection.Forward;
//                    else if (motion == 1)
//                        facing = PlayerDirection.Backward;
//                    //position.X = track;
//                    break;
//                case PlayerDirection.Backward:
//                    relativeRight = actor.PhysicsObject.CharacterController.Body.WorldTransform.Backward;
//                    movement = motion * Vector2.UnitY;
//                    if (motion == -1)
//                        facing = PlayerDirection.Forward;
//                    else if (motion == 1)
//                        facing = PlayerDirection.Backward;
//                    //position.X = track;
//                    break;
//            }
//            FaceMovementPlayerDirection();

//            return movement;

//        }

//        private void moveCharacter(Vector2 movement)
//        {
//            if (!actor.PhysicsObject.CharacterController.Dashing)
//            {
//                actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.Speed = 16.0f;
//            }
//            actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Normalize(movement);



//            /*Vector3 pos = actor.PhysicsObject.CharacterController.Body.Position;
//            switch (MovePlayerDirection)
//            {
//                case PlayerDirection.Left:
//                    goto case PlayerDirection.Right;
//                case PlayerDirection.Right:
//                    //pos.Z = track;
//                    break;
//                case PlayerDirection.Backward:
//                    goto case PlayerDirection.Forward;
//                case PlayerDirection.Forward:
//                    break;
//            }
//            actor.PhysicsObject.CharacterController.Body.Position = pos;*/
//            //actor.PhysicsObject.CharacterController.Body.Position = position;
//        }

//        private void controlScheme_buttonAttack_strumMove(float dt, ref Move attack, ref int motion)
//        {
//            if (jump.IsNewAction)
//            {
//                jumpDash = true;
//                DashToEnemy(0);
//            }

//            shouldMove = true;
//            ButtonInput input;

//            input = attackButtonPress();
//            motion = movementGuitarButtons();

//            attack = guitarCombo.getAttack(input);
//        }

//        private void controlScheme_buttonStrumAttack_strumMove(float dt, ref Move attack, ref int motion)
//        {
//            //jump on whammy
//            if (jump.value != -1.0 && jump.value != jump.lastValue)
//            {
//                jumpDash = true;
//                DashToEnemy(0);
//            }

//            ButtonInput input;
//            input = attackGuitarButtons();

//            if (strum.IsNewAction)
//            {
//                attack = guitarCombo.getAttack(input);

//                if (input.Orange)
//                    shouldDash = true;
//                if (input.Green)
//                {
//                    jumpDash = true;
//                    DashToEnemy((int)strum.value);
//                }
//            }

//            if (!input.isSet())
//                shouldMove = true;
//            else
//                shouldMove = false;

//            motion = movementGuitarButtons();
//        }

//        private void controlScheme_buttonStrumAll(float dt, ref Move attack, ref int motion)
//        {
//            motion = motionDurationHandler(dt);

//            ButtonInput input;
//            if (jump.IsNewAction)
//            {
//                jumpDash = true;
//                DashToEnemy(0);
//            }

//            if (strum.IsNewAction)
//            {
//                input = attackGuitarChords();
//                attack = guitarCombo.getAttack(input);
//                motion = movementGuitarChords();
//                if (attack != null)
//                {
//                    shouldMove = false;

//                    if (input.Orange)
//                        motion = -1;
//                    else
//                        motion = 1;
//                }
//                else
//                    shouldMove = true;
//            }
//        }

//        private void controlScheme_GamePad(float dt, ref Move attack, ref int motion)
//        {
//            shouldMove = true;
//            //actor.PhysicsObject.CharacterController.Body.Position = position;
//            //motion += moveRight.value * new Vector2(relativeRight.X, relativeRight.Z);
//            ButtonInput input;

//            CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
//            if (cameraQB.FreeCamEnabled == false)
//            {
//                if (moveRight.value > 0)
//                    motion = 1;
//                else if (moveRight.value < 0)
//                    motion = -1;
//            }

//            //attacking
//            input = gamepadAttack();
//            attack = guitarCombo.getAttack(input);

//        }

//        /// <summary>
//        /// dash in a direcion
//        /// </summary>
//        /// <param name="dir">the PlayerDirection to dash in -1 if left, 1 if right, 0 if the current facing PlayerDirection</param>
//        public void Dash(int dir)
//        {
//            if (dashDelay <= 0.0f)
//            {
//                shouldDash = false;
//                dashDelay = actor.PhysicsObject.CharacterController.DashTime + DASH_DELAY_TIME;
//                recover = actor.PhysicsObject.CharacterController.DashTime;

//                Vector2 dashPlayerDirection = Vector2.Zero;

//                if (dir == 0) //dash in the PlayerDirection we are facing
//                {
//                    switch (facing)
//                    {
//                        case PlayerDirection.Right:
//                            dashPlayerDirection = Vector2.UnitX;
//                            break;
//                        case PlayerDirection.Left:
//                            dashPlayerDirection = -Vector2.UnitX;
//                            break;
//                        case PlayerDirection.Forward:
//                            dashPlayerDirection = Vector2.UnitY;
//                            break;
//                        case PlayerDirection.Backward:
//                            dashPlayerDirection = -Vector2.UnitY;
//                            break;

//                    }
//                }
//                else //go whichever PlayerDirection we were given
//                {
//                    switch (MovePlayerDirection)
//                    {
//                        case PlayerDirection.Right:
//                            dashPlayerDirection = dir * Vector2.UnitX;
//                            facing = (dir > 0) ? PlayerDirection.Right : PlayerDirection.Left;
//                            break;
//                        case PlayerDirection.Left:
//                            dashPlayerDirection = -dir * Vector2.UnitX;
//                            facing = (dir > 0) ? PlayerDirection.Left : PlayerDirection.Right;
//                            break;
//                        case PlayerDirection.Forward:
//                            dashPlayerDirection = -dir * Vector2.UnitY;
//                            facing = (dir > 0) ? PlayerDirection.Backward : PlayerDirection.Forward;
//                            break;
//                        case PlayerDirection.Backward:
//                            dashPlayerDirection = dir * Vector2.UnitY;
//                            facing = (dir > 0) ? PlayerDirection.Forward : PlayerDirection.Backward;
//                            break;
//                    }
//                }
//                FaceMovementPlayerDirection();
//                actor.PhysicsObject.CharacterController.Dash(dashPlayerDirection);
//            }
//        }

//        public void DashToEnemy(int dir)
//        {
//            if (dashDelay <= 0.0f)
//            {
//                //if(flowMechanic.isFlowing()) flowMechanic.setPeriod(float.PositiveInfinity);
//                shouldDash = false;
//                //dashDelay = actor.PhysicsObject.CharacterController.DashTime + DASH_DELAY_TIME;

//                Vector2 dashPlayerDirection = Vector2.Zero;

//                //try to find an enemy in the PlayerDirection we were given
//                GameLib.Engine.AI.AIQB aiQB = Stage.ActiveStage.GetQB<GameLib.Engine.AI.AIQB>();

//                Vector3 myPos = actor.PhysicsObject.Position;

//                if (dir == 0) //dash in the PlayerDirection we are facing
//                {
//                    switch (facing)
//                    {
//                        case PlayerDirection.Right:
//                            dashPlayerDirection = Vector2.UnitX;
//                            break;
//                        case PlayerDirection.Left:
//                            dashPlayerDirection = -Vector2.UnitX;
//                            break;
//                        case PlayerDirection.Forward:
//                            dashPlayerDirection = Vector2.UnitY;
//                            break;
//                        case PlayerDirection.Backward:
//                            dashPlayerDirection = -Vector2.UnitY;
//                            break;

//                    }
//                }
//                else //go whichever PlayerDirection we were given
//                {
//                    switch (MovePlayerDirection)
//                    {
//                        case PlayerDirection.Right:
//                            dashPlayerDirection = dir * Vector2.UnitX;
//                            facing = (dir > 0) ? PlayerDirection.Right : PlayerDirection.Left;
//                            break;
//                        case PlayerDirection.Left:
//                            dashPlayerDirection = -dir * Vector2.UnitX;
//                            facing = (dir > 0) ? PlayerDirection.Left : PlayerDirection.Right;
//                            break;
//                        case PlayerDirection.Forward:
//                            dashPlayerDirection = -dir * Vector2.UnitY;
//                            facing = (dir > 0) ? PlayerDirection.Backward : PlayerDirection.Forward;
//                            break;
//                        case PlayerDirection.Backward:
//                            dashPlayerDirection = dir * Vector2.UnitY;
//                            facing = (dir > 0) ? PlayerDirection.Forward : PlayerDirection.Backward;
//                            break;
//                    }
//                }

//                FaceMovementPlayerDirection();

//                aiQB.FindClosestEnemyInDir(facing, ref myPos, ref dashAttack, lockOnRange);

//                if (dashAttack.Target != null)
//                {

//                    if (dashAttack.Dist > lockOnRange)
//                    {
//                        state = PlayerState.AttackDash;

//                        recover = actor.PhysicsObject.CharacterController.DashDistOverTime(dashPlayerDirection, dashAttack.Dist);
//                        dashDelay = recover + DASH_DELAY_TIME;

//                        //recover = actor.PhysicsObject.CharacterController.DashTime + currentAttack.TimeBeforeAttack;
//                    }
//                    else //dodge in place
//                    {
//                        actor.PhysicsObject.CharacterController.Dash(Vector2.Zero);
//                        dashDelay = actor.PhysicsObject.CharacterController.DashTime + DASH_DELAY_TIME;
//                        recover = actor.PhysicsObject.CharacterController.DashTime;
//                    }
//                }
//                else
//                {
//                    //there is no enemy in that PlayerDirection so do a normal dash
//                    actor.PhysicsObject.CharacterController.Dash(dashPlayerDirection);
//                    dashDelay = actor.PhysicsObject.CharacterController.DashTime + DASH_DELAY_TIME;
//                    recover = actor.PhysicsObject.CharacterController.DashTime;
//                }

//                //set the flow timers
//                //flowMechanic.setTimers(recover, 0.3f);
//            }
//        }

//        private void FaceMovementPlayerDirection()
//        {
//            switch (facing)
//            {
//                case PlayerDirection.Left:
//                    actor.PhysicsObject.CharacterController.Body.Orientation = faceLeft;
//                    break;
//                case PlayerDirection.Right:
//                    actor.PhysicsObject.CharacterController.Body.Orientation = faceRight;
//                    break;
//                case PlayerDirection.Backward:
//                    actor.PhysicsObject.CharacterController.Body.Orientation = faceBackward;
//                    break;
//                case PlayerDirection.Forward:
//                    actor.PhysicsObject.CharacterController.Body.Orientation = faceForward;
//                    break;
//            }
//        }

//        public void FaceActor(ref Actor a)
//        {
//            if (a == null || a.IsShutdown || a.MarkedForDeath) return;

//            switch (MovePlayerDirection)
//            {
//                case PlayerDirection.Left:
//                    if (a.PhysicsObject.Position.X < actor.PhysicsObject.Position.X)
//                        facing = PlayerDirection.Left;
//                    else
//                        facing = PlayerDirection.Right;
//                    break;
//                case PlayerDirection.Right:
//                    if (a.PhysicsObject.Position.X < actor.PhysicsObject.Position.X)
//                        facing = PlayerDirection.Left;
//                    else
//                        facing = PlayerDirection.Right;
//                    break;
//                case PlayerDirection.Backward:
//                    if (a.PhysicsObject.Position.Z < actor.PhysicsObject.Position.Z)
//                        facing = PlayerDirection.Forward;
//                    else
//                        facing = PlayerDirection.Backward;
//                    break;
//                case PlayerDirection.Forward:
//                    if (a.PhysicsObject.Position.Z < actor.PhysicsObject.Position.Z)
//                        facing = PlayerDirection.Backward;
//                    else
//                        facing = PlayerDirection.Forward;
//                    break;
//            }

//            FaceMovementPlayerDirection();
//        }

//        public void ChangeMovePlayerDirection(PlayerDirection newPlayerDirection, bool rotateRight, float timeForRotation)
//        {
//            CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
//            //stun(timeForRotation);

//            //find the new yaw based on the new PlayerDirection
//            switch (newPlayerDirection)
//            {
//                case PlayerDirection.Left:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, -20.0f);
//                    track = actor.PhysicsObject.Position.Z;
//                    break;
//                case PlayerDirection.Right:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, 20.0f);
//                    track = actor.PhysicsObject.Position.Z;
//                    break;
//                case PlayerDirection.Forward:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(20.0f, 3.0f, 0.0f);
//                    track = actor.PhysicsObject.Position.X;
//                    break;
//                case PlayerDirection.Backward:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(-20.0f, 3.0f, 0.0f);
//                    track = actor.PhysicsObject.Position.X;
//                    break;
//                case PlayerDirection.Up:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, -20.0f);
//                    break;
//                case PlayerDirection.Down:
//                    (cameraQB.ActiveCamera as PlayerCamera).DesiredPositionOffset = new Vector3(0.0f, 3.0f, 20.0f);
//                    break;
//            }

//            MovePlayerDirection = newPlayerDirection;
//        }

//        private int motionDurationHandler(float dt)
//        {
//            //i want to try a system with continuous strumming to move forward

//            if (motionDuration > 0.0f)
//            {
//                motionDuration -= dt;
//                switch (GameLib.Engine.AI.AIQB.MoveDirection)
//                {
//                    case PlayerDirection.Right:
//                        if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.X > 0)
//                            return 1;
//                        else if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.X < 0)
//                            return -1;
//                        break;
//                    case PlayerDirection.Left:
//                        if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.X > 0)
//                            return -1;
//                        else if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.X < 0)
//                            return 1;
//                        break;
//                    case PlayerDirection.Forward:
//                        if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.Y < 0)
//                            return 1;
//                        else if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.Y > 0)
//                            return -1;
//                        break;
//                    case PlayerDirection.Backward:
//                        if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.Y < 0)
//                            return -1;
//                        else if (actor.PhysicsObject.CharacterController.HorizontalMotionConstraint.MovementDirection.Y > 0)
//                            return 1;
//                        break;
//                }
//            }
//            return 0;
//        }

//        private ButtonInput attackGuitarChords()
//        {
//            //rethink how this works... could just grab each input...
//            ButtonInput inputs = new ButtonInput();
//            if (Green.value == 0.0f)
//            {
//                if (Red.value != 0.0f)
//                {
//                    inputs.Red = true;
//                }
//                if (Yellow.value != 0.0f)
//                {
//                    inputs.Yellow = true;
//                }
//                if (Blue.value != 0.0f)
//                {
//                    inputs.Blue = true;
//                }
//                if (Orange.value != 0.0f)
//                {
//                    inputs.Orange = true;
//                }
//            }

//            return inputs;
//        }

//        private ButtonInput attackGuitarButtons()
//        {
//            Engine.AttackSystem.ButtonInput inputs = new Engine.AttackSystem.ButtonInput();

//            if (Green.value != 0.0f)
//            {
//                //actor.PhysicsObject.CharacterController.Jump();
//                inputs.Green = true;
//            }
//            else if (Orange.value != 0.0f)
//            {
//                inputs.Orange = true;
//            }
//            else
//            {
//                if (Red.value != 0.0f)
//                {
//                    inputs.Red = true;
//                }

//                if (Yellow.value != 0.0f)
//                {
//                    inputs.Yellow = true;
//                }

//                if (Blue.value != 0.0f)
//                {
//                    inputs.Blue = true;
//                }
//            }

//            return inputs;
//        }

//        private ButtonInput gamepadAttack()
//        {
//            ButtonInput inputs = new ButtonInput();
//            if (Orange.IsNewAction)
//            {
//                jumpDash = false;
//                Dash(-1);
//            }
//            else if (rightBumper.IsNewAction)
//            {
//                jumpDash = false;
//                Dash(1);
//            }
//            else if (Green.IsNewAction)
//            {
//                jumpDash = true;
//                DashToEnemy(0);
//            }
//            else if (Red.IsNewAction)
//            {
//                //weak
//                inputs.Red = true;
//            }
//            else if (triggers.IsNewAction)
//            {
//                //super
//                inputs.Red = true;
//                inputs.Blue = true;
//                inputs.Yellow = true;
//            }

//            else if (Yellow.IsNewAction)
//            {
//                //heavy
//                inputs.Blue = true;
//            }

//            else if (Blue.IsNewAction)
//            {
//                //medium attack
//                inputs.Yellow = true;
//            }
//            return inputs;
//        }

//        private ButtonInput attackButtonPress()
//        {

//            ButtonInput inputs = new ButtonInput();

//            if (Green.IsNewAction)
//            {
//                jumpDash = true;
//                DashToEnemy(0);
//            }

//            if (Red.IsNewAction)
//            {
//                inputs.Red = true;
//            }

//            else if (Yellow.IsNewAction)
//            {
//                inputs.Yellow = true;
//            }

//            else if (Blue.IsNewAction)
//            {
//                inputs.Blue = true;
//            }

//            else if (Orange.IsNewAction)
//            {
//                jumpDash = false;
//                Dash(0);
//            }

//            return inputs;
//        }

//        private int movementGuitarChords()
//        {
//            float time = 1.0f / 4.0f;
//            int rtn = 0;
//            if (Green.value != 0.0f)
//            {
//                motionDuration = time;

//                if (Red.value != 0.0f)
//                {
//                    shouldDash = true;
//                }

//                if (Yellow.value != 0.0f)
//                {
//                    rtn = -1;
//                }
//                else
//                {
//                    rtn = 1;
//                }
//            }
//            return rtn;
//        }

//        private int movementGuitarButtons()
//        {
//            int motion = 0;

//            if (strum.value > 0.0f)
//            {
//                motion = 1;
//            }

//            if (strum.value < 0.0f)
//            {
//                motion = -1;
//            }

//            return motion;
//        }

//        private bool isSmashingAttackInput()
//        {
//            GamePadType gamePadType = Stage.ActiveStage.GetQB<ControlsQB>().GetGamePadType();
//            ControlsQB.controlSchemEnum controlScheme = Stage.ActiveStage.GetQB<ControlsQB>().controlScheme;

//            switch (controlScheme)
//            {
//                case ControlsQB.controlSchemEnum.Guitar_buttonAttack_strumMove:
//                    break;
//                case ControlsQB.controlSchemEnum.Guitar_buttonStrumAttacks_strumMove:
//                case ControlsQB.controlSchemEnum.Guitar_buttonStrumAll:
//                    return Red.value != 0.0f && Yellow.value != 0.0f && Blue.value != 0.0f && strum.IsNewAction;
//                case ControlsQB.controlSchemEnum.GamePad:
//                    return Red.IsNewAction;
//                default:
//                    //TODO: use this switch as the only switch
//                    switch (gamePadType)
//                    {
//                        case GamePadType.GamePad:
//                            return Red.IsNewAction;
//                        case GamePadType.Guitar:
//                        case GamePadType.AlternateGuitar:
//                            return Red.value != 0.0f && Yellow.value != 0.0f && Blue.value != 0.0f && strum.IsNewAction;
//                        default:
//                            break;
//                    }
//                    break;
//            }

//            return false;
//        }

//    }
//}