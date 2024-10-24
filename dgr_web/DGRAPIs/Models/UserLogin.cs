﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class UserLogin
    {
        public int login_id { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string password { get; set; }
        public string user_role { get; set; }
        public DateTime created_on { get; set; }
        public string auth_session { get; set; }
        public DateTime last_accessed { get; set; }
        public int active_user { get; set; }
        public bool islogin { get; set; }
        public string device_id { get; set; }
    }
    public class UserInfomation
    {
        public int login_id { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string user_role { get; set; }
        public int To_Daily_Wind { get; set; }
        public int Cc_Daily_Wind { get; set; }
        public int To_Weekly_Wind { get; set; }
        public int Cc_Weekly_Wind { get; set; }
        public int To_Daily_Solar { get; set; }
        public int Cc_Daily_Solar { get; set; }
        public int To_Weekly_Solar { get; set; }
        public int Cc_Weekly_Solar { get; set; }
        // public DateTime created_on { get; set; }
        public int active_user { get; set; }
        public string device_id { get; set; }

    }
    public class HFEPage
    {
        public int Id { get; set; }
        public string Display_name { get; set; }
        public string Action_url { get; set; }
        public string Controller_name { get; set; }
        public int Page_type { get; set; }
        public int Order_no { get; set; }
        public int Visible { get; set; }
        public int isGroupAvailable { get; set; }
        public List<page_group_elements> pageGroupData { get; set; }

    }
    public class HFEPage1
    {
       
        public int login_id { get; set; }
        public int page_type { get; set; }
        public int identity { get; set; }
        public int upload_access { get; set; }
        public string Display_name { get; set; }
        public string Action_url { get; set; }
        public string Controller_name { get; set; }
    }
    public class CustomGroup
    {
        public int Id { get; set; }
        public int cust_group { get; set; }
    }
    public class CustomGroupAccess
    {
        public int cust_group { get; set; }
    }
}
