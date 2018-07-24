using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class FaceManager : MonoBehaviour 
{
	[SerializeField]
	private string subscriptionKey = "778e6bec0a3649c6b476ca0749268549";
	[SerializeField]
	private string baseUri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";

	private FaceManagerClient watcherClient;

	private void Awake()
	{
		watcherClient = new FaceClient(subscriptionKey, baseUri);
	}

	private void OnDestroy()
	{
		DisposeClient();
	}

	private void DisposeClient()
	{
		watcherClient.Dispose();
	}

	public void SetCredentials(string subscriptionKey, string baseUri)
	{
		this.subscriptionKey = subscriptionKey;
		this.baseUri = baseUri;
	}

	public async Task<IList<DetectedFace>> Detect(string uri)
	{
		return await watcherClient.Detect(uri);
	}

	public async Task<IList<DetectedFace>> Detect(Stream stream)
	{
		return await watcherClient.Detect(stream);
	}
}