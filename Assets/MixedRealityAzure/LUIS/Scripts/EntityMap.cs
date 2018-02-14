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
    /// Represents a map between an <see cref="Entity"/> recognised by LUIS and a <see cref="GameObject"/> in the scene.
    /// </summary>
    public class EntityMap
    {
        /// <summary>
        /// Gets or sets the <see cref="Entity"/> recognised by LUIS.
        /// </summary>
        public Entity Entity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GameObject"/> that represents the LUIS Entity.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// Gets or sets the resolver that resolved the entity.
        /// </summary>
        public IEntityResolver Resolver { get; set; }
    }
}
