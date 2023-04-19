using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertWindTMLData
    {
        public int uploading_file_TMR_Data_id { get; set; }
        public string WTGs { get; set; }
        public int wtg_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic timestamp { get; set; }
        public double avg_active_power { get; set; }
        public double avg_wind_speed { get; set; }
        public int restructive_WTG { get; set; }
        public string date { get; set; }
        public dynamic from_time { get; set; }
        public dynamic to_time { get; set; }
        public string status { get; set; }
        public int status_code { get; set; }
        public double recon_wind_speed { get; set; }
        public double exp_power_kw { get; set; }
        public double deviation_kw { get; set; }
        public double loss_kw { get; set; }
        public string manual_bd { get; set; }
        public string all_bd { get; set; }
        public dynamic created_on { get; set; }
    }
    public class WindSpeedData
    {
        public int windspeed_tmd_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public dynamic tmd_date { get; set; }
        public dynamic tmd_time { get; set; }
        public double windspeed { get; set; }
        public double averageWindSpeed { get; set; }
    }
}
