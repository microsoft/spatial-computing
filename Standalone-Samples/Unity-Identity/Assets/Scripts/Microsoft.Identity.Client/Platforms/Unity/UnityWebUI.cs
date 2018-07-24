#if UNITY_EDITOR
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Internal.Interfaces;
using System;
using System.Threading.Tasks;

namespace Assets.Scripts.MSAL.Platforms.Unity
{
    internal class UnityWebUI
        : IWebUI
    {
        public RequestContext RequestContext { get; set; }

        public Task<AuthorizationResult> AcquireAuthorizationAsync(Uri authorizationUri, Uri redirectUri, RequestContext requestContext)
        {
            throw new NotImplementedException();
        }
    }
}
#endif