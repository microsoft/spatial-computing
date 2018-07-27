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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

#if !UNITY_WSA || UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
#endif

namespace Microsoft.MR.Face
{
	/// <summary>
	/// Manages configuration and connection to the Face service.
	/// </summary>
	public class FaceManager : MonoBehaviour
	{
		#region Member Variables
		private FaceClient client;
        #endregion // Member Variables

        #region Unity Inspector Variables
        [Tooltip("The subscription key of the Face service.")]
		[SecretValue("Face.SubscriptionKey")]
		public string SubscriptionKey;	

		[Tooltip("String that represents the domain of the Face endpoint.")]
		[SecretValue("Face.Domain")]
		public string Domain = "";
	    #endregion // Unity Inspector Variables

        #region Internal Methods
        /// <summary>
        /// Makes sure that the client has been created.
        /// </summary>
        private void EnsureClient()
		{
			if (client == null)
			{
				if (!this.enabled) { throw new InvalidOperationException($"Attempting to connect to Face but {nameof(FaceManager)} is not enabled."); }
                
                String baseUri = "https://"+this.Domain+".api.cognitive.microsoft.com/face/v1.0";
			    client = new FaceClient(new ApiKeyServiceClientCredentials(this.SubscriptionKey), new System.Net.Http.DelegatingHandler[] { });
			    client.BaseUri = new Uri(baseUri);
			}
		}
		#endregion // Internal Methods

		#region Unity Overrides
		protected virtual void Awake()
		{
			#if !UNITY_WSA || UNITY_EDITOR
			//This works, and one of these two options are required as Unity's TLS (SSL) support doesn't currently work like .NET
			//ServicePointManager.CertificatePolicy = new CustomCertificatePolicy();

			//This 'workaround' seems to work for the .NET Storage SDK, but not event hubs. 
			//If you need all of it (ex Storage, event hubs,and app insights) then consider using the above.
			//If you don't want to check an SSL certificate coming back, simply use the return true delegate below.
			//Also it may help to use non-ssl URIs if you have the ability to, until Unity fixes the issue (which may be fixed by the time you read this)
			ServicePointManager.ServerCertificateValidationCallback = CheckValidCertificateCallback; //delegate { return true; };
			#endif

			// Attempt to load secrets
			SecretHelper.LoadSecrets(this);

			// Validate that member variables are set
			if (string.IsNullOrEmpty(SubscriptionKey))
			{
				Debug.LogErrorFormat($"'{nameof(SubscriptionKey)}' is required but is not set. {nameof(FaceManager)} has been disabled.");
				this.enabled = false;
			}

			if (string.IsNullOrEmpty(Domain))
			{
				Debug.LogErrorFormat($"'{nameof(Domain)}' is required but is not set. {nameof(FaceManager)} has been disabled.");
				this.enabled = false;
			}
            	
			
        }

        private void OnDestroy()
        {
            client.Dispose();
        }
        #endregion // Unity Overrides

        #region Overridables / Event Triggers
                
		#if !UNITY_WSA || UNITY_EDITOR
		protected virtual bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool valid = true;

			//// If there are errors in the certificate chain, look at each error to determine the cause.
			//if (sslPolicyErrors != SslPolicyErrors.None)
			//{
			//	for (int i = 0; i < chain.ChainStatus.Length; i++)
			//	{
			//		if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
			//		{
			//			chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
			//			chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
			//			chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
			//			chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
			//			bool chainIsValid = chain.Build((X509Certificate2)certificate);
			//			if (!chainIsValid)
			//			{
			//				valid = false;
			//			}
			//		}
			//	}
			//}
			return valid;
		}
        #endif
        #endregion // Overridables / Event Triggers

        #region Public Methods
        /// <summary>
        /// Attempts a Face detect with the given uri.
        /// </summary>
        /// <param name="uri">
        /// Uri of the api
        /// </param>
        /// <returns>
        /// Returns a list of detected Face objects
        /// </returns>
        public async Task<IList<DetectedFace>> Detect(string uri)
		{
			return await client.Face.DetectWithUrlAsync(uri);
		}
		
		public async Task<IList<DetectedFace>> Detect(Stream stream, IList<FaceAttributeType> faceAttributes = null)
		{
			return await watcherClient.Detect(stream, faceAttributes);
		}

        /// <summary>
        /// Attempts a Face detect with the given stream.
        /// </summary>
        /// <param name="stream">
        /// Stream
        /// </param>
        /// <returns>
        /// Returns a list of detected Face objects
        /// </returns>
        public async Task<IList<DetectedFace>> Detect(Stream stream)
		{
			return await client.Face.DetectWithStreamAsync(stream);
		}
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="FaceClient"/> instance created by this manager.
        /// </summary>
        public FaceClient Client
		{
			get
			{
				this.EnsureClient();
				return client;
			}
		}


        public void SetCredentials(string SubscriptionKey, string Domain)
        {
            this.SubscriptionKey = SubscriptionKey;
            this.Domain = Domain;
        }
        #endregion // Public Properties
    }
}