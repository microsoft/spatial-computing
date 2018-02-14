using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MR.LUIS
{
    /// <summary>
    /// Captures context for a LUIS prediction.
    /// </summary>
    public interface IContextProvider
    {
        /// <summary>
        /// Captures context information for a LUIS prediction.
        /// </summary>
        /// <param name="context">
        /// The <see cref="PredictionContext"/> where information will be stored.
        /// </param>
        void CaptureContext(PredictionContext context);
    }
}
