using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Microsoft.MR.Vision
{
    public class Options
    {
        //Class to set options for image classification/detection, which the Detector class will use
        //Must always be set:
        public bool useOffline;
        public bool useSDK;
        public bool useCustomVision;
        public float predictionConfidenceThreshold;

        //Must be set for using CustomVision:
        public string customVisionURL;
        public string customVisionPredictionKey;
        public string customVisionProjectID;

        //Must be set for using ComputerVision:
        public string visionURL;
        public string visionKey;

        public Options()
        {
            useOffline = false;
            useSDK = false;
            useCustomVision = true;
            predictionConfidenceThreshold = 0.5f;
        }
    }
}
