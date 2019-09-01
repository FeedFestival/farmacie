using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.ML.Trainers;
using System.IO;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Utils;

public class MLFarmacie : MonoBehaviour
{
    private static MLFarmacie _mlFarmacie;
    public static MLFarmacie Instance { get { return _mlFarmacie; } }

    MLContext mlContext;
    ITransformer model;
    PredictionEngine<MedicRating, MedicRatingPrediction> _predictionEngine;
    PredictionEngine<FeedBackTrainingData, FeedBackPrediction> _dangerousPredictionsEngine;

    public string NumeSimtom = "Tuse";
    public string NumeMedicament = "Strepsils";

    public bool DangerousEngineCreated = false;

    void Awake()
    {
        _mlFarmacie = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        mlContext = new MLContext();

        (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);
        model = BuildAndTrainModel(trainingDataView);
        EvaluateModel(testDataView);
        _predictionEngine = mlContext.Model.CreatePredictionEngine<MedicRating, MedicRatingPrediction>(model);

        var simtom = Data.Instance.GetSimtoms().FirstOrDefault(s => s.nume == NumeSimtom);
        var medicament = Data.Instance.GetMedicamente().FirstOrDefault(m => m.nume == NumeMedicament);
        MakePrediction(simtom, medicament);
    }

    public (IDataView training, IDataView test) LoadData(MLContext mlContext)
    {
        var trainingPath = Data.Instance.TraingDataPath;
        var testingPath = Data.Instance.TraingDataPath;


        //var trainingDataPath = string.Format(@"Assets/StreamingAssets/{0}", trainingPath);
        //var testDataPath = string.Format(@"Assets/StreamingAssets/{0}", testingPath);

        var trainingDataPath = UsefullUtils.GetPathToStreamingAssetsFile(trainingPath);
        var testDataPath = UsefullUtils.GetPathToStreamingAssetsFile(testingPath);
        
        IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MedicRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
        IDataView testDataView = mlContext.Data.LoadFromTextFile<MedicRating>(testDataPath, hasHeader: true, separatorChar: ',');

        return (trainingDataView, testDataView);
    }

    public ITransformer BuildAndTrainModel(IDataView trainingDataView)
    {
        IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "simtomEncoded", inputColumnName: "simtom_id")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "pastilaEncoded", inputColumnName: "pastila_id"));

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "simtomEncoded",
            MatrixRowIndexColumnName = "pastilaEncoded",
            LabelColumnName = "Label",
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

        Debug.Log("=============== Training the model ===============");
        ITransformer model = trainerEstimator.Fit(trainingDataView);

        return model;
    }

    public void EvaluateModel(IDataView testDataView)
    {
        //Debug.Log("=============== Evaluating the model ===============");
        var prediction = model.Transform(testDataView);

        var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");

        //Debug.Log("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
        //Debug.Log("RSquared: " + metrics.RSquared.ToString());
    }

    public (string, bool, double) MakePrediction(Simtom simtom, Medicament medicament)
    {
        var medicRating = new MedicRating()
        {
            simtom_id = simtom.id,
            pastila_id = medicament.id
        };

        //Debug.Log("=============== Making a prediction ===============");

        var movieRatingPrediction = _predictionEngine.Predict(medicRating);

        string message = "Pastilele " + medicament.nume + " NU sunt recomandate pentru simtomul: " + simtom.nume;
        bool recomended = false;
        double score = Math.Round(movieRatingPrediction.Score, 1);

        if (score > 3.5)
        {
            message = "Pastilele " + medicament.nume + " sunt recomandate pentru simtomul " + simtom.nume;
            recomended = true;
        }

        //Debug.Log(message);
        return (message, recomended, score);
    }

    private (string, string) GetFeatureAndLabel(string[] values)
    {
        string features = string.Empty;
        string label = string.Empty;
        for (var i = 0; i < values.Length - 1; i++)
        {
            features += values[i];
        }
        label = values[values.Length - 1];

        return (features, label);
    }

    private List<FeedBackTrainingData> GetDataFromCsv(string filePath)
    {

        var data = new List<FeedBackTrainingData>();

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = System.Text.RegularExpressions.Regex.Unescape(reader.ReadLine());
                line = line.Replace("\"", "");
                var values = line.Split(',');

                (string features, string label) = GetFeatureAndLabel(values);
                var value = (label.ToLower() == "true" ? true : false);

                data.Add(
                    new FeedBackTrainingData()
                    {
                        Features = features,
                        Label = value
                    }
                );
            }
        }

        return data;
    }

    public void CreateDangerousEngine()
    {
        DangerousEngineCreated = true;

        //var trainingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TraingDangerousDataPath);
        //var testingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TestDangerousDataPath);
        var trainingFilePath = UsefullUtils.GetPathToStreamingAssetsFile(Data.Instance.TraingDangerousDataPath);
        var testingFilePath = UsefullUtils.GetPathToStreamingAssetsFile(Data.Instance.TestDangerousDataPath);

        var trainingData = GetDataFromCsv(trainingFilePath);
        var testData = GetDataFromCsv(testingFilePath);

        IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);
        IDataView testDataView = mlContext.Data.LoadFromEnumerable(testData);

        // Step 4 - we need to create the pipeline and define the workflows in it
        //                  switch params: ("FeedBackText", "Features") to => ("Features", "FeedbackText")
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Features")

            .Append(mlContext.BinaryClassification.Trainers
            .FastTree(numberOfLeaves: 50, numberOfTrees: 50, minimumExampleCountPerLeaf: 1));

        // Step 5 - traing the algorithm and get the model out
        var model = pipeline.Fit(dataView);

        var predictions = model.Transform(testDataView);
        var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

        // Step 7 - Use model
        _dangerousPredictionsEngine = mlContext.Model.CreatePredictionEngine
                                                <FeedBackTrainingData, FeedBackPrediction>
                                                (model);
    }

    public bool MakeDangerousPrediction(string nume)
    {
        var feedbackInput = new FeedBackTrainingData()
        {
            Features = nume
        };

        var feedbackPredicted = _dangerousPredictionsEngine.Predict(feedbackInput);

        return feedbackPredicted.PredictedLabel;
    }
}

public class MedicRating
{
    [LoadColumn(0)]
    public float simtom_id;
    [LoadColumn(2)]
    public float pastila_id;
    [LoadColumn(4)]
    public float Label;     // rating
}

public class MedicRatingPrediction
{
    public float Label;
    public float Score;
}
