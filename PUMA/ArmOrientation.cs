using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUMA
{
    public class ArmOrientation
    {
        public Vector3 AlphaX;
        public Vector3 AlphaY;
        public Vector3 AlphaZ;
        public Vector3 Position;
        
        public ArmOrientation(Vector3 alphaX, Vector3 alphaY, Vector3 alphaZ, Vector3 position)
        {
            AlphaX = alphaX;
            AlphaY = alphaY;
            AlphaZ = alphaZ;
            Position = position;
        }

        public Matrix ToMatrix()
        {
            return new Matrix(AlphaX.X, AlphaY.X, AlphaZ.X, Position.X,
                AlphaX.Y, AlphaY.Y, AlphaZ.Y, Position.Y,
                AlphaX.Z, AlphaY.Z, AlphaZ.Z, Position.Z,
                0,0,0,1);
        }


    }
}
