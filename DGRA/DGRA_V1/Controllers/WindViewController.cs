﻿using DGRA_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace DGRA_V1.Controllers
{

    //[Authorize]
    [AllowAnonymous]
    public class WindViewController : Controller
    {
        private IDapperRepository _idapperRepo;
        public WindViewController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }

        
        public async Task<IActionResult> WindGenView(string site, string fromDate, string ToDate)
        {

            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenSummary?site=" + site + "&fromDate=" + fromDate + "&ToDate=" + ToDate + "";
               // var url = "http://localhost:23835/api/DGR/GetWindDailyGenSummary?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> WindDailyTargetKPI(string site, string fromDate, string toDate)
        {
           
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?site=" +site+ "&fromDate=" + fromDate + "&toDate=" + toDate + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&toDate=" + toDate + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                       line = readStream.ReadToEnd().Trim();
                       //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetMonthlyTargetKPI(string site, string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyTargetKPI?site="+site+"&fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyTargetKPI?fy=" + year + "&month=" + month + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetMonthlyLinelossView(string site, string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyLineLoss?site="+site+"&fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyLineLoss?fy=" + year + "&month=" + month + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetDailyLoadshedding(string site, string fromDate,string toDate)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyloadShedding?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyloadShedding?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate +"";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult>GetMonthlyJMRView(string site, string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyJMR?site=" +site+ "&fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyJMR?fy=" + year + "&month=" + month + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetImportBatches(string importFromDate, string importToDate, string siteId,int importType, int status)
        {
            int user_id = 0;
            if(HttpContext.Session.GetString("role") =="User")
            {
                user_id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            }
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetImportBatches?importFromDate=" + importFromDate + "&importToDate=" + importToDate + "&siteId=" + siteId + "&importType=" + importType + "&status=" + status + "&userid="+ user_id + "";
                //var url = "http://localhost:23835/api/DGR/GetImportBatches?importFromDate=" + importFromDate + "&importToDate=" + importToDate + "&siteId="+ siteId + "&importType="+ importType+ "&status=" + status + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
             catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        
        public async Task<IActionResult> DataApproved(string data, int approvedBy, string approvedByName, int status,int actionType)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetApprovalFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
                //var url = "http://localhost:23835/api/DGR/SetApprovalFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
                WebRequest request = WebRequest.Create(url);
                request.Timeout = Timeout.Infinite;
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> DataReject(string data, int rejectedBy, string rejectByName, int status, int actionType)
        {
            //var json = JsonConvert.SerializeObject(data);

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetRejectFlagForImportBatches?dataId=" + data + "&rejectedBy=" + rejectedBy + "&rejectByName=" + rejectByName + "&status=" + status + "";
                //var url = "http://localhost:23835/api/DGR/SetRejectFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetGenerationImportData(int importId)
        {
           
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetImportGenData?importId=" + importId +"";
               
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
        public async Task<IActionResult> GetBrekdownImportData(int importId)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBrekdownImportData?importId=" + importId + "";
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
        public async Task<IActionResult> GetWindTMLData(string site, string fromDate, string toDate)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindTMLData?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate;
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
        //GetWindTMLGraphData
        public async Task<IActionResult> GetWindTMLGraphData(string site, string fromDate, string toDate, int isAdmin, int isYearly)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindTMLGraphData?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate + "&isAdmin=" + isAdmin + "&isYearly=" + isYearly;
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
        //changes
        //public IActionResult DownloadExcelWind(string importId)
        //{
        //    try
        //    {
        //        var apiUrl = _idapperRepo.GetAppSettingValue("API_URL");
        //        var url = $"{apiUrl}/api/DGR/DownloadExcelWind?importId={importId}";

        //        // Create an HttpWebRequest
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        //        // Get the response from the server
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            // Check the status code to ensure a successful response
        //            if (response.StatusCode == HttpStatusCode.OK)
        //            {
        //                // Read the response stream (assuming it's a binary file)
        //                using (Stream responseStream = response.GetResponseStream())
        //                {
        //                    // Read the stream into a byte array
        //                    using (MemoryStream memoryStream = new MemoryStream())
        //                    {
        //                        responseStream.CopyTo(memoryStream);
        //                        byte[] excelData = memoryStream.ToArray();

        //                        // Check if data is present
        //                        if (excelData == null || excelData.Length == 0)
        //                        {
        //                            TempData["notification"] = "Data Not Present!";
        //                            return Content("Data Not Present!", "text/plain");
        //                        }

        //                        // Set the Content-Disposition header for file download
        //                        Response.Headers.Add("Content-Disposition", "attachment; filename=Wind_Import.xlsx");

        //                        // Return the Excel data as a file "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //                        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Wind_Import.xlsx");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                TempData["notification"] = $"Unexpected response status: {response.StatusCode}";
        //                return Content($"Unexpected response status: {response.StatusCode}", "text/plain");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["notification"] = "An error occurred while exporting data!";
        //        return Content(ex.Message, "text/plain");
        //    }
        //}
    }
}
