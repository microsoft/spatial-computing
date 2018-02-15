using Microsoft.Cognitive.LUIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MR.LUIS
{
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
    }
}
