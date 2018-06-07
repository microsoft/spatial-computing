using System;
using System.Collections.Generic;
using System.Text;

namespace LuisCacheModel.Extension
{
    public static class TelemetryEvents
    {
        public const string CACHE_HIT = "Cache.Hit";
        public const string CACHE_MISS = "Cache.Miss";
        public const string LUIS_INTENT = "Luis.Intent";
    }
}
