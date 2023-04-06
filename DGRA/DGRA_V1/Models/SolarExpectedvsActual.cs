using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class SolarExpectedvsActual
    {

        public int site_id { get; set; }
        public string site { get; set; }
        public string date { get; set; }
        public string location_name { get; set; }
        public int total_strings { get; set; }
        public double ghi { get; set; }
        public double poa { get; set; }
        public double expected_kwh { get; set; }
        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }


        public double pr { get; set; }
        public double toplining_PR { get; set; }


        public double ma { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }

        public double prod_hrs { get; set; }
        public double lull_hrs_bd { get; set; }
        public double usmh_bs { get; set; }
        public double smh_bd { get; set; }
        public double oh_bd { get; set; }
        public double igbdh_bd { get; set; }
        public double egbdh_bd { get; set; }
        public double load_shedding_bd { get; set; }
        public double total_bd_hrs { get; set; }

        public double usmh { get; set; }
        public double smh { get; set; }
        public double oh { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }
        public double total_losses { get; set; }

        public double Pexpected { get; set; }
        public double target { get; set; }


    }
}
