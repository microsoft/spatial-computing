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
/// Resolves entities in our prediction with entities in the scene. This resolver finds scene gameobjects by name or type.
/// The name and type of a gameobject in the scene is defined with the monobehaviour 'EntityMetaData'.
/// </summary>
public class EntityMetaDataResolver : IEntityResolver
{
    public string[] validEntityNames = new string[]
    {
        "MR.InstanceName",
        "MR.InstanceType"
    };

    /// <summary>
    /// Find all the scene GameObjects that have names matching the Entity value
    /// </summary>
    /// <param name="result"></param>
    public void Resolve(LuisMRResult result)
    {
        //Collect any entities that match the entity names we're looking for
        var predictionEntities = result.PredictionResult.Entities.Where(x => validEntityNames.Contains(x.Key)).SelectMany(y => y.Value);

        if (predictionEntities.Count() < 1)
            return;

        //Join the list of scene objects with prediction entities to get matches in the scene
        IEnumerable<EntityMap> matchedEntities =
            from entity in predictionEntities
            let entityName = entity.Value.ToLower()
            from sceneEntity in GameObject.FindObjectsOfType<EntityMetaData>()
            where entityName.Equals(sceneEntity.EntityName.ToLower()) || entityName.Equals(sceneEntity.EntityType.ToLower())
            select new EntityMap()
            {
                Entity = entity,
                GameObject = sceneEntity.gameObject,
                Resolver = this
            };

        //Add all our found entities to the result's entity map, which maps LUIS entities with scene entities.
        foreach (EntityMap entityMap in matchedEntities)
        {
            result.Map(entityMap);
        }
    }
}

