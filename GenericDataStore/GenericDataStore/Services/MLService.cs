using GenericDataStore.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Globalization;
using System.IO;

namespace GenericDataStore.Services
{
    public class MLService
    {
        KMeansTrainer trainer;
        ITransformer transformer;
        int num = 0;
        public MLService(int num)
        {
            trainer = new KMeansTrainer(num);
           this.num = num;
        }

        public ClusteringMetrics TrainModel(IEnumerable<MLModel> query, List<bool> fieldtypestr)
        {
            
            transformer = trainer.Fit(query,fieldtypestr);
           
            var metrics = trainer.Evaluate();
            return metrics;
        }

        public Prediction Predict(MLModel model)
        {
            var predictor = new Predictor();
            var prediction = predictor.Predict(model,trainer.GetMLContext(),transformer);
            return prediction;
        }


        public static MLModel TransformDataObject(DataObject obj, List<Field> fields)
        {
         string nullValue = "-";
        MLModel model = new MLModel();
            var valuelist = obj.Value.ToList();
            for (int i = 0; i < 40; i++)
            {
                if (valuelist.Count > i)
                {
                    var value = valuelist.FirstOrDefault(x => x.Name == fields[i].Name)?.ValueString;

                    if (fields[i].Type == "numeric" || fields[i].Type == "calculatednumeric")
                    {
                        if (value != null && value.Contains("."))
                        {
                            typeof(MLModel).GetProperty("FieldFloat" + (i + 1)).SetValue(model, value != null && value != "" ? float.Parse(value, new CultureInfo("en-US")) : 0);

                        }
                        else
                        {
                            typeof(MLModel).GetProperty("FieldFloat" + (i + 1)).SetValue(model, value != null && value != "" ? float.Parse(value) : 0);

                        }
                        typeof(MLModel).GetProperty("Field" + (i + 1)).SetValue(model, nullValue);
                    }
                    else
                    {
                        typeof(MLModel).GetProperty("Field" + (i + 1)).SetValue(model, value != null ? value : nullValue);
                    }
                }
                else
                {
                    typeof(MLModel).GetProperty("Field" + (i + 1)).SetValue(model, nullValue);
                }
            }

            return model;
        }
    }

    public class Prediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }

    public interface ITrainerBase
    {
        string Name { get; }
        ITransformer Fit(IEnumerable<MLModel> query, List<bool> fieldtypestr);
        ClusteringMetrics Evaluate();
        void Save();
    }
    public abstract class TrainerBase<TParameters> : ITrainerBase
      where TParameters : class
    {
        public string Name { get; protected set; }

        protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "cluster.mdl");

        protected readonly MLContext MlContext;

        protected DataOperationsCatalog.TrainTestData _dataSplit;
        protected ITrainerEstimator<ClusteringPredictionTransformer<TParameters>, TParameters>
                                                      _model;
        protected ITransformer _trainedModel;

        protected TrainerBase()
        {
            MlContext = new MLContext(111);
        }
        public ITransformer Fit(IEnumerable<MLModel> query, List<bool> fieldtypestr)
        {
            _dataSplit = LoadAndPrepareData(query);
            var dataProcessPipeline = BuildDataProcessingPipeline(fieldtypestr);
            var trainingPipeline = dataProcessPipeline
                                    .Append(_model);

            _trainedModel = trainingPipeline.Fit(_dataSplit.TrainSet);
            return _trainedModel;
        }

        public ClusteringMetrics Evaluate()
        {
            var testSetTransform = _trainedModel.Transform(_dataSplit.TestSet);
            

            return MlContext.Clustering.Evaluate(
                data: testSetTransform,
                labelColumnName: "PredictedLabel",
                scoreColumnName: "Score",
                featureColumnName: "Features");
        }

        public MLContext GetMLContext()
        {
            return MlContext;
        }

        public void Save()
        {
            MlContext.Model.Save(_trainedModel, _dataSplit.TrainSet.Schema, ModelPath);
        }
        private EstimatorChain<ColumnConcatenatingTransformer> BuildDataProcessingPipeline(List<bool> fieldtypestr)
        {
            List<string> parameters = new List<string>();
            int idx = 1;
            foreach (var item in fieldtypestr)
            {
                if(item == false)
                {
                    parameters.Add("FieldFloat" + idx);
                }
                else
                {
                    parameters.Add("Field" + idx + "F");
                }
                idx++;
            }
            var dataProcessPipeline =
                MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field1", outputColumnName: "Field1F")
                .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field2", outputColumnName: "Field2F"))
                                .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field3", outputColumnName: "Field3F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field4", outputColumnName: "Field4F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field5", outputColumnName: "Field5F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field6", outputColumnName: "Field6F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field7", outputColumnName: "Field7F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field8", outputColumnName: "Field8F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field9", outputColumnName: "Field9F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field10", outputColumnName: "Field10F"))
                                 .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field11", outputColumnName: "Field11F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field12", outputColumnName: "Field12F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field13", outputColumnName: "Field13F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field14", outputColumnName: "Field14F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field15", outputColumnName: "Field15F"))
                                  .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field16", outputColumnName: "Field16F"))
                                   .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field17", outputColumnName: "Field17F"))
                                   .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field18", outputColumnName: "Field18F"))
                                   .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field19", outputColumnName: "Field19F"))
                                   .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field20", outputColumnName: "Field20F"))
                                   .Append(MlContext.Transforms.Text
                    .FeaturizeText(inputColumnName: "Field21", outputColumnName: "Field21F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field22", outputColumnName: "Field22F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field23", outputColumnName: "Field23F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field24", outputColumnName: "Field24F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field25", outputColumnName: "Field25F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field26", outputColumnName: "Field26F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field27", outputColumnName: "Field27F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field28", outputColumnName: "Field28F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field29", outputColumnName: "Field29F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field30", outputColumnName: "Field30F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field31", outputColumnName: "Field31F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field32", outputColumnName: "Field32F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field33", outputColumnName: "Field33F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field34", outputColumnName: "Field34F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field35", outputColumnName: "Field35F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field36", outputColumnName: "Field36F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field37", outputColumnName: "Field37F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field38", outputColumnName: "Field38F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field39", outputColumnName: "Field39F"))
                                   .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Field40", outputColumnName: "Field40F"))



                .Append(MlContext.Transforms.Concatenate("Features", parameters.ToArray()
                                               ))
               .AppendCacheCheckpoint(MlContext);

            return dataProcessPipeline;
        }

        private DataOperationsCatalog.TrainTestData LoadAndPrepareData(IEnumerable<MLModel> query)
        {
            var trainingDataView = MlContext.Data.LoadFromEnumerable<MLModel>(query);

            return MlContext.Data.TrainTestSplit(trainingDataView, testFraction: 0.3);
        }
    }

    public class KMeansTrainer : TrainerBase<KMeansModelParameters>
    {
        public KMeansTrainer(int numberOfClusters) : base()
        {
            Name = $"K Means Clulstering - {numberOfClusters} Clusters";
            _model = MlContext.Clustering.Trainers
                  .KMeans(numberOfClusters: numberOfClusters, featureColumnName: "Features");
        }
    }

    public class Predictor
    {


        public Predictor()
        {
        }
        public Prediction Predict(MLModel model, MLContext _mlContext, ITransformer transformer)
        {
            var predictionEngine = _mlContext.Model
           .CreatePredictionEngine<MLModel, Prediction>(transformer);

            return predictionEngine.Predict(model);
        }
     }

    public class MLModel
    {
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
