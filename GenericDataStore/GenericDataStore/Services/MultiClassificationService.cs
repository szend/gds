using GenericDataStore.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Text;
using System.Security.Cryptography.Xml;

namespace GenericDataStore.Services
{
    public class MultiClassificationService
    {
        LbfgsMaximumEntropyTrainer trainer;
        ITransformer transformer;
        public MultiClassificationService()
        {
            trainer = new LbfgsMaximumEntropyTrainer();
        }

        public MulticlassClassificationMetrics TrainModel(List<MLModelRegression> query, List<bool> fieldtypestr)
        {

            transformer = trainer.Fit(query, fieldtypestr);

            var metrics = trainer.Evaluate();
            return metrics;
        }

        public PredictionClass Predict(MLModelRegression model)
        {
            var predictor = new ClassificationPredictor();
            var prediction = predictor.Predict(model, trainer.GetMLContext(), trainer.GetTransformer());
            return prediction;
        }

        public void SaveModel(ObjectType type, string name)
        {
            trainer.Save(type, name);
        }

        public void LoadModel(ObjectType type, string name)
        {
            trainer.LoadFromFile(type, name);
        }


    }

    public class ClassificationPredictor
    {


        public ClassificationPredictor()
        {
        }
        public PredictionClass Predict(MLModelRegression model, MLContext _mlContext, ITransformer transformer)
        {
            var predictionEngine = _mlContext.Model
           .CreatePredictionEngine<MLModelRegression, PredictionClass>(transformer);

            return predictionEngine.Predict(model);
        }
    }

    public class PredictionClass
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel;

        [ColumnName("Score")]
        public float[] Distances;
    }
    public abstract class TrainerBaseClassification<TParameters>
  where TParameters : class
    {
        public string Name { get; protected set; }

        protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "cluster.mdl");

        protected readonly MLContext MlContext;
        string _assetsPath = Path.Combine(Environment.CurrentDirectory, "AIModels");

        protected DataOperationsCatalog.TrainTestData _dataSplit;
        protected ITrainerEstimator<MulticlassPredictionTransformer<TParameters>, TParameters>
                                                      _model;
        protected ITransformer _trainedModel;

        protected TrainerBaseClassification()
        {
            MlContext = new MLContext(111);
        }
        public ITransformer Fit(List<MLModelRegression> query, List<bool> fieldtypestr)
        {
            _dataSplit = LoadAndPrepareData(query);
            var dataProcessPipeline = BuildDataProcessingPipeline(fieldtypestr,query);
            var trainingPipeline = dataProcessPipeline
                                    .Append(_model)
                                    .Append(MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = trainingPipeline.Fit(_dataSplit.TrainSet);
            return _trainedModel;
        }

        public MulticlassClassificationMetrics Evaluate()
        {
            var testSetTransform = _trainedModel.Transform(_dataSplit.TestSet);


            return MlContext.MulticlassClassification.Evaluate(
                data: testSetTransform
                );
        }

        public MLContext GetMLContext()
        {
            return MlContext;
        }
        public void LoadFromFile(ObjectType type, string name)
        {
            string regpath = Path.Combine(_assetsPath, "classificationmodels");
            string path = Path.Combine(regpath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            _trainedModel = MlContext.Model.Load(final, out var schema);
        }

        public ITransformer GetTransformer()
        {
            return _trainedModel;
        }

        public void Save(ObjectType type, string name)
        {
            string regpath = Path.Combine(_assetsPath, "classificationmodels");
            string path = Path.Combine(regpath, type.ObjectTypeId.ToString());
            string final = Path.Combine(path, name + ".mdl");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(final));

            }
            MlContext.Model.Save(_trainedModel, _dataSplit.TrainSet.Schema, final);
        }
        private EstimatorChain<NormalizingTransformer> BuildDataProcessingPipeline(List<bool> fieldtypestr, List<MLModelRegression> models)
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
            ValueToKeyMappingEstimator tokey;
            if (models[0].PredictStr != RegressionService.nullValue)
            {
                tokey = MlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: "PredictStr");
                EstimatorChain<ITransformer>? encoded = new EstimatorChain<ITransformer>();
                for (int i = 1; i < 41; i++)
                {

                   encoded = encoded.Append(MlContext.Transforms.Text.FeaturizeText(outputColumnName: "Field" + i + "Encoded", inputColumnName: "Field" + i));


                }

                var pipeline1 = tokey.Append(encoded).Append(MlContext.Transforms.Concatenate("Features", parameters.ToArray()))
                 .Append(MlContext.Transforms.NormalizeMinMax("Features", "Features"))
               .AppendCacheCheckpoint(MlContext);

                return pipeline1;
            }
            return null;

        }
    

        private DataOperationsCatalog.TrainTestData LoadAndPrepareData(IEnumerable<MLModelRegression> query)
        {
            var trainingDataView = MlContext.Data.LoadFromEnumerable<MLModelRegression>(query);

            return MlContext.Data.TrainTestSplit(trainingDataView, testFraction: 0.3);
        }
    }


    public class LbfgsMaximumEntropyTrainer : TrainerBaseClassification<MaximumEntropyModelParameters>
    {
        public LbfgsMaximumEntropyTrainer() : base()
        {
            Name = "LBFGS Maximum Entropy";
            _model = MlContext.MulticlassClassification.Trainers
              .LbfgsMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features");
        }
    }
}
