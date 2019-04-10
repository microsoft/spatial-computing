using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;
using Newtonsoft.Json;

public class ImageCapture : MonoBehaviour
{
    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static ImageCapture Instance;


    public Resolution cameraResolution;

    [HideInInspector]
    public Matrix4x4 projectionMat;

    [HideInInspector]
    public Matrix4x4 worldMat;

    /// <summary>
    /// Keep counts of the taps for image renaming
    /// </summary>
    private int captureCount = 0;

    /// <summary>
    /// Photo Capture object
    /// </summary>
    private PhotoCapture photoCaptureObject = null;

    /// <summary>
    /// Allows gestures recognition in HoloLens
    /// </summary>
    private GestureRecognizer recognizer;

    /// <summary>
    /// Flagging if the capture loop is running
    /// </summary>
    internal bool captureIsActive;

    private byte[] _imageBytes;

    private Texture2D targetTexture;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Runs at initialization right after Awake method
    /// </summary>
    void Start()
    {
        DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            try
            {
                file.Delete();
            }
            catch (Exception)
            {
                Debug.LogFormat("Cannot delete file: ", file.Name);
            }
        }

        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.Tapped += TapHandler;
        recognizer.StartCapturingGestures();
    }

    /// <summary>
    /// Respond to Tap Input.
    /// </summary>
    private void TapHandler(TappedEventArgs obj)
    {
        if (!captureIsActive)
        {
            captureIsActive = true;

            SceneController.Instance.PlayTapSound();
            SceneController.Instance.SetCursorColor(Color.red);

            Invoke("ExecuteImageCaptureAndAnalysis", 0);
        }
    }

    /// <summary>
    /// Begin process of image capturing and send to Azure Custom Vision Service.
    /// </summary>
    private void ExecuteImageCaptureAndAnalysis()
    {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending
            ((res) => res.width * res.height).First();

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        PhotoCapture.CreateAsync(true, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            CameraParameters camParameters = new CameraParameters
            {
                hologramOpacity = 0f,
                cameraResolutionWidth = targetTexture.width,
                cameraResolutionHeight = targetTexture.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                captureCount++;
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemoryCallback);
            });
        });
    }

    private void OnCapturedPhotoToMemoryCallback(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            _imageBytes = ImageConversion.EncodeToJPG(targetTexture);

            var filename = string.Format(@"CapturedImage{0}.jpg", captureCount);
            var filePath = Path.Combine(Application.persistentDataPath, filename);
            SaveImage(filePath, _imageBytes);

            if (photoCaptureFrame.hasLocationData)
            {
                Debug.Log("Save matrices");
                photoCaptureFrame.TryGetProjectionMatrix(out projectionMat);
                photoCaptureFrame.TryGetCameraToWorldMatrix(out worldMat);
            }
        }

        photoCaptureFrame.Dispose();
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    /// <summary>
    /// Register the full execution of the Photo Capture. 
    /// </summary>
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        try
        {
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception capturing photo to disk: {0}", e.Message);
        }
    }

    /// <summary>
    /// The camera photo mode has stopped after the capture.
    /// Begin the image analysis process.
    /// </summary>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.LogFormat("Stopped Photo Mode");

        photoCaptureObject.Dispose();
        photoCaptureObject = null;

        if(_imageBytes != null)
            StartCoroutine(ImageAnalyzer.Instance.AnalyseLastImageCaptured(_imageBytes, string.Format(@"CapturedImage{0}.jpg", captureCount), (data) =>
            {
                if(data != null)
                {
                    var results = JsonConvert.DeserializeObject<PredictionResults>(data);
                    Debug.Log(data);
                    SceneController.Instance.HandleResponse(results);
                }
            }));
    }

    /// <summary>
    /// Stops all capture pending actions
    /// </summary>
    internal void ResetImageCapture()
    {
        captureIsActive = false;
        CancelInvoke();
    }

    /// <summary>
    /// Saves the jpeg encoded image stream to a file.
    /// </summary>
    static void SaveImage(string imageFilePath, byte[] imageBytes)
    {
        using (FileStream fileStream = new FileStream(imageFilePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(imageBytes);
            }
        }
    }
}
