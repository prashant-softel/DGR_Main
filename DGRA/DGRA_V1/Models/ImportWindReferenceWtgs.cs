using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class ImportWindReferenceWtgs
    {
        public int tml_reference_wtgs_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public dynamic created_on { get; set; }
    }
}
