using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertWindTMR
    {
        public string onmWTG { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic date_time { get; set; }
        public string date { get; set; }
        public dynamic from_time { get; set; }
        public dynamic to_time { get; set; }
        public double avgActivePower { get; set; }
        public double avgWindSpeed { get; set; }
        public int mostRestructiveWTG { get; set; }
        public string status { get; set; }
        public int status_code { get; set; }
    }
}
