using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DGRAPIs.Models
{
    //Model Not Used
    public class SolarTrackerLoss
    {
        public int uploading_file_tracker_loss_id { get; set; }
        public string site { get; set; }
        public double ac_capacity { get; set; }
        public DateTime date { get; set; }
        public TimeSpan from_time { get; set; }
        public TimeSpan to_time { get; set; }
        public int trackers_in_BD { get; set; }
        public int module_tracker { get; set; }
        public int module_WP { get; set; }
        public string remark { get; set; }
        public dynamic tracker_loss { get; set; }
        public dynamic breakdown_tra_capacity { get; set; }
        public dynamic actual_poa { get; set; }
        public dynamic actual_ghi { get; set; }
        public dynamic target_aop_pr { get; set; }
        public string month { get; set; }

    }
}
