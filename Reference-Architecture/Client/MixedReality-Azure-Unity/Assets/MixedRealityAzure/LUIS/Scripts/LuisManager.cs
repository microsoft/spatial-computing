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
using System.Linq;

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
        private List<IContextProvider> contextProviders = new List<IContextProvider>();
        private List<IEntityResolver> entityResolvers = new List<IEntityResolver>();
        private List<IIntentHandler> intentHandlers = new List<IIntentHandler>();
        #endregion // Member Variables

        #region Unity Inspector Variables
        [Tooltip("The application ID of the LUIS application.")]
		[SecretValue("LUIS.AppId")]
		public string AppId;

		[Tooltip("The application subscription key of the LUIS application.")]
		[SecretValue("LUIS.AppKey")]
		public string AppKey;

		[Tooltip("String that represents the domain of the LUIS endpoint.")]
		[SecretValue("LUIS.Domain")]
		public string Domain = "";

        [Tooltip("The minimum confidence level for an intent to be handled.")]
        [Range(0,1)]
        public double MinimumIntentScore = 0.5;

		[Tooltip("Whether to return all intents and not just the top scoring intent.")]
		public bool Verbose = true;
	    #endregion // Unity Inspector Variables

        #region Internal Methods
        /// <summary>
        /// Makes sure that the client has been created.
        /// </summary>
        private void EnsureClient()
		{
			if (client == null)
			{
				if (!this.enabled) { throw new InvalidOperationException($"Attempting to connect to LUIS but {nameof(LuisManager)} is not enabled."); }
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
				Debug.LogErrorFormat($"'{nameof(AppId)}' is required but is not set. {nameof(LuisManager)} has been disabled.");
				this.enabled = false;
			}

			if (string.IsNullOrEmpty(AppKey))
			{
				Debug.LogErrorFormat($"'{nameof(AppKey)}' is required but is not set. {nameof(LuisManager)} has been disabled.");
				this.enabled = false;
			}

			if (string.IsNullOrEmpty(Domain))
			{
				Debug.LogErrorFormat($"'{nameof(Domain)}' is required but is not set. {nameof(LuisManager)} has been disabled.");
				this.enabled = false;
			}

			// Add default strategies (can be overridden)
			AddDefaultStrategies();
        }
        #endregion // Unity Overrides

        #region Overridables / Event Triggers
        /// <summary>
        /// Adds default context providers, entity resolvers and intent handlers.
        /// </summary>
        protected virtual void AddDefaultStrategies()
        {
			// Default Resolvers
			EntityResolvers.Add(new EntityMetadataResolver());

			// Default Handlers
			IntentHandlers.Add(new ResolvedIntentForwarder());
        }

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

        /// <summary>
        /// Capture the current context for a prediction.
        /// </summary>
        /// <param name="context">
        /// The <see cref="PredictionContext"/> where context is stored.
        /// </param>
        /// <remarks>
        /// This is stage 1 in processing a LUIS utterance. 
        /// The default implementation enumerates through all objects in 
        /// <see cref="ContextProviders"/> and asks them to provide context.
        /// </remarks>
        protected virtual void CaptureContext(PredictionContext context)
        {
            foreach (IContextProvider provider in contextProviders)
            {
                provider.CaptureContext(context);
            }
        }

        /// <summary>
        /// Handles the final result for a LUIS prediction.
        /// </summary>
        /// <param name="result">
        /// The <see cref="LuisMRResult"/> that contains information about the prediction.
        /// </param>
        /// <remarks>
        /// This is stage 4 in processing a LUIS utterance. The default implementation looks for 
        /// a global handler for the specified intent and executes it. If a global handler is not 
        /// found and there is only one mapped entity, the entity will be checked to see if it 
        /// can handle the intent.
        /// </remarks>
        protected virtual void HandleIntent(LuisMRResult result)
        {
            // Which intent?
            Intent intent = result.PredictionResult.TopScoringIntent;

            // Handle the intent
            foreach (IIntentHandler handler in intentHandlers)
            {
                if (handler.CanHandle(intent.Name))
                {
                    handler.Handle(intent, result);
                }
            }
        }

        /// <summary>
        /// Attempts to resolve scene entities from LUIS entities.
        /// </summary>
        /// <param name="result">
        /// The <see cref="LuisMRResult"/> that provides context and storage for the resolution.
        /// </param>
        /// <remarks>
        /// This is stage 3 in processing a LUIS utterance. 
        /// The default implementation enumerates through all objects in the 
        /// <see cref="EntityResolvers"/> and asks them to resolve entities.
        /// </remarks>
        protected virtual void ResolveEntities(LuisMRResult result)
        {
            foreach (IEntityResolver resolver in entityResolvers)
            {
                resolver.Resolve(result);
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Methods
        /// <summary>
        /// Attempts a LUIS prediction on the specified context and if confidence is high enough, the intent is executed.
        /// </summary>
        /// <param name="context">
        /// Context that is used for the prediction. Additional context may still be collected by  
        /// the <see cref="ContextProviders"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the result of the operation as a <see cref="LuisMRResult"/>.
        /// </returns>
        /// <remarks>
        /// At a minimum, <see cref="PredictionContext.PredictionText"/> must be included within the context.
        /// </remarks>
        public virtual async Task<LuisMRResult> PredictAndHandleAsync(PredictionContext context)
        {
            // Validate
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(context.PredictionText)) throw new InvalidOperationException("At a minimum, PredictionText must be included within the context.");

            // Make sure we have a client
            EnsureClient();

            // Create our result object
            LuisMRResult mrResult = new LuisMRResult() { Context = context };

            // Stage 1: Capture context
            CaptureContext(context);

            // Stage 2: Predict using the LUIS client
            mrResult.PredictionResult = await client.Predict(context.PredictionText);

            // Only do the next two stages if we have the minimum required confidence
            if (mrResult.PredictionResult.TopScoringIntent.Score >= MinimumIntentScore)
            {
                // Stage 3: Resolve Entities
                ResolveEntities(mrResult);

                // Stage 4: Handle Intents
                HandleIntent(mrResult);
            }

            // Done
            return mrResult;
		}
        /// <summary>
        /// Attempts a LUIS prediction on the specified text and if confidence is high enough, the intent is executed.
        /// </summary>
        /// <param name="text">
        /// The text used for the prediction.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the result of the operation as a <see cref="LuisMRResult"/>.
        /// </returns>
        public virtual Task<LuisMRResult> PredictAndHandleAsync(string text)
        {
            // Validate
            if (string.IsNullOrEmpty(text)) throw new ArgumentException(nameof(text));

            // Create a context and store the text
            PredictionContext context = new PredictionContext() { PredictionText = text };

            // Pass to context override
            return PredictAndHandleAsync(context);
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

        /// <summary>
        /// Gets or sets the list of classes that can provide context for a LUIS prediction.
        /// </summary>
        public List<IContextProvider> ContextProviders
        {
            get
            {
                return contextProviders;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.contextProviders = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of classes that can resolve LUIS Entity to GameObject relationships.
        /// </summary>
        public List<IEntityResolver> EntityResolvers
        {
            get
            {
                return entityResolvers;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.entityResolvers = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of handlers for LUIS intents.
        /// </summary>
        public List<IIntentHandler> IntentHandlers
        {
            get
            {
                return intentHandlers;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.intentHandlers = value;
            }
        }
        #endregion // Public Properties
    }
}