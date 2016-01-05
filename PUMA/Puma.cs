using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUMA
{
    public class Puma
    {
        GraphicsDevice device;

        public readonly float Length1 = 2;
        public float Length2 = 20;
        public readonly float Length3 = 2;
        public readonly float Length4 = 2;
        public readonly float Length5 = 2;
        public float Angle1;
        public float Angle2;
        public float Angle3;
        public float Angle4;
        public float Angle5;

        private Cylinder L1;
        private Cylinder L2;
        private Cylinder L3;
        private Cylinder L4;
        private Cylinder L5;
        

        public Puma(GraphicsDevice device, float length1, float length2, float length3, float length4)
        {
            Length1 = length1;
            Length2 = length2;
            Length3 = length3;

            Angle1 = (float)Math.PI / 6;
            Angle2 = (float)Math.PI/2.5f;
            Angle3 = (float)Math.PI/2.5f;
            Angle4 = 0;
            Angle5 = 0;

            this.device = device;
            L1 = new Cylinder(device, 2, 0.5f, 16, Color.Green);
            L2 = new Cylinder(device, 2, 0.5f, 16, Color.Red);
            L3 = new Cylinder(device, 2, 0.5f, 16, Color.Green);
            L4 = new Cylinder(device, 2, 0.5f, 16, Color.Green);
            L5 = new Cylinder(device, 2, 0.5f, 16, Color.Green);
            SetPositionsOfArms();
        }

        private void SetPositionsOfArms()
        {
            Matrix[] transform = CalculateTransformation();
            Matrix F1 = transform[0];
            Matrix F2 = transform[1];
            Matrix F3 = transform[2];
            Matrix F4 = transform[3];
            Matrix F5 = transform[4];

            L1.Transform(F1);
            L2.Transform(F2);
            L3.Transform(F3);
            L4.Transform(F4);
            L5.Transform(F5);
        }

        private Matrix[] CalculateTransformation()
        {

            //Matrix rotatelocally = 
            //    Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) *
            //    Matrix.CreateRotationZ(angle) *
            //    Matrix.CreateTranslation(origin.X, origin.Y, 0f);




            var F0 = new ArmOrientation(Vector3.Right, Vector3.Up, Vector3.Backward, Vector3.Zero);
            var F01 = Matrix.CreateFromAxisAngle(F0.AlphaY, Angle1); 
            Matrix F1 = F0.ToMatrix() * F01;

            var F12 = Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateFromAxisAngle(F1.Forward, Angle2); //here
            //Matrix F2 = F1 * F12;
            Matrix F2 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * F0.ToMatrix() * Matrix.CreateTranslation(Vector3.Up * Length1);

            var F23 = Matrix.CreateTranslation(F2.Up * Length2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3);
            //Matrix F3 = F2 * F23;
            Matrix F3 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) * F0.ToMatrix() * Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateTranslation(F2.Up * Length2); //ok

            var F34 = Matrix.CreateTranslation(Vector3.Up * -Length3) * Matrix.CreateFromAxisAngle(F3.Up, Angle4);
            //Matrix F4 = F3 * F34;
            Matrix F4 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) * Matrix.CreateFromAxisAngle(F3.Up, Angle4) * F0.ToMatrix() * Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateTranslation(Vector3.Right * Length2) * Matrix.CreateTranslation(Vector3.Up * -Length3);

            var F45 = Matrix.CreateTranslation(Vector3.Right * Length4) * Matrix.CreateFromAxisAngle(F4.Right, Angle5);
            //Matrix F5 = F4 * F45;
            Matrix F5= F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) * Matrix.CreateFromAxisAngle(F3.Up, Angle4) * Matrix.CreateFromAxisAngle(F4.Right, Angle5) * F0.ToMatrix() * Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateTranslation(Vector3.Right * Length2) * Matrix.CreateTranslation(Vector3.Up * -Length3) * Matrix.CreateTranslation(Vector3.Right * Length4);

            Matrix[] result = { F1, F2, F3, F4, F5 };

            //result.F0 = new ArmOrientation(Vector3.Right, Vector3.Forward, Vector3.Up, Vector3.Zero);
            //result.F01 = Matrix.CreateRotationY(Angle1);
            //result.F12 = Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateRotationY(Angle2);
            //result.F23 = Matrix.CreateTranslation(Vector3.Right * Length2) * Matrix.CreateRotationY(Angle3);
            //result.F34 = Matrix.CreateTranslation(Vector3.Up * -Length3) * Matrix.CreateRotationZ(Angle4);
            //result.F45 = Matrix.CreateTranslation(Vector3.Right * Length4) * Matrix.CreateRotationX(Angle5);


            //Matrix F05 =
            //    Matrix.CreateRotationZ(Angle1) * Matrix.CreateTranslation(Vector3.Up * Length1) *
            //    Matrix.CreateRotationY(Angle2) * Matrix.CreateTranslation(Vector3.Right * Length2) *
            //    Matrix.CreateRotationY(Angle3) * Matrix.CreateTranslation(Vector3.Up * -Length3) *
            //    Matrix.CreateRotationZ(Angle4) * Matrix.CreateTranslation(Vector3.Right * Length4) *
            //    Matrix.CreateRotationX(Angle5);
            //Matrix F5 = F0.ToMatrix() * F05;

            return result;
        }


        public void DrawStage(ArcBallCamera camera, BasicEffect effect)
        {
            L1.Draw(effect);
            L2.Draw(effect);
            L3.Draw(effect);
            L4.Draw(effect);
            L5.Draw(effect);
        }
    }

}
