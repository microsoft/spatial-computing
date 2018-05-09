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

using Microsoft.MR.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Cognitive.LUIS;
using System.Linq;

/// <summary>
/// An intent handler that can modify the transform of an object.
/// </summary>
public class TransformIntentHandler : MonoBehaviour, IIntentHandler
{
    #region Unity Inspector Variables
    [Tooltip("The amount to rotate by if no amount is specified.")]
    public float unspecifiedRotateAmount = 45f;

    [Tooltip("The amount to scale by if no amount is specified.")]
    public float unspecifiedScaleAmount = 0.25f;
	#endregion // Unity Inspector Variables

	#region Internal Methods
	private void DoRotate(LuisMRResult result)
	{
		// Which direction?
		var roateDirection = result.PredictionResult.Entities.FirstOrDefaultItem("MR.RotateDirection");
		bool counterClockwise = (roateDirection?.Value == "counterclockwise");

		// TODO: Determine which amount
		float amount = unspecifiedRotateAmount;

		// Rotate
		gameObject.transform.Rotate(gameObject.transform.up, (counterClockwise ? -amount : amount));
	}

	private void DoScale(LuisMRResult result)
	{
		// Which direction?
		var scaleDirection = result.PredictionResult.Entities.FirstOrDefaultItem("MR.ScaleDirection");
		if (scaleDirection == null) { return; }
		var direction = scaleDirection.FirstOrDefaultResolution();

		// TODO: Determine which amount
		float amount = unspecifiedScaleAmount;

		// Actual scale
		switch (direction.ToLower())
		{
			case "decrease":
				//gameObject.transform.localScale.Scale(new Vector3(amount, amount, amount));
				gameObject.transform.localScale *= (1.0f - amount);
				break;
			case "increase":
				gameObject.transform.localScale *= (1.0f + amount);
				break;
		}
	}

	private void DoTranslate(LuisMRResult result)
	{

	}

	private void HandleTransform(LuisMRResult result)
	{
		// Which transform?
		string transformTypeName = null;
		var transformType = result.PredictionResult.Entities.FirstOrDefaultItem("MR.TransformType");
		if (transformType != null) { transformTypeName = transformType.Value; }

		// If a transform type wasn't specified, let's see if there's a scale?
		if (transformTypeName == null)
		{
			// Try to get a scale direction
			var scaleDirection = result.PredictionResult.Entities.FirstOrDefaultItem("MR.ScaleDirection");
			if (scaleDirection != null) { transformTypeName = "scale"; }

		}

		// If there's still no transform type name, nothing to do.
		if (transformTypeName == null)
		{
			Debug.LogWarning("Received Transform intent but could not determine transform type.");
			return;
		}

		// Which transform type?
		switch (transformTypeName)
		{
			case "rotate":
				DoRotate(result);
				break;
			case "scale":
				DoScale(result);
				break;
			case "translate":
				DoTranslate(result);
				break;
		}
	}
	#endregion // Internal Methods

	#region Public Methods
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public bool CanHandle(string intentName)
	{
		switch (intentName)
		{
			case "MR.Transform":
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public void Handle(Intent intent, LuisMRResult result)
	{
		switch (intent.Name)
		{
			case "MR.Transform":
				HandleTransform(result);
				break;
		}
	}
	#endregion // Public Methods
}
