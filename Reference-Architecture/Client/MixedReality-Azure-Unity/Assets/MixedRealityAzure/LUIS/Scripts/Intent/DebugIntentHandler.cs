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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A global intent handler that prints information about all received intents.
/// </summary>
public class DebugIntentHandler : IIntentHandler
{
	#region Public Methods
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public bool CanHandle(string intentName)
	{
		//This is the debug handler, we want it to be able to act on any intent.
		return true;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public void Handle(Intent intent, LuisMRResult result)
	{
		//Build up a string of information about the result we got from LUIS
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"Utterance: {result.Context.PredictionText}");
		sb.Append($"Intent: {intent.Name}");
		foreach (string entityKey in result.PredictionResult.Entities.Keys)
		{
			sb.Append($" [{entityKey}]");
		}
		sb.AppendLine();
		sb.AppendLine($"Intent Score: {intent.Score.ToString("P")}");
		sb.AppendLine("Entities: ");
		foreach (string entityKey in result.PredictionResult.Entities.Keys)
		{
			sb.AppendLine($"  Entity: {entityKey}");
			foreach (Entity e in result.PredictionResult.Entities[entityKey])
			{
				sb.AppendLine($"    Value:  {e.Value}");
				sb.AppendLine($"    Score:  {e.Score}");
			}
		}
		//Then write it to the console
		Debug.Log(sb.ToString());
	}
	#endregion // Public Methods
}