using Microsoft.Cognitive.LUIS;
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
        luisManager.EntityResolvers.Add(new EntityMetaDataResolver());
        luisManager.IntentHandlers.Add(new DebugHandler());

        var result = await luisManager.PredictAndHandle(testUtterence);

    }
}
