using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisCacheModel
{
    public class ProcessingResult
    {
        public string Intent;

        public Dictionary<string, string> Entities = new Dictionary<string, string>();

        public bool IsFromCache;

        public double Confidence;
    }
}
