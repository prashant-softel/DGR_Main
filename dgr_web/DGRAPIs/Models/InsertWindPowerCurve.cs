using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class InsertWindPowerCurve
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public double active_power { get; set; }
        public double wind_speed { get; set; }
    }
}
