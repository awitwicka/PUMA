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
        
        public struct Configuration
        {
            public float Angle1;
            public float Angle2;
            public float Length2;
            public float Angle3;
            public float Angle4;
            public float Angle5;
        }


        public Puma(GraphicsDevice device, float length1, float length2, float length3, float length4)
        {
            Length1 = length1;
            Length2 = length2;
            Length3 = length3;
            Length4 = length4;

            Configuration StartConfiguration = new Configuration();
            StartConfiguration.Angle1 = 0;
            StartConfiguration.Length2 = Length2;
            StartConfiguration.Angle2 = (float)Math.PI / 2;
            StartConfiguration.Angle3 = (float)Math.PI / 2;
            StartConfiguration.Angle4 = 0;
            StartConfiguration.Angle5 = 0;

            this.device = device;
            L1 = new Cylinder(device, Length1, 0.5f, 16, Color.Green);
            L2 = new Cylinder(device, Length2, 0.5f, 16, Color.Red);
            L3 = new Cylinder(device, Length3, 0.5f, 16, Color.Green);
            L4 = new Cylinder(device, Length4, 0.5f, 16, Color.Green);
            L5 = new Cylinder(device, Length5, 0.2f, 16, Color.Green);
            
            //SetPositionsOfArms(StartConfiguration);
            var angles = CalculateAnglesFromEndPosition(new ArmOrientation(
                Vector3.Normalize(new Vector3(0.14f, 1.044f, -1.2f)), 
                Vector3.Normalize(new Vector3(-0.14f, 1.044f, 1.2f)), 
                Vector3.Normalize(new Vector3(0.14f, -1.044f, 1.2f)), 
                new Vector3(10, 10, 10))); //everything -4 in z????
            SetPositionsOfArms(angles);
        }

        private void SetPositionsOfArms(Configuration angles)
        {
            Angle1 = angles.Angle1;
            Angle2 = angles.Angle2;
            Length2 = angles.Length2;
            Angle3 = angles.Angle3;
            Angle4 = angles.Angle4;
            Angle5 = angles.Angle5;

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

            return result;
        }
        private Configuration CalculateAnglesFromEndPosition(ArmOrientation endOrientation)
        {
            //flipping z axis to match puma cooedinate system
            endOrientation.Position.Z *= -1;
            Configuration angles = new Configuration();

            float angle1a = (float)Math.Atan((endOrientation.Position.Z - Length4 * endOrientation.AlphaX.Z) / (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X));
            //float angle1b = ((angle1a < 0)? angle1a + (float)Math.PI : angle1a - (float)Math.PI);
            angles.Angle1 = angle1a;
            angles.Angle4 = (float)Math.Asin(Math.Cos(angles.Angle1) * endOrientation.AlphaX.Z - Math.Sin(angles.Angle1) * endOrientation.AlphaX.X); //1 or 2 sol for given a1
            /********* angle 5 ***********/
            var Angle5a = (float)Math.Acos((Math.Cos(angles.Angle1) * endOrientation.AlphaZ.Z - Math.Sin(angles.Angle1) * endOrientation.AlphaZ.X) / (Math.Cos(angles.Angle4))); //uniq
            float angle5a2 = ((Angle5a < 0) ? Angle5a + (float)Math.PI : Angle5a - (float)Math.PI);
            var Angle5b = (float)Math.Asin((Math.Sin(angles.Angle1) * endOrientation.AlphaY.X - Math.Cos(angles.Angle1) * endOrientation.AlphaY.Z) / (Math.Cos(angles.Angle4)));
            float angle5b2 = ((Angle5a < 0) ? Angle5b + (float)Math.PI : Angle5b - (float)Math.PI);
            if (Math.Round(Angle5a,2) == Math.Round(Angle5b,2))
                angles.Angle5 = Angle5a;
            else if (Math.Round(angle5a2) == Math.Round(angle5b2))
                angles.Angle5 = angle5b2;
            else if (Math.Round(Angle5a) == Math.Round(angle5b2))
                angles.Angle5 = angle5b2;
            else if (Math.Round(Angle5b) == Math.Round(angle5a2))
                angles.Angle5 = angle5a2;
            //angles.Angle5 = (float)Math.Asin((Math.Sin(angles.Angle1) * endOrientation.AlphaY.X - Math.Cos(angles.Angle1) * endOrientation.AlphaY.Z) / (Math.Cos(angles.Angle4)));
            /********* angle 5 ***********/
            angles.Angle2 = (float)Math.Atan(-(Math.Cos(angles.Angle1) * Math.Cos(angles.Angle4) * (endOrientation.Position.Y - Length4 * endOrientation.AlphaX.Y - Length1) + Length3 * (endOrientation.AlphaX.X + Math.Sin(angles.Angle1) * Math.Sin(angles.Angle4)))// 2 sol
                / (Math.Cos(angles.Angle4) * (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X) - Math.Cos(angles.Angle1) * Length3 * endOrientation.AlphaX.Y));
            angles.Length2 = (float)((Math.Cos(angles.Angle4) * (endOrientation.Position.X - Length4 * endOrientation.AlphaX.X) - Math.Cos(angles.Angle1) * Length3 * endOrientation.AlphaX.Y)
                / (Math.Cos(angles.Angle1) * Math.Cos(angles.Angle2) * Math.Cos(angles.Angle4)));

            var Angle3a = (float)Math.Acos((endOrientation.AlphaX.X + Math.Sin(angles.Angle1) * Math.Sin(angles.Angle4)) / (Math.Cos(angles.Angle1) * Math.Cos(angles.Angle4))) - angles.Angle2; //uniq
            //float angle3a2 = ((Angle3a < 0) ? Angle3a + (float)Math.PI : Angle3a - (float)Math.PI);
            /*var Angle3b*/
            angles.Angle3 = (float)Math.Asin(-endOrientation.AlphaX.Y / Math.Cos(angles.Angle4)) - angles.Angle2;
            //float angle3b2 = ((Angle3b < 0) ? Angle3b + (float)Math.PI : Angle3b - (float)Math.PI);
            /***/
            //if (Math.Round(Angle3a, 2) == Math.Round(Angle3b, 2))
            //    angles.Angle3 = Angle3a;
            //else if (Math.Round(angle3a2) == Math.Round(angle3b2))
            //    angles.Angle3 = angle3b2;
            //else if (Math.Round(Angle3a) == Math.Round(angle3b2))
            //    angles.Angle3 = angle3b2;
            //else if (Math.Round(Angle3b) == Math.Round(angle3a2))
            //    angles.Angle3 = angle3a2;
            /***/
            angles.Angle2 += (float)Math.PI / 2;
            Angle3a += (float)Math.PI / 2;
            angles.Angle3 += (float)Math.PI / 2;

            return angles;
        }
        private void NextStepAngleInterpolation(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float elapsedTime)
        {
            //ToDo: check if rotation ok
            startRot = Quaternion.Normalize(startRot);
            endRot = Quaternion.Normalize(endRot);
            var startTransform = Matrix.CreateFromQuaternion(startRot);
            var endTransform = Matrix.CreateFromQuaternion(endRot);
            var x0 = Vector3.Transform(Vector3.Right, startTransform);
            var y0 = Vector3.Transform(Vector3.Up, startTransform);
            var z0 = Vector3.Transform(Vector3.Backward, startTransform);
            var x1 = Vector3.Transform(Vector3.Right, endTransform);
            var y1 = Vector3.Transform(Vector3.Up, endTransform);
            var z1 = Vector3.Transform(Vector3.Backward, endTransform);
            var anglesStart = CalculateAnglesFromEndPosition(new ArmOrientation(x0, y0, z0, startPos));
            //var nextPos = (1 - elapsedTime) * startPos + elapsedTime * endPos;
            var anglesEnd = CalculateAnglesFromEndPosition(new ArmOrientation(x1, y1, z1, endPos));
            var angleNext = new Configuration();

            angleNext.Angle1 = anglesStart.Angle1 * (1 - elapsedTime) + anglesEnd.Angle1 * elapsedTime;
            angleNext.Angle2 = anglesStart.Angle2 * (1 - elapsedTime) + anglesEnd.Angle2 * elapsedTime;
            angleNext.Length2 = anglesStart.Length2 * (1 - elapsedTime) + anglesEnd.Length2 * elapsedTime;
            angleNext.Angle3 = anglesStart.Angle3 * (1 - elapsedTime) + anglesEnd.Angle3 * elapsedTime;
            angleNext.Angle4 = anglesStart.Angle4 * (1 - elapsedTime) + anglesEnd.Angle4 * elapsedTime;
            angleNext.Angle5 = anglesStart.Angle5 * (1 - elapsedTime) + anglesEnd.Angle5 * elapsedTime;
            SetPositionsOfArms(angleNext);
        }
        private void NextStepQuaternionSpherInterpolation(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float elapsedTime)
        {
            startRot = Quaternion.Normalize(startRot);
            endRot = Quaternion.Normalize(endRot);
            var transformation = Matrix.CreateFromQuaternion(Quaternion.Slerp(startRot, endRot, elapsedTime));
            var x = Vector3.Transform(Vector3.Right, transformation); //transformation.Right;
            var y = Vector3.Transform(Vector3.Up, transformation); //transformation.Up;
            var z = Vector3.Transform(Vector3.Backward, transformation); //transformation.Backward;

            var nextPos = (1 - elapsedTime) * startPos + elapsedTime * endPos;
            var nextAngles = CalculateAnglesFromEndPosition(new ArmOrientation(x, y, z, nextPos));
            SetPositionsOfArms(nextAngles);
        }
        /// <summary>
        /// Calculates angles set up for start and end position of PUMA, then interpolates linearly all angles from start position till end position;
        /// </summary>
        public void DrawAngleLinInterpolationSimulation(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float elapsedTime, float totalAnimationTime, BasicEffect effect)
        {
            totalAnimationTime *= 1000;
            if (elapsedTime <= totalAnimationTime)
                NextStepAngleInterpolation(startPos, endPos, startRot, endRot, elapsedTime/totalAnimationTime);
            DrawStage(effect);
        }
        /// <summary>
        /// Calculates angles set up for every position on the way, than interpolates rotation of the last segment of an arm with spherical Quaternion interpolation; 
        /// </summary>
        public void DrawPositionCalcSphericalSimulation(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float elapsedTime, float totalAnimationTime, BasicEffect effect)
        {
            totalAnimationTime *= 1000;
            if (elapsedTime <= totalAnimationTime)
                NextStepQuaternionSpherInterpolation(startPos, endPos, startRot, endRot, elapsedTime / totalAnimationTime);
            DrawStage(effect);
        }
        public void DrawStage(BasicEffect effect)
        {
            L1.Draw(effect);
            L2.Draw(effect);
            L3.Draw(effect);
            L4.Draw(effect);
            L5.Draw(effect);
        }
    }

}
