using GenericDataStore.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace GenericDataStore.Services
{
    public class ImageClassificationService
    {

        public ImageClassificationService() 
        {
            this.mlContext = new MLContext();

        }

        struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        string Name = "";
        MLContext mlContext;
        ITransformer model;
        IDataView trainingData;
        string _assetsPath = Path.Combine(Environment.CurrentDirectory, "AIModels");

        public MulticlassClassificationMetrics Init(ObjectType type, string name)
        {
            this.Name = name;
        //    foreach (var field in type.Field)
        //    {
        //        if(field.Type != "image")
        //        {
        //            List<ImageData> imageDatas = new List<ImageData>();
        //            var pathToSave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SaveFile").Value;
        //            foreach (var ob in type.DataObject)
        //            {
        //                Value filemeta = ob.Value.FirstOrDefault(x => x.Name == type.Field.FirstOrDefault(y => y.Type == "image" && x.Name == name).Name);
        //                if (filemeta != null && filemeta.ValueString != null && filemeta.ValueString != "")
        //                {
        //                    var fullPath = Path.Combine(pathToSave, filemeta.ValueString);
        //                    if (System.IO.File.Exists(fullPath))
        //                    {
        //                        imageDatas.Add(new ImageData() { ImagePath = filemeta.ValueString, Label = ob.Value.FirstOrDefault(x => x.Name == field.Name).ValueString});
        //                    }
        //                }
        //            }

        //            var dataView = mlContext.Data.LoadFromEnumerable(imageDatas);
        //            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        //            trainingData = trainTestData.TrainSet;
        //            var testData = trainTestData.TestSet;
        //            string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "tensorflow_inception_graph.pb");

        //            var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: pathToSave, inputColumnName: nameof(ImageData.ImagePath))
        //                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
        //                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
        //                .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
        //    ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
        //                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
        //                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
        //                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
        //.AppendCacheCheckpoint(mlContext);

        //            model = pipeline.Fit(trainingData);

        //            Save(type,field.Name);
        //        }
        //    }
            return null;
        }


        public List<ImagePrediction> ClassifySingleImage(DataObject ob,ObjectType type, string name)
        {
            List<ImagePrediction> predictions = new List<ImagePrediction>();
            foreach (var field in type.Field)
            {
                if (field.Type != "image")
                {
                    LoadFromFile(type, field.Name);
                    var imageData = new ImageData()
                    {

                        ImagePath = ob.Value.FirstOrDefault(x => x.Name == type.Field.FirstOrDefault(y => y.Type == "image" && y.Name == name).Name).ValueString,

                    };
                    var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
                    var prediction = predictor.Predict(imageData);
                    prediction.FieldName = field.Name;
                    predictions.Add(prediction);
                }
            }
               
            return predictions;
        }

        public void LoadFromFile(ObjectType type, string name)
        {
            string imagepath = Path.Combine(_assetsPath, "imagemodels");
            string path = Path.Combine(imagepath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            model = mlContext.Model.Load(final, out var schema);
        }

        public void Save(ObjectType type,string name)
        {
            string imagepath = Path.Combine(_assetsPath, "imagemodels");
            string path = Path.Combine(imagepath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(final));

            }
            mlContext.Model.Save(model, trainingData.Schema, final);
        }
    }

    public class ImageData
    {
        public string? ImagePath;

        public string? Label;

        public string? Label2;
    }

    public class ImagePrediction : ImageData
    {
        public float[]? Score;

        public string? PredictedLabelValue;

        public string? FieldName;

    }

}
