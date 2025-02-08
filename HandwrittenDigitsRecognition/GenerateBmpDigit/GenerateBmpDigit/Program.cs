using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

class BitmapCreator
{
    public static double Clamp(double val, double min, double max)
    {
        if (val.CompareTo(min) < 0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }
    static void CreateGrayscaleImage(ImageTrainingData pixels, string outputPath)
    {
        if (pixels.Values.Count != 784) // 28 * 28
            throw new ArgumentException("Input array must contain exactly 784 values");

        // Create a new 28x28 Bitmap
        using (Bitmap bmp = new Bitmap(28, 28, PixelFormat.Format8bppIndexed))
        {
            // Set up the grayscale palette
            ColorPalette palette = bmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = palette;

            // Lock the bitmap's bits
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            // Create array to hold the byte values
            byte[] imageData = new byte[bmpData.Stride * bmpData.Height];

            // Convert doubles to bytes and copy to image data
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    double pixelValue = pixels.Values[y * 28 + x];
                    byte byteValue = (byte)(Clamp(pixelValue, 0, 1) * 255);
                    imageData[y * bmpData.Stride + x] = byteValue;
                }
            }

            // Copy the bytes to the bitmap
            Marshal.Copy(imageData, 0, bmpData.Scan0, imageData.Length);

            // Unlock the bits
            bmp.UnlockBits(bmpData);

            // Save the bitmap
            bmp.Save(outputPath, ImageFormat.Bmp);
        }
    }

    public class ImageTrainingData
    {
        public int Digit { get; set; }
        public List<double> Values { get; set; } = new List<double>();
    }
    

    public static List<ImageTrainingData> ReadDoubleValues(string filePath)
    {
        try
        {
            var r = new List<ImageTrainingData>();
            var lines = File.ReadAllLines(filePath);
            var xx = 0;
            while (xx < lines.Length)
            {
                var line = lines[xx];
                var itd = new ImageTrainingData { 
                    Digit = int.Parse(line)
                };
                line = lines[xx+1];
                if (!string.IsNullOrWhiteSpace(line))
                {
                    // Split the line by comma and convert each value to double
                    itd.Values = line.Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => double.Parse(x.Trim()))
                        .ToList();

                    r.Add(itd);
                }
                xx += 2;
            }

            return r;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        catch (FormatException)
        {
            throw new FormatException("File contains invalid numeric values");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading file: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        try
        {
            var outputFolder = @".\output";
            CreateDirIfNeeded(outputFolder);
            Print("Reading...");
            var imagesPixels = ReadDoubleValues(@".\mnist_output.txt");
            var index = 0;
            Print("Creating images...");
            foreach (var image in imagesPixels)
            {
                var outputFolder2 = Path.Combine(outputFolder, $"{image.Digit}");
                CreateDirIfNeeded(outputFolder2);
                var outputPath = Path.Combine(outputFolder2, $"img_{image.Digit}_{index:000000}.bmp");
                CreateGrayscaleImage(image, outputPath);
                if (index % 1000 == 0)
                    Console.WriteLine($"Image {index} created successfully!");
                index++;
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating image: {ex.Message}");
        }
    }

    private static void Print(string v)
    {
        Console.WriteLine(v);
    }

    private static void CreateDirIfNeeded(string outputFolder2)
    {
        if (!Directory.Exists(outputFolder2))
            Directory.CreateDirectory(outputFolder2);
    }
}
