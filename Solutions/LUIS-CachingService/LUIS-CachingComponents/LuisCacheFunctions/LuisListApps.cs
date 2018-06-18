using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace LuisCacheFunctions
{
    public static class LuisListApps
    {
        [FunctionName("ListApps")]
        public static async System.Threading.Tasks.Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req, TraceWriter log)
        {
            // check if there is untrained 
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var versionId = Environment.GetEnvironmentVariable("LuisAppId");
            var appId = Environment.GetEnvironmentVariable("LuisAppId");
            var subscriptionKey = Environment.GetEnvironmentVariable("LuisSubscriptionKey");
            var LUISendpoint = Environment.GetEnvironmentVariable("LuisApiEndpoint");
            var request = $"{LUISendpoint}/apps/?skip=0&take=100";

            // Final LUIS Query
            log.Info("LUIS query:" + request);
            var httpClient = new HttpClient();

            // LUIS HTTP CALL
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                // Can add the query at the end
            var response = await httpClient.GetAsync(request);

            // 
            if (!response.IsSuccessStatusCode)
            {
                // Log (HttpStatusCode.NoContent);
                log.Info($"HttpStatusCode.NoContent - status : {response.StatusCode} - reason :  {response.ReasonPhrase}");
            }

            var apiResult = await response.Content.ReadAsStringAsync();

            log.Info($"Response {apiResult}");

            return new OkObjectResult(apiResult);
        }
    }
}
