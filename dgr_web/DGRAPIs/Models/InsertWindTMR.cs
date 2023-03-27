using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertWindTMR
    {
        public string onmWTG { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public dynamic date_time { get; set; }
        public double avgActivePower { get; set; }
        public double avgWindSpeed { get; set; }
        public int mostRestructiveWTG { get; set; }
    }
}
