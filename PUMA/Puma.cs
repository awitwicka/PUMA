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

            Angle1 = 0;
            Angle2 = (float)Math.PI / 2;
            Angle3 = (float)Math.PI / 2;
            Angle4 = 0;
            Angle5 = 0;

            this.device = device;
            L1 = new Cylinder(device, Length1, 0.5f, 16, Color.Green);
            L2 = new Cylinder(device, Length2, 0.5f, 16, Color.Red);
            L3 = new Cylinder(device, Length3, 0.5f, 16, Color.Green);
            L4 = new Cylinder(device, Length4, 0.5f, 16, Color.Green);
            L5 = new Cylinder(device, Length5, 0.2f, 16, Color.Green);
            SetPositionsOfArms();
            CalculateAnglesFromEndPosition(new ArmOrientation(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(3, 0, -8))); //everything -4 in z????
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
            //Matrix EndPoint = transform[5];

            //TODO: Reset all cylinders to 0 0 0 position 
            L1 = new Cylinder(device, Length1, 0.5f, 16, Color.Green);
            L2 = new Cylinder(device, Length2, 0.5f, 16, Color.Red);
            L3 = new Cylinder(device, Length3, 0.5f, 16, Color.Green);
            L4 = new Cylinder(device, Length4, 0.5f, 16, Color.Green);
            L5 = new Cylinder(device, Length5, 0.2f, 16, Color.Green);
            //TODO: clear memory?
            
            L1.Transform(F1);
            L2.Transform(F2);
            L3.Transform(F3);
            L4.Transform(F4);
            L5.Transform(F5);

            //debug
            var a1 = Angle1 * 180 / Math.PI;
            var a2 = Angle2 * 180 / Math.PI;
            var a3 = Angle3 * 180 / Math.PI;
            var a4 = Angle4 * 180 / Math.PI;
            var a5 = Angle5 * 180 / Math.PI;
        }

        private Matrix[] CalculateTransformation()
        {
            var F0 = new ArmOrientation(Vector3.Right, Vector3.Up, Vector3.Backward, Vector3.Zero);
            var F01 = Matrix.CreateFromAxisAngle(F0.AlphaY, Angle1); 
            Matrix F1 = F01 * F0.ToMatrix();

            //var F12 = Matrix.CreateTranslation(Vector3.Up * Length1) * Matrix.CreateFromAxisAngle(F1.Forward, Angle2); //here
            //Matrix F2 = F1 * F12;
            Matrix F2 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * 
                F0.ToMatrix() * Matrix.CreateTranslation(F1.Up * Length1);

            //var F23 = Matrix.CreateTranslation(F2.Up * Length2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3);
            //Matrix F3 = F2 * F23;
            Matrix F3 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) * 
                F0.ToMatrix() * Matrix.CreateTranslation(F1.Up * Length1) * Matrix.CreateTranslation(F2.Up * Length2); //ok

            //var F34 = Matrix.CreateTranslation(Vector3.Up * -Length3) * Matrix.CreateFromAxisAngle(F3.Up, Angle4);
            //Matrix F4 = F3 * F34;
            Matrix F4 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) *
                Matrix.CreateFromAxisAngle(F3.Forward, (float)(2 * Math.PI - Math.PI/2)) * Matrix.CreateFromAxisAngle(F3.Up, -Angle4) * 
                F0.ToMatrix() * Matrix.CreateTranslation(F1.Up * Length1) * Matrix.CreateTranslation(F2.Up * Length2) * Matrix.CreateTranslation(F3.Up * Length3);

            //var F45 = Matrix.CreateTranslation(Vector3.Right * Length4) * Matrix.CreateFromAxisAngle(F4.Right, Angle5);
            //Matrix F5 = F4 * F45;
            Matrix F5 = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) *
                Matrix.CreateFromAxisAngle(F3.Forward, (float)(2 * Math.PI - Math.PI / 2)) * Matrix.CreateFromAxisAngle(F3.Up, -Angle4) * Matrix.CreateFromAxisAngle(F4.Up, Angle5) * 
                F0.ToMatrix() * Matrix.CreateTranslation(F1.Up * Length1) * Matrix.CreateTranslation(F2.Up * Length2) * Matrix.CreateTranslation(F3.Up * Length3) * Matrix.CreateTranslation(F4.Up * Length4);

            Matrix EndPoint = F01 * Matrix.CreateFromAxisAngle(F1.Forward, Angle2) * Matrix.CreateFromAxisAngle(F2.Forward, Angle3) *
                Matrix.CreateFromAxisAngle(F3.Forward, (float)(2 * Math.PI - Math.PI / 2)) * Matrix.CreateFromAxisAngle(F3.Up, -Angle4) * Matrix.CreateFromAxisAngle(F4.Up, Angle5) *
                F0.ToMatrix() * Matrix.CreateTranslation(F1.Up * Length1) * Matrix.CreateTranslation(F2.Up * Length2) * Matrix.CreateTranslation(F3.Up * Length3) * Matrix.CreateTranslation(F4.Up * Length4) * Matrix.CreateTranslation(F5.Up * Length5);

            Matrix[] result = { F1, F2, F3, F4, F5, EndPoint };

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

        private void CalculateAnglesFromEndPosition(ArmOrientation endOrientation)
        {
            //var p0 = startPosition;
            //var p1 = p0;
            //var p2 = p1;
            //p2.Y += (p1.Y * Length1);

            //var p5 = endPosition;
            //var p4 = p5;
            //p4.

            //Angle1 = Math.Atan2((p4 - p0) * y, (p4 - po) * x);


            Angle1 = (float)Math.Atan((endOrientation.Position.Z - Length4 * endOrientation.AlphaX.Z) / (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X)); //2 sol
            Angle4 = (float)Math.Asin(Math.Cos(Angle1) * endOrientation.AlphaX.Z - Math.Sin(Angle1) * endOrientation.AlphaX.X); //1 or 2 sol for given a1

            var Angle5a = (float)Math.Acos((Math.Cos(Angle1) * endOrientation.AlphaZ.Z - Math.Sin(Angle1) * endOrientation.AlphaZ.X) / (Math.Cos(Angle4))); //uniq
                                                                                                                                                            /*var Angle5b*/
            Angle5 = (float)Math.Asin((Math.Sin(Angle1) * endOrientation.AlphaY.X - Math.Cos(Angle1) * endOrientation.AlphaY.Z) / (Math.Cos(Angle4)));

            Angle2 = (float)Math.Atan(-(Math.Cos(Angle1) * Math.Cos(Angle4) * (endOrientation.Position.Y - Length4 * endOrientation.AlphaX.Y - Length1) + Length3 * (endOrientation.AlphaX.X + Math.Sin(Angle1) * Math.Sin(Angle4)))// 2 sol
                / (Math.Cos(Angle4) * (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X) - Math.Cos(Angle1) * Length3 * endOrientation.AlphaX.Y));
            Length2 = (float)((Math.Cos(Angle4) * (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X) - Math.Cos(Angle1) * Length3 * endOrientation.AlphaX.Y)
                / (Math.Cos(Angle1) * Math.Cos(Angle2) * Math.Cos(Angle4)));

            var Angle3a = (float)Math.Acos((endOrientation.AlphaX.X + Math.Sin(Angle1) * Math.Sin(Angle4)) / (Math.Cos(Angle1) * Math.Cos(Angle4))) - Angle2; //uniq
            /*var Angle3b*/
            Angle3 = (float)Math.Asin(-endOrientation.AlphaX.Y / Math.Cos(Angle4)) - Angle2;


            Angle2 += (float)Math.PI / 2;
            Angle3a += (float)Math.PI / 2;
            Angle3 += (float)Math.PI / 2;
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
