//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

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

namespace Microsoft.MR.Face
{
	// TODO: If this wrapper turns out to be too thin, remove it.
	public class FaceDetectClient
	{
		private FaceClient faceClient;

		public FaceDetectClient(string subscriptionKey, string baseUri)
		{
			faceClient = new FaceClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
			faceClient.BaseUri = new Uri(baseUri);
		}

		public async Task<IList<DetectedFace>> Detect(string uri)
		{
			return await faceClient.Face.DetectWithUrlAsync(uri);
		}

		public async Task<IList<DetectedFace>> Detect(Stream stream, IList<FaceAttributeType> faceAttributes = null)
		{
			return await faceClient.Face.DetectWithStreamAsync(stream, true, true, faceAttributes);
		}

		public void Dispose()
		{
			faceClient.Dispose();
		}
	}
}
