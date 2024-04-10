using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading;

namespace DGRA_V1.Controllers
{

    // [Authorize]
    [AllowAnonymous]
    public class WindReportController : Controller
    {
        private IDapperRepository _idapperRepo;
        public WindReportController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
        
        //FInacnial Year APi Hardcoded 
        public async Task<IActionResult> GetFinacialYear()
        {
            //countryname = "India";
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL")+"/api/DGR/GetFinancialYear";
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
        // Country List
        public async Task<IActionResult> GetCountryList()
        {
            string country = "";
            CountryList countrylist = new CountryList();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetCountryList";
               // var url = "http://localhost:23835/api/DGR/GetCountryList";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string line = readStream.ReadToEnd().Trim();
                        countrylist.list = JsonConvert.DeserializeObject<List<CountryList>>(line);
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            //return Content(country, "application/json");
            return View(countrylist);

        }
        // State List
        public async Task<IActionResult>GetStateList(string countryname,string sitelist)
        {
            //countryname = "India";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetStateList?country=" + countryname + "&site="+ sitelist;
                //var url = "http://localhost:23835/api/DGR/GetStateList?country=" + countryname + "";
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
        // SPV List
        public async Task<IActionResult> GetSPVList(string state, string sitelist)
        {
            string line = "";
            string statedata = "";
            if (state != "undefined" && state !=null)
            {
                statedata = state;
            }
          
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSPVList?state=" + statedata + "&site="+ sitelist;
               // var url = "http://localhost:23835/api/DGR/GetSPVList?state=" + state + "";
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
        // Site List
        public async Task<IActionResult> GetSiteList(string state, string spv,string sitelist)
        {
            string line = "";
            string spvdata = "";
            string statedata = "";
            if (state == "undefined" || state == null)
            {
                statedata = "";
            }
            else
            {
                statedata = state;
            }
            if (spv == "undefined" || spv == null)
            {
                spvdata = ""; 
            }
            else
            {
                spvdata = spv;
            }
            
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSiteList?state=" + statedata + "&spvdata=" + spvdata+"&site="+ sitelist;
               // var url = "http://localhost:23835/api/DGR/GetSiteList?state="+ statedata + "&spvdata="+ spvdata;
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
        // Site List
        public async Task<IActionResult> GetWTGList(string siteid, string state,string spv)
        {
           
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWTGList?siteid=" + siteid + "&state=" + state + "&spv=" + spv;
                //var url = "http://localhost:23835/api/DGR/GetWTGList?siteid=" + siteid + "";
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
        public async Task<IActionResult> GetSiteMaster(string sitelist)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster?site="+ sitelist;

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
        public async Task<IActionResult> GetLocationMaster(string sitelist)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster?site="+ sitelist;

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
        public async Task<IActionResult> GetWindDailyGenerationReportWTGWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string reportType = "WTG";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenerationReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&reportType="+ reportType + "";
               
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

        public async Task<IActionResult> GetWindDailyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string month = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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

        //Monthly Gen WTG WIse
        public async Task<IActionResult> GetWindMonthlyGenerationReportWTGWise(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyGenerationReport?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&reportType=" + reportType + "";

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

        //Monthly Gen SIteWIse
        public async Task<IActionResult> GetWindMonthlyGenerationReportSiteWise(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {

        
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyYearlyGenSummaryReport2?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "";

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
        public async Task<IActionResult> GetWindYearlyGenerationReportWTGWise(string fromDate,string toDate,  string country, string state, string spv, string site, string wtg, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindYearlyGenSummaryReport1?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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
        public async Task<IActionResult> GetWindYearlyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindYearlyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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
       
        public async Task<IActionResult> GetWindBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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
        public async Task<IActionResult> GetWindPRReportSPVWise(string fy, string fromDate, string toDate,string sitelist,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportBySPVWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist +"&spv="+ spv;
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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
        public async Task<IActionResult> GetWindPRReportSiteWise(string fy, string fromDate, string toDate,string sitelist,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist+"&spv="+spv;
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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
        public async Task<IActionResult> GetOperationHeadData(string site,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetOperationHeadData?site="+ site + "&spv="+spv;
                
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
        public async Task<IActionResult> GetWeeklyOperation(string fy, string fromDate, string toDate, string site,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site+ "&spv="+spv;
                
                //old 
               // var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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

        public async Task<IActionResult> GetMontlyOperation(string fy, string fromDate, string toDate, string site,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site+"&spv="+spv;
                ///old api 
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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
        public async Task<IActionResult> GetYearlyOperation(string fy, string fromDate, string toDate, string site,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ site+ "&spv="+spv;
                //old 
               // var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
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
        public async Task<IActionResult> DeleteWindSite(int siteid)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/DeleteWindSite?siteid=" + siteid;
               
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
        public async Task<IActionResult> GetWindMajorBreakdown(string fromDate, string toDate,string siteList,string spv)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMajorBreakdown?fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ siteList+"&spv="+spv;
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

        public async Task<IActionResult> GetWindPowerCurveData(string site, string fromDate, string toDate)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPowerCurveData?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate;
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
        public async Task<IActionResult> GetWindTmlPowerCurveData(string site, string fromDate, string toDate, string wtgs)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindTmlPowerCurveData?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate + "&wtgs=" + wtgs;
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


        //DGR Version 3 functions.
        public async Task<IActionResult> OPGetSiteListForEdit(int month_no, int year, int siteType, int bdTypes, int isMonthly)
        {
            string line = "";

            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/OPGetSiteListForEdit?month_no=" + month_no + "&year=" + year + "&siteType=" + siteType + "&bdTypes=" + bdTypes + "&isMonthly=" + isMonthly;
                // var url = "http://localhost:23835/api/DGR/GetSiteList?state="+ statedata + "&spvdata="+ spvdata;
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
        public async Task<IActionResult> OPGetSpvListForEdit(int month_no, int year, int siteType, int bdTypes, int isMonthly)
        {
            string line = "";

            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/OPGetSpvListForEdit?month_no=" + month_no + "&year=" + year + "&siteType=" + siteType + "&bdTypes=" + bdTypes + "&isMonthly=" + isMonthly;
                // var url = "http://localhost:23835/api/DGR/GetSiteList?state="+ statedata + "&spvdata="+ spvdata;
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

        public async Task<IActionResult> GetOPComments(int month_no, int year, int siteType, string site_id, int isSPV, string spv, int bdType, int isDisplay, int isMonthly)
        {
            string line = "";
            //?month_no =' + monthNo + '&year=' + year + '&siteType=' + 2 + '&site_id=' + site_id + '&isSPV=' + isSPV + '&spv=' + spv
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetOPComments?month_no=" + month_no + "&year=" + year + "&siteType=" + siteType + "&spv=" + spv + "&site_id=" + site_id + "&bdType=" + bdType + "&isDisplay=" + isDisplay + "&isMonthly=" + isMonthly + "&isSpv=" + isSPV;

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
        public async Task<IActionResult> OPACM([FromBody] List<OPComments> ACMFinalData)
        {
            string line = "";
            //string ty = typeof(ACMFinalData).ToString();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/OPCommentsInsert";

                using (var client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                    // Convert acmDataList to JSON
                    var json = JsonConvert.SerializeObject(ACMFinalData);
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

        [HttpPost]
        public async Task<IActionResult> OPECM([FromBody] List<OPComments> ECMFinalData)
        {
            string line = "";
            //string ty = typeof(ACMFinalData).ToString();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/OPCommentsEdit";

                using (var client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                    // Convert acmDataList to JSON
                    var json = JsonConvert.SerializeObject(ECMFinalData);
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

        [HttpPost]
        public async Task<IActionResult> OPDCM([FromBody] List<OPComments> ECMFinalData)
        {
            string line = "";
            //string ty = typeof(ACMFinalData).ToString();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/OPCommentsDelete";

                using (var client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                    // Convert acmDataList to JSON
                    var json = JsonConvert.SerializeObject(ECMFinalData);
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

    }
}
