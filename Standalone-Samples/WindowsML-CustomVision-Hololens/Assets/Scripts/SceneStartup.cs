using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Storage;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Capture.Frames;
#endif

public class SceneStartup : MonoBehaviour
{
    public GameObject Label;
    private TextMesh LabelText;
    TimeSpan predictEvery = TimeSpan.FromMilliseconds(50);
    string textToDisplay;
    bool textToDisplayChanged;

#if UNITY_WSA && !UNITY_EDITOR
    Image_RecoModel imageRecoModel;
    MediaCapture MediaCapture;
#endif

    void Start()
    {
        LabelText = Label.GetComponent<TextMesh>();

#if UNITY_WSA && !UNITY_EDITOR
        CreateMediaCapture();
        InitializeModel();
#else
        DisplayText("Does not work in player.");
#endif
    }

    private void DisplayText(string text)
    {
        textToDisplay = text;
        textToDisplayChanged = true;
    }

#if UNITY_WSA && !UNITY_EDITOR
    public async void InitializeModel()
    {
        StorageFile imageRecoModelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Data/StreamingAssets/model.onnx"));
        imageRecoModel = await Image_RecoModel.CreateImage_RecoModel(imageRecoModelFile);
    }

    public async void CreateMediaCapture()
    {
        MediaCapture = new MediaCapture();
        MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
        settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        await MediaCapture.InitializeAsync(settings);

        CreateFrameReader();
    }

    private async void CreateFrameReader()
    {
        var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

        MediaFrameSourceGroup selectedGroup = null;
        MediaFrameSourceInfo colorSourceInfo = null;

        foreach (var sourceGroup in frameSourceGroups)
        {
            foreach (var sourceInfo in sourceGroup.SourceInfos)
            {
                if (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview
                    && sourceInfo.SourceKind == MediaFrameSourceKind.Color)
                {
                    colorSourceInfo = sourceInfo;
                    break;
                }
            }
            if (colorSourceInfo != null)
            {
                selectedGroup = sourceGroup;
                break;
            }
        }

        var colorFrameSource = MediaCapture.FrameSources[colorSourceInfo.Id];
        var preferredFormat = colorFrameSource.SupportedFormats.Where(format =>
        {
            return format.Subtype == MediaEncodingSubtypes.Argb32;

        }).FirstOrDefault();

        var mediaFrameReader = await MediaCapture.CreateFrameReaderAsync(colorFrameSource);
        await mediaFrameReader.StartAsync();
        StartPullFrames(mediaFrameReader);
    }

    private void StartPullFrames(MediaFrameReader sender)
    {
        Task.Run(async () =>
        {
            for (;;)
            {
                var frameReference = sender.TryAcquireLatestFrame();
                var videoFrame = frameReference?.VideoMediaFrame?.GetVideoFrame();
                if (videoFrame == null)
                {
                    continue; //ignoring frame
                }
                var input = new Image_RecoModelInput();

                input.data = videoFrame;
                if(videoFrame.Direct3DSurface == null)
                {
                    continue; //ignoring frame
                }

                try
                {
                    Image_RecoModelOutput prediction = await imageRecoModel.EvaluateAsync(input).ConfigureAwait(false);
                    var classWithHighestProb = prediction.classLabel[0];
                    if (prediction.loss[classWithHighestProb] > 0.5)
                    {
                        DisplayText("I see a " + classWithHighestProb);
                    }
                    else
                    {
                        DisplayText("I see nothing");
                    }
                }
                catch
                {
                   //Log errors
                }

                await Task.Delay(predictEvery);
            }

        });
    }
#endif

    void Update()
    {
        if (textToDisplayChanged)
        {
            LabelText.text = textToDisplay;
            textToDisplayChanged = false;
        }
    }
}
