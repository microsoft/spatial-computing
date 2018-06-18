using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisCacheLib.DataModel
{
    public class IntentItem
    {
        public string Id { get; set; }

        // Store timestamp
        public string CacheDate { get; set; }
        //and confidence
        public double Confidence { get; set; }

        // String match
        public string Utterance { get; set; }
        // String match
        public string Intent { get; set; }
        // Json ? entities / Key Value Pair
        public string JsonEntities { get; set; }

        public bool IsProcessed { get; set; }
    }
}
