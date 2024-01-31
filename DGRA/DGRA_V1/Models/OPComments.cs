using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class OPComments
    {
        //opc_id, month, month_no, year, type, spv, site_id, site, bd_type, isDeleted, isMonthly, isSPV, comment, added_at, added_by, updated_by, updated_at, updated_comment
        public int opc_id { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public int type { get; set; }
        public string spv { get; set; }
        public int site_id { get; set; }
        public string site { get; set; }
        //public int BDType { get; set; }
        public int bd_type { get; set; }
        public int isDeleted { get; set; }
        public int isMonthly { get; set; }
        public int isSPV { get; set; }
        public string comment { get; set; }
        public int added_by { get; set; }
        public int added_at { get; set; }
        public int updated_by { get; set; }
        public int updated_at { get; set; }
        public string updated_comment {get;set;}
        public int deleted_by { get; set; }
        public string deleted_at { get; set; }

    }
}
