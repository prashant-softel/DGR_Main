using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertWindTMLData
    {
        public string WTGs { get; set; }
        public dynamic timestamp { get; set; }
        public double avg_active_power { get; set; }
        public double avg_wind_speed { get; set; }
        public int restructive_WTG { get; set; }
        public string date { get; set; }
        public dynamic from_time { get; set; }
        public dynamic to_time { get; set; }
        public string status { get; set; }
        public int status_code { get; set; }
    }
}
