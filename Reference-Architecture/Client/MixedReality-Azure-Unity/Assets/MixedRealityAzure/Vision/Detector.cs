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

public class Detector{ 
    //Takes in an image, and outputs an instance of the Prediction class.
    //Uses the settings set in an Options class to decide what to do with the input image.
    System.Action<PredictionResult> resultCallbackFunction;
    Options options;
    MonoBehaviour callingScript;

    public Detector(Options options, MonoBehaviour caller) {
        this.options = options;
        //needs calling script to use Coroutines:
        callingScript = caller;
    }

    public void SendImage(byte[] imageData, System.Action<PredictionResult> callback) {
        resultCallbackFunction = callback;
        GetPrediction(imageData);
    }

    public void GetPrediction(byte[] imageData){
        //send the image to the appropriate place:
        if (options.useOffline){
            throw new NotImplementedException();
        }
        else{
            //Use online calls with SDK or REST API:
            if (options.useSDK){
                callingScript.StartCoroutine(SDKPrediction(imageData));
            }
            else{
                //Use the REST API:
                callingScript.StartCoroutine(APIPrediction(imageData));
            }
        }
    }

    public IEnumerator APIPrediction(byte[] imageData){
        //Predicts using a trained CustomVision model:
        UnityWebRequest wr = new UnityWebRequest();
        if (options.useCustomVision) {
            wr.url = options.customVisionURL;
            wr.SetRequestHeader("Prediction-Key", options.customVisionPredictionKey);
        }
        else {
            //use Computer Vision:
            wr.url = options.visionURL;
            wr.SetRequestHeader("Ocp-Apim-Subscription-Key", options.visionKey);
        }
        wr.method = UnityWebRequest.kHttpVerbPOST;

        //set body:
        UploadHandler uploader = new UploadHandlerRaw(imageData);
        uploader.contentType = "application/octet-stream";
        wr.uploadHandler = uploader;
        wr.downloadHandler = new DownloadHandlerBuffer();
        //send request:
        yield return wr.SendWebRequest();
        yield return wr.isDone;

        if (wr.isNetworkError || wr.isHttpError){
            UnityEngine.Debug.Log(wr.downloadHandler.text);
            yield return null;
        }
        else{
            // Successful request, now we can output the results:
            PredictionResult predRes = new PredictionResult();
            predRes.jsonResultsString = wr.downloadHandler.text;
            predRes.JsonStringToPredictionList();
            resultCallbackFunction(predRes);
            yield return null;
        }
    }

    //Uses either the Custom Vision SDK or the Computer Vision SDK, depending on the set option property "useCustom":
    public IEnumerator SDKPrediction(byte[] imageData){
        PredictionResult predRes = new PredictionResult();
        if (options.useCustomVision){
            ImagePredictionResultModel result = null;
            PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = options.customVisionPredictionKey };
            MemoryStream imageStream = new MemoryStream(imageData);
            Guid customGuid = new Guid(options.customVisionProjectID);
            //Send to Custom Vision project, on another thread:
            Thread sendThread = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                result = endpoint.PredictImage(customGuid, imageStream);
                return;
            });
            sendThread.Start();
            //wait for result:
            while (result == null) {
                yield return new WaitForSeconds(1f);
            }
            predRes.ListToPredictionsList(result.Predictions);
        }
        else{
            //use ComputerVision:
            ComputerVisionAPI computerVision = new ComputerVisionAPI(
                new ApiKeyServiceClientCredentials(options.visionKey),
                new System.Net.Http.DelegatingHandler[] { });

            //get the Azure Region:
            computerVision.AzureRegion = GetAzureRegionFromURL(options.visionURL);
            //send the image to ComputerVision:
            Stream imageStream = new MemoryStream(imageData);
            ImageAnalysis analysis = null;
            Thread sendThread = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                analysis = AnalyzeImageRemoteAsync(computerVision, imageStream).Result;
                return;
            });
            sendThread.Start();

            //wait for result:
            while (analysis == null) {
                yield return new WaitForSeconds(1f);
            }
            predRes.ListToPredictionsList(analysis.Categories);
        }
        resultCallbackFunction(predRes);
        yield return null;
    }

    AzureRegions GetAzureRegionFromURL(string visionURL){
        //get the region based on the predictionURL (must be same as in visionURL):
        int startRegionIndex = 0;
        int endRegionIndex = 0;
        for (int i = 0; i < visionURL.Length; i++){
            if (visionURL[i] == '.'){
                endRegionIndex = i;
                break;
            }
            if (visionURL[i] == '/'){
                startRegionIndex = i + 1;
            }
        }
        char[] regionCharArray = visionURL.Substring(startRegionIndex, (endRegionIndex - startRegionIndex)).ToCharArray();
        //make the first char capitalized:
        if ((int)regionCharArray[0] > 95){
            regionCharArray[0] = (char)((int)regionCharArray[0] - 32);
        }
        string regionString = new string(regionCharArray);
        AzureRegions region = (AzureRegions)Enum.Parse(typeof(AzureRegions), regionString);
        return region;
    }

    public async Task<ImageAnalysis> AnalyzeImageRemoteAsync(ComputerVisionAPI computerVision, Stream imageStream){
        try{
            ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream);
            return analysis;
        }
        catch(Exception ex){
            Debug.Log(ex.ToString());
            return null;
        }
    }
}