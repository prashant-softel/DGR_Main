using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class GetPowerCurveData
    {
        public int power_curve_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public double active_power { get; set; }
        public double wind_speed { get; set; }
        public dynamic date { get; set; }
        public double avg_active_power { get; set; }
        public double avg_wind_speed { get; set; }
    }
}
