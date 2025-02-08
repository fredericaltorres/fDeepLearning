using System;
using System.Collections.Generic;
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
    static void CreateGrayscaleImage(double[] pixels, string outputPath)
    {
        if (pixels.Length != 784) // 28 * 28
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
                    double pixelValue = pixels[y * 28 + x];
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


    public static List<List<double>> ReadDoubleValues(string filePath)
    {
        try
        {
            List<List<double>> result = new List<List<double>>();

            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            // Process each line
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    // Split the line by comma and convert each value to double
                    List<double> rowValues = line.Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => double.Parse(x.Trim()))
                        .ToList();

                    result.Add(rowValues);
                }
            }

            return result;
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
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
            var imagesPixels = ReadDoubleValues(@".\mnist_output.txt");
            var index = 0;
            foreach(var image in imagesPixels)
            {
                CreateGrayscaleImage(image.ToArray(), Path.Combine(outputFolder, $"image_{index:000000}.bmp"));
                if(index % 1000 == 0)
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
}
