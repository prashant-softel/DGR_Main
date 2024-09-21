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
            public dynamic dc_capacity { get; set; }
            public dynamic ac_capacity { get; set; }
        	public dynamic total_tarrif { get; set; }
			public dynamic commissioning_date { get; set; }

         
    }
}
