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

public class AppearanceTransformHandler : MonoBehaviour, IIntentHandler
{
    #region Unity Inspector Variables
    [Tooltip("The amount to rotate by if no amount is specified.")]
    public float unspecifiedRotateAmount = 45f;

    [Tooltip("The amount to scale by if no amount is specified.")]
    public float unspecifiedScaleAmount = 0.25f;
    #endregion // Unity Inspector Variables



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


    private void DoChangeAppearance(LuisMRResult result)
    {
        // Get the colors mentioned in the prediction
        var colorEntity = result.PredictionResult.Entities.FirstOrDefaultItem("MR.Color");
        if (colorEntity == null) { return; }

        // Try to convert the entity color to a Unity color
        Color color;
        if (!ColorUtility.TryParseHtmlString(colorEntity.Value.ToLower(), out color)) { return; }

        // Get renderer
        var renderer = GetComponent<Renderer>();
        if (renderer == null) { return; }

        // Set the color
        renderer.material.color = color;
    }

    private void DoTransform(LuisMRResult result)
    {
        // Which transform?
        string transformTypeName = null;
        var transformType = result.PredictionResult.Entities.FirstOrDefaultItem("MR.TransformType");
        if (transformType != null) { transformTypeName = transformType.Value; }

        // If a transform type wasn't specified, let's see if there's a scale?
        if (transformTypeName == null)
        {
            // Try to get a scale direciton
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




    public bool CanHandle(string intentName)
    {
        switch (intentName)
        {
            case "MR.ChangeAppearance":
            case "MR.Transform":
                return true;
            default:
                return false;
        }
    }

    public void Handle(Intent intent, LuisMRResult result)
    {
        switch (intent.Name)
        {
            case "MR.ChangeAppearance":
                DoChangeAppearance(result);
                break;
            case "MR.Transform":
                DoTransform(result);
                break;
        }
    }
}
