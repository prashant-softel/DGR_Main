using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDashboardData
    {
        public dynamic Date { get; set; }
        public double month { get; set; }
        public dynamic year { get; set; }
        public string fy { get; set; }

        public string Site { get; set; }
        public double IR { get; set; }

        public double inv_kwh { get; set; }
        public double line_loss { get; set; }
        public double lineLoss { get; set; }
        public double jmrkwh { get; set; }
        public double tarkwh { get; set; }
        public double tarIR { get; set; }
        public double ac_capacity { get; set; }
        public int site_id { get; set; }
        public double expected_power { get; set; }

    }
    public class SolarDashboardData1
    {
        public dynamic Date { get; set; }
        public string fy { get; set; }
        public double IR { get; set; }
        public double inv_kwh { get; set; }
        public double tarkwh { get; set; }
        public double tarIR { get; set; }
        public double ac_capacity { get; set; }


    }
}
