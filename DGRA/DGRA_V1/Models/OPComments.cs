using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class OPComments
    {
        //month, month_no, year, type, spv, site_id, BD_type, isDeleted, isMonthly, comment, added_by, updated_by
        public int opc_id { get; set; }
        public dynamic month { get; set; }
        public dynamic month_no { get; set; }
        public dynamic year { get; set; }
        public dynamic type { get; set; }
        public dynamic spv { get; set; }
        public dynamic site_id { get; set; }
        public dynamic BDType { get; set; }
        public dynamic isDeleted { get; set; }
        public dynamic isMonthly { get; set; }
        public dynamic comment { get; set; }
        public dynamic added_by { get;set; }
        public dynamic added_at { get;set; }
        public dynamic updated_by { get;set; }
        public dynamic updated_at { get;set; }

    }
}
