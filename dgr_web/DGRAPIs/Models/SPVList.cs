
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SPVList
    {
        public string spv { get; set; }
       
    }
    public class SPVSiteWind
    {
        public int site_master_id {get; set;}
        public string spv { get; set; }
        public string site { get; set; }
    }
}
