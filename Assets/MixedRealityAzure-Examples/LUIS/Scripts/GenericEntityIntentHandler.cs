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