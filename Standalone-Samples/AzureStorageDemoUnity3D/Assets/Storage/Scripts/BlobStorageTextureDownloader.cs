using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class BlobStorageTextureDownloader : MonoBehaviour {

    [Tooltip("Name of texture file to automatically download from Azure Blob Storage")]
    public string TextureFile = "earth_8k.jpg";
    // Unity unfortunately doesn't support the Description Attribute in the Inspector, 
    // but I'm leaving this here hoping it gets suppoirted someday
    //public enum TextureSizes {
    //    [Description("512")]
    //    Size512 = 512,
    //    [Description("1024")]
    //    Size1024 = 512,
    //    [Description("2048")]
    //    Size2048 = 512,
    //    [Description("4096")]
    //    Size4096 = 512,
    //    [Description("8192")]
    //    Size8192 = 512,
    //};
    // Set the texture size in the inspector, 1024 is the default. Note that this has no effect at this time.
    // TO DO: Add support for dynamic texture size selection before download based on naming conventions
    //        (e.g. "earth_512.jpg" for the 512 version, "earth_1k.jpg" for the 1K version, etc.
    //public TextureSizes TextureSize = TextureSizes.Size1024;

    // Use this for initialization
    async void Start () {

        string localimagefile = await AzureBlobStorageClient.instance.DownloadStorageBlockBlobSegmentedOperationAsync(TextureFile);

        if (localimagefile.Length > 0)
        {
            // Create a texture. Texture size does not matter, since LoadImage replaces it with incoming image size.
            Texture2D LoadedImage = new Texture2D(1024, 1024);
            byte[] byteFile = File.ReadAllBytes(localimagefile);
            LoadedImage.LoadImage(byteFile);
            // Assign new texture to material of attached MeshRenderer. Nothing fancy for now.
            GetComponent<Renderer>().material.mainTexture = LoadedImage;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
