using Microsoft.MR.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Cognitive.LUIS;
using System.Linq;

public class ColorAndSizeHandler : MonoBehaviour, IIntentHandler
{
    private void DoChangeAppearance(LuisMRResult result)
    {
        // Get the colors mentioned in the prediction
        var colorList = result.PredictionResult.Entities["MR.Color"];

        // If there is no color list, ignore
        if (colorList == null) { return; }

        // Get the first color in the list
        var colorEntity = colorList.FirstOrDefault();

        // If there was no color, done
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
