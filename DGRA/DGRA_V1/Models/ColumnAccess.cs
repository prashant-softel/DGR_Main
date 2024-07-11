using System;
using DGRA_V1.Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class ColumnAccess
    {

    }
    public class PageData
    {
        public int page_id { get; set; }
        public string page_name { get; set; }
        public int type { get; set; }
        public int page_type { get; set; }

    }
    public class GroupData
    {
        public int page_groups_id { get; set; }
        public string page_group_name { get; set; }
        public int page_id { get; set; }
        public int is_active { get; set; }
        public int created_by { get; set; }

    }
    public class ColumnData
    {
        public int column_id { get; set; }
        public string column_name { get; set; }

    }

    public class CreateGroupData
    {
        public int column_id { get; set; }
        public int page_id { get; set; }
    }

}
