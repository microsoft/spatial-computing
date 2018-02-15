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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Microsoft.MR.LUIS
{
    /// <summary>
    /// The interface for a class that can handle a LUIS MR intent.
    /// </summary>
    public interface IIntentHandler : IEventSystemHandler
    {
        /// <summary>
        /// Returns true if the handler can handle the specified intent.
        /// </summary>
        /// <param name="intentName">
        /// The name of the intent to test.
        /// </param>
        /// <returns>
        /// <c>true</c> if the intent can be handled; otherwise <c>false</c>.
        /// </returns>
        bool CanHandle(string intentName);

        /// <summary>
        /// Handles the intent stored within the <see cref="LuisMRResult"/>.
        /// </summary>
        /// <param name="intent">
        /// The <see cref="Intent"/> to handle.
        /// </param>
        /// <param name="result">
        /// The <see cref="LuisMRResult"/> that contains the result of the prediction.
        /// </param>
        void Handle(Intent intent, LuisMRResult result);
    }
}
