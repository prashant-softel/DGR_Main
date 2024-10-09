using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDashboardData
    {
        public DateTime Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string fy { get; set; }
        public string Site { get; set; }
        public decimal IR { get; set; }
        public decimal inv_kwh { get; set; }
        public decimal line_loss { get; set; }
        public decimal lineLoss { get; set; }
        public decimal jmrkwh { get; set; }
        public decimal tarkwh { get; set; }
        public decimal tarIR { get; set; }
        public decimal ac_capacity { get; set; }
        public int site_id { get; set; }
        public decimal expected_power { get; set; }

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
