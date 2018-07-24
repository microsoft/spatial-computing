using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.CognitiveServices.Vision.LUISFace
{
	// TODO: If this wrapper turns out to be too thin, remove it.
	public class FaceManagerClient
	{
		private FaceClient faceClient;

		public FaceManagerClient(string subscriptionKey, string baseUri)
		{
			faceClient = new FaceClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
			faceClient.BaseUri = new Uri(baseUri);
		}

		public async Task<IList<DetectedFace>> Detect(string uri)
		{
			return await faceClient.Face.DetectWithUrlAsync(uri);
		}

		public async Task<IList<DetectedFace>> Detect(Stream stream)
		{
			return await faceClient.Face.DetectWithStreamAsync(stream);
		}

		public void Dispose()
		{
			faceClient.Dispose();
		}
	}
}
