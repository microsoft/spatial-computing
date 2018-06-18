using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisCacheLib.DataModel
{
    public class TelemetryItem
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string CustomEvent { get; set; }

        public string Metrics { get; set; }
    }
}
