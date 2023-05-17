using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class InsertWindBDCodesGamesa
    {
        public int wind_bd_codes_gamesa_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public int codes { get; set; }
        public string description { get; set; }
        public string conditions { get; set; }
    }

    public class ImportWindBDCodeINOX
    {
        public int wind_bd_codes_inox_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string plc_state { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }
    public class InsertWindBDCodeREGEN
    {
        public int wind_bd_codes_regen_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public int code { get; set; }
        public string operation_mode { get; set; }
        public string conditions { get; set; }
    }
}
