using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyGenSummary
    {
        public int daily_gen_summary_solar_id { get; set; }
        public string state { get; set; }
        public string site { get; set; }
        public dynamic date { get; set; }
        public string location_name { get; set; }
        public int total_strings { get; set; }
        public dynamic ghi { get; set; }
        public dynamic poa { get; set; }
        public dynamic expected_kwh { get; set; }
        public dynamic inv_kwh { get; set; }
        public dynamic plant_kwh { get; set; }
        public dynamic inv_pr { get; set; }
        public dynamic plant_pr { get; set; }
        public dynamic ma { get; set; } 
        public dynamic iga { get; set; }
        public dynamic ega { get; set; }
        public dynamic ega_b { get; set; }
        public dynamic ega_c { get; set; }
        public dynamic inv_plf_ac { get; set; }
        public dynamic inv_plf_dc { get; set; }
        public dynamic plant_plf_ac { get; set; }
        public dynamic plant_plf_dc { get; set; }
        public dynamic pi { get; set; }
        public dynamic prod_hrs { get; set; }
        public dynamic lull_hrs_bd { get; set; }
        public dynamic usmh_bs { get; set; }
        public dynamic smh_bd { get; set; }
        public dynamic oh_bd { get; set; }
        public dynamic igbdh_bd { get; set; }
        public dynamic egbdh_bd { get; set; }
        public dynamic load_shedding_bd { get; set; }
        public dynamic total_bd_hrs { get; set; }
        public dynamic usmh { get; set; }
        public dynamic smh { get; set; }
        public dynamic oh { get; set; }
        public dynamic igbdh { get; set; }
        public dynamic egbdh { get; set; }
        public dynamic load_shedding { get; set; }
        public dynamic total_losses { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
        public string inverter { get; set; }
    }

}
