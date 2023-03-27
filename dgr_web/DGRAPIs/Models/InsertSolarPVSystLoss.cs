using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertSolarPVSystLoss
    {
        public string FY { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public int site_id { get; set; }
        public string site_name { get; set; }
        public dynamic alpha { get; set; }
        public dynamic near_shading { get; set; }
        public dynamic IAM_factor { get; set; }
        public dynamic soiling_factor { get; set; }
        public dynamic pv_loss { get; set; }
        public dynamic lid { get; set; }
        public dynamic array_missmatch { get; set; }
        public dynamic dc_ohmic { get; set; }
        public dynamic conversion_loss { get; set; }
        public dynamic plant_aux { get; set; }
        public dynamic system_unavailability { get; set; }
        public dynamic ac_ohmic { get; set; }
        public dynamic external_transformer { get; set; }
        public dynamic yoy_degradation { get; set; }
        public string module_degradation { get; set; }
        public int tstc { get; set; }
        public int tcnd { get; set; }
    }
}
