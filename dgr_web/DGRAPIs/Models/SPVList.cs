
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SPVList
    {
        public string spv { get; set; }
       
    }
    public class SPVSiteWind
    {
        public int site_id {get; set;}
        public string spv { get; set; }
        public string site { get; set; }
    }

    public class SPVGroup
    {
        public string site_ids { get; set; }
        public string spv { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double total_tarrif { get; set; }

    }
    public class CustomeGroup
    {
        public string site_ids { get; set; }
        public string cust_group { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double total_tarrif { get; set; }

    }
}
