using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartupScript
    : MonoBehaviour
{
    public GameObject UnityWebBrowser;

    // Use this for initialization
    void Start()
    {
        var clientId = "2437fe4e-b874-48ec-b3ee-e6658f6abdb9";
        var scopes = new List<string>()
        {
            "User.Read",
        };

        var publicClientApplication = new PublicClientApplication(clientId);
        var user = publicClientApplication.Users.FirstOrDefault();

        if (user == default(User))
        {
            // No user in token cache
            var authenticationResult = publicClientApplication.AcquireTokenAsync(scopes).Result;
        }
        else
        {
            // Call out to UI to get username and password
            var authenticationResult = publicClientApplication.AcquireTokenSilentAsync(scopes, user).Result;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
