using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CylinderShadingPhong.Entities;
using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Avalonia.Controls;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace CylinderShadingPhong.Views;

public partial class MainWindow : Window
{
    private WriteableBitmap _bitmap;
    private WriteableBitmap _bitmap2;

    readonly Material material;
    readonly int width, height;
    Camera camera;
    readonly Vector<double> lightingPosition;
    readonly Cylinder cylinder;
    readonly Scene scene;
    const int iShowSpeed = 2;
    PixelFormat pixelFormat = PixelFormat.Rgba8888;
    AlphaFormat alphaFormat = AlphaFormat.Premul;
    public MainWindow()
    {
        material = new Material{
            ambientReflectionCoefficient = Vector<double>.Build.DenseOfArray([0,0,0,0]),
            diffuseReflectionCoefficient = Vector<double>.Build.DenseOfArray([0,0,0,0]),
            specularReflectionCoefficient = Vector<double>.Build.DenseOfArray([0,0,0,0])
        };
        width = 10; height = 10;
        camera = new Camera(Vector<double>.Build.DenseOfArray([0,20,20]), Vector<double>.Build.DenseOfArray([0,0,0]), width, height);
        lightingPosition = Vector<double>.Build.DenseOfArray([500,500,500]);
        cylinder = new Cylinder(10, 4, Vector<double>.Build.DenseOfArray([0,0,0,1]), material);

        scene = new Scene(cylinder, lightingPosition, camera, width, height);

        InitializeComponent();

        scene.drawPoints();
        UpdateCanvas(scene);

        this.KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([-iShowSpeed,0,0]);
                scene.pixels = new int[width * height];
                for(int i = 0;i<scene.pixels.Length; i++){
                    scene.pixels[i] = Scene.rgbToRgba8888(255, 255, 255, 255);
                }
                break;
            case Key.Down:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([iShowSpeed,0,0]);
                break;
            case Key.Left:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([0,0,iShowSpeed]);
                break;
            case Key.Right:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([0,0,-iShowSpeed]);
                break;
             case Key.Space:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([0,iShowSpeed,0]);
                break;
             case Key.C:
                scene.camera.cPos += Vector<double>.Build.DenseOfArray([0,-iShowSpeed,0]);
                break;
        
            default:
                return;
        }

        scene.drawPoints();
        UpdateCanvas(scene);
    }
    private void UpdateCanvas(Scene scene)
    {
        // Initialize the WriteableBitmap with the desired size
        int width = scene.width;
        int height = scene.height;

        _bitmap = CreateBitmapFromPixels(scene.pixels, width, height);
        
    }
    public WriteableBitmap CreateBitmapFromPixels(int[] pixels, int width, int height)
    {
        // Ensure the dimensions match the pixel array length
        if (pixels.Length != width * height)
        {
            throw new ArgumentException("The length of the pixels array does not match the width and height specified.");
        }

        // Create a WriteableBitmap
        var pixelSize = new PixelSize(width, height);
        var dpi = new Avalonia.Vector(96, 96); // Default DPI
        var format = PixelFormat.Rgba8888;
        var alphaFormat = AlphaFormat.Premul;

        var writeableBitmap = new WriteableBitmap(pixelSize, dpi, format, alphaFormat);

        // Lock the WriteableBitmap to get access to its pixel buffer
        if(first == true){
        //first = false;
        using (var bitmapLock = writeableBitmap.Lock())
        {
            Marshal.Copy(pixels, 0, bitmapLock.Address, pixels.Length);
        }
        // Return the created WriteableBitmap
        return writeableBitmap;
    }
}