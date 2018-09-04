using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json.Linq;

using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using LuisCacheModel.DataModel;
using LuisCacheModel;
using Newtonsoft.Json;
//using LuisCacheModel;

namespace LuisCacheFunctions
{
    public static class LuisProcessIntent
    {
        static HttpClient httpClient = new HttpClient();
        static MobileServiceClient MobileService = new MobileServiceClient(Environment.GetEnvironmentVariable("LuisCacheMobileAppUri"));
        static IMobileServiceTable<IntentItem> intentTable = MobileService.GetTable<IntentItem>();

        [FunctionName("LuisProcessIntent")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("* */1 * * * *")]TimerInfo myTimer,
            TraceWriter log)
        {
            IList<IntentItem> items = null;

            try
            {
                // Retrieve all non processed items
                items = await intentTable.Where(i => i.IsProcessed == false).ToListAsync();
            }
            catch(Exception ex)
            {
                log.Error($"Error while Retrieving all IntentItems : {ex}");
                throw ex;
            }

            log.Info($"Total items to process = {items.Count}");

            foreach (var item in items)
            {
                var intentId = item.Id;

                log.Info($"C# Processing Intent ID : {intentId}");

                // Erro handling
                if (item == null)
                {
                    log.Info($"No itent item retrieved (null) - {intentId}");
                    return;
                }

                // Ensure item require procesing otherwise skip it
                if (item.IsProcessed == true)
                {
                    log.Info($" ItemItemd with if {intentId} has already been processed {item.IsProcessed} - will be skipped");
                    return;
                }

                // Calling Luis
                var appId = Environment.GetEnvironmentVariable("LuisAppId");
                var subscriptionKey = Environment.GetEnvironmentVariable("LuisSubscriptionKey");
                bool isPreview = true;

                var client = new LuisClient(appId, subscriptionKey, isPreview);
                LuisResult res = await client.Predict(item.Utterance);
                log.Info($"Luis result : {res.DialogResponse}");
                var processingResult = new ProcessingResult();

                List<string> entitiesNames = new List<string>();
                var entities = res.GetAllEntities();
                foreach (Entity entity in entities)
                {
                    processingResult.Entities.Add(entity.Name, entity.Value);
                }

                processingResult.Confidence = res.TopScoringIntent.Score;
                processingResult.Intent = res.TopScoringIntent.Name;
                processingResult.IsFromCache = false;

                item.Intent = processingResult.Intent;
                item.JsonEntities = JsonConvert.SerializeObject(processingResult.Entities);
                item.IsProcessed = true;

                await intentTable.UpdateAsync(item);
            }
        }
    }
}
