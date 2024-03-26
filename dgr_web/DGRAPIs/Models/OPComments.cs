using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
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
        public int bd_type { get; set; }
        public int isDeleted { get; set; }
        public int isMonthly { get; set; }
        public int isSPV { get; set; }
        public string comment { get; set; }
        public string updated_comment { get; set; }
        public int added_by { get; set; }
        public int added_at { get; set; }
        public int updated_by { get; set; }
        public int updated_at { get; set; }
        public int deleted_by { get; set; }
        public string deleted_at { get; set; }

    }
    public class OPCFetch
    {
        //month, month_no, year, type, spv, site_id, BD_type, isDeleted, isMonthly, comment, added_by, updated_by
        public string opc_id { get; set; }
        public string month { get; set; }
        public string month_no { get; set; }
        public string year { get; set; }
        public string type { get; set; }
        public string spv { get; set; }
        public string site_id { get; set; }
        public string site { get; set; }
        public string bd_type { get; set; }
        public string isDeleted { get; set; }
        public string isMonthly { get; set; }
        public string isSPV { get; set; }
        public string comment { get; set; }
        public string updated_comment { get; set; }
        public string added_by { get; set; }
        public string added_at { get; set; }
        public string updated_by { get; set; }
        public string updated_at { get; set; }

    }
    public class OPSPV
    {
        public int spv_id { get; set; }
        public string spv { get; set; }
    }
    public class OPSite
    {
        public int site_id { get; set; }
        public string site { get; set; }
        public int bdTypes { get; set; }
    }
}
