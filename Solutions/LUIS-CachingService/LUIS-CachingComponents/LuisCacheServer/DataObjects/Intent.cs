using Microsoft.Azure.Mobile.Server;
using System;

namespace KsparkAPI.DataObjects
{
    public class IntentItem : EntityData
    {
        // Store the luis confidence
        public double Confidence { get; set; }
        // String match
        public string Utterance { get; set; }
        // String match
        public string Intent { get; set; }
        // Json ? entities / Key Value Pair
        public string JsonEntities { get; set; }
        // Last sync date
        public string CacheDate { get; set; }

        public bool IsProcessed { get; set; }
    }
}