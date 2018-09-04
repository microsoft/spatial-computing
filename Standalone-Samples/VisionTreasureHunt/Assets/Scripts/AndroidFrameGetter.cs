using UnityEngine;
using GoogleARCore.Examples.ComputerVision;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

public class AndroidFrameGetter : MonoBehaviour
{
#if UNITY_ANDROID
    TextureFormat format = TextureFormat.RGBA32;
    TextureReader textureReader;
    Texture2D texture;
    byte[] imageData;

    // Use this for initialization
    void Start()
    {
        imageData = null;
    }

    public async Task<byte[]> GetImageAsync()
    {
        //wait for textureReader referrence:
        if(textureReader == null)
        {
            textureReader = GetComponent<DemoDriver>().arCoreRoot.GetComponent<TextureReader>();
        }
        
        //set OnImageAvailable to recieve a frame:
        textureReader.OnImageAvailableCallback += OnImageAvailable;

        //equivalent of yield return new WaitForSeconds:
        while (imageData == null)
        {
            await Task.Run(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(100);
                return;
            });
        }
        byte[] imageBytes = imageData;
        imageData = null;
        return imageBytes;
    }

    private void OnImageAvailable(TextureReaderApi.ImageFormatType inputFormat, int width, int height, IntPtr pixelBuffer, int bufferSize)
    {
        textureReader.OnImageAvailableCallback -= OnImageAvailable;
        texture = new Texture2D(width, height, format, false, false);
        byte[] imageBuffer = new byte[width * height * 4];
        System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, imageBuffer, 0, bufferSize);
        ImageHandler(texture, imageBuffer);
    }

    private void ImageHandler(Texture2D texture, byte[] imageBuffer)
    {
        //format the imagebuffer and rotate/mirror it:
        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            //rotate the image:
            byte[] rotatedImageBuffer = RotateArray(imageBuffer, texture.width, texture.height);
            //format the array:
            byte[] bmpArray = BMPFormatter(rotatedImageBuffer);
            imageData = bmpArray;
        }).Start();

        return;
    }

    private byte[] RotateArray(byte[] imageBuffer, int originalWidth, int originalHeight)
    {
        //rotates the array 90 degrees counterclockwise, and mirrors the image. This may need to be different for different devices, depending on the camera sensor orientation.
        int[] imageBufferInts = new int[imageBuffer.Length / 4];
        Buffer.BlockCopy(imageBuffer, 0, imageBufferInts, 0, imageBuffer.Length);

        int[] rotatedImageArray = new int[imageBufferInts.Length];

        for (int i = 0; i < originalWidth; i++)
        {
            for (int j = 0; j < originalHeight; j++)
            {
                rotatedImageArray[i * originalHeight + j] = imageBufferInts[j*originalWidth + i];
            }
        }

        Buffer.BlockCopy(rotatedImageArray, 0, imageBuffer, 0, imageBuffer.Length);
        return imageBuffer;
    }

    private byte[] BMPFormatter(byte[] imageBuffer)
    {
        //takes an RGBA32 raw pixel buffer and adds a bmp header:
        byte[] bmpArray = new byte[imageBuffer.Length + 122];

        //BMP HEADER (bytes 0-13s):
        // 0-1 "BM"
        bmpArray[0] = 0x42;
        bmpArray[1] = 0x4d;

        // 2-5 Size of the BMP file - Byte count + Header 122
        Array.Copy(BitConverter.GetBytes(imageBuffer.Length + 122), 0, bmpArray, 2, 4);

        // 6-7 Application Specific : normally, set zero
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 6, 2);

        // 8-9 Application Specific : normally, set zero
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 8, 2);

        // 10-13 Offset where the pixel array can be found - 32bit bitmap data always starts at 122 offset.
        Array.Copy(BitConverter.GetBytes(122), 0, bmpArray, 10, 4);

        //DIB HEADER:
        // 14-17 Number of bytes in the DIB header. 108 from this point.
        Array.Copy(BitConverter.GetBytes(108), 0, bmpArray, 14, 4);

        // 18-21 Width of the bitmap.
        Array.Copy(BitConverter.GetBytes(texture.height), 0, bmpArray, 18, 4);

        // 22-25 Height of the bitmap.
        Array.Copy(BitConverter.GetBytes(texture.width), 0, bmpArray, 22, 4);

        // 26-27 Number of color planes being used
        Array.Copy(BitConverter.GetBytes(1), 0, bmpArray, 26, 2);

        // 28-29 Number of bits. If you don't know the pixel format, trying to calculate it with the quality of the video/image source.
        Array.Copy(BitConverter.GetBytes(32), 0, bmpArray, 28, 2);

        // 30-33 BI_RGB no pixel array compression used : most of the time, just set zero if it is raw data.
        Array.Copy(BitConverter.GetBytes(3), 0, bmpArray, 30, 4);

        // 34-37 Size of the raw bitmap data ( including padding )
        Array.Copy(BitConverter.GetBytes(imageBuffer.Length), 0, bmpArray, 34, 4);

        // 38-45 Print resolution of the image, 72 DPI x 39.3701 inches per meter yields
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 38, 4);
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 42, 4);

        // 46-49 Number of colors in the palette
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 46, 4);

        // 50-53 All colors are important
        Array.Copy(BitConverter.GetBytes(0), 0, bmpArray, 50, 4);

        // 54-57 Red channel bitmask
        Array.Copy(BitConverter.GetBytes(255), 0, bmpArray, 54, 4);

        // 58-61 Green channel bitmask
        Array.Copy(BitConverter.GetBytes(65280), 0, bmpArray, 58, 4);

        // 62-65 Blue channel bitmask
        Array.Copy(BitConverter.GetBytes(16711680), 0, bmpArray, 62, 4);

        // 66-69 Alpha channel bitmask
        Array.Copy(BitConverter.GetBytes(4278190080), 0, bmpArray, 66, 4);

        // 70-73 LCS:
        Array.Copy(BitConverter.GetBytes(1466527264), 0, bmpArray, 70, 4);

        // 74-121 Unused
        // 122~end : Pixel Data : Finally, time to combine your raw data, BmpBuffer in this code, with a bitmap header you've just created.
        Array.Copy(imageBuffer as Array, 0, bmpArray, 122, imageBuffer.Length);

        return bmpArray;
    }
#endif
}
