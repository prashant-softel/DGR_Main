using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using DGRA_V1.Controllers;
using System.Net.Http;
using System.Diagnostics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static System.Net.WebRequestMethods;

namespace DGRA_V1.Common
{
    public class SchedulerService : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private System.Threading.Timer _timerNotification;
        public IConfiguration _iconfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private readonly HttpClient _httpClient;

        public SchedulerService(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration, HttpClient httpClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
            _httpClient = httpClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            API_ErrorLog("Schedulerstarted web at1   :- " + DateTime.Now);
            _timerNotification = new Timer(RunJob, null, TimeSpan.Zero,
              TimeSpan.FromMinutes(1)); /*Set Interval time here*/
            API_ErrorLog("Schedulerstarted web at2 :- " + DateTime.Now);
            return Task.CompletedTask;
        }

        private async void RunJob(object state)
        {
            API_ErrorLog("Schedulerstarted web at 3:- " + DateTime.Now);
            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
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
                    //Weekly report mail
                    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                    string weeklyTime = MyConfig.GetValue<string>("Timer:WeeklyReportTime_ppt");

                    var convertoTime = Convert.ToDateTime(weeklyTime);
                    //convertoTime.AddMinutes(-30);
                    string WeeklyReportDayOfWeek = MyConfig.GetValue<string>("Timer:WeeklyReportDayOfWeek");

                    //if (DateTime.Now.ToString("HH:mm") == convertoTime.ToString("HH:mm") && DateTime.Now.ToString("ddd") == WeeklyReportDayOfWeek)
                    //{
                        string hostName = MyConfig.GetValue<string>("Timer:hostName");

                        //move this to appsetting
                        //string hostName = "https://localhost:44378";
                        //string hostName = "https://cmms.herofutureenergies.com";
                        API_InformationLog("Inside if where time is  =" + convertoTime.ToString() + " hostName :" + hostName);
                    //var psi = new ProcessStartInfo
                    //{
                        //FileName = hostName + "/Home/WindWeeklyPPTReports",// + DateTime.Now.ToString("dd/MM/yyyy"),
                    //    //FileName = "https://cmms.herofutureenergies.com/Home/WindWeeklyPRReports?" + DateTime.Now.ToString("yyyy/MM/dd"),

                       // UseShellExecute = true,

                   // };
                    var psi = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $"/c start \"\" /b \"{hostName + "/Home/WindWeeklyPPTReports"}\" & pause",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(psi);

                    /*var psi = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $"/c start \"\" \"{hostName + "/Home/WindWeeklyPPTReports"}\"",
                        UseShellExecute = true,
                        CreateNoWindow = false,
                    };*/
                    Process.Start(psi);

                    API_InformationLog("Url Web :" + hostName);
                        Process.Start(psi);
                        API_InformationLog("FileName Web :" + Process.Start(psi));



                    var psiSolar = new ProcessStartInfo
                   {
                    FileName = hostName + "/Home/SolarWeeklyPPTReports",//+ DateTime.Now.ToString("yyyy/MM/dd"),
                    //FileName = "https://cmms.herofutureenergies.com/Home/SolarWeeklyPRReports?" + DateTime.Now.ToString("yyyy/MM/dd"),
                    UseShellExecute = true
                    };
                    /*var psiSolar = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $"/c start \"\" \"{hostName + "/Home/SolarWeeklyPPTReports"}\"",
                        UseShellExecute = true,
                        CreateNoWindow = false,
                    };*/
                    Process.Start(psiSolar);
                        API_InformationLog("FileName  Web:" + Process.Start(psiSolar));
                    //}
                }

                catch (Exception ex)
                {
                    //pending error logging
                }
                Interlocked.Increment(ref executionCount);
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
