using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace LuisCacheFunctions
{
    public static class LuisRetrainModel
    {
        [FunctionName("RetrainModel")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 * * */1 * *")]TimerInfo myTimer, TraceWriter log)

        {
            // check if there is untrained 
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            //var uri = "https://{location}.api.cognitive.microsoft.com/luis/api/v2.0/apps/{appId}/versions/{versionId}/train?" + queryString;

            var versionId = Environment.GetEnvironmentVariable("LuisAppVersion");
            var appId = Environment.GetEnvironmentVariable("LuisAppId");
            var subscriptionKey = Environment.GetEnvironmentVariable("LuisSubscriptionKey");
            var LUISendpoint = Environment.GetEnvironmentVariable("LuisApiEndpoint");
            var request = $"{LUISendpoint}/apps/{appId}/versions/{versionId}/train/?verbose=true&q=";

            // Final LUIS Query

            log.Info("LUIS query:" + request);
            var httpClient = new HttpClient();


            // Check if model need to be updated


            // LUIS HTTP CALL
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                // Can add the query at the end
            var response = await httpClient.PostAsync(request, null);

            // If LUIS error, return 204 - YOU SHOULD CHANGE THIS!!!
            if (!response.IsSuccessStatusCode)
            {
                // Log (HttpStatusCode.NoContent);
                log.Info($"HttpStatusCode.NoContent - status : {response.StatusCode} - reason :  {response.ReasonPhrase}");
            }

            var apiResult = await response.Content.ReadAsStringAsync();

            log.Info($"Response {apiResult}");

            #region auto publish
            // Model is trained above but not automatically published as this operation can cause unexpected behaviors and
            // you might want to test it befor publishing

            // Uncomment the code below to enable the feature (send a post request with the following settings)

            // Body to post
            //            {
            //                "versionId": "0.1",
            //   "isStaging": false,
            //   "region": "westus"
            //}

            // POST https://westus.api.cognitive.microsoft.com/luis/api/v2.0/apps/{appId}/publish
            #endregion
        
        }
    }
}
