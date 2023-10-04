
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class ImportBatch
    {
        public string importFilePath { get; set; }
        public int importSiteId { get; set; }
        public string importType { get; set; }
        public string importLogName { get; set; }
        public int importFileType { get; set; }
        public string automationDataDate { get; set; }
    }

    public class BatchIdImport
    { 
        public int import_batch_id{ get; set; }
    }

    public class ImportBatchStatus
    {
        public int import_batch_id { get; set; }
        public int is_approved { get; set; }
    }

    public class ImportBatchesForUploadStatus
    {
        public int import_batch_id { get; set; } //batch_id generated in this table.
        public int import_type { get; set; } //Solar =1, wind = 2
        public int site_id { get; set; }
        public dynamic import_date { get; set; }
        public dynamic data_date { get; set; }
        public int imported_by { get; set; } //login table id i.e. userid
        public int approved_by { get; set; } //user id of admin
        public int is_approved { get; set; } // 0 waiting, 1 approved, 2 rejected

    }

    public class site_id_date
    {
        public dynamic date { get; set; }
        public int site_id { get; set; }
    }
}
