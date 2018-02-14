using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MR.LUIS
{
    /// <summary>
    /// Provides contextual information that can be used during a Mixed Reality LUIS prediction.
    /// </summary>
    public class PredictionContext : Dictionary<string, object>
    {
        #region Constants
        private const string GazedObjectKey = "GazedObjectKey";
        private const string PointedObjectKey = "PointedObjectKey";
        #endregion // Constants

        public GameObject GazedObject
        {
            get
            {
                return this[GazedObjectKey] as GameObject;
            }
            set
            {
                this[GazedObjectKey] = value;
            }
        }
    }
}
