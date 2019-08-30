using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.ML.Trainers;
using System.IO;

public class MLFarmacie : MonoBehaviour
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

        IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating2>(trainingDataPath, hasHeader: true, separatorChar: ',');
        IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating2>(testDataPath, hasHeader: true, separatorChar: ',');

        //var trainingDataPath = GetMovieRating2s();
        //var testDataPath = GetMovieRating2sTest();

        //IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(trainingDataPath);
        //IDataView testDataView = mlContext.Data.LoadFromEnumerable(testDataPath);

        return (trainingDataView, testDataView);
    }

    public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
    {
        IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userEncoded", inputColumnName: "user")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "moviesEncoded", inputColumnName: "movies"));

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "userEncoded",
            MatrixRowIndexColumnName = "moviesEncoded",
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
        var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating2, MovieRating2Prediction>(model);

        var testInput = new MovieRating2 { user = 6, movies = 10 };

        var movieRatingPrediction = predictionEngine.Predict(testInput);

        if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
        {
            Debug.Log("Movie " + testInput.movies + " is recommended for user " + testInput.user);
        }
        else
        {
            Debug.Log("Movie " + testInput.movies + " is not recommended for user " + testInput.user);
        }
    }

}

public class MovieRating2
{
    [LoadColumn(0)]
    public float user;
    [LoadColumn(1)]
    public float movies;
    [LoadColumn(2)]
    public float Label;     // rating
}

public class MovieRating2Prediction
{
    public float Label;
    public float Score;
}
