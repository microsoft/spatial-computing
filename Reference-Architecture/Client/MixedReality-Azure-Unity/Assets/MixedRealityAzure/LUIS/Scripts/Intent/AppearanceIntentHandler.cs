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

using Microsoft.MR;
using Microsoft.MR.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Cognitive.LUIS;
using System.Linq;

/// <summary>
/// An intent handler that can change the visual appearance of an object.
/// </summary>
public class AppearanceIntentHandler : MonoBehaviour, IIntentHandler
{
	#region Internal Methods
	private void HandleChangeAppearance(LuisMRResult result)
	{
		// Get the colors mentioned in the prediction
		var colorEntity = result.PredictionResult.Entities.FirstOrDefaultItem("MR.Color");
		if (colorEntity == null) { return; }

		// Try to convert the entity color to a Unity color
		Color color;
		if (!ColorMapper.TryParseCssString(colorEntity.Value.ToLower(), out color))
		{
			Debug.LogWarning($"The value \"{colorEntity.Value}\" does not map to a known color.");
			return;
		}

		// Get renderer
		var renderer = GetComponent<Renderer>();
		if (renderer == null) { return; }

		// Set the color
		renderer.material.color = color;
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
			case "MR.ChangeAppearance":
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
			case "MR.ChangeAppearance":
				HandleChangeAppearance(result);
				break;
		}
	}
	#endregion // Public Methods
}
