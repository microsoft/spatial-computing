using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        [Tooltip("If true, use SDK. If false, use API.")]
        public bool useSDK = false;

        [Tooltip("If true, use CustomVision. If false, use ComputerVision")]
        public bool useCustomVision = true;

        [Tooltip("The minimum confidence level for a Prediction to be handled.")]
        [Range(0f, 1.0f)]
        public float predictionConfidenceThreshold = 0.5f;

        //Must be set for using CustomVision:
        [Tooltip("The prediction URL for your CustomVision project.")]
        public string customVisionURL;

        [Tooltip("The Prediction Key for your CustomVision project.")]
        public string customVisionPredictionKey;

        [Tooltip("The Project ID of your CustomVision project.")]
        public string customVisionProjectID;

        //Must be set for using ComputerVision:
        [Tooltip("Your Prediction URL for ComputerVision.")]
        public string visionURL;

        [Tooltip("Your Subscription Key for ComputerVision.")]
        public string visionKey;

        void Awake()
        {

        }

        void Update()
        {

        }

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
                //Use online calls with SDK or REST API:
                if (useSDK)
                {
                    result = await SDKPrediction(imageData);
                }
                else
                {
                    //Use the REST API:
                    result = await APIPrediction(imageData);
                }
            }
            return result;
        }

        public async Task<PredictionResult> APIPrediction(byte[] imageData)
        {
            //Predicts using a trained CustomVision model:
            UnityWebRequest wr = new UnityWebRequest();
            if (useCustomVision)
            {
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
            //Task this since UnityWebRequest is not awaitable:
            while (!wr.isDone)
            {
                await Task.Run(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Thread.Sleep(100);
                    return;
                });
            }
            

            if (wr.isNetworkError || wr.isHttpError)
            {
                UnityEngine.Debug.Log(wr.downloadHandler.text);
                return null;
            }
            else
            {
                // Successful request, now we can output the results:
                PredictionResult predRes = new PredictionResult();
                predRes.jsonResultsString = wr.downloadHandler.text;
                predRes.JsonStringToPredictionList();
                return predRes;
            }
        }


        public async Task<PredictionResult> SDKPrediction(byte[] imageData)
        {
            PredictionResult predRes = new PredictionResult();
            if (useCustomVision)
            {
                ImagePredictionResultModel result = null;
                PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = customVisionPredictionKey };
                MemoryStream imageStream = new MemoryStream(imageData);
                Guid customGuid = new Guid(customVisionProjectID);
                //Send to Custom Vision project:

                result = await endpoint.PredictImageAsync(customGuid, imageStream);

                predRes.ListToPredictionsList(result.Predictions);
            }
            else
            {
                //use ComputerVision:
                ComputerVisionAPI computerVision = new ComputerVisionAPI(
                    new ApiKeyServiceClientCredentials(visionKey),
                    new System.Net.Http.DelegatingHandler[] { });

                //get the Azure Region:
                computerVision.AzureRegion = GetAzureRegionFromURL(visionURL);
                //send the image to ComputerVision:
                Stream imageStream = new MemoryStream(imageData);
                ImageAnalysis analysis = null;
                analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream);

                predRes.ListToPredictionsList(analysis.Categories);
            }
            return predRes;
        }

        AzureRegions GetAzureRegionFromURL(string visionURL)
        {
            //get the region based on the predictionURL (must be same as in visionURL):
            int startRegionIndex = 0;
            int endRegionIndex = 0;
            for (int i = 0; i < visionURL.Length; i++)
            {
                if (visionURL[i] == '.')
                {
                    endRegionIndex = i;
                    break;
                }
                if (visionURL[i] == '/')
                {
                    startRegionIndex = i + 1;
                }
            }
            char[] regionCharArray = visionURL.Substring(startRegionIndex, (endRegionIndex - startRegionIndex)).ToCharArray();
            //make the first char capitalized:
            if ((int)regionCharArray[0] > 95)
            {
                regionCharArray[0] = (char)((int)regionCharArray[0] - 32);
            }
            string regionString = new string(regionCharArray);
            AzureRegions region = (AzureRegions)Enum.Parse(typeof(AzureRegions), regionString);
            return region;
        }
    }
}
