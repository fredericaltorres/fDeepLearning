﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigitRecognizer.Core.Data;
using DigitRecognizer.Core.Extensions;
using DigitRecognizer.Core.Utilities;
using DigitRecognizer.MachineLearning.Pipeline;
using DigitRecognizer.MachineLearning.Infrastructure.Functions;
using DigitRecognizer.MachineLearning.Infrastructure.Initialization;
using DigitRecognizer.MachineLearning.Infrastructure.Models;
using DigitRecognizer.MachineLearning.Infrastructure.NeuralNetwork;
using DigitRecognizer.MachineLearning.Optimization.Optimizers;
using DigitRecognizer.MachineLearning.Providers;

namespace DigitRecognizer.Engine
{
    // 
    // https://www.youtube.com/watch?v=wgNZWnua-90
    // Building a Custom Neural Network in C# | AI and Machine Learning Tutorial (https://www.youtube.com/watch?v=XkXGrQ5RqgU)
    // https://www.youtube.com/watch?v=GW6ZkGvtREA C# embedding
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "Digit Recognizer - Engine";

            var learningRate = 0.00035;
            var epochs = 10;
            var regularizationFactor = 15.0;
            
            LearningPipeline pipeline = new LearningPipeline()
                .UseGradientClipping()
                .UseL2Regularization(regularizationFactor)
                .UseDropout(0.5)
                .SetWeightsInitializer(InitializerType.RandomInitialization)
                .SetEpochCount(epochs);

            var layers = new List<NnLayer>
            {
                //new NnLayer(784, 200, new LeakyRelu()),
                //new NnLayer(200, 100, new LeakyRelu()),
                //new NnLayer(100, 10, new Softmax())

                //new NnLayer(784, 15, new LeakyRelu()),
                //new NnLayer(15, 15, new LeakyRelu()),
                //new NnLayer(15, 10, new Softmax())

                new NnLayer(784, 64, new LeakyRelu()),
                new NnLayer(64,  32, new LeakyRelu()),
                new NnLayer(32,  10, new Softmax())

                //new NnLayer(784, 15, new LeakyRelu()),
                //new NnLayer(15, 10, new Softmax())
            };

            var nn = new NeuralNetwork(layers, learningRate);
            
            var optimizer = new MomentumOptimizer(nn, new CrossEntropy(), 0.93);

            var provider = new BatchDataProvider(DirectoryHelper.ExpandedTrainLabelsPath, DirectoryHelper.ExpandedTrainImagesPath, 100);

            pipeline.Add(optimizer);

            pipeline.Add(nn);

            pipeline.Add(provider);

            PredictionModel model = pipeline.Run();

            var provider1 = new BatchDataProvider(DirectoryHelper.TestLabelsPath, DirectoryHelper.TestImagesPath, 10000);
            var acc = 0.0;

            MnistImageBatch data = provider1.GetData();

            List<double[]> predictions = data.Pixels.Select(pixels => model.Predict(pixels)).ToList();
            
            for (var i = 0; i < data.Labels.Length; i++)
            {
                if (data.Labels[i] == predictions[i].ArgMax())
                {
                    acc++;
                }
            }
            
            acc /= 10000.0;

            Console.WriteLine($"Accuracy on the test data is: {acc:P2}");

            string basePath = Path.GetFullPath(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) +
                                               DirectoryHelper.ModelsFolder);

            string modelName = $"{Guid.NewGuid()}-{acc:N4}.nn";

            string filename = $"{basePath}/{modelName}";

            model.Save(filename);

            Console.ReadKey();
        }
    }
}
