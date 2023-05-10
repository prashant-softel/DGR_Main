using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class TemperatureCorrectedPR {

        public dynamic date { get; set; }
        public int site_id { get; set; }
        public double actModWtTemp { get; set; }
        public double act_avg_mod_temp { get; set; }
        public double est_avg_mod_temp { get; set; }
        public double estModTemp { get; set; }


        public double plantTempPR { get; set; }
        public double jmrTempPR { get; set; }
    }
}
    