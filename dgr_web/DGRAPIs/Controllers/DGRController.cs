﻿using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
//using ClosedXML.Excel;
using System.IO;

namespace DGRAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DGRController : ControllerBase
    {
        private readonly IDGRBS _dgrBs;
        public DGRController(IDGRBS dgr)
        {
            _dgrBs = dgr;
        }

        [Route("GetFinancialYear")]
        [HttpGet]
        public async Task<IActionResult> GetFinancialYear()
        {
            try
            {
                var data = await _dgrBs.GetFinancialYear();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindDailyGenSummary")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummary(string site, string fromDate, string ToDate)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummary(site, fromDate, ToDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        #region get Masters

        [Route("GetWindSiteMaster")]
        [HttpGet]
        public async Task<IActionResult> GetWindSiteMaster(string site)
        {
            try
            {
                var data = await _dgrBs.GetWindSiteMaster(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindLocationMaster")]
        [HttpGet]
        public async Task<IActionResult> GetWindLocationMaster(string site)
        {
            try
            {
                var data = await _dgrBs.GetWindLocationMaster(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarSiteList")]
        [HttpGet]
        public async Task<IActionResult> GetSolarSiteList(string state, string spvdata, string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarSiteList(state, spvdata, site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetSolarSiteMaster")]
        [HttpGet]
        public async Task<IActionResult> GetSolarSiteMaster(string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarSiteMaster(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarLocationMaster")]
        [HttpGet]
        public async Task<IActionResult> GetSolarLocationMaster()
        {
            try
            {
                var data = await _dgrBs.GetSolarLocationMaster();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarLocationMasterForFileUpload")]
        [HttpGet]
        public async Task<IActionResult> GetSolarLocationMasterForFileUpload(string siteName)
        {
            try
            {
                var data = await _dgrBs.GetSolarLocationMasterForFileUpload(siteName);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarLocationMasterBySite")]
        [HttpGet]
        public async Task<IActionResult> GetSolarLocationMasterBySite(string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarLocationMasterBySite(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion //Masters

         [Route("GetWindDashboardData")]
        //[Route("GetWindDashboardData/{startDate}/{endDate}/{FY}/{sites}")]
      
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardData(startDate, endDate, FY, sites, monthly);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDashboardDataCache/{startDate}/{endDate}/{FY}/{sites}")]
        //[HttpGet]
        //public async IActionResult GetWindDashboardDataCache(string startDate, string endDate, string FY, string sites)
        //{
        //    if (!_cache.TryGetValue(CacheKeys.Employees, out List<Employee> employees))
        //    {
        //        employees = GetEmployeesDeatilsFromDB(); // Get the data from database
        //        var cacheEntryOptions = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
        //            SlidingExpiration = TimeSpan.FromMinutes(2),
        //            Size = 1024,
        //        };
        //        _cache.Set(CacheKeys.Employees, employees, cacheEntryOptions);
        //    }
        //    return Ok(employees);
        //}


       // [Route("GetWindDashboardDataByLastDay/{startDate}/{endDate}/{FY}/{sites}/{date}")]
        [Route("GetWindDashboardDataByLastDay")]
        [HttpGet]
        //public async Task<IActionResult> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites,string date)
             public async Task<IActionResult> GetWindDashboardDataByLastDay(string FY, string sites, string date)
            {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByLastDay(FY, sites, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetWindDashboardDataByCurrentMonth/{startDate}/{endDate}/{FY}/{sites}/{month}")]
        [Route("GetWindDashboardDataByCurrentMonth")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetWindDashboardDataByYearly/{startDate}/{endDate}/{FY}/{sites}")]
        [Route("GetWindDashboardDataByYearly")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByYearly(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        //[Route("GetSolarDashboardData/{startDate}/{endDate}/{FY}/{sites}")]
        [Route("GetSolarDashboardData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardData(startDate, endDate, FY, sites, monthly);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetSolarDashboardDataByLastDay/{startDate}/{endDate}/{FY}/{sites}/{date}")]
        [Route("GetSolarDashboardDataByLastDay")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByLastDay(string FY, string sites, string date)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByLastDay(FY, sites, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetSolarDashboardDataByCurrentMonth/{startDate}/{endDate}/{FY}/{sites}/{month}")]
        [Route("GetSolarDashboardDataByCurrentMonth")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetSolarDashboardDataByYearly/{startDate}/{endDate}/{FY}/{sites}")
        [Route("GetSolarDashboardDataByYearly")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByYearly(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDailyGenerationReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}")]
        [Route("GetWindDailyGenerationReport")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg,string reportType)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenerationReport(fromDate, toDate, country, state, spv, site, wtg, reportType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarDailyGenerationReport")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string reportType)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenerationReport(fromDate, toDate, country, state, spv, site, inv, reportType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDailyBreakdownReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}")]
        [Route("GetWindDailyBreakdownReport")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyBreakdownReport(fromDate, toDate, country, state, spv, site, wtg);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarDailyGenSummary")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummary()
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummary();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummary1")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummary1(string site, string fromDate, string ToDate)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummary1(site, fromDate, ToDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SolarGhi_Poa_1Min")]
        [HttpGet]
        public async Task<IActionResult> SolarGhi_Poa_1Min(string site, string fromDate, string ToDate)
        {
            try
            {
                var data = await _dgrBs.SolarGhi_Poa_1Min(site, fromDate, ToDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SolarGhi_Poa_15Min")]
        [HttpGet]
        public async Task<IActionResult> SolarGhi_Poa_15Min(string site, string fromDate, string ToDate)
        {
            try
            {
                var data = await _dgrBs.SolarGhi_Poa_15Min(site, fromDate, ToDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #region get wind reports

        [Route("GetWindDailyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDailyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")
        [Route("GetWindDailyGenSummaryReport2")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryReport2")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inv, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarLocationMaster")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarLocationMaster(List<SolarLocationMaster> set)
        {
            try
            {
                var data = await _dgrBs.InsertSolarLocationMaster(set);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

       // [Route("GetWindMonthlyGenerationReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [Route("GetWindMonthlyGenerationReport")]
        [HttpGet]
        //public async Task<IActionResult> GetWindMonthlyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        public async Task<IActionResult> GetWindMonthlyGenerationReport(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyGenerationReport(fy, month, country, state, spv, site, wtg, reportType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindMonthlyYearlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [Route("GetWindMonthlyYearlyGenSummaryReport2")]
        [HttpGet]
        //public async Task<IActionResult> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        public async Task<IActionResult> GetWindMonthlyYearlyGenSummaryReport2(string fy, string month, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyYearlyGenSummaryReport2(fy, month, country, state, spv, site, wtg);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        //[Route("GetWindYearlyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [Route("GetWindYearlyGenSummaryReport1")]
        [HttpGet]
        public async Task<IActionResult> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
		[Route("InsertSolarSiteMaster")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarSiteMaster(List<SolarSiteMaster> set)
        {
            try
            {
                var data = await _dgrBs.InsertSolarSiteMaster(set);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindYearlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [Route("GetWindYearlyGenSummaryReport2")]
        [HttpGet]
        public async Task<IActionResult> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetWindWtgFromdailyGenSummary/{state}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindWtgFromdailyGenSummary(string state, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindWtgFromdailyGenSummary(state, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
		[Route("InsertWindSiteMaster")]
        [HttpPost]
        public async Task<IActionResult> InsertWindSiteMaster(List<WindSiteMaster> set)
        {
            try
            {
                var data = await _dgrBs.InsertWindSiteMaster(set);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindPerformanceReportSiteWise_2")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceReportSiteWise_2(string fromDate, string toDate, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceReportSiteWise_2(fromDate, toDate, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarPerformanceReportSiteWise_2")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceReportSiteWise_2(string fromDate, string toDate, string site,int cnt)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceReportSiteWise_2(fromDate, toDate, site,cnt);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMajorBreakdownData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMajorBreakdownData(string fromDate, string toDate, string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetSolarMajorBreakdownData(fromDate, toDate, site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindPerformanceReportSiteWise")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate,string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceReportSiteWise(fy, fromDate, todate,site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("EmailWindReport")]
        [HttpGet]
        public async Task<IActionResult> EmailWindReport(string fy, string fromDate , string site)
        {
            try
            {
                var data = await _dgrBs.EmailWindReport(fy, fromDate, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("EmailSolarReport")]
        [HttpGet]
        public async Task<IActionResult> EmailSolarReport(string fy, string fromDate, string site)
        {
            try
            {
                var data = await _dgrBs.EmailSolarReport(fy, fromDate, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertWindLocationMaster")]
        [HttpPost]
        public async Task<IActionResult> InsertWindLocationMaster(List<WindLocationMaster> set)
        {
            try
            {
                var data = await _dgrBs.InsertWindLocationMaster(set);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

       // [Route("GetWindPerformanceReportBySPVWise/{fy}/{fromDate}/{toDate}")
        [Route("GetWindPerformanceReportBySPVWise")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate,string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceReportBySPVWise(fy, fromDate, todate,site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarDailyBreakdownReport")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyBreakdownReport(fromDate, toDate, country, state, spv, site, inv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion //Wind reports


        #region KPI Calculations


        [Route("CalculateDailySolarKPI")]
        [HttpGet]
        public async Task<IActionResult> CalculateDailySolarKPI(string site, string fromDate, string toDate, string logFileName)
        {
            try
            {
                var data = await _dgrBs.CalculateDailySolarKPI(site, fromDate, toDate, logFileName);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("PowerExpected")]
        [HttpGet]
        public async Task<IActionResult> PowerExpected(string site, string fromDate, string toDate, string logFileName)
        {
            try
            {
                var data = await _dgrBs.PowerExpected(site, fromDate, toDate, logFileName);
                return Ok(data);

            }
            catch (Exception ex)
            {
                API_ErrorLog("Inside CalculateDailySolarKPI failed : Exception caught: "+ ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [Route("SolarExpectedReport")]
        [HttpGet]
        public async Task<IActionResult> SolarExpectedReport(string site, string fromDate, string toDate, string prType)
        {
            try
            {
                var data = await _dgrBs.GetSolarExpectedReport(site, fromDate, toDate, prType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarTrackerLoss")]
        [HttpGet]
        public async Task<IActionResult> GetSolarTrackerLoss(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetSolarTrackerLoss(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindTMLData")]
        [HttpGet]
        public async Task<IActionResult> GetWindTMLData(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetWindTMLData(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindPowerCurveData")]
        [HttpGet]
        public async Task<IActionResult> GetWindPowerCurveData(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetWindPowerCurveData(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindTmlPowerCurveData")]
        [HttpGet]
        public async Task<IActionResult> GetWindTmlPowerCurveData(string site, string fromDate, string toDate, string wtgs)
        {
            try
            {
                var data = await _dgrBs.GetWindTmlPowerCurveData(site, fromDate, toDate, wtgs);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //GetWindTMLGraphData
        [Route("GetWindTMLGraphData")]
        [HttpGet]
        public async Task<IActionResult> GetWindTMLGraphData(string site, string fromDate, string toDate, int isAdmin, int isYearly)
        {
            try
            {
                var data = await _dgrBs.GetWindTMLGraphData(site, fromDate, toDate, isAdmin, isYearly);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("CalculateDailyWindKPI")]
        [HttpGet]
        public async Task<IActionResult> CalculateDailyWindKPI(string fromDate, string toDate, string site)
        {
            try
            {
                var data = await _dgrBs.CalculateDailyWindKPI(fromDate, toDate, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("LogInfo")]
        [HttpGet]
        public async Task<IActionResult> LogInfo(int userId, int import_type, int module, string api_name, string Message, int is_frontend)
        {
            try
            {
                var data = await _dgrBs.LogInfo(userId, import_type, module, api_name, Message, is_frontend);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("LogError")]
        [HttpGet]
        public async Task<IActionResult> LogError(int userId, int import_type, int module, string api_name, string Message, int is_frontend)
        {
            try
            {
                var data = await _dgrBs.LogError(userId, import_type, module, api_name, Message, is_frontend);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion
        private void API_ErrorLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "DGRA Controllers **Error**:" + Message + "\r\n");
        }
        private void API_InformationLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "DGRA Controllers **Info**:" + Message + "\r\n");
        }
        #region inserts

        [Route("InsertDailyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertDailyTargetKPI(List<WindDailyTargetKPI> windDailyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertDailyTargetKPI(windDailyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertMonthlyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> set)
        {
            try
            {
                var data = await _dgrBs.InsertMonthlyTargetKPI(set);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertMonthlyUploadingLineLosses")]
        [HttpPost]
        public async Task<IActionResult> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> set)
        {
            try
            {
                var data = await _dgrBs.InsertMonthlyUploadingLineLosses(set);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Route("InsertWindJMR")]
        [HttpPost]
        public async Task<IActionResult> InsertWindJMR(List<WindMonthlyJMR> windMonthlyJMR)
        {
            try
            {
                var data = await _dgrBs.InsertWindJMR(windMonthlyJMR);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("InsertWindDailyLoadShedding")]
        [HttpPost]
        public async Task<IActionResult> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> set)
        {
            try
            {
                var data = await _dgrBs.InsertWindDailyLoadShedding(set);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertWindTMLData
        [Route("InsertWindTMLData")]
        [HttpPost]
        public async Task<IActionResult> InsertWindTMLData(List<InsertWindTMLData> InsertWindTMLData, int type)
        {
            try
            {
                var data = await _dgrBs.InsertWindTMLData(InsertWindTMLData, type);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertWindPowerCurve
        [Route("InsertWindPowerCurve")]
        [HttpPost]
        public async Task<IActionResult> InsertWindPowerCurve(List<InsertWindPowerCurve> InsertWindPowerCurve)
        {
            try
            {
                var data = await _dgrBs.InsertWindPowerCurve(InsertWindPowerCurve);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //InsertWindBDCodeGamesa
        [Route("InsertWindBDCodeGamesa")]
        [HttpPost]
        public async Task<IActionResult> InsertWindBDCodeGamesa(List<InsertWindBDCodeGamesa> InsertWindBDCodeGamesa)
        {
            try
            {
                var data = await _dgrBs.InsertWindBDCodeGamesa(InsertWindBDCodeGamesa);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //ImportWindBDCodeINOX
        [Route("ImportWindBDCodeINOX")]
        [HttpPost]
        public async Task<IActionResult> ImportWindBDCodeINOX(List<ImportWindBDCodeINOX> ImportWindBDCodeINOX)
        {
            try
            {
                var data = await _dgrBs.ImportWindBDCodeINOX(ImportWindBDCodeINOX);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertWindBDCodeREGEN
        [Route("InsertWindBDCodeREGEN")]
        [HttpPost]
        public async Task<IActionResult> InsertWindBDCodeREGEN(List<InsertWindBDCodeREGEN> InsertWindBDCodeREGEN)
        {
            try
            {
                var data = await _dgrBs.InsertWindBDCodeREGEN(InsertWindBDCodeREGEN);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //GetWindBdCodeINOX
        [Route("GetWindBdCodeINOX")]
        [HttpPost]
        public async Task<IActionResult> GetWindBdCodeINOX(int site_id)
        {
            try
            {
                var data = await _dgrBs.GetWindBdCodeINOX(site_id);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertWindSpeedTMD
        [Route("InsertWindSpeedTMD")]
        [HttpPost]
        public async Task<IActionResult> InsertWindSpeedTMD(List<InsertWindSpeedTMD> InsertWindSpeedTMD)
        {
            try
            {
                var data = await _dgrBs.InsertWindSpeedTMD(InsertWindSpeedTMD);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //ImportWindReferenceWtgs
        [Route("ImportWindReferenceWtgs")]
        [HttpPost]
        public async Task<IActionResult> ImportWindReferenceWtgs(List<ImportWindReferenceWtgs> ImportWindReferenceWtgs)
        {
            try
            {
                var data = await _dgrBs.ImportWindReferenceWtgs(ImportWindReferenceWtgs);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        /*[Route("InsertDailyJMR")]
         [HttpPost]
         public async Task<IActionResult> InsertDailyJMR(List<WindDailyJMR> windDailyJMR)
         {
             try
             {
                 var data = await _dgrBs.InsertDailyJMR(windDailyJMR);
                 return Ok(data);

             }
             catch (Exception ex)
             {

                 return BadRequest(ex.Message);
             }
         }*/

        [Route("InsertSolarDailyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> solarDailyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyTargetKPI(solarDailyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarMonthlyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> solarMonthlyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertSolarMonthlyTargetKPI(solarMonthlyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarMonthlyUploadingLineLosses")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> solarMonthlyUploadingLineLosses)
        {
            try
            {
                var data = await _dgrBs.InsertSolarMonthlyUploadingLineLosses(solarMonthlyUploadingLineLosses);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarJMR")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarJMR(List<SolarMonthlyJMR> set)
        {
            try
            {
                var data = await _dgrBs.InsertSolarJMR(set);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarDailyLoadShedding")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> solarDailyLoadShedding)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyLoadShedding(solarDailyLoadShedding);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarInvAcDcCapacity")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> solarInvAcDcCapacity)
        {
            try
            {
                var data = await _dgrBs.InsertSolarInvAcDcCapacity(solarInvAcDcCapacity);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarTrackerLoss")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarTrackerLoss(List<InsertSolarTrackerLoss> InsertSolarTrackerLoss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarTrackerLoss(InsertSolarTrackerLoss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertSolarTrackerLossMonthly
        [Route("InsertSolarTrackerLossMonthly")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarTrackerLossMonthly(List<InsertSolarTrackerLoss> InsertSolarTrackerLoss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarTrackerLossMonthly(InsertSolarTrackerLoss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //CalculateTrackerLosses
        //[Route("CalculateTrackerLosses")]
        //[HttpPost]
        //public async Task<IActionResult> CalculateTrackerLosses(List<InsertSolarTrackerLoss> InsertSolarTrackerLoss)
        //{
        //    try
        //    {
        //        var data = await _dgrBs.CalculateTrackerLosses(InsertSolarTrackerLoss);
        //        return Ok(data);

        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.Message);
        //    }
        //}

        //InsertSolarSoilingLoss
        [Route("InsertSolarSoilingLoss")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarSoilingLoss(List<InsertSolarSoilingLoss> InsertSolarSoilingLoss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarSoilingLoss(InsertSolarSoilingLoss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertSolarPVSystLoss
        [Route("InsertSolarPVSystLoss")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarPVSystLoss(List<InsertSolarPVSystLoss> InsertSolarPVSystLoss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarPVSystLoss(InsertSolarPVSystLoss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertSolarEstimatedHourlyData
        [Route("InsertSolarEstimatedHourlyData")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarEstimatedHourlyData(List<InsertSolarEstimatedHourlyData> InsertSolarEstimatedHourlyData)
        {
            try
            {
                var data = await _dgrBs.InsertSolarEstimatedHourlyData(InsertSolarEstimatedHourlyData);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //InsertWindTMR
        [Route("InsertWindTMR")]
        [HttpPost]
        public async Task<IActionResult> InsertWindTMR(List<InsertWindTMR> InsertWindTMR)
        {
            try
            {
                var data = await _dgrBs.InsertWindTMR(InsertWindTMR);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarDailyBDloss")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyBDloss(List<SolarDailyBDloss> solarDailyBDloss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyBDloss(solarDailyBDloss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingPyranoMeter1Min")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingPyranoMeter1Min(set, batchId);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingPyranoMeter15Min")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingPyranoMeter15Min(set, batchId);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingFileGeneration")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingFileGeneration(List<SolarUploadingFileGeneration> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingFileGeneration(set, batchId);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
       
        [Route("InsertSolarUploadingFileBreakDown")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingFileBreakDown(set, batchId);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("importMetaData")]
        [HttpPost]
        public async Task<IActionResult> importMetaData(ImportBatch meta, string userName, int userId)
        {
            try
            {
                var data = await _dgrBs.importMetaData(meta, userName, userId);
                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("GetBatchStatus")]
        [HttpGet]
        public async Task<IActionResult> GetBatchStatus(int import_type, int site_id, string import_date)
        {
            try
            {
                var data = await _dgrBs.GetBatchStatus(import_type, site_id, import_date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }



        [Route("GetBatchId")]
        [HttpGet]
        public async Task<IActionResult> GetBatchId(string logFileName)
        {
            try
            {
                var data = await _dgrBs.GetBatchId(logFileName);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteRecordsAfterFailure")]
        [HttpGet]
        public async Task<IActionResult> DeleteRecordsAfterFailure(int batchId, int siteType)
        {
            try
            {
                var data = await _dgrBs.DeleteRecordsAfterFailure(batchId, siteType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarInverterFromdailyGenSummary/{state}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarInverterFromdailyGenSummary(string state, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarInverterFromdailyGenSummary(state, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarMonthlyGenSummaryReport1")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyGenSummaryReport1(string fy, string month, string country, string state, string spv, string site, string inverter)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyGenSummaryReport1(fy, month, country, state, spv, site, inverter);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarMonthlyGenSummaryReport2")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyGenSummaryReport2(string fy, string month, string country, string state, string spv, string site, string inverter)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyGenSummaryReport2(fy, month, country, state, spv, site, inverter);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarYearlyGenSummaryReport1")]
        [HttpGet]
        public async Task<IActionResult> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarYearlyGenSummaryReport2")]
        [HttpGet]
        public async Task<IActionResult> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }




        [Route("GetSolarPerformanceReportBySiteWise")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate,string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceReportBySiteWise(fy, fromDate, todate,site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarPerformanceReportBySPVWise")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate,string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceReportBySPVWise(fy, fromDate, todate,site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertWindUploadingFileGeneration")]
        [HttpPost]
        public async Task<IActionResult> InsertWindUploadingFileGeneration(List<WindUploadingFileGeneration> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertWindUploadingFileGeneration(set, batchId);
                return Ok(data);
                //Console.WriteLine("Entering wind file generation while debugging");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("InsertWindUploadingFileBreakDown")]
        [HttpPost]
        public async Task<IActionResult> InsertWindUploadingFileBreakDown(List<WindUploadingFileBreakDown> set, int batchId)
        {
            try
            {
                var data = await _dgrBs.InsertWindUploadingFileBreakDown(set, batchId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryData(string fromDate, string toDate)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarDailyGenSummary(fromDate, toDate);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        //[Route("GetWindDailyTargetKPI/{fromDate}/{todate}")]
        [Route("GetWindDailyTargetKPI")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyTargetKPI(string site, string fromDate, string todate)
        {

            try
            {
                var data = await _dgrBs.GetWindDailyTargetKPI(site, fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("GetSolarDailyTargetKPI")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyTargetKPI(string fromDate, string toDate, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyTargetKPI(fromDate, toDate, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetWindMonthlyTargetKPI/{fy}/{month}")
        [Route("GetWindMonthlyTargetKPI")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyTargetKPI(string site, string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyTargetKPI(site, fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyTargetKPI")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyTargetKPI(string fy, string month, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyTargetKPI(fy, month, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetWindMonthlyLineLoss/{fy}/{month}")]
        [Route("GetWindMonthlyLineLoss")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyLineLoss(string site, string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyLineLoss(site, fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyLineLoss")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyLineLoss(string fy, string month, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyLineLoss(fy, month, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //[Route("GetWindMonthlyJMR/{fy}/{month}")]
        [Route("GetWindMonthlyJMR")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyJMR(string site, string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyJMR(site, fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyJMR")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyJMR(string fy, string month, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyJMR(fy, month, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarACDCCapacity")]
        [HttpGet]
        public async Task<IActionResult> GetSolarACDCCapacity(string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarACDCCapacity(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetUserManagement/{userMail}/{date}")]
        [HttpGet]
        public async Task<IActionResult> GetUserManagement(string userMail, string date)
        {
            try
            {
                var data = await _dgrBs.GetUserManagement(userMail, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetBDType")]
        [HttpGet]
        public async Task<IActionResult> GetBDType()
        {
            try
            {
                var data = await _dgrBs.GetBDType();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDailyloadShedding/{site}/{fromDate}/{toDate}")]
        [Route("GetWindDailyloadShedding")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyloadShedding(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetSolarDailyloadShedding/{site}/{fromDate}/{toDate}")]
        [Route("GetSolarDailyloadShedding")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyloadShedding(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyGenSummaryPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        
        [Route("GetWindDailyBreakdownPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyBreakdownPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyBreakdownPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyBreakdownPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyBreakdownPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyBreakdownPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateWindDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.UpdateWindDailyGenSummaryApproveStatus(dailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateWindDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                var data = await _dgrBs.UpdateWindDailyBreakdownApproveStatus(windDailyBreakdownReport);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteWindDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.DeleteWindDailyGenSummaryApproveStatus(dailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteWindDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                var data = await _dgrBs.DeleteWindDailyBreakdownApproveStatus(windDailyBreakdownReport);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("UpdateSolarDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.UpdateSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("DeleteSolarDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.DeleteSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateSolarDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                var data = await _dgrBs.UpdateSolarDailyBreakdownApproveStatus(solarFileBreakdown);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("DeleteSolarDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                var data = await _dgrBs.DeleteSolarDailyBreakdownApproveStatus(solarFileBreakdown);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetImportBatches")]
        [HttpGet]
        public async Task<IActionResult> GetImportBatches(string importFromDate, string importToDate, string siteId, int importType, int status,int userid)
        {
            {
                try
                {
                    var data = await _dgrBs.GetImportBatches(importFromDate, importToDate, siteId, importType, status, userid);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SetApprovalFlagForImportBatches")]
        [HttpGet]
        public async Task<IActionResult> SetApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName,int status)
        {
            {
                try
                {
                    var data = await _dgrBs.SetApprovalFlagForImportBatches(dataId, approvedBy, approvedByName, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SetRejectFlagForImportBatches")]
        [HttpGet]
        public async Task<IActionResult> SetRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status)
        {
            {
                try
                {
                    var data = await _dgrBs.SetRejectFlagForImportBatches(dataId, rejectedBy, rejectByName, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SetSolarApprovalFlagForImportBatches")]
        [HttpGet]
        public async Task<IActionResult> SetSolarApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status)
        {
            {
                try
                {
                    var data = await _dgrBs.SetSolarApprovalFlagForImportBatches(dataId, approvedBy, approvedByName, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SetSolarRejectFlagForImportBatches")]
        [HttpGet]
        public async Task<IActionResult> SetSolarRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status)
        {
            {
                try
                {
                    var data = await _dgrBs.SetSolarRejectFlagForImportBatches(dataId, rejectedBy, rejectByName, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetCountryList")]
        [HttpGet]
        public async Task<IActionResult> GetCountryList()
        {
            {
                try
                {
                    var data = await _dgrBs.GetCountryList();
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
       [Route("GetStateList")]
        [HttpGet]
        public async Task<IActionResult> GetStateList(string country,string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetStateList(country,site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetStateListSolar")]
        [HttpGet]
        public async Task<IActionResult> GetStateListSolar(string country,string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetStateListSolar(country,site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSPVList")]
        [HttpGet]
        public async Task<IActionResult> GetSPVList(string state,string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSPVList(state, site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSPVSolarList")]
        [HttpGet]
        public async Task<IActionResult> GetSPVSolarList(string state,string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSPVListSolar(state,site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSiteList")]
        [HttpGet]
        public async Task<IActionResult> GetSiteList(string state, string spvdata,string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSiteList(state, spvdata, site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSiteListSolar")]
        [HttpGet]
        public async Task<IActionResult> GetSiteListSolar(string state, string spvdata, string site)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSiteListSolar(state, spvdata, site);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetWTGList")]
        [HttpGet]
        public async Task<IActionResult> GetWTGList(string siteid, string state, string spv )
        {
            {
                try
                {
                    var data = await _dgrBs.GetWTGList(siteid,state,spv);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetInvList")]
        [HttpGet]
        public async Task<IActionResult> GetInvList(string siteid, string state, string spv)
        {
            {
                try
                {
                    var data = await _dgrBs.GetInvList(siteid,  state, spv);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        // Get Generation imported Data
        [Route("GetImportGenData")]
        [HttpGet]
        public async Task<IActionResult> GetImportGenData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetImportGenData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetBrekdownImportData")]
        [HttpGet]
        public async Task<IActionResult> GetBrekdownImportData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetBrekdownImportData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        // Get Solar Generation imported Data
        [Route("GetSolarImportGenData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarImportGenData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarImportGenData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSolarBrekdownImportData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarBrekdownImportData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarBrekdownImportData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSolarP1ImportData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarP1ImportData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarP1ImportData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSolarP15ImportData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarP15ImportData(int importId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarP15ImportData(importId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetOperationHeadData")]
        [HttpGet]
        public async Task<IActionResult> GetOperationHeadData(string site,string spv)
        {
            {
                try
                {
                    var data = await _dgrBs.GetOperationHeadData(site,spv);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
		[Route("GetSolarOperationHeadData")]
        [HttpGet]
        public async Task<IActionResult> GetSolarOperationHeadData(string site,string spv)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarOperationHeadData(site,spv);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("DeleteWindSite")]
        [HttpGet]
        public async Task<IActionResult> DeleteWindSite(int siteid)
        {
            {
                try
                {
                    var data = await _dgrBs.DeleteWindSite(siteid);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("DeleteSolarSite")]
        [HttpGet]
        public async Task<IActionResult> DeleteSolarSite(int siteid)
        {
            {
                try
                {
                    var data = await _dgrBs.DeleteSolarSite(siteid);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetWindMajorBreakdown")]
        [HttpGet]
        public async Task<IActionResult> GetWindMajorBreakdown(string fromDate, string toDate,string site,string spv)
        {
            try
            {
                var data = await _dgrBs.GetWindMajorBreakdown(fromDate, toDate,site,spv);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetTotalMWforDashbord")]
        [HttpGet]
        public async Task<IActionResult> GetTotalMWforDashbord(string w_site, string s_site)
        {
            try
            {
                var data = await _dgrBs.GetTotalMWforDashbord(w_site, s_site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("MailSend")]
        [HttpGet]
        public async Task<IActionResult> MailSend(string fname)
        {
            try
            {
                var data = await _dgrBs.MailSend(fname);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
		/*[Route("GetActualVSExpected")]
        [HttpGet]
        public async Task<IActionResult> GetActualVSExpected(string fromDate, string toDate, string spv,string site,string pr)
        {
            try
            {
                var data = await _dgrBs.GetActualVSExpected(fromDate, toDate, spv, site, pr);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetActualVSExpectedYearly")]
        public async Task<IActionResult> GetActualVSExpectedYearly(string fromDate, string toDate, string spv, string site, string prType)
        {
            try
            {
                var data = await _dgrBs.GetActualVSExpectedYearly(fromDate, toDate, spv, site, prType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }*/
        [Route("PPTCreate")]
        [HttpGet]
        public async Task<IActionResult> PPTCreate()
        {
            try
            {
                var data = await _dgrBs.PPTCreate();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("PPTCreate_Solar")]
        [HttpGet]
        public async Task<IActionResult> PPTCreate_Solar()
        {
            try
            {
                var data = await _dgrBs.PPTCreate_Solar();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("eQry/{qry}")]
        [HttpGet]
        public async Task<IActionResult> eQry(string qry)
        {
            try
            {
                var data = await _dgrBs.eQry(qry);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("BulkMapBdToTML")]
        [HttpGet]
        public async Task<IActionResult> BulkMapBdToTML(string fromDate, string toDate)
        {
            {
                try
                {
                    var data = await _dgrBs.BulkMapBdToTML(fromDate, toDate);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.ToString());
                }
            }
        }

        //DGR Version 3.
        //GetWindTMLGraphData
        [Route("GetHeatMapData")]
        [HttpGet]
        public async Task<IActionResult> GetHeatMapData(string site, string fromDate, string toDate, int isAdmin, int siteType)
        {
            try
            {
                var data = await _dgrBs.GetHeatMapData(site, fromDate, toDate, isAdmin, siteType);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("OPGetSiteListForEdit")]
        [HttpGet]
        public async Task<IActionResult> OPGetSiteListForEdit(string month_no, string year, int siteType, int bdTypes, int isMonthly)
        {
            {
                try
                {
                    var data = await _dgrBs.OPGetSiteListForEdit(month_no, year, siteType, bdTypes, isMonthly);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("OPGetSpvListForEdit")]
        [HttpGet]
        public async Task<IActionResult> OPGetSpvListForEdit(string month_no, string year, int siteType, int bdTypes, int isMonthly)
        {
            {
                try
                {
                    var data = await _dgrBs.OPGetSpvListForEdit(month_no, year, siteType, bdTypes, isMonthly);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("OPCommentsInsert")]
        [HttpPost]
        public async Task<IActionResult> OPCommentsInsert(List<OPComments> set)
        {
            {
                try
                {
                    var data = await _dgrBs.OPCommentsInsert(set);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        //Task<List<OPComments>> GetOPCommentsMonthly(string site_id, int month_no, int year, string spv, int siteType)
        [Route("GetOPComments")]
        [HttpGet]
        public async Task<IActionResult> GetOPComments(string site_id, int month_no, int year, string spv, int siteType, int isSPV, int bdType, int isDisplay, int isMonthly)
        {
            {
                try
                {
                    var data = await _dgrBs.GetOPComments(site_id, month_no, year, spv, siteType, isSPV, bdType, isDisplay, isMonthly);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("OPCommentsEdit")]
        [HttpPost]
        public async Task<IActionResult> OPCommentsEdit(List<OPComments> set)
        {
            {
                try
                {
                    var data = await _dgrBs.OPCommentsEdit(set);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("OPCommentsDelete")]
        [HttpPost]
        public async Task<IActionResult> OPCommentsDelete(List<OPComments> set)
        {
            {
                try
                {
                    var data = await _dgrBs.OPCommentsDelete(set);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("CalculateDailyExpected")]
        [HttpGet]
        public async Task<IActionResult> CalculateDailyExpected(string site, string data_date)
        {
            try
            {
                var data = await _dgrBs.CalculateDailyExpected(site, data_date);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("CalculateDailyExpectedSolar")]
        [HttpGet]
        public async Task<IActionResult> CalculateDailyExpectedSolar(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.CalculateDailyExpectedSolar(site, fromDate, toDate);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("BulkCalculateDailyWindExpected")]
        [HttpGet]
        //Task<List<ExpectedResult>> BulkCalculateDailyWindExpected(string site, string fromDate, string toDate)
        public async Task<IActionResult> BulkCalculateDailyWindExpected(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.BulkCalculateDailyWindExpected(site, fromDate, toDate);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("BulkCalculateDailySolarExpected")]
        [HttpGet]
        //Task<List<ExpectedResult>> BulkCalculateDailyWindExpected(string site, string fromDate, string toDate)
        public async Task<IActionResult> BulkCalculateDailySolarExpected(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.BulkCalculateDailySolarExpected(site, fromDate, toDate);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //DGR_v3 Email_report changes
        [Route("dgrUploadingReminder")]
        [HttpGet]
        public async Task<IActionResult> dgrUploadingReminder()
        {
            try
            {
                var data = await _dgrBs.dgrUploadingReminder();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // Operation performance ppt creation for spv 
        [Route("GetGroupBySPVSite")]
        [HttpGet]
        public async Task<IActionResult> getGroupBySPV(int siteType)
        {
            try
            {
                var data = await _dgrBs.getGroupBySPV(siteType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("WindReviewMail")]
        [HttpGet]
        public async Task<IActionResult> WindReviewMail()
        {
            try
            {
                var data = await _dgrBs.WindReviewMail();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SolarReviewMail")]
        [HttpGet]
        public async Task<IActionResult> SolarReviewMail()
        {
            try
            {
                var data = await _dgrBs.SolarReviewMail();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("MonthlyMailSend")]
        [HttpGet]
        public async Task<IActionResult> MonthlyMailSend(string fname)
        {
            try
            {
                var data = await _dgrBs.MonthlyMailSend(fname);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindPerformanceGroupBySite")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceGroupBySite(string fy, string fromDate, string todate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceGroupBySite(fy, fromDate, todate, site_list);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarPerformanceGroupBySite")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceGroupBySite(string fy, string fromDate, string todate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceGroupBySite(fy, fromDate, todate, site_list);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomGroup")]
        [HttpGet]
        public async Task<IActionResult> GetCustomGroup(int siteType)
        {
            try
            {
                var data = await _dgrBs.GetCustomGroup(siteType);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeSolarDaily")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeSolarDaily(string fromDate, string toDate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeSolarDaily(fromDate, toDate, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeSolarMonthly")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeSolarMonthly(string fy, string month, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeSolarMonthly(fy, month, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeSolarYearly")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeSolarYearly(string fromDate, string toDate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeSolarYearly(fromDate, toDate, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeWindDaily")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeWindDaily(string fromDate, string toDate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeWindDaily(fromDate, toDate, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeWindMonthly")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeWindMonthly(string fy, string month, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeWindMonthly(fy, month, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomeWindYearly")]
        [HttpGet]
        public async Task<IActionResult> GetCustomeWindYearly(string fromDate, string toDate, string site_list)
        {
            try
            {
                var data = await _dgrBs.GetCustomeWindYearly(fromDate, toDate, site_list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DGR_v3 COLUMN ACCESS CODE START

        [Route("GetPageList")]
        [HttpGet]
        public async Task<IActionResult> GetPageList(int type, int pageType)
        {
            {
                try
                {
                    var data = await _dgrBs.GetPageList(type, pageType);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetGroupList_CA")]
        [HttpGet]
        public async Task<IActionResult> GetGroupList_CA(int page_id)
        {
            {
                try
                {
                    var data = await _dgrBs.GetGroupList_CA(page_id);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetCGColumns_CA")]
        [HttpGet]
        public async Task<IActionResult> GetCGColumns_CA(int page_id)
        {
            {
                try
                {
                    var data = await _dgrBs.GetCGColumns_CA(page_id);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("CreateGroup_CA")]
        [HttpPost]
        public async Task<IActionResult> CreateGroup_CA(List<CreateGroupData> set, int page_id, string group_name)
        {
            {
                try
                {
                    var data = await _dgrBs.CreateGroup_CA(set, page_id, group_name);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetPageColumns")]
        [HttpGet]
        public async Task<IActionResult> GetPageColumns(int page_id)
        {
            {
                try
                {
                    var data = await _dgrBs.GetPageColumns(page_id);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetUserGroupColumns")]
        [HttpGet]
        public async Task<IActionResult> GetUserGroupColumns(int page_id, int userId)
        {
            {
                try
                {
                    var data = await _dgrBs.GetUserGroupColumns(page_id, userId);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("UpdateGroup_CA")]
        [HttpPost]
        public async Task<IActionResult> UpdateGroup_CA(int[] set, int page_id, int page_groups_id)
        {
            {
                try
                {
                    var data = await _dgrBs.UpdateGroup_CA(set, page_id, page_groups_id);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("ActiDeactiGroup_CA")]
        [HttpGet]
        public async Task<IActionResult> ActiDeactiGroup_CA(int page_groups_id, int status)
        {
            {
                try
                {
                    var data = await _dgrBs.ActiDeactiGroup_CA(page_groups_id, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetUserAssignedGroups")]
        [HttpGet]
        public async Task<IActionResult> GetUserAssignedGroups(int user_id)
        {
            {
                try
                {
                    var data = await _dgrBs.GetUserAssignedGroups(user_id);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        //COLUMN ACCESS CODE END
        [Route("GetFormula")]
        [HttpGet]
        public async Task<IActionResult> GetFormula(int site_id, string site_type)
        {
            try
            {
                var data = await _dgrBs.GetFormula(site_id, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SaveFormula")]
        [HttpGet]
        public async Task<IActionResult> SaveFormula(int id, int site_id, string formulas, int login_id, string fieldType, string oldFormulas)
        {
            try
            {
                var data = await _dgrBs.SaveFormula(id, site_id, formulas, login_id, fieldType, oldFormulas);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCalculatedValue")]
        [HttpGet]
        public async Task<IActionResult> GetCalculatedValue(double U, double S, double IG, double EG, double OTHER, double LS, string updateFormulaValue)
        {
            try
            {
                var data = await _dgrBs.GetCalculatedValue(U, S, IG, EG, OTHER, LS, updateFormulaValue);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetFormulaLog")]
        [HttpGet]
        public async Task<IActionResult> GetFormulaLog(int site_id)
        {
            try
            {
                var data = await _dgrBs.GetFormulaLog(site_id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
