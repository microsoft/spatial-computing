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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

#if !UNITY_WSA || UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
#endif

namespace Microsoft.MR
{
	/// <summary>
	/// An attribute that can be used to designate where a secret value is held.
	/// </summary>
	[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class SecretValueAttribute : Attribute
	{
		#region Member Variables
		private string name;
		#endregion // Member Variables

		#region Constructors
		/// <summary>
		/// Initializes a new <see cref="SecretValueAttribute"/> instance.
		/// </summary>
		/// <param name="name">
		/// The name of the secret value.
		/// </param>
		public SecretValueAttribute(string name)
		{
			// Validate
			if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

			// Store
			this.name = name;
		}
		#endregion // Constructors

		#region Public Properties
		/// <summary>
		/// Gets the name of the secret.
		/// </summary>
		public string Name => name;
		#endregion // Public Properties
	}

	/// <summary>
	/// A class that helps deal with secret keys and other values that should not be 
	/// checked into source control.
	/// </summary>
	static public class SecretHelper
	{
		#region Internal Methods
		/// <summary>
		/// Attempts to load a secret value into the specified field.
		/// </summary>
		/// <param name="sva">
		/// A <see cref="SecretValueAttribute"/> that indicates the source of the secret value.
		/// </param>
		/// <param name="field">
		/// The field where the value will be loaded.
		/// </param>
		/// <param name="obj">
		/// The object instance where the value will be set.
		/// </param>
		static private void TryLoadValue(SecretValueAttribute sva, FieldInfo field, object obj)
		{
			// Try to get the environment variable
			string svalue = Environment.GetEnvironmentVariable(sva.Name);

			// No variable or no value?
			if (string.IsNullOrEmpty(svalue))
			{
				Debug.LogWarning($"The environment variable {sva.Name} is either missing or has no value.");
				return;
			}

			// If string, just assign. Otherwise attempt to convert.
			if (field.FieldType == typeof(string))
			{
				field.SetValue(obj, svalue);
			}
			else
			{
				try
				{
					object cvalue = Convert.ChangeType(svalue, field.FieldType);
					field.SetValue(obj, cvalue);
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"The value '{svalue}' of environment variable {sva.Name} could not be converted to {field.FieldType.Name}. {ex.Message}");
				}
			}
		}
		#endregion // Internal Methods

		#region Public Methods
		/// <summary>
		/// Attempts to load all secret values for the specified object.
		/// </summary>
		/// <param name="obj">
		/// The object where secret values will be loaded.
		/// </param>
		static public void LoadSecrets(object obj)
		{
			// Validate
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			// Get all fields
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			// Look for secret fields
			foreach (var field in fields)
			{
				// Try to get attribute
				SecretValueAttribute sva = field.GetCustomAttribute<SecretValueAttribute>();

				// If not a secret, skip
				if (sva == null) { continue; }

				// Try to load the value
				TryLoadValue(sva, field, obj);
			}
		}
		#endregion // Public Methods
	}
}