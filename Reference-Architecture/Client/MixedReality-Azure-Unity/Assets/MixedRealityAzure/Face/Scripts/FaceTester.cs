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
using Microsoft.MR.Face; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides a quick way to test Face in a Unity scene.
/// </summary>
public class FaceTester : MonoBehaviour
{
	#region Unity Inspector Variables
	[Tooltip("Enable debug handlers to increase debug information.")]
	public bool EnableDebugging = true;

	[Tooltip("The FaceManager that the tester will interface with.")]
	public FaceManager FaceManager;

    // TODO
    // What is the equivalent of this? Make face api call?
	// [Tooltip("Predict the Test Utterance automatically on start.")]
	// public bool PredictOnStart = true;

    [Tooltip("Optional UI Button in the scene that can initiate the face detection.")]
    public Button SceneTestButton;

    [Tooltip("Optional UI Input field in the scene that supplies the test image.")]
    public InputField SceneImageInput;

	[Tooltip("The image to test")]
	public string TestImage = "";
	#endregion // Unity Inspector Variables

	#region Unity Overrides
	void Start()
	{
		// If no manager specified, see if one is on the same GameObject
		if (FaceManager == null)
		{
			FaceManager = GetComponent<FaceManager>();
		}

		// Validate components
		if (FaceManager == null)
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load.", nameof(FaceManager), this.GetType().Name);
			enabled = false;
			return;
		}

        // If there is a test button in the scene, wire up the click handler.
        //if (SceneTestButton != null)
        //{
        //    SceneTestButton.onClick.AddListener(() =>
        //    {
        //        TryPredict();
        //    });
        //}

        // If there is a test text field, setup the default
        if ((SceneImageInput != null) && (string.IsNullOrEmpty(SceneImageInput.text)))
        {
            SceneImageInput.text = TestImage;
        }

		// // Enable debugging?
		// if (EnableDebugging)
		// {
		// 	FaceManager.IntentHandlers.Add(new DebugIntentHandler());
		// }

		// Detect on start?
		//if ((DetectOnStart) && (!string.IsNullOrEmpty(TestImage)))
		//{
		//	TryDetect();
		//}
	}
	#endregion // Unity Overrides

	#region Public Methods
	/// <summary>
	/// Attempts to try a detection of the <see cref="TestImage"/>.
	/// </summary>
	public async void TryDetect()
	{
        // Make sure we're enabled
		if (!enabled)
		{
			Debug.LogError($"{nameof(FaceTester)} is not enabled. Can't detect.");
			return;
		}

        // Make sure we have a FaceManager assigned
		if (FaceManager == null)
		{
			Debug.LogError($"{nameof(FaceManager)} is not set to a valid instance.");
			return;
		}

        // If there is a scene text control and its contents aren't empty use that
        if ((SceneImageInput != null) && (!string.IsNullOrEmpty(SceneImageInput.text)))
        {
            TestImage = SceneImageInput.text;
        }

        // Make sure we have something to detect
		if (string.IsNullOrEmpty(TestImage))
		{
			Debug.LogError($"{nameof(TestImage)} is empty. Nothing to detect.");
			return;
		}

		// Detect
		await FaceManager.Detect(TestImage);
	}
	#endregion // Public Methods
}
