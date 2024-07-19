using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class GetFormulas
    {
        public int id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String formulas { get; set; }

    }
    public class GetFormulas1
    {
        public int id { get; set; }
        public int site_id { get; set; }
        public String site_type { get; set; }
        public String MA_Actual { get; set; }
        public String MA_Contractual { get; set; }
        public String IGA { get; set; }
        public String EGA { get; set; }

    }

    public class GetFormulaLog
    {
        public int site_id { get; set; }
        public String username { get; set; }
        public String formulas { get; set; }
        public String formula_name { get; set; }
        
        public dynamic change_by_date { get; set; }

    }
}
