using Microsoft.MR.Vision;
using System.Collections;
using UnityEngine;

public class DemoDriver : MonoBehaviour
{
    //Handles setting up the correct AR system, asking for images, sending images to the VisionManager, and dealing with results.
    VisionManager visionManager;
    AndroidFrameGetter androidFrameGetter;
    IOSFrameGetter iOSFrameGetter;
    HoloFrameGetter holoFrameGetter;
    public GameObject arKitRoot;
    public GameObject arCoreRoot;
    public GameObject mrtkRoot;


    public Transform objectMarker;
    bool predictionStarted;
    bool objectFound;

    void Start()
    {
        Application.targetFrameRate = 60;
        visionManager = GetComponent<VisionManager>();
        predictionStarted = false;


        //set up the correct sdk:
#if UNITY_ANDROID && !UNITY_EDITOR
        arKitRoot.SetActive(false);
        mrtkRoot.SetActive(false);
        androidFrameGetter = GetComponent<AndroidFrameGetter>();
        androidFrameGetter.enabled = true;
#elif UNITY_IOS && !UNITY_EDITOR
        arCoreRoot.SetActive(false);
        mrtkRoot.SetActive(false);
        iOSFrameGetter = GetComponent<IOSFrameGetter>();
        iOSFrameGetter.enabled = true;
#elif UNITY_WSA && !UNITY_EDITOR
        arKitRoot.SetActive(false);
        arCoreRoot.SetActive(false);
        holoFrameGetter = GetComponent<HoloFrameGetter>();
        holoFrameGetter.enabled = true;
#else
        arKitRoot.SetActive(false);
        arCoreRoot.SetActive(false);
        mrtkRoot.SetActive(false);
        Debug.Log("Only works on Android, iOS, or Hololens.");
        return;
#endif
        GetImageAndSend();
    }

    public IEnumerator DelaySend()
    {
        objectFound = false;
        for (; ; )
        {
            if (objectFound == true)
            {
                break;
            }
            yield return new WaitForSeconds(1.0f);
            GetImageAndSend();
        }
    }

    public async void GetImageAndSend()
    {
        //get an image from the camera:
        byte[] imageData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
        imageData = await androidFrameGetter.GetImageAsync();
#elif UNITY_IOS && !UNITY_EDITOR
        imageData = await iOSFrameGetter.GetImageAsync();
#elif UNITY_WSA && !UNITY_EDITOR
        imageData = await holoFrameGetter.GetImageAsync();
#else
        Debug.Log("Only works on Android, iOS, or Hololens!");
#endif
        //send the image for prediction:
        PredictionResult result = await visionManager.SendImageAsync(imageData);
        ResultHandler(result);
    }

    public void ResultHandler(PredictionResult result)
    {
        if (result == null)
        {
            Debug.Log("Error in prediction");
            return;
        }

        if (objectFound == true)
        {
            //ignore results that come in after we've found our object.
            return;
        }

        //handles what to do with a prediction result. In this sample it just puts a cube where it sees an object.
        if (result.predictions.Count > 0)
        {
            //Found the object!
            objectFound = true;
            string highestConfidenceResult = result.predictions[result.indexOfHighestConfidence].name;
            Debug.Log(highestConfidenceResult);
            StartCoroutine(SuccessMethod());
        }
        else
        {
            Debug.Log("Nothing seen");
        }

        if (predictionStarted == false)
        {
            //After the first image get, start the capture loop:
            predictionStarted = true;
            StartCoroutine(DelaySend());
        }
    }

    IEnumerator SuccessMethod()
    {
        //When the object is found, launch 100 cubes in random directions:
        Transform currTransform = Camera.main.transform;
        for (int i = 0; i < 100; i++)
        {
            Transform cube = Instantiate(objectMarker, currTransform.position, Quaternion.identity);
            cube.rotation = Random.rotation;
            cube.GetComponent<Rigidbody>().AddForce(cube.forward * 50);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
}
