using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertWindTMLData
    {
        public string onm_wtg { get; set; }
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
        public dynamic variable { get; set; }
        public string PLC_min { get; set; }
        public string PLC_max { get; set; }
        public int PC_validity { get; set; }
    }
    public class GetWindTMLGraphData
    {

    }
}
