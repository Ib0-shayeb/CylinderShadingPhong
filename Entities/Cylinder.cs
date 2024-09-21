using System;
using System.Collections.Generic;
using System.Linq;
using CylinderShadingPhong.Entities;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CylinderShadingPhong.Entities
{
    public class Cylinder
    {
        public float height;
        public float radius;
        public Vector<double> botBaseCenter;
        public Vector<double> topBaseCenter;
        //vertices
        public List<CylinderTriangle> topTriangles = new List<CylinderTriangle>();
        public List<CylinderTriangle> botTriangles = new List<CylinderTriangle>();
        public List<CylinderTriangle> sideTriangles = new List<CylinderTriangle>();

        public List<Vector<double>> topPreVerticies = new List<Vector<double>>();
        public List<Vector<double>> botPreVerticies = new List<Vector<double>>();


        //material coeficients
        public Material material {get; set;} = new Material();

        //method to create triangles
        public Cylinder(float height, float radius, Vector<double> botBaseCenter, Material material){
            this.height = height;
            this.radius = radius;
            this.botBaseCenter = Vector<double>.Build.DenseOfVector(botBaseCenter);

            topBaseCenter = Vector<double>.Build.DenseOfVector( botBaseCenter);
            topBaseCenter[1] += height;
            this.material = new Material{
                ambientReflectionCoefficient = material.ambientReflectionCoefficient,
                diffuseReflectionCoefficient = material.diffuseReflectionCoefficient,
                specularReflectionCoefficient = material.specularReflectionCoefficient
            };

            //verticies coordinates
            int n = 100;
            
            for ( int i = 1; i < n + 1; i++){
                topPreVerticies.Add(Vector<double>.Build.DenseOfArray([
                    (float)(radius * Math.Cos(2 * Math.PI * i / n)), height, (float)(radius * Math.Sin(2 * Math.PI * i / n)), 1
                ]));

                botPreVerticies.Add(Vector<double>.Build.DenseOfArray([
                    (float)(radius * Math.Cos(2 * Math.PI * i / n)), 0, (float)(radius * Math.Sin(2 * Math.PI * i / n)), 1
                ]));
            }

            Vector<double> topBaseNormal = Vector<double>.Build.DenseOfArray([0,1,0,0]);
            Vector<double> botBaseNormal = Vector<double>.Build.DenseOfArray([0,-1,0,0]);
            //creatre triangles
            for ( int i = 0; i < n; i++){
                topTriangles.Add(new CylinderTriangle{
                    A = new CylinderVertex{position = topPreVerticies[i], normal = topBaseNormal},
                    B = new CylinderVertex{position = topPreVerticies[(i + 1) % (n)], normal = topBaseNormal},
                    C = new CylinderVertex{position = topBaseCenter, normal = topBaseNormal},
                });
                botTriangles.Add(new CylinderTriangle{
                    A = new CylinderVertex{position = botPreVerticies[i], normal = botBaseNormal},
                    B = new CylinderVertex{position = botPreVerticies[(i + 1) % (n)], normal = botBaseNormal},
                    C = new CylinderVertex{position = botBaseCenter, normal = botBaseNormal},
                });
                sideTriangles.Add(new CylinderTriangle{
                    B = new CylinderVertex{position = botPreVerticies[i],
                     normal = Vector<double>.Build.DenseOfArray([botPreVerticies[i][0] / radius, 0, botPreVerticies[i][2] / radius, 0])},
                    A = new CylinderVertex{position = botPreVerticies[(i + 1) % (n)],
                     normal = Vector<double>.Build.DenseOfArray([botPreVerticies[(i + 1) % (n)][0] / radius, 0, botPreVerticies[(i + 1) % (n)][2] / radius, 0])},
                    C = new CylinderVertex{position = topPreVerticies[i],
                     normal = Vector<double>.Build.DenseOfArray([topPreVerticies[i][0] / radius, 0, topPreVerticies[i][2] / radius, 0])}
                });
                sideTriangles.Add(new CylinderTriangle{
                    A = new CylinderVertex{position = topPreVerticies[i],
                     normal = Vector<double>.Build.DenseOfArray([topPreVerticies[i][0] / radius, 0, topPreVerticies[i][2] / radius, 0])},
                    B = new CylinderVertex{position = topPreVerticies[(i + 1) % (n)],
                     normal = Vector<double>.Build.DenseOfArray([topPreVerticies[(i + 1) % (n)][0] / radius, 0, topPreVerticies[(i + 1) % (n)][2] / radius, 0])},
                    C = new CylinderVertex{position = botPreVerticies[(i + 1) % (n)],
                     normal = Vector<double>.Build.DenseOfArray([botPreVerticies[(i + 1) % (n)][0] / radius, 0, botPreVerticies[(i + 1) % (n)][2] / radius, 0])}
                });
            }
        }
    }
}