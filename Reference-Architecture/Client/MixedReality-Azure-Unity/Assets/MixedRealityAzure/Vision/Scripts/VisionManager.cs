using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MR.Vision
{
    public class VisionManager : MonoBehaviour
    {
        //Handles option setting and image setting for cognitive services CustomVision and ComputerVision
        //Options:
        //Must always be set:
        [Tooltip("Use online prediction or offline prediction with a downloaded CustomVision model")]
        public bool useOffline = false;

        [Tooltip("If true, use CustomVision. If false, use ComputerVision")]
        public bool useCustomVision = true;

        [Tooltip("The minimum confidence level for a Prediction to be handled.")]
        [Range(0f, 1.0f)]
        public float predictionConfidenceThreshold = 0.5f;

        //CustomVision:
        [Tooltip("Required for CustomVision. The prediction URL for your CustomVision project.")]
        public string customVisionURL;

        [Tooltip("Required for CustomVision. The Prediction Key for your CustomVision project.")]
        public string customVisionPredictionKey;

        [Tooltip("Required for CustomVision. The Project ID of your CustomVision project.")]
        public string customVisionProjectID;

        [Tooltip("Optional for CostumVision. The Training Key for your CustomVision project.")]
        public string customVisionTrainingKey;

        //ComputerVision:
        [Tooltip("Required for ComputerVision. Your Prediction URL for ComputerVision.")]
        public string visionURL;

        [Tooltip("Required for ComputerVision. Your Subscription Key for ComputerVision.")]
        public string visionKey;

        public async Task<PredictionResult> SendImageAsync(byte[] imageData)
        {
            PredictionResult result;
            //send the image to the appropriate place:
            if (useOffline)
            {
                throw new NotImplementedException();
            }
            else
            {
                result = await APIPrediction(imageData);
            }
            return result;
        }

        public async Task<PredictionResult> APIPrediction(byte[] imageData)
        {
            //Predicts using MS ComputerVision or a trained CustomVision model:
            UnityWebRequest wr = new UnityWebRequest();
            if (useCustomVision)
            {
                //use Custom Vision:
                wr.url = customVisionURL;
                wr.SetRequestHeader("Prediction-Key", customVisionPredictionKey);
            }
            else
            {
                //use Computer Vision:
                wr.url = visionURL;
                wr.SetRequestHeader("Ocp-Apim-Subscription-Key", visionKey);
            }
            wr.method = UnityWebRequest.kHttpVerbPOST;

            //set body:
            UploadHandler uploader = new UploadHandlerRaw(imageData);
            uploader.contentType = "application/octet-stream";
            wr.uploadHandler = uploader;
            wr.downloadHandler = new DownloadHandlerBuffer();

            //send request:
            wr.SendWebRequest();

            //Task this since UnityWebRequest is not awaitable (wr.isDone can only be checked from main thread):
            while (!wr.isDone)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(10);
                });
            }

            if (wr.isNetworkError || wr.isHttpError)
            {
                Debug.Log(wr.downloadHandler.text);
                return null;
            }
            else
            {
                // Successful request, now we can output the results:
                PredictionResult predRes = new PredictionResult(predictionConfidenceThreshold);
                predRes.jsonResultsString = wr.downloadHandler.text;
                predRes.JsonStringToPredictionList();
                return predRes;
            }
        }
    }
}
