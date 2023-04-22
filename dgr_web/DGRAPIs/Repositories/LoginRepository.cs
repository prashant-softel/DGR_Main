using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Models;
using Nancy.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace DGRAPIs.Repositories
{
    public class LoginRepository : GenericRepository
    {
        private int approve_status = 0;
        private object json;

        public LoginRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {

        }
        //Login 

        //internal async Task<List<UserLogin>> GetUserLogin(string username, string password)
        internal async Task<UserLogin> GetUserLogin(string username, string password, bool isSSO, string device_id)
        {
            string qry = "";
            /*//qry = "SELECT * FROM `login` where `username`='" + username + "' and `password` ='" + password + "' and `active_user` = 1 ;";
            qry = "SELECT login_id,username,useremail,user_role FROM `login` where `useremail`='" + username + "' and `password` ='" + password + "' and `active_user` = 1 ;";
            List<UserLogin> _UserLogin = new List<UserLogin>();
            _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            if(_UserLogin.Count > 0)
            {
                string qry1 = "update login set last_accessed=NOW() where login_id=" + _UserLogin[0].login_id + ";";
                await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            }
            return _UserLogin;
           */
            if (isSSO)
            {
                qry = "SELECT login_id,username,useremail,user_role,islogin as islogin, device_id FROM `login` where `useremail`='" + username + "'  and `active_user` = 1 ;";
               
            }
            else {
                qry = "SELECT login_id,username,useremail,user_role,islogin as islogin, device_id FROM `login` where `useremail`='" + username + "' and (`password` = md5('" + password + "') or password = '"+ password +"') and `active_user` = 1 ;";
                //qry = "SELECT login_id,username,useremail,user_role,islogin as islogin FROM `login` where `useremail`='" + username + "' and `password` = md5('" + password + "') and `active_user` = 1 ;";

            }
            try
            {
                var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
                if (_UserLogin.Count > 0)
                {
                    string qry1 = "update login set last_accessed=NOW(),islogin=1, device_id = '" + device_id + "' where login_id=" + _UserLogin[0].login_id + ";";
                    await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
                }
                return _UserLogin.FirstOrDefault();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                var _UserLogin1 = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
                return _UserLogin1.FirstOrDefault();
            }
            
            

        }

        internal async Task<UserLogin> GetUserLoginFromDeviceId(string device_id)
        {
            string qry = "";
            API_InformationLog("Inside GetUserLoginFromDeviceID function : Device Id is :-"+ device_id);


            {
                //SELECT * FROM `login` where `device_id`=1494303526 AND `islogin` = 1 AND last_accessed > date_add(now(),interval -30 minute);
                qry = "SELECT login_id, username, password, useremail,last_accessed, user_role, islogin as islogin, device_id FROM `login` where `device_id`='" + device_id + "' AND `islogin` = 1 AND last_accessed > date_add(now(),interval -30 minute) ;";
            }
            var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            if (_UserLogin.Count > 0)
            {
                string qry1 = "update login set islogin= 0 WHERE login_id = " + _UserLogin[0].login_id + " ;";
                try
                {                    
                    await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
                    //await GetUserLogin(_UserLogin[0].useremail, _UserLogin[0].password, false);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                }
            }
            return _UserLogin.FirstOrDefault();

        }
        //UpdateLoginLog
        internal async Task<int> UpdateLoginLog(int UserID, string userRole)
        {
            
            string InsertQry = "";
            if (userRole == "login")
            {
                string selectQry = "SELECT login_id,username,useremail,user_role,islogin as islogin, device_id FROM `login` where `login_id`=" + UserID + " AND `active_user` = 1 AND islogin = 1 ;";

                var _UserLogin = await Context.GetData<UserLogin>(selectQry).ConfigureAwait(false);

                InsertQry = "INSERT INTO login_log (user_id, user_name, user_role, login_time, device_id, user_email, created_on) VALUES (" + UserID + ", '" + _UserLogin[0].username + "', '" + _UserLogin[0].user_role + "', NOW(), '" + _UserLogin[0].device_id + "', '" + _UserLogin[0].useremail + "', NOW() );";
            }
            if(userRole == "logout")
            {
                string selectQry = "SELECT login_id,username,useremail,user_role,islogin as islogin, device_id FROM `login` where `login_id`=" + UserID + " AND `active_user` = 1 ;";

                var _UserLogin = await Context.GetData<UserLogin>(selectQry).ConfigureAwait(false);

                InsertQry = "INSERT INTO login_log (user_id, user_name, user_role, logout_time, device_id, user_email, created_on) VALUES (" + UserID + ", '" + _UserLogin[0].username + "', '" + _UserLogin[0].user_role + "', NOW(), '" + _UserLogin[0].device_id + "', '" + _UserLogin[0].useremail + "', NOW() );";
            }
            return await Context.ExecuteNonQry<int>(InsertQry).ConfigureAwait(false);

        }
        internal async Task<int> UpdateLoginStatus(int UserID)
        {
            string qry1 = "Update login set islogin=0 where last_accessed<  date_add(now(),interval -10 minute); update login set last_accessed=NOW(),islogin=1 where login_id=" + UserID + ";";
            return await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);

        }
        internal async Task<int> DirectLogOut(int UserID)
        {
            //Maybe device id should be 0 
            string qry1 = "Update login set islogin=0 where  login_id=" + UserID + ";";
            return await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);

        }
        internal async Task<int> WindUserRegistration(string fname, string useremail, string role, string userpass)
        {
            string qry1 = "SELECT useremail FROM login WHERE useremail = '"+useremail+"'";
            List<UserLogin> email = new List<UserLogin>();
            email = await Context.GetData<UserLogin>(qry1).ConfigureAwait(false);
            if (email.Capacity > 0)
            {
                return -1;
            }
            else
            {
                string qry = "insert into login (`username`,`useremail`,`user_role`,`password`) VALUES('" + fname + "','" + useremail + "','" + role + "', MD5('" + userpass + "'))";
                //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
                return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
            }
            
        }

        internal async Task<int> EmailReportTimeChangeSetting(string dailytime, string windweeklytime, string solarweeklytime, string windWeekDay, string solarWeekDay, string username, int user_id, string role)
        {
            
            int finalRes = 1;
            int insertTimeDataRes = 0;
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configurationBuilder.Build();
            var json = File.ReadAllText("appsettings.json");
            var jsonObject = JObject.Parse(json);
            var updatedJson = "";
                try
                {
                MyConfig["Timer:DailyReportTime"] = dailytime;
                jsonObject["Timer"]["DailyReportTime"] = dailytime;
                updatedJson = jsonObject.ToString();
                File.WriteAllText("appsettings.json", updatedJson);

                try
                {
                    MyConfig["Timer:WeeklyReportTime"] = windweeklytime;
                    jsonObject["Timer"]["WeeklyReportTime"] = windweeklytime;
                    updatedJson = jsonObject.ToString();
                    File.WriteAllText("appsettings.json", updatedJson);
                }
                catch (Exception e)
                {
                    string msg = e.ToString();
                    API_ErrorLog("Exception while changing Wind weekly email report time, due to : " + msg);
                    finalRes = 0;
                    return finalRes;
                }
                try
                {
                    MyConfig["Timer:WeeklyReportTimeSolar"] = solarweeklytime;
                    jsonObject["Timer"]["WeeklyReportTimeSolar"] = solarweeklytime;
                    updatedJson = jsonObject.ToString();
                    File.WriteAllText("appsettings.json", updatedJson);
                }
                catch (Exception e)
                {
                    string msg = e.ToString();
                    API_ErrorLog("Exception while changing Solar weekly email report time, due to : " + msg);
                    finalRes = 0;
                    return finalRes;
                }
                try
                {
                    MyConfig["Timer:WeeklyReportDayOfWeek"] = solarweeklytime;
                    jsonObject["Timer"]["WeeklyReportDayOfWeek"] = windWeekDay;
                    updatedJson = jsonObject.ToString();
                    File.WriteAllText("appsettings.json", updatedJson);
                }
                catch (Exception e)
                {
                    string msg = e.ToString();
                    API_ErrorLog("Exception while changing Wind weekly email report Day, due to : " + msg);
                    finalRes = 0;
                    return finalRes;
                }
                try
                {
                    MyConfig["Timer:WeeklyReportDayOfWeekSolar"] = solarweeklytime;
                    jsonObject["Timer"]["WeeklyReportDayOfWeekSolar"] = solarWeekDay;
                    updatedJson = jsonObject.ToString();
                    File.WriteAllText("appsettings.json", updatedJson);
                }
                catch (Exception e)
                {
                    string msg = e.ToString();
                    API_ErrorLog("Exception while changing Solar weekly email report Day, due to : " + msg);
                    finalRes = 0;
                    return finalRes;
                }

            }
            catch(Exception e)
                {
                    string msg = e.ToString();
                    finalRes = 0;
                    return finalRes;
            }
            if(finalRes == 1)
            {
                try
                {
                    string insertTimingsDataQry = "INSERT INTO email_report_timings_log (daily_report, wind_weekly, solar_weekly, wind_weekly_day, solar_weekly_day, updated_by_name, updated_by_id, updated_by_role) VALUES ( '" + dailytime + "', '" + windweeklytime + "', '" + solarweeklytime + "', '" + windWeekDay + "', '" + solarWeekDay + "', '" + username + "', " + user_id + ", '" + role + "' );" ;
                    insertTimeDataRes = await Context.ExecuteNonQry<int>(insertTimingsDataQry).ConfigureAwait(false);

                }
                catch(Exception e)
                {
                    string msg = e.ToString();
                    API_ErrorLog("Exception while inserting data into email_report_timings_log table : due to " + msg);
                    finalRes = 0;
                }
            }               

            return finalRes;

        }
        //GetEmailTime
        internal async Task<List<EmailReportTimingsLog>> EmailReportTimings()
        {
            /*
            ConfigurationManager.RefreshSection("appSettings");
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            List<EmailReportTimings> EmailReportTimeList = new List<EmailReportTimings>();
            string dailyTime = MyConfig.GetValue<string>("Timer:DailyReportTime");
            string windWeeklyTime = MyConfig.GetValue<string>("Timer:WeeklyReportTime");
            string solarWeeklyTime = MyConfig.GetValue<string>("Timer:WeeklyReportTimeSolar");
            string windWeekDay = MyConfig.GetValue<string>("Timer:WeeklyReportDayOfWeek");
            string solarWeekDay = MyConfig.GetValue<string>("Timer:WeeklyReportDayOfWeekSolar");            
            EmailReportTimings emailReportTimings = new EmailReportTimings()
            {
                dailyTime = dailyTime,
                windWeeklyTimw = windWeeklyTime,
                solarWeeklyTime = solarWeeklyTime,
                windWeekDay = windWeekDay,
                solarWeekDay = solarWeekDay
            };
            EmailReportTimeList.Add(emailReportTimings);
            */

            List<EmailReportTimingsLog> TimeRecordList = new List<EmailReportTimingsLog>();
            string fetchTimerecordQry = "SELECT * FROM email_report_timings_log ORDER BY email_report_timings_log_id DESC LIMIT 1;";
            try
            {
                TimeRecordList = await Context.GetData<EmailReportTimingsLog>(fetchTimerecordQry).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                string msg = e.ToString();
                API_ErrorLog("Error while fetching records from email_report_timings_log table. due to : " + msg );
            }
            return TimeRecordList;
        }
        internal async Task<int> UpdatePassword(int loginid, string updatepass)
        {
            string qry = "update login set password=MD5('" + updatepass + "') where login_id=" + loginid + "";
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }

        internal async Task<int> DeactivateUser(int loginid)
        {
            //UPDATE login SET active_user = 0 WHERE login_id = 3;
            string qry = "update login set active_user= 0 where login_id=" + loginid + "";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            // string a = qry;
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }

        //ActivateUser
        internal async Task<int> ActivateUser(int loginid)
        {
            //UPDATE login SET active_user = 1 WHERE login_id = 3;
            string qry = "update login set active_user= 1 where login_id=" + loginid + "";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            // string a = qry;
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }
        //DeleteUser
        internal async Task<int> DeleteUser(int loginid)
        {
            //DELETE FROM `login` WHERE login_id = 3 ;
            string qry = "delete from login where login_id=" + loginid + "";
            string qry1 = "delete from user_access where login_id=" + loginid + "";
            await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }

        //GetUserLoginId
        internal async Task<List<UserInfomation>> GetUserLoginId(string username, string useremail)
        {
            string qry = "select login_id, username,useremail,user_role,active_user from login where username='" + username + "' AND useremail= '" + useremail + "';";
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        //Clone User access
        internal async Task<int> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access)
        {
            string qry = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES (" +login_id +"," + site_type + "," + page_type + "," + identity + "," + upload_access + ");";
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }
        public async Task<List<UserInfomation>> GetWindUserInformation(int  login_id)
        {
            string filter = "";
            if (login_id != 0)
            {
                filter = " where login_id='"+ login_id + "'";
            }
            string qry = "";
           // qry = "SELECT login_id,username,useremail,user_role,created_on,active_user FROM `login` " + filter;
            qry = "SELECT login_id,username,useremail,user_role,active_user FROM `login` " + filter;
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        public async Task<List<UserInfomation>> GetSolarUserInformation(int login_id)
        {
            string filter = "";
            if (login_id != 0)
            {
                filter = " where login_id=" + login_id;
            }
            string qry = "";
            // qry = "SELECT login_id,username,useremail,user_role,created_on,active_user FROM `login` " + filter;
            qry = "SELECT login_id, username,useremail,user_role,active_user  FROM `login` " + filter;
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        public async Task<List<HFEPage>> GetPageList(int login_id, int site_type)
        {
            /* string filter = "";
             if (login_id != 0)
             {
                 filter = " where login_id='" + login_id + "'";
             }*/
            string qry = "";

           //qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2";
            if(site_type == 2)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2 or site_type = 0";
            }
            if (site_type == 1)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=1 or site_type = 0";
            }
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<HFEPage> _pagelist = new List<HFEPage>();
            _pagelist = await Context.GetData<HFEPage>(qry).ConfigureAwait(false);
            return _pagelist;


        }
        public async Task<List<HFEPage>> GetEmailList(int login_id, int site_type)
        {
            /* string filter = "";
             if (login_id != 0)
             {
                 filter = " where login_id='" + login_id + "'";
             }*/
            string qry = "";

            //qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2";
            if (site_type == 2)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2 or site_type = 0";
            }
            if (site_type == 1)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=1 or site_type = 0";
            }
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<HFEPage> _pagelist = new List<HFEPage>();
            _pagelist = await Context.GetData<HFEPage>(qry).ConfigureAwait(false);
            return _pagelist;


        }
        public async Task<List<UserAccess>> GetWindUserAccess(int login_id,string role)
        {
            
            string qry = "";
            if (role == "Admin")
            {
                qry = "SELECT Site_Type as site_type,Page_type as page_type,display_name,Action_url,Controller_name FROM `hfe_pages` where Visible=1 order by Order_no";
            }
            else
            {
                qry = "SELECT t1.login_id,t1.site_type,if(t3.Page_type IS NOT NULL,t3.Page_type,3) as page_type,t1.identity,t1.upload_access,t3.display_name,t3.Action_url,t3.Controller_name FROM `user_access`as t1 left join hfe_pages as t3 on t3.Id=t1.identity and t1.category_id NOT IN(3) where t1.login_id='" + login_id + "'";
            }
            List<UserAccess> _accesslist = new List<UserAccess>();
            _accesslist = await Context.GetData<UserAccess>(qry).ConfigureAwait(false);
            return _accesslist;


        }
        public async Task<List<UserInfomation>> GetEmailAccess(int login_id, int site ,int action, string notifications)
        {

            string qry = "";
            string delAccess = "";
            string setAccess = "";
            //var foos = "Foo1,Foo2,Foo3";
           // var fooArray = foos.Split(',');  // now you have an array of 3 strings
           // foos = String.Join(",", fooArray);

            if (action == 1)
            {
                if (site == 1)
                {
                    qry = "SELECT login_id ,To_Daily_Wind, Cc_Daily_Wind, To_Weekly_Wind, Cc_Weekly_Wind FROM `login` where login_id = '" + login_id + "'";
                }
                else
                {
                    qry = "SELECT login_id ,To_Daily_Solar, Cc_Daily_Solar, To_Weekly_Solar, Cc_Weekly_Solar FROM `login` where login_id = '" + login_id + "'";

                }
            }
            else if(action == 2)
            {   //var str = 
                var noti = Regex.Replace(notifications, @"[^0-9a-zA-Z:,]+", " ");
                var notArray = noti.Split(',');
                //var notificationList = new JavaScriptSerializer().Deserialize<dynamic>(notifications);
                if (site == 1) {

                    delAccess = "UPDATE login SET To_Daily_Wind=0, Cc_Daily_Wind=0, To_Weekly_Wind=0, Cc_Weekly_Wind=0 where login_id = '" + login_id + "'";
                    await Context.ExecuteNonQry<int>(delAccess).ConfigureAwait(false);

                    foreach(var notification in notArray)
                    {
                        var col = Regex.Replace(notification, " ", "_");
                        var col1 = col.Substring(1, col.Length - 2);
                        setAccess = "UPDATE login SET "+ col1 +" = 1 where login_id = '" + login_id + "'";
                        await Context.ExecuteNonQry<int>(setAccess).ConfigureAwait(false);
                    }
                    //setAccess = "UPDATE login SET To_Daily_Wind = 0, Cc_Daily_Wind=0, To_Weekly_Wind=0, Cc_Weekly_Wind=0 where login_id = '" + login_id + "'";
                    //await Context.ExecuteNonQry<int>(setAccess).ConfigureAwait(false);
                }
                else if(site == 2)
                {
            
                    delAccess = "UPDATE login SET To_Daily_Wind=0, Cc_Daily_Wind=0, To_Weekly_Wind=0, Cc_Weekly_Wind=0 where login_id = '" + login_id + "'";
                    await Context.ExecuteNonQry<int>(delAccess).ConfigureAwait(false);

                    foreach (var notification in notArray)
                    {
                        var col = Regex.Replace(notification, " ", "_");
                        var col1 = col.Substring(1, col.Length - 2);
                        setAccess = "UPDATE login SET " + col1 + " = 1 where login_id = '" + login_id + "'";
                        await Context.ExecuteNonQry<int>(setAccess).ConfigureAwait(false);
                    }
                }


            }
            List<UserInfomation> _accesslist = new List<UserInfomation>();
            _accesslist = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _accesslist;


        }
        internal async Task<int> SubmitUserAccess(int login_id, string siteList, string pageList, string reportList, string site_type,int importapproval)
        {
            var SiteList = new JavaScriptSerializer().Deserialize<dynamic>(siteList);
            var PageList = new JavaScriptSerializer().Deserialize<dynamic>(pageList);
            var ReportList = new JavaScriptSerializer().Deserialize<dynamic>(reportList);
            int flag = 0;
            if (string.IsNullOrEmpty(site_type)) site_type = "1";
            string delAccess ="DELETE FROM `user_access` WHERE login_id  = '" + login_id + "' AND `site_type` = '"+ site_type + "'";
            await Context.ExecuteNonQry<int>(delAccess).ConfigureAwait(false);
           
            
            string qry= "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
            string pagevalues = "";
            foreach (var page in PageList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = page.Key;
                var b = page.Value;
                if(page.Value == true)
                {
                    string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and site_type = " + site_type + " and " +
                        "category_id = 1 and identity = " + page.Key;
                    List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false); 
                    if (_accesslist.Capacity > 0) continue;
                    //string qry = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
                    flag = 1;
                    pagevalues += "('" + login_id + "',"+site_type+",'1','" + page.Key + "'),";
                }
            }
            if (flag == 1)
            {

                qry += pagevalues;
                await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            string qry1 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
            string reportvalues = "";
            int flag2 = 0; 
            foreach (var report in ReportList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = report.Key;
                var b = report.Value;
                if (report.Value == true)
                {
                    string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and site_type = " + site_type + " and " +
                        "category_id = 2 and identity = " + report.Key;
                    List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false);
                    if (_accesslist.Capacity > 0) continue;
                    //string qry1 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
                    flag2 = 1;
                    reportvalues += "('" + login_id + "',"+site_type+",'2','" + report.Key + "'),";
                }
            }
            if (flag2 == 1)
            {

                qry1 += reportvalues;
                await Context.ExecuteNonQry<int>(qry1.Substring(0, (qry1.Length - 1)) + ";").ConfigureAwait(false);
            }

            string qry2 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES";
            string sitevalues = "";
            bool flag3 = false;
            foreach (var site in SiteList)
            {

                int upload_access = 0;
                if (site.Value == true)
                {
                    upload_access = 1;
                }
                if (site.Key == "") continue;
                string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and " +
                        "category_id = 3 and identity = " + site.Key ;
                List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false);
                if (_accesslist.Capacity > 0) 
                {
                    string qry3 = "update`user_access` set `upload_access` = " + upload_access + " where login_id = " + login_id + " and indentity =" +
                        site.Key;
                }
                else
                {
                    flag3 = true;
                    sitevalues += "('" + login_id + "'," + site_type + ",3,'" + site.Key + "','" + upload_access + "'),";
                }
            }
            if(site_type != "1" || site_type != "2")
            {
                string delAccess1 = "DELETE FROM `user_access` WHERE login_id  = '" + login_id + "' AND `site_type` = '0' AND 	category_id = 4";
                await Context.ExecuteNonQry<int>(delAccess1).ConfigureAwait(false);
            }
            if(importapproval > 0)
            {
                string qry4 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES  ('" + login_id + "', 0, 4, '" + importapproval + "', '0')";
                await Context.ExecuteNonQry<int>(qry4).ConfigureAwait(false);
            }
            
            if (flag3 == true)
            {
                qry2 += sitevalues;
                return await Context.ExecuteNonQry<int>(qry2.Substring(0, (qry2.Length - 1)) + ";").ConfigureAwait(false);
            }
            return 0;

        }


        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        private void API_ErrorLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Error**:" + Message + "\r\n");
        }
        private void API_InformationLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Info**:" + Message + "\r\n");
        }

    }

}
