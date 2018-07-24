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

using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Microsoft.MR.Face
{
    public class FaceManager : MonoBehaviour
    {
        [SerializeField]
        private string subscriptionKey = "778e6bec0a3649c6b476ca0749268549";
        [SerializeField]
        private string baseUri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";

        private FaceDetectClient watcherClient;

        private void Awake()
        {
            watcherClient = new FaceDetectClient(subscriptionKey, baseUri);
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
}