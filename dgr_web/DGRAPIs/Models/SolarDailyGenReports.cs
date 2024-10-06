using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyGenReports
    {
        public int year { get; set; }
        public string month { get; set; }
        public dynamic date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string Inverter { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
        public dynamic ghi { get; set; }
        public dynamic poa { get; set; }
        public dynamic expected_kwh { get; set; }
        public dynamic inv_kwh { get; set; }
        public dynamic plant_kwh { get; set; }
        public dynamic inv_pr { get; set; }
        public dynamic plant_pr { get; set; }
        public dynamic inv_plf { get; set; }
        public dynamic plant_plf { get; set; }
        public dynamic ma_actual { get; set; }
        public dynamic ma_contractual { get; set; }
        public dynamic iga { get; set; }
        public dynamic ega { get; set; }
        public dynamic ega_b { get; set; }
        public dynamic ega_c { get; set; }
        public dynamic prod_hrs { get; set; }
        public dynamic usmh { get; set; }
        public dynamic smh { get; set; }
        public dynamic oh { get; set; }
        public dynamic igbdh { get; set; }
        public dynamic egbdh { get; set; }
        public dynamic load_shedding { get; set; }
        public dynamic lull_hrs { get; set; }
        public string tracker_losses { get; set; }
        public dynamic total_losses { get; set; }
        public dynamic lull_hrs_bd { get; set; }
        public dynamic usmh_bs { get; set; }
        public dynamic smh_bd { get; set; }
        public dynamic oh_bd { get; set; }
        public dynamic igbdh_bd { get; set; }
        public dynamic egbdh_bd { get; set; }
        public dynamic load_shedding_bd { get; set; }
        public dynamic total_bd_hrs { get; set; }
    }
    public class SolarDailyGenReports1
    {
        public int year { get; set; }
        public string month { get; set; }
        public dynamic date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string Inverter { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
        public dynamic ghi { get; set; }
        public dynamic poa { get; set; }
        public dynamic expected_kwh { get; set; }
        public dynamic inv_kwh { get; set; }
        public dynamic plant_kwh { get; set; }
        public dynamic inv_pr { get; set; }
        public dynamic plant_pr { get; set; }
        public dynamic inv_plf { get; set; }
        public dynamic plant_plf { get; set; }
        public dynamic ma_actual { get; set; }
        public dynamic ma_contractual { get; set; }
        public dynamic iga { get; set; }
        public dynamic ega { get; set; }
        public dynamic ega_b { get; set; }
        public dynamic ega_c { get; set; }
        public dynamic gen_hrs { get; set; }
        public dynamic usmh { get; set; }
        public dynamic smh { get; set; }
        public dynamic oh { get; set; }
        public dynamic igbdh { get; set; }
        public dynamic egbdh { get; set; }
        public dynamic load_shedding { get; set; }
        public dynamic lull_hrs { get; set; }
        public dynamic tracker_losses { get; set; }
        public dynamic total_losses { get; set; }
        public dynamic total_bd_hrs { get; set; }
        public dynamic lull_hrs_bd { get; set; }
        public dynamic usmh_bs { get; set; }
        public dynamic smh_bd { get; set; }
        public dynamic oh_bd { get; set; }
        public dynamic igbdh_bd { get; set; }
        public dynamic egbdh_bd { get; set; }
        public dynamic load_shedding_bd { get; set; }
       // public double total_bd_hrs { get; set; }
    }
    public class SolarDailyGenReports2
    {
        public long year { get; set; }
        public string month { get; set; }
        public dynamic date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string Inverter { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
        public dynamic ghi { get; set; }
        public dynamic poa { get; set; }
        public dynamic expected_kwh { get; set; }
        public dynamic inv_kwh { get; set; }
        public dynamic plant_kwh { get; set; }
        public dynamic inv_pr { get; set; }
        public dynamic plant_pr { get; set; }
        public dynamic inv_plf { get; set; }
        public dynamic plant_plf { get; set; }
        public dynamic ma_actual { get; set; }
        public dynamic ma_contractual { get; set; }
        public dynamic iga { get; set; }
        public dynamic ega { get; set; }
        public dynamic ega_b { get; set; }
        public dynamic ega_c { get; set; }
        public dynamic prod_hrs { get; set; }
        public dynamic usmh { get; set; }
        public dynamic smh { get; set; }
        public dynamic oh { get; set; }
        public dynamic igbdh { get; set; }
        public dynamic egbdh { get; set; }
        public dynamic load_shedding { get; set; }
        public dynamic lull_hrs { get; set; }
        public dynamic tracker_losses { get; set; }
        public dynamic total_losses { get; set; }
        public dynamic lull_hrs_bd { get; set; }
        public dynamic usmh_bs { get; set; }
        public dynamic smh_bd { get; set; }
        public dynamic oh_bd { get; set; }
        public dynamic igbdh_bd { get; set; }
        public dynamic egbdh_bd { get; set; }
        public dynamic load_shedding_bd { get; set; }
        public dynamic total_bd_hrs { get; set; }
    }
    public class SolarDailyGenReports3
    {
        public string site { get; set; }
        public dynamic date { get; set; }
        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }
        public double expected_kwh { get; set; }
        public double plant_pr { get; set; }
        public double usmh { get; set; }
        public double smh { get; set; }
        public double oh { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double targer_kwh { get; set; }
        
    }
    public class SolarDailyInvCount
    {
        public string site { get; set; }
        public Int64 inv_count { get; set; }



    }
    public class SolarDailyGenReportsGroup
    {
        public long year { get; set; }
        public string month { get; set; }
        public DateTime? date { get; set; } // Date should ideally be DateTime
        public string country { get; set; }
        public string state { get; set; }
        public string cust_group { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string Inverter { get; set; }

        // Numeric fields should be decimal? if they can contain null values
        public decimal? dc_capacity { get; set; }
        public decimal? ac_capacity { get; set; }
        public decimal? ghi { get; set; }
        public decimal? poa { get; set; }
        public decimal? expected_kwh { get; set; }
        public decimal? inv_kwh { get; set; }
        public decimal? plant_kwh { get; set; }
        public decimal? inv_pr { get; set; }
        public decimal? plant_pr { get; set; }
        public decimal? inv_plf { get; set; }
        public decimal? plant_plf { get; set; }
        public decimal? ma_actual { get; set; }
        public decimal? ma_contractual { get; set; }
        public decimal? iga { get; set; }
        public decimal? ega { get; set; }
        public decimal? ega_b { get; set; }
        public decimal? ega_c { get; set; }
        public decimal? prod_hrs { get; set; }
        public decimal? usmh { get; set; }
        public decimal? smh { get; set; }
        public decimal? oh { get; set; }
        public decimal? igbdh { get; set; }
        public decimal? egbdh { get; set; }
        public decimal? load_shedding { get; set; }
        public decimal? lull_hrs { get; set; }
        public decimal? tracker_losses { get; set; }
        public decimal? total_losses { get; set; }
        public decimal? lull_hrs_bd { get; set; }
        public decimal? usmh_bs { get; set; }
        public decimal? smh_bd { get; set; }
        public decimal? oh_bd { get; set; }
        public decimal? igbdh_bd { get; set; }
        public decimal? egbdh_bd { get; set; }
        public decimal? load_shedding_bd { get; set; }
        public decimal? total_bd_hrs { get; set; }
        public decimal? total_tarrif { get; set; }

        // If this is a list of nested SolarDailyGenReportsGroup objects
        public List<SolarDailyGenReportsGroup> item { get; set; }
    }

}
