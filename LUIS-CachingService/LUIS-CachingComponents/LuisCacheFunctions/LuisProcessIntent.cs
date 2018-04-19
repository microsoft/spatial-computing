using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json.Linq;

using System.Net.Http.Headers;

namespace LuisCacheFunctions
{
    public static class LuisProcessIntent
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("LuisProcessIntent")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("luisToProcess", Connection = "")]string intentId,
            [MobileTable(ApiKeySetting = "LuisCacheMobileAppKey", TableName = "MyTable",
            MobileAppUriSetting = "LuisCacheMobileAppUri", Id = "{queueTrigger}")]JObject item,
            TraceWriter log)
        {
            log.Info($"C# Processing Intent ID : {intentId}");

            var appId = Environment.GetEnvironmentVariable("LuisAppId");
            var subscriptionKey = Environment.GetEnvironmentVariable("LuisSubscriptionKey");
            var LUISendpoint = $"{Environment.GetEnvironmentVariable("LuisEndpoint")}/apps/";
            bool isPreview = true;

            var client = new LuisClient(appId, subscriptionKey, isPreview);
            
            LuisResult res = await client.Predict(item["Utterance"].Value<string>());

            log.Info($"Luis result : {res.DialogResponse}");

        }
    }
}
