using Microsoft.Azure.Mobile.Server;

namespace KsparkAPI.DataObjects
{
    public class TelemetryItem : EntityData
    {
        public string UserId { get; set; }

        public string CustomEvent { get; set; }

        public string Metrics { get; set; }
    }
}