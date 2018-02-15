using Microsoft.Cognitive.LUIS;
using Microsoft.MR.LUIS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEntityIntentHandler : MonoBehaviour, IIntentHandler
{
    [Serializable]
    public struct IntentAction
    {
        public string entityValueTrigger;
        public List<UnityEvent> actions;
    }

    public string intentName;
    public string entityTypeName;
    public IntentAction[] handledIntents;

    Dictionary<string, List<UnityEvent>> activateValues = new Dictionary<string, List<UnityEvent>>();

    void Start()
    {
        foreach (IntentAction intentAction in handledIntents)
        {
            if(!activateValues.ContainsKey(intentAction.entityValueTrigger))
                activateValues.Add(intentAction.entityValueTrigger, intentAction.actions);
            else
                activateValues[intentAction.entityValueTrigger].AddRange(intentAction.actions);
        }
    }

    public bool CanHandle(string intentName)
    {
        return intentName.Equals(intentName);
    }

    public void Handle(Intent intent, LuisMRResult result)
    {
        if (!result.PredictionResult.Entities.ContainsKey(entityTypeName))
            return;

        foreach (Entity entity in result.PredictionResult.Entities[entityTypeName])
        {
            if (activateValues.ContainsKey(entity.Value))
            {
                foreach(UnityEvent uEvent in activateValues[entity.Value])
                    uEvent.Invoke();
            }
        }
    }
}