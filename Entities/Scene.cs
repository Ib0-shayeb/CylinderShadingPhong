using System;
using MathNet.Numerics.LinearAlgebra;

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

        public int[] pixels;

        public int width;
        public int height;

        public Scene(Cylinder cylinder, Vector<double> lightingPosition, Camera camera, int width, int height){
            this.camera = camera;
            this.cylinder = cylinder;
            this.lightingPosition = lightingPosition;
            this.width = width;
            this.height = height;

            pixels = new int[width * height];
        }

        public void drawPoints(){
            pixels = new int[width * height];
            
            for (int i = 0; i < cylinder.botPreVerticies.Count; i++){
                var v = camera.ProjectPoint(camera.AdjuctCam(cylinder.botPreVerticies[i]));
                int x = (int)Math.Round(v[0]);
                int y = (int)Math.Round(v[1]);

                if(x>width || y >height) return; // tmp
                pixels[y * width + x] = rgbToRgba8888(0, 0, 0, 255);

                v = camera.ProjectPoint(camera.AdjuctCam(cylinder.topPreVerticies[i]));
                x = (int)Math.Round(v[0]);
                y = (int)Math.Round(v[1]);
                pixels[y * width + x] = rgbToRgba8888(0, 0, 0, 255);
            }
        }

        public static int rgbToRgba8888(int r, int g, int b, int a){
            return (a << 24) | (b << 16) | (g << 8) | r;
        }
        
    }
}