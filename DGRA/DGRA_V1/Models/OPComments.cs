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
        public string month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public int type { get; set; }
        public string spv { get; set; }
        public int site_id { get; set; }
        public int BDType { get; set; }
        public int isDeleted { get; set; }
        public int isMonthly { get; set; }
        public string comment { get; set; }
        public int added_by { get;set; }
        public int added_at { get;set; }
        public int updated_by { get;set; }
        public int updated_at { get;set; }

    }
}
