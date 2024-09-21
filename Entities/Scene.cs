using System;
using Avalonia.Input.Raw;
using MathNet.Numerics.LinearAlgebra;
using SkiaSharp;

namespace CylinderShadingPhong.Entities
{
    public class Scene
    {
        //object (cylinder) no need to generalise
        public Cylinder cylinder {get; set;}

        //lighting
        public Vector<double> lightingPosition { get; set; }

        //camera
        public Camera camera {get; set;}

        public Material material {get; set;}

        public int[] pixels;

        public int width;
        public int height;

        public Scene(Cylinder cylinder, Vector<double> lightingPosition, Camera camera, int width, int height, Material material){
            this.camera = camera;
            this.cylinder = cylinder;
            this.lightingPosition = lightingPosition;
            this.width = width;
            this.height = height;
            this.material = material;

            pixels = new int[width * height];
        }

        public void drawPoints(){
            pixels = new int[width * height];
            
            // for (int i = 0; i < cylinder.botPreVerticies.Count; i++){
            //     var v = camera.ProjectPoint(camera.AdjustCam(cylinder.botPreVerticies[i]));
            //     int x = (int)Math.Round(v[0]);
            //     int y = (int)Math.Round(v[1]);

            //     if(isInFrame(x,y,width,height)) pixels[y * width + x] = rgbToRgba8888(0, 0, 0, 255);

            //     v = camera.ProjectPoint(camera.AdjustCam(cylinder.topPreVerticies[i]));
            //     x = (int)Math.Round(v[0]);
            //     y = (int)Math.Round(v[1]);

            //     if(isInFrame(x,y,width,height)) pixels[y * width + x] = rgbToRgba8888(0, 0, 0, 255);
            // }

            for(int i = 0; i < cylinder.sideTriangles.Count; i+=1){//every otherfor visiility tmp
                var p1 = camera.ProjectPoint(camera.AdjustCam(cylinder.sideTriangles[i].A.position));
                var p2 = camera.ProjectPoint(camera.AdjustCam(cylinder.sideTriangles[i].B.position));
                var p3 = camera.ProjectPoint(camera.AdjustCam(cylinder.sideTriangles[i].C.position));
                //if triangle is out of frame totaly
                if(!isInFrame((int)p1[0], (int)p1[1], width, height) &&
                !isInFrame((int)p2[0], (int)p2[1], width, height) && 
                !isInFrame((int)p3[0], (int)p3[1], width, height))
                    break;
                
                var cp = Camera.CrossProduct(Vector<double>.Build.DenseOfArray([p2[0] - p1[0],p2[1] - p1[1],0]),
                                        Vector<double>.Build.DenseOfArray([p3[0] - p1[0],p3[1] - p1[1],0]));
                if(cp[2] > 0){
                    // setPoint((int)p1[0], (int)p1[1], rgbToRgba8888(255, 0, 0, 255));
                    // setPoint((int)p2[0], (int)p2[1], rgbToRgba8888(255, 0, 0, 255));
                    // setPoint((int)p3[0], (int)p3[1], rgbToRgba8888(255, 0, 0, 255));

                FillTriangle(p1,p2,p3,Vector<double>.Build.DenseOfArray([255,0,0]), cylinder.sideTriangles[i], false);
                }
            }

            bool viewTopBase = camera.cZ * Camera.ThreeD(cylinder.topTriangles[0].A.normal) > 0;

            if(!viewTopBase){//bottm
                for(int i = 0; i < cylinder.topTriangles.Count; i++){//every otherfor visiility tmp
                var p1 = camera.ProjectPoint(camera.AdjustCam(cylinder.topTriangles[i].A.position));
                var p2 = camera.ProjectPoint(camera.AdjustCam(cylinder.topTriangles[i].B.position));
                var p3 = camera.ProjectPoint(camera.AdjustCam(cylinder.topTriangles[i].C.position));
                //if triangle is out of frame totaly
                if(!isInFrame((int)p1[0], (int)p1[1], width, height) &&
                !isInFrame((int)p2[0], (int)p2[1], width, height) && 
                !isInFrame((int)p3[0], (int)p3[1], width, height))
                    break;
                

                FillTriangle(p1,p2,p3,Vector<double>.Build.DenseOfArray([255,0,0]), cylinder.topTriangles[i], true);
                
                }
            }
            else{//top
                for(int i = 0; i < cylinder.botTriangles.Count; i++){//every otherfor visiility tmp
                var p1 = camera.ProjectPoint(camera.AdjustCam(cylinder.botTriangles[i].A.position));
                var p2 = camera.ProjectPoint(camera.AdjustCam(cylinder.botTriangles[i].B.position));
                var p3 = camera.ProjectPoint(camera.AdjustCam(cylinder.botTriangles[i].C.position));
                //if triangle is out of frame totaly
                if(!isInFrame((int)p1[0], (int)p1[1], width, height) &&
                !isInFrame((int)p2[0], (int)p2[1], width, height) && 
                !isInFrame((int)p3[0], (int)p3[1], width, height))
                    break;
                

                FillTriangle(p1,p2,p3, Vector<double>.Build.DenseOfArray([255,0,0]), cylinder.botTriangles[i], true);
                
                }
            }

            
     
            //frame
            for (int i = 0; i< width; i++){
                pixels[i] = rgbToRgba8888(0, 0, 0, 255);
                pixels[(height-1) * width + i] = rgbToRgba8888(0, 0, 0, 255);
            }
            for (int i = 0; i< height; i++){
                pixels[i * width] = rgbToRgba8888(0, 0, 0, 255);
                pixels[i * width + width -1] = rgbToRgba8888(0, 0, 0, 255);
            }
        }

        public bool setPoint(int x, int y, int color){
            if(!isInFrame(x,y,width,height)) return false;
            pixels[y * width + x] = color;
            return true;
        }
        public static bool isInFrame(int x, int y, int width, int height){
            return x<width && x>=0 && y>=0 && y <height;
        }
        public static int rgbToRgba8888(int r, int g, int b, int a){
            return (a << 24) | (b << 16) | (g << 8) | r;
        }
        public void FillTriangle(Vector<double> v0, Vector<double> v1, Vector<double> v2, Vector<double> Ia, CylinderTriangle Triangle, bool IsBase)
        {
            int x0 = (int)v0[0], y0 = (int)v0[1], x1 = (int)v1[0], y1 = (int)v1[1], x2 = (int)v2[0], y2 = (int)v2[1];
            int[] order = new int[] {0, 1, 2};
            if (y0 > y1) { Swap(ref x0, ref y0, ref x1, ref y1); int tmp = order[0]; order[0] = order[1]; order[1] = tmp;}
            if (y0 > y2) { Swap(ref x0, ref y0, ref x2, ref y2); int tmp = order[0]; order[0] = order[2]; order[2] = tmp;}
            if (y1 > y2) { Swap(ref x1, ref y1, ref x2, ref y2); int tmp = order[1]; order[1] = order[2]; order[2] = tmp;}

            //
            Vector<double> p1, p1g, p2, p2g, ptx, ptgx, pty, ptgy, p3, p3g;
            if(v0[0] == x0 && v0[1] == y0) {p1 = v0.Clone(); p1g = Triangle.A.position.Clone();}
            else if(v1[0] == x0 && v1[1] == y0) {p1 = v1.Clone(); p1g = Triangle.B.position.Clone();}
            else {p1 = v2.Clone(); p1g = Triangle.C.position.Clone();}

            p2g = p1g - 100 * Camera.FourD(camera.cX);
            p3g = p1g + 100 * Camera.FourD(camera.cY);
            p2 = camera.ProjectPoint(camera.AdjustCam(p2g));
            p3 = camera.ProjectPoint(camera.AdjustCam(p3g));

            double z1 = p1[2];
            double z2 = p2[2];
            double z3 = p3[2];

            ptx = p1 + Vector<double>.Build.DenseOfArray([1,0,0,0]);
            pty = p1 + Vector<double>.Build.DenseOfArray([0,1,0,0]);

            double tx = (p2[0] - p1[0] != 0) ? (ptx[0] - p1[0])/(p2[0] - p1[0]) : (ptx[1] - p1[1])/(p2[1] - p1[1]);
            double ty = (p3[0] - p1[0] != 0) ? (pty[0] - p1[0])/(p3[0] - p1[0]) : (pty[1] - p1[1])/(p3[1] - p1[1]);

            double ux = ((1/((z2-z1) * tx + z1)) - 1/z1) /( (1/z2) -1/z1);
            double uy = ((1/((z3-z1) * ty + z1)) - 1/z1) /( (1/z3) -1/z1);

            ptgx = ux * (p2g - p1g) + p1g;     
            ptgy = uy * (p3g - p1g) + p1g;

            Vector<double> dPosX = ptgx - p1g;
            Vector<double> dPosY = ptgy - p1g;
            dPosX[3] = 0;
            dPosY[3] = 0;//its a vector


            double m1 = (v2[1] - v1[1])/(v2[0] - v1[0]);
            double m2 = (v1[1] - v0[1])/(v1[0] - v0[0]);
            double b2 = v1[1] - m2 * v1[0];
            double b1;

            int totalHeight = y2 - y0;

            for (int i = 0; i < totalHeight; i++)
            {
                bool secondHalf = i > y1 - y0 || y1 == y0;
                int segmentHeight = secondHalf ? y2 - y1 : y1 - y0;

                float alpha = (float)i / totalHeight;
                float beta = (float)(i - (secondHalf ? y1 - y0 : 0)) / segmentHeight;

                int ax = (int)(x0 + (x2 - x0) * alpha);
                int bx = secondHalf ? (int)(x1 + (x2 - x1) * beta) : (int)(x0 + (x1 - x0) * beta);

                if (ax > bx) { Swap(ref ax, ref bx); }

                for (int j = ax; j <= bx; j++)
                {   
                    Vector<double> posG = p1g + j * dPosY + i * dPosX;

                    b1 = j - m1 * (y0 + i);
                    double X = (b2 - b1) / (m1 - m2); //x intercept to AB
                    double Y = X * m2 + b2;
                    //double px = (v1[0] - X) / (v1[0] - v0[0]);//fraction of AB line from v1

                    Vector<double> normal;
                    if(!IsBase){
                        z1 = v0[2];
                        z2 = v1[2];

                        double t = (v1[0] - v0[0] != 0) ? (X - v0[0])/(v1[0] - v0[0]) : (Y - v0[1])/(v1[1] - v0[1]);
                        double u = ((1/((z2-z1) * t + z1)) - 1/z1) /( (1/z2) -1/z1);

                        normal = u * (Triangle.B.normal - Triangle.A.normal) + Triangle.A.normal;
                    }
                    else {
                        normal = Triangle.A.normal;
                    }

                    setPoint(j, y0 + i, calculatingPointColor(posG, normal, Ia));
                }
            }
        }
        public int calculatingPointColor(Vector<double> pos, Vector<double> normal, Vector<double> Ia){
            Vector<double> v = (Camera.FourD(camera.cPos) - pos).Normalize(2);
            Vector<double> li = (Camera.FourD(lightingPosition) - pos).Normalize(2);
            var ri = 2 * (normal * li) * normal - li;
            var Ii = Vector<double>.Build.DenseOfArray([0,0,0]);
            // var Ia = Vector<double>.Build.DenseOfArray([255,0,0]);

            var IRed =  Ia[0] * material.ambientReflectionCoefficient[0];
                    //+ material.diffuseReflectionCoefficient[0] * Ii[0] * Math.Max( normal * li, 0);
                    //+ material.specularReflectionCoefficient[0] * Ii[0] * Math.Max( v * ri, 0);

            var IGreen =  Ia[1] * material.ambientReflectionCoefficient[1];
                    //+ material.diffuseReflectionCoefficient[1] * Ii[1] * Math.Max( normal * li, 0);
                    //+ material.specularReflectionCoefficient[1] * Ii[1] * Math.Max( v * ri, 0);

            var IBlue =  Ia[2] * material.ambientReflectionCoefficient[2];
                    //+ material.diffuseReflectionCoefficient[2] * Ii[2] * Math.Max( normal * li, 0);
                    //+ material.specularReflectionCoefficient[2] * Ii[2] * Math.Max( v * ri, 0);
            
            return rgbToRgba8888(Math.Max(Math.Min((int)IRed, 255), 0) , Math.Max(Math.Min((int)IGreen, 255), 0), Math.Max(Math.Min((int)IBlue, 255), 0), 255);
        }

        private void Swap(ref Vector<double> v0, ref Vector<double> v1)
        {
            Vector<double> temp = v0;
            v0 = v1;
            v1 = temp;
        }
        private void Swap(ref int v0, ref int v1)
        {
            int temp = v0;
            v0 = v1;
            v1 = temp;
        }
        private void Swap(ref int x0, ref int y0, ref int x1, ref int y1)
        {
            int tempx = x0;
            int tempy = y0;
            x0 = x1;
            y0 = y1;
            x1 = tempx;
            y1 = tempy;
        }
    }
}