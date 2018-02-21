using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using KsparkAPI.DataObjects;
using KsparkAPI.Models;
using Microsoft.Cognitive.LUIS;
using LuisCacheModel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KsparkAPI.Controllers
{
    public class IntentItemController : TableController<IntentItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<IntentItem>(context, Request);
        }

        // GET tables/IntentItem
        public IQueryable<IntentItem> GetAllIntentItems()
        {
            System.Diagnostics.Trace.TraceInformation("Get All Intent Items");

            return Query();
        }

        // GET tables/IntentItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<IntentItem> GetIntentItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/IntentItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<IntentItem> PatchIntentItem(string id, Delta<IntentItem> patch)
        {
            System.Diagnostics.Trace.TraceInformation("Patch item");

            return UpdateAsync(id, patch);
        }

        // POST tables/IntentItem
        public async Task<IHttpActionResult> PostIntentItem(IntentItem item)
        {
            System.Diagnostics.Trace.TraceInformation("Positing new intent: " + item.Utterance);

            IntentItem current;
            if (!item.IsProcessed)
            {
                current = await CreateIntentItem(item);
                current.IsProcessed = true;
            }
            else
            {
                current = item;
            }

            current = await InsertAsync(current);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        private async Task<IntentItem> CreateIntentItem(IntentItem item)
        {
            string appId = "9f25daaa-3d94-4988-b201-f10a2d41a6a5";
            string subscriptionKey = "07f77f9e723140aeb54b4ed346752851";
            bool preview = true;

            var client = new LuisClient(appId, subscriptionKey, preview);

            LuisResult res = await client.Predict(item.Utterance);

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

            return item;
        }

        // DELETE tables/IntentItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteIntentItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}