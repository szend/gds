using System;

using System.Collections.Generic;

using Microsoft.ML;

using Microsoft.ML.Data;



namespace CodeGenerationML

{

    public class Program

    {

        // Bemeneti adat osztálya

        public class InputData

        {

            [LoadColumn(0)]

            public string Keywords { get; set; }



            [LoadColumn(1)]

            public string Fields { get; set; }



            [LoadColumn(2)]

            public int Flag { get; set; }

        }



        // Kimeneti adat osztálya

        public class GeneratedCode

        {

            public string OutputCode { get; set; }

        }



        static void Main(string[] args)

        {

            var mlContext = new MLContext();



            // Példa adatok feltöltése

            var trainingData = new List<InputData>

            {

                new InputData { Keywords = "chart, graph", Fields = "Sales, Year", Flag = 1 },

                new InputData { Keywords = "color, palette", Fields = "Theme, Hue", Flag = 2 },

                new InputData { Keywords = "column, calculation", Fields = "Price, Tax", Flag = 3 }

            };



            // Training Data betöltése

            var trainingDataView = mlContext.Data.LoadFromEnumerable(trainingData);



            // Text featurization pipeline

            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("KeywordsFeaturized", nameof(InputData.Keywords))

                .Append(mlContext.Transforms.Text.FeaturizeText("FieldsFeaturized", nameof(InputData.Fields)))

                .Append(mlContext.Transforms.Concatenate("Features", "KeywordsFeaturized", "FieldsFeaturized"));



            // Regresszor modell (kód generáláshoz egyszerű regressziót használunk itt)

            var trainer = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(InputData.Flag))

                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())

                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));



            var trainingPipeline = dataProcessPipeline.Append(trainer);



            // Modell betanítása

            var model = trainingPipeline.Fit(trainingDataView);



            // Predikciós adatok

            var testData = new List<InputData>

            {

                new InputData { Keywords = "chart, bar", Fields = "Revenue, Month", Flag = 1 },

                new InputData { Keywords = "gradient, shade", Fields = "Color, Intensity", Flag = 2 },

                new InputData { Keywords = "aggregate, column", Fields = "Quantity, Discount", Flag = 3 }

            };



            var testDataView = mlContext.Data.LoadFromEnumerable(testData);



            // Predikció

            var predictions = model.Transform(testDataView);

            var predictedResults = mlContext.Data.CreateEnumerable<GeneratedCode>(predictions, reuseRowObject: false);



            // Eredmények kiíratása

            Console.WriteLine("Generated Code Predictions:");

            foreach (var prediction in predictedResults)

            {

                Console.WriteLine($"Generated Code: {prediction.OutputCode}");

            }

        }

    }

}

