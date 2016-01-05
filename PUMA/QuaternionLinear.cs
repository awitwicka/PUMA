﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUMA
{
    public class QuaternionLinear
    {
        GraphicsDevice device;

        public Vector3 Position0;
        public Vector3 Position1;
        public Quaternion Rotation0;
        public Quaternion Rotation1;
        public Color LineColor = Color.Black;

        private Axis Axe0;
        private Axis Axe1;
        private Axis AxeNext;
        private List<VertexPositionColor> LineVertices = new List<VertexPositionColor>();
        private List<short> LineIndices = new List<short>();
        private List<Axis> Steps = new List<Axis>();

        public QuaternionLinear(GraphicsDevice device, Vector3 position0, Vector3 position1, Quaternion rotation0, Quaternion rotation1)
        {
            this.device = device;
            Position0 = position0;
            Position1 = position1;
            Rotation0 = rotation0;
            Rotation1 = rotation1;
            Initialize();
        }
        //TODO: additional constructor with color & defult contructor

        private void Initialize()
        {
            LineVertices.Add(new VertexPositionColor(new Vector3(Position0.X, Position0.Z, Position0.Y), LineColor));
            Axe0 = new Axis(device);
            Axe0.QuanterionRotation(Rotation0);
            Axe0.Translate(Position0);

            Axe1 = new Axis(device);
            Axe1.QuanterionRotation(Rotation1);
            Axe1.Translate(Position1);

            AxeNext = new Axis(device);
            AxeNext.QuanterionRotation(Rotation0);
            AxeNext.Translate(Position0);
        }

        public void ResetSteps(int steps)
        {
            Steps.Clear();
            float step = 1 / (float)(steps + 1);
            for (int i = 1; i <= steps; i++)
            {
                Steps.Add(new Axis(device));
                NextStep(step * i, Steps.Last(), false);
            }
        }

        private void NextStep(float time, Axis axis, bool drawLine) //0-1
        {
            var nextPos = (1 - time) * Position0 + time * Position1;
            var nextAngle = Rotation0 * (1 - time) + Rotation1 * time;
            //var nextAngle = Quaternion.Lerp(Rotation0, Rotation1, time); //both are good

            //drawing line
            if (drawLine)
            {
                short vertCount = (short)LineVertices.Count;
                LineVertices.Add(new VertexPositionColor(new Vector3(nextPos.X, nextPos.Z, nextPos.Y), LineColor));
                LineIndices.Add((short)(vertCount - 1));
                LineIndices.Add(vertCount);
            }

            axis.Reset();
            axis.QuanterionRotation(nextAngle); //reset and rotate
            axis.Translate(nextPos); //translate

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="effect"></param>
        /// <param name="timeElapsedFromAnimationStart">in miliseconds</param>
        /// <param name="totalAnimationTime">in seconds</param>
        public void Draw(ArcBallCamera camera, BasicEffect effect, BasicEffect wireframeEffect, double timeElapsedFromAnimationStart, double totalAnimationTime, bool isAnimated)
        {
            if (isAnimated)
            {
                totalAnimationTime *= 1000;
                if (timeElapsedFromAnimationStart <= totalAnimationTime /*&& gameTime.ElapsedGameTime.TotalMilliseconds>=1*/)
                    NextStep((float)(timeElapsedFromAnimationStart / totalAnimationTime), AxeNext, true);
            }
            Axe0.Draw(effect);
            Axe1.Draw(effect);
            AxeNext.Draw(effect);

            if (LineVertices.Count - 1 > 0)
                foreach (EffectPass pass in wireframeEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, LineVertices.ToArray(), 0, LineVertices.Count, LineIndices.ToArray(), 0, LineVertices.Count - 1);
                }
            
        }
        public void DrawStages(ArcBallCamera camera, BasicEffect effect, BasicEffect wireframeEffect)
        {
            Axe0.Draw(effect);
            Axe1.Draw(effect);
            foreach (var a in Steps)
                a.Draw(effect);
        }
    }
}
