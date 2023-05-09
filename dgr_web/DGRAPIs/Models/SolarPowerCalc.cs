using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarPowerCalc { 
        public int year { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public int site_id { get; set; }
        public string site { get; set; }
        public double near_sheding { get; set; }

        public double iam_factor { get; set; }
        public double soiling_factor { get; set; }
        public double pv_loss { get; set; }
        public double lid { get; set; }
        public double array_missmatch { get; set; }
        public double dc_ohmic { get; set; }
        public double conversion_loss { get; set; }
        public double plant_auxilary { get; set; }
        public double system_unavailability { get; set; }
        public double ac_ohmic { get; set; }
        public double external_transformer { get; set; }

        public double yoy_degradation { get; set; }
        public double alpha { get; set; }
        //For POA
        public double tstc { get; set; }
        public double tcnd { get; set; }

        public double far_shading { get; set; }
        public double module_quality_loss { get; set; }
        public double electrical_loss { get; set; }
        public double night_consumption { get; set; }
        public double idt { get; set; }
        public double line_losses { get; set; }



        public string module_degradarion { get; set; }
        //TBCalculated
        public double Loss_factor { get; set; }
    }
    public class SolarPowerCalcReturn
    {
        public int year { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public string date { get; set; }
        public string site_id { get; set; }
        public double Pexpected { get; set; }

    }

}
    