using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Class to hold all the information of the results of an image prediction, for later use:
namespace Microsoft.MR.Vision
{
    public class PredictionResult
    {
        public string jsonResultsString { get; set; }
        public List<Prediction> predictions;//list to hold all predictions for a given image
        public int indexOfHighestConfidence;
        public float minConfidence;

        public PredictionResult(float confidenceThreshold)
        {
            predictions = new List<Prediction>();
            minConfidence = confidenceThreshold;
        }

        //turns the json string into a list of distinct Prediction objects:
        public void JsonStringToPredictionList()
        {
            if (jsonResultsString != null)
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(this.jsonResultsString);

                //get the predicted classes into a JToken:
                List<JProperty> subProperties = obj.Properties().ToList();
                bool isUsingCustomVision = true;
                JProperty property = null;
                try
                {
                    property = subProperties.Find(p => p.Name == "predictions");
                    if (property == null)
                    {
                        property = subProperties.Find(p => p.Name == "categories");
                        isUsingCustomVision = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }

                JToken resultJToken = property.Value;

                //make sure there are some classes:
                if (resultJToken.Count() == 0)
                {
                    return;
                }
                //add the predictions to the prediction list:
                //also keep track of which has highest confidence:
                for (int i = 0; i < resultJToken.Count(); i++)
                {
                    if (isUsingCustomVision)
                    {
                        Prediction thisPrediction = new Prediction();
                        thisPrediction.name = resultJToken[i].Value<string>("tagName");
                        thisPrediction.confidence = resultJToken[i].Value<float>("probability");
                        if (thisPrediction.confidence < minConfidence)
                        {
                            //skip this prediction:
                            continue;
                        }

                        if (resultJToken[i].Value<JToken>("boundingBox") != null)
                        {
                            SetupBoundingBox(thisPrediction, resultJToken[i].Value<JToken>("boundingBox"));
                        }
                        predictions.Add(thisPrediction);
                        if (thisPrediction.confidence > predictions[indexOfHighestConfidence].confidence)
                        {
                            indexOfHighestConfidence = predictions.Count - 1;//set this new prediction as highest confidence
                        }
                    }
                    else
                    {
                        //using Computer Vision format:
                        Prediction thisPrediction = new Prediction();
                        thisPrediction.name = resultJToken[i].Value<string>("name");
                        thisPrediction.confidence = resultJToken[i].Value<float>("score");
                        if (thisPrediction.confidence < minConfidence)
                        {
                            //skip this prediction:
                            continue;
                        }
                        predictions.Add(thisPrediction);
                        if (thisPrediction.confidence > predictions[indexOfHighestConfidence].confidence)
                        {
                            indexOfHighestConfidence = predictions.Count - 1;//set this new prediction as highest confidence
                        }
                    }
                }
            }
        }

        //set up the corner points of bounding boxes, so that they can be shown later:
        public void SetupBoundingBox(Prediction thisPrediction, JToken boundingBoxToken)
        {
            //box values are the fraction of the image:
            thisPrediction.boundingBoxInfo = new float[4];
            thisPrediction.boundingBoxInfo[0] = boundingBoxToken.Value<float>("left");
            thisPrediction.boundingBoxInfo[1] = boundingBoxToken.Value<float>("top");
            thisPrediction.boundingBoxInfo[2] = boundingBoxToken.Value<float>("width");
            thisPrediction.boundingBoxInfo[3] = boundingBoxToken.Value<float>("height");
        }
    }
}
