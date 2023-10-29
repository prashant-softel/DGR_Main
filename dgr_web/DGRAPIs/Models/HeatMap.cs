using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class HeatMapData
    {
        public int upload_status_id { get; set; }
        public int type { get; set; }
        public int site_id { get; set; }
        public dynamic import_date { get; set; }
        public dynamic data_date { get; set; }
        public int approved_by { get; set; }
        public int uploaded_by { get; set; }
        public int import_batch_id { get; set; }
        public int TML_uploaded { get; set; }
        public int automation { get; set; }
        public int approve_count { get; set; }
        public int expected_TML { get; set; }
        public int actual_TML { get; set; }
        public int wtg_count { get; set; }
        public int created_on { get; set; }
    }
}
