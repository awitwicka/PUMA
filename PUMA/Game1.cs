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
        public bool IsAnimated = true;
        public Puma Puma1;
        private BasicEffect effect;
        private BasicEffect wireframeEffect;
        private ArcBallCamera camera;
        private List<Axis> PositionAxis = new List<Axis>();
        private List<VertexPositionColor> GlobalAxisVertices = new List<VertexPositionColor>();
        private List<short> GlobalAxisIndices = new List<short>();
        private double timeElapsedFromAnimationStart = 0;

        private Vector3 position0 = new Vector3(-1, -1, -1);
        private Vector3 position1 = new Vector3(10, 10, 10);
        private Quaternion quaternionRotation0 = new Quaternion(1, 0, 0, 30);
        private Quaternion quaternionRotation1 = new Quaternion(0, 0, 1, 180);

        #region interface
        private float animationTime = 10;
        public float AnimationTime { get { return animationTime; } set { animationTime = value; ResetScene(); } }

        public float X0 { get { return position0.X; } set { position0.X = value; ResetScene(); } }
        public float Y0 { get { return position0.Z; } set { position0.Z = value; ResetScene(); } }
        public float Z0 { get { return position0.Y; } set { position0.Y = value; ResetScene(); } }
        public float X1 { get { return position1.X; } set { position1.X = value; ResetScene(); } }
        public float Y1 { get { return position1.Z; } set { position1.Z = value; ResetScene(); } }
        public float Z1 { get { return position1.Y; } set { position1.Y = value; ResetScene(); } }
        //TODO: check direction of rotation if corresponds to puma axis
        public float QX0 { get { return quaternionRotation0.X; } set { quaternionRotation0.X = value; ResetScene(); } }
        public float QY0 { get { return quaternionRotation0.Z; } set { quaternionRotation0.Z = value; ResetScene(); } }
        public float QZ0 { get { return quaternionRotation0.Y; } set { quaternionRotation0.Y = value; ResetScene(); } }
        public float QW0 { get { return quaternionRotation0.W; } set { quaternionRotation0.W = value; ResetScene(); } }
        public float QX1 { get { return quaternionRotation1.X; } set { quaternionRotation1.X = value; ResetScene(); } }
        public float QY1 { get { return quaternionRotation1.Z; } set { quaternionRotation1.Z = value; ResetScene(); } }
        public float QZ1 { get { return quaternionRotation1.Y; } set { quaternionRotation1.Y = value; ResetScene(); } }
        public float QW1 { get { return quaternionRotation1.W; } set { quaternionRotation1.W = value; ResetScene(); } }
        #endregion

        //camera control
        private float scrollRate = 1.0f;
        private MouseState previousMouse;
        //viewports
        private Viewport defaultViewport;
        private Viewport leftViewport;
        private Viewport rightViewport;
        private Matrix projectionMatrix;
        private Matrix halfprojectionMatrix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            effect = new BasicEffect(graphics.GraphicsDevice);
            wireframeEffect = new BasicEffect(graphics.GraphicsDevice);
            camera = new ArcBallCamera(new Vector3(0f, 0f, 0f), MathHelper.ToRadians(-200), 0f, 10f, 300f, 50f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 512f);
            Puma1 = new Puma(graphics.GraphicsDevice, 2, 2, 2, 2);
            InitializeGlobalAxis(4);
            InitializePositionAxis();

            base.Initialize();
        }
        private void InitializeGlobalAxis(int size)
        {
            //Global axis
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
        private void InitializePositionAxis()
        {
            var q = quaternionRotation0;
            q.W = MathHelper.ToRadians(q.W);

            //start / end position axis
            Axis Axe0 = new Axis(graphics.GraphicsDevice);
            Axe0.QuaterionRotation(Quaternion.Normalize(q));
            Axe0.Translate(position0);
            PositionAxis.Add(Axe0);

            q = quaternionRotation1;
            q.W = MathHelper.ToRadians(q.W);

            Axis Axe1 = new Axis(graphics.GraphicsDevice);
            Axe1.QuaterionRotation(Quaternion.Normalize(q));
            Axe1.Translate(position1);
            PositionAxis.Add(Axe1);
        }
        public void ResetScene()
        {
            PositionAxis.Clear();
            InitializePositionAxis();
            timeElapsedFromAnimationStart = 0;
        }
        protected override void LoadContent()
        {
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
            if (keyState.IsKeyDown(Keys.Up))
            {
                camera.Elevation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                camera.Elevation += MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                camera.Rotation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.Right))
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

            var q0 = quaternionRotation0;
            q0.W = MathHelper.ToRadians(q0.W);
            var q1 = quaternionRotation1;
            q1.W = MathHelper.ToRadians(q1.W);

            //LEFT VIEWPORT
            GraphicsDevice.Viewport = leftViewport;
            DrawAxis(camera, wireframeEffect);
            foreach (var a in PositionAxis)
                a.Draw(effect);
            //Puma1.DrawStage(effect);
            Puma1.DrawAngleLinInterpolationSimulation(position0, position1, q0, q1, (float)timeElapsedFromAnimationStart, AnimationTime, effect);

            //RIGHT VIEWPORT
            GraphicsDevice.Viewport = rightViewport;
            DrawAxis(camera, wireframeEffect);
            foreach (var a in PositionAxis)
                a.Draw(effect);
            Puma1.DrawPositionCalcSphericalSimulation(position0, position1, q0, q1, (float)timeElapsedFromAnimationStart, AnimationTime, effect);

            base.Draw(gameTime);
        }
    }
}
