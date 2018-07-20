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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MR.SharedAlignment
{
	/// <summary>
	/// The main manager for shared alignment control.
	/// </summary>
	public class AlignmentManager : MonoBehaviour
	{
		#region Unity Inspector Variables
		[Tooltip("The optimized model")]
		public GameObject optimizedModel;

		[Tooltip("The original unoptimized model")]
		public GameObject originalModel;
		#endregion // Unity Inspector Variables

		#region Public Methods
		/// <summary>
		/// Shows the optimized model.
		/// </summary>
		public void ShowOptimized()
		{
			if (originalModel != null) { originalModel.SetActive(false); }
			if (optimizedModel != null) { optimizedModel.SetActive(true); }
		}

		/// <summary>
		/// Shows the original model
		/// </summary>
		public void ShowOriginal()
		{
			if (optimizedModel != null) { optimizedModel.SetActive(false); }
			if (originalModel != null) { originalModel.SetActive(true); }
		}
		#endregion // Public Methods
	}
}