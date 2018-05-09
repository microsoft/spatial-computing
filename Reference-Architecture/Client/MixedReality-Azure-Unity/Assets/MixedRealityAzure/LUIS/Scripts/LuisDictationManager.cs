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
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;


namespace Microsoft.MR.LUIS
{
	/// <summary>
	/// Provides a method of invoking LUIS using speech recognized by <see cref="DictationRecognizer"/>.
	/// </summary>
    public class LuisDictationManager : MonoBehaviour
    {
        #region Member Variables
        private string deviceName = string.Empty; // Empty string specifies the default microphone.
        private DictationRecognizer dictationRecognizer;
        private string dictationResult; // String result of the current dictation.
        private bool isListening;
        private bool isRecording;
        private bool isTransitioning;
        private int samplingRate; // The device audio sampling rate.
        private StringBuilder textSoFar = new StringBuilder(); // Caches the text currently being displayed in dictation display text.
        #endregion // Member Variables

        #region Unity Inspector Variables
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
        [Range(1f, 60f)]
        [SerializeField]
        private float autoSilenceTimeout = 2f;

        [Tooltip("Whether or not the dictation manager should listen on start.")]
        [SerializeField]
        private bool autoStartListening = true;

        [Tooltip("Whether or not the dictation manager should continue to listen after a recognition or timeout.")]
        [SerializeField]
        private bool continuousRecognition = true;

        [Tooltip("Optional TextMesh that can be used to show the status of the manager.")]
        [SerializeField]
        private TextMesh debugOutput;

        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
        [Range(1f, 60f)]
        [SerializeField]
        private float initialSilenceTimeout = 5f;

        [Tooltip("The LuisManager where recognitions will be routed.")]
        [SerializeField]
        private LuisManager luisManager;

        [Tooltip("The minimum confidence level for the recognition to be passed to LuisManager.")]
        [SerializeField]
        private ConfidenceLevel minimumConfidenceLevel = ConfidenceLevel.Medium;

        [Tooltip("Length in seconds for the manager to listen.")]
        [Range(1, 60)]
        [SerializeField]
        private int recordingTime = 10;
        #endregion // Unity Inspector Variables

        #region Internal Methods
        private void LogInfo(string message, bool toConsole = true)
        {
            if (toConsole)
            {
                Debug.Log(message);
            }
            if (debugOutput != null)
            {
                debugOutput.color = Color.white;
                debugOutput.text = message;
            }
        }

        private void LogError(string message, bool toConsole = true)
        {
            if (toConsole)
            {
                Debug.LogError(message);
            }
            if (debugOutput != null)
            {
                debugOutput.color = Color.red;
                debugOutput.text = message;
            }

        }

        private void LogWarn(string message, bool toConsole = true)
        {
            if (toConsole)
            {
                Debug.LogWarning(message);
            }
            if (debugOutput != null)
            {
                debugOutput.color = Color.yellow;
                debugOutput.text = message;
            }
        }

        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        /// <param name="initialSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</param>
        /// <param name="autoSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input.</param>
        /// <param name="recordingTime">Length in seconds for the manager to listen.</param>
        /// <returns></returns>
        private IEnumerator StartListeningInternal(float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10)
        {
            #if UNITY_WSA || UNITY_STANDALONE_WIN
            if (isListening || isTransitioning)
            {
                Debug.LogWarning("Unable to start recording");
                yield break;
            }

            LogInfo("Starting recognizer");

            isListening = true;
            isTransitioning = true;

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            while (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            LogInfo("Waiting for speech system");

            while (dictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                yield return null;
            }

            LogInfo("Starting microphone");

            // Start recording from the microphone.
            Microphone.Start(deviceName, false, recordingTime, samplingRate);
			textSoFar.Clear();
            isTransitioning = false;
            
            LogInfo("Listening");

            #else
            return null;
            #endif
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        private IEnumerator StopListeningInternal()
        {
            #if UNITY_WSA || UNITY_STANDALONE_WIN
            if (!isListening || isTransitioning)
            {
                LogWarn("Unable to stop recording");
                yield break;
            }

            isListening = false;
            isTransitioning = true;

            LogInfo("Stopping microphone");
            Microphone.End(deviceName);

            LogInfo("Stopping dictation");
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            LogInfo("Waiting for speech to stop");
            while (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            PhraseRecognitionSystem.Restart();
            isTransitioning = false;
            
            LogInfo("Stopped listening");

            #else
            return null;
            #endif
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        #if UNITY_WSA || UNITY_STANDALONE_WIN
        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = textSoFar.ToString() + " " + text + "...";
            LogInfo(text, toConsole:false);
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private async void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
			// We have final text
			dictationResult = text;

			// Clear intermediate text
			textSoFar.Clear();

            // Make sure we have a minimum confidence level
            if (confidence > minimumConfidenceLevel) // Numerically this is inverted. Lower confidence levels are higher numbers.
            {
                LogWarn($"Heard '{dictationResult}' but confidence was too low.");
            }
            else
            {
                if (luisManager != null)
                {
                    LogInfo($"Heard '{dictationResult}', sending to LUIS.");
                    await luisManager.PredictAndHandleAsync(text);
                }
                else
                {
                    LogError($"Heard '{dictationResult}' but no LuisManager available.");
                }
            }
        }

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            // Clear for next recognition
            textSoFar.Clear();
            dictationResult = string.Empty;

            // If continuous, start again. Otherwise stop.
            if (continuousRecognition)
            {
				dictationRecognizer.Start();
			}
			else
			{
				StopListening();
			}
		}

        /// <summary>
        /// This event is fired when an error occurs.
        /// TODO: Hook up something so any external actors know we've errored. 
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = error + "\nHRESULT: " + hresult.ToString();

            textSoFar.Clear();
            dictationResult = string.Empty;

            if (debugOutput != null)
            {
                debugOutput.color = Color.red;
                debugOutput.text = error;
            }

            Debug.LogError(error);
            StartCoroutine(StopListeningInternal());

        }
        #endif
        #endregion // Overrides / Event Handlers

        #region Unity Overrides
        protected virtual void Awake()
        {
            luisManager = gameObject.GetComponent<LuisManager>();
            // Query the maximum frequency of the default microphone.
            int minSamplingRate; // Not used.
            Microphone.GetDeviceCaps(deviceName, out minSamplingRate, out samplingRate);

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        }

        protected virtual void Start()
        {
            // If no LuisManager, try to get one from the same GameObject.
            if (luisManager == null)
            {
                luisManager = GetComponent<LuisManager>();
            }

			// Validate components
			if (luisManager == null)
			{
				Debug.LogError($"{nameof(luisManager)} is not set to a valid instance. {nameof(LuisDictationManager)} is disabled.");
				enabled = false;
				return;
			}

			// Start listening?
			if (autoStartListening)
            {
                StartListening();
            }
        }

        protected virtual void OnDestroy()
        {
            if (dictationRecognizer != null)
            {
                dictationRecognizer.Dispose();
                dictationRecognizer = null;
            }
        }
        #endregion // Unity Overrides

        #region Public Methods
        /// <summary>
        /// Starts the dictation listening.
        /// </summary>
        public void StartListening()
        {
            if (isTransitioning)
            {
                return;
            }
            else
            {
                StartCoroutine(StartListeningInternal(initialSilenceTimeout, autoSilenceTimeout, recordingTime));
            }
        }

        /// <summary>
        /// Stops the dictation from listening.
        /// </summary>
        public void StopListening()
        {
            if (isTransitioning)
            {
                return;
            }
            else
            {
                StartCoroutine(StopListeningInternal());
            }
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the time length in seconds before dictation recognizer session ends due to lack of audio input.
        /// </summary>
        public float AutoSilenceTimeout
        {
            get
            {
                return autoSilenceTimeout;
            }
            set
            {
                if ((value < 1) || (value > 60)) throw new ArgumentOutOfRangeException(nameof(value));
                autoSilenceTimeout = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates if dictation is listening.
        /// </summary>
        public bool IsListening => isListening;
        #endregion // Public Properties
    }
}
