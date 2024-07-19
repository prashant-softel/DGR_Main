using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
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
        public int required { get; set; }
        public int selected { get; set; }

    }
    public class CreateGroupData
    {
        public int column_id { get; set; }
        public int page_id { get; set; }
    }
    public class page_group_elements
    {
        public int pge_id { get; set; }
        public int page_groups_id { get; set; }
        public int column_id { get; set; }
        public string page_group_name { get; set; }
    }
    public class user_page_group_ca
    {
        public int upgca_id { get; set; }
        public int user_id { get; set; }
        public int page_id { get; set; }
        public int page_groups_id { get; set; }
    }
    public class pageGroupdata
    {
        public int page_id { get; set; }
        public int isGroupAvailable { get; set; }
        public List<page_group_elements> pageGroupData { get; set; }
    }
    public class pageColumns
    {
        public int page_id { get; set; }
        public int column_id { get; set; }
        public string column_name { get; set; }
        public int required { get; set; }
        public int page_groups_id { get; set; }
    }

    public class groupsData
    {
        public int page_groups_id { get; set; }
        public string page_group_name { get; set; }
        public int page_id { get; set; }
        public int is_active { get; set; }
        public List<ColumnData> column_data { get; set; }
    }

}
