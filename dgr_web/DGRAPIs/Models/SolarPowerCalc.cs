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
        public double Near_shading { get; set; }

        public double IAM { get; set; }
        public double Soiling { get; set; }
        public double PV { get; set; }
        public double LID { get; set; }
        public double Array_mismatch { get; set; }
        public double DC_Ohmic { get; set; }
        public double Conversion { get; set; }
        public double Plant_aux { get; set; }
        public double Sys_unavailability { get; set; }
        public double AC_Ohmic { get; set; }
        public double Ext_transformer { get; set; }

        public double Ageing_factor { get; set; }
        public double alpha { get; set; }
        //For POA
        public double T_cnd { get; set; }
        public double T_stc { get; set; }

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
    