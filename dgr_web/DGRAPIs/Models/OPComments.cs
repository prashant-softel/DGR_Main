using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    //public class OPComments
    //{
    //    public int opc_id { get; set; }
    //    public string month { get; set; }
    //    public int month_no { get; set; }
    //    public int year { get; set; }
    //    public int type { get; set; }
    //    public int site_id { get; set; }
    //    public string site { get; set; }
    //    public int spv_id { get; set; }
    //    public string spv { get; set; }
    //    public int BD_type { get; set; }
    //    public int isDeleted { get; set; }
    //    public int isMonthly { get; set; }
    //    public string comment { get; set; }
    //    public int added_by { get;set; }
    //    public int added_at { get;set; }
    //    public int updated_by { get;set; }
    //    public int updated_at { get;set; }

    //}
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
