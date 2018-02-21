using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using KsparkAPI.DataObjects;
using KsparkAPI.Models;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace KsparkAPI.Controllers
{
    public class TelemetryItemController : TableController<TelemetryItem>
    {
        TelemetryClient telemetryClient = new TelemetryClient();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<TelemetryItem>(context, Request);
        }

        // GET tables/TelemetryItem
        public IQueryable<TelemetryItem> GetAllTelemetryItems()
        {
            return Query();
        }

        // GET tables/TelemetryItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TelemetryItem> GetTelemetryItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TelemetryItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TelemetryItem> PatchTelemetryItem(string id, Delta<TelemetryItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TelemetryItem
        public async Task<IHttpActionResult> PostTelemetryItem(TelemetryItem item)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("metrics", item.Metrics);

            telemetryClient.TrackEvent(item.CustomEvent, dict);
            TelemetryItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TelemetryItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTelemetryItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}