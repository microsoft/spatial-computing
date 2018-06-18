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

namespace Microsoft.MR.LUIS
{
	/// <summary>
	/// Resolves entities in our prediction with entities in the scene. This resolver finds scene GameObjects by name or type.
	/// The name and type of a GameObject in the scene is defined with the <see cref="EntityMetadata"/> behavior.
	/// </summary>
	public class EntityMetadataResolver : IEntityResolver
	{
		#region Member Variables
		private bool debugMessages = true;
		private string[] entityNames = new string[]
			{
				"MR.InstanceName",
				"MR.InstanceType"
			};
		#endregion // Member Variables

		#region Public Methods
		/// <summary>
		/// Find all the scene GameObjects that have names matching the Entity value
		/// </summary>
		/// <param name="result"></param>
		public void Resolve(LuisMRResult result)
		{
			// Collect any entities that match the entity names we're looking for
			List<Entity> predictionEntities = result.PredictionResult.Entities.Where(x => entityNames.Contains(x.Key)).SelectMany(y => y.Value).ToList();

			// If there are no entities in the prediction that we're looking for, nothing to do
			if (predictionEntities.Count < 1)
			{
				if (debugMessages)
				{
					Debug.Log("No entities in the prediction match the names configured for the resolver.");
				}
				return;
			}

			// Join the list of scene objects with prediction entities to get matches in the scene
			IEnumerable<EntityMap> meq =
				from entity in predictionEntities
				let entityName = entity.Value.ToLower()
				from sceneEntity in GameObject.FindObjectsOfType<EntityMetadata>()
				where entityName.Equals(sceneEntity.EntityName.ToLower()) || entityName.Equals(sceneEntity.EntityType.ToLower())
				select new EntityMap()
				{
					Entity = entity,
					GameObject = sceneEntity.gameObject,
					Resolver = this
				};
			List<EntityMap> matchedEntities = meq.ToList();

			//Add all our found entities to the result's entity map, which maps LUIS entities with scene entities.
			foreach (EntityMap entityMap in matchedEntities)
			{
				// Create map entry
				result.Map(entityMap);

				// Remove the entity from the prediction list to mark it as "mapped"
				predictionEntities.Remove(entityMap.Entity);
			}

			// If any entity is still in the prediction list, it was not matched. Log a warning.
			if (debugMessages)
			{
				foreach (var entity in predictionEntities)
				{
					Debug.LogWarning($"Warning: {entity.Name} \"{entity.Value}\" could not be mapped to the scene.");
				}
			}
		}
		#endregion // Public Methods

		#region Public Properties
		/// <summary>
		/// Gets or sets a value that indicates if debug messages will be printed during resolution.
		/// </summary>
		/// <value>
		/// <c>true</c> if debug messages will be printed during resolution; otherwise <c>false</c>.
		/// </value>
		public bool DebugMessages
		{
			get
			{
				return debugMessages;
			}
			set
			{
				debugMessages = value;
			}
		}

		/// <summary>
		/// Gets or sets the list of entity names that will be resolved from the result.
		/// </summary>
		public string[] EntityNames
		{
			get
			{
				return entityNames;
			}
			set
			{
				// Changing?
				if (entityNames != value)
				{
					// Validate
					if (value.Length < 1) { throw new ArgumentException("Must have at least one valid entity name."); }
					if (value.Any((s) => string.IsNullOrEmpty(s))) { throw new ArgumentException("Entity names cannot be null or empty."); }

					// Store
					entityNames = value;
				}
			}
		}
		#endregion // Public Properties
	}
}
