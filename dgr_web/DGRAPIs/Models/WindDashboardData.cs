using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDashboardData
    {
        public dynamic Date { get; set; }
        public dynamic month { get; set; }
        public dynamic year { get; set; }
        public string fy { get; set; }
        public string Site { get; set; }
        public dynamic Wind { get; set; }
        public dynamic KWH { get; set; }
        public dynamic line_loss { get; set; }
        public dynamic jmrkwh { get; set; }
        public dynamic tarkwh { get; set; }
        public dynamic tarwind { get; set; }
        public dynamic tar_date { get; set; }
        public dynamic total_mw { get; set; }
        public int site_id { get; set; }
        public dynamic expected_power { get; set; }
    }
    public class WindDashboardData1
    {

        public dynamic Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string fy { get; set; }
        public string Site { get; set; }
        public double Wind { get; set; }

       // public double KWH { get; set; }
       // public string line_loss { get; set; }
        public double jmrkwh { get; set; }
        public double tarkwh { get; set; }
        public double tarwind { get; set; }
        public double total_mw { get; set; }
       
    }
}
