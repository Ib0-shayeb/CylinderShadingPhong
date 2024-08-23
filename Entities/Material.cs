using System.Data.SqlTypes;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CylinderShadingPhong.Entities
{
    public class Material
    {
        public Vector<double> ambientReflectionCoefficient { get; set; }
        public Vector<double> diffuseReflectionCoefficient { get; set; }
        public Vector<double> specularReflectionCoefficient { get; set; }
    }
}