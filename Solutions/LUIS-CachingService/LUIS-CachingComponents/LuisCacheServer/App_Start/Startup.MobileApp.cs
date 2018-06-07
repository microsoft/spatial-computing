using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using KsparkAPI.DataObjects;
using KsparkAPI.Models;
using Owin;
using LuisCacheServer.App_Start;
using WebApi2WithMVC;

namespace KsparkAPI
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            // Intent

            List<IntentItem> intentItems = new List<IntentItem>
            {
                new IntentItem { Id = Guid.NewGuid().ToString(), Confidence = 0.9, Utterance = "Move the turbine",
                    Intent = "MoveHologram", JsonEntities = @"{target:motor}" , CacheDate = (DateTime.Now.AddDays(-1)).ToString()},
                new IntentItem { Id = Guid.NewGuid().ToString(), Confidence = 0.9, Utterance= "make this bigger",
                    Intent = "Scale", JsonEntities = @"{target:this}", CacheDate = (DateTime.Now.AddDays(-1)).ToString() },
            };

            foreach (IntentItem item in intentItems)
            {
                context.Set<IntentItem>().Add(item);
            }

            // Telemetry
            List<TelemetryItem> telemetryItems = new List<TelemetryItem>
            {
                new TelemetryItem { Id = Guid.NewGuid().ToString(), CustomEvent = "MoveHologram", Metrics = @"name:Turbine" }
            };

            foreach (IntentItem item in intentItems)
            {
                context.Set<IntentItem>().Add(item);
            }

            base.Seed(context);
        }
    }
}

