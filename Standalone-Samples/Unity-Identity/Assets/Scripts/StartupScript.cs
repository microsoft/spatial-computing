using Microsoft.Identity.Client;
using System.Collections.Generic;
using UnityEngine;

public class StartupScript
    : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var clientId = "";
        var scopes = new List<string>()
        {
            "User.Read",
        };

        var publicClientApplication = new PublicClientApplication(clientId);
        var authenticationResult = publicClientApplication.AcquireTokenAsync(scopes).Result;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
