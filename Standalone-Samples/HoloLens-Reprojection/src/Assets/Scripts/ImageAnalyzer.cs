using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageAnalyzer : MonoBehaviour
{
    public static ImageAnalyzer Instance;

    public CustomVisionServiceConfig config;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator AnalyseLastImageCaptured(string imagePath, Action<string> response)
    {
        var imageBytes = GetImageAsByteArray(imagePath);

        return AnalyseLastImageCaptured(imageBytes, Path.GetFileName(imagePath), response);
    }

    public IEnumerator AnalyseLastImageCaptured(byte[] imageBytes, string imageName, Action<string> callback)
    {
        Debug.Log(string.Format("Analyzing {0} bytes of {1}", imageBytes.Length, imageName));

        WWWForm webForm = new WWWForm();

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(config.PredictionEndpoint, webForm))
        {
            unityWebRequest.timeout = unityWebRequest.timeout;
            unityWebRequest.SetRequestHeader("Prediction-Key", config.PredictionKey);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.chunkedTransfer = true;
            unityWebRequest.useHttpContinue = true;

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isHttpError)
            {
                Debug.Log("http error: " + unityWebRequest.error);
                callback?.Invoke(null);
            }
            else
            {

                Debug.Log("http response: " + unityWebRequest.responseCode);

                string jsonResponse = unityWebRequest.downloadHandler.text;

                Debug.Log("response: " + jsonResponse);

                callback?.Invoke(jsonResponse);
            }
        }
    }

    static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);

        BinaryReader binaryReader = new BinaryReader(fileStream);

        return binaryReader.ReadBytes((int)fileStream.Length);
    }
}
