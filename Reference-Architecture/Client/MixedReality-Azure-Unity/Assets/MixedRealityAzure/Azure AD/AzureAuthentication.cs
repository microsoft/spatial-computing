using Assets.MixedRealityAzure.AzureAD.Models;
using Microsoft.MR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class AzureAuthentication
    : MonoBehaviour
{
    [Tooltip("The application ID from Azure AD.")]
    [SecretValue("AAD.ClientId")]
    public string ClientId;

    [Tooltip("The client secret for the application.")]
    [SecretValue("AAD.ClientSecret")]
    public string ClientSecret;

    [Tooltip("The tenant ID for AAD.")]
    [SecretValue("AAD.TenantId")]
    public string TenantId;

    [Tooltip("The resource name like https://graph.microsoft.com.")]
    [SecretValue("AAD.Resource")]
    public string Resource;

    [Tooltip("The user's Username.")]
    [SecretValue("AAD.Username")]
    public string Username;

    [Tooltip("The user's password.")]
    [SecretValue("AAD.Password")]
    public string Password;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Authenticate((identityResult) =>
        {
            // Update UI here...

            // Call out to other service using identityResult.access_token
        }));
    }

    IEnumerator Authenticate(Action<IdentityResult> identityCallback)
    {
        var grantType = "password";
        var scope = "openid";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("resource", Resource),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("client_secret", ClientSecret),
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("username", Username),
            new KeyValuePair<string, string>("password", Password),
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
