using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Cognitive.LUIS;
using System.Collections;
using Microsoft.WindowsAzure.MobileServices;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using LuisCacheLib.DataModel;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;
using LuisCacheModel;
using System.IO;
using LuisCacheModel.Extension;

namespace LuisCacheLib
{
    public class TextProcessor
    {
        // TODO: Refactor to get from App Settings
        string luisAppId = null;
        // TODO: Refactor to get from App Settings
        string luisSubscriptionKey = null;
        // TODO: Refactor to get from App Settings
        string mobileAppUri = null;
        public MobileServiceClient MobileService { get; set; }

        private IMobileServiceSyncTable<IntentItem> intentTable = null; // offline sync
        private MobileServiceCollection<IntentItem, IntentItem> items;

        private IMobileServiceSyncTable<TelemetryItem> telemetryTable = null;
        private MobileServiceCollection<TelemetryItem, TelemetryItem> telemetryItems;

        private LuisClient _client;

        //private static TextProcessor instance = null;

        public TextProcessor(string luisAppId, string luisSubscriptionKey, string mobileAppUri) //:base()
        {
            this.mobileAppUri = mobileAppUri;
            this.luisAppId = luisAppId;
            this.luisSubscriptionKey = luisSubscriptionKey;

            MobileService = new MobileServiceClient(mobileAppUri);
            intentTable = MobileService.GetSyncTable<IntentItem>();
            telemetryTable = MobileService.GetSyncTable<TelemetryItem>();

            try
            {
                var t = Task.Run(async () =>
                {
                    await InitLocalStoreAsync();
                });
                t.Wait();
            }
            // Case of offline start
            // TODO imporve error handling
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            bool preview = true;

            _client = new LuisClient(luisAppId, luisSubscriptionKey, preview);

        }

        //private TextProcessor()
        //{
        //}

        //public static TextProcessor Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new TextProcessor();
        //        }
        //        return instance;
        //    }
        //}

        public async Task<ProcessingResult> Predict(string textToPredict)
        {
            textToPredict = textToPredict.Trim();
            textToPredict = textToPredict.ToLower();

            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the IntentItems table.
                // The query excludes completed IntentItems.
                items = await intentTable
                    .Where(IntentItem => IntentItem.Utterance == (textToPredict))
                    .ToCollectionAsync();

                telemetryItems = await telemetryTable.Take(1)
                    .ToCollectionAsync();

                if (items.Count > 0)
                {
                    var item = items[0];
                    var processingResult = new ProcessingResult();
                    processingResult.Entities = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.JsonEntities);
                    processingResult.Intent = item.Intent;
                    processingResult.IsFromCache = true;

                    await Track(TelemetryEvents.CACHE_HIT, textToPredict);
                    await Track(TelemetryEvents.LUIS_INTENT, processingResult.Intent);

                    return processingResult;
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
                Debug.WriteLine(e);
                throw e;
            }
            

            await Track(TelemetryEvents.CACHE_MISS, textToPredict);

            try
            {                
                LuisResult res = await _client.Predict(textToPredict);
                var intent = processRes(res);

                var temp = new ProcessingResult();
                temp.Confidence = intent.Confidence;
                temp.Entities = intent.Entities;
                temp.Intent = intent.Intent;
                temp.IsFromCache = true;

                var intentItem = new IntentItem();
                intentItem.Intent = temp.Intent;
                intentItem.Utterance = textToPredict;
                intentItem.IsProcessed = true;
                intentItem.JsonEntities = JsonConvert.SerializeObject(temp.Entities);

                // Insert into database
                await InsertIntentItem(intentItem);
                await Track(TelemetryEvents.LUIS_INTENT, intentItem.Intent);

                return intent;
            }
            catch (MobileServicePushFailedException push)
            {
                //
                Debug.WriteLine(push);
                throw push;
            }
            // TODO: Refactor to have a better error handling
            catch (System.Exception e)
            {
                // Check App ID,  Luis Key , Luis URL
                // Trace error
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e);
            }

            // Save offline
            var offlineIntent = new IntentItem();
            offlineIntent.Utterance = textToPredict;
            offlineIntent.IsProcessed = false;

            await InsertIntentItem(offlineIntent);

            return null;
        }

        private async Task Track(string customEvent, string mectrics)
        {
            TelemetryItem telemetryItem = new TelemetryItem();
            telemetryItem.CustomEvent = customEvent;
            telemetryItem.Metrics = mectrics;

            await InsertTelemetryItem(telemetryItem);
        }

        private ProcessingResult processRes(LuisResult res)
        {
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
        
            return processingResult;
        }

        private async Task InsertTelemetryItem(TelemetryItem telemetryItem)
        {
            try
            {
                // This code inserts a new IntentItem into the database. After the operation completes
                // and the mobile app backend has assigned an id, the item is added to the CollectionView.
                await telemetryTable.InsertAsync(telemetryItem);

                telemetryItems.Add(telemetryItem);

                await MobileService.SyncContext.PushAsync(); // offline sync
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task InsertIntentItem(IntentItem IntentItem)
        {
            try
            {
                // This code inserts a new IntentItem into the database. After the operation completes
                // and the mobile app backend has assigned an id, the item is added to the CollectionView.
                await intentTable.InsertAsync(IntentItem);
                
                items.Add(IntentItem);

                await MobileService.SyncContext.PushAsync(); // offline sync
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #region Offline sync

        private async Task InitLocalStoreAsync()
        {
            string databaseFilename = "localstoreluis.db";

            if (!MobileService.SyncContext.IsInitialized)
            {
                var folder = ApplicationData.Current.LocalFolder;
                
                try
                {
                    var file = await folder.GetFileAsync(databaseFilename);
                    await file.DeleteAsync();
                }
                catch (FileNotFoundException e)
                {
                    Debug.WriteLine("DB does not exit - " + e);
                }                

                var store = new MobileServiceSQLiteStore(databaseFilename);
                
                store.DefineTable<IntentItem>();
                store.DefineTable<TelemetryItem>();
                await MobileService.SyncContext.InitializeAsync(store);
            }

            await SyncAsync();
        }

        private async Task SyncAsync()
        {
            try
            {
                await MobileService.SyncContext.PushAsync();
                await intentTable.PullAsync("IntentItems", intentTable.Where(i => i.IsProcessed == true ));
                //await intentTable.PullAsync(null, intentTable.Where(i => i.IsProcessed == true));
                await telemetryTable.PullAsync("Telemetry", telemetryTable.Where(t => t.UserId == "1"));
            }
            // TODO: improve error handling
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }
        #endregion
    }
}
