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
            PPT_InformationLog("From Scheduler Service : Scheduler started at :- " + DateTime.Now);
            return Task.CompletedTask;
        }
        private void RunJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    string msg = "From Scheduler Service : Sechduler run at : " + DateTime.Now;
                    PPT_InformationLog(msg);

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

                    msg = "From Scheduler Service : Current Time : " + DateTime.Now;
                    PPT_InformationLog(msg);

                    msg = "From Scheduler Service : Daily Scheduled Mail Sending Time : " + dailyTime;
                    PPT_InformationLog(msg);

                    if (DateTime.Now.ToString("HH:mm") == dailyTime)
                    {
                        PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Inside if where dailytime =" + dailyTime);
                        PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Started Mail Send functionality.");

                        EmailFunction();

                        async Task<int> EmailFunction()
                        {
                            PPT_InformationLog("From Scheduler Service : For Daily Mail Send : EmailFunction() Called from scheduler");
                            string hostName = MyConfig.GetValue<string>("Timer:hostName");

                            bool SolarMailSuccess = false;
                            bool WindMailSuccess = false;
                            try
                            {
                                string apiUrlSolar = hostName + "/api/DGR/EmailSolarReport?fy=" + fy + "&fromDate=" + datetimenow.ToString("yyyy-MM-dd") + "&site=";
                                PPT_InformationLog("From Scheduler Service : For Daily Mail Send Solar : API URL " + apiUrlSolar);
                                CallAPI(apiUrlSolar);
                               // await repo.EmailSolarReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                SolarMailSuccess = true;
                                PPT_InformationLog("From Scheduler Service : For Daily Mail Send Solar : Inside try Solar Daily mail send");
                            }
                            catch(Exception e)
                            {
                                string msg = e.Message;
                                PPT_ErrorLog("From Scheduler Service : For Daily Mail Send Solar : Inside catch Solar Daily mail failed" + msg);
                            }
                            try
                            {
                                string apiUrlWind = hostName + "/api/DGR/EmailWindReport?fy=" + fy + "&fromDate=" + datetimenow.ToString("yyyy-MM-dd") + "&site=";
                                PPT_InformationLog("From Scheduler Service : For Daily Mail Send Wind : API URL " + apiUrlWind);
                                CallAPI(apiUrlWind);
                                // await repo.EmailWindReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                                PPT_InformationLog("From Scheduler Service : For Daily Mail Send Wind : Inside try Wind mail send");
                                WindMailSuccess = true;
                            }
                            catch(Exception e)
                            {
                                string msg = e.Message;
                                PPT_ErrorLog("From Scheduler Service : For Daily Mail Send Wind : Inside catch Wind mail failed" + msg);
                            }
                            if(WindMailSuccess && SolarMailSuccess)
                            {
                                PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Solar and Wind Mail Sent" + msg);
                                return 1;
                            }
                            else
                            {
                                if (!(WindMailSuccess))
                                {
                                    PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Wind Mail send Failed ");
                                }
                                else
                                {
                                    PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Wind Mail send Successful.");
                                }
                                if (!(SolarMailSuccess))
                                {
                                    PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Solar Mail send Failed ");
                                }
                                else
                                {
                                    PPT_InformationLog("From Scheduler Service : For Daily Mail Send : Wind Mail send Successful. ");
                                }
                                return 0;
                            }
                        }                        
                    }

                    //Weekly report mail
                    string weeklyTime = MyConfig.GetValue<string>("Timer:WeeklyReportTime");
                    string weeklyTimeSolar = MyConfig.GetValue<string>("Timer:WeeklyReportTimeSolar");
                    string WeeklyReportDayOfWeek = MyConfig.GetValue<string>("Timer:WeeklyReportDayOfWeek");

                    msg = "From Scheduler Service : Weekly Wind Scheduled Mail Sending Time : " + weeklyTime;
                    PPT_InformationLog(msg);
                    msg= "From Scheduler Service : Weekly Solar Scheduled Mail Sending Time : " + weeklyTimeSolar;
                    PPT_InformationLog(msg);

                    if (DateTime.Now.ToString("HH:mm") == weeklyTime)// && DateTime.Now.ToString("ddd") == WeeklyReportDayOfWeek)
                    {
                        PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : Inside if where Weekly =" + weeklyTime);
                        PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : Started sending Email for Wind Weekly Mail Send ");

                        EmailWeeklyFunction();

                        async Task<int> EmailWeeklyFunction()
                        {
                            PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : Email Weekly Wind Function Called from scheduler");
                            string hostName = MyConfig.GetValue<string>("Timer:hostName");

                            bool WindMailSuccess = false;
                            try
                            {
                                string apiUrlWind = hostName + "/api/DGR/PPTCreate";
                                PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : API URL " + apiUrlWind);
                                CallAPI(apiUrlWind);
                                WindMailSuccess = true;
                                PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : Inside try Wind Weekly Mail Send Flag : " + WindMailSuccess);
                            }
                            catch (Exception e)
                            {
                                string msg = e.Message;
                                PPT_ErrorLog("From Scheduler Service : For Wind Weekly Mail Send : Inside catch Wind weeekly mail failed" + msg + " Flag : " + WindMailSuccess);
                            }
                            if (WindMailSuccess)
                            {
                                PPT_InformationLog("From Scheduler Service : For Wind Weekly Mail Send : Mail Sent" + msg);
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }

                    //Solar Weekly Mail Send Function 
                    if (DateTime.Now.ToString("HH:mm") == weeklyTimeSolar)// && DateTime.Now.ToString("ddd") == WeeklyReportDayOfWeek)
                    {
                        PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Inside if where Weekly time = " + weeklyTime);
                        PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Started sending Email for Solar Weekly Mail Send ");

                        EmailWeeklyFunction();

                        async Task<int> EmailWeeklyFunction()
                        {
                            PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Email Weekly Mail Send Solar Function Called from scheduler");
                            string hostName = MyConfig.GetValue<string>("Timer:hostName");

                            bool SolarMailSuccess = false;
                            try
                            {
                                string apiUrlSolar = hostName + "/api/DGR/PPTCreate_Solar";
                                PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Email weekly mail send solar API URL " + apiUrlSolar);
                                CallAPI(apiUrlSolar);
                                SolarMailSuccess = true;
                                PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Inside try Solar weekly mail send Flag : " + SolarMailSuccess );
                            }
                            catch (Exception e)
                            {
                                string msg = e.Message;
                                PPT_ErrorLog("From Scheduler Service : For Solar Weekly Mail Send : Inside catch Solar Weekly mail failed" + msg);
                            }
                            if (SolarMailSuccess)
                            {
                                PPT_InformationLog("From Scheduler Service : For Solar Weekly Mail Send : Solar Mail Sent" + msg + " Flag : " + SolarMailSuccess );
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message+  " exception at :-  " + DateTime.Now;
                    PPT_ErrorLog(msg);
                }
                Interlocked.Increment(ref executionCount);
            }
        }

        public void CallAPI (string apiUrl)
        {
            try
            {
                Uri address = new Uri(apiUrl);
                PPT_InformationLog("From Scheduler Service : Inside CallAPI function : Api Url :" + apiUrl);
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
                    PPT_InformationLog("From Scheduler Service : Inside CallAPI function : " + reader.ReadToEnd());

                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                PPT_ErrorLog("inside callApi Function: " + msg);
            
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

        private void PPT_ErrorLog(string Message)
        {
            try
            {
                System.IO.File.AppendAllText(@"C:\LogFile\PPT_Log.txt", "**Error**:" + Message + "\r\n");
            }
            catch (Exception e)
            {
            }
            //Read variable from appsetting to enable disable log

        }
        private void PPT_InformationLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            try
            {
                System.IO.File.AppendAllText(@"C:\LogFile\PPT_Log.txt", "**Info**:" + Message + "\r\n");
            }
            catch (Exception e)
            {
            }
        }
    }
}
