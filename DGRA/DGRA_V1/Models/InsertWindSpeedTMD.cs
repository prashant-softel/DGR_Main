using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertWindSpeedTMD
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public string time { get; set; }
        public dynamic date { get; set; }
        public double wind_speed { get; set; }
    }
}
