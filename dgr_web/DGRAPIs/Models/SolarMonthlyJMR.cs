﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarMonthlyJMR
    {
        public int monthly_jmr_solar_id { get; set; }
        public string FY { get; set; }
        public string Site { get; set; }
        public int site_id { get; set; }
        public string Plant_Section { get; set; }
        public dynamic Controller_KWH_INV { get; set; }
        public dynamic Scheduled_Units_kWh { get; set; }
        public dynamic Export_kWh { get; set; }
        public dynamic Import_kWh { get; set; }
        public dynamic Net_Export_kWh { get; set; }
        public dynamic Net_Billable_kWh { get; set; }
        public dynamic Export_kVAh { get; set; }
        public dynamic Import_kVAh { get; set; }
        public dynamic Export_kVArh_lag { get; set; }
        public dynamic Import_kVArh_lag { get; set; }
        public dynamic Export_kVArh_lead { get; set; }
        public dynamic Import_kVArh_lead { get; set; }
        public dynamic JMR_date { get; set; }
        public dynamic JMR_Month { get; set; }
        public int JMR_Month_no { get; set; }
        public dynamic JMR_Year { get; set; }
        public dynamic LineLoss { get; set; }
        public dynamic Line_Loss_percentage { get; set; }
        public dynamic RKVH_percentage { get; set; }
        public string remarks { get; set; }
    }
}
