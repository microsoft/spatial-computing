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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MR.LUIS
{
    /// <summary>
    /// The interface for a class that can resolve a LUIS Entity into an application entity.
    /// </summary>
    public interface IEntityResolver
    {
        /// <summary>
        /// Attempts to resolve entities in a <see cref="LuisResult"/> to scene entities in a <see cref="LuisMRResult"/>.
        /// </summary>
        /// <param name="result">
        /// The <see cref="LuisMRResult"/> where entities will be resolved.
        /// </param>
        /// <remarks>
        /// An Entity resolver may use all of the information available in the <see cref="LuisMRResult"/> to resolve LUIS 
        /// entities to scene entities. For example, the LUIS Entity "that" might be resolved using the 
        /// <see cref="PredictionContext.GazedObject">GazedObject</see> stored in the 
        /// <see cref="LuisMRResult.Context">Context</see>.
        /// </remarks>
        void Resolve(LuisMRResult result);
    }
}
