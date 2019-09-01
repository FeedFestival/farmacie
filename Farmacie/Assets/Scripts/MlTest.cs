using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.ML;
using Microsoft.ML.FastTree;
using Assets.Scripts;
using UnityEngine.UI;

public class MlTest : MonoBehaviour
{
    public InputField InputField;
    public Text PredictedText;

    public string TestText;

    List<FeedBackTrainingData> trainingData;
    List<FeedBackTrainingData> testData;

    // Start is called before the first frame update
    void Start()
    {
        //InputField.text = "Terrible";
        TestText = "Awesome";

        Init();
    }

    private void Init()
    {
        // Step 1 - load training data
        trainingData = new List<FeedBackTrainingData>();
        LoadTrainingData();

        // Step 2 - Create ml Context
        var mlContext = new MLContext();

        //  Step 3 - Convert your data
        //                          mlContext.CreateStreamingDataView<FeedBackTrainingData>()
        IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);

        // Step 4 - we need to create the pipeline and define the workflows in it
        //                  switch params: ("FeedBackText", "Features") to => ("Features", "FeedbackText")
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Features")

            .Append(mlContext.BinaryClassification.Trainers
            .FastTree(numberOfLeaves: 50, numberOfTrees: 50, minimumExampleCountPerLeaf: 1));

        // Step 5 - traing the algorithm and get the model out
        var model = pipeline.Fit(dataView);

        // Step 6 - Load test data and run the test data
        //  check our model accuracy
        testData = new List<FeedBackTrainingData>();
        LoadTestData();
        IDataView  testDataView = mlContext.Data.LoadFromEnumerable(testData);

        var predictions = model.Transform(testDataView);
        var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

        // Step 7 - Use model
        var predictionsFunction = mlContext.Model.CreatePredictionEngine
                                                <FeedBackTrainingData, FeedBackPrediction>
                                                (model);
        var feedbackInput = new FeedBackTrainingData();

        feedbackInput.Features = TestText;

        var feedbackPredicted = predictionsFunction.Predict(feedbackInput);

        PredictedText.text = "Predicted: " + feedbackPredicted.PredictedLabel;
    }

    void LoadTrainingData()
    {
        trainingData.Add(
            new FeedBackTrainingData()
            {
                Features = "this is Good",
                Label = true
            });
        trainingData.Add(
            new FeedBackTrainingData()
            {
                Features = "this is Bad",
                Label = false
            });
        trainingData.Add(
            new FeedBackTrainingData()
            {
                Features = "this is Awesome",
                Label = true
            });
        trainingData.Add(
            new FeedBackTrainingData()
            {
                Features = "this is Terrible",
                Label = false
            });
    }

    void LoadTestData()
    {
        testData.Add(
            new FeedBackTrainingData()
            {
                Features = "Good",
                Label = true
            });
        testData.Add(
            new FeedBackTrainingData()
            {
                Features = "Bad",
                Label = false
            });
        testData.Add(
            new FeedBackTrainingData()
            {
                Features = "Awesome",
                Label = false
            });
        testData.Add(
            new FeedBackTrainingData()
            {
                Features = "Terrible",
                Label = false
            });
    }
}
