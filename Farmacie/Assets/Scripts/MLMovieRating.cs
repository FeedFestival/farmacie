using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.ML.Trainers;
using System.IO;

public class MLMovieRating : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        MLContext mlContext = new MLContext();

        (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);

        ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);

        EvaluateModel(mlContext, testDataView, model);

        UseModelForSinglePrediction(mlContext, model);
    }

    public (IDataView training, IDataView test) LoadData(MLContext mlContext)
    {
        //var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
        //var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

        var trainingDataPath = string.Format(@"Assets/StreamingAssets/{0}", "recommendation-ratings-train.csv");
        var testDataPath = string.Format(@"Assets/StreamingAssets/{0}", "recommendation-ratings-test.csv");

        IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
        IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

        //var trainingDataPath = GetMovieRatings();
        //var testDataPath = GetMovieRatingsTest();

        //IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(trainingDataPath);
        //IDataView testDataView = mlContext.Data.LoadFromEnumerable(testDataPath);

        return (trainingDataView, testDataView);
    }

    public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
    {
        IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "userIdEncoded",
            MatrixRowIndexColumnName = "movieIdEncoded",
            LabelColumnName = "Label",
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

        Debug.Log("=============== Training the model ===============");
        ITransformer model = trainerEstimator.Fit(trainingDataView);

        return model;
    }

    public void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
    {
        Debug.Log("=============== Evaluating the model ===============");
        var prediction = model.Transform(testDataView);

        var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");

        Debug.Log("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
        Debug.Log("RSquared: " + metrics.RSquared.ToString());
    }

    public void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
    {
        Debug.Log("=============== Making a prediction ===============");
        var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

        var testInput = new MovieRating { userId = 6, movieId = 10 };

        var movieRatingPrediction = predictionEngine.Predict(testInput);

        if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
        {
            Debug.Log("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
        }
        else
        {
            Debug.Log("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
        }
    }

}

public class MovieRating
{
    [LoadColumn(0)]
    public float userId;
    [LoadColumn(1)]
    public float movieId;
    [LoadColumn(2)]
    public float Label;     // rating
}

public class MovieRatingPrediction
{
    public float Label;
    public float Score;
}
