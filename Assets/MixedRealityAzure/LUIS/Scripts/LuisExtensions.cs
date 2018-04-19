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

namespace Microsoft.MR.LUIS
{
	/// <summary>
	/// Provides extensions to the LUIS SDK for .Net.
	/// </summary>
    static public class LuisExtensions
    {
        /// <summary>
        /// Gets the first item in the list with the specified key or <see langword = "null" /> 
        /// if the list or item is not found.
        /// </summary>
        /// <param name="lookup">
        /// The lookup list where the item may be found.
        /// </param>
        /// <param name="name">
        /// The name of the list to search.
        /// </param>
        /// <returns>
        /// The first item in the list, if found; otherwise <see langword = "null" />.
        /// </returns>
        static public T FirstOrDefaultItem<T>(this IDictionary<string, IList<T>> lookup, string name)
        {
            // Validate
            if (lookup == null) throw new ArgumentNullException(nameof(lookup));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

            // Try and get the table
            if (!lookup.ContainsKey(name)) { return default(T); }

            // Get the table and return the first if found
            return lookup[name].FirstOrDefault();
        }

        /// <summary>
        /// Gets the first resolved value for the entity or <see langword = "null" /> 
        /// if there is no resolution.
        /// </summary>
        /// <param name="entity">
        /// The entity to resolve.
        /// </param>
        /// <returns>
        /// The first resolved value for the entity or <see langword = "null" /> 
        /// if there is no resolution.
        /// </returns>
        static public string FirstOrDefaultResolution(this Entity entity)
        {
            // Validate
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Attempt to get resolution
            if (entity.Resolution.ContainsKey("values"))
            {
                var resolution = entity.Resolution["values"];

                // Remove extra characters
                var resString = resolution.ToString().Replace("[\r\n  \"", "");
                resString = resString.Replace("\"\r\n]", "");

                // Return cleaned resolution
                return resString;
            }
            else
            {
                // No resolution
                return null;
            }
        }
    }
}
