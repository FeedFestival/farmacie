using UnityEngine;
using System.Collections;
using Microsoft.ML.Data;

namespace Assets.Scripts
{
    public static class MLLearning
    {


    }

    public class FeedBackPrediction
    {
        [ColumnName(name: "PredictedLabel")]
        public bool PredictedLabel { get; set; }
    }

    public class FeedBackTrainingData
    {
        // ordinal: "1", 
        [ColumnName(name: "Label")]
        public bool Label { get; set; }

        [ColumnName(name: "Features")]
        public string Features { get; set; }
    }
}