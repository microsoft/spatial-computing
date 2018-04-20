using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json.Linq;

using System.Net.Http.Headers;
using System.Collections.Generic;
//using LuisCacheModel;

namespace LuisCacheFunctions
{
    public static class LuisProcessIntent
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("LuisProcessIntent")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("luistoprocess", Connection = "")]string intentId,
            [MobileTable(TableName = "IntentItems",
            MobileAppUriSetting = "LuisCacheMobileAppUri", Id = "{queueTrigger}")]JObject item,
            TraceWriter log)
        {
            log.Info($"C# Processing Intent ID : {intentId}");

            // Erro handling
            if (item == null)
            {

                log.Info($"No itent item retrieved (null) - {intentId}");
                return;
            }

            // Ensure item require procesing otherwise skip it
            var isProcessed = (bool) item["IsProcessed"];
            if ( isProcessed == true)
            {
                log.Info($" ItemItemd with if {intentId} has already been processed {isProcessed} - will be skipped");
                return;
            }

            var appId = Environment.GetEnvironmentVariable("LuisAppId");
            var subscriptionKey = Environment.GetEnvironmentVariable("LuisSubscriptionKey");
            bool isPreview = true;

            var client = new LuisClient(appId, subscriptionKey, isPreview);
            //LuisResult res = await client.Predict(item["Utterance"].Value<string>());
            //log.Info($"Luis result : {res.DialogResponse}");
            //var processingResult = new ProcessingResult();

            List<string> entitiesNames = new List<string>();
            //var entities = res.GetAllEntities();
            //foreach (Entity entity in entities)
            //{
            //    processingResult.Entities.Add(entity.Name, entity.Value);
            //}

            //processingResult.Confidence = res.TopScoringIntent.Score;
            //processingResult.Intent = res.TopScoringIntent.Name;
            //processingResult.IsFromCache = false;

            //item["Intent"] = processingResult.Intent;
            //item["JsonEntities"] = JsonConvert.SerializeObject(processingResult.Entities);
            item["Intent"] = "Demo Intent";
            item["JsonEntities"] = "Demo entities";

        }
    }
}
