using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDashboardData
    {
        public DateTime Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string fy { get; set; }
        public string Site { get; set; }
        public decimal Wind { get; set; }
        public decimal KWH { get; set; }
        public decimal line_loss { get; set; }
        public decimal jmrkwh { get; set; }
        public decimal tarkwh { get; set; }
        public decimal tarwind { get; set; }
        public decimal tar_date { get; set; }
        public decimal total_mw { get; set; }
        public int site_id { get; set; }
        public decimal expected_power { get; set; }
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
