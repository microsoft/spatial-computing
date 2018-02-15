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
using UnityEngine;

namespace Microsoft.MR.LUIS
{
    public class NamedEntityResolver : IEntityResolver
    {
        #region Member Variables
        private Dictionary<string, GameObject> nameTable = new Dictionary<string, GameObject>();
        #endregion // Member Variables

        #region Public Methods
        /// <summary>
        /// Gets the specified named entity.
        /// </summary>
        /// <param name="name">
        /// The name to retrieve.
        /// </param>
        public GameObject GetNamedEntity(string name)
        {
            return nameTable[name];
        }

        /// <summary>
        /// Registers the specified GameObject as the named entity.
        /// </summary>
        /// <param name="name">
        /// The name to register with.
        /// </param>
        /// <param name="entity">
        /// The entity to register.
        /// </param>
        public void RegisterNamedEntity(string name, GameObject entity)
        {
            // Test for valid name
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException(nameof(name) + " must not be null or empty"); }

            // Make sure valid entity
            if (entity == null) throw new ArgumentNullException(nameof(entity) + " must not be null");

            // Register the name
            nameTable.Add(name, entity);
        }

        /// <summary>
        /// Removes the specified named entity.
        /// </summary>
        /// <param name="name">
        /// The name to remove.
        /// </param>
        public void RemoveNamedEntity(string name)
        {
            nameTable.Remove(name);
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Resolve(LuisMRResult result)
        {
            // Loop through all LUIS entities of the specific type MR.InstanceName
            foreach (Entity entity in result.PredictionResult.Entities["MR.InstanceName"])
            {
                // The Value of this type should be the name in the name table
                GameObject gameObject = nameTable[entity.Value];

                // If it's a valid entity, map it
                if (gameObject != null)
                {
                    result.Map(entity, gameObject, this);
                }
            }
        }
        #endregion // Public Methods
    }
}
