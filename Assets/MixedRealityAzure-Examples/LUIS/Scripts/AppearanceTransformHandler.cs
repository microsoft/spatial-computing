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
    public float unspecifiedRotateAmount = 0.25f;

    [Tooltip("The amount to scale by if no amount is specified.")]
    public float unspecifiedScaleAmount = 36f;
    #endregion // Unity Inspector Variables



    private void DoRotate(LuisMRResult result)
    {
        // Which direction?
        var roateDirection = result.PredictionResult.Entities.FirstOrDefaultItem("MR.RotateDirection");
        if (roateDirection == null) { return; }

        // TODO: Determine which amount
        float amount = unspecifiedRotateAmount;

        // Actual scale
        switch (roateDirection.Value)
        {
            case "Clockwise":
                gameObject.transform.Rotate(gameObject.transform.up, amount);
                break;
            case "Counterclockwise":
                gameObject.transform.Rotate(gameObject.transform.up, -amount);
                break;
        }
    }

    private void DoScale(LuisMRResult result)
    {
        // Which direction?
        var scaleDirection = result.PredictionResult.Entities.FirstOrDefaultItem("MR.ScaleDirection");
        if (scaleDirection == null) { return; }

        // TODO: Determine which amount
        float amount = unspecifiedScaleAmount;

        // Actual scale
        switch (scaleDirection.Value)
        {
            case "Decrease":
                gameObject.transform.localScale.Scale(new Vector3(amount, amount, amount));
                break;
            case "Increase":
                gameObject.transform.localScale.Scale(new Vector3(1f + amount, 1f + amount, 1f + amount));
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
        if (!ColorMapper.GetColor(colorEntity.Value.ToLower(), out color)) { return; }

        // Get renderer
        var renderer = GetComponent<Renderer>();
        if (renderer == null) { return; }

        // Set the color
        renderer.material.color = color;
    }

    private void DoTransform(LuisMRResult result)
    {
        // Which transform?
        var transformType = result.PredictionResult.Entities.FirstOrDefaultItem("MR.TransformType");
        if (transformType == null) { return; }

        switch (transformType.Value)
        {
            case "Rotate":
                DoRotate(result);
                break;
            case "Scale":
                DoScale(result);
                break;
            case "Translate":
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
