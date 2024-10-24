﻿using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DGRA_V1.Filters;
using System.Net.Http;
using System.Threading;


namespace DGRA_V1.Controllers
{

    //  [Authorize]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;

        private IDapperRepository _idapperRepo;
       
        private readonly GraphServiceClient _graphServiceClient;


        public HomeController(ILogger<HomeController> logger,
                         GraphServiceClient graphServiceClient,
                         IDapperRepository idapperRepo)
        {
            _logger = logger;
           _graphServiceClient = graphServiceClient;
            _idapperRepo = idapperRepo;
        }

        public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
       
       // private readonly GraphServiceClient _graphServiceClient;
        // TEST A 123456789
      //  public HomeController(ILogger<HomeController> logger, IDapperRepository idapperRepo, GraphServiceClient graphServiceClient)
       // {
        //    _logger = logger;
          //  _idapperRepo = idapperRepo;

       // }
      


      
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        // [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            //var user = await _graphServiceClient.Me.Request().GetAsync();
            //ViewData["ApiResult"] = user.DisplayName;
            //if(!string.IsNullOrEmpty( user.DisplayName))
            //{
            //    return RedirectToAction("Dashbord");
            //}
          //  HttpContext.Session.SetString("product", "laptop");

            return View();
        }


        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
      
        public async Task<IActionResult> SSOLogin ()
        {

          var user = await _graphServiceClient.Me.Request().GetAsync();
            string line = "";
           //bool IsSSO = true;
            string pass = "";
            TempData["username"] = user.UserPrincipalName;
            try
            {
                //user.DisplayName
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UserLogin?username=" + user.Mail + "&password="+ pass+ "&isSSO=true";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        if (string.IsNullOrEmpty(line))
                        {

                            
                            return RedirectToAction("Error");
                        }
                        else
                        {
                           
                            LoginModel model = JsonConvert.DeserializeObject<LoginModel>(line);
                            HttpContext.Session.SetString("DisplayName", model.username);
                            HttpContext.Session.SetString("role", model.user_role);
                            HttpContext.Session.SetString("userid", model.login_id.ToString());

                            int loginid = model.login_id;
                            string role = model.user_role;
                            var actionResult = await GetUserAccess(loginid, role, true);

                            HttpContext.Response.Cookies.Append("userid", model.login_id.ToString());
                            return RedirectToAction("Dashbord");





                        }

                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["notification"] = "Username and password invalid Please try again !";
                string message = ex.Message;
                return RedirectToAction("Error");
            }

            //}
            // return Ok(model);
            //return RedirectToAction("Dashbord", "Home");
           // return Content(line, "application/json");

         
           // }
            //return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string pass, string device_id)
        {
            string status = "";
            string line = "";
            bool IsSSO = false;
            string[] userList = username.Split("@");
            string last = userList[1];
            string encodedPassword = System.Web.HttpUtility.UrlEncode(pass);
            //if (last.Equals("herofutureenergies.com")) {
            // SSOLogin();
            // }
            // else {
            bool login_status = false;
                LoginModel model = new LoginModel();
                //UserAccess usermodel = new UserAccess();
                System.Collections.Generic.Dictionary<string, object>[] map = new System.Collections.Generic.Dictionary<string, object>[1];
                try
                {
                     var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UserLogin?username=" + username + "&password=" + encodedPassword + "&isSSO=false&device_id=" + device_id ;
                    WebRequest request = WebRequest.Create(url);
                    using (WebResponse response = (HttpWebResponse)request.GetResponse()){
                        Stream receiveStream = response.GetResponseStream();
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8)){
                             line = readStream.ReadToEnd().Trim();
                        if (string.IsNullOrEmpty(line)){
                                
                                login_status = false;
                            return Content("Credentials are Incorrect", "application/json");
                        }
                        else{
                                login_status = true;
                                model = JsonConvert.DeserializeObject<LoginModel>(line);
                            line = "Success";

                            if (model.islogin)
                            {
                                login_status = false;
                                return Content("User Already Login", "application/json");
                            }
                            else
                            {

                                HttpContext.Session.SetString("DisplayName", model.username);
                                HttpContext.Session.SetString("role", model.user_role);
                                HttpContext.Session.SetString("userid", model.login_id.ToString());
                                HttpContext.Session.SetString("useremail", model.useremail.ToString());

                                int loginid = model.login_id;
                                string role = model.user_role;
                                HttpContext.Response.Cookies.Append("userid", model.login_id.ToString());
                                var actionResult = await GetUserAccess(loginid, role, true);

                                return Content(line, "application/json");
                            }
                                
                              

                            }

                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["notification"] = "Username and password invalid Please try again !";
                string message = ex.Message;
            }
           
        //}
            // return Ok(model);
            //return RedirectToAction("Dashbord", "Home");
            return Content(line, "application/json");
        }

        //UpdateLoginLog
        public async Task<IActionResult> UpdateLoginLog(int userId, string userRole)
        {
            LoginModel model = new LoginModel();
            string line = "";

            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UpdateLoginLog?userId=" + userId + "&userRole=" + userRole;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        model = JsonConvert.DeserializeObject<LoginModel>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        public async Task<IActionResult> GetUserLoginFromDeviceId(string device_id)
        {
            LoginModel model = new LoginModel();
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetUserLoginFromDeviceId?device_id=" + device_id;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        model = JsonConvert.DeserializeObject<LoginModel>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        public IActionResult UpdateLoginStatus(int userID)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UpdateLoginStatus?userID=" + userID;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();


                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["notification"] = "Username and password invalid Please try again !";
                string message = ex.Message;
            }


            return Content("success", "application/json");
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        // Added Code on Redirection Remote 
       /* [ActionName("signin-oidc")]
        public IActionResult signinoidc()
        {
            //return View();
            return RedirectToAction("Dashbord");
        }*/

        //[Authorize]
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult Dashbord()
        {
            TempData["notification"] = "";
            return View();
            
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindDailyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindGenView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyLinelossView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyJMRView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindDailyLoadSheddingView()
        {
            TempData["notification"] = "";
            return View();
        }

        // Report Routs
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindSiteMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindLocationMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindGenReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindBDReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindPRReport()
        {
            TempData["notification"] = "";
            return View();
        }
        //[TypeFilter(typeof(SessionValidation))]
        public IActionResult WindWeeklyPRReports()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SiteUserMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult UserRegister()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult ImportApproval()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult ColumnAccess()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> WindNewUserRegister(string fname,string useremail,string role,string userpass)
        {
            string line = "";
            string encodedPassword = System.Web.HttpUtility.UrlEncode(userpass);

            try
            {
               /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role + "&userpass="+ encodedPassword + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = ""; 
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> EmailReportTimeChangeSetting(string dailytime, string windweeklytime, string solarweeklytime, string windweekday, string solarweekday, string firstReminderTime, string secondReminderTime ,string username, int user_id, string role, string SolarMonthlyTime,string WindMonthlyTime,string solarmonthdate,string windmonthdate,int email_logId)
        {
            var line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/EmailReportTimeChangeSetting?dailytime=" + dailytime + "&windweeklytime=" + windweeklytime + "&solarweeklytime=" + solarweeklytime + "&windweekday=" + windweekday + "&solarweekday=" + solarweekday +
                    "&firstReminderTime=" + firstReminderTime + "&secondReminderTime=" + secondReminderTime + "&SolarMonthlyTime=" + SolarMonthlyTime + "&WindMonthlyTime=" + WindMonthlyTime + "&solarmonthdate=" + solarmonthdate + "&windmonthdate=" + windmonthdate +
                    "&username=" + username + "&user_id=" + user_id + "&role=" + role+"&EmailLogId="+ email_logId;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        //GetEmailTime
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetEmailTime()
        {
            var line = "";
            try
            {
                //EmailReportTimingsLog emailReportTimingsList = new EmailReportTimingsLog();
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetEmailTime";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //emailReportTimingsList.time_list = JsonConvert.DeserializeObject<List<EmailReportTimingsLog>>(line);
                        //string daily = emailReportTimingsList.time_list[0].daily_report;
                        //string windWeekly = emailReportTimingsList.time_list[0].wind_weekly;
                        //string solarWeekly = emailReportTimingsList.time_list[0].solar_weekly;
                        //string windWeekDay = emailReportTimingsList.time_list[0].wind_weekly_day;
                        //string solarWeekDay = emailReportTimingsList.time_list[0].solar_weekly_day;
                        //HttpContext.Session.SetString("DailyReportTime", daily.ToString());
                        //HttpContext.Session.SetString("WindWeeklyTime", windWeekly.ToString());
                        //HttpContext.Session.SetString("SolarWeeklyTime", solarWeekly.ToString());
                        //HttpContext.Session.SetString("WindWeeklyDay", windWeekDay.ToString());
                        //HttpContext.Session.SetString("SolarWeeklyDay", solarWeekDay.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> UpdatePassword(int loginid, string updatepass)
        {
            string line = "";
            string encodedPassword = System.Web.HttpUtility.UrlEncode(updatepass);

            try
            {
                /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UpdatePassword?loginid=" + loginid + "&updatepass=" + encodedPassword + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> DeactivateUser(int loginid)
        {
            string line = "";
            try
            {
                /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/DeactivateUser?loginid=" + loginid +"";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        // DeleteUser
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult>DeleteUser(int loginid)
        {
            string line = "";
            try
            {
                /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/DeleteUser?loginid=" + loginid + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> ActivateUser(int loginid)
        {
            string line = "";
            try
            {
                /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/ActivateUser?loginid=" + loginid + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetWindUserInfo(int login_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
               // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url =  _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetWindUserInformation?login_id=" + login_id;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetSolarUserInfo(int login_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetSolarUserInformation?login_id=" + login_id;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetUserAccess(int login_id,string role ,bool actionType = false)
        {
            UserAccess usermodel = new UserAccess();
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetWindUserAccess?login_id=" + login_id+"&role="+ role;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        if (actionType == true)
                        {
                            usermodel.access_list = JsonConvert.DeserializeObject<List<UserAccess>>(line);
                            HttpContext.Session.SetString("UserAccess", JsonConvert.SerializeObject( usermodel));
                           // var people = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(line);
                           // HttpContext.Session.SetString("UserAccess", people.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        public async Task<IActionResult> GetCustomGroup(int login_id, int site_type,string groupPage)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetCustomGroup?login_id=" + login_id + "&site_type=" + site_type+ "&groupPage="+ groupPage;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetEmailAccess(int login_id, int site, int access, string notifications=null, bool actionType = false)
        {
            UserInfomation usermodel = new UserInfomation();
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetEmailAccess?login_id=" + login_id + "&site=" + site + "&action=" + access + "&notifications=" + notifications;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        if (actionType == true)
                        {
                            usermodel.access_list = JsonConvert.DeserializeObject<List<UserInfomation>>(line);
                            HttpContext.Session.SetString("UserAccess", JsonConvert.SerializeObject(usermodel));
                            // var people = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(line);
                            // HttpContext.Session.SetString("UserAccess", people.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        //[HttpPost]
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetUserLoginId(string username, string useremail)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetUserLoginId?username=" + username + "&useremail=" +useremail;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        //[HttpPost]
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> SubmitAccess(int login_id,string site,string pages,string reports, string site_type,int importapproval, int heatmap)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/SubmitUserAccess?login_id=" + login_id + "&siteList=" + site + "&pageList=" + pages + "&reportList=" + reports + "&site_type="+site_type+ "&importapproval=" + importapproval + "&heatmap=" + heatmap;
                WebRequest request = WebRequest.Create(url);
                 using (WebResponse response = (HttpWebResponse)request.GetResponse())
                 {
                     Stream receiveStream = response.GetResponseStream();
                     using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                     {
                         line = readStream.ReadToEnd().Trim();
                     }
                 }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> SubmitGroupBySiteAccess(int login_id, string reportgroup, string site_type)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/SubmitGroupBySite?login_id=" + login_id + "&reportgroup=" + reportgroup + "&site_type=" + site_type;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        //SubmitCloneUserAccess
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/SubmitCloneUserAccess?login_id=" + login_id + "&site_type=" + site_type + "&page_type=" + page_type + "&identity=" + identity + "&upload_access=" + upload_access;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        public async Task<IActionResult> GetCustomGroupAccess(int login_id, int site_type)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetCustomGroupAccess?login_id=" + login_id + "&site_type=" + site_type;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindUserView()
        {
            
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult EmailReportTimeSetting()
        {
            //TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarUserView()
        {

            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetPageList(int login_id, int site_type)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
               // var url = "http://localhost:23835/api/Login/GetPageList?login_id=" + login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetPageList?login_id=" + login_id + "&site_type=" + site_type;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        
        public IActionResult SolarGenView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarDailyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarDailyLoadSheddingView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyLinelossView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyJMRView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarAcDcCapacityView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGHIPOA1MinView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGHIPOA15MinView()
        {
            TempData["notification"] = "";
            return View();
        }
        // Report Routs
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGenReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarBDReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarSiteMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarLocationMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarPRReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarWeeklyPRReports()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult ExpectedVsActuallosses()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult TrackerLoss()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult heatMap()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult WindTmlView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult powerCurveReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult WindUserDetails(string id)
        {
           return RedirectToAction("WindUserView", new { id });
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult SolarUserDetails(string id,int site_type)
        {
            return RedirectToAction("SolarUserView", new { id , site_type});
        }
        public async Task<ActionResult> Logout(string username, string pass)
        {
            TempData["notification"] = "";
            int userID = 0;
            userID = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            // UpdateLoginStatus(userID);
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/DirectLogOut?userID=" + userID;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();


                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["notification"] = "Username and password invalid Please try again !";
                string message = ex.Message;
            }


          
            return RedirectToAction("Index", "Home");
            // return View();
        }
        [HttpGet("SignOut")]
        public IActionResult SignOut([FromRoute] string scheme)
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult WindWeeklyPPTReports()
        {
            TempData["notification"] = "";
            return View();
        }
        public IActionResult SolarWeeklyPPTReports()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult FormulaCalculate()
        {

            return View();
        }

        public IActionResult FormulaCalculator()
        {
            TempData["notification"] = "";
            return View();
        }

        //DGR version 3.


		[TypeFilter(typeof(SessionValidation))]
        public ActionResult OPWind()
        {
            TempData["notification"] = "";
            return View();
        }
        
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult OPSolar()
        {
            TempData["notification"] = "";
            return View();
        }
        public IActionResult OPSolarPPT()
        {
            TempData["notification"] = "";
            return View();
        }
        public ActionResult OPWindPPT()
        {
            TempData["notification"] = "";
            return View();
        }
		//DGR_v3 Column Access Code START

        public async Task<IActionResult> GetPageList_CA(int type, int pageType)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetPageList?type=" + type + "&pageType=" + pageType;

                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetGroupList_CA(int page_id)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetGroupList_CA?page_id=" + page_id;

                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetCGColumns_CA(int page_id)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetCGColumns_CA?page_id=" + page_id;

                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateGroup_CA([FromBody] List<CreateGroupData> groupData, int page_id, string group_name)
        {
            string line = "";
            //string ty = typeof(ACMFinalData).ToString();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CreateGroup_CA?page_id=" + page_id + "&group_name=" + group_name;

                using (var client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                    // Convert acmDataList to JSON
                    var json = JsonConvert.SerializeObject(groupData);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    // Make the POST request using HttpClient
                    var response = await client.PostAsync(url, data);
                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        line = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        TempData["notification"] = "Error making API request";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Present!";
            }

            return Content(line, "application/json");
        }

        public async Task<IActionResult> AssignGroup(int login_id, string group_data)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/AssignGroup?login_id=" + login_id + "&group_data=" + group_data;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetPageColumns(int page_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetPageColumns?page_id=" + page_id;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetUserGroupColumns(int page_id, int userId)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetUserGroupColumns?page_id=" + page_id + "&userId=" + userId;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetUserAssignedGroups(int user_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetUserAssignedGroups?user_id=" + user_id;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateGroup_CA([FromBody] int[] groupData, int page_id, int page_groups_id)
        {
            string line = "";
            //string ty = typeof(ACMFinalData).ToString();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/UpdateGroup_CA?page_id=" + page_id + "&page_groups_id=" + page_groups_id;

                using (var client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                    // Convert acmDataList to JSON
                    var json = JsonConvert.SerializeObject(groupData);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    // Make the POST request using HttpClient
                    var response = await client.PostAsync(url, data);
                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        line = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        TempData["notification"] = "Error making API request";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Present!";
            }

            return Content(line, "application/json");
        }
        public async Task<IActionResult> ActiDeactiGroup_CA(int page_groups_id, int status)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/ActiDeactiGroup_CA?page_groups_id=" + page_groups_id + "&status=" + status;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> SaveFormula(int id, string formulas)
        {
            string line = "";
            

            try
            {
                
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SaveFormula?id=" + id + "&formulas=" + formulas + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }

        public IActionResult GetFromula(int site_id, string site_type)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetFormula?site_id=" + site_id + "&site_type=" + site_type + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();


                    }

                }
            }
            catch (Exception ex)
            {
           
                string message = ex.Message;
            }


            return Content(line, "application/json");
        }

        public IActionResult GetFormulaLog(int site_id)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetFormulaLog?site_id=" + site_id +"";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();


                    }

                }
            }
            catch (Exception ex)
            {

                string message = ex.Message;
            }


            return Content(line, "application/json");
        }
    }
}