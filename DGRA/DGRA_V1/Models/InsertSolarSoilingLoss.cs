using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertSolarSoilingLoss
    {
        public dynamic month { get; set; }
        public int month_no { get; set; }
        public dynamic site_name { get; set; }
        public int site_id { get; set; }
        public dynamic five_days { get; set; }
        public dynamic ten_days { get; set; }
        public dynamic fifteen_days { get; set; }
        public dynamic fifteen_days_original { get; set; }
        public int rainy_days { get; set; }
        public double sandstorm_days { get; set; }
        public double total_rain { get; set; }
        public string manual_or_SCADA { get; set; }
        public int isManual_or_SCADA { get; set; }
        public string reason { get; set; }
        public dynamic toplining_losses { get; set; }

    }
}
