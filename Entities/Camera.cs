using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CylinderShadingPhong.Entities
{
    public class Camera
    {
        public Vector<double> cPos { get; set; }

        public Vector<double> cTarget { get; set; }

        public Vector<double> cUp = Vector<double>.Build.DenseOfArray([0,1,0]); 
        //width and height of resulting image
        public int sx;
        public int sy;

        //not sure if i should calculate this or assign it!!!
        public float fov = (float)Math.PI / 2;

        public Matrix<double> projectionMatrix;
        public Matrix<double> cameraViewMatrix;

        public Camera(Vector<double> cPos, Vector<double> cTarget, int sy, int sx){
            this.cPos = cPos;
            this.cTarget = cTarget;
            this.sx = sx;
            this.sy = sy;
            
            double tmp = -1 * (sx/2) * (1/Math.Tan(fov/2));
            
            projectionMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {tmp, 0, sx/2, 0},
                {0, -tmp, sx/2, 0},
                {0, 0, 0, 1},
                {0, 0, 1, 0}});

            Vector<double> cZ = (cPos - cTarget).Normalize(2);
            Vector<double> cX = CrossProduct(cUp, cZ).Normalize(2);
            Vector<double> cY = CrossProduct(cZ, cX).Normalize(2);
            

            cameraViewMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
            {cX[0], cX[1], cX[2], cX * cPos},
            {cY[0], cY[1], cY[2], cY * cPos},
            {cZ[0], cZ[1], cZ[2], cZ * cPos},
            {0, 0, 0, 1}});
        }


        public Vector<double> AdjuctCam(Vector<double> point){
            //multyplyin by cameras matrix
            Vector<double> tmp = cameraViewMatrix * point;
            return tmp;
        }
        //projects poind from 3d to 2d (camera coordinate system)
        public Vector<double> ProjectPoint(Vector<double> point){
            Vector<double> tmp = projectionMatrix * point;
            return tmp/ tmp[3];
        }

        public static Vector<double> to3D(Vector<double> vector4D){
            return Vector<double>.Build.DenseOfArray(new double[] { vector4D[0], vector4D[1], vector4D[2] });
        }

        static Vector<double> CrossProduct(Vector<double> a, Vector<double> b)
    {
        if (a.Count != 3 || b.Count != 3)
        {
            throw new ArgumentException("Cross product is only defined for 3D vectors.");
        }

        return Vector<double>.Build.DenseOfArray(new double[]
        {
            a[1] * b[2] - a[2] * b[1],  // X component
            a[2] * b[0] - a[0] * b[2],  // Y component
            a[0] * b[1] - a[1] * b[0]   // Z component
        });
    }

        //transformation matrix field
        //to transform from coordinate system of the scene
        //to camera coordinate system

        //function to get the transformation matrix from the camera props
    }
}