using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindSiteMaster
    {
        public int site_master_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public dynamic doc { get; set; }
        public string spv { get; set; }
        public string state { get; set; }
        public string model { get; set; }
        public dynamic capacity_mw { get; set; }
        public dynamic wtg { get; set; }
        public dynamic total_mw { get; set; }
        public dynamic tarrif { get; set; }
        public dynamic gbi { get; set; }
        public dynamic total_tarrif { get; set; }
        public dynamic ll_compensation { get; set; }
    }
}
