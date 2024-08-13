using GenericDataStore.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Transforms;
using System.Globalization;
using Tensorflow.Contexts;
using static Microsoft.ML.DataOperationsCatalog;

namespace GenericDataStore.Services
{
    public class RegressionService
    {
        MLContext mlContext;
        string _assetsPath = Path.Combine(Environment.CurrentDirectory, "AIModels");
        ITransformer model;
        public static string nullValue = "-";
        IDataView trainingData;
        public RegressionService()
        {
            this.mlContext = new MLContext(seed: 0);

        }

        public RegressionMetrics Init(List<MLModelRegression> models, List<MLModelRegression> testmodels, List<bool> fieldtypestr)
        {
            var model = Train(models, fieldtypestr,testmodels);
            return Evaluate(testmodels);
        }

        ITransformer Train(List<MLModelRegression> models,List<bool> fieldtypestr, List<MLModelRegression> testmodels)
        {
            List<string> parameters = new List<string>();
            int idx = 1;
            foreach (var item in fieldtypestr)
            {
                if (item == false)
                {
                    parameters.Add("FieldFloat" + idx);
                }
                else
                {
                    parameters.Add("Field" + idx + "Encoded");
                }
                idx++;
            }
            IDataView dataView = mlContext.Data.LoadFromEnumerable<MLModelRegression>(models);
            trainingData = dataView;
            ColumnCopyingEstimator colcopy;
            if (models[0].PredictStr == nullValue)
            {
                colcopy = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "PredictFloat");
                EstimatorChain<OneHotEncodingTransformer>? encoded = new EstimatorChain<OneHotEncodingTransformer>();
                for (int i = 1; i < 41; i++)
                {

                    encoded = encoded.Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "Field" + i + "Encoded", inputColumnName: "Field" + i));

                    
                }

                var pipeline1 = colcopy.Append(encoded).Append(mlContext.Transforms.Concatenate("Features", parameters.ToArray()))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
  .Append(mlContext.Regression.Trainers.FastForest());

                var model1 = pipeline1.Fit(dataView);
                var res = Evaluate(testmodels, model1);

                var pipeline2 = colcopy.Append(encoded).Append(mlContext.Transforms.Concatenate("Features", parameters.ToArray()))
                                    .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                  .Append(mlContext.Regression.Trainers.FastTreeTweedie(numberOfLeaves: 40, numberOfTrees: 200));

                var model2 = pipeline2.Fit(dataView);
                var res2 = Evaluate(testmodels, model2);

                //var pipeline3 = colcopy.Append(encoded).Append(mlContext.Transforms.Concatenate("Features", parameters.ToArray()))
                //.Append(mlContext.Regression.Trainers.Sdca());

                //var model3 = pipeline3.Fit(dataView);
                //var res3 = Evaluate(testmodels, model3);

                var pipeline4 = colcopy.Append(encoded).Append(mlContext.Transforms.Concatenate("Features", parameters.ToArray()))
                 .Append(mlContext.Regression.Trainers.Gam());

                var model4 = pipeline4.Fit(dataView);
                var res4 = Evaluate(testmodels, model4);

                if (res.MeanAbsoluteError < res2.MeanAbsoluteError /*&& res.MeanAbsoluteError < res3.MeanAbsoluteError*/ && res.MeanAbsoluteError < res4.MeanAbsoluteError)
                {
                    model = model1;
                }
                else if (res2.MeanAbsoluteError < res.MeanAbsoluteError /*&& res2.MeanAbsoluteError < res3.MeanAbsoluteError*/ && res2.MeanAbsoluteError < res4.MeanAbsoluteError)
                {
                    model = model2;
                }
                //else if (res3.MeanAbsoluteError < res.MeanAbsoluteError && res3.MeanAbsoluteError < res2.MeanAbsoluteError && res3.MeanAbsoluteError < res4.MeanAbsoluteError)
                //{
                //    model = model3;
                //}
                else
                {
                    model = model4;
                }

                return model;

            }
            else
            {
                return null;
            }
            
        }

        RegressionMetrics Evaluate(List<MLModelRegression> models, ITransformer testmodel = null)
        {
            if(model == null && testmodel == null)
            {
                return null;
            }
            if(testmodel == null)
            {
                IDataView dataView = mlContext.Data.LoadFromEnumerable<MLModelRegression>(models);
                var predictions = model.Transform(dataView);
                var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
                return metrics;
            }
            else
            {
                IDataView dataView = mlContext.Data.LoadFromEnumerable<MLModelRegression>(models);
                var predictions = testmodel.Transform(dataView);
                var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
                return metrics;
            }

        }

        public RegressionPrediction TestSinglePrediction(MLModelRegression value)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<MLModelRegression, RegressionPrediction>(model);
            var prediction = predictionFunction.Predict(value);
            return prediction;
        }

        public void LoadFromFile(ObjectType type, string name)
        {
            string regpath = Path.Combine(_assetsPath, "regressionmodels");
            string path = Path.Combine(regpath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            model = mlContext.Model.Load(final, out var schema);
        }

        public void Save(ObjectType type, string name)
        {
            string regpath = Path.Combine(_assetsPath, "regressionmodels");
            string path = Path.Combine(regpath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(final));

            }
            mlContext.Model.Save(model, trainingData.Schema, final);
        }

        public static MLModelRegression TransformDataObject(DataObject obj, List<Field> fields, string name)
        {
            MLModelRegression model = new MLModelRegression();
            var valuelist = obj.Value.ToList();
            for (int i = 0; i < 40; i++)
            {
                if (valuelist.Count > i)
                {
                    var value = valuelist.FirstOrDefault(x => x.Name == fields[i].Name)?.ValueString;
                    if (fields[i].Name == name)
                    {
                        typeof(MLModelRegression).GetProperty("Field"+ (i+1)).SetValue(model, nullValue);
                        if (fields[i].Type == "numeric")
                        {
                            model.PredictStr = nullValue;
                            model.PredictFloat = value != null && value != "" ? float.Parse(value, new CultureInfo("en-US")) : 0;
                        }
                        else
                        {
                            model.PredictStr = valuelist != null ? value : nullValue;
                        }
                    }
                    else if (fields[i].Type == "numeric" || fields[i].Type == "calculatednumeric")
                    {
                        typeof(MLModelRegression).GetProperty("FieldFloat" + (i + 1)).SetValue(model, value != null && value != "" ? float.Parse(value, new CultureInfo("en-US")) : 0);
                        typeof(MLModelRegression).GetProperty("Field" + (i + 1)).SetValue(model, nullValue);
                    }
                    else
                    {
                        typeof(MLModelRegression).GetProperty("Field" + (i + 1)).SetValue(model, value != null ? value : nullValue);
                    }
                }
                else
                {
                    typeof(MLModelRegression).GetProperty("Field" + (i + 1)).SetValue(model, nullValue);
                }
            }

            return model;
        }

    }

    public class RegressionPrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }

    public class MLModelRegression
    {
        public string? PredictStr { get; set; }
        public float PredictFloat { get; set; }

        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? Field7 { get; set; }
        public string? Field8 { get; set; }
        public string? Field9 { get; set; }
        public string? Field10 { get; set; }
        public string? Field11 { get; set; }
        public string? Field12 { get; set; }
        public string? Field13 { get; set; }
        public string? Field14 { get; set; }
        public string? Field15 { get; set; }
        public string? Field16 { get; set; }
        public string? Field17 { get; set; }
        public string? Field18 { get; set; }
        public string? Field19 { get; set; }
        public string? Field20 { get; set; }
        public string? Field21 { get; set; }
        public string? Field22 { get; set; }
        public string? Field23 { get; set; }
        public string? Field24 { get; set; }
        public string? Field25 { get; set; }
        public string? Field26 { get; set; }
        public string? Field27 { get; set; }
        public string? Field28 { get; set; }
        public string? Field29 { get; set; }

        public string? Field30 { get; set; }
        public string? Field31 { get; set; }
        public string? Field32 { get; set; }
        public string? Field33 { get; set; }
        public string? Field34 { get; set; }
        public string? Field35 { get; set; }
        public string? Field36 { get; set; }
        public string? Field37 { get; set; }

        public string? Field38 { get; set; }
        public string? Field39 { get; set; }
        public string? Field40 { get; set; }



        public float FieldFloat1 { get; set; }
        public float FieldFloat2 { get; set; }
        public float FieldFloat3 { get; set; }
        public float FieldFloat4 { get; set; }
        public float FieldFloat5 { get; set; }
        public float FieldFloat6 { get; set; }
        public float FieldFloat7 { get; set; }
        public float FieldFloat8 { get; set; }
        public float FieldFloat9 { get; set; }

        public float FieldFloat10 { get; set; }
        public float FieldFloat11 { get; set; }
        public float FieldFloat12 { get; set; }
        public float FieldFloat13 { get; set; }
        public float FieldFloat14 { get; set; }
        public float FieldFloat15 { get; set; }
        public float FieldFloat16 { get; set; }
        public float FieldFloat17 { get; set; }
        public float FieldFloat18 { get; set; }
        public float FieldFloat19 { get; set; }
        public float FieldFloat20 { get; set; }
        public float FieldFloat21 { get; set; }
        public float FieldFloat22 { get; set; }
        public float FieldFloat23 { get; set; }
        public float FieldFloat24 { get; set; }

        public float FieldFloat25 { get; set; }
        public float FieldFloat26 { get; set; }
        public float FieldFloat27 { get; set; }
        public float FieldFloat28 { get; set; }
        public float FieldFloat29 { get; set; }
        public float FieldFloat30 { get; set; }
        public float FieldFloat31 { get; set; }
        public float FieldFloat32 { get; set; }
        public float FieldFloat33 { get; set; }
        public float FieldFloat34 { get; set; }
        public float FieldFloat35 { get; set; }
        public float FieldFloat36 { get; set; }
        public float FieldFloat37 { get; set; }
        public float FieldFloat38 { get; set; }
        public float FieldFloat39 { get; set; }
        public float FieldFloat40 { get; set; }


    }
}
