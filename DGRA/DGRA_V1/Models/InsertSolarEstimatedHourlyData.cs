using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertSolarEstimatedHourlyData
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic fy_date { get; set; }
        public int year { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public dynamic time { get; set; }
        public double glob_hor { get; set; }
        public double glob_inc { get; set; }
        public double t_amb { get; set; }
        public double t_array { get; set; }
        public double e_out_inv { get; set; }
        public double e_grid {get;set;}
        public double phi_ang { get; set; }

    }
}
