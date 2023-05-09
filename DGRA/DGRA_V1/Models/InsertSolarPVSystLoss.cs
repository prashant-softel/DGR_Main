using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertSolarPVSystLoss
    {
        public string FY { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public double alpha { get; set; }
        public double near_shading { get; set; }
        public double IAM_factor { get; set; }
        public double soiling_factor { get; set; }
        public double pv_loss { get; set; }
        public double lid { get; set; }
        public double array_missmatch { get; set; }
        public double dc_ohmic { get; set; }
        public double conversion_loss { get; set; }
        public double plant_aux { get; set; }
        public double system_unavailability { get; set; }
        public double ac_ohmic { get; set; }
        public double external_transformer { get; set; }
        public double yoy_degradation { get; set; }
        public string module_degradation { get; set; }
        public int tstc { get; set; }
        public int tcnd { get; set; }
        public double far_shading { get; set; }
        public double pv_loss_dueto_temp { get; set; }
        public double module_quality_loss { get; set; }
        public double electrical_loss { get; set; }
        public double inv_loss_over_power { get; set; }
        public double inv_loss_max_input_current { get; set; }
        public double inv_loss_voltage { get; set; }
        public double inv_loss_power_threshold { get; set; }
        public double inv_loss_voltage_threshold { get; set; }
        public double night_consumption { get; set; }
        public double idt { get; set; }
        public double line_losses { get; set; }
        public double unused_energy { get; set; }

    }
}
