using System;
using System.ComponentModel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CylinderShadingPhong.Entities
{
    public class Camera: INotifyPropertyChanged
    {
        private Vector<double> _cPos;
        public Vector<double> cPos { get => _cPos; set
        {
            _cPos = value;
            OnPropertyChanged(nameof(cPos));
        }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        

        private Vector<double> _cTarget;
        public Vector<double> cTarget { get => _cTarget; set
        {
            _cTarget = value;
            OnPropertyChanged(nameof(cTarget ));
        }
        }

        public Vector<double> cUp = Vector<double>.Build.DenseOfArray([0,1,0]); 
        public Vector<double> cZ;
        public Vector<double> cX;
        public Vector<double> cY;
        //width and height of resulting image
        public int sx;
        public int sy;

        //not sure if i should calculate this or assign it!!!
        public float fov = (float)Math.PI / 2;

        public Matrix<double> projectionMatrix;
        public Matrix<double> cameraViewMatrix;

        protected void OnPropertyChanged(string propertyName)
        {

            //PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if(cPos!=null && cTarget!=null){
                cZ = (cPos - cTarget).Normalize(2);
                cX = CrossProduct(cUp, cZ).Normalize(2);
                cY = CrossProduct(cZ, cX).Normalize(2);
                

                cameraViewMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {cX[0], cX[1], cX[2], cX * cPos},
                {cY[0], cY[1], cY[2], cY * cPos},
                {cZ[0], cZ[1], cZ[2], cZ * cPos},
                {0, 0, 0, 1}}); 
            }
            
        }
        public Camera(Vector<double> cPos, Vector<double> cTarget, int sy, int sx){
            this._cPos = cPos;
            this._cTarget = cTarget;
            this.sx = sx;
            this.sy = sy;

            cZ = (cPos - cTarget).Normalize(2);
            cX = CrossProduct(cUp, cZ).Normalize(2);
            cY = CrossProduct(cZ, cX).Normalize(2);
                
            cameraViewMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
            {cX[0], cX[1], cX[2], cX * cPos},
            {cY[0], cY[1], cY[2], cY * cPos},
            {cZ[0], cZ[1], cZ[2], cZ * cPos},
            {0, 0, 0, 1}}); 
            
            double tmp = -1 * (sx/2) * (1/Math.Tan(fov/2));
            
            projectionMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {tmp, 0, sx/2, 0},
                {0, -tmp, sx/2, 0},
                {0, 0, 0, 1},
                {0, 0, 1, 0}});
        }


        public Vector<double> AdjustCam(Vector<double> point){
            //multyplyin by cameras matrix
            Vector<double> tmp = cameraViewMatrix * point;
            return tmp;
        }
        //projects poind from 3d to 2d (camera coordinate system)
        public Vector<double> ProjectPoint(Vector<double> point){
            Vector<double> tmp = projectionMatrix * point;
            return tmp/ tmp[3];
        }

        //left n right
        public void RotateAroundCY(double rotationAngle){
            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {Math.Cos(rotationAngle), 0, Math.Sin(rotationAngle), 0},
                {0, 1, 0, 0},
                {-Math.Sin(rotationAngle), 0, Math.Cos(rotationAngle), 0},
                {0, 0, 0, 1}});
            cX = ThreeD(rotationMatrix * FourD(cX));
            cZ = ThreeD(rotationMatrix * FourD(cZ));
            _cTarget = cPos + cZ;

            cameraViewMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {cX[0], cX[1], cX[2], cX * cPos},
                {cY[0], cY[1], cY[2], cY * cPos},
                {cZ[0], cZ[1], cZ[2], cZ * cPos},
                {0, 0, 0, 1}}); 
        }

        //up n down
        public void RotateAroundCX(double rotationAngle){
            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {Math.Cos(rotationAngle), -Math.Sin(rotationAngle), 0, 0},
                {Math.Sin(rotationAngle), Math.Cos(rotationAngle), 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}});
            cY = ThreeD(rotationMatrix * FourD(cY));
            cZ = ThreeD(rotationMatrix * FourD(cZ));
            _cTarget = cPos + cZ;

            cameraViewMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                {cX[0], cX[1], cX[2], cX * cPos},
                {cY[0], cY[1], cY[2], cY * cPos},
                {cZ[0], cZ[1], cZ[2], cZ * cPos},
                {0, 0, 0, 1}}); 
        }
        
        public static Vector<double> FourD(Vector<double> threeDVector){
            return Vector<double>.Build.DenseOfArray([threeDVector[0],threeDVector[1],threeDVector[2],0]);
        }

        public static Vector<double> ThreeD(Vector<double> fourDVector){
            return Vector<double>.Build.DenseOfArray([fourDVector[0],fourDVector[1],fourDVector[2]]);
        }

        public static Vector<double> CrossProduct(Vector<double> a, Vector<double> b)
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