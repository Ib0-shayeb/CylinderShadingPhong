using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CylinderShadingPhong.Entities
{
    public class CylinderVertex
    {
        //position
        public Vector<double> position { get; set; }

        //normal
        public Vector<double> normal { get; set; }
    }
}