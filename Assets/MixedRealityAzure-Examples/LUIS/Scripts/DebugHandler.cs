using Microsoft.Cognitive.LUIS;
using Microsoft.MR.LUIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugHandler : IIntentHandler
{
    public bool CanHandle(string intentName)
    {
        //This is the debug handler, we want it to be able to act on any intent.
        return true;
    }

    public void Handle(Intent intent, LuisMRResult result)
    {
        //Build up a string of information about the result we got from LUIS
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Utterance: " + result.Context.PredictionText);
        sb.AppendLine("Intent: " + intent.Name);
        sb.AppendLine("  Score: " + intent.Score.ToString("P"));
        sb.AppendLine("Entities: ");
        foreach(string entityKey in result.PredictionResult.Entities.Keys)
        {
            sb.AppendLine("  Entity: " + entityKey);
            foreach(Entity e in result.PredictionResult.Entities[entityKey])
            {
                sb.AppendLine("    Value:  " + e.Value);
                sb.AppendLine("    Score:  " + e.Score);
            }
        }
        //Then write it to the console
        Debug.Log(sb.ToString());
    }
}
