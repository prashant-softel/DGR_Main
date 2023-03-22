using DGRAPIs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DGRAPIs.Repositories;
using System.Net;
using System.IO;

namespace DGRAPIs.Helper
{
    public class SchedulerService : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private System.Threading.Timer _timerNotification;
        public IConfiguration _iconfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public SchedulerService(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            _timerNotification = new Timer(RunJob, null, TimeSpan.Zero,
              TimeSpan.FromMinutes(1)); /*Set Interval time here*/
            API_ErrorLog("Scheduler started at :- " + DateTime.Now);
            return Task.CompletedTask;
        }
        //public Task StartAsync(CancellationToken stoppingToken)
        //{
        //    // Calculate the time until the next 7:30 PM
        //    DateTime now = DateTime.Now;
        //    DateTime next730PM = now.Date.AddDays(1).AddHours(19).AddMinutes(30);
        //    if (now > next730PM)
        //    {
        //        next730PM = next730PM.AddDays(1);
        //    }
        //    TimeSpan initialDelay = next730PM - now;

        //    // Set up the timer with a 24-hour interval
        //    _timerNotification = new Timer(RunJob, null, initialDelay, TimeSpan.FromHours(24));

        //    return Task.CompletedTask;
        //}


        private void RunJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    //  var store = scrope.ServiceProvider.GetService<IStoreRepo>(); /* You can access any interface or service like this here*/
                    //store.GetAll(); /* You can access any interface or service method like this here*/

                    /*
                     Place your code here which you want to schedule on regular intervals
                     */

                    string msg = "Sechduler run at : " + DateTime.Now;
                    API_ErrorLog(msg);
                    
                    //MYSQLDBHelper db = new MYSQLDBHelper("temp");
                    //var repo = new DGRRepository(db);
                    //repo.MailSend("Calling this function  repo.EmailSolarReport(fy, '2023-03-01'   at " + DateTime.Now + "", " Test mail sechduler");

                    DateTime datetimenow = DateTime.Now;
                    DateTime oneWeekAgo = datetimenow.Date.AddDays(-7);
                    string fy = "";

                    if (datetimenow.Month > 3)
                    {
                        if (oneWeekAgo.Month < 4)
                            oneWeekAgo = new DateTime(oneWeekAgo.Year, 04, 01);

                        fy = datetimenow.Year.ToString() + "-" + datetimenow.AddYears(1).Year.ToString().Substring(2, 2);
                    }
                    else
                    {
                        fy = datetimenow.AddYears(-1).Year.ToString() + "-" + datetimenow.Year.ToString().Substring(2, 2);
                    }

                    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                    string dailyTime = MyConfig.GetValue<string>("Timer:DailyReportTime");
                    //daily mail

                    msg = "Current Time : " + DateTime.Now;
                    API_InformationLog(msg);

                    msg = "Daily time Time : " + dailyTime;
                    API_InformationLog(msg);

                    if (DateTime.Now.ToString("HH:mm") == dailyTime)
                    {
                        API_InformationLog("Inside if where dailytime ="+dailyTime);

                        EmailFunction();

                        async Task<int> EmailFunction()
                        {
                            API_InformationLog("Email Function Called from scheduler");
                            string hostName = MyConfig.GetValue<string>("Timer:hostName");

                            bool SolarMailSuccess = false;
                            bool WindMailSuccess = false;
                            try
                            {
                                string apiUrlSolar = hostName+ "/api/DGR/EmailSolarReport?fy=" + fy +"&fromDate="+ datetimenow.ToString("yyyy-MM-dd") +"&site=";
                                API_InformationLog("API URL " + apiUrlSolar);
                                CallAPI(apiUrlSolar);
                               // await repo.EmailSolarReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                SolarMailSuccess = true;
                                API_InformationLog("Inside try Solar mail send");
                            }
                            catch(Exception e)
                            {
                                string msg = e.Message;
                                API_ErrorLog("Inside catch Solar mail failed"+ msg);
                            }
                            try
                            {
                                
                                string apiUrlWind = hostName + "/api/DGR/EmailWindReport?fy=" + fy + "&fromDate=" + datetimenow.ToString("yyyy-MM-dd") + "&site=";

                                API_InformationLog("API URL "+ apiUrlWind);
                                CallAPI(apiUrlWind);
                                // await repo.EmailWindReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                API_InformationLog("Inside try Wind mail send");
                                WindMailSuccess = true;
                            }
                            catch(Exception e)
                            {
                                string msg = e.Message;
                                API_ErrorLog("Inside catch Wind mail failed"+ msg);
                            }
                            if(WindMailSuccess && SolarMailSuccess)
                            {
                                API_InformationLog("Solar and Wind Mail Sent"+msg);
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        
                    }

                    //Weekly report mail
                    string weeklyTime = MyConfig.GetValue<string>("Timer:WeeklyReportTime");
                    string WeeklyReportDayOfWeek = MyConfig.GetValue<string>("Timer:WeeklyReportDayOfWeek");

                    if (DateTime.Now.ToString("HH:mm") == weeklyTime)// && DateTime.Now.ToString("ddd") == WeeklyReportDayOfWeek)
                    {
                        API_InformationLog("Inside if where dailytime =" + dailyTime);

                        EmailWeeklyFunction();

                        async Task<int> EmailWeeklyFunction()
                        {
                            API_InformationLog("Email Weekly Function Called from scheduler");
                            string hostName = MyConfig.GetValue<string>("Timer:hostName");

                            bool SolarMailSuccess = false;
                            bool WindMailSuccess = false;
                            try
                            {
                                string apiUrlSolar = hostName + "/api/DGR/PPTCreate";
                                API_InformationLog("API URL " + apiUrlSolar);
                                CallAPI(apiUrlSolar);
                                // await repo.EmailSolarReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                SolarMailSuccess = true;
                                API_InformationLog("Inside try Solar mail send");
                            }
                            catch (Exception e)
                            {
                                string msg = e.Message;
                                API_ErrorLog("Inside catch WInd weeekly mail failed" + msg);
                            }
                            try
                            {

                                string apiUrlSolar = hostName + "/api/DGR/PPTCreate_Solar";

                                API_InformationLog("API URL " + apiUrlSolar);
                                CallAPI(apiUrlSolar);
                                // await repo.EmailWindReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                API_InformationLog("Inside try Solar mail send");
                                WindMailSuccess = true;
                            }
                            catch (Exception e)
                            {
                                string msg = e.Message;
                                API_ErrorLog("Inside catch Wind mail failed" + msg);
                            }
                            if (WindMailSuccess && SolarMailSuccess)
                            {
                                API_InformationLog("Solar and Wind Mail Sent" + msg);
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }



                        //repo.PPTCreate(fy, datetimenow.ToString("yyyy-MM-dd"), datetimenow.ToString("yyyy-MM-dd"), "");
                        // repo.PPTCreate_Solar(fy, datetimenow.ToString("yyyy-MM-dd"), datetimenow.ToString("yyyy-MM-dd"), "");
                    }

                    //repo.EmailSolarReport(fy, "2022-12-31", "");
                    //API_ErrorLog("Scheduler method EmailWindReport calling at :- " + DateTime.Now);
                    //repo.EmailWindReport(fy, "2023-02-27", "");
                    //API_ErrorLog("Scheduler mail sent :- " + DateTime.Now);
                }


                catch (Exception ex)
                {

                    string msg = ex.Message+  " exception at :-  " + DateTime.Now;
                    API_ErrorLog(msg);
                }
                Interlocked.Increment(ref executionCount);
            }
        }

        public void CallAPI (string apiUrl)
        {
            Uri address = new Uri(apiUrl);
            API_InformationLog("Api Url :"+ apiUrl);
            // Create the web request
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST
            request.Method = "GET";
            request.ContentType = "text/xml";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output
                string strOutputXml = reader.ReadToEnd();
                API_InformationLog(reader.ReadToEnd());

            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timerNotification?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerNotification?.Dispose();
        }

        private void API_ErrorLog(string Message)
        {
            try
            {
                System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Error**:" + Message + "\r\n");
            }
            catch (Exception e)
            {
            }
            //Read variable from appsetting to enable disable log
            
        }
        private void API_InformationLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            try
            {
                System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Info**:" + Message + "\r\n");
            }
            catch (Exception e)
            { 
            }
           
        }
    }
}
