using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.iOS;

public class IOSFrameGetter : MonoBehaviour
{
#if UNITY_IOS
    Material cameraMaterial;
    UnityARVideo unityARVideo;
    RenderTexture renderTexture;
    byte[] imageBytes;
#endif
    // Use this for initialization
    void Start()
    {
#if UNITY_IOS
        unityARVideo = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityARVideo>();
        imageBytes = null;
#endif
    }
#if UNITY_IOS
    public async Task<byte[]> GetImageAsync()
    {
        StartCoroutine(GetTextureToBytes());

        //equivalent of yield return new WaitForSeconds:
        await Task.Run(() =>
        {
            while (imageBytes == null)
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(10);
            }
        });
        

        byte[] imageData = imageBytes;
        imageBytes = null;
        return imageData;
    }

    private IEnumerator GetTextureToBytes()
    {
        //Gets camera image from the ARCam material, and turns it into a byte[] in jpg format:
        while (cameraMaterial == null)
        {
            Debug.Log("null material still");
            yield return new WaitForSeconds(0.1f);
        }
        cameraMaterial = unityARVideo.m_ClearMaterial;
        renderTexture = new RenderTexture(cameraMaterial.mainTexture.width, cameraMaterial.mainTexture.height, 16);
        Graphics.Blit(null, renderTexture, cameraMaterial);
        yield return null;

        Texture2D cameraTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, true);
        var activeRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        cameraTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        cameraTexture.Apply();
        RenderTexture.active = activeRenderTexture;
        yield return null;

        imageBytes = cameraTexture.EncodeToJPG();
    }
#endif
}
