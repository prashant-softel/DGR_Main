﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDailyGenReports
    {
        public long year { get; set; }
        public string month { get; set; }
        public DateTime date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public double capacity_mw { get; set; }
        public string wtg { get; set; }
        public double wind_speed { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }

        public double grid_hrs { get; set; }
        public double lull_hrs { get; set; }
        public double production_hrs { get; set; }
        public dynamic unschedule_hrs { get; set; }
        public double unschedule_num { get; set; }
        public dynamic schedule_hrs { get; set; }
        public double schedule_num { get; set; }
        public dynamic others { get; set; }
        public double others_num { get; set; }
        public dynamic igbdh { get; set; }
        public double igbdh_num { get; set; }
        public dynamic egbdh { get; set; }
        public double egbdh_num { get; set; }
        public dynamic load_shedding { get; set; }
        public double load_shedding_num { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbdh_loss {get;set;}
        public double egbdh_loss { get; set; }
        public double loadShedding_loss { get; set; }
        public double total_loss { get; set; }
        public double total_hrs { get; set; }
    }
    public class WindDailyGenReports1
    {
        public long year { get; set; }
        public string month { get; set; }
        public DateTime date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public double capacity_mw { get; set; }
        public double total_mw { get; set; }
        public string wtg { get; set; }
        public double wind_speed { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }
        public double grid_hrs { get; set; }
        public double lull_hrs { get; set; }
        public dynamic unschedule_hrs { get; set; }
        public double unschedule_num { get; set; }
        public dynamic schedule_hrs { get; set; }
        public double schedule_num { get; set; }
        public dynamic others { get; set; }
        public double others_num { get; set; }
        public dynamic igbdh { get; set; }
        public double igbdh_num { get; set; }
        public dynamic egbdh { get; set; }
        public double egbdh_num { get; set; }
        public dynamic load_shedding { get; set; }
        public double load_shedding_num { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbdh_loss { get; set; }
        public double egbdh_loss { get; set; }
        public double loadShedding_loss { get; set; }
        public double total_loss { get; set; }
        public double total_hrs { get; set; }
    }
    public class WindDailyGenReports2
    {
        public long year { get; set; }
        public string month { get; set; }
        public DateTime date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public double capacity_mw { get; set; }
        public double total_mw { get; set; }
        public string wtg { get; set; }
        public double wind_speed { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }
        public double grid_hrs { get; set; }
        public double lull_hrs { get; set; }
        public dynamic unschedule_hrs { get; set; }
        public double unschedule_num { get; set; }
        public dynamic schedule_hrs { get; set; }
        public double schedule_num { get; set; }
        public dynamic others { get; set; }
        public double others_num { get; set; }
        public dynamic igbdh { get; set; }
        public double igbdh_num { get; set; }
        public dynamic egbdh { get; set; }
        public double egbdh_num { get; set; }
        public dynamic load_shedding { get; set; }
        public double load_shedding_num { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbdh_loss { get; set; }
        public double egbdh_loss { get; set; }
        public double loadShedding_loss { get; set; }
        public double total_loss { get; set; }
        public double total_hrs { get; set; }
    }
    public class WindDailyGenReportsGroup
    {
        public long year { get; set; }
        public string month { get; set; }
        public DateTime date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string cust_group { get; set; }
        public double capacity_mw { get; set; }
        public double total_mw { get; set; }
        public string wtg { get; set; }
        public double wind_speed { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double ega_b { get; set; }
        public double ega_c { get; set; }
        public double grid_hrs { get; set; }
        public double lull_hrs { get; set; }
        public double unschedule_hrs { get; set; }
        public double unschedule_num { get; set; }
        public double schedule_hrs { get; set; }
        public double schedule_num { get; set; }
        public double others { get; set; }
        public double others_num { get; set; }
        public double igbdh { get; set; }
        public double igbdh_num { get; set; }
        public double egbdh { get; set; }
        public double egbdh_num { get; set; }
        public double load_shedding { get; set; }
        public double load_shedding_num { get; set; }
        public double usmh_loss { get; set; }
        public double smh_loss { get; set; }
        public double others_loss { get; set; }
        public double igbdh_loss { get; set; }
        public double egbdh_loss { get; set; }
        public double loadShedding_loss { get; set; }
        public double total_loss { get; set; }
        public double total_hrs { get; set; }
        public List<WindDailyGenReportsGroup> item { get; set; }
    }
}
