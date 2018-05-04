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
using UnityEngine;

namespace Microsoft.MR.LUIS
{
    /// <summary>
    /// Provides contextual information that can be used during a LUIS prediction.
    /// </summary>
    public class PredictionContext : Dictionary<string, object>
    {
        #region Constants
        public const string GazedObjectKey = "GazedObject";
        public const string PointedObjectKey = "PointedObject";
        public const string PredictionTextKey = "PredictionText";
        public const string SpokenTextKey = "SpokenText";
        #endregion // Constants

        /// <summary>
        /// Gets or sets the object that the user was gazing at for the prediction.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the object that the user was pointing at for the prediction.
        /// </summary>
        public GameObject PointedObject
        {
            get
            {
                return this[PointedObjectKey] as GameObject;
            }
            set
            {
                this[PointedObjectKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets the object that the user was pointing at for the prediction.
        /// </summary>
        public string PredictionText
        {
            get
            {
                return this[PredictionTextKey] as string;
            }
            set
            {
                this[PredictionTextKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets the text that the user spoke.
        /// </summary>
        public string SpokenText
        {
            get
            {
                return this[SpokenTextKey] as string;
            }
            set
            {
                this[SpokenTextKey] = value;

                // If spoken text is a valid string and there is no 
                // prediction text, set the prediction text as well.
                if (!string.IsNullOrEmpty(value) && (!this.ContainsKey(PredictionTextKey)))
                {
                    PredictionText = value;
                }
            }
        }
    }
}
