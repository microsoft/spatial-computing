using UnityEngine;
using UnityEngine.UI;

// Button event handlers for Azure Blob Storage Client demo scene
public class BlockBlobMediaTest : MonoBehaviour
{
    public Dropdown inputFilename;

    public async void BlockBlobMediaUpload()
    {
        string filename = inputFilename.captionText.text.Trim();
        if (filename.Length == 0)
        {
            AzureBlobStorageClient.instance.WriteLine("Please specify the file you wish to transfer.");
        }
        AzureBlobStorageClient.instance.ClearOutput();
        AzureBlobStorageClient.instance.WriteLine("-- Uploading to Blob Storage --");
        await AzureBlobStorageClient.instance.UploadStorageBlockBlobBasicOperationAsync(filename);
        AzureBlobStorageClient.instance.WriteLine("-- Upload Test Complete --");
    }

    public async void BlockBlobMediaDownload()
    {
        string filename = inputFilename.captionText.text.Trim();
        if (filename.Length == 0)
        {
            AzureBlobStorageClient.instance.WriteLine("Please specify the file you wish to transfer.");
        }
        AzureBlobStorageClient.instance.ClearOutput();
        AzureBlobStorageClient.instance.WriteLine("-- Downloading from Blob Storage --");
        await AzureBlobStorageClient.instance.DownloadStorageBlockBlobBasicOperationAsync(filename);
        AzureBlobStorageClient.instance.WriteLine("-- Download Test Complete --");
    }

    public async void BlockBlobMediaDownloadBySegments()
    {
        string filename = inputFilename.captionText.text.Trim();
        if (filename.Length == 0)
        {
            AzureBlobStorageClient.instance.WriteLine("Please specify the file you wish to transfer.");
        }
        AzureBlobStorageClient.instance.ClearOutput();
        AzureBlobStorageClient.instance.WriteLine("-- Downloading from Blob Storage by Segments --");
        await AzureBlobStorageClient.instance.DownloadStorageBlockBlobSegmentedOperationAsync(filename);
    }

}
