using Microsoft.MR.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LUISTester : MonoBehaviour
{
    public LuisManager luisManager;
    public string testUtterence = "Set the box to blue";

    // Use this for initialization
    async void Start()
    {
        var result = await luisManager.PredictAndHandle(testUtterence);
        Debug.Log($"Utterence '{testUtterence}' confidence: {result.OriginalResult.TopScoringIntent.Score} was handled: {result.Handled}.");
    }
}
