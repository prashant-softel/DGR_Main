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
        public dynamic active_power { get; set; }
        public dynamic wind_speed { get; set; }
        public dynamic date { get; set; }
        public dynamic avg_active_power { get; set; }
        public dynamic avg_wind_speed { get; set; }
        public string wtg { get; set; }
    }
}
