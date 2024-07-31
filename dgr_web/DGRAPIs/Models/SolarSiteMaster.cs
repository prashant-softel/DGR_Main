using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarSiteMaster
    {
            public int site_master_solar_id { get; set; }
            public string country { get; set; }
       
            public string site { get; set; }
 			public dynamic doc { get; set; }
            public string spv { get; set; }
            public string state { get; set; }
            public double dc_capacity { get; set; }
            public double ac_capacity { get; set; }
        	public double total_tarrif { get; set; }
            public string inv_make { get; set; }
            public string inv_model { get; set; }
            public double inv_capacity { get; set; }
            public double lat_long { get; set; }  
            public double pss_capacity { get; set; }
            public string gss_name { get; set; }
            public double gss_voltage { get; set; }
            public double gss_capacity { get; set; }
            public double type_of_project { get; set; }
            public dynamic commissioning_date { get; set; }
   
    }
}
