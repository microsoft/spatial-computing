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
    /// <summary>
    /// Represents a result of a LUIS prediction including entities mapped to the scene.
    /// </summary>
    public class LuisMRResult
    {
        #region Member Variables
        private PredictionContext context = new PredictionContext();
        private Dictionary<string, List<EntityMap>> resolvedEntities = new Dictionary<string, List<EntityMap>>();
        #endregion // Member Variables

        #region Public Methods
        /// <summary>
        /// Gets all entities that have been resolved to scene objects.
        /// </summary>
        /// <returns>
        /// A list of all resolved entities.
        /// </returns>
        public List<EntityMap> GetAllResolvedEntities()
        {
            List<EntityMap> list = new List<EntityMap>();
            foreach (var entityList in ResolvedEntities)
            {
                list.AddRange(entityList.Value);
            }
            return list;
        }

        /// <summary>
        /// Maps the specified entity to the specified game object.
        /// </summary>
        /// <param name="map">
        /// The set of context entity data to map.
        /// </param>

        public void Map(EntityMap map)
        {
            // First, make sure the entity table is created
            List<EntityMap> entityMapList;
            if (!resolvedEntities.TryGetValue(map.Entity.Name, out entityMapList))
            {
                resolvedEntities[map.Entity.Name] = new List<EntityMap>() { map };
            }
            else
            {
                entityMapList.Add(map);
            }
        }

        /// <summary>
        /// Maps the specified entity to the specified game object.
        /// </summary>
        /// <param name="entity">
        /// The LUIS <see cref="Entity"/> to map.
        /// </param>
        /// <param name="gameObject">
        /// The Unity <see cref="GameObject"/> that the entity maps to.
        /// </param>
        /// <param name="resolver">
        /// The resolver that resolved the entity.
        /// </param>
        public void Map(Entity entity, GameObject gameObject, IEntityResolver resolver)
        {
            // Validate
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));

            // Create the map entry
            EntityMap map = new EntityMap { Entity = entity, GameObject = gameObject, Resolver = resolver };

            // Map
            Map(map);
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="PredictionContext"/> that supplies additional context for the prediction.
        /// </summary>
        public PredictionContext Context
        {
            get
            {
                return context;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.context = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the result was handled by an intent handler.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LuisResult"/> that was returned from the LUIS prediction.
        /// </summary>
        /// <value>
        /// The <see cref="LuisResult"/> returned by the LUIS prediction.
        /// </value>
        public LuisResult PredictionResult { get; set; }

        ///// <summary>
        ///// Collection of <see cref="CompositeEntities"/> objects parsed accessed though a dictionary for easy access.
        ///// </summary>
        //public DDictionary<string, IList<CompositeEntity>> CompositeEntities { get; set; }

        /// <summary>
        /// Gets the collection of mappings between LUIS <see cref="Entity"/> objects and their corresponding scene 
        /// <see cref="GameObject"/>. Each mapping table is keyed by the entity type.
        /// </summary>
        public Dictionary<string, List<EntityMap>> ResolvedEntities => resolvedEntities;
        #endregion // Public Properties
    }
}
