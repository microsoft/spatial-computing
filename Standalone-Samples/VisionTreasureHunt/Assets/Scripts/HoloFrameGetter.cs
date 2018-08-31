using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Capture.Frames;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

public class HoloFrameGetter : MonoBehaviour
{
#if UNITY_WSA && !UNITY_EDITOR
        MediaCapture MediaCapture;
        MediaFrameReader mediaFrameReader;
#endif

    // Use this for initialization
    void Start()
    {
#if UNITY_WSA && !UNITY_EDITOR
        CreateMediaCapture();
#endif
    }

#if UNITY_WSA && !UNITY_EDITOR
    public async void CreateMediaCapture()
    {
        MediaCapture = new MediaCapture();
        MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
        settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        settings.MemoryPreference = MediaCaptureMemoryPreference.Cpu;
        await MediaCapture.InitializeAsync(settings);

        CreateFrameReader();
    }

    private async void CreateFrameReader()
    {
    Debug.Log("making frame reader");
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
        Debug.Log("making frame reader about to wait");

        mediaFrameReader = await MediaCapture.CreateFrameReaderAsync(colorFrameSource);
    }

    public async Task<byte[]> GetImageAsync()
    {
        byte[] imageData = null;
        Task.Run(async () =>
        {
            Debug.Log("in get image");
            while(mediaFrameReader == null)
            {
                Debug.Log("null frame reader");
                Thread.Sleep(100);
            }
            await mediaFrameReader.StartAsync();
            //grab image from camera:
            for (; ; )
            {
                Debug.Log("Get Image looping");
    if(mediaFrameReader == null){Debug.Log("null frame reader");}
                var frameReference = mediaFrameReader.TryAcquireLatestFrame();
    Debug.Log("Get Image looping 2");
                var videoFrame = frameReference?.VideoMediaFrame?.GetVideoFrame();
    Debug.Log("Get Image looping 3");
                if (videoFrame == null)
                {
    Debug.Log("null image");
                    continue; //ignoring frame
                }
                else
                {
                    Debug.Log("got a frame");
                    if(videoFrame.SoftwareBitmap == null)
                    {
                        continue; //ignoring frame
                    }
                    SoftwareBitmap softwareBitmap = videoFrame.SoftwareBitmap;
    Debug.Log("got softwarebmp");
                     using (var ms = new InMemoryRandomAccessStream())
                     {
        Debug.Log("in the using 1");
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
    Debug.Log("in the using 2");
    if(softwareBitmap == null){Debug.Log("null swbm");}
    else{Debug.Log("swbm format: " + softwareBitmap.BitmapPixelFormat);}
                        if(softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Rgba16)
                        {
                            softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Rgba16);
                        }
                        encoder.SetSoftwareBitmap(softwareBitmap);
        Debug.Log("in the using 3");

                        try
                        {
                            await encoder.FlushAsync();
                        }
                        catch ( Exception ex )
                        {
                            Debug.Log("Exception:");
                            Debug.Log(ex.Message);
                        }
                        Debug.Log("about to read into imagedata");
                        imageData = new byte[ms.Size];
                        await ms.ReadAsync(imageData.AsBuffer(), (uint) ms.Size, InputStreamOptions.None);
                    }
                    break;
                }
            }
        });

        //wait for the image to be ready, while not blocking the main thread:
        while (imageData == null)
        {
            Debug.Log("Waiting");
            await Task.Run(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(100);
                return;
            });
        }
        Debug.Log("Done Waiting");
        await mediaFrameReader.StopAsync();

        return imageData;
    }
#endif
}
