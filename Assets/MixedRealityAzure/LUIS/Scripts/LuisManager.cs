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

using Microsoft.Cognitive.LUIS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_WSA || UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
#endif

namespace Microsoft.MR.LUIS
{
	/// <summary>
	/// Manages configuration and connection to the LUIS service.
	/// </summary>
	public class LuisManager : MonoBehaviour
	{
		#region Member Variables
		private LuisClient client;
		#endregion // Member Variables

		#region Unity Inspector Variables
		[Tooltip("The application ID of the LUIS application.")]
		[SecretValue("LUIS.AppId")]
		public string AppId;

		[Tooltip("The application subscription key of the LUIS application.")]
		[SecretValue("LUIS.AppKey")]
		public string AppKey;

		[Tooltip("String that represents the domain of the LUIS endpoint.")]
		public string Domain = "westus";

		[Tooltip("Whether to use preview LUIS features.")]
		public bool Preview = false;

		[Tooltip("Whether to return full result of all intents not just the top scoring intent (for preview features only).")]
		public bool Verbose = false;
		#endregion // Unity Inspector Variables

		#region Internal Methods
		/// <summary>
		/// Makes sure that the client has been created.
		/// </summary>
		private void EnsureClient()
		{
			this.AssertEnabled();
			if (client == null)
			{
				client = new LuisClient(AppId, AppKey, Verbose, Domain);
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

			// Make sure we have enough to continue
			if (string.IsNullOrEmpty(AppId))
			{
				Debug.LogErrorFormat("'{0}' is required but is not set. {1} has been disabled.", "AppId", this.GetType().Name);
				this.enabled = false;
			}

			if (string.IsNullOrEmpty(AppKey))
			{
				Debug.LogErrorFormat("'{0}' is required but is not set. {1} has been disabled.", "AppKey", this.GetType().Name);
				this.enabled = false;
			}
		}

		protected virtual async void Start()
		{
			await Predict("Turn the box blue");
		}
		#endregion // Unity Overrides

		#region Overridables / Event Triggers
		#if !UNITY_WSA || UNITY_EDITOR
		protected virtual bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool valid = true;

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid)
						{
							valid = false;
						}
					}
				}
			}
			return valid;
		}
		#endif

		protected virtual void ProcessResult(LuisResult result)
		{
			Debug.LogFormat($"Top Intent: {result.TopScoringIntent.Name} with a score of {result.TopScoringIntent.Score}");
		}
		#endregion // Overridables / Event Triggers

		#region Public Methods
		public async Task Predict(string text)
		{
			// Make sure we have a client
			EnsureClient();

			// Use client to do the prediction
			LuisResult result = await client.Predict(text);

			// Process the result
			ProcessResult(result);
		}
		#endregion // Public Methods

		#region Public Properties
		/// <summary>
		/// Gets the <see cref="LuisClient"/> instance created by this manager.
		/// </summary>
		public LuisClient Client
		{
			get
			{
				this.EnsureClient();
				return client;
			}
		}
		#endregion // Public Properties
	}
}