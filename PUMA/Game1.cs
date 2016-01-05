using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PUMA
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public bool IsAnimated = false;

        private Vector3 position0 = new Vector3(-1, -1, -1);
        private Vector3 position1 = new Vector3(10, 10, 10);
        private Vector3 eulerRotation0 = new Vector3(0, 0, 0);
        private Vector3 eulerRotation1 = new Vector3(0, 0, 0);

        #region interface
        private int stepsNumber = 0;
        public int StepsNumber { get { return stepsNumber; } set { stepsNumber = value; ResetInterpolationsSteps(); } }
        private float animationTime = 10;
        public float AnimationTime { get { return animationTime; } set { animationTime = value; ResetScene(); } }

        public float X0 { get { return position0.X; } set { position0.X = value; ResetScene(); } }
        public float Y0 { get { return position0.Y; } set { position0.Y = value; ResetScene(); } }
        public float Z0 { get { return position0.Z; } set { position0.Z = value; ResetScene(); } }
        public float X1 { get { return position1.X; } set { position1.X = value; ResetScene(); } }
        public float Y1 { get { return position1.Y; } set { position1.Y = value; ResetScene(); } }
        public float Z1 { get { return position1.Z; } set { position1.Z = value; ResetScene(); } }

        public float EX0 { get { return eulerRotation0.X; } set { eulerRotation0.X = value; ResetScene(); } }
        public float EY0 { get { return eulerRotation0.Y; } set { eulerRotation0.Y = value; ResetScene(); } }
        public float EZ0 { get { return eulerRotation0.Z; } set { eulerRotation0.Z = value; ResetScene(); } }
        public float EX1 { get { return eulerRotation1.X; } set { eulerRotation1.X = value; ResetScene(); } }
        public float EY1 { get { return eulerRotation1.Y; } set { eulerRotation1.Y = value; ResetScene(); } }
        public float EZ1 { get { return eulerRotation1.Z; } set { eulerRotation1.Z = value; ResetScene(); } }
        #endregion
        public Puma Puma1;
        public QuaternionLinear QuaternionLinInterpolation;
        private Quaternion Rotation0;
        private Quaternion Rotation1;

        private BasicEffect effect;
        private BasicEffect wireframeEffect;
        private ArcBallCamera camera;
        private List<VertexPositionColor> GlobalAxisVertices = new List<VertexPositionColor>();
        private List<short> GlobalAxisIndices = new List<short>();
        private double timeElapsedFromAnimationStart = 0;

        //camera control
        private float scrollRate = 1.0f;
        private MouseState previousMouse;

        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            //todo check if not in constructor
            effect = new BasicEffect(graphics.GraphicsDevice);
            wireframeEffect = new BasicEffect(graphics.GraphicsDevice);
            camera = new ArcBallCamera(new Vector3(0f, 0f, 0f), MathHelper.ToRadians(-200), 0f, 10f, 300f, 50f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 512f);
            Puma1 = new Puma(graphics.GraphicsDevice, 2, 2, 2, 2);
            InitializeGlobalAxis(4);

            base.Initialize();
        }
        private void InitializeGlobalAxis(int size)
        {
            //x
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(size, 0, 0), Color.Red));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Red));
            //z
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, size, 0), Color.Blue));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue));
            //y
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, size), Color.Green));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Green));

            for (int i = 0; i < GlobalAxisVertices.Count; i++)
            {
                GlobalAxisIndices.Add((short)i);
            }
        }
        protected override void LoadContent()
        {
            Rotation0 = Utility.EulerToQuaternion(MathHelper.ToRadians(-eulerRotation0.Z), MathHelper.ToRadians(eulerRotation0.Y), MathHelper.ToRadians(-eulerRotation0.X));
            Rotation1 = Utility.EulerToQuaternion(MathHelper.ToRadians(-eulerRotation1.Z), MathHelper.ToRadians(eulerRotation1.Y), MathHelper.ToRadians(-eulerRotation1.X));
            //Rotation0 = Utility.EulerToQuaternion(MathHelper.ToRadians(eulerRotation0.X), MathHelper.ToRadians(eulerRotation0.Y), MathHelper.ToRadians(eulerRotation0.Z));
            //Rotation1 = Utility.EulerToQuaternion(MathHelper.ToRadians(eulerRotation1.X), MathHelper.ToRadians(eulerRotation1.Y), MathHelper.ToRadians(eulerRotation1.Z));
            QuaternionLinInterpolation = new QuaternionLinear(graphics.GraphicsDevice, position0, position1, Rotation0, Rotation1);

            defaultViewport = GraphicsDevice.Viewport;
            leftViewport = defaultViewport;
            rightViewport = defaultViewport;
            leftViewport.Width = leftViewport.Width / 2;
            rightViewport.Width = rightViewport.Width / 2;
            rightViewport.X = leftViewport.Width;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            //CAMERA
            if (keyState.IsKeyDown(Keys.W))
            {
                camera.Elevation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                camera.Elevation += MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                camera.Rotation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                camera.Rotation += MathHelper.ToRadians(2);
            }
            if (mouseState.ScrollWheelValue - previousMouse.ScrollWheelValue != 0)
            {
                float wheelChange = mouseState.ScrollWheelValue - previousMouse.ScrollWheelValue;
                camera.ViewDistance -= (wheelChange / 120) * scrollRate;
            }
            previousMouse = mouseState;
            base.Update(gameTime);
        }

        public void ResetScene()
        {
            Rotation0 = Utility.EulerToQuaternion(MathHelper.ToRadians(-eulerRotation0.Z), MathHelper.ToRadians(eulerRotation0.Y), MathHelper.ToRadians(-eulerRotation0.X));
            Rotation1 = Utility.EulerToQuaternion(MathHelper.ToRadians(-eulerRotation1.Z), MathHelper.ToRadians(eulerRotation1.Y), MathHelper.ToRadians(-eulerRotation1.X));
            //Rotation0 = Utility.EulerToQuaternion(MathHelper.ToRadians(eulerRotation0.X), MathHelper.ToRadians(eulerRotation0.Y), MathHelper.ToRadians(eulerRotation0.Z));
            //Rotation1 = Utility.EulerToQuaternion(MathHelper.ToRadians(eulerRotation1.X), MathHelper.ToRadians(eulerRotation1.Y), MathHelper.ToRadians(eulerRotation1.Z));
            //TODO: check if should not unload and then load content again

            QuaternionLinInterpolation = new QuaternionLinear(graphics.GraphicsDevice, position0, position1, Rotation0, Rotation1);

            timeElapsedFromAnimationStart = 0;
            ResetInterpolationsSteps();
        }
        private void ResetInterpolationsSteps()
        {
            QuaternionLinInterpolation.ResetSteps(StepsNumber);
        }
        private void DrawAxis(ArcBallCamera camera, Effect wireframeEffect)
        {
            foreach (EffectPass pass in wireframeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, GlobalAxisVertices.ToArray(), 0, GlobalAxisVertices.Count, GlobalAxisIndices.ToArray(), 0, GlobalAxisIndices.Count / 2);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.VertexColorEnabled = true;
            effect.EnableDefaultLighting();
            effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
            wireframeEffect.World = Matrix.Identity;
            wireframeEffect.View = camera.View;
            wireframeEffect.Projection = camera.Projection;
            wireframeEffect.VertexColorEnabled = true;

            if (IsAnimated)
                timeElapsedFromAnimationStart += gameTime.ElapsedGameTime.TotalMilliseconds;

            //LEFT VIEWPORT
            GraphicsDevice.Viewport = leftViewport;
            DrawAxis(camera, wireframeEffect);
            Puma1.DrawStage(camera, effect);

            //RIGHT VIEWPORT
            GraphicsDevice.Viewport = rightViewport;
            DrawAxis(camera, wireframeEffect);
            //QuaternionLinInterpolation.Draw(camera, effect, wireframeEffect, timeElapsedFromAnimationStart, AnimationTime, IsAnimated);
            //QuaternionLinInterpolation.DrawStages(camera, effect, wireframeEffect);
            base.Draw(gameTime);
        }
    }
}
