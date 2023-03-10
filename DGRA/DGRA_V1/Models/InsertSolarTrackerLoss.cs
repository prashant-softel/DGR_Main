using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertSolarTrackerLoss
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public double ac_capacity { get; set; }
        public dynamic date { get; set; }
        public dynamic from_time { get; set; }
        public dynamic to_time { get; set; }
        public int trackers_in_BD { get; set; }
        public int module_tracker { get; set; }
        public int module_WP { get; set; }
        public double BD_tracker_cap { get; set; }
        public double Actual_POA { get; set; }
        public double Actual_GHI { get; set; }
        public dynamic target_AOP { get; set; }
        public int gen_losses { get; set; }
        public string remark { get; set; }
        public string month { get; set; }
        public string reason { get; set; }

    }
}
