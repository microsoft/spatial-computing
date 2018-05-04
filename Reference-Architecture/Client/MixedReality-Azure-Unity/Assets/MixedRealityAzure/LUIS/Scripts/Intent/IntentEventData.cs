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

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Cognitive.LUIS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MR.LUIS
{
	/// <summary>
	/// Provides a method of handling LUIS intents through the Unity Event system.
	/// </summary>
    public class IntentEventData : BaseEventData
    {
		#region Constructors
		/// <summary>
		/// Initializes a new <see cref="IntentEventData"/>.
		/// </summary>
		/// <param name="eventSystem">
		/// The event system that the event should attach to.
		/// </param>
		public IntentEventData(EventSystem eventSystem) : base(eventSystem) { }
		#endregion // Constructors

		#region Public Methods
		/// <summary>
		/// Reinitializes the event to new values.
		/// </summary>
		/// <param name="intent">
		/// The <see cref="Intent"/> that should be handled by the event.
		/// </param>
		/// <param name="result">
		/// The <see cref="LuisMRResult"/> that was returned by the prediction.
		/// </param>
		public void Initialize(Intent intent, LuisMRResult result)
		{
			// Validate
			if (intent == null) throw new ArgumentNullException(nameof(intent));
			if (result == null) throw new ArgumentNullException(nameof(result));

			// Store
			Reset();
			Result = result;
			Intent = intent;
		}
		#endregion // Public Methods

		#region Public Properties
		/// <summary>
		/// Gets the <see cref="Intent"/> that should be handled by the event.
		/// </summary>
		public Intent Intent { get; private set; }

		/// <summary>
		/// Gets the <see cref="LuisMRResult"/> that was returned by the prediction.
		/// </summary>
		/// <remarks>
		/// The <see cref="LuisMRResult"/> contains additional information that may be helpful 
		/// to process the intent. For example, <see cref="LuisMRResult"/> contains the 
		/// <see cref="LuisMRResult.Context">Context</see> information that was captured for 
		/// the prediction.
		/// </remarks>
		public LuisMRResult Result { get; private set; }
		#endregion // Public Properties
	}
}
