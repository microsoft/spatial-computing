using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class StartupScript
    : MonoBehaviour
{
    public string ClientId;

    public string ClientSecret;

    public string TenantId;

    // Use this for initialization
    void Start()
    {
        // TODO: use secure string...
        var username = "testuser@jwendl.net";
        var password = "";

        StartCoroutine(Authenticate((identityResult) =>
        {
            // Update UI here...
        }, username, password));

        //var scopes = new List<string>()
        //{
        //    "User.Read",
        //};

        //var publicClientApplication = new PublicClientApplication(clientId);
        //var user = publicClientApplication.Users.FirstOrDefault();

        //if (user == default(User))
        //{
        //    // No user in token cache
        //    var authenticationResult = publicClientApplication.AcquireTokenAsync(scopes).Result;
        //}
        //else
        //{
        //    // Call out to UI to get username and password
        //    var authenticationResult = publicClientApplication.AcquireTokenSilentAsync(scopes, user).Result;
        //}
    }

    IEnumerator Authenticate(Action<IdentityResult> identityCallback, string username, string password)
    {
        var resource = "https://graph.microsoft.com";
        // TODO: these two are public properties so they are entered in unity UI?

        var grantType = "password";
        var scope = "openid";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("resource", resource),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("client_secret", ClientSecret),
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("scope", scope),
        });

        var httpClient = new HttpClient();
        var httpResponseMessageTask = httpClient.PostAsync($"https://login.windows.net/{TenantId}/oauth2/token", formContent);
        var jsonTask = httpResponseMessageTask.Result.Content.ReadAsStringAsync();

        // Below doesn't work because their deserializer expects numbers to be 3 instead of "3"...
        //var identityResult = JsonUtility.FromJson<IdentityResult>(jsonTask.Result);
        var identityResult = JsonConvert.DeserializeObject<IdentityResult>(jsonTask.Result);

        yield return new WaitUntil(() => true);

        identityCallback(identityResult);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
