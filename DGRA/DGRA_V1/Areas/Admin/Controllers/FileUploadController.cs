using DGRA_V1.Common;
using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DGRA_V1.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace DGRA_V1.Areas.admin.Controllers
{
    [Area("admin")]
    [AllowAnonymous]
    [ServiceFilter(typeof(SessionValidation))]
    [TypeFilter(typeof(SessionValidation))]
    public class FileUploadController : Controller
    {
        ImportBatch objImportBatch = new ImportBatch();
        private IDapperRepository _idapperRepo;
        private IWebHostEnvironment env;
        private static IHttpContextAccessor HttpContextAccessor;
        //        CultureInfo timeCulture = CultureInfo.InvariantCulture;
        public FileUploadController(IDapperRepository idapperobj, IWebHostEnvironment iwebhostobj, IHttpContextAccessor httpObj)
        {
            HttpContextAccessor = httpObj;
            _idapperRepo = idapperobj;
            m_ErrorLog = new ErrorLog(iwebhostobj);
            env = iwebhostobj;
        }
        int user_id = 0;
        int batchIdDGRAutomation = 0;
        string siteUserRole;
        int previousSite = 0;
        int isInox = 0;
        int isSuzlon = 0;
        int isRegen = 0;
        int TMLType = 0;
        int isMultiSheet = 0;
        int tmlMultiError = 0;
        string[] importData = new string[2];
        string generationDate = "";
        bool isGenValidationSuccess = false;
        bool isBreakdownValidationSuccess = false;
        bool isPyro1ValidationSuccess = false;
        bool isPyro15ValidationSuccess = false;
        bool isTrackerLossvalidationSuccess = false;
        bool isGenSheet = false;
        bool isBdSheet = false;
        bool isPyrano1Min = false;
        bool isPyrano15Min = false;
        bool isTracker = false;
        string genJson = string.Empty;
        string breakJson = string.Empty;
        string pyro1Json = string.Empty;
        string pyro15Json = string.Empty;
        string trackerJson = string.Empty;
        string windGenJson = string.Empty;
        string windBreakJson = string.Empty;
        ArrayList kpiArgs = new ArrayList();
        List<int> windSiteUserAccess = new List<int>();
        List<int> solarSiteUserAccess = new List<int>();
        List<string> fileSheets = new List<string>();
        List<string> inverterList = new List<string>();
        List<string> icrList = new List<string>();
        List<string> invList = new List<string>();
        List<string> IGBD = new List<string>();
        List<string> SMBList = new List<string>();
        List<string> StringsList = new List<string>();
        List<InsertWindTMLData> TMLDataSet = new List<InsertWindTMLData>();


        ErrorLog m_ErrorLog;

        //Dynamic hash Tables required for getting important data in accordance to key.
        Hashtable equipmentId = new Hashtable();
        Hashtable onm2equipmentName = new Hashtable();
        Hashtable SiteByWtg = new Hashtable();
        Hashtable maxkWhMap_wind = new Hashtable();
        Hashtable breakdownType = new Hashtable();//(B)Gets bdTypeID from bdTypeName: BDType table
        Hashtable siteNameId = new Hashtable(); //(C)Gets siteId from siteName
        Hashtable siteName = new Hashtable(); //(D)Gets siteName from siteId
        Hashtable eqSiteId = new Hashtable();//(E)Gets siteId from (wtg)equipmentName

        //Static Hash Tables required for getting data in accordance to keys.
        Hashtable MonthList = new Hashtable() { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
        Hashtable longMonthList = new Hashtable() { { "January", 1 }, { "February", 2 }, { "March", 3 }, { "April", 4 }, { "May", 5 }, { "June", 6 }, { "July", 7 }, { "August", 8 }, { "September", 9 }, { "October", 10 }, { "November", 11 }, { "December", 12 } };


        /*~FileUploadController()
        {
            //Destructor
            //delete m_ValidationObject;
            //delete m_ErrorLog;
        }*/

        [HttpGet]
        public ActionResult Upload()
        {
            TempData["notification"] = "";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Upload(string FileUpload)
        {
            user_id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            try
            {
                //  string response = await ExceldatareaderAndUpload(Request.Files["Path"]);
                string finalResponse = "";
                int fileCount = HttpContext.Request.Form.Files.Count;
                bool isMultiFiles = false;
                if(fileCount > 1)
                {
                    isMultiFiles = true;
                }
                for (int i = 0; i < fileCount; i++)
                {
                    //Clearing hashtables and list before importing new file.
                    equipmentId.Clear();
                    onm2equipmentName.Clear();
                    SiteByWtg.Clear();
                    maxkWhMap_wind.Clear();
                    breakdownType.Clear();
                    siteNameId.Clear();
                    siteName.Clear();
                    eqSiteId.Clear();
                    finalResponse = "";
                    fileSheets.Clear();
                    m_ErrorLog.SetInformation(",File Name : " + HttpContext.Request.Form.Files[i].FileName);
                    string response = await ExcelDataReaderAndUpload(HttpContext.Request.Form.Files[i], FileUpload, isMultiFiles);
                    finalResponse = response;
                }
                if (isMultiFiles)
                {
                    try
                    {
                        var json = JsonConvert.SerializeObject(TMLDataSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                        //***********Here sending dynamic type is remaining, as we have to test the performance. did for Inox multiple imports.*************
                        //insertWindTMLData type = 1 : Gamesa ; type = 2 : INOX ; type = 3 : Suzlon.
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=" + TMLType;
                        using (var client = new HttpClient())
                        {
                            client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                            var response = await client.PostAsync(url, data);
                            string returnResponse = response.Content.ReadAsStringAsync().Result;
                            if (response.IsSuccessStatusCode)
                            {
                                if (returnResponse == "5")
                                {
                                    m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                                }
                                else
                                {
                                    m_ErrorLog.SetError(",Error in Calculation.");
                                }
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Wind TMR API Failure,: responseCode <" + (int)response.StatusCode + ">");

                                //for solar 0, wind 1, other 2;
                                int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                                if (deleteStatus == 1)
                                {
                                    m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                }
                                else if (deleteStatus == 0)
                                {
                                    m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                }
                                else
                                {
                                    m_ErrorLog.SetInformation(", File not uploaded");
                                }
                            }
                        }
                        finalResponse += "Information : Calculation and storing of data successful. ";
                    }
                    catch (Exception e)
                    {
                        string msg = "Exception while sending data to api of TML for multiple files, due to : " + e.ToString();
                        finalResponse += "Exception while sending data to api of TML for multiple files, due to : " + e.Message + ", ";
                    }
                }
                TempData["notification"] = finalResponse;
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Failed to upload";
                string message = ex.Message;
                m_ErrorLog.SetError(",Failed to upload {fileUploadType},");
                string msg = "Failed to upload <" + FileUpload + ">  File <" + HttpContext.Request.Form.Files[0] + "> Reason : " + ex.ToString();
                //ErrorLog(msg);
                LogError(user_id, 0,4,"Upload", msg);

            }
            return View();
        }

        public async Task<string> ExcelDataReaderAndUpload(IFormFile file, string fileUploadType, bool isMultiFiles)
        {
            var usermodel = JsonConvert.DeserializeObject<UserAccess>(@HttpContextAccessor.HttpContext.Session.GetString("UserAccess"));
            //var UserName = JsonConvert.DeserializeObject<UserAccess>(@HttpContextAccessor.HttpContext.Session.GetString("UserName"));
            var UserName = " ";
            //m_ErrorLog.SetImportInformation("" + UserName + " ,");

            for (int i = 0; i < usermodel.access_list.Count; i++)
            {
                if (usermodel.access_list[i].page_type == 3 && usermodel.access_list[i].site_type == 1)
                {
                    windSiteUserAccess.Add(Convert.ToInt32(usermodel.access_list[i].identity));
                }
                if (usermodel.access_list[i].page_type == 3 && usermodel.access_list[i].site_type == 2)
                {
                    solarSiteUserAccess.Add(Convert.ToInt32(usermodel.access_list[i].identity));
                }
            }
            //windSiteList = HttpContextAccessor.HttpContext.Session.GetString("UserAccess");
            siteUserRole = HttpContext.Session.GetString("role");
            user_id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            m_ErrorLog.SetImportInformation("" + siteUserRole + " ,");
            DateTime today = DateTime.Now;
            m_ErrorLog.SetImportInformation("" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ",");
            m_ErrorLog.SetImportInformation("" + file.FileName + " ,");
            m_ErrorLog.SetImportInformation("" + fileUploadType + " ,");
            //InformationLog("UserRole, fileName, fileUploadType, time :" +siteUserRole +", " + file.FileName+ ", " + fileUploadType + ", " + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss"));

            // string csvFileName = env.ContentRootPath +@"\LogFile\"+ file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            string csvFileName = file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            /*int isGamesa = 0;
            if(file.FileName == "Gamesa.xlsx")
            {
                isGamesa = 1;
            }*/

            if (file.FileName.ToString().StartsWith("TenMinLog"))
            {
                isInox = 1;
                //fileSheets.Add("Sheet1");
            }
            if (file.FileName.ToString().ToLower().StartsWith("zhb") || file.FileName.ToString().ToLower().StartsWith("kd") || file.FileName.ToString().ToLower().StartsWith("vl333") || file.FileName.ToString().ToLower().StartsWith("vl20") || file.FileName.ToString().ToLower().StartsWith("m24") || file.FileName.ToString().ToLower().StartsWith("m32"))
            {
                isSuzlon = 1;
            }
            importData[0] = fileUploadType;
            importData[1] = csvFileName;
            string status = "";
            int statusCode = 400;   //Bad request
            DataSet dataSetMain = new DataSet();
            var allowedExtensions = new[] { ".xlsx", ".xls", ".csv", ".txt" };
            var ext = Path.GetExtension(file.FileName);

            if (allowedExtensions.Contains(ext))
            {
                try
                {
                    /* if (!Directory.Exists(env.ContentRootPath + @"\TempFile"))
                     {
                         DirectoryInfo dinfo = Directory.CreateDirectory(env.ContentRootPath + @"\TempFile");
                     }
                     else
                     {
                         string[] filePaths = Directory.GetFiles(env.ContentRootPath + @"\TempFile");
                         if (filePaths.Length > 0)
                         {
                             foreach (String path in filePaths)
                             {
                                 System.IO.File.Delete(path);
                             }
                         }
                     }*/
                    if (!Directory.Exists(@"\TempFile"))
                    {
                        DirectoryInfo dinfo = Directory.CreateDirectory(@"\TempFile");
                    }
                    else
                    {
                        string[] filePaths = Directory.GetFiles(@"\TempFile");
                        if (filePaths.Length > 0)
                        {
                            foreach (String path in filePaths)
                            {
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                else
                                {
                                    string msg = "Didn't find file at path:  " + path;
                                    //ErrorLog(msg);
                                    LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                                }
                            }
                        }
                    }
                    if (ext == ".xlsx") //Pending what about xls file.
                    {
                        try
                        {
                            //InformationLog("Inside try block as extension type is .xlsx");
                            /* using (var stream = new FileStream(env.ContentRootPath + @"\TempFile\docupload.xlsx", FileMode.Create))
                             {
                                 file.CopyTo(stream);
                             }*/
                            using (var stream = new FileStream(@"\TempFile\docupload.xlsx", FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            DataTable dt = null;

                            string _filePath = @"C:\TempFile\docupload.xlsx";
                            //string _filePath = @"G:\TempFile\docupload.xlsx";
                            //string _filePath = env.ContentRootPath + @"\TempFile\docupload.xlsx";
                            dataSetMain = GetDataTableFromExcel(_filePath, true, ref fileSheets);
                            if (dataSetMain == null)
                            {
                                m_ErrorLog.SetError(",Unable to extract excel sheet data for importing,");
                                string msg = "datSetMain null " + dataSetMain;
                                //ErrorLog(msg);
                                LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                            }


                            //masterHashtable_SiteName_To_SiteId();//C
                            masterHashtable_SiteIdToSiteName();
                            if (fileUploadType == "Wind")
                            {
                                masterHashtable_WtgToWtgId();
                                masterHashtable_WtgToSiteId();
                            }
                            //if (fileUploadType == "Solar")
                            //{
                            //    masterInverterList();
                            //}

                            if (fileSheets.Contains("Uploading_File_Breakdown"))
                            {
                                masterHashtable_BDNameToBDId();//B
                            }

                            if (fileSheets.Contains("TML_Data"))
                            {
                                var temp = 100;//B
                            }
                            if (fileUploadType == "Solar")
                            {
                                if (fileSheets.Contains("Uploading_File_Generation"))
                                {
                                    isGenSheet = true;
                                    isBdSheet = fileSheets.Contains("Uploading_File_Breakdown");
                                    isPyrano1Min = fileSheets.Contains("Uploading_PyranoMeter1Min");
                                    isPyrano15Min = fileSheets.Contains("Uploading_PyranoMeter15Min");
                                    isTracker = fileSheets.Contains("Solar_tracker_loss");
                                }
                            }

                            if ((fileSheets[0].ToString().StartsWith("GKK") || fileSheets[0].ToString().StartsWith("GA") || fileSheets[0].ToString().StartsWith("GS") || fileSheets[0].ToString().StartsWith("GG") || fileSheets[0].ToString().StartsWith("BD") || fileSheets[0].ToString().StartsWith("NEW") || fileSheets[0].ToString().StartsWith("NL") || fileSheets[0].ToString().StartsWith("GK") || fileSheets[0].ToString().StartsWith("GBR") || fileSheets[0].ToString().StartsWith("G114")) && fileSheets.Count > 1)
                            {
                                isMultiSheet = 1;
                            }

                                //Status Codes:
                                //200 = Success ; 400 = Failure(BadRequest)
                                FileSheetType.FileImportType fileImportType = FileSheetType.FileImportType.imporFileType_Invalid;
                            foreach (var excelSheet in fileSheets)
                            {
                                //InformationLog("Inside foreach loop line number : 238 : Excelsheet name :" +excelSheet);
                                DataSet ds = new DataSet();
                                if (excelSheet == FileSheetType.Uploading_File_Generation)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation(" DGR Automation ,");
                                        if (fileUploadType == "Solar")
                                        {
                                            if (isBdSheet)
                                            {
                                                if (isPyrano1Min)
                                                {
                                                    if (isPyrano15Min)
                                                    {
                                                        if (isTracker)
                                                        {
                                                            m_ErrorLog.SetInformation(",Importing Solar_Uploading_File_Generation WorkSheet:");
                                                            statusCode = await InsertSolarFileGeneration(status, ds);
                                                        }
                                                        else
                                                        {
                                                            m_ErrorLog.SetError(",Tracker Loss sheet is missing");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        m_ErrorLog.SetError(",Pyranometer 1 Minute sheet is missing");
                                                    }
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(",Pyranometer 15 Minutes sheet is missing");
                                                }
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Breakdown sheet is missing");
                                            }

                                        }
                                        else if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind_Uploading_File_Generation WorkSheet");
                                            statusCode = await InsertWindFileGeneration(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_File_Breakdown)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            if (isBdSheet && isPyrano1Min && isPyrano15Min && isTracker)
                                            {
                                                m_ErrorLog.SetInformation(",Importing Solar Uploading_File_Breakdown WorkSheet:");
                                                statusCode = await InsertSolarFileBreakDown(status, ds);
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Uploading_File_Breakdown WorkSheet:");
                                            statusCode = await InsertWindFileBreakDown(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter1Min)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            if (isBdSheet && isPyrano1Min && isPyrano15Min && isTracker)
                                            {
                                                m_ErrorLog.SetInformation(",Importing Solar_Uploading_PyranoMeter1Min: ");
                                                statusCode = await InsertSolarPyranoMeter1Min(status, ds);
                                            }
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for pyranometer import";
                                            m_ErrorLog.SetError("," + status);
                                            //ErrorLog(status);
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);

                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter15Min)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            if (isBdSheet && isPyrano1Min && isPyrano15Min && isTracker)
                                            {
                                                m_ErrorLog.SetInformation(",Importing Solar_Uploading_PyranoMeter15Min :");
                                                statusCode = await InsertSolarPyranoMeter15Min(status, ds);
                                            }
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for pyranometer import";
                                            m_ErrorLog.SetError("," + status);
                                            //ErrorLog(status);
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_JMR_Input_and_Output)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_JMR_Input_and_Output;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Monthly JMR Input and Output ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertWindMonthlyJMR(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertSolarMonthlyJMR(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_LineLoss)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_LineLoss;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Monthly Lineloss ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertWindMonthlyLineLoss(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertSolarMonthlyLineLoss(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_Target_KPI)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_Target_KPI;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Monthly Target KPI ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindMonthlyTargetKPI(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarMonthlyTargetKPI(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Load_Shedding)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Daily_Load_Shedding;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Daily Loadshedding ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertWindDailyLoadShedding(status, ds);

                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertSolarDailyLoadShedding(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Target_KPI)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Daily_Target_KPI;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Daily Target KPI ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindDailyTargetKPI(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarDailyTargetKPI(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Site_Master)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Site_Master;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Site Master ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Site_Master WorkSheet:");
                                            statusCode = await InsertWindSiteMaster(status, ds);
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Site_Master WorkSheet:");
                                            statusCode = await InsertSolarSiteMaster(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Location_Master)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Location_Master;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("Location Master ,");
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Wind Location_Master WorkSheet:");
                                            statusCode = await InsertWindLocationMaster(status, ds);
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar Location_Master WorkSheet:");
                                            statusCode = await InsertSolarLocationMaster(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_AC_DC_Capacity)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Solar_AC_DC_Capacity;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        m_ErrorLog.SetImportInformation("AC DC Capacity ,");
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Importing Solar AC_DC_Capacity WorkSheet:");
                                            statusCode = await InsertSolarAcDcCapacity(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for acdc import";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_tracker_loss)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            if (isBdSheet && isPyrano1Min && isPyrano15Min && isTracker)
                                            {
                                                m_ErrorLog.SetInformation(",Reviewing Solar Tracker_Loss WorkSheet:");
                                                statusCode = await InsertSolarTrackerLoss(status, ds);
                                            }
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for tracker_loss import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_tracker_loss_monthly)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Solar_tracker_loss_monthly;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Tracker_Loss Monthly WorkSheet:");
                                            statusCode = await InsertSolarTrackerLossMonthly(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for tracker_loss Monthly import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_soiling_loss)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Solar_soiling_loss;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetImportInformation("Solar Soiling Loss ,");
                                            m_ErrorLog.SetInformation(",Reviewing Solar Soiling_Loss WorkSheet:");
                                            statusCode = await InsertSolarSoilingLoss(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for soiling_loss import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_PVSyst_loss)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Solar_PVSyst_loss;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetImportInformation("PVSyst Loss ,");
                                            m_ErrorLog.SetInformation(",Reviewing Solar PVSyst_Loss WorkSheet:");
                                            statusCode = await InsertSolarPVSystLoss(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for PVSyst_Loss import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Estimated_Hourly_Data)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Estimated_Hourly_Data;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetImportInformation("Estimated Hourly Loss ,");
                                            m_ErrorLog.SetInformation(",Reviewing Solar Estimated_Hourly_Data WorkSheet:");
                                            statusCode = await InsertSolarEstimatedHourlyData(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for Estimated_Hourly_Data import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.TML_Data)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_TML_Data;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",TML_Data file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for TML_Data import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                            //statusCode = await InsertWindTMLData(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("TML Data ,");
                                            m_ErrorLog.SetInformation(",Importing Wind TML_Data WorkSheet:");
                                            statusCode = await InsertWindTMLData(status, ds, file.FileName, isMultiFiles);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Power_Curve)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Power_Curve;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",TML_Data file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for Power_Curve import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                            //statusCode = await InsertWindPowerCurve(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("Power Curve ,");
                                            m_ErrorLog.SetInformation(",Importing Wind Power_Curve WorkSheet:");
                                            statusCode = await InsertWindPowerCurve(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Breakdown_Code_Gamesa)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Breakdown_Code_Gamesa;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",BD_Code_Gamesa file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for BD_Code_Gamesa import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("Breakdown Type ,");
                                            m_ErrorLog.SetInformation(",Importing Wind BD_Code_Gamesa WorkSheet:");
                                            statusCode = await InsertWindBDCodeGamesa(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.WindSpeed_TMD)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_WindSpeed_TMD;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",WindSpeed_TMD file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for WindSpeed_TMD import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                            //statusCode = await InsertWindPowerCurve(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("TML Data ,");
                                            m_ErrorLog.SetInformation(",Importing Wind WindSpeed_TMD WorkSheet:");
                                            statusCode = await InsertWindSpeedTMD(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Reference_WTGs)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Reference_WTGs;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",Reference_WTGs file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for Reference_WTGs import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                            //statusCode = await InsertWindPowerCurve(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("Reference WTG ,");
                                            m_ErrorLog.SetInformation(",Importing Wind Reference_WTGs WorkSheet:");
                                            statusCode = await ImportWindReferenceWtgs(status, ds);
                                        }
                                    }
                                }
                                else if (isSuzlon == 1)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Suzlon_TMD;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Soalr")
                                        {
                                            m_ErrorLog.SetError(",Suzlon_TMD filecannot be imported for Solar.");
                                            status = "wrong file upload type selected for Suzlon_TMD import.";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("TML Data ,");
                                            m_ErrorLog.SetInformation(", Importing Wind Suzlon_TMD Worksheet :");
                                            statusCode = await ImportWindSuzlonTMD(status, ds, file.FileName, isMultiFiles);
                                        }
                                    }
                                }
                                else if (isInox == 1)
                                {
                                    //fileImportType = FileSheetType.FileImportType.imporFileType_Sheet1;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Soalr")
                                        {
                                            m_ErrorLog.SetError(",Inox_TMD filecannot be imported for Solar.");
                                            status = "wrong file upload type selected for Inox_TMD import.";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("TML Data ,");
                                            m_ErrorLog.SetInformation(", Importing Wind Inox_TMD Worksheet :");
                                            statusCode = await ImportWindInoxTMD(status, ds, file.FileName, isMultiFiles);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Breakdown_Code_INOX)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Breakdown_Code_INOX;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Soalr")
                                        {
                                            m_ErrorLog.SetError(",BD_Code_INOX filecannot be imported for Solar.");
                                            status = "wrong file upload type selected for BD_Code_INOX import.";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("Breakdown Codes ,");
                                            m_ErrorLog.SetInformation(", Importing Wind BD_Code_INOX Worksheet :");
                                            statusCode = await InsertWindBDCodeINOX(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Breakdown_Code_REGEN)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Breakdown_Code_REGEN;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Soalr")
                                        {
                                            m_ErrorLog.SetError(",BD_Code_REGEN filecannot be imported for Solar.");
                                            status = "wrong file upload type selected for BD_Code_REGEN import.";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetImportInformation("Breakdown Codes ,");
                                            m_ErrorLog.SetInformation(", Importing Wind BD_Code_REGEN Worksheet :");
                                            statusCode = await InsertWindBDCodeREGEN(status, ds);
                                        }
                                    }
                                }
                                /*else if (isGamesa == 1)
                                {
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            string sheetName = excelSheet;
                                            m_ErrorLog.SetInformation(",Reviewing Solar PVSyst_Loss WorkSheet:");
                                            statusCode = await InsertSolarPVSystLoss(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for PVSyst_Loss import";
                                        }
                                    }
                                }*/
                                //GA, GS, GG, BD, NEW, NL, GK, GBR, G114
                                else if (excelSheet.ToString().StartsWith("GKK") || excelSheet.ToString().StartsWith("GA") || excelSheet.ToString().StartsWith("GS") || excelSheet.ToString().StartsWith("GG") || excelSheet.ToString().StartsWith("BD") || excelSheet.ToString().StartsWith("NEW") || excelSheet.ToString().StartsWith("NL") || excelSheet.ToString().StartsWith("GK") || excelSheet.ToString().StartsWith("GBR") || excelSheet.ToString().StartsWith("G114"))
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_GKK;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        string tabName = excelSheet.ToString();
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetError(",GKK file cannot be imported for Solar");
                                            status = "Wrong file upload type selected for GKK import";
                                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                            //statusCode = await InsertWindTMR(status, ds, tabName);
                                        }
                                        else if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetImportInformation("TML Data ,");
                                            m_ErrorLog.SetInformation(",Reviewing Wind TMR:");
                                            statusCode = await InsertWindTMR(status, ds, tabName, isMultiFiles, isMultiSheet);
                                        }
                                    }
                                }
                                else
                                {
                                    status = "Unsupported Tab name <" + excelSheet + ">. Pl do not change tab name.";
                                    m_ErrorLog.SetError(status);
                                    //ErrorLog(status);
                                    LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
                                    //responseCode = 400;
                                }
                                //clear all rows in ds
                                ds.Clear();
                                //statusCode = 200;
                            } // end of foreach (var excelSheet in fileSheets)
                            if (statusCode == 200)
                            {
                                // await UploadFileToImportedFileFolder(file);
                                if (!(fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min") || fileSheets.Contains("Solar_tracker_loss")))
                                {
                                    await importMetaData(fileUploadType, file.FileName, fileImportType);
                                }

                                //DGR Automation Function Logic
                                if (fileUploadType == "Wind")
                                {
                                    if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown"))
                                    {
                                        if (isGenValidationSuccess && isBreakdownValidationSuccess)
                                        {
                                            await importMetaData(fileUploadType, file.FileName, fileImportType);
                                            statusCode = await dgrWindImport(batchIdDGRAutomation);
                                            if (statusCode == 200)
                                            {
                                                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailyWindKPI?fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                                //remove after testing
                                                // m_ErrorLog.SetInformation("Url" + url);
                                                using (var client = new HttpClient())
                                                {
                                                    //InformationLog("added timeout to InfiniteTimeSpan");
                                                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                                                    var response = await client.GetAsync(url);
                                                    //status = "Respose" + response;
                                                    if (response.IsSuccessStatusCode)
                                                    {
                                                        m_ErrorLog.SetInformation(",Wind KPI Calculations Updated Successfully:");
                                                        statusCode = (int)response.StatusCode;
                                                        status = "Successfully Uploaded";

                                                        // Added Code auto approved if uploaded by admin
                                                        string userName = HttpContext.Session.GetString("DisplayName");
                                                        int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                                        siteUserRole = HttpContext.Session.GetString("role");
                                                        if (siteUserRole == "Admin")
                                                        {

                                                            var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                                            using (var client1 = new HttpClient())
                                                            {
                                                                await Task.Delay(10000);
                                                                var response1 = await client1.GetAsync(url1);
                                                                if (response1.IsSuccessStatusCode)
                                                                {
                                                                    //status = "Successfully Data Approved";
                                                                }
                                                                else
                                                                {
                                                                    //status = "Data Not Approved";
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        statusCode = (int)response.StatusCode;
                                                        string errorMsg = response.Content.ReadAsStringAsync().Result;
                                                        status = "Wind KPI Calculation Import API Failed";
                                                        m_ErrorLog.SetError(",Wind KPI Calculations API Failed. Reason : " + errorMsg);

                                                        //for solar 0, wind 1;
                                                        int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 1);
                                                        if (deleteStatus == 1)
                                                        {
                                                            m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                        }
                                                        else
                                                        {
                                                            m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind KPI Calculations API Failed.");
                                                int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 1);
                                                if (deleteStatus == 1)
                                                {
                                                    m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetError(",Data not imported.");
                                        }
                                    }
                                    else
                                    {
                                        status = "Successfully Uploaded";
                                    }
                                }
                                else if (fileUploadType == "Solar")
                                {
                                    if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min") || fileSheets.Contains("Solar_tracker_loss"))
                                    {
                                        string msg = "isGenValidationSuccess" + isGenValidationSuccess;
                                        //InformationLog(msg);
                                        LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                        msg = "isPyro15ValidationSuccess" + isPyro15ValidationSuccess;
                                        //InformationLog(msg);
                                        LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                        msg = "isPyro1ValidationSuccess" + isPyro1ValidationSuccess;
                                        //InformationLog(msg);
                                        LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                        msg = "isTrackerLossvalidationSuccess " + isTrackerLossvalidationSuccess;
                                        //InformationLog(msg);
                                        LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                        msg = "Before Validation";
                                        //InformationLog(msg);
                                        LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                        //pending : instead check the  success flags
                                        if (isGenValidationSuccess && isBreakdownValidationSuccess && isPyro15ValidationSuccess && isPyro1ValidationSuccess && isTrackerLossvalidationSuccess)
                                        {
                                            DateTime dttt = DateTime.Now;
                                            //InformationLog("ImportMetaData function called from FUCtrl" + dttt);
                                            // Added Task
                                            //await Task.WhenAll(
                                            //    importMetaData(fileUploadType, file.FileName, fileImportType),
                                            //    dgrSolarImport(batchIdDGRAutomation)
                                            //);
                                            await importMetaData(fileUploadType, file.FileName, fileImportType);
                                            statusCode = await dgrSolarImport(batchIdDGRAutomation);
                                            //await importMetaData(fileUploadType, file.FileName, fileImportType);
                                            //statusCode = await dgrSolarImport(batchIdDGRAutomation);
                                            if (statusCode == 200)
                                            {
                                                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?fromDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?site=" + (string)kpiArgs[2] + "&fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "";
                                                //var client = new HttpClient();
                                                //var task = Task.Run(async () =>
                                                //{ });
                                                using (var client = new HttpClient())
                                                {
                                                    //InformationLog("added timeout to InfiniteTimeSpan");
                                                    client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                                                    dttt = DateTime.Now;
                                                    msg = "CalculateDailysolarKPI API Called." + dttt;
                                                    //InformationLog(msg);
                                                    LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                                                    //InformationLog(url);
                                                    LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", url);
                                                    var response = await client.GetAsync(url);
                                                    //response.Timeout = TimeSpan.FromSeconds(300); // set the request timeout to 5 minutes
                                                    if (response.IsSuccessStatusCode)
                                                    {
                                                        dttt = DateTime.Now;
                                                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                                                        returnResponse = returnResponse.TrimEnd(',');
                                                        if (returnResponse != "0")
                                                        {
                                                            string[] errCodes = returnResponse.Split(",");
                                                            for (int i = 0; i < errCodes.Length; i++)
                                                            {
                                                                int code = Convert.ToInt32(errCodes[i]);
                                                                ErrorCode ec = (ErrorCode)code;
                                                                //retrieve error code from enum and common file.
                                                                ErrorCodes.ErrorDescriptionMap.TryGetValue(ec, out string errorDescription);
                                                                if (code != 0)
                                                                {
                                                                    m_ErrorLog.SetError("," + errorDescription);
                                                                }
                                                                else
                                                                {
                                                                    m_ErrorLog.SetInformation("," + errorDescription);
                                                                }
                                                            }
                                                        }
                                                        //InformationLog("CalculateDailySolarKpI function returned successfully to frontend." + dttt);
                                                        m_ErrorLog.SetInformation(",SolarKPI Calculations Updated Successfully:");
                                                        statusCode = (int)response.StatusCode;
                                                        status = "Successfully Uploaded";
                                                        // Added Code auto approved if uploaded by admin
                                                        string userName = HttpContext.Session.GetString("DisplayName");
                                                        int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                                        siteUserRole = HttpContext.Session.GetString("role");
                                                        dttt = DateTime.Now;
                                                        if (siteUserRole == "Admin")
                                                        {
                                                            //InformationLog("Uploading User is Admin. So SetsolarApprovalFlagForImpoortBatches API call" +dttt);
                                                            var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetSolarApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                                            using (var client1 = new HttpClient())
                                                            {
                                                                await Task.Delay(10000);
                                                                var response1 = await client1.GetAsync(url1);
                                                                if (response1.IsSuccessStatusCode)
                                                                {
                                                                    //InformationLog("Approved Data Successfully." + DateTime.Now);
                                                                    //status = "Successfully Data Approved";
                                                                }
                                                                else
                                                                {
                                                                    //ErrorLog("Data not approved"+ DateTime.Now);
                                                                    //status = "Data Not Approved";
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        statusCode = (int)response.StatusCode;
                                                        string errorMsg = response.Content.ReadAsStringAsync().Result;

                                                        m_ErrorLog.SetError(",SolarKPI Calculations API Failed. Reason : " + errorMsg);
                                                        status = "Solar KPI Calculation Import API Failed. Reason : " + errorMsg;

                                                        //for solar 0, wind 1;
                                                        int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 0);
                                                        if (deleteStatus == 1)
                                                        {
                                                            m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                        }
                                                        else
                                                        {
                                                            m_ErrorLog.SetError(", Records deletion failed due to incomplete upload");
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",SolarKPI Calculations API Failed.");
                                                int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 0);
                                                if (deleteStatus == 1)
                                                {
                                                    m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(", Records deletion failed due to incomplete upload");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetError(",Data not imported.");
                                        }
                                    }
                                    else
                                    {
                                        status = "Successfully Uploaded";
                                    }
                                }
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Import Operation Failed:");
                            }

                            if (isMultiSheet == 1 && TMLType == 1)
                            {
                                if(tmlMultiError == 0)
                                {
                                    try
                                    {
                                        var json = JsonConvert.SerializeObject(TMLDataSet);
                                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                                        //***********Here sending dynamic type is remaining, as we have to test the performance. did for Inox multiple imports.*************
                                        //insertWindTMLData type = 1 : Gamesa ; type = 2 : INOX ; type = 3 : Suzlon.
                                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=" + TMLType;
                                        using (var client = new HttpClient())
                                        {
                                            client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                                            var response = await client.PostAsync(url, data);
                                            string returnResponse = response.Content.ReadAsStringAsync().Result;
                                            if (response.IsSuccessStatusCode)
                                            {
                                                if (returnResponse == "5")
                                                {
                                                    m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(",Error in Calculation.");
                                                }
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind TMR API Failure,: responseCode <" + (int)response.StatusCode + ">");

                                                //for solar 0, wind 1, other 2;
                                                int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                                                if (deleteStatus == 1)
                                                {
                                                    m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                }
                                                else if (deleteStatus == 0)
                                                {
                                                    m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetInformation(", File not uploaded");
                                                }
                                            }
                                        }
                                        m_ErrorLog.SetInformation("Information : Calculation and storing of data successful. ");
                                        m_ErrorLog.SaveToCSV(csvFileName);
                                        //if (statusCode != 200)
                                        ArrayList messageList1 = m_ErrorLog.errorLog();
                                        foreach (var item in messageList1)
                                        {
                                            status += ((string)item).Replace(",", "") + ",";
                                        }
                                         return status;
                                    }
                                    catch (Exception e)
                                    {
                                        string msg = "Exception while sending data to api of TML for multiple files, due to : " + e.ToString();
                                        m_ErrorLog.SetError("Exception while sending data to api of TML for multiple files, due to : " + e.Message);
                                    }
                                }
                                if (tmlMultiError == 1)
                                {
                                    m_ErrorLog.SetError("TML import failure.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            status = "Exception Caught Debugging Required";
                            m_ErrorLog.SetError("," + status + ":" + ex.Message);
                            //ErrorLog("Inside" + ex.ToString());
                            string msg = status + " " + ex.ToString();
                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                        }
                    }
                    else if (ext == ".txt" || ext == ".csv")
                    {
                        try
                        {
                            using (var stream = new FileStream(@"\TempFile\docupload.csv", FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            DataTable dt = null;

                            string _filePath = @"C:\TempFile\docupload.csv";
                            //string _filePath = @"G:\TempFile\docupload.csv";
                            dataSetMain = GetDataTableFromTxtOrCsv(_filePath, true, ref fileSheets);
                            if (dataSetMain == null)
                            {
                                m_ErrorLog.SetError(",Unable to extract imported file data for importing,");
                                string msg = "datSetMain null " + dataSetMain;
                                //ErrorLog(msg);
                                LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                            }

                            //masterHashtable_SiteName_To_SiteId();//C
                            masterHashtable_SiteIdToSiteName();
                            if (fileUploadType == "Wind")
                            {
                                masterHashtable_WtgToWtgId();
                                masterHashtable_WtgToSiteId();
                            }
                            /*
                            if (fileUploadType == "Solar")
                            {
                                masterInverterList();
                            }
                            */

                            //Status Codes:
                            //200 = Success ; 400 = Failure(BadRequest)
                            if (fileUploadType == "Wind")
                            {
                                m_ErrorLog.SetInformation(",Reviewing Wind file for TML data import: " + file.FileName);
                                statusCode = await InsertWindRegen(status, dataSetMain, file.FileName, isMultiFiles);
                            }
                        }
                        catch (Exception ex)
                        {
                            status = "Exception Caught Debugging Required";
                            m_ErrorLog.SetError("," + status + ":" + ex.Message);
                            string msg = "Inside" + ex.ToString();
                            //ErrorLog(msg);
                            LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                        }
                    }
                    else
                    {
                        string msg = ", Invalid file type <" + ext + "> Please upload file(s) either of 'xlsx' or 'txt' or 'csv'.";
                        m_ErrorLog.SetError(msg);
                        LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                    }
                }
                catch (Exception ex)
                {
                    status = "Something went wrong : Exception Caught Debugging Required";
                    //status = status.Substring(0, (status.IndexOf("Exception") + 9));
                    m_ErrorLog.SetError("," + status);
                    string msg = status + "True\r\n" + ex.ToString();
                    //ErrorLog(msg);
                    LogError(user_id, 0, 4, "ExcelDataReadAndUpload", msg);
                }
            }
            else
            {
                //excel file format condition
                status = "File extension not supported. Upload type <" + fileUploadType + ">  Filename < " + csvFileName + ">";
                m_ErrorLog.SetError("," + status + ":");
                //ErrorLog(status);
                LogError(user_id, 0, 4, "ExcelDataReadAndUpload", status);
            }

            m_ErrorLog.SaveToCSV(csvFileName);
            //if (statusCode != 200)
            ArrayList messageList = m_ErrorLog.errorLog();
            foreach (var item in messageList)
            {
                status += ((string)item).Replace(",", "") + ",";
            }
            return status;
        }


        private DataSet GetDataTableFromExcel(string filePath, bool hasHeader, ref List<string> _worksheetList)
        {
            string status = "";
            bool isGKK = false;
            DataSet dataSet = new DataSet();

            try
            {
                DataTable dt = new DataTable();
                FileInfo excelFile = new FileInfo(filePath);
                var excel = new ExcelPackage(excelFile);
                foreach (var worksheet in excel.Workbook.Worksheets)
                {
                    //GA, GS, GG, BD, NEW, NL, GK, GBR, G114
                    if (worksheet.Name.StartsWith("GKK") || worksheet.Name.StartsWith("GA") || worksheet.Name.StartsWith("GS") || worksheet.Name.StartsWith("GG") || worksheet.Name.StartsWith("BD") || worksheet.Name.StartsWith("NEW") || worksheet.Name.StartsWith("NL") || worksheet.Name.StartsWith("GK") || worksheet.Name.StartsWith("GBR") || worksheet.Name.StartsWith("G114"))
                    {
                        isGKK = true;
                    }
                    if (FileSheetType.sheetList.Contains(worksheet.Name) || isGKK)
                    {
                        _worksheetList.Add(worksheet.Name);
                    }
                    if (isInox == 1)
                    {
                        _worksheetList.Add(worksheet.Name);
                    }
                    if (isSuzlon == 1)
                    {
                        _worksheetList.Add(worksheet.Name);
                    }
                }

                // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(excelFile))
                {
                    foreach (var li in _worksheetList)
                    {
                        dt = new DataTable();
                        dt.TableName = li;
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[li];

                        if (isGKK)
                        {
                            int rowsToDelete = 4;
                            int totalRows = workSheet.Dimension.End.Row;

                            //for (int rN = totalRows; rN > totalRows - rowsToDelete; rN--)
                            //{
                            //    workSheet.DeleteRow(rN);
                            //}

                            workSheet.DeleteRow(1, rowsToDelete);
                            workSheet.DeleteColumn(1);
                        }

                        //add column header
                        try
                        {
                            foreach (var header in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                            {
                                dt.Columns.Add(header.Text);
                            }
                            for (int rN = 2; rN <= workSheet.Dimension.End.Row; rN++)
                            {
                                ExcelRange row = workSheet.Cells[rN, 1, rN, workSheet.Dimension.End.Column];
                                DataRow newR = dt.NewRow();
                                foreach (var cell in row)
                                {
                                    try
                                    {
                                        newR[cell.Start.Column - 1] = cell.Text;
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Something went wrong : Exception Caught Debugging Required";
                                        ex.GetType();
                                        LogError(user_id, 0, 4, "GetDataTableFromExcel", status);
                                        //+ ex.ToString();
                                        //status = status.Substring(0, (status.IndexOf("Exception") + 8));
                                        // m_ErrorLog.SetError("," + status);
                                    }
                                }
                                dt.Rows.Add(newR);
                            }
                            dataSet.Tables.Add(dt);
                        }
                        catch (Exception ex)
                        {
                            status = "Exception Caught : " + ex.Message;
                            m_ErrorLog.SetError("," + status);
                            LogError(user_id, 0, 4, "GetDataTableFromExcel", status);
                        }
                        // add rows
                    }
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                status = "Exception Caught : " + ex.Message;
                m_ErrorLog.SetError("," + status);
                LogError(user_id, 0, 4, "GetDataTableFromExcel", ex.ToString());
                //throw new Exception(ex.Message);
                return dataSet;
            }
        }

        private DataSet GetDataTableFromTxtOrCsv(string filePath, bool hasHeader, ref List<string> _worksheetList)
        {
            string status = "";
            try
            {
                DataSet dataSet = new DataSet();
                DataTable dt = new DataTable();

                var dataTable = new DataTable();
                using (var reader = new StreamReader(filePath))
                {
                    //string[] headers = reader.ReadLine().Split('\t');
                    string[] headers = reader.ReadLine().Split(';');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        //string[] rows = reader.ReadLine().Split('\t');
                        string[] rows = reader.ReadLine().Split(';');
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = rows[i];
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                dataSet.Tables.Add(dataTable);
                return dataSet;
            }
            catch (Exception ex)
            {
                status = "Something went wrong : Exception Caught Debugging Required";
                m_ErrorLog.SetError("," + status + ",");
                LogError(user_id, 0, 4, "GetDataTableFromExcel", ex.ToString());
                throw new Exception(ex.Message);
            }
        }

        //Remove static
        //Beginning of all DGR Import functions for both Wind and Solar Upload types

        private async Task<int> GetBatchId(string importData)
        {
            var urlGetId = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchId?logFileName=" + importData + "";
            var result = string.Empty;
            WebRequest request = WebRequest.Create(urlGetId);
            using (var responses = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = responses.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                BatchIdImport obj = new BatchIdImport();
                obj = JsonConvert.DeserializeObject<BatchIdImport>(result);
                batchIdDGRAutomation = obj.import_batch_id;
                if (batchIdDGRAutomation == 0)
                {
                    return 0;
                }
                else
                {
                    return batchIdDGRAutomation;
                }
            }
        }
        private async Task<int> DeleteRecordsAfterFailure(string importData, int siteType)
        {
            //for solar 0, wind 1;
            int batchId = await GetBatchId(importData);
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/DeleteRecordsAfterFailure?batchId=" + batchId + "&siteType=" + siteType + "";
            var result = "";
            DateTime dtt = DateTime.Now;
            //InformationLog("DeleterecordsAfterFailure API called : "+ dtt);
            WebRequest request = WebRequest.Create(url);
            using (var responses = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = responses.GetResponseStream();
                dtt = DateTime.Now;
                //InformationLog("DeleteRecordsAfterFailure returned to frontend. "+dtt);
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                if (result == "1")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private async Task<int> InsertSolarFileGeneration(string status, DataSet ds)
        {
            //InformationLog("InsertSolarFileGeneration function Called : "+ DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            string siteName = "";
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string kpiSite = "";
            try
            {
                //fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
                //toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
                fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                //InformationLog("InsertSolarfileGeneration from and to Date converted successfully");
            }
            catch (Exception e)
            {
                //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                //ErrorLog("InsertSolarFileGeneration function exception while date conversion exception :"+e.ToString());
                string msg = ",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy";
                m_ErrorLog.SetError(msg);
                msg += e.ToString();
                LogError(user_id, 0, 4, "InsertSolarFileGeneration", msg);
                fromDate = DateTime.MaxValue;
                toDate = DateTime.MinValue;
            }
            //---------------------
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                SolarUploadingFileValidation solarValidation = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarUploadingFileGeneration> addSet = new List<SolarUploadingFileGeneration>();
                int i = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        //InformationLog("InsertSolarFileGeneration try block : "+ i + " /"+ ds.Tables[0].Rows.Count);
                        SolarUploadingFileGeneration addUnit = new SolarUploadingFileGeneration();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);

                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));

                        addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if (rowNumber == 2)
                        {
                            generationDate = addUnit.date;
                            siteName = addUnit.site;
                            masterInverterList(siteName);
                        }
                        if (rowNumber > 2)
                        {
                            if (generationDate != addUnit.date)
                            {
                                m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                                errorCount++;
                                skipRow = true;
                                continue;
                            }
                        }


                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        if (siteUserRole != "Admin")
                        {
                            errorFlag.Add(uniformWindSiteValidation(rowNumber, addUnit.site_id, ""));
                        }

                        objImportBatch.importSiteId = addUnit.site_id;//C
                        kpiSite = Convert.ToString(addUnit.site_id);
                        if (rowNumber == 2)
                        {
                            objImportBatch.automationDataDate = addUnit.date;
                            dateValidate = Convert.ToDateTime((string)dr["Date"]);
                            errorFlag.Add(importDateValidation(2, addUnit.site_id, dateValidate));

                        }
                        //nextDate = Convert.ToDateTime(dr["Date"], timeCulture);
                        nextDate = Convert.ToDateTime(dr["Date"]);
                        fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                        toDate = (nextDate > toDate) ? (nextDate) : (toDate);

                        addUnit.inverter = string.IsNullOrEmpty((string)dr["Inverter"]) ? "Nil" : Convert.ToString(dr["Inverter"]);
                        errorFlag.Add(solarInverterValidation(addUnit.inverter, "Inverter", rowNumber));

                        // addUnit.inv_act = string.IsNullOrEmpty((string)dr["Inv_Act(KWh)"]) ? 0 : Convert.ToDouble(dr["Inv_Act(KWh)"]);
                        // addUnit.plant_act = string.IsNullOrEmpty((string)dr["Plant_Act(kWh)"]) ? 0 : Convert.ToDouble(dr["Plant_Act(kWh)"]);

                        double importValue = 0.00;
                        int logErrorFlag = 0; //log as information
                                              //                        addUnit.inv_act = string.IsNullOrEmpty((string)dr["Inv_Act(KWh)"]) ? 0 : Convert.ToDouble(dr["Inv_Act(KWh)"]);
                        addUnit.inv_act = validateNumeric(((string)dr["Inv_Act(KWh)"]), "Inv_Act(KWh)", rowNumber, dr["Inv_Act(KWh)"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(negativeNullValidation(addUnit.inv_act, "Inv_Act(KWh)", rowNumber));
                        //addUnit.plant_act = string.IsNullOrEmpty((string)dr["Plant_Act(kWh)"]) ? 0 : Convert.ToDouble(dr["Plant_Act(kWh)"]);
                        addUnit.plant_act = validateNumeric(((string)dr["Plant_Act(KWh)"]), "Plant_Act(KWh)", rowNumber, dr["Plant_Act(KWh)"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(negativeNullValidation(addUnit.plant_act, "Inv_Act(KWh)", rowNumber));

                        addUnit.pi = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PI(%)"]), "PI(%)");
                        errorFlag.Add(solarValidation.validateGenerationData(rowNumber, addUnit.inverter, addUnit.inv_act, addUnit.plant_act));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": function: InsertSolarFileGeneration,");
                        //ErrorLog(",Exception Occurred In Function: InsertSolarFileGeneration: " + e.ToString() + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    //kpiArgs.Add(minDate);
                    //kpiArgs.Add(maxDate);
                    m_ErrorLog.SetInformation(",Solar Generation Validation Successful");
                    //InformationLog("InsertSolarFileGeneration function validation completed.");
                    isGenValidationSuccess = true;
                    responseCode = 200;
                    kpiArgs.Add(fromDate);
                    kpiArgs.Add(toDate);
                    kpiArgs.Add(kpiSite);
                    genJson = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Solar Generation Validation Failed");
                    //ErrorLog("InsertSolarFileGeneration function validation failed.");
                    isGenValidationSuccess = false;
                }
            }
            //InformationLog("InsertSolarFileGeneration function Returned : " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertWindFileGeneration(string status, DataSet ds)
        {
            //InformationLog("InsertWindFileGeneration function Called : " + DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string site = "";
            try
            {
                fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);

            }
            catch (Exception e)
            {
                //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy");
                fromDate = DateTime.MaxValue;
                toDate = DateTime.MinValue;
            }
            //---------------------------------
            WindUploadingFileValidation validationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);
            if (ds.Tables.Count > 0)
            {
                var numberofrows = ds.Tables[0].Rows.Count;

                for (int i = 0; i < numberofrows; i++)
                {
                    //for each row, get the 3rd column
                    var cell = ds.Tables[0].Rows[i][6];
                }

                generationDate = "";
                double max_kWh = 0;
                List<WindUploadingFileGeneration> addSet = new List<WindUploadingFileGeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindUploadingFileGeneration addUnit = new WindUploadingFileGeneration();
                        bool skipRow = false;
                        rowNumber++;
                        //ErrorLog("Before date conversion\r\n");

                        //addUnit.date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString((string)dr["Date"]);
                        // errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        //addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if (addUnit.wtg == "" && addUnit.wind_speed == "" && addUnit.kwh == "" && addUnit.grid_hrs == "" && addUnit.lull_hrs == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" + "Is blank.");
                            string msg = ",File row <" + rowNumber + ">" + "Is blank.";
                            //ErrorLog(msg);
                            LogError(user_id, 2, 4, "InsertWIndFileGeneration", msg);
                            skipRow = true;
                            continue;
                        }

                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            generationDate = addUnit.date;
                        }
                        if (rowNumber > 2)
                        {
                            if (generationDate != addUnit.date)
                            {
                                m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                                errorCount++;
                                skipRow = true;
                                continue;
                            }
                        }

                        addUnit.wtg = dr["WTG"] is DBNull || string.IsNullOrEmpty((string)dr["WTG"]) ? "Nil" : Convert.ToString(dr["WTG"]);
                        addUnit.wtg_id = equipmentId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(equipmentId[addUnit.wtg]) : 0;
                        errorFlag.Add(wtgValidation(addUnit.wtg, addUnit.wtg_id, rowNumber));
                        if (rowNumber == 2)
                        {
                            if (maxkWhMap_wind.ContainsKey(addUnit.wtg_id))
                            {
                                max_kWh = Convert.ToDouble(maxkWhMap_wind[addUnit.wtg_id]);
                            }
                            else
                            {
                                errorFlag.Add(true);
                                m_ErrorLog.SetError(",WTG <" + addUnit.wtg + "> WTG_id<" + addUnit.wtg_id + "> does not have max kWh set in location master");
                            }
                        }

                        addUnit.site_id = eqSiteId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(eqSiteId[addUnit.wtg]) : 0;
                        addUnit.site_name = siteName.ContainsKey(addUnit.site_id) ? (string)(siteName[addUnit.site_id]) : "Nil";
                        site = Convert.ToString(addUnit.site_id);
                        if (siteUserRole != "Admin")
                        {
                            errorFlag.Add(uniformWindSiteValidation(rowNumber, addUnit.site_id, addUnit.wtg));
                        }

                        objImportBatch.importSiteId = addUnit.site_id;
                        //dateValidate = Convert.ToDateTime((string)dr["Date"]).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            objImportBatch.automationDataDate = addUnit.date;
                            dateValidate = Convert.ToDateTime(dr["Date"]);
                            errorFlag.Add(importDateValidation(1, addUnit.site_id, dateValidate));
                        }

                        int logErrorFlag = 0;   //log as information
                        double importValue = 0;

                        //addUnit.wind_speed = string.IsNullOrEmpty((string)dr["Wind_Speed"]) ? 0 : Convert.ToDouble(dr["Wind_Speed"]);
                        addUnit.wind_speed = validateNumeric(((string)dr["Wind_Speed"]), "Wind_Speed", rowNumber, dr["Wind_Speed"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(numericNullValidation(addUnit.wind_speed, "Wind_Speed", rowNumber));

                        //addUnit.operating_hrs = dr["Gen_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Gen_Hrs"]) ? 0 : Convert.ToDouble(dr["Gen_Hrs"]);
                        addUnit.operating_hrs = validateNumeric(((string)dr["Gen_Hrs"]), "Gen_Hrs", rowNumber, dr["Gen_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        //errorFlag.Add(numericNullValidation(addUnit.operating_hrs, "Gen_Hrs", rowNumber));

                        //addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        addUnit.kwh = validateNumeric(((string)dr["kWh"]), "kWh", rowNumber, dr["kWh"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(kwhValidation(addUnit.kwh, addUnit.operating_hrs, "kWh", rowNumber, max_kWh));

                        // addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        // errorFlag.Add(numericNullValidation(addUnit.kwh, "kWh", rowNumber));

                        // addUnit.operating_hrs = dr["Gen_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Gen_Hrs"]) ? 0 : ////Convert.ToDouble(dr["Gen_Hrs"]);

                        //errorFlag.Add(numericNullValidation(addUnit.operating_hrs, "Gen_Hrs", rowNumber));
                        //last column is Blank
                        if (addUnit.grid_hrs == "" || addUnit.grid_hrs == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Grid_hrs> :  Grid hours column is blank <" + addUnit.grid_hrs + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                        //addUnit.lull_hrs = dr["Lull_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Lull_Hrs"]) ? 0 : Convert.ToDouble(dr["Lull_Hrs"]);
                        addUnit.lull_hrs = validateNumeric(((string)dr["Lull_Hrs"]), "Lull_Hrs", rowNumber, dr["Lull_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        //addUnit.grid_hrs = dr["Grid_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Grid_Hrs"]) ? 0 : Convert.ToDouble(dr["Grid_Hrs"]);
                        addUnit.grid_hrs = validateNumeric(((string)dr["Grid_Hrs"]), "Grid_Hrs", rowNumber, dr["Grid_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        nextDate = Convert.ToDateTime(dr["Date"]);
                        fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                        toDate = (nextDate > toDate) ? (nextDate) : (toDate);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        errorCount++;
                        m_ErrorLog.SetError(",File row <" + rowNumber + ">" + e.GetType() + ": Function: InsertWindFileGeneration,");
                        string msg = ",Exception Occurred In Function: InsertWindFileGeneration: " + e.ToString() + "";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindFileGeneration", msg);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Generation File Validation Successful,");
                    isGenValidationSuccess = true;
                    responseCode = 200;
                    genJson = JsonConvert.SerializeObject(addSet);
                    kpiArgs.Add(fromDate);
                    kpiArgs.Add(toDate);
                    kpiArgs.Add(site);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Generation File Validation Failed");
                    isGenValidationSuccess = false;
                }
            }
            //InformationLog("InsertWindFileGeneration function Completed : " + DateTime.Now);

            return responseCode;
        }

        private async Task<int> InsertSolarFileBreakDown(string status, DataSet ds)
        {
            //InformationLog("InsertSolarFileBreakDown function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;

            if (ds.Tables.Count > 0)
            {
                SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarUploadingFileBreakDown> addSet = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarUploadingFileBreakDown addUnit = new SolarUploadingFileBreakDown();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : (string)dr["Date"];
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? "Nil" : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if (generationDate != addUnit.date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        //last row is Blank
                        if (addUnit.date == "" && addUnit.bd_type == "" && addUnit.action_taken == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" + "Is blank.");
                            string msg = ",Exception Occurred In : SolarUploadingFileBreakdown: ";
                            //ErrorLog(msg);
                            LogError(user_id, 1, 4, "InsertSolarFileBreakdown", msg);
                            skipRow = true;
                            continue;
                        }

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if(rowNumber == 2)
                        {
                            errorFlag.Add(importDateValidation(2, addUnit.site_id, Convert.ToDateTime(addUnit.date)));
                            masterIcrInvList(addUnit.site);
                            masterInverterList(addUnit.site);
                        }
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;
                        //can be nil:
                        addUnit.ext_int_bd = string.IsNullOrEmpty((string)dr["Ext_BD"]) ? "Nil" : Convert.ToString(dr["Ext_BD"]);

                        //can be nil:
                        //addUnit.wtg_id = equipmentId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(equipmentId[addUnit.wtg]) : 0;
                        addUnit.igbd = string.IsNullOrEmpty((string)dr["IGBD"]) ? "Nil" : Convert.ToString(dr["IGBD"]);
                        if (addUnit.igbd != "Nil")
                        {
                            errorFlag.Add(solarIGBDValidation((string)dr["IGBD"], "IGBD", rowNumber));
                        }

                        //can be nil:
                        addUnit.icr = string.IsNullOrEmpty((string)dr["ICR"]) ? "Nil" : Convert.ToString(dr["ICR"]);
                        if (addUnit.icr != "Nil")
                        {
                            errorFlag.Add(solarICRValidation((string)dr["ICR"], "ICR", rowNumber));
                        }
                        //can be nil:
                        addUnit.inv = string.IsNullOrEmpty((string)dr["INV"]) ? "Nil" : Convert.ToString(dr["INV"]);
                        if (addUnit.inv != "Nil")
                        {
                            errorFlag.Add(solarINVValidation((string)dr["INV"], "INV", rowNumber));
                        }
                        //can be nil:
                        addUnit.smb = string.IsNullOrEmpty((string)dr["SMB"]) ? "Nil" : Convert.ToString(dr["SMB"]);
                        if (addUnit.smb != "Nil")
                        {
                            errorFlag.Add(solarSMBValidation((string)dr["SMB"], "SMB", rowNumber));
                        }
                        //can be nil:
                        addUnit.strings = dr["Strings"] is DBNull || string.IsNullOrEmpty((string)dr["Strings"]) ? "Nil" : Convert.ToString(dr["Strings"]);
                        if (addUnit.strings != "Nil")
                        {
                            errorFlag.Add(solarStringsValidation((string)dr["Strings"], "Strings", rowNumber));
                        }
                        //from_bd and to_bd conversion for validation
                        addUnit.from_bd = dr["From"] is DBNull || string.IsNullOrEmpty((string)dr["From"]) ? "Nil" : Convert.ToDateTime(dr["From"]).ToString("HH:mm:ss");
                        errorFlag.Add(timeValidation(addUnit.from_bd, "From", rowNumber));

                        addUnit.to_bd = dr["To"] is DBNull || string.IsNullOrEmpty((string)dr["To"]) ? "Nil" : Convert.ToDateTime(dr["To"]).ToString("HH:mm:ss");
                        errorFlag.Add(timeValidation(addUnit.to_bd, "To", rowNumber));

                        addUnit.total_bd = validationObject.breakDownCalc(addUnit.from_bd, addUnit.to_bd, rowNumber);
                        string remarks = dr["BDRemarks"] is DBNull || string.IsNullOrEmpty((string)dr["BDRemarks"]) ? "Nil" : Convert.ToString(dr["BDRemarks"]);
                        remarks = validateAndCleanSpChar(rowNumber, "BDRemarks", remarks);
                        addUnit.bd_remarks = remarks;
                        addUnit.bd_type = dr["BDType"] is DBNull || string.IsNullOrEmpty((string)dr["BDType"]) ? "Nil" : Convert.ToString(dr["BDType"]);
                        addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                        errorFlag.Add(bdTypeValidation(addUnit.bd_type, rowNumber));

                        string sActionTaken = dr["ActionTaken"] is DBNull || string.IsNullOrEmpty((string)dr["ActionTaken"]) ? "Nil" : Convert.ToString(dr["ActionTaken"]);
                        sActionTaken = validateAndCleanSpChar(rowNumber, "Action_Taken", sActionTaken);
                        if (sActionTaken.Length > 300)
                        {
                            sActionTaken = sActionTaken.Substring(0, 300);
                            m_ErrorLog.SetError(",String at <" + rowNumber + "> has trimed to 300 character length.");
                        }
                        addUnit.action_taken = sActionTaken;
                        errorFlag.Add(validationObject.validateBreakDownData(rowNumber, addUnit.from_bd, addUnit.to_bd, addUnit.igbd));
                        if (addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        //last column is Blank
                        if (addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File row <" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarFileBreakDown");
                        string msg = ",Exception Occurred In Function: InsertSolarFileBreakDown: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarFileBreakdown", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Breakdown Validation Successful:");
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Solar Breakdown Validation Failed");
                    isBreakdownValidationSuccess = false;
                }
            }
            //InformationLog("InsertSolarFileBreakDown function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindFileBreakDown(string status, DataSet ds)
        {
            //InformationLog("InsertWindFileBreakDown function Called : " + DateTime.Now);
            WindUploadingFileValidation ValidationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;

            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileBreakDown> addSet = new List<WindUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindUploadingFileBreakDown addUnit = new WindUploadingFileBreakDown();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? "Nil" : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        objImportBatch.importSiteId = addUnit.site_id;
                        //dateValidate = Convert.ToDateTime((string)dr["Date"]).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            objImportBatch.automationDataDate = addUnit.date;
                            dateValidate = Convert.ToDateTime(dr["Date"]);
                            errorFlag.Add(importDateValidation(1, addUnit.site_id, dateValidate));
                        }

                        //last if last row is Blank then skip? delete the row
                        if (addUnit.wtg == "" && addUnit.bd_type == "" && addUnit.stop_from == "" && addUnit.stop_to == "" && addUnit.error_description == "" && addUnit.action_taken == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" + "Is blank.");
                            string msg = ",Exception Occurred In : WindUploadingFileBreakdown: ";
                            //ErrorLog(msg);
                            LogError(user_id, 2, 4, "InsertWindFileBreakdown", msg);
                            skipRow = true;
                            continue;
                        }
                        if (addUnit.date == "" || addUnit.date == "Nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : Date field is empty <" + addUnit.date + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        if (generationDate != addUnit.date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        addUnit.wtg = Convert.ToString(dr["WTG"]);
                        addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                        addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);//E
                        addUnit.site_name = (string)siteName[addUnit.site_id];//D
                        objImportBatch.importSiteId = addUnit.site_id;
                        addUnit.bd_type = Convert.ToString(dr["BD_Type"]);
                        addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                        /*                        if (addUnit.bd_type_id == 0)
                                                {
                                                    m_ErrorLog.SetError(",Row <" + rowNumber + "> column <BD_Type> : Invalid BD_Type <" + addUnit.bd_type + ">");
                                                    errorFlag.Add(true);                                
                                                }*/
                        errorFlag.Add(bdTypeValidation(addUnit.bd_type, rowNumber));
                        addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("HH:mm:ss");
                        addUnit.stop_to = Convert.ToDateTime(dr["Stop To"]).ToString("HH:mm:ss");
                        addUnit.total_stop = ValidationObject.breakDownCalc(addUnit.stop_from, addUnit.stop_to);
                        string errorDescription = Convert.ToString(dr["Error description"]);
                        errorDescription = validateAndCleanSpChar(rowNumber, "Error Description", errorDescription);
                        addUnit.error_description = errorDescription;
                        string sActionTaken = dr["Action Taken"] is DBNull || string.IsNullOrEmpty((string)dr["Action Taken"]) ? "Nil" : Convert.ToString(dr["Action Taken"]);
                        sActionTaken = validateAndCleanSpChar(rowNumber, "Action Taken", sActionTaken);
                        if (sActionTaken.Length > 300)
                        {
                            sActionTaken = sActionTaken.Substring(0, 300);
                            m_ErrorLog.SetError(",String at <" + rowNumber + "> has trimed to 300 character length.");
                        }
                        addUnit.action_taken = sActionTaken;
                        errorFlag.Add(ValidationObject.validateBreakDownData(rowNumber, addUnit.bd_type, addUnit.wtg, addUnit.stop_from, addUnit.stop_to));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        //last column is Blank
                        if (addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File row <" + rowNumber + ">" + e.GetType() + ": Function: InsertWindFileBreakDown,");
                        string msg = ",Exception Occurred In Function: InsertWindFileBreakDown: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarFileBreakdown", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Breakdown File Validation Successful,");
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);

                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Breakdown File Validation Failed,");
                    isBreakdownValidationSuccess = false;
                }
            }
            //InformationLog("InsertWindFileBreakDown function Completed : " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertSolarPyranoMeter1Min(string status, DataSet ds)
        {
            //InformationLog("InsertSolarPyranoMeter1Min function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            try
            {
                if (ds.Tables.Count > 0)
                {
                    List<SolarUploadingPyranoMeter1Min> addSet = new List<SolarUploadingPyranoMeter1Min>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SolarUploadingPyranoMeter1Min addUnit = new SolarUploadingPyranoMeter1Min();
                        rowNumber++;
                        bool skipRow = false;
                        //addUnit.date_time = string.IsNullOrEmpty((string)dr["Time stamp"]) ? "Nil" : Convert.ToDateTime(dr["Time stamp"], timeCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        bool isdateEmpty = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date_time = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string dateFull = Convert.ToDateTime(addUnit.date_time).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            errorFlag.Add(importDateValidation(2, addUnit.site_id, Convert.ToDateTime(dateFull)));
                        }
                        errorFlag.Add(stringNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        errorFlag.Add(dateNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        string temp = addUnit.date_time;
                        string temp_date = temp.Substring(0, 10);
                        if (generationDate != temp_date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Pyranometer 1 Minute <" + temp_date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        string site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[site]);
                        errorFlag.Add(siteValidation(site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        int logErrorFlag = 1;
                        double importValue = 0.00;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-1"]), "GHI-1", rowNumber, dr["GHI-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-2"]), "GHI-2", rowNumber, dr["GHI-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-1"]), "POA-1", rowNumber, dr["POA-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-2"]), "POA-2", rowNumber, dr["POA-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-3", rowNumber, dr["POA-3"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_3 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-4"]), "POA-4", rowNumber, dr["POA-4"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_4 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-5"]), "POA-5", rowNumber, dr["POA-5"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_5 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-6"]), "POA-6", rowNumber, dr["POA-6"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_6 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-7"]), "POA-7", rowNumber, dr["POA-7"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_7 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average GHI (w/m²)"]), "Average GHI (w/m²)", rowNumber, dr["Average GHI (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_ghi = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average POA (w/m²)"]), "Average POA (w/m²)", rowNumber, dr["Average POA (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_poa = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Ambient Temp"]), "Ambient Temp", rowNumber, dr["Ambient Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.amb_temp = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Module Temp"]), "Module Temp", rowNumber, dr["Module Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.mod_temp = importValue;
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    //pending check error
                    //set IsPyro1 flag
                    if (!(errorCount > 0))
                    {
                        m_ErrorLog.SetInformation(",Solar PyranoMeter-1Min Validation Successful");
                        //set the  validationgeneration sucess flag
                        isPyro1ValidationSuccess = true;
                        responseCode = 200;
                        pyro1Json = JsonConvert.SerializeObject(addSet);
                    }
                    else
                    {
                        // add to error log that validation of generation failed
                        m_ErrorLog.SetError(",Solar PyranoMeter-1Min Validation Failed");
                        isPyro1ValidationSuccess = false;
                    }
                }

            }
            catch (Exception e)
            {
                //developer errorlog
                m_ErrorLog.SetError(",Exception in Function: InsertSolarPyranoMeter1Min on line " + rowNumber);
                string msg = ",Exception Occurred In Function: InsertSolarPyranoMeter1Min: " + e.ToString();
                //ErrorLog(msg);
                LogError(user_id, 1, 4, "InsertSolarPyranometer1Min", msg);
            }
            //InformationLog("InsertSolarPyranoMeter1Min function Completed : " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertSolarPyranoMeter15Min(string status, DataSet ds)
        {
            //InformationLog("InsertSolarPyranoMeter15Min function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            try
            {
                if (ds.Tables.Count > 0)
                {
                    List<SolarUploadingPyranoMeter15Min> addSet = new List<SolarUploadingPyranoMeter15Min>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SolarUploadingPyranoMeter15Min addUnit = new SolarUploadingPyranoMeter15Min();
                        rowNumber++;
                        bool skipRow = false;
                        // addUnit.date_time = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]) ? "Nil" : Convert.ToDateTime(dr["Time stamp"], timeCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        bool isdateEmpty = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date_time = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string dateFull = Convert.ToDateTime(addUnit.date_time).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            errorFlag.Add(importDateValidation(2, addUnit.site_id, Convert.ToDateTime(dateFull)));
                        }
                        errorFlag.Add(stringNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        errorFlag.Add(dateNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        string temp = addUnit.date_time;
                        string temp_date = temp.Substring(0, 10);
                        if (generationDate != temp_date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Pyranometer 15 Minute <" + temp_date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        string site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[site]);
                        errorFlag.Add(siteValidation(site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        int logErrorFlag = 1;
                        double importValue = 0.00;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-1"]), "GHI-1", rowNumber, dr["GHI-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-2"]), "GHI-2", rowNumber, dr["GHI-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-1"]), "POA-1", rowNumber, dr["POA-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-2"]), "POA-2", rowNumber, dr["POA-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-3", rowNumber, dr["POA-3"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_3 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-4"]), "POA-4", rowNumber, dr["POA-4"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_4 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-5"]), "POA-5", rowNumber, dr["POA-5"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_5 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-6"]), "POA-6", rowNumber, dr["POA-6"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_6 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-7"]), "POA-7", rowNumber, dr["POA-7"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_7 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average GHI (w/m²)"]), "Average GHI (w/m²)", rowNumber, dr["Average GHI (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_ghi = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average POA (w/m²)"]), "Average POA (w/m²)", rowNumber, dr["Average POA (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_poa = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Ambient Temp"]), "Ambient Temp", rowNumber, dr["Ambient Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.amb_temp = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Module Temp"]), "Module Temp", rowNumber, dr["Module Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.mod_temp = importValue;
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    //pending check error
                    //set IsPyro1 flag
                    if (!(errorCount > 0))
                    {
                        //set the  validationgeneration sucess flag
                        m_ErrorLog.SetInformation(",Solar PyranoMeter15Min Validation Successful");
                        isPyro15ValidationSuccess = true;
                        responseCode = 200;
                        pyro15Json = JsonConvert.SerializeObject(addSet);
                    }
                    else
                    {
                        // add to error log that validation of generation failed
                        status = "";
                        m_ErrorLog.SetError(",Solar PyranoMeter15Min Validation Failed");
                        isPyro15ValidationSuccess = false;
                    }
                }
            }
            catch (Exception e)
            {
                m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarPyranoMeter15Min " + rowNumber);
                string msg = ",Exception Occurred In Function: InsertSolarPyranoMeter15Min: " + e.ToString();
                //ErrorLog(msg);
                LogError(user_id, 1, 4, "InsertSolarPyranometer15Min", msg);
            }
            //InformationLog("InsertSolarPyranoMeter15Min function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyJMR(string status, DataSet ds)
        {
            //InformationLog("InsertSolarMonthlyJMR function Called : " + DateTime.Now);
            long rowNumber = 0;
            int errorCount = 0;
            int responseCode = 400;
            //bool errorInRow = false;
            //bool bValidationFailed = false;
            List<bool> errorFlag = new List<bool>();
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);

                List<SolarMonthlyJMR> addSet = new List<SolarMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        bool skipRow = false;
                        SolarMonthlyJMR addUnit = new SolarMonthlyJMR();
                        rowNumber++;
                        addUnit.Site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : (string)dr["Site"];
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.Site) ? Convert.ToInt32(siteNameId[addUnit.Site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.site_id, rowNumber));

                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : (string)(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        //addUnit.JMR_date = (dr["JMR date"] is DBNull) ? "Nil" : Convert.ToDateTime(dr["JMR date"], timeCulture).ToString("yyyy-MM-dd");
                        addUnit.JMR_date = (dr["JMR date"] is DBNull) ? "Nil" : Convert.ToDateTime(dr["JMR date"]).ToString("yyyy-MM-dd");
                        errorFlag.Add(dateNullValidation(addUnit.JMR_date, "JMR date", rowNumber));

                        addUnit.JMR_Month = dr["JMR Month"] is DBNull || string.IsNullOrEmpty((string)dr["JMR Month"]) ? "Nil" : Convert.ToString(dr["JMR Month"]);
                        addUnit.JMR_Month_no = longMonthList.ContainsKey((string)dr["JMR Month"]) ? Convert.ToInt32(longMonthList[addUnit.JMR_Month]) : 0;
                        addUnit.JMR_Year = dr["JMR Year"] is DBNull || string.IsNullOrEmpty((string)dr["JMR Year"]) ? 0 : Convert.ToInt32(dr["JMR Year"]);
                        errorFlag.Add(monthValidation(addUnit.JMR_Month, addUnit.JMR_Month_no, rowNumber));
                        errorFlag.Add(yearValidation(addUnit.JMR_Year, rowNumber));

                        addUnit.Plant_Section = dr["Plant Section"] is DBNull || string.IsNullOrEmpty((string)dr["Plant Section"]) ? "Nil" : Convert.ToString(dr["Plant Section"]);
                        //errorFlag.Add(stringNullValidation(addUnit.Plant_Section, "Plant Section", rowNumber));

                        addUnit.Controller_KWH_INV = Convert.ToDouble((dr["Controller (kWh)/INV (kWh)"] is DBNull) ? 0 : dr["Controller (kWh)/INV (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Controller_KWH_INV, "Controller (kWh)/INV (kWh)", rowNumber));

                        addUnit.Scheduled_Units_kWh = Convert.ToDouble(dr["Scheduled Units (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units (kWh)"]) ? 0 : dr["Scheduled Units (kWh)"]);
                        // errorFlag.Add(negativeNullValidation(addUnit.Scheduled_Units_kWh, "Scheduled Units (kWh)", rowNumber));

                        addUnit.Export_kWh = Convert.ToDouble((dr["Export (kWh)"] is DBNull) ? 0 : dr["Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Export_kWh, "Export (kWh)", rowNumber));

                        addUnit.Import_kWh = Convert.ToDouble((dr["Import (kWh)"] is DBNull) ? 0 : dr["Import (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Import_kWh, "Import (kWh)", rowNumber));

                        addUnit.Net_Export_kWh = Convert.ToDouble((dr["Net Export (kWh)"] is DBNull) ? 0 : dr["Net Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Net_Export_kWh, "Net Export (kWh)", rowNumber));

                        addUnit.Net_Billable_kWh = Convert.ToDouble((dr["Net Billable (kWh)"] is DBNull) ? 0 : dr["Net Billable (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Net_Billable_kWh, "Net Billable (kWh)", rowNumber));

                        //addUnit.Export_kVAh = Convert.ToDouble((dr["Export (kVAh)"] is DBNull) ? 0 : dr["Export (kVAh)"]);
                        addUnit.Export_kVAh = Convert.ToDouble(dr["Export (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVAh)"]) ? 0 : dr["Export (kVAh)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Export_kVAh, "Export (kVAh)", rowNumber));

                        //addUnit.Import_kVAh = Convert.ToDouble((dr["Import (kVAh)"] is DBNull) ? 0 : dr["Import (kVAh)"]);
                        addUnit.Import_kVAh = Convert.ToDouble(dr["Import (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVAh)"]) ? 0 : dr["Import (kVAh)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Import_kVAh, "Import (kVAh)", rowNumber));

                        //addUnit.Export_kVArh_lag = Convert.ToDouble((dr["Export (kVArh lag)"] is DBNull) ? 0 : dr["Export (kVArh lag)"]);
                        addUnit.Export_kVArh_lag = Convert.ToDouble(dr["Export (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lag)"]) ? 0 : dr["Export (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Export_kVArh_lag, "Export (kVArh lag)", rowNumber));

                        //addUnit.Import_kVArh_lag = Convert.ToDouble((dr["Import (kVArh lag)"] is DBNull) ? 0 : dr["Import (kVArh lag)"]);
                        addUnit.Import_kVArh_lag = Convert.ToDouble(dr["Import (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lag)"]) ? 0 : dr["Import (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Import_kVArh_lag, "Import (kVArh lag)", rowNumber));

                        //addUnit.Export_kVArh_lead = Convert.ToDouble((dr["Export (kVArh lead)"] is DBNull) ? 0 : dr["Export (kVArh lead)"]);
                        addUnit.Export_kVArh_lead = Convert.ToDouble(dr["Export (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lead)"]) ? 0 : dr["Export (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Export_kVArh_lead, "Export (kVArh lead)", rowNumber));

                        //addUnit.Import_kVArh_lead = Convert.ToDouble((dr["Import (kVArh lead)"] is DBNull) ? 0 : dr["Import (kVArh lead)"]);
                        addUnit.Import_kVArh_lead = Convert.ToDouble(dr["Import (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lead)"]) ? 0 : dr["Import (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Import_kVArh_lead, "Import (kVArh lead)", rowNumber));

                        addUnit.LineLoss = Convert.ToDouble((dr["Line Loss"] is DBNull) ? 0 : dr["Line Loss"]);
                        //errorFlag.Add(numericNullValidation(addUnit.LineLoss, "LineLoss", rowNumber));

                       // addUnit.Line_Loss_percentage = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        //errorFlag.Add((addUnit.Line_Loss_percentage > 100 || addUnit.Line_Loss_percentage < 0) ? true : false);
                        bool isNullOrEmpty = dr["Line Loss%"] is DBNull || string.IsNullOrEmpty((string)dr["Line Loss%"]);
                        if (isNullOrEmpty)
                        {
                           
                        }
                        else
                        {
                            addUnit.Line_Loss_percentage = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        }

                       //addUnit.RKVH_percentage = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        //errorFlag.Add((addUnit.RKVH_percentage > 100 || addUnit.RKVH_percentage < 0) ? true : false);
                        bool isNullOrEmptyRKVH = dr["RKVH%"] is DBNull || string.IsNullOrEmpty((string)dr["RKVH%"]);
                        if (isNullOrEmptyRKVH)
                        {
                            addUnit.RKVH_percentage = 0.00;
                        }
                        else
                        {
                            addUnit.RKVH_percentage =  commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        }

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_JMR(addUnit, addSet, rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyJMR " + rowNumber);
                        //m_ErrorLog.SetError(",File row <" + rowNumber + "> exception type <" + e.Message + ">: Function: InsertSolarMonthlyJMR");
                        m_ErrorLog.SetError(",File row <" + rowNumber + "> exception type <" + e.Message + ">");
                        string msg = ",Exception Occurred In Function: InsertSolarMonthlyJMR: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarJMR", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly JMR Validation Successful");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly JMR Import API Successful");
                            return responseCode = (int)response.StatusCode;

                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly JMR Import API Failed");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly JMR Validation Failed");
                }
            }
            //InformationLog("InsertSolarMonthlyJMR function Completed : " + DateTime.Now);
            return responseCode;
        }
        //End of all DGR Import functions for both Wind and Solar Upload types
        private async Task<int> InsertWindMonthlyJMR(string status, DataSet ds)
        {
            //InformationLog("InsertWindMonthlyJMR function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindMonthlyJMR> addSet = new List<WindMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindMonthlyJMR addUnit = new WindMonthlyJMR();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.siteId = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.siteId, rowNumber));
                        objImportBatch.importSiteId = addUnit.siteId;

                        addUnit.plantSection = string.IsNullOrEmpty((string)dr["Plant Section"]) ? "Nil" : Convert.ToString(dr["Plant Section"]);
                        //errorFlag.Add(stringNullValidation(addUnit.plantSection, "Plant Section", rowNumber));

                        addUnit.jmrDate = string.IsNullOrEmpty((string)dr["JMR date"]) ? "Nil" : Convert.ToString(dr["JMR date"]);
                        errorFlag.Add(dateNullValidation(addUnit.jmrDate, "JMR date", rowNumber));
                        addUnit.jmrDate = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["JMR date"]).ToString("yyyy-MM-dd");

                        addUnit.jmrMonth = string.IsNullOrEmpty((string)dr["JMR Month"]) ? "Nil" : Convert.ToString(dr["JMR Month"]);
                        addUnit.jmrMonth_no = longMonthList.Contains(addUnit.jmrMonth) ? Convert.ToInt32(longMonthList[addUnit.jmrMonth]) : 0;
                        errorFlag.Add(monthValidation(addUnit.jmrMonth, addUnit.jmrMonth_no, rowNumber));

                        addUnit.jmrYear = string.IsNullOrEmpty((string)dr["JMR Year"]) ? 0 : Convert.ToInt32(dr["JMR Year"]);
                        errorFlag.Add(yearValidation(addUnit.jmrYear, rowNumber));



                       //addUnit.lineLossPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                       //errorFlag.Add((addUnit.lineLossPercent > 100 || addUnit.lineLossPercent < 0) ? true : false);
                        bool isNullOrEmpty = dr["Line Loss%"] is DBNull || string.IsNullOrEmpty((string)dr["Line Loss%"]);
                        if (isNullOrEmpty)
                        {

                        }
                        else
                        {
                            addUnit.lineLossPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        }


                        //addUnit.rkvhPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        //errorFlag.Add((addUnit.rkvhPercent > 100 || addUnit.rkvhPercent < 0) ? true : false);
                        bool isNullOrEmptyRKVH = dr["RKVH%"] is DBNull || string.IsNullOrEmpty((string)dr["RKVH%"]);
                        if (isNullOrEmptyRKVH)
                        {
                            addUnit.rkvhPercent = 0.00;
                        }
                        else
                        {
                            addUnit.rkvhPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        }

                        addUnit.controllerKwhInv = Convert.ToDouble(dr["Controller (kWh)/INV (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Controller (kWh)/INV (kWh)"]) ? 0 : dr["Controller (kWh)/INV (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.controllerKwhInv, "Controller (kWh)/INV (kWh)", rowNumber));

                        addUnit.scheduledUnitsKwh = Convert.ToDouble(dr["Scheduled Units (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units (kWh)"]) ? 0 : dr["Scheduled Units (kWh)"]);
                        //errorFlag.Add(negativeNullValidation(addUnit.scheduledUnitsKwh, "Scheduled Units  (kWh)", rowNumber));

                        addUnit.exportKwh = Convert.ToDouble(dr["Export (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kWh)"]) ? 0 : dr["Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.exportKwh, "Export (kWh)", rowNumber));

                        addUnit.importKwh = Convert.ToDouble(dr["Import (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kWh)"]) ? 0 : dr["Import (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.importKwh, "Import (kWh)", rowNumber));

                        addUnit.netExportKwh = Convert.ToDouble(dr["Net Export (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Net Export (kWh)"]) ? 0 : dr["Net Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.netExportKwh, "Net Export (kWh)", rowNumber));
                        
                        addUnit.netBillableKwh = Convert.ToDouble(dr["Net Billable (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Net Billable (kWh)"]) ? 0 : dr["Net Billable (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.netExportKwh, "Net Billable (kWh)", rowNumber));

                        addUnit.exportKvah = Convert.ToDouble(dr["Export (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVAh)"]) ? 0 : dr["Export (kVAh)"]);
                        //errorFlag.Add(negativeNullValidation(addUnit.exportKvah, "Export (kVAh)", rowNumber));

                        addUnit.importKvah = Convert.ToDouble(dr["Import (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVAh)"]) ? 0 : dr["Import (kVAh)"]);
                        //errorFlag.Add(negativeNullValidation(addUnit.importKvah, "Import (kVAh)", rowNumber));

                        addUnit.exportKvarhLag = Convert.ToDouble(dr["Export (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lag)"]) ? 0 : dr["Export (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.exportKvarhLag, "Export (kVArh lag)", rowNumber));

                        addUnit.importKvarhLag = Convert.ToDouble(dr["Import (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lag)"]) ? 0 : dr["Import (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.importKvarhLag, "Import (kVArh lag)", rowNumber));

                        addUnit.exportKvarhLead = Convert.ToDouble(dr["Export (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lead)"]) ? 0 : dr["Export (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.exportKvarhLead, "Export (kVArh lead)", rowNumber));

                        addUnit.importKvarhLead = Convert.ToDouble(dr["Import (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lead)"]) ? 0 : dr["Import (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.importKvarhLead, "Import (kVArh lead)", rowNumber));

                        addUnit.lineLoss = Convert.ToDouble(dr["Line Loss"] is DBNull || string.IsNullOrEmpty((string)dr["Line Loss"]) ? 0 : dr["Line Loss"]);
                        //errorFlag.Add(numericNullValidation(addUnit.lineLoss, "Line Loss", rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        //m_ErrorLog.SetError(",File row <" + rowNumber + "> exception type <" + e.Message + ">: Function: InsertWindMonthlyJMR");
                        m_ErrorLog.SetError(",File row <" + rowNumber + "> exception type <" + e.Message + ">");
                        string msg = ",Exception <" + e.GetType() + "> Occurred In Function: InsertWindMonthlyJMR: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindMonthlyJMR", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly JMR Validation Successful");
                    //api call used for importing wind:monthly jmr client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly JMR Import API Successful:");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly JMR Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Monthly JMR Validation Failed");
                }
            }
            //InformationLog("InsertWindMonthlyJMR function Called : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyLineLoss(string status, DataSet ds)
        {
            // InformationLog("InsertSolarMonthlyLineLoss function Called : " + DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            long rowNumber = 1;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarMonthlyUploadingLineLosses> addSet = new List<SolarMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarMonthlyUploadingLineLosses addUnit = new SolarMonthlyUploadingLineLosses();
                        bool skipRow = false;
                        rowNumber++;

                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.Month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.Month) ? Convert.ToInt32(MonthList[addUnit.Month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.Month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.FY.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? year : year + 1;
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        addUnit.LineLoss = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss (%)"]), "Line Loss (%)");
                        if (addUnit.LineLoss > 100)
                        {
                            m_ErrorLog.SetError("," + ": Line loss can not be more than 100,");
                            errorFlag.Add(true);
                        }

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_LineLoss(addUnit, addSet, rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyLineLoss," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarMonthlyLineLoss: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarMonthlyLineLoss", msg);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly Lineloss Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly Lineloss Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly Lineloss Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly Lineloss Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Solar Monthly Lineloss File Empty,");
            }
            // InformationLog("InsertSolarMonthlyLineLoss function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyLineLoss(string status, DataSet ds)
        {
            // InformationLog("InsertWindMonthlyLineLoss function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindMonthlyUploadingLineLosses> addSet = new List<WindMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindMonthlyUploadingLineLosses addUnit = new WindMonthlyUploadingLineLosses();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["FY"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.month = string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.month) ? Convert.ToInt32(MonthList[addUnit.month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));

                        int finalYear = errorFlag[0] == false ? Convert.ToInt32(addUnit.fy.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? finalYear : finalYear + 1;

                        addUnit.lineLoss = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        //errorFlag.Add((addUnit.lineLoss > 100 || addUnit.lineLoss < 0) ? true : false);
                        if (addUnit.lineLoss > 100)
                        {
                            m_ErrorLog.SetError("," + ": Line loss can not be more than 100,");
                            errorFlag.Add(true);
                        }
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindMonthlyLineLoss," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertWindMonthlyLineLoss: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindMonthlyLineLoss", msg);
                        errorCount++;
                    }
                }
                //validation success
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly Line Loss Validation Successful,");
                    //api call used for importing wind:monthly linelosses client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly Line Loss Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly Line Loss Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    m_ErrorLog.SetError(",Wind Monthly Line Loss Validation Failed,");
                }
            }
            else
            {
                // add to error log that validation of file failed
                m_ErrorLog.SetError(",Wind Monthly Line Loss File empty,");
            }
            //InformationLog("InsertWindMonthlyLineLoss function Completed : " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertSolarMonthlyTargetKPI(string status, DataSet ds)
        {
            //InformationLog("InsertSolarMonthlyTargetKPI function Called : " + DateTime.Now);
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarMonthlyTargetKPI> addSet = new List<SolarMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarMonthlyTargetKPI addUnit = new SolarMonthlyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        //addUnit.month_no = Convert.ToInt32(MonthList[addUnit.Month]);
                        addUnit.Month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.Month) ? Convert.ToInt32(MonthList[addUnit.Month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.Month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.FY.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? year : year + 1;
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        //addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);
                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.GHI = Convert.ToDouble((dr[3] is DBNull) || string.IsNullOrEmpty((string)dr[3]) ? 0 : dr[3]);
                        errorFlag.Add(numericNullValidation(addUnit.GHI, "GHI", rowNumber));

                        addUnit.POA = Convert.ToDouble((dr[4] is DBNull) || string.IsNullOrEmpty((string)dr[4]) ? 0 : dr[4]);
                        errorFlag.Add(numericNullValidation(addUnit.POA, "POA", rowNumber));

                        addUnit.kWh = Convert.ToDouble((dr[5] is DBNull) || string.IsNullOrEmpty((string)dr[5]) ? 0 : dr[5]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kWh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA (%)"]), "MA (%)");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);

                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA (%)"]), "IGA (%)");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);

                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA (%)"]), "EGA (%)");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);

                        addUnit.PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PR (%)"]), "PR (%)");
                        errorFlag.Add((addUnit.PR > 100 || addUnit.PR < 0) ? true : false);

                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF (%)"]), "PLF (%)");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_KPI(addUnit, addSet, rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyTargetKPI," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarMonthlyTargetKPI: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarMonthlyTargetKPI", msg);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly Target KPI Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly Target KPI Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly Target KPI Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly Target KPI Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Solar Monthly Target KPI File empty,");
            }
            // InformationLog("InsertSolarMonthlyTargetKPI function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyTargetKPI(string status, DataSet ds)
        {
            // InformationLog("InsertWindMonthlyTargetKPI function Called : " + DateTime.Now);
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyTargetKPI> addSet = new List<WindMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                        WindMonthlyTargetKPI addUnit = new WindMonthlyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.month = string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = string.IsNullOrEmpty((string)dr["Month"]) ? 0 : Convert.ToInt32(MonthList[addUnit.month]);
                        errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.fy.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no < 4 ? year + 1 : year);
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Sites"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        addUnit.site_id = string.IsNullOrEmpty(addUnit.site) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);//C
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.windSpeed = string.IsNullOrEmpty((string)dr["WindSpeed"]) ? 0 : Convert.ToDouble(dr["WindSpeed"]);
                        errorFlag.Add(numericNullValidation(addUnit.windSpeed, "WindSpeed", rowNumber));

                        addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        errorFlag.Add(negativeNullValidation(addUnit.kwh, "kwh", rowNumber));

                        addUnit.ma = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                        errorFlag.Add((addUnit.ma > 100 || addUnit.ma < 0) ? true : false);
                        addUnit.iga = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                        errorFlag.Add((addUnit.iga > 100 || addUnit.iga < 0) ? true : false);
                        addUnit.ega = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                        errorFlag.Add((addUnit.ega > 100 || addUnit.ega < 0) ? true : false);
                        addUnit.plf = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                        errorFlag.Add((addUnit.ega > 100 || addUnit.plf < 0) ? true : false);
                        errorFlag.Add(MonthList.Contains(addUnit.month) ? false : true);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindMonthlyTargetKPI," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertWindMonthlyTargetKPI: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindMonthlyTargetKPI", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly Target KPI Validation Successful,");
                    //api call used for importing wind:monthly target kpi client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly Target KPI Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly Target KPI Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    m_ErrorLog.SetError(",Wind Monthly Target KPI Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Wind Monthly Target KPI File empty,");
            }
            // InformationLog("InsertWindMonthlyTargetKPI function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertSolarDailyLoadShedding(string status, DataSet ds)
        {
            //  InformationLog("InsertSolarDailyLoadShedding function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyLoadShedding> addSet = new List<SolarDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarDailyLoadShedding addUnit = new SolarDailyLoadShedding();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.Site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Site) ? Convert.ToInt32(siteNameId[addUnit.Site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");


                        addUnit.Start_Time = (string.IsNullOrEmpty((string)dr["Start Time"])) ? "Nil" : Convert.ToString(dr["Start Time"]);
                        errorFlag.Add(timeValidation(addUnit.Start_Time, "Start Time", rowNumber));

                        addUnit.End_Time = string.IsNullOrEmpty((string)dr["End Time"]) ? "Nil" : Convert.ToString(dr["End Time"]);
                        errorFlag.Add(timeValidation(addUnit.End_Time, "End Time", rowNumber));

                        addUnit.Total_Time = (string.IsNullOrEmpty((string)dr["Total Time"])) ? "Nil" : Convert.ToString(dr["Total Time"]);
                        errorFlag.Add(timeValidation(addUnit.Total_Time, "Total Time", rowNumber));

                        //addUnit.Permissible_Load_MW = Convert.ToDouble(string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]) ? "" : dr[" Permissible Load (MW)"]);
                        bool isNullOrEmpty = dr[" Permissible Load (MW)"] is DBNull || string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]);
                        if (isNullOrEmpty)
                        {

                        }
                        else
                        {
                            addUnit.Permissible_Load_MW =  Convert.ToDouble((string)dr[" Permissible Load (MW)"]);
                        }
                        //errorFlag.Add(numericNullValidation(addUnit.Permissible_Load_MW, " Permissible Load (MW)", rowNumber));

                        addUnit.Gen_loss_kWh = Convert.ToDouble(string.IsNullOrEmpty((string)dr["Generation loss in KWH due to Load shedding"]) ? 0 : dr["Generation loss in KWH due to Load shedding"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Gen_loss_kWh, "Generation loss in KWH due to Load shedding", rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarDailyLoadShedding," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarDailyLoadShedding: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindMonthlyTargetKPI", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Daily Load Shedding Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyLoadShedding";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Daily Load Shedding Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Daily Load Shedding Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Daily Load Shedding Validation Failed,");
                }

            }
            else
            {
                m_ErrorLog.SetError(",Solar Daily Load Shedding File empty,");
            }
            //InformationLog("InsertSolarDailyLoadShedding function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindDailyLoadShedding(string status, DataSet ds)
        {
            //InformationLog("InsertWindDailyLoadShedding function Called : " + DateTime.Now);
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindDailyLoadShedding> addSet = new List<WindDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindDailyLoadShedding addUnit = new WindDailyLoadShedding();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = string.IsNullOrEmpty(addUnit.site) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");


                        addUnit.startTime = string.IsNullOrEmpty((string)dr["Start Time"]) ? "Nil" : Convert.ToString(dr["Start Time"]);
                        errorFlag.Add(timeValidation(addUnit.startTime, "Start Time", rowNumber));

                        addUnit.endTime = string.IsNullOrEmpty((string)dr["End Time"]) ? "Nil" : Convert.ToString(dr["End Time"]);
                        errorFlag.Add(timeValidation(addUnit.startTime, "End Time", rowNumber));

                        addUnit.totalTime = string.IsNullOrEmpty((string)dr["Total Time"]) ? "Nil" : Convert.ToString(dr["Total Time"]);
                        errorFlag.Add(timeValidation(addUnit.totalTime, "Total Time", rowNumber));

                        //addUnit.permLoad = string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]) ? 0 : Convert.ToDouble(dr[" Permissible Load (MW)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.permLoad, " Permissible Load (MW)", rowNumber));
                        bool isNullOrEmpty = dr[" Permissible Load (MW)"] is DBNull || string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]);
                        if (isNullOrEmpty)
                        {

                        }
                        else
                        {
                            addUnit.permLoad = Convert.ToDouble((string)dr[" Permissible Load (MW)"]);
                        }

                        addUnit.genShedding = string.IsNullOrEmpty((string)dr["Generation loss in KWH due to Load shedding"]) ? 0 : Convert.ToDouble(dr["Generation loss in KWH due to Load shedding"]);
                        errorFlag.Add(negativeNullValidation(addUnit.genShedding, "Generation loss in KWH due to Load shedding", rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindDailyLoadShedding," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertWindDailyLoadShedding: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindDailyLoadShedding", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Daily Load Shedding Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindDailyLoadShedding";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Daily Load Shedding API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Daily Load Shedding API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Daily Load Shedding Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Daily Load Shedding File is empty,");
            }
            //InformationLog("InsertWindDailyLoadShedding function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertSolarDailyTargetKPI(string status, DataSet ds)
        {
            //InformationLog("InsertSolarDailyTargetKPI function Called : " + DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<SolarDailyTargetKPI> addSet = new List<SolarDailyTargetKPI>();
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        rowNumber++;
                        SolarDailyTargetKPI addUnit = new SolarDailyTargetKPI();
                        bool skipRow = false;
                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.GHI = Convert.ToDouble((dr[3] is DBNull) ? 0 : dr[3]);
                        errorFlag.Add(numericNullValidation(addUnit.GHI, " GHI", rowNumber));

                        addUnit.POA = Convert.ToDouble((dr[4] is DBNull) ? 0 : dr[4]);
                        errorFlag.Add(numericNullValidation(addUnit.POA, " POA", rowNumber));

                        addUnit.kWh = Convert.ToDouble((dr[5] is DBNull) ? 0 : dr[5]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kwh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA (%)"]), "MA (%)");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);
                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA (%)"]), "IGA (%)");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);
                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA (%)"]), "EGA (%)");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);
                        addUnit.PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PR (%)"]), "PR (%)");
                        errorFlag.Add((addUnit.PR > 100 || addUnit.PR < 0) ? true : false);
                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF (%)"]), "PLF (%)");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);

                        addUnit.Toplining_kWh = Convert.ToDouble((dr[11] is DBNull) ? 0 : dr[11]);
                        errorFlag.Add(negativeNullValidation(addUnit.Toplining_kWh, "Toplining kWh (in MU)", rowNumber));

                        /*addUnit.Toplining_kWh = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining kWh (in MU)"]), "Toplining kWh (in MU)");
                        errorFlag.Add((addUnit.Toplining_kWh > 100 || addUnit.Toplining_kWh < 0) ? true : false); */

                        //addUnit.ghi_1 = Convert.ToDouble((dr["GHI-1"] is DBNull) || string.IsNullOrEmpty((string)dr["GHI-1"]) || ((string)dr["GHI-1"] == "") ? 0 : dr["GHI-1"]);

                        addUnit.Toplining_MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining MA (%)"]), "Toplining MA (%)");
                        errorFlag.Add((addUnit.Toplining_MA > 100 || addUnit.Toplining_MA < 0) ? true : false);

                        addUnit.Toplining_IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining IGA (%)"]), "Toplining IGA (%)");
                        errorFlag.Add((addUnit.Toplining_IGA > 100 || addUnit.Toplining_IGA < 0) ? true : false);

                        addUnit.Toplining_EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining EGA (%)"]), "Toplining EGA (%)");
                        errorFlag.Add((addUnit.Toplining_EGA > 100 || addUnit.Toplining_EGA < 0) ? true : false);

                        addUnit.Toplining_PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining PR (%)"]), "Toplining PR (%)");
                        errorFlag.Add((addUnit.Toplining_PR > 100 || addUnit.Toplining_PR < 0) ? true : false);

                        addUnit.Toplining_PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Toplining PLF (%)"]), "Toplining PLF (%)");
                        errorFlag.Add((addUnit.Toplining_PLF > 100 || addUnit.Toplining_PLF < 0) ? true : false);

                        addUnit.Plant_kWh = Convert.ToDouble((dr[17] is DBNull) ? 0 : dr[17]);
                        errorFlag.Add(negativeNullValidation(addUnit.Plant_kWh, "Plant kWH (in MU)", rowNumber));

                        addUnit.Plant_PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Plant PR (%)"]), "Plant PR (%)");
                        errorFlag.Add((addUnit.Plant_PR > 100 || addUnit.Plant_PR < 0) ? true : false);

                        addUnit.Plant_PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Plant PLF (%)"]), "Plant PLF (%)");
                        errorFlag.Add((addUnit.Plant_PLF > 100 || addUnit.Plant_PLF < 0) ? true : false);

                        addUnit.Inv_kWh = Convert.ToDouble((dr[20] is DBNull) ? 0 : dr[20]);
                        errorFlag.Add(negativeNullValidation(addUnit.Inv_kWh, "Inv kWh (in MU)", rowNumber));

                        /* addUnit.Inv_kWh = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Inv kWh (in MU)"]), "Inv kWh (in MU)");
                        errorFlag.Add((addUnit.Inv_kWh > 100 || addUnit.Inv_kWh < 0) ? true : false); */

                        addUnit.Inv_PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Inv PR (%)"]), "Inv PR (%)");
                        errorFlag.Add((addUnit.Inv_PR > 100 || addUnit.Inv_PR < 0) ? true : false);

                        addUnit.Inv_PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Inv PLF (%)"]), "Inv PLF (%)");
                        errorFlag.Add((addUnit.Inv_PLF > 100 || addUnit.Inv_PLF < 0) ? true : false);

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarDailyTargetKPI" + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarDailyTargetKPI: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarDailyTargetKPI", msg);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Daily Target KPI Validation SuccessFul");

                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Daily Target KPI API Successful");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Daily Target KPI API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Daily Target KPI Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Solar Daily Target KPI File is empty,");
            }
            //InformationLog("InsertSolarDailyTargetKPI function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindDailyTargetKPI(string status, DataSet ds)
        {
            //InformationLog("InsertWindDailyTargetKPI function Called : " + DateTime.Now);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindDailyTargetKPI> addSet = new List<WindDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindDailyTargetKPI addUnit = new WindDailyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.FY = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        addUnit.Site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.Site]);
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.WindSpeed = string.IsNullOrEmpty((string)dr["WindSpeed"]) ? 0 : Convert.ToDouble(dr["WindSpeed"]);
                        errorFlag.Add(numericNullValidation(addUnit.WindSpeed, "WindSpeed", rowNumber));

                        addUnit.kWh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kWh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);
                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);
                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);
                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindDailyTargetKPI," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertWindDailyTargetKPI: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindDailyTargetKPI", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Daily Target KPI Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Daily Target KPI API Success,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Daily Target KPI API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Daily Target KPI Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Daily Target KPI File is empty,");
            }
            // InformationLog("InsertWindDailyTargetKPI function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertSolarSiteMaster(string status, DataSet ds)
        {
            // InformationLog("InsertSolarSiteMaster function Called : " + DateTime.Now);
            int responseCode = 400;
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 0;
            int errorCount = 0;
            if (ds.Tables.Count > 0)
            {
                List<SolarSiteMaster> addSet = new List<SolarSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarSiteMaster addUnit = new SolarSiteMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = Convert.ToString(dr["Country"]);
                        errorFlag.Add(stringNullValidation(addUnit.country, "Country", rowNumber));

                        addUnit.site = Convert.ToString(dr["Site"]);
                        errorFlag.Add(stringNullValidation(addUnit.site, "Site", rowNumber));
                        objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C

                        addUnit.spv = Convert.ToString(dr["SPV"]);
                        errorFlag.Add(stringNullValidation(addUnit.spv, "SPV", rowNumber));

                        addUnit.state = Convert.ToString(dr["State"]);
                        errorFlag.Add(stringNullValidation(addUnit.state, "State", rowNumber));

                        addUnit.doc = string.IsNullOrEmpty((string)dr["DOC"]) ? "Nil" : Convert.ToString(dr["DOC"]);
                        errorFlag.Add(stringNullValidation(addUnit.doc, "DOC", rowNumber));

                        addUnit.dc_capacity = Convert.ToDouble(dr["DC Capacity (MWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "DC Capacity (MWp)", rowNumber));

                        addUnit.ac_capacity = Convert.ToDouble(dr["AC Capacity (MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.ac_capacity, "AC Capacity (MW)", rowNumber));

                        addUnit.total_tarrif = Convert.ToDouble(dr["Total Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_tarrif, "Total Tariff", rowNumber));
                        uniqueRecordCheckSolarSiteMaster(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarSiteMaster" + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarSiteMaster: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarSiteMaster", msg);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Solar Site Master Validation SuccessFul");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarSiteMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Site Master API SuccessFul");
                            responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Site Master API Failure: responseCode <" + (int)response.StatusCode + ">");
                            responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Site Master Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Solar Site Master File is empty,");
            }
            //InformationLog("InsertSolarSiteMaster function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindSiteMaster(string status, DataSet ds)
        {
            //InformationLog("InsertWindSiteMaster function Called : " + DateTime.Now);
            int errorCount = 0;
            long rowNumber = 1;
            int responseCode = 400;
            List<bool> errorFlag = new List<bool>();

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindSiteMaster> addSet = new List<WindSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindSiteMaster addUnit = new WindSiteMaster();
                        errorFlag.Clear();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = string.IsNullOrEmpty((string)dr["Country"]) ? "Nil" : Convert.ToString(dr["Country"]);
                        errorFlag.Add(countryValidation(addUnit.country, "Country", rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        errorFlag.Add(stringNullValidation(addUnit.site, "Site", rowNumber));
                        objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C

                        addUnit.spv = string.IsNullOrEmpty((string)dr["SPV"]) ? "Nil" : Convert.ToString(dr["SPV"]);
                        errorFlag.Add(stringNullValidation(addUnit.spv, "SPV", rowNumber));

                        addUnit.state = string.IsNullOrEmpty((string)dr["State"]) ? "Nil" : Convert.ToString(dr["State"]);
                        errorFlag.Add(stringNullValidation(addUnit.state, "State", rowNumber));

                        addUnit.doc = string.IsNullOrEmpty((string)dr["DOC"]) ? "Nil" : Convert.ToString(dr["DOC"]);
                        errorFlag.Add(stringNullValidation(addUnit.doc, "DOC", rowNumber));

                        addUnit.model = string.IsNullOrEmpty((string)dr["Model"]) ? "Nil" : Convert.ToString(dr["Model"]);
                        errorFlag.Add(stringNullValidation(addUnit.model, "Model", rowNumber));

                        addUnit.capacity_mw = string.IsNullOrEmpty((string)dr["Capacity(MW)"]) ? 0 : Convert.ToDouble(dr["Capacity(MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.capacity_mw, "Capacity(MW)", rowNumber));

                        addUnit.wtg = dr["WTG"] is DBNull || string.IsNullOrEmpty((string)dr["WTG"]) ? 0 : Convert.ToDouble(dr["WTG"]);
                        errorFlag.Add(numericNullValidation(addUnit.wtg, "WTG", rowNumber));

                        addUnit.total_mw = string.IsNullOrEmpty((string)dr["Total_MW"]) ? 0 : Convert.ToDouble(dr["Total_MW"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_mw, "Total_MW", rowNumber));

                        addUnit.tarrif = string.IsNullOrEmpty((string)dr["Tariff"]) ? 0 : Convert.ToDouble(dr["Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.tarrif, "Tariff", rowNumber));

                        addUnit.gbi = string.IsNullOrEmpty((string)dr["GBI"]) ? 0 : Convert.ToDouble(dr["GBI"]);
                        // errorFlag.Add(numericNullValidation(addUnit.gbi, "GBI", rowNumber));
                        //addUnit.gbi = Convert.ToDouble((string)dr["GBI"]);
                        // errorFlag.Add(numericNullValidation(addUnit.gbi, "GBI", rowNumber));

                        addUnit.total_tarrif = string.IsNullOrEmpty((string)dr["Total_Tariff"]) ? 0 : Convert.ToDouble(dr["Total_Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_tarrif, "Total_Tariff", rowNumber));

                        addUnit.ll_compensation = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["LL_Compensation%"]), "LL_Compensation%");
                        uniqueRecordCheckWindSiteMaster(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindSiteMaster," + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertWindSiteMaster: " + e.Message + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindSiteMaster", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Site Master Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindSiteMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Site Master API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Site Master API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Site Master Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Site Master File is empty,");
            }
            //InformationLog("InsertWindSiteMaster function Completed : " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertSolarLocationMaster(string status, DataSet ds)
        {//siteID recorded
            //InformationLog("InsertSolarLocationMaster function Called : " + DateTime.Now);
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarLocationMaster> addSet = new List<SolarLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarLocationMaster addUnit = new SolarLocationMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = dr["Country"] is DBNull || string.IsNullOrEmpty((string)dr["Country"]) ? "Nil" : Convert.ToString(dr["Country"]);
                        errorFlag.Add(countryValidation(addUnit.country, "Country", rowNumber));
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.eg = Convert.ToString(dr["EG"]);
                        errorFlag.Add(stringNullValidation(addUnit.eg, "EG", rowNumber));

                        addUnit.ig = Convert.ToString(dr["IG"]);
                        errorFlag.Add(stringNullValidation(addUnit.ig, "IG", rowNumber));

                        addUnit.icr_inv = Convert.ToString(dr["ICR/INV"]);
                        errorFlag.Add(stringNullValidation(addUnit.icr_inv, "ICR/INV", rowNumber));

                        addUnit.icr = Convert.ToString(dr["ICR"]);
                        errorFlag.Add(stringNullValidation(addUnit.icr, "ICR", rowNumber));

                        addUnit.inv = Convert.ToString(dr["INV"]);
                        errorFlag.Add(stringNullValidation(addUnit.inv, "INV", rowNumber));
                        //SMB can remain Nil
                        addUnit.smb = Convert.ToString(dr["SMB"] is DBNull || string.IsNullOrEmpty((string)dr["SMB"]) ? "Nil" : dr["SMB"]);

                        addUnit.strings = Convert.ToString(dr["String"]);
                        errorFlag.Add(stringNullValidation(addUnit.strings, "String", rowNumber));

                        addUnit.string_configuration = Convert.ToString(dr["String Configuration"] is DBNull || string.IsNullOrEmpty((string)dr["String Configuration"]) ? "Nil" : dr["String Configuration"]);
                        errorFlag.Add(stringNullValidation(addUnit.string_configuration, "String", rowNumber));

                        addUnit.total_string_current = Convert.ToDouble(dr["Total String Current (amp)"] is DBNull || string.IsNullOrEmpty((string)dr["Total String Current (amp)"]) ? 0 : dr["Total String Current (amp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_string_current, "Total String Current (amp)", rowNumber));

                        addUnit.total_string_voltage = Convert.ToDouble(dr["Total String voltage"] is DBNull || string.IsNullOrEmpty((string)dr["Total String voltage"]) ? 0 : dr["Total String voltage"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_string_voltage, "Total String voltage", rowNumber));

                        addUnit.modules_quantity = Convert.ToDouble(dr[12] is DBNull || string.IsNullOrEmpty((string)dr[12]) ? 0 : dr[12]);
                        errorFlag.Add(numericNullValidation(addUnit.modules_quantity, "Modules Qty", rowNumber));

                        addUnit.wp = Convert.ToDouble(dr["Wp"] is DBNull || string.IsNullOrEmpty((string)dr["Wp"]) ? 0 : (dr["Wp"]));
                        errorFlag.Add(numericNullValidation(addUnit.wp, "Wp", rowNumber));

                        addUnit.capacity = Convert.ToDouble(dr["Capacity (KWp)"] is DBNull || string.IsNullOrEmpty((string)dr["Capacity (KWp)"]) ? 0 : dr["Capacity (KWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.modules_quantity, "Modules", rowNumber));

                        addUnit.module_make = Convert.ToString(dr["Module Make"]);
                        errorFlag.Add(stringNullValidation(addUnit.module_make, "Module Make", rowNumber));

                        addUnit.module_model_no = Convert.ToString(dr[16]);
                        errorFlag.Add(stringNullValidation(addUnit.module_model_no, "Module Model No.", rowNumber));

                        addUnit.module_type = Convert.ToString(dr["Module Type"]);
                        errorFlag.Add(stringNullValidation(addUnit.module_type, "Module Type", rowNumber));

                        addUnit.string_inv_central_inv = (Convert.ToString(dr["String Inv / Central Inv"]) == "Central Inverter") ? 2 : 1;
                        errorFlag.Add(stringNullValidation((string)dr["String Inv / Central Inv"], "String Inv / Central Inv", rowNumber));

                        errorFlag.Add(uniqueRecordCheckSolarLocationMaster(addUnit, addSet, rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarLocationMaster" + rowNumber);
                        string msg = ",Exception Occurred In Function: InsertSolarLocationMaster: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarLocationMaster", msg);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Solar Location Master Validation SuccessFul,");

                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarLocationMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Location Master API SuccessFul");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Location Master API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Location Master Validation Failed");
                }
            }
            //InformationLog("InsertSolarLocationMaster function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task<int> InsertWindLocationMaster(string status, DataSet ds)
        {
            //InformationLog("InsertWindLocationMaster function Called : " + DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            bool errorInRow = false;
            bool bValidationFailed = false;
            if (ds.Tables.Count > 0)
            {
                List<WindLocationMaster> addSet = new List<WindLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindLocationMaster addUnit = new WindLocationMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : (string)dr["Site"];
                        addUnit.site_master_id = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_master_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_master_id;

                        addUnit.wtg = string.IsNullOrEmpty((string)dr["WTG"]) ? "Nil" : (string)dr["WTG"];
                        errorFlag.Add(stringNullValidation(addUnit.wtg, "WTG", rowNumber));

                        addUnit.wtg_onm = string.IsNullOrEmpty((string)dr["ONM_WTG"]) ? "Nil" : (string)dr["ONM_WTG"];
                        errorFlag.Add(stringNullValidation(addUnit.wtg, "ONM_WTG", rowNumber));

                        addUnit.feeder = string.IsNullOrEmpty((string)dr["Feeder"]) ? 0 : Convert.ToDouble(dr["Feeder"]);
                        errorFlag.Add(numericNullValidation(addUnit.feeder, "Feeder", rowNumber));

                        addUnit.max_kwh_day = string.IsNullOrEmpty((string)dr["Max. kWh/Day"]) ? 0 : Convert.ToDouble(dr["Max. kWh/Day"]);
                        errorFlag.Add(negativeNullValidation(addUnit.max_kwh_day, "Max. kWh/Day", rowNumber));

                        //uniqueRecordCheckWtgWise();
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindLocationMaster");
                        string msg = ",Exception Occurred In Function: InsertWindLocationMaster: " + e.ToString();
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindLocationMaster", msg);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Wind Location Master Validation SuccessFul");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindLocationMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Location Master API SuccessFul");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Location Master API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Location Master Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Location Master File is emplty,");
            }
            //InformationLog("InsertWindLocationMaster function Completed" +
            //    ": " + DateTime.Now);
            return responseCode;
        }

        private async Task<int> InsertSolarAcDcCapacity(string status, DataSet ds)
        {
            //InformationLog("InsertSolarAcDcCapacity function Called : " + DateTime.Now);
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarInvAcDcCapacity> addSet = new List<SolarInvAcDcCapacity>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarInvAcDcCapacity addUnit = new SolarInvAcDcCapacity();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        if(rowNumber == 2)
                        {
                            masterInverterList(addUnit.site);
                        }
                        addUnit.inverter = Convert.ToString(dr["Inverter"]);
                        errorFlag.Add(solarInverterValidation((string)dr["Inverter"], "Inverter", rowNumber));

                        addUnit.dc_capacity = dr["DC Capacity(kWp)"] is DBNull || string.IsNullOrEmpty((string)dr["DC Capacity(kWp)"]) ? 0 : Convert.ToDouble(dr["DC Capacity(kWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "DC Capacity(kWp)", rowNumber));

                        addUnit.ac_capacity = dr["AC Capacity (kW)"] is DBNull || string.IsNullOrEmpty((string)dr["AC Capacity (kW)"]) ? 0 : Convert.ToDouble(dr["AC Capacity (kW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "AC Capacity (kW)", rowNumber));
                        uniqueRecordCheckAcDcCapacity(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarAcDcCapacity,");
                        string msg = ",Exception Occurred In Function: InsertSolarAcDcCapacity: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarAcDcCapacity", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar ACDC Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarInvAcDcCapacity";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar ACDC API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar ACDC API Failure,: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar ACDC Validation Failed,");
                }
            }
            //InformationLog("InsertSolarAcDcCapacity function Completed : " + DateTime.Now);
            return responseCode;
        }
        private async Task importMetaData(string importType, string fileName, FileSheetType.FileImportType fileImportType)
        {
            int responseCode = 400;
            string status = "";
            objImportBatch.importFilePath = fileName;
            objImportBatch.importLogName = importData[1];
            string userName = HttpContext.Session.GetString("DisplayName");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            if (importType == "Solar")
            {
                objImportBatch.importType = "2";
            }
            else if (importType == "Wind")
            {
                objImportBatch.importType = "1";
            }
            objImportBatch.importFileType = (int)fileImportType;
            var json = JsonConvert.SerializeObject(objImportBatch);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/importMetaData?userName=" + userName + "&userId=" + userId;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, data);
                if (response.IsSuccessStatusCode)
                {
                    //status = "Batch Id Created Successfully";
                    //m_ErrorLog.SetInformation("," + status + ":");
                    responseCode = (int)response.StatusCode;
                }
                else
                {
                    status = "Batch Id Creation API Failed";
                    m_ErrorLog.SetInformation("," + status + ":");
                    responseCode = (int)response.StatusCode;
                }
            }

            if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min") || fileSheets.Contains("Solar_tracker_loss"))
            {
                var urlGetId = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchId?logFileName=" + importData[1] + "";
                var result = string.Empty;
                WebRequest request = WebRequest.Create(urlGetId);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                    BatchIdImport obj = new BatchIdImport();
                    obj = JsonConvert.DeserializeObject<BatchIdImport>(result);
                    batchIdDGRAutomation = obj.import_batch_id;
                    if (batchIdDGRAutomation == 0)
                    {
                        m_ErrorLog.SetError(",BatchId not returned successfully,");
                    }
                    else
                    {
                        m_ErrorLog.SetInformation(",BatchId <" + batchIdDGRAutomation + "> created successfully,");
                    }
                }
            }
        }

        //HashTable List
        public void masterHashtable_WtgToWtgId()
        {
            //fills a hashtable with key = wtg and value = location_master_id from table : Wind Location Master
            //gets equipmentID from equipmentName in Wind Location Master
            DataTable dTable = new DataTable();
            //InformationLog("Inside masterHashTable_WTG_TO_WTG_ID function : before API call");
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster";
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }

            foreach (DataRow dr in dTable.Rows)
            {
                int wtgId = (int)Convert.ToInt64(dr["location_master_id"]);//D
                equipmentId.Add((string)dr["wtg"], wtgId);
                onm2equipmentName.Add((string)dr["wtg_onm"], (string)dr["wtg"]);
                SiteByWtg.Add((string)dr["wtg"], (string)dr["site"]);
                double max_kWh = Convert.ToDouble(dr["max_kwh_day"]);
                maxkWhMap_wind.Add(wtgId, max_kWh);
            }
        }
        public void masterHashtable_BDNameToBDId()
        {
            //fills a hashtable with key = bdTypeName and value = bdTypeId from table : BDType 
            //gets breakdownId from breakdownName in BDType

            DataTable dTable = new DataTable();
            //InformationLog("masterHashtable_BDNameToBDId function Called Before API Call: " + DateTime.Now);
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBDType";
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                //InformationLog("masterHashtable_BDNameToBDId function Called After API Call: " + DateTime.Now);
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }
            breakdownType.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                int bd_type_id = (int)Convert.ToInt64(dr["bd_type_id"]);//D
                breakdownType.Add((string)dr["bd_type_name"], bd_type_id);
            }
            //InformationLog("masterHashtable_BDNameToBDId function Called Data received : " + breakdownType);
        }
        /*
         * this function was causing duplicate API calls to get sitemaster date. Merged into masterHashtable_SiteIdToSiteName
        public void masterHashtable_SiteName_To_SiteId()
        {
            //fills a hashtable with as key = siteName and value = siteId from table : Wind Site Master
            //gets siteID from siteName in Wind Site Master

            siteNameId.Clear();
            DataTable dTable = new DataTable();
            var url = "";
            if (importData[0] == "Solar")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster";
            }
            else if (importData[0] == "Wind")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster";
            }
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }

            if (importData[0] == "Solar")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
        }
        */
        public void masterInverterList(string siteName)
        {
            DataTable dTable = new DataTable();
            //InformationLog("Inside masterInverterList function : Before getSolarLocationMaster API call :" + DateTime.Now);
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarLocationMasterForFileUpload?siteName=" + siteName;
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }
            //InformationLog("Inside masterInverterList function : After getSolarLocationMaster API call :" + DateTime.Now);
            inverterList.Clear();
            IGBD.Clear();
            SMBList.Clear();
            StringsList.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                inverterList.Add((string)dr["icr_inv"]);
                IGBD.Add((string)dr["ig"]);
                SMBList.Add((string)dr["smb"]);
                StringsList.Add((string)dr["strings"]);
            }
            //InformationLog("Inside masterInverterList function : Data Received : Inverter List : " + inverterList + "\n IGBD list " + IGBD + "\n SMBList "+SMBList + "\n StringList "+ StringsList);
        }

        public void masterIcrInvList(string siteName)
        {
            DataTable dTable = new DataTable();
            //InformationLog("Inside masterInverterList function : Before getSolarLocationMaster API call :" + DateTime.Now);
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarLocationMasterForFileUpload?siteName=" + siteName;
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }
            //InformationLog("Inside masterInverterList function : After getSolarLocationMaster API call :" + DateTime.Now);
            icrList.Clear();
            invList.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                icrList.Add((string)dr["icr"]);
                invList.Add((string)dr["inv"]);
            }
        }
        public void masterHashtable_SiteIdToSiteName()
        {
            //fills a hashtable with as key = siteId and value = siteNameId from table : Wind Site Master
            //gets siteName from siteId in Wind Site Master
            //InformationLog("masterHashtable_SiteIdToSiteName function Called Before API Call: " + DateTime.Now);
            siteName.Clear();
            DataTable dTable = new DataTable();
            var url = "";
            if (importData[0] == "Solar")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster";
            }
            else if (importData[0] == "Wind")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster";
            }
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                //InformationLog("masterHashtable_SiteIdToSiteName function Called After API Call: " + DateTime.Now);

                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }
            if (importData[0] == "Solar")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D
                    siteName.Add(siteMasterId, (string)dr["site"]);
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteName.Add(siteMasterId, (string)dr["site"]);
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
            //InformationLog("masterHashtable_SiteIdToSiteName function Data Received : \n Site Name :" + siteName +"\n Site NameId  " +siteNameId );
        }
        public void masterHashtable_WtgToSiteId()
        {
            //fills a hashtable with as key = siteId and value = siteNameId from table : Wind Site Master
            //gets siteId from wtg(equipment) in Wind Location Master

            DataTable dTable = new DataTable();
            //InformationLog("Inside masterHashtable_WtgToSiteId function : before getwindlocationmaster API : "+ DateTime.Now);
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster";
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }
            //InformationLog("Inside masterHashtable_WtgToSiteId function : after getwindlocationmaster API : " + DateTime.Now);
            eqSiteId.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                eqSiteId.Add((string)dr["wtg"], siteMasterId);
            }
            //InformationLog("masterHashtable_WtgToSiteId Received data : " + eqSiteId);
        }
        private async Task<bool> UploadFileToImportedFileFolder(IFormFile ufile)
        {
            bool retValue = false;
            if (ufile != null && ufile.Length > 0)
            {
                var fileName = Path.GetFileName(ufile.FileName);
                var filePath = env.ContentRootPath + @"C:\ImportedFile\" + fileName;
                //var filePath = env.ContentRootPath + @"\ImportedFile\" + fileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ufile.CopyToAsync(fileStream);
                }
                retValue = true;
            }
            return retValue;
        }

        // Automation files import.
        public async Task<int> dgrSolarImport(int batchId)
        {
            //InformationLog("Inside dgrSolarImport<br>\r\n");
            LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", "Inside dgrSolarImport");
            int responseCodeGen = 0;
            int responseCodeBreak = 0;
            int responseCodePyro1 = 0;
            int responseCodePyro15 = 0;
            int responseCodeTracker = 0;

            var dataGeneration = new StringContent(genJson, Encoding.UTF8, "application/json");
            var urlGeneration = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileGeneration?batchId=" + batchId + "";
            //InformationLog("Gen Url<br>\r\n" + urlGeneration);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                var response = await client.PostAsync(urlGeneration, dataGeneration);
                responseCodeGen = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Solar Gen Import API Successful");
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Generation Import API Failed");
                }
            }

            var dataBreakdown = new StringContent(breakJson, Encoding.UTF8, "application/json");
            var urlBreakdown = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileBreakDown?batchId=" + batchId + "";
            //InformationLog("BreakDown\r\n" + urlBreakdown);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                var response = await client.PostAsync(urlBreakdown, dataBreakdown);
                responseCodeBreak = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Solar Break Import API Successful");
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Break Import API Failed. Error code <" + responseCodeBreak + ">");
                }
            }

            var dataPyro1Min = new StringContent(pyro1Json, Encoding.UTF8, "application/json");
            var urlPyro1Min = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter1Min?batchId=" + batchId + "";
            //InformationLog("1Min\r\n" + urlPyro1Min);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                var response = await client.PostAsync(urlPyro1Min, dataPyro1Min);
                responseCodePyro1 = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Solar Pyro-1 Import API Successful");
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Pyro-1 Import API Failed. Error code <" + responseCodePyro1 + ">");
                }
            }

            var dataPyro15Min = new StringContent(pyro15Json, Encoding.UTF8, "application/json");
            var urlPyro15Min = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter15Min?batchId=" + batchId + "";
            //ErrorLog("15Min\r\n" + urlPyro15Min);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                var response = await client.PostAsync(urlPyro15Min, dataPyro15Min);
                responseCodePyro15 = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Solar Pyro-15 Import API Successful");
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Pyro-15 Import API Failed. Error code <" + responseCodePyro15 + ">");
                }
            }

            var dataTrackerloss = new StringContent(trackerJson, Encoding.UTF8, "application/json");
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarTrackerLoss";
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                var response = await client.PostAsync(url, dataTrackerloss);
                responseCodeTracker = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Solar Tracker Loss API SuccessFul,");
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Tracker Loss API Failure,: responseCode <" + (int)response.StatusCode + ">");

                    //for solar 0, wind 1, other 2;
                    //int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                    //if (deleteStatus == 1)
                    //{
                    //    m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                    //}
                    //else
                    //{
                    //    m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                    //}
                }
            }

            if (responseCodeGen == 200 && responseCodeBreak == 200 && responseCodePyro1 == 200 && responseCodePyro15 == 200 && responseCodeTracker == 200)
            {
                //pending : Need to add code to cleanup import that is failed.
                return 200;
            }
            else
            {
                return 400;
            }
        }
        public async Task<int> dgrWindImport(int batchId)
        {
            int finalResult = 0;
            int responseCodeGen = 0;
            int responseCodeBreak = 0;

            var dataGeneration = new StringContent(genJson, Encoding.UTF8, "application/json");
            var urlGeneration = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileGeneration?batchId=" + batchId + "";
            //ErrorLog(urlGeneration);
            // ErrorLog(breakJson);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlGeneration, dataGeneration);
                responseCodeGen = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Wind Generation Import API Successful");
                    //InformationLog("InsertWindUploadingFileGeneration success ");
                }
                else
                {
                    //responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind Generation Import API Failed. Error code <" + responseCodeGen + ">\r\n");
                    //InformationLog("InsertWindUploadingFileGeneration failed code line 3725");

                }
            }
            var dataBreakdown = new StringContent(breakJson, Encoding.UTF8, "application/json");
            var urlBreakdown = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileBreakDown?batchId=" + batchId + "";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlBreakdown, dataBreakdown);
                responseCodeBreak = (int)response.StatusCode;
                string returnResponse = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    m_ErrorLog.SetInformation(",Wind BreakDown Import API Successful:");
                    // InformationLog("InsertWindUploadingFileBreakDown success ");

                }
                else
                {
                    //responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind BreakDown Import API Failed. Error code <" + responseCodeBreak + ">\r\n");
                    // InformationLog("InsertWindUploadingFileBreakDown Failed code line :3745 " + responseCodeBreak);

                }
            }
            if (responseCodeGen == 200 && responseCodeBreak == 200)
            {
                return 200;
            }
            else
            {
                return 400;
            }
        }

        //Error Logging into text file.
        private void ErrorLog(string Message)
        {
            System.IO.File.AppendAllText(@"C:\LogFile\test.txt", "***Validaion ERROR***" + Message + "\r\n");
        }
        private void InformationLog(string Message)
        {
            System.IO.File.AppendAllText(@"C:\LogFile\test.txt", "*Validaion Information*" + Message + "\r\n");
        }

        //Validation Functions
        public bool uniqueRecordCheckWtgWise(string value, string columnName, long rowNo, Hashtable equipmentList)
        {
            bool retValue = false;
            if (equipmentList.ContainsKey(value))
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> column <" + columnName + ">: WTG <" + value + "> already exists. Data will be updated, ");
            }
            return retValue;
        }
        public bool uniqueRecordCheckSolarLocationMaster(SolarLocationMaster currentRecord, List<SolarLocationMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarLocationMaster existingRecord = new SolarLocationMaster();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tableRecord => tableRecord.site_id.Equals(currentRecord.site_id) && tableRecord.icr.Equals(currentRecord.icr) && tableRecord.inv.Equals(currentRecord.inv) && tableRecord.smb.Equals(currentRecord.smb) && tableRecord.strings.Equals(currentRecord.strings));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> Duplicate record error: There already exists a row in excel sheet with following values: Site<" + currentRecord.site + ">; ICR<" + currentRecord.icr + ">; INV<" + currentRecord.inv + ">; SMB<" + currentRecord.smb + ">; Strings<" + currentRecord.strings + ">,");
                retVal = true;
            }
            return retVal;
        }
        public bool uniqueRecordCheckSolarPerMonthYear_JMR(SolarMonthlyJMR thisRecord, List<SolarMonthlyJMR> tableData, long rowNo)
        {
            bool retValue = false;
            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyJMR existingRecord = tableData.Find(tableRecord => tableRecord.site_id.Equals(thisRecord.site_id) && tableRecord.JMR_Year.Equals(thisRecord.JMR_Year) && tableRecord.JMR_Month_no.Equals(thisRecord.JMR_Month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Site + ">, year<" + thisRecord.JMR_Year + ">, month<" + thisRecord.JMR_Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
        }
        public bool uniqueRecordCheckSolarPerMonthYear_LineLoss(SolarMonthlyUploadingLineLosses thisRecord, List<SolarMonthlyUploadingLineLosses> tableData, long rowNo)
        {
            bool retValue = false;
            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyUploadingLineLosses existingRecord = tableData.Find(tableRecord => tableRecord.Site_Id.Equals(thisRecord.Site_Id) && tableRecord.year.Equals(thisRecord.year) && tableRecord.month_no.Equals(thisRecord.month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Sites + ">, year<" + thisRecord.year + ">, month<" + thisRecord.Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
            //SetInformation instead of error : 
        }
        public bool uniqueRecordCheckSolarPerMonthYear_KPI(SolarMonthlyTargetKPI thisRecord, List<SolarMonthlyTargetKPI> tableData, long rowNo)
        {
            bool retValue = false;

            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyTargetKPI existingRecord = tableData.Find(tableRecord => tableRecord.Site_Id.Equals(thisRecord.Site_Id) && tableRecord.year.Equals(thisRecord.year) && tableRecord.month_no.Equals(thisRecord.month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Sites + ">, year<" + thisRecord.year + ">, month<" + thisRecord.Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
        }
        public bool uniqueRecordCheckAcDcCapacity(SolarInvAcDcCapacity currentRecord, List<SolarInvAcDcCapacity> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarInvAcDcCapacity existingRecord = new SolarInvAcDcCapacity();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tSite => (tSite.site_id == currentRecord.site_id && tSite.inverter == currentRecord.inverter));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + ">; Inverter <" + currentRecord.inverter + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }
        public bool uniqueRecordCheckSolarSiteMaster(SolarSiteMaster currentRecord, List<SolarSiteMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarSiteMaster existingRecord = new SolarSiteMaster();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tableRecord => tableRecord.site.Equals(currentRecord.site));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }
        public bool uniqueRecordCheckWindSiteMaster(WindSiteMaster currentRecord, List<WindSiteMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            WindSiteMaster existingRecord = new WindSiteMaster();
            existingRecord = recordSet.Find(tableRecord => tableRecord.site.Equals(currentRecord.site));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }
        public bool uniformDateValidation(string date, int siteId, dynamic recordSet)
        {
            bool retVal = false;
            //doubt as to how date and site should be checked
            return retVal;
        }
        public bool uniformWindSiteValidation(long rowNo, int siteId, string wtg)
        {
            bool invalidSiteUniformity = false;

            if (rowNo == 2)
            {
                previousSite = siteId;
                if (fileSheets.Contains("Uploading_File_Generation") && importData[0] == "Wind")
                {
                    if (siteId == 0)
                    {
                        m_ErrorLog.SetError(",File Row <" + rowNo + "> Invalid Site Id: " + siteId + " due to invalid wtg: " + wtg);
                    }
                    //else if (windSiteUserAccess.IndexOf(siteId)<-1)
                    else if (windSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        //m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                        invalidSiteUniformity = true;
                    }

                }
                else if (fileSheets.Contains("Uploading_File_Generation") && importData[0] == "Solar")
                {
                    if (siteId == 0)
                    {
                        m_ErrorLog.SetError(",File Row <" + rowNo + "> Invalid Site Id: " + siteId + " due to invalid wtg: " + wtg);
                    }
                    //else if (solarSiteUserAccess.IndexOf(siteId) < -1)
                    else if (solarSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                        invalidSiteUniformity = true;
                    }
                }
                else
                {
                    /*if (windSiteUserAccess.Contains(siteId))
                     {
                         //add error log : user has access to site
                         m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                     }
                     else
                     {
                         //add error log : user does not have access to site
                         m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                         invalidSiteUniformity = true;
                     }*/
                }
                //valdiate if the user has access to this site
                //Collection of site for upload access
                //Check if this siteid is in the above collection of sites accessible to the user
                //Get Site name should be in csv file
            }
            if (previousSite != siteId)
            {
                //pending log error in csv
                m_ErrorLog.SetError(",File Row <" + rowNo + "> Site entry is not the same as in other rows,");
                invalidSiteUniformity = true;
            }
            return invalidSiteUniformity;
        }
        public bool importDateValidation(int importType, int siteID, DateTime importDate)
        {
            bool retValue = false;
            DateTime dtToday = DateTime.Now;
            string currentDate = dtToday.ToString("yyyy-MM-dd");
            string docDate = importDate.ToString("yyyy-MM-dd");
            bool SameDate = true;
            if (currentDate == docDate)
            {
                SameDate = false;
            }
            DateTime CrDate = Convert.ToDateTime(currentDate);
            DateTime dtImportDate = Convert.ToDateTime(importDate);
            TimeSpan dayDiff = CrDate - dtImportDate;
           
            int dayOfWeek = (int)dtToday.DayOfWeek;


            //for DayOfWeek function 
            //if it's not true that file-date is of previous day and today is from Tuesday-Friday
            //&& dayOfWeek > 1 && dayOfWeek < 6
            if (SameDate)
            {
                if (dayDiff.Days < 0)
                {
                    m_ErrorLog.SetError(",The import date <" + importDate + ">  is of future, so cannot import this.");
                    retValue = true;
                }
                else
                {
                    if (!(dayDiff.Days >= 0 && dayDiff.Days <= 5))
                    {
                        if (siteUserRole == "Admin")
                        {
                            m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the admin user can import it.");
                        }
                        else
                        {
                            // file date is incorrect
                            m_ErrorLog.SetError(",The import date <" + importDate + ">  is more than 5 days older but the site user cannot import it.");
                            retValue = true;
                        }

                    }
                    if (retValue == false)
                    {
                        //if date is within 5 days
                        //Check if the data is already import and/or Approved

                        int IBStatus = 0;
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchStatus?site_id=" + siteID + "&import_type=" + importType + "&import_date=" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd");

                        string result = "";
                        // string line = "";
                        WebRequest request = WebRequest.Create(url);
                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            Stream receiveStream = response.GetResponseStream();
                            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                            {
                                // result = readStream.ReadToEnd();
                                result = readStream.ReadToEnd().Trim();
                            }
                            //ImportBatchStatus obj = new ImportBatchStatus();
                            //obj = JsonConvert.DeserializeObject<ImportBatchStatus>(result);
                            //obj = JsonConvert.DeserializeObject<ImportBatchStatus>(result);

                            IBStatus = Convert.ToInt32(result);
                            if (IBStatus == 1)
                            {
                                if (siteUserRole == "Admin")
                                {
                                    m_ErrorLog.SetInformation(",Data for <" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd") + "> exist in database and is already approved but the admin user can reimport it.");
                                }
                                else
                                {
                                    m_ErrorLog.SetError(",Data for <" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd") + "> exist in database and is already approved. The site user cannot reimport it.");
                                    retValue = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                m_ErrorLog.SetError(",The import date <" + importDate + ">  is of future, so cannot import this.");
                retValue = true;
            }
            return retValue;
        }
        public bool wtgValidation(string wtg, int wtgId, long rowNumber)
        {
            bool retVal = false;
            if (stringNullValidation(wtg, "WTG", rowNumber))
            {
                retVal = true;
            }
            else if (!(equipmentId.ContainsKey(wtg)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid WTG <" + wtg + "> is not found in master records");
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Site due to invalid <" + wtg + "> not found in master records");
            }
            return retVal;
        }
        public bool siteValidation(string site, int siteId, long rowNumber)
        {
            bool retVal = false;
            if (stringNullValidation(site, "Site", rowNumber))
            {
                retVal = true;
            }
            else if (!(siteName.ContainsKey(siteId)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Site <" + site + "> is not found in master records");
            }
            return retVal;
        }
        public bool bdTypeValidation(string bdtype, long rowNumber)
        {
            bool retVal = false;
            if (!(breakdownType.ContainsKey(bdtype)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid breakdown type - <" + bdtype + "> is not found in master records");
            }
            return retVal;
        }
        public bool dateNullValidation(string value, string columnName, long rowNo)
        {
            //ErrorLog("Inside dateNullValidation");
            //ErrorLog("Date  Value :" + value + "T\r\n");
            bool retVal = false;
            try
            {
                //ErrorLog("Inside try block");
                // string dateValue = Convert.ToDateTime(value, timeCulture).ToString("dd-MM-yyyy");
                //value = Convert.ToDateTime(value, timeCulture).ToString("dd-MM-yyyy");
                string dateValue = Convert.ToDateTime(value).ToString("dd-MM-yyyy");
                //ErrorLog("Inside try block 1");
                value = Convert.ToDateTime(value).ToString("dd-MM-yyyy");
                //ErrorLog("Inside try block 2");
                //ErrorLog("Date  Value :" + dateValue + "Value :" + value + "T\r\n");
                //ErrorLog("Date  Tome Min :"  +DateTime.MinValue.ToString("dd-MM-yyyy"));
                if (value == "Nil")
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Cell value cannot be empty 1,");
                    retVal = true;
                }
                else if (value != dateValue)
                {
                    //m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format: MM-dd-yyyy,");
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format: yyyy-mm-dd,");
                    retVal = true;
                }
                else if (value == DateTime.MinValue.ToString("dd-MM-yyyy"))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format:  yyyy-mm-dd,");
                    retVal = true;
                }
            }
            catch (Exception e)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Incorrect date conversion <" + value + ">. While feeding data use following format:  yyyy-mm-dd catch " + e.ToString() + ", ");
                retVal = true;
            }
            return retVal;
        }
        /* public bool numericNullValidation(double value, string columnName, long rowNo)
         {
             bool retVal = false;
             if (value == 0)
             {
                 m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be empty or zero,");
                 retVal = true;
             }
             return retVal;
         }*/
        public bool numericNullValidation(double value, string columnName, long rowNo)
        {
            bool retVal = false;
            if(columnName == "LineLoss")
            {
                return retVal;
            }
            else
            {
                if (value == 0)
                {
                    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be null or zero,");
                    retVal = true;
                }
                if (value < 0)
                {
                    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be negative,");
                    retVal = true;
                }
            }
            return retVal;

        }
        public bool stringNullValidation(string value, string columnName, long rowNo)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(value) || value == "Nil")
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: value   <" + value + "> cannot be empty,");
                retVal = true;
            }
            return retVal;
        }
        public bool financialYearValidation(string year, long rowNo)
        {
            bool retVal = false;
            try
            {
                if (year == "Nil" || string.IsNullOrEmpty(year))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Please add financial year in foll format:'2022-23',");
                    retVal = true;
                }
                int yearLimit1 = Convert.ToInt32(year.Substring(0, 4));
                int yearLimit2 = Convert.ToInt32("20" + year.Substring(year.IndexOf("-") + 1));
                if (!((yearLimit1 < yearLimit2) && (yearLimit1 <= 2040 && yearLimit1 >= 2020) && (yearLimit2 <= 2040 && yearLimit2 >= 2020)))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Invalid financial year <" + year + "> range must be between year 2020 and 2040,");
                    retVal = true;
                }
                if (!(yearLimit2 - yearLimit1 == 1))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Invalid financial year <" + year + ">,");
                    retVal = true;

                }
            }
            catch (Exception)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> Please add financial year in foll format:'2022-23',");
                retVal = true;
            }
            return retVal;
        }
        public bool timeValidation(string timeValue, string columnName, long rowNo)
        {
            bool retVal = false;
            try
            {
                //conversion error should throw format error
                // string standardTime = Convert.ToDateTime(timeValue, timeCulture).ToString("HH:mm:ss");
                string standardTime = Convert.ToDateTime(timeValue).ToString("HH:mm:ss");
                //if cell empty then throw empty error
                string msg = "Standatd  Time :" + standardTime + "TimeValue :" + timeValue + "T";
                //InformationLog(msg);
                LogInfo(user_id, 1, 4, "ExcelDataReadANdUpload", msg);
                if (timeValue == "Nil")
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + "> Cell value empty. Add valid time value in HH:mm:ss 24 hour format,");
                    retVal = true;
                }
                else if (timeValue != standardTime)
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + "> Add valid time value in HH:mm:ss 24 hour format,");
                    retVal = true;
                }
            }
            catch (Exception)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + "> Invalid Time Format. Add valid time value in HH:mm:ss format,");
                retVal = true;
            }
            return retVal;
        }
        public bool monthValidation(string month, int monthNo, long rowNo)
        {
            bool retVal = false;
            if (!(monthNo >= 1 && monthNo <= 12))
            {
                retVal = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> Incorrect Month<" + month + "> Add a valid full name of a month,");
            }
            return retVal;
        }
        public bool yearValidation(int year, long rowNo)
        {
            bool retValue = false;
            if (!(year >= 2020 && year <= 2040))
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> Incorrect Year<" + year + "> Add a valid year between 2020 and 2040,");
                retValue = true;
            }
            return retValue;
        }
        public bool solarInverterValidation(string inverterValue, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(inverterValue) || !(inverterList.Contains(inverterValue)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid Inverter value <" + inverterValue + "> not found in master records,");
            }
            return retValue;
        }
        public bool solarIGBDValidation(string IGBD_value, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(IGBD_value) || !(IGBD.Contains(IGBD_value)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid IGBD value <" + IGBD_value + "> not found in master records,");
            }
            return retValue;
        }
        public bool solarICRValidation(string ICR_value, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(ICR_value) || !(icrList.Contains(ICR_value)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid ICR value <" + ICR_value + "> not found in master records,");
            }
            return retValue;
        }
        public bool solarINVValidation(string INV_value, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(INV_value) || !(invList.Contains(INV_value)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid INV value <" + INV_value + "> not found in master records,");
            }
            return retValue;
        }
        public bool solarSMBValidation(string SMB_value, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(SMB_value) || !(SMBList.Contains(SMB_value)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid SMB value <" + SMB_value + "> not found in master records,");
            }
            return retValue;
        }
        public bool solarStringsValidation(string Strings_value, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(Strings_value) || !(StringsList.Contains(Strings_value)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid String value <" + Strings_value + "> not found in master records,");
            }
            return retValue;
        }
        public bool countryValidation(string countryValue, string columnName, long rowNo)
        {
            bool retVal = false;
            if (!(countryValue == "India") || countryValue == "Nil")
            {
                m_ErrorLog.SetError(",File Row<" + rowNo + "> column <" + columnName + ">: Invalid Country name<" + countryValue + ">,");
                retVal = true;
            }
            return retVal;
        }
        bool validateNumeric(string val, string columnName, long rowNo, bool dbNullError, int logErrorFlag, out double importValue)
        {
            bool retValue = false;
            importValue = 0;

            //            if (val is DBNull)
            if (dbNullError)
            {
                //dont log error but set value to 0
                //retValue = true;
                //if (logErrorFlag == 0)
                //{
                //    m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be DBNull,");
                //}
                //else if (logErrorFlag == 1)
                //{
                //    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be DBNull,");
                //}
            }
            if (string.IsNullOrEmpty(val))
            {
                //dont log error but set value to 0
                //retValue = true;
                //if (logErrorFlag == 0)
                //{
                //    m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be null or empty,");
                //}
                //else if (logErrorFlag == 1)
                //{
                //    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be null or empty,");

                //}
            }
            else
            {
                if (!Regex.IsMatch(val, @"^-?([0-9]*|\d*\.\d{1}?\d*)$"))
                {
                    //log error for non numeric values
                    retValue = true;
                    if (logErrorFlag == 0)
                    {
                        m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value  <" + val + "> cannot be non numeric,");
                    }
                    else if (logErrorFlag == 1)
                    {
                        m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value  <" + val + "> cannot be non numeric,");
                    }
                }
                else
                {
                    //all good, convert value to double
                    importValue = Convert.ToDouble(val);
                }
            }
            return retValue;
        }
        bool validateNumeric(string val, out double importValue)
        {
            bool retValue = false;
            importValue = 0;

            if (val is DBNull)
            {
                retValue = true;
            }
            if (string.IsNullOrEmpty(val))
            {
                retValue = true;
            }
            else
            {
                if (!Regex.IsMatch(val, @"^([0-9]*|\d*\.\d{1}?\d*)$"))
                {
                    retValue = true;
                }
                else
                {
                    importValue = Convert.ToDouble(val);
                }
            }
            return retValue;
        }
        public bool kwhValidation(double kwhValue, double prodHrsValue, string columnName, long rowNo, double kwhMax)
        {
            bool retVal = false;
            if (kwhValue == 0 && prodHrsValue != 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be zero, because production hours  <" + prodHrsValue + "> is not zero,");
                retVal = true;
            }
            string value = Convert.ToString(kwhValue);
            if (kwhValue < 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be negative or null or 0,");
                retVal = true;
            }

            //get max kWh from location master and validate.
            if (kwhValue > kwhMax)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be more than <" + kwhMax + "> as set in location master,");
                retVal = true;
            }

            return retVal;
        }
        public bool negativeNullValidation(double value, string columnName, long rowNo)
        {
            bool retVal = false;
            string checker = Convert.ToString(value);
            if (value < 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be null or negative,");
                retVal = true;
            }
            return retVal;

        }
        public string Filter(long rowNumber, string colName, string str, List<char> charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                str = str.Replace(c.ToString(), string.Empty);
            }
            //m_ErrorLog.SetInformation(",Row <" + rowNumber + "> column <" + colName + "> : special character removed from string,");

            return str;
        }
        public string validateAndCleanSpChar(long rowNumber, string colName, string sActionTaken)
        {
            string retValue = "";
            //List<char> charsToRemove = new List<char>() { '\'', '/', '\\', ')', '(' };
            List<char> charsToRemove = new List<char>() { '\'', '/', '\\'};
            retValue = Filter(rowNumber, colName, sActionTaken, charsToRemove);
            return retValue;
        }

        //DGRA_v2 Functions.

        //InsertSolarTrackerLoss
        private async Task<int> InsertSolarTrackerLoss(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string site = "";
            bool noData = false;
            int tableRowCount = ds.Tables[0].Rows.Count;
            List<InsertSolarTrackerLoss> addSet = new List<InsertSolarTrackerLoss>();

            if (ds.Tables[0].Rows.Count >= 1)
            {
                if (ds.Tables[0].Rows[0]["Site"] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[0]["Site"]) || ds.Tables[0].Rows[0]["Date"] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[0]["Date"]))
                {
                    noData = true;
                }

                if (!noData)
                {
                    try
                    {
                        fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                        toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);

                    }
                    catch (Exception e)
                    {
                        //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                        m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy");
                        fromDate = DateTime.MaxValue;
                        toDate = DateTime.MinValue;
                    }

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        InsertSolarTrackerLoss addUnit = new InsertSolarTrackerLoss();
                        try
                        {
                            bool skipRow = false;
                            rowNumber++;
                            if (addUnit.date == "" && addUnit.from_time == "" && addUnit.to_time == "")
                            {
                                errorCount++;
                                skipRow = true;
                                continue;
                            }
                            addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                            if (addUnit.site != "Nil")
                            {
                                addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(",Site value is Nill at row <" + rowNumber + "> row will be skipped.");
                                continue;
                            }
                            //errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                            objImportBatch.importSiteId = addUnit.site_id;
                            bool siteIssue = siteValidation(addUnit.site, addUnit.site_id, rowNumber);
                            errorFlag.Add(siteIssue);
                            if (siteIssue)
                            {
                                errorCount++;
                                continue;
                            }

                            addUnit.ac_capacity = Convert.ToDouble(dr["Plant AC Capacity (MW)"]);
                            errorFlag.Add(numericNullValidation(addUnit.ac_capacity, "Plant AC Capacity (MW)", rowNumber));

                            bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                            if (isdateEmpty)
                            {
                                m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                                continue;
                            }
                            addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);
                            bool dateIssue = dateNullValidation(addUnit.date, "Date", rowNumber);
                            errorFlag.Add(dateIssue);
                            if (!dateIssue)
                            {
                                addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                            }

                            if (generationDate != addUnit.date)
                            {
                                m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and TrackerLoss Date <" + addUnit.date + "> missmatched");
                                errorCount++;
                                skipRow = true;
                                continue;
                            }

                            //if (rowNumber == 2)
                            //{
                            //    generationDate = addUnit.date;
                            //}
                            //if (rowNumber > 2)
                            //{
                            //    if (generationDate != addUnit.date)
                            //    {
                            //        m_ErrorLog.SetError(",Row <" + rowNumber + "> <Date> : <" + addUnit.date + "> mismatched with  <" + generationDate + "> ");
                            //        errorCount++;
                            //        skipRow = true;
                            //        continue;
                            //    }
                            //}

                            // addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("HH:mm:ss");
                            addUnit.from_time = Convert.ToDateTime(dr["From Time"]).ToString("HH:mm:ss");

                            addUnit.to_time = Convert.ToDateTime(dr["To Time"]).ToString("HH:mm:ss");

                            addUnit.trackers_in_BD = Convert.ToInt32(dr["No. of Trackers in Breakdown (Nos)"]);
                            errorFlag.Add(numericNullValidation(addUnit.trackers_in_BD, "No. of Trackers in Breakdown (Nos)", rowNumber));

                            addUnit.module_tracker = Convert.ToInt32(dr["No. of Module Tracker (Nos)"]);
                            errorFlag.Add(numericNullValidation(addUnit.trackers_in_BD, "No. of Module Tracker (Nos)", rowNumber));

                            addUnit.module_WP = Convert.ToInt32(dr["Module WP (Watt)"]);
                            errorFlag.Add(numericNullValidation(addUnit.module_WP, "Module WP (Watt)", rowNumber));

                            //addUnit.reason = Convert.ToString(dr["Remark"]);
                            string remarks = Convert.ToString(dr["Remark"]);
                            remarks = validateAndCleanSpChar(rowNumber, "Remark", remarks);
                            addUnit.reason = remarks;
                            errorFlag.Clear();
                            if (!(skipRow))
                            {
                                addSet.Add(addUnit);
                            }
                        }
                        catch (Exception e)
                        {
                            //developer errorlog
                            m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarTrackerLoss,");
                            string msg = ",Exception Occurred In Function: InsertSolarTrackerLoss: " + e.ToString() + ",";
                            //ErrorLog(msg);
                            LogError(user_id, 1, 4, "InsertSolarTrackerLoss", msg);
                            errorCount++;
                        }
                    }
                    if (!(errorCount > 0))
                    {
                        m_ErrorLog.SetInformation(",Solar Tracker Loss Validation SuccessFul,");
                        isTrackerLossvalidationSuccess = true;
                        responseCode = 200;
                        trackerJson = JsonConvert.SerializeObject(addSet);

                        //var json = JsonConvert.SerializeObject(addSet);
                        //var data = new StringContent(json, Encoding.UTF8, "application/json");

                    }
                    else
                    {
                        m_ErrorLog.SetError(",Solar Tracker Loss Validation Failed,");
                    }
                }
                else
                {
                    m_ErrorLog.SetInformation("No Data in Tracker Loss Sheet");
                    if (!(errorCount > 0))
                    {
                        isTrackerLossvalidationSuccess = true;
                        responseCode = 200;
                        trackerJson = JsonConvert.SerializeObject(addSet);
                    }
                }
            }
            else
            {
                m_ErrorLog.SetInformation("No Data in Tracker Loss Sheet");
                if (!(errorCount > 0))
                {
                    isTrackerLossvalidationSuccess = true;
                    responseCode = 200;
                    trackerJson = JsonConvert.SerializeObject(addSet);
                }
            }
            return responseCode;
        }

        //InsertSolarTrackerLossMonthly
        private async Task<int> InsertSolarTrackerLossMonthly(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string site = "";
            try
            {
                fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);

            }
            catch (Exception e)
            {
                //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy");
                fromDate = DateTime.MaxValue;
                toDate = DateTime.MinValue;
            }

            if (ds.Tables.Count > 0)
            {
                List<InsertSolarTrackerLoss> addSet = new List<InsertSolarTrackerLoss>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertSolarTrackerLoss addUnit = new InsertSolarTrackerLoss();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        if (addUnit.date == "" && addUnit.from_time == "" && addUnit.to_time == "")
                        {
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.ac_capacity = Convert.ToDouble(dr["Plant AC Capacity (MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.ac_capacity, "Plant AC Capacity (MW)", rowNumber));

                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        // addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("HH:mm:ss");
                        addUnit.from_time = Convert.ToDateTime(dr["From Time"]).ToString("HH:mm:ss");

                        addUnit.to_time = Convert.ToDateTime(dr["To Time"]).ToString("HH:mm:ss");

                        addUnit.trackers_in_BD = Convert.ToInt32(dr["No. of Trackers in Breakdown (Nos)"]);
                        errorFlag.Add(numericNullValidation(addUnit.trackers_in_BD, "No. of Trackers in Breakdown (Nos)", rowNumber));

                        addUnit.module_tracker = Convert.ToInt32(dr["No. of Module Tracker (Nos)"]);
                        errorFlag.Add(numericNullValidation(addUnit.trackers_in_BD, "No. of Module Tracker (Nos)", rowNumber));

                        addUnit.module_WP = Convert.ToInt32(dr["Module WP (Watt)"]);
                        errorFlag.Add(numericNullValidation(addUnit.module_WP, "Module WP (Watt)", rowNumber));

                        addUnit.reason = Convert.ToString(dr["Remark"]);
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        bool containsSingleQuote = addUnit.reason.Contains("'");
                        if (containsSingleQuote)
                        {
                            addUnit.reason = addUnit.reason.Replace("'", " ");
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarTrackerLossMonthly,");
                        string msg = ",Exception Occurred In Function: InsertSolarTrackerLossMonthly: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarTrackerLossMonthly", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Tracker Loss Monthly Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarTrackerLossMonthly";
                    using (var client = new HttpClient())
                    {
                        //InformationLog("added timeout to InfiniteTimeSpan");
                        client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                        var response = await client.PostAsync(url, data);
                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                        if (returnResponse.Contains("True") || returnResponse.Contains("False"))
                        {
                            string[] substrings = returnResponse.Split(',');
                            bool[] values = new bool[substrings.Length];
                            int totalCalculations = substrings.Length;
                            int successfulCalculations = 0;
                            int failedCalculations = 0;
                            for (int i = 0; i < substrings.Length; i++)
                            {
                                if (substrings[i] == "False")
                                {
                                    int count = i + 1;
                                    m_ErrorLog.SetError(", Calculation of Tracker loss failed for row number <" + count + "> ");
                                    failedCalculations++;
                                }
                                else
                                {
                                    successfulCalculations++;
                                }
                            }
                            m_ErrorLog.SetInformation(", " + successfulCalculations + " number of calculations completed out of " + totalCalculations + " number of calculations.");
                            if (failedCalculations != 0)
                            {
                                m_ErrorLog.SetError(", " + failedCalculations + " number of calculations failed out of " + totalCalculations + " number of calculations.");
                            }
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Tracker Loss Monthly API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Tracker Loss Monthly API Failure,: responseCode <" + (int)response.StatusCode + "> Reason : " + returnResponse);

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Tracker Loss Monthly Validation Failed,");
                }
            }
            return responseCode;
        }
        //InsertSolarSoilingLoss
        private async Task<int> InsertSolarSoilingLoss(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertSolarSoilingLoss> addSet = new List<InsertSolarSoilingLoss>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertSolarSoilingLoss addUnit = new InsertSolarSoilingLoss();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nill" : Convert.ToString(dr["Month"]);
                        if (addUnit.month == "" || addUnit.month == null)
                        {
                            m_ErrorLog.SetError(", Month column of" + rowNumber + " row is empty");
                            continue;
                        }
                        else
                        {
                            addUnit.month_no = MonthList.ContainsKey(addUnit.month) ? Convert.ToInt32(MonthList[addUnit.month]) : 0;
                            errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));
                        }

                        addUnit.site_name = dr["Site Name"] is DBNull || string.IsNullOrEmpty((string)dr["Site Name"]) ? "Nil" : Convert.ToString(dr["Site Name"]);
                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site_name]);
                        errorFlag.Add(siteValidation(addUnit.site_name, addUnit.site_id, rowNumber));
                        //objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.five_days = dr["5 days"] is DBNull || string.IsNullOrEmpty((string)dr["5 days"]) ? "0" : Convert.ToString(dr["5 days"]);

                        if (addUnit.five_days == "" || addUnit.five_days == null)
                        {
                            m_ErrorLog.SetError(", 5 Days column of " + rowNumber + " row is empty.");
                            continue;
                        }

                        addUnit.ten_days = dr["10 days"] is DBNull || string.IsNullOrEmpty((string)dr["10 days"]) ? "0" : Convert.ToString(dr["10 days"]);
                        if (addUnit.ten_days == "" || addUnit.ten_days == null)
                        {
                            m_ErrorLog.SetError(", 10 Days column of " + rowNumber + " row is empty.");
                            continue;
                        }

                        addUnit.fifteen_days = dr["15 days"] is DBNull || string.IsNullOrEmpty((string)dr["15 days"]) ? "0" : Convert.ToString(dr["15 days"]);
                        if (addUnit.fifteen_days == "" || addUnit.fifteen_days == null)
                        {
                            m_ErrorLog.SetError(", 15 Days column of " + rowNumber + " row is empty.");
                            continue;
                        }

                        addUnit.fifteen_days_original = dr["15 days original"] is DBNull || string.IsNullOrEmpty((string)dr["15 days original"]) ? "0" : Convert.ToString(dr["15 days original"]);
                        if (addUnit.fifteen_days_original == "" || addUnit.fifteen_days_original == null)
                        {
                            m_ErrorLog.SetError(", 15 days original column of " + rowNumber + " row is empty.");
                            continue;
                        }

                        addUnit.rainy_days = dr["No of rainy days"] is DBNull ? 0 : Convert.ToInt32(dr["No of rainy days"]);

                        addUnit.sandstorm_days = dr["No of sandstorm days"] is DBNull ? 0 : Convert.ToInt32(dr["No of sandstorm days"]);

                        addUnit.total_rain = dr["Total rain in mm"] is DBNull ? 0 : Convert.ToDouble(dr["Total rain in mm"]);

                        addUnit.manual_or_SCADA = dr["Manual / SCADA"] is DBNull || string.IsNullOrEmpty((string)dr["Manual / SCADA"]) ? "Nill" : Convert.ToString(dr["Manual / SCADA"]);
                        if (addUnit.manual_or_SCADA == "" || addUnit.manual_or_SCADA == null)
                        {
                            m_ErrorLog.SetError(", Manual / SCADA column of " + rowNumber + " row is empty.");
                            errorCount++;
                            continue;
                        }
                        else
                        {
                            string check = addUnit.manual_or_SCADA.ToLower();
                            if (check == "manual")
                            {
                                addUnit.isManual_or_SCADA = 1;
                            }
                            else if (check == "scada")
                            {
                                addUnit.isManual_or_SCADA = 2;
                            }
                            else
                            {
                                addUnit.isManual_or_SCADA = 0;
                            }
                        }

                        addUnit.toplining_losses = dr["Toplining Soiling Losses"] is DBNull ? 0 : Convert.ToDouble(dr["Total Rain in mm"]);

                        addUnit.reason = Convert.ToString(dr["Reason"]);
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarSoilingLoss,");
                        string msg = ",Exception Occurred In Function: InsertSolarSoilingLoss: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarSoilingLoss", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Soiling Loss Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarSoilingLoss";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Soiling Loss API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Soiling Loss API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Soiling Loss Validation Failed,");
                }
            }
            return responseCode;
        }

        //InsertSolarPVSystLoss
        private async Task<int> InsertSolarPVSystLoss(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertSolarPVSystLoss> addSet = new List<InsertSolarPVSystLoss>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertSolarPVSystLoss addUnit = new InsertSolarPVSystLoss();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : (string)(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.FY.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? year : year += 1;
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        addUnit.month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nill" : Convert.ToString(dr["Month"]);
                        if (addUnit.month == "" || addUnit.month == null)
                        {
                            m_ErrorLog.SetError(", Month column of" + rowNumber + " row is empty");
                            continue;
                        }
                        else
                        {
                            addUnit.month_no = MonthList.ContainsKey(addUnit.month) ? Convert.ToInt32(MonthList[addUnit.month]) : 0;
                            errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));
                        }

                        addUnit.site_name = dr["Site Name"] is DBNull || string.IsNullOrEmpty((string)dr["Site Name"]) ? "Nil" : Convert.ToString(dr["Site Name"]);
                        if (addUnit.site_name == "" || addUnit.site_name == null)
                        {
                            m_ErrorLog.SetError(", Site Name column of <" + rowNumber + "> row is empty");
                            continue;
                        }

                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site_name]);

                        addUnit.alpha = dr["α"] is DBNull || string.IsNullOrEmpty((string)dr["α"]) ? 0 : Convert.ToDouble(dr["α"]);
                        //errorFlag.Add(numericNullValidation(addUnit.alpha, "α", rowNumber));

                        addUnit.near_shading = dr["Near Shading"] is DBNull || string.IsNullOrEmpty((string)dr["Near Shading"]) ? 0 : Convert.ToDouble(dr["Near Shading"]);
                        //errorFlag.Add(numericNullValidation(addUnit.near_shading, "10 Days", rowNumber));

                        addUnit.IAM_factor = dr["IAM factor"] is DBNull || string.IsNullOrEmpty((string)dr["IAM factor"]) ? 0 : Convert.ToDouble(dr["IAM factor"]);
                        //errorFlag.Add(numericNullValidation(addUnit.IAM_factor, "IAM factor", rowNumber));

                        addUnit.soiling_factor = dr["Soiling factor"] is DBNull || string.IsNullOrEmpty((string)dr["Soiling factor"]) ? 0 : Convert.ToDouble(dr["Soiling factor"]);
                        //errorFlag.Add(numericNullValidation(addUnit.soiling_factor, "Soiling factor", rowNumber));

                        addUnit.pv_loss = dr["PV loss due to Irradiance level"] is DBNull || string.IsNullOrEmpty((string)dr["PV loss due to Irradiance level"]) ? 0 : Convert.ToDouble(dr["PV loss due to Irradiance level"]);
                        //errorFlag.Add(numericNullValidation(addUnit.pv_loss, "PV loss due to Irradiance level", rowNumber));

                        addUnit.lid = dr["LID"] is DBNull || string.IsNullOrEmpty((string)dr["LID"]) ? 0 : Convert.ToDouble(dr["LID"]);
                        //errorFlag.Add(numericNullValidation(addUnit.lid, "LID", rowNumber));

                        addUnit.array_missmatch = dr["Array mismatch"] is DBNull || string.IsNullOrEmpty((string)dr["Array mismatch"]) ? 0 : Convert.ToDouble(dr["Array mismatch"]);
                        //errorFlag.Add(numericNullValidation(addUnit.array_missmatch, "Array mismatch", rowNumber));

                        addUnit.dc_ohmic = dr["DC Ohmic"] is DBNull || string.IsNullOrEmpty((string)dr["DC Ohmic"]) ? 0 : Convert.ToDouble(dr["DC Ohmic"]);
                        //errorFlag.Add(numericNullValidation(addUnit.dc_ohmic, "DC Ohmic", rowNumber));

                        addUnit.conversion_loss = dr["Conversion loss"] is DBNull || string.IsNullOrEmpty((string)dr["Conversion loss"]) ? 0 : Convert.ToDouble(dr["Conversion loss"]);
                        //errorFlag.Add(numericNullValidation(addUnit.conversion_loss, "Conversion loss", rowNumber));

                        //addUnit.plant_aux = Convert.ToInt32(dr["Plant Auxiliary"]);
                        addUnit.plant_aux = dr["Plant Auxiliary"] is DBNull || string.IsNullOrEmpty((string)dr["Plant Auxiliary"]) ? 0 : Convert.ToDouble(dr["Plant Auxiliary"]);
                        //errorFlag.Add(numericNullValidation(addUnit.plant_aux, "Plant Auxiliary", rowNumber));

                        addUnit.system_unavailability = dr["System unavailability"] is DBNull || string.IsNullOrEmpty((string)dr["System unavailability"]) ? 0 : Convert.ToDouble(dr["System unavailability"]);
                        //errorFlag.Add(numericNullValidation(addUnit.system_unavailability, "System unavailability", rowNumber));

                        addUnit.ac_ohmic = dr["AC Ohmic"] is DBNull || string.IsNullOrEmpty((string)dr["AC Ohmic"]) ? 0 : Convert.ToDouble(dr["AC Ohmic"]);
                        //errorFlag.Add(numericNullValidation(addUnit.ac_ohmic, "AC Ohmic", rowNumber));

                        addUnit.external_transformer = dr["External Transformer"] is DBNull || string.IsNullOrEmpty((string)dr["External Transformer"]) ? 0 : Convert.ToDouble(dr["External Transformer"]);
                        //errorFlag.Add(numericNullValidation(addUnit.external_transformer, "External Transformer", rowNumber));

                        addUnit.yoy_degradation = dr["YoY degradation"] is DBNull || string.IsNullOrEmpty((string)dr["YoY degradation"]) ? 0 : Convert.ToDouble(dr["YoY degradation"]);
                        //errorFlag.Add(numericNullValidation(addUnit.yoy_degradation, "YoY degradation", rowNumber));

                        addUnit.module_degradation = dr["Module Degradation"] is DBNull || string.IsNullOrEmpty((string)dr["Module Degradation"]) ? "Nill" : Convert.ToString(dr["Module Degradation"]);
                        if (addUnit.module_degradation == "" || addUnit.module_degradation == null)
                        {
                            m_ErrorLog.SetError(", Module Degradation column of <" + rowNumber + "> row is empty.");
                            continue;
                        }

                        addUnit.tstc = Convert.ToInt32(dr["Tstc"]);
                        errorFlag.Add(numericNullValidation(addUnit.tstc, "Tstc", rowNumber));

                        addUnit.tcnd = Convert.ToInt32(dr["Tcnd"]);
                        errorFlag.Add(numericNullValidation(addUnit.tcnd, "Tcnd", rowNumber));

                        addUnit.far_shading = dr["Far Shading"] is DBNull || string.IsNullOrEmpty((string)dr["Far Shading"]) ? 0 : Convert.ToDouble(dr["Far Shading"]);

                        addUnit.pv_loss_dueto_temp = dr["PV Loss due to Temperature"] is DBNull || string.IsNullOrEmpty((string)dr["PV Loss due to Temperature"]) ? 0 : Convert.ToDouble(dr["PV Loss due to Temperature"]);

                        addUnit.module_quality_loss = dr["Module Quality Loss"] is DBNull || string.IsNullOrEmpty((string)dr["Module Quality Loss"]) ? 0 : Convert.ToDouble(dr["Module Quality Loss"]);

                        addUnit.electrical_loss = dr["Electrical Loss acc. to strings"] is DBNull || string.IsNullOrEmpty((string)dr["Electrical Loss acc. to strings"]) ? 0 : Convert.ToDouble(dr["Electrical Loss acc. to strings"]);

                        addUnit.inv_loss_over_power = dr["Inverter Loss over nominal inv. power"] is DBNull || string.IsNullOrEmpty((string)dr["Inverter Loss over nominal inv. power"]) ? 0 : Convert.ToDouble(dr["Inverter Loss over nominal inv. power"]);

                        addUnit.inv_loss_max_input_current = dr["Inverter Loss due to max. input current"] is DBNull || string.IsNullOrEmpty((string)dr["Inverter Loss due to max. input current"]) ? 0 : Convert.ToDouble(dr["Inverter Loss due to max. input current"]);

                        addUnit.inv_loss_voltage = dr["Inverter Loss over nominal inv. Voltage"] is DBNull || string.IsNullOrEmpty((string)dr["Inverter Loss over nominal inv. Voltage"]) ? 0 : Convert.ToDouble(dr["Inverter Loss over nominal inv. Voltage"]);

                        addUnit.inv_loss_power_threshold = dr["Inverter Loss due to power threshold"] is DBNull || string.IsNullOrEmpty((string)dr["Inverter Loss due to power threshold"]) ? 0 : Convert.ToDouble(dr["Inverter Loss due to power threshold"]);

                        addUnit.inv_loss_voltage_threshold = dr["Inverter Loss due to voltage threshold"] is DBNull || string.IsNullOrEmpty((string)dr["Inverter Loss due to voltage threshold"]) ? 0 : Convert.ToDouble(dr["Inverter Loss due to voltage threshold"]);

                        addUnit.night_consumption = dr["Night consumption"] is DBNull || string.IsNullOrEmpty((string)dr["Night consumption"]) ? 0 : Convert.ToDouble(dr["Night consumption"]);

                        addUnit.idt = dr["IDT"] is DBNull || string.IsNullOrEmpty((string)dr["IDT"]) ? 0 : Convert.ToDouble(dr["IDT"]);

                        addUnit.line_losses = dr["Line Losses"] is DBNull || string.IsNullOrEmpty((string)dr["Line Losses"]) ? 0 : Convert.ToDouble(dr["Line Losses"]);

                        addUnit.unused_energy = dr["Unused energy"] is DBNull || string.IsNullOrEmpty((string)dr["Unused energy"]) ? 0 : Convert.ToDouble(dr["Unused energy"]);


                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.Message + ": Function: InsertSolarPVSystLoss,");
                        string msg = ",Exception Occurred In Function: InsertSolarPVSystLoss: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarPVSystLoss", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar PVSyst Loss Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarPVSystLoss";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar PVSyst Loss API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar PVSyst Loss API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar PVSyst Loss Validation Failed,");
                }
            }
            return responseCode;
        }

        //InsertSolarEstimatedHourlyData
        private async Task<int> InsertSolarEstimatedHourlyData(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertSolarEstimatedHourlyData> addSet = new List<InsertSolarEstimatedHourlyData>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertSolarEstimatedHourlyData addUnit = new InsertSolarEstimatedHourlyData();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;

                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);

                        bool isFy = dr["FY Date"] is DBNull || string.IsNullOrEmpty((string)dr["FY Date"]) ? false : true;
                        if (!isFy)
                        {
                            m_ErrorLog.SetError(", FY Date column of <" + rowNumber + "> row is empty.");
                            errorCount++;
                        }
                        addUnit.fy_date = isFy ? (string)(dr["FY Date"]) : "";
                        if (addUnit.fy_date != "")
                        {
                            string finalDate = Convert.ToDateTime(addUnit.fy_date).ToString("yyyy-MM-dd");
                            addUnit.fy_date = finalDate;
                        }

                        addUnit.month = isFy ? Convert.ToDateTime(addUnit.fy_date).ToString("MMM") : "0";

                        addUnit.month_no = isFy ? int.Parse(Convert.ToDateTime(addUnit.fy_date).ToString("MM")) : 0;

                        int year = isFy ? int.Parse(Convert.ToDateTime(addUnit.fy_date).ToString("yyyy")) : 0;

                        addUnit.year = (addUnit.month_no > 3) ? year : year += 1;
                        if (!(dr["Time"] is DBNull || string.IsNullOrEmpty((string)dr["Time"])))
                        {
                            addUnit.time = Convert.ToDateTime(dr["Time"]).ToString("HH:mm:ss");
                        }

                        addUnit.glob_hor = dr["GlobHor"] is DBNull || string.IsNullOrEmpty((string)dr["GlobHor"]) ? 0 : Convert.ToDouble(dr["GlobHor"]);

                        addUnit.glob_inc = dr["GlobInc"] is DBNull || string.IsNullOrEmpty((string)dr["GlobInc"]) ? 0 : Convert.ToDouble(dr["GlobInc"]);

                        addUnit.t_amb = dr["T_Amb"] is DBNull || string.IsNullOrEmpty((string)dr["T_Amb"]) ? 0 : Convert.ToDouble(dr["T_Amb"]);

                        addUnit.t_array = dr["Tarray"] is DBNull || string.IsNullOrEmpty((string)dr["Tarray"]) ? 0 : Convert.ToDouble(dr["Tarray"]);

                        addUnit.e_out_inv = dr["EOutInv"] is DBNull || string.IsNullOrEmpty((string)dr["EOutInv"]) ? 0 : Convert.ToDouble(dr["EOutInv"]);

                        addUnit.e_grid = dr["E_Grid"] is DBNull || string.IsNullOrEmpty((string)dr["E_Grid"]) ? 0 : Convert.ToDouble(dr["E_Grid"]);

                        addUnit.phi_ang = dr["PhiAng"] is DBNull || string.IsNullOrEmpty((string)dr["PhiAng"]) ? 0 : Convert.ToDouble(dr["PhiAng"]);

                        if (addUnit.glob_hor == 0 && addUnit.glob_inc == 0 && addUnit.t_amb == 0 && addUnit.t_array == 0 && addUnit.e_out_inv == 0 && addUnit.e_grid == 0 && addUnit.phi_ang == 0)
                        {
                            m_ErrorLog.SetInformation(", All data at <" + rowNumber + "> row is 0.");
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarPVSystLoss,");
                        string msg = ",Exception Occurred In Function: InsertSolarPVSystLoss: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 1, 4, "InsertSolarPVSystLoss", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Estimated Hourly Data Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarEstimatedHourlyData";
                    using (var client = new HttpClient())
                    {
                        client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                        var response = await client.PostAsync(url, data);
                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            if (returnResponse == "1")
                            {
                                m_ErrorLog.SetError(",Error Deleting records form database.");
                            }
                            else if (returnResponse == "2")
                            {
                                m_ErrorLog.SetInformation(",Solar Estimated Hourly Data imported SuccessFully,");
                            }
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Estimated Hourly Data API Failure,: responseCode <" + (int)response.StatusCode + "> Reason : " + returnResponse);

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Estimated Hourly Data Validation Failed,");
                }
            }
            return responseCode;
        }

        //InsertWindTMR
        private async Task<int> InsertWindTMR(string status, DataSet ds, string tabName, bool isMultiFiles, int isMultiSheet)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            string fileDateFormat = "";
            TMLType = 1;
            bool isDateCorrect= false;
            if (ds.Tables.Count > 0)
            {
                List<InsertWindTMLData> addSet = new List<InsertWindTMLData>();
                string previousTime = "00:00:00";
                string dataDate = "";
                m_ErrorLog.SetInformation("Reviewing sheet : " + tabName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindTMLData addUnit = new InsertWindTMLData();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        if(rowNumber > 2)
                        {
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                        }
                        addUnit.onm_wtg = tabName;
                        addUnit.WTGs = onm2equipmentName.ContainsKey(tabName) ? onm2equipmentName[tabName].ToString() : "";
                        if (addUnit.WTGs == "")
                        {
                            //addUnit.WTGs = equipmentId.ContainsKey(tabName) ? tabName : "";
                            m_ErrorLog.SetError(", ONM WTG not found as per master record.");
                            errorCount++;
                        }
                        //error handling
                        //addUnit.wtg_id = equipmentId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(equipmentId[addUnit.wtg]) : 0;
                        if (addUnit.WTGs != "")
                        {
                            addUnit.wtg_id = equipmentId.ContainsKey(addUnit.WTGs) ? Convert.ToInt32(equipmentId[addUnit.WTGs]) : 0;
                            //addUnit.wtg = onm2equipmentName.ContainsKey(tabName) ? onm2equipmentName[tabName].ToString() : "";
                            addUnit.site = SiteByWtg.ContainsKey(addUnit.WTGs) ? SiteByWtg[addUnit.WTGs].ToString() : "";
                            if (addUnit.site != " ")
                            {
                                addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                            }
                            else
                            {
                                addUnit.site_id = 0;
                            }
                        }
                        else
                        {
                            m_ErrorLog.SetError(", ONM WTG not found & Site id is not found.");
                        }
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        string convertedDate = "";
                        //if (isdateEmpty)
                        //{
                        //    m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                        //    continue;
                        //}
                        //else
                        //{
                        //    string tempDate = Convert.ToString(dr["Date"]);
                        //    string[] sepDate = tempDate.Split(' ');
                        //    if (sepDate.Length > 0)
                        //    {
                        //        string[] fDate = sepDate[0].Split('/');
                        //        string d = fDate[0];
                        //        string m = fDate[1];
                        //        string y = fDate[2];

                        //        string ymd = y + "-" + m + "-" + d;
                        //        convertedDate = ymd + " " + sepDate[1];
                        //    }
                        //}
                        string tempDate = Convert.ToString(dr["Date"]);
                        if (rowNumber == 2)
                        {
                            fileDateFormat = getDateFormat(tempDate);
                        }
                        try
                        {
                            if (fileDateFormat != "Invalid Slash separated date" || fileDateFormat != "Invalid Slash separated date")
                            {
                                switch (fileDateFormat)
                                {
                                    case "dd/MM/yyyy HH:mm:ss" :
                                        convertedDate = DateTime.ParseExact(tempDate, "dd/MM/yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    case "yyyy/MM/dd HH:mm:ss":
                                        convertedDate = DateTime.ParseExact(tempDate, "yyyy/MM/dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    case "Contains Hyphen":
                                        convertedDate = Convert.ToDateTime(tempDate).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    default:
                                        m_ErrorLog.SetError(", Cannot recognize <" + tempDate + "> in Date column of Row <" + rowNumber + "> as date.");
                                        break;
                                }
                            }
                            //else if (fileDateFormat == "Contains Hyphen")
                            //{

                            //}

                            //convertedDate = DateTime.ParseExact(tempDate, "dd/MM/yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");


                            //convertedDate = DateTime.ParseExact(tempDate, "yyyy/MM/dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                            //convertedDate = DateTime.ParseExact(tempDate, "dd-MM-yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                            //convertedDate = DateTime.ParseExact(tempDate, "yyyy-MM-dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");

                            //convertedDate = Convert.ToDateTime(tempDate).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        catch (Exception e)
                        {
                            string msg = e.ToString();
                            m_ErrorLog.SetError(", File row<" + rowNumber + "> '" + tempDate + "' is not recognised as valid date time, it should be 'dd/MM/yyyy HH:mm:ss'");
                            errorCount++;
                            continue;
                        }
                        //convertedDate = Convert.ToDateTime(tempDate).ToString("yyyy-MM-dd HH:mm:ss");
                        addUnit.timestamp = isdateEmpty ? "Nil" : convertedDate;
                        //errorFlag.Add(stringNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        //errorFlag.Add(dateNullValidation(addUnit.timestamp, "Date", rowNumber));

                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToDateTime(convertedDate).ToString("dd-MMM-yy");
                        //string temp_date = temp.Substring(0, 10);
                        if (rowNumber == 2)
                        {
                            isDateCorrect = importTMLDateValidationNew(TMLType, addUnit.site_id, Convert.ToDateTime(convertedDate));
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                            previousTime = Convert.ToDateTime(convertedDate).ToString("HH:mm:ss");
                            dataDate = addUnit.date;
                        }
                        if (dataDate != addUnit.date)
                        {
                            previousTime = "00:00:00";
                            //m_ErrorLog.SetError(", Row <" + rowNumber + "> column <Time Stamp> : <" + dataDate + "> and Date <" + addUnit.date + "> missmatched");
                            //errorCount++;
                        }
                        addUnit.from_time = previousTime;
                        addUnit.to_time = Convert.ToDateTime(convertedDate).ToString("HH:mm:ss");

                        previousTime = Convert.ToDateTime(convertedDate).ToString("HH:mm:ss");

                        bool isActivePowerEmpty = false;

                        if (dr.Table.Columns.Contains("Average Active Power 10M (kW)"))
                        {
                            isActivePowerEmpty = dr["Average Active Power 10M (kW)"] is DBNull;
                        } else if (dr.Table.Columns.Contains("Average Active Power 10M \n(kW)"))
                        {
                            isActivePowerEmpty = dr["Average Active Power 10M \n(kW)"] is DBNull;
                        }

                        if (!isActivePowerEmpty)
                        {
                            if (rowNumber > 39)
                            {
                                int te = 1;
                            }
                            if (dr.Table.Columns.Contains("Average Active Power 10M (kW)"))
                            {
                                addUnit.avg_active_power = string.IsNullOrEmpty((string)dr["Average Active Power 10M (kW)"]) ? 0 : Convert.ToDouble(dr["Average Active Power 10M (kW)"]);
                                addUnit.status_code = 0;
                            } else if (dr.Table.Columns.Contains("Average Active Power 10M \n(kW)"))
                            {
                                addUnit.avg_active_power = string.IsNullOrEmpty((string)dr["Average Active Power 10M \n(kW)"]) ? 0 : Convert.ToDouble(dr["Average Active Power 10M \n(kW)"]);
                                addUnit.status_code = 0;
                            }
                        }

                        if (isActivePowerEmpty)
                        {
                            if (dr.Table.Columns.Contains("Average Active Power 10M (kW)"))
                            {
                                addUnit.avg_active_power = 0;
                                addUnit.status_code = 1;
                            }
                            else if (dr.Table.Columns.Contains("Average Active Power 10M \n(kW)"))
                            {
                                addUnit.avg_active_power = 0;
                                addUnit.status_code = 1;
                            }
                        }

                        //addUnit.avgActivePower = dr["Average Active Power 10M (kW)"] is DBNull ? 0 : Convert.ToDouble(dr["Average Active Power 10M (kW)"]);
                        if (dr.Table.Columns.Contains("Average Wind Speed 10M (m/s)"))
                        {
                            addUnit.avg_wind_speed = dr["Average Wind Speed 10M (m/s)"] is DBNull || string.IsNullOrEmpty((string)dr["Average Wind Speed 10M (m/s)"]) ? 0 : Convert.ToDouble(dr["Average Wind Speed 10M (m/s)"]);
                        }
                        else if (dr.Table.Columns.Contains("Average Wind Speed 10M \n(m/s)"))
                        {
                            addUnit.avg_wind_speed = dr["Average Wind Speed 10M \n(m/s)"] is DBNull || string.IsNullOrEmpty((string)dr["Average Wind Speed 10M \n(m/s)"]) ? 0 : Convert.ToDouble(dr["Average Wind Speed 10M \n(m/s)"]);
                        }

                        if (dr.Table.Columns.Contains("Most restrictive WTG Status 10M ()"))
                        {
                            addUnit.restructive_WTG = dr["Most restrictive WTG Status 10M ()"] is DBNull || string.IsNullOrEmpty((string)dr["Most restrictive WTG Status 10M ()"]) ? 1000 : Convert.ToInt32(dr["Most restrictive WTG Status 10M ()"]);
                        }
                        else if (dr.Table.Columns.Contains("Most restrictive WTG Status 10M \n()"))
                        {
                            addUnit.restructive_WTG = dr["Most restrictive WTG Status 10M \n()"] is DBNull || string.IsNullOrEmpty((string)dr["Most restrictive WTG Status 10M \n()"]) ? 1000 : Convert.ToInt32(dr["Most restrictive WTG Status 10M \n()"]);
                        }


                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.Message + ": Function: InsertWindTMR,");
                        string msg = ",Exception Occurred In Function: InsertWindTMR: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindTMR", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind TMR Validation SuccessFul,");
                    if (isMultiFiles || isMultiSheet == 1)
                    {
                        TMLDataSet.AddRange(addSet);
                        return responseCode = (int)200;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(addSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=1";
                        using (var client = new HttpClient())
                        {

                        client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                        var response = await client.PostAsync(url, data);
                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            if (returnResponse == "5")
                            {
                                m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Error in Calculation.");
                            }
                            //m_ErrorLog.SetInformation(",Wind TMR API SuccessFul,");
                            return responseCode = (int)response.StatusCode;

                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind TMR API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                                return responseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind TML Data Validation Failed,");
                    if (isMultiFiles || isMultiSheet == 1)
                    {
                        tmlMultiError = 1;
                    }
                }
            }
            return responseCode;
        }

        //TML_Data
        private async Task<int> InsertWindTMLData(string status, DataSet ds, string fileName, bool isMultiFiles)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            TMLType = 1;
            int responseCode = 400;
            bool isDateCorrect = false;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindTMLData> addSet = new List<InsertWindTMLData>();
                string previousTime = "00:00:00";
                string previousWTG = "";
                string dataDate = "";
                string fileDateFormat = "";
                string convertedDate = "";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindTMLData addUnit = new InsertWindTMLData();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.file_name = fileName;
                        if(rowNumber > 2)
                        {
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                        }
                        addUnit.onm_wtg = dr["WTGs"] is DBNull || string.IsNullOrEmpty((string)dr["WTGs"]) ? "Nil" : (string)(dr["WTGs"]);
                        if (addUnit.onm_wtg == "" || addUnit.onm_wtg == null)
                        {
                            m_ErrorLog.SetError(", WTGs column of " + rowNumber + " row is empty.");
                            errorCount++;
                            continue;
                        }
                        addUnit.WTGs = onm2equipmentName.ContainsKey(addUnit.onm_wtg) ? onm2equipmentName[addUnit.onm_wtg].ToString() : "";
                        if (addUnit.WTGs == "" || addUnit.WTGs is DBNull || string.IsNullOrEmpty(addUnit.WTGs))
                        {
                            //addUnit.WTGs = addUnit.onm_wtg;
                            m_ErrorLog.SetError(", ONM WTG not found as per master record.");
                            errorCount++;
                        }
                        if (addUnit.WTGs != " ")
                        {
                            addUnit.wtg_id = equipmentId.ContainsKey(addUnit.WTGs) ? Convert.ToInt32(equipmentId[addUnit.WTGs]) : 0;
                            //addUnit.wtg = onm2equipmentName.ContainsKey(tabName) ? onm2equipmentName[tabName].ToString() : "";
                            addUnit.site = SiteByWtg.ContainsKey(addUnit.WTGs) ? SiteByWtg[addUnit.WTGs].ToString() : "";
                            if (addUnit.site != " ")
                            {
                                addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                            }
                            else
                            {
                                addUnit.site_id = 0;
                            }

                        }

                        bool isdateEmpty = dr["Time Stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time Stamp"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Time Stamp value at row " + rowNumber + " is empty. The row would be skiped.");
                            errorCount++;
                            continue;
                        }
                        string timeStampTemp = dr["Time Stamp"].ToString();
                        if (rowNumber == 2)
                        {
                            fileDateFormat = getDateFormat(timeStampTemp);
                        }
                        try
                        {
                            if (fileDateFormat != "Invalid Slash separated date" || fileDateFormat != "Invalid Slash separated date")
                            {
                                switch (fileDateFormat)
                                {
                                    case "dd/MM/yyyy HH:mm:ss":
                                        convertedDate = DateTime.ParseExact(timeStampTemp, "dd/MM/yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    case "yyyy/MM/dd HH:mm:ss":
                                        convertedDate = DateTime.ParseExact(timeStampTemp, "yyyy/MM/dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    case "Contains Hyphen":
                                        convertedDate = Convert.ToDateTime(timeStampTemp).ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    default:
                                        m_ErrorLog.SetError(", Cannot recognize <" + timeStampTemp + "> in Date column of Row <" + rowNumber + "> as date.");
                                        break;
                                }
                            }
                            //string msg1 = "fileDateFormat = " + fileDateFormat + ", convertedDate = " + convertedDate;
                            //LogError(user_id, 2, 4, "InsertWindTMLData", msg1);
                            addUnit.timestamp = convertedDate;
                            errorFlag.Add(stringNullValidation(addUnit.timestamp, "Time Stamp", rowNumber));
                        }
                        catch (Exception e)
                        {
                            string msg = e.ToString();
                            m_ErrorLog.SetError(", File row<" + rowNumber + "> '" + timeStampTemp + "' is not recognised as valid date time, it should be 'yyyy-MM-dd HH:mm:ss'");
                            errorCount++;
                            continue;
                        }

                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToDateTime(convertedDate).ToString("dd-MMM-yy");
                        //string temp_date = temp.Substring(0, 10);
                        if (rowNumber == 2)
                        {
                            isDateCorrect = importTMLDateValidationNew(TMLType, addUnit.site_id, Convert.ToDateTime(convertedDate));
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                            previousTime = Convert.ToDateTime(convertedDate).ToString("HH:mm:ss");
                            dataDate = addUnit.date;
                        }
                        if (rowNumber > 2 && (dataDate != addUnit.date || previousWTG != addUnit.WTGs))
                        {
                            previousTime = "00:00:00";
                            //m_ErrorLog.SetError(", Row <" + rowNumber + "> column <Time Stamp> : <" + dataDate + "> and Date <" + addUnit.date + "> missmatched");
                            //errorCount++;
                        }
                        addUnit.from_time = previousTime;
                        if (addUnit.from_time == "23:40:00")
                        {
                            previousTime = "00:00:00";
                        }
                        addUnit.to_time = Convert.ToDateTime(dr["Time Stamp"]).ToString("HH:mm:ss");
                        if (addUnit.to_time == "23:50:00")
                        {
                            previousTime = "00:00:00";
                        }

                        previousTime = Convert.ToDateTime(convertedDate).ToString("HH:mm:ss");

                        bool isActivePowerEmpty = dr["Actual_Avg_Active_Power_10M"] is DBNull;
                        if (!isActivePowerEmpty)
                        {
                            if (string.IsNullOrEmpty((string)dr["Actual_Avg_Active_Power_10M"]))
                            {
                                isActivePowerEmpty = true;
                            }
                        }

                        if (!isActivePowerEmpty)
                        {
                            addUnit.avg_active_power = Convert.ToDouble(dr["Actual_Avg_Active_Power_10M"]);
                            //Remove Status column from here and database.
                            addUnit.status = "Available";
                            addUnit.status_code = 0;
                            //Change to 1;
                        }

                        if (isActivePowerEmpty)
                        {
                            addUnit.status = "Missing";
                            addUnit.status_code = 1;

                        }

                        addUnit.avg_wind_speed = dr["Actual_Avg_Wind_Speed_10M"] is DBNull || string.IsNullOrEmpty((string)dr["Actual_Avg_Wind_Speed_10M"]) ? 0 : Convert.ToDouble(dr["Actual_Avg_Wind_Speed_10M"]);

                        addUnit.restructive_WTG = dr["Most restrictive WTG Status 10M"] is DBNull || string.IsNullOrEmpty((string)dr["Most restrictive WTG Status 10M"]) ? 1000 : Convert.ToInt32(dr["Most restrictive WTG Status 10M"]);

                        //errorFlag.Add(numericNullValidation(addUnit.restructive_WTG, "Most restrictive WTG Status 10M", rowNumber));

                        previousWTG = addUnit.WTGs;
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.Message + ": Function: InsertWindTMLData,");
                        string msg = ",Exception Occurred In Function: InsertWindTMLData: at rownumber <" + rowNumber + ">" + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindTMLData", msg);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Wind TML Data Validation SuccessFul,");
                    if (isMultiFiles)
                    {
                        TMLDataSet.AddRange(addSet);
                        return responseCode = (int)200;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(addSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=1";
                        using (var client = new HttpClient())
                        {
                            client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                            var response = await client.PostAsync(url, data);
                            string returnResponse = response.Content.ReadAsStringAsync().Result;
                            if (response.IsSuccessStatusCode)
                            {
                                m_ErrorLog.SetInformation(",Wind TML Data API SuccessFul,");
                                //FinalResult = 0 : Complete failure
                                //FinalResult = 1 : Completed till deletion.
                                //FinalResult = 2 : Completed till insertion.
                                //FinalResult = 3 : Completed till updating manual bd column
                                //FinalResult = 4 : Completed till updating reconstructed windspeed.
                                //FinalResult = 5 : Completed till updating expected power column.
                                //FinalResult = 6 : Completed till updating deviation kw column.
                                //FinalResult = 7 : Completed till updating loss kw column.
                                //FinalResult = 8 : Completed till updating all breakdown column.
                                //FinalResult = 9 : Completed till updating all breakdown code column.

                            if (returnResponse == "5")
                            {
                                m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Error in Calculation.");
                            }

                            /*
                            if (returnResponse == "1")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetError(",Error inserting new TML Data.");
                                m_ErrorLog.SetError(",Error updating manual BD column.");
                                m_ErrorLog.SetError(",Error updating reconstructed windspeed column.");
                                m_ErrorLog.SetError(",Error updating expected power column.");
                                m_ErrorLog.SetError(",Error updating deviation kw column.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "2")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetError(",Error updating manual BD column.");
                                m_ErrorLog.SetError(",Error updating reconstructed windspeed column.");
                                m_ErrorLog.SetError(",Error updating expected power column.");
                                m_ErrorLog.SetError(",Error updating deviation kw column.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "3")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetError(",Error updating reconstructed windspeed column.");
                                m_ErrorLog.SetError(",Error updating expected power column.");
                                m_ErrorLog.SetError(",Error updating deviation kw column.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "4")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                m_ErrorLog.SetError(",Error updating expected power column.");
                                m_ErrorLog.SetError(",Error updating deviation kw column.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "5")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                m_ErrorLog.SetInformation(",Updated Expected Power column successfully.");
                                m_ErrorLog.SetError(",Error updating deviation kw column.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "6")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                m_ErrorLog.SetInformation(",Updated Expected Power column successfully.");
                                m_ErrorLog.SetInformation(",Updated Deviation kw column successfully.");
                                m_ErrorLog.SetError(",Error updating loss kw column.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "7")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                m_ErrorLog.SetInformation(",Updated Expected Power column successfully.");
                                m_ErrorLog.SetInformation(",Updated Deviation kw column successfully.");
                                m_ErrorLog.SetInformation(",Updated Loss kw column successfully.");
                                m_ErrorLog.SetError(",Error updating all breakdown column.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "8")
                            {
                                m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                m_ErrorLog.SetInformation(",Updated Expected Power column successfully.");
                                m_ErrorLog.SetInformation(",Updated Deviation kw column successfully.");
                                m_ErrorLog.SetInformation(",Updated Loss kw column successfully.");
                                m_ErrorLog.SetInformation(",Updated All Breakdown column successfully.");
                                m_ErrorLog.SetError(",Error updating all breakdown code column.");
                            }
                            if (returnResponse == "9")
                            {
                                //m_ErrorLog.SetInformation(",Old TML Data deleted successfully.");
                                //m_ErrorLog.SetInformation(",Inserted new TML Data successfully.");
                                //m_ErrorLog.SetInformation(",Updated Manual Breakdown column successfully.");
                                //m_ErrorLog.SetInformation(",Updated Reconstructed Windspeed column successfully.");
                                //m_ErrorLog.SetInformation(",Updated Expected Power column successfully.");
                                //m_ErrorLog.SetInformation(",Updated Deviation kw column successfully.");
                                //m_ErrorLog.SetInformation(",Updated Loss kw column successfully.");
                                //m_ErrorLog.SetInformation(",Updated All Breakdown column successfully.");
                                //m_ErrorLog.SetInformation(",Updated All Breakdown Code column successfully."); 
                                m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                            }
                            */
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind TML Data API Failure,: responseCode <" + (int)response.StatusCode + "> due to exception : " + returnResponse);

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                                return responseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind TML Data Validation Failed,");
                }
            }
            return responseCode;
        }

        //ImportWindSuzlonTMD
        private async Task<int> ImportWindSuzlonTMD(string status, DataSet ds, string fileName, bool isMultiFiles)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            TMLType = 3;
            int responseCode = 400;
            bool isDateCorrect = false;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindTMLData> addSet = new List<InsertWindTMLData>();
                string previoustoTime = "00:00:00";
                string dataDate = "";
                int rowCount = ds.Tables[0].Rows.Count;
                int ColumnCount = ds.Tables[0].Columns.Count;
                int rowcount = 0;
                string LogTime = "";
                //zhb01.xlsx
                string inputString = fileName;
                char separator = '.';
                string[] substrings = inputString.Split(separator);
                string fileNameNew = substrings[0];
                fileNameNew = fileNameNew.ToUpper();
                int loopCount = 0;

                string wtgName = onm2equipmentName.ContainsKey(fileNameNew) ? onm2equipmentName[fileNameNew].ToString() : "";
                if (wtgName == "")
                {
                    wtgName = fileNameNew;
                    m_ErrorLog.SetError(", ONM WTG not found as per master record.");
                    errorCount++;
                }
                int wtgId = equipmentId.ContainsKey(wtgName) ? Convert.ToInt32(equipmentId[wtgName]) : 0;
                string siteName = SiteByWtg.ContainsKey(wtgName) ? SiteByWtg[wtgName].ToString() : "";
                int siteId = siteNameId.ContainsKey(siteName) ? Convert.ToInt32(siteNameId[siteName]) : 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindTMLData addUnit = new InsertWindTMLData();
                    try
                    {
                        if (loopCount < rowCount - 3)
                        {
                            bool skipRow = false;
                            rowNumber++;

                            if (rowNumber > 2)
                            {
                                if (isDateCorrect)
                                {
                                    errorCount++;
                                    continue;
                                }
                            }

                            addUnit.file_name = fileName;
                            addUnit.onm_wtg = fileNameNew;
                            addUnit.WTGs = wtgName;
                            addUnit.site_id = siteId;
                            addUnit.wtg_id = wtgId;
                            addUnit.site = siteName;

                            bool isdateEmpty = dr["Timestamp"] is DBNull || string.IsNullOrEmpty((string)dr["Timestamp"]);
                            if (isdateEmpty)
                            {
                                m_ErrorLog.SetInformation(", Timestamp value is empty. The row would be skiped.");
                                errorCount++;
                                continue;
                            }
                            string timeStampTemp = dr["Timestamp"].ToString();
                            try
                            {
                                addUnit.timestamp = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Timestamp"]).ToString("yyyy-MM-dd HH:mm");
                                addUnit.timestamp += ":00";
                                errorFlag.Add(dateNullValidation(addUnit.timestamp, "Timestamp", rowNumber));
                            }
                            catch (Exception e)
                            {
                                string msg = e.ToString();
                                m_ErrorLog.SetError(", File row<" + rowNumber + "> '" + timeStampTemp + "' is not recognised as valid date time, it should be 'yyyy-MM-dd HH:mm:ss'");
                                errorCount++;
                                continue;
                            }

                            if(rowNumber == 2)
                            {
                                isDateCorrect = importTMLDateValidationNew(TMLType, addUnit.site_id, Convert.ToDateTime(addUnit.timestamp));
                                if (isDateCorrect)
                                {
                                    errorCount++;
                                    continue;
                                }
                            }

                            LogTime = Convert.ToDateTime(addUnit.timestamp).ToString("yyyy-MM-dd");

                            addUnit.date = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Timestamp"]).ToString("dd-MMM-yy");
                            //if (rowNumber == 2)
                            //{
                            //    previousTime = Convert.ToDateTime(dr["Timestamp"]).ToString("HH:mm:ss");
                            //    dataDate = addUnit.date;
                            //}
                            //if (dataDate != addUnit.date)
                            //{
                            //    previousTime = "00:00:00";
                            //}

                            addUnit.from_time = Convert.ToDateTime(dr["Timestamp"]).ToString("HH:mm");
                            addUnit.from_time += ":00";
                            if (addUnit.from_time != "23:50:00")
                            {
                                TimeSpan fromTime = TimeSpan.Parse(addUnit.from_time);
                                addUnit.to_time = fromTime.Add(TimeSpan.FromMinutes(10)).ToString();
                                //nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));
                            }
                            else
                            {
                                addUnit.to_time = "23:59:59";
                            }

                            bool isActivePowerEmpty = string.IsNullOrEmpty((string)dr["AI_intern_ActivPower"]) || dr["AI_intern_ActivPower"] is DBNull;

                            if (!isActivePowerEmpty)
                            {
                                addUnit.avg_active_power = Convert.ToDouble(dr["AI_intern_ActivPower"]);
                                addUnit.status_code = 0;

                            }

                            if (isActivePowerEmpty)
                            {
                                addUnit.status_code = 1;
                            }

                            addUnit.avg_wind_speed = dr["AI_intern_WindSpeed"] is DBNull ? 0 : Convert.ToDouble(dr["AI_intern_WindSpeed"]);

                            //addUnit.restructive_WTG = dr["Most restrictive WTG Status 10M"] is DBNull ? 0 : Convert.ToInt32(dr["Most restrictive WTG Status 10M"]);

                            errorFlag.Clear();
                            if (!(skipRow))
                            {
                                addSet.Add(addUnit);
                            }

                            //Code to get the missing samples.
                            if (rowcount < rowCount - 4)
                            {
                                string nextVariable = ds.Tables[0].Rows[rowcount + 1]["Timestamp"].ToString();
                                string nextFrom = Convert.ToDateTime(nextVariable).ToString("HH:mm");
                                nextFrom += ":00";
                                if (addUnit.to_time != nextFrom)
                                {
                                    int insideCount = 0;
                                    string missingTo = "";
                                    string missingFrom = "";
                                    do
                                    {
                                        TimeSpan fromTimeSpan = new TimeSpan();
                                        TimeSpan nextFromFinal = new TimeSpan();
                                        TimeSpan nextToFinal = new TimeSpan();

                                        if (insideCount == 0)
                                        {
                                            fromTimeSpan = TimeSpan.Parse(addUnit.from_time);
                                        }
                                        if (insideCount > 0)
                                        {
                                            fromTimeSpan = TimeSpan.Parse(missingFrom);
                                        }

                                        nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                        nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                        InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                        addMissingUnit.file_name = fileName;
                                        addMissingUnit.onm_wtg = fileNameNew;
                                        addMissingUnit.WTGs = wtgName;
                                        addMissingUnit.site_id = siteId;
                                        addMissingUnit.wtg_id = wtgId;
                                        addMissingUnit.site = siteName;
                                        addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                        addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                        bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                        addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                        addMissingUnit.from_time = nextFromFinal.ToString();
                                        addMissingUnit.to_time = nextToFinal.ToString();
                                        addMissingUnit.status_code = 1;
                                        addMissingUnit.avg_wind_speed = 0;
                                        missingTo = addMissingUnit.to_time;
                                        missingFrom = addMissingUnit.from_time;
                                        previoustoTime = addMissingUnit.to_time;

                                        addSet.Add(addMissingUnit);

                                        insideCount++;
                                    }
                                    while (missingTo != nextFrom);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.Message + ": Function: InsertWindTMR,");
                        string msg = ",Exception Occurred In Function: InsertWindTMR: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindSuzlonTMD", msg);
                        errorCount++;
                    }

                    rowcount++;
                    loopCount++;
                }
                int sampleCount = addSet.Count();
                if (sampleCount != 144)
                {
                    bool lastSampleMissing = false;
                    bool firstSampleMissing = false;

                    if (Convert.ToString(addSet[sampleCount - 1].from_time) != "23:50:00")
                    {
                        lastSampleMissing = true;
                    }
                    if (Convert.ToString(addSet[0].from_time) != "00:00:00")
                    {
                        firstSampleMissing = true;
                    }

                    if (firstSampleMissing)
                    {
                        string firstFrom = Convert.ToString(addSet[0].from_time);
                        string firstTo = Convert.ToString(addSet[0].to_time);
                        if (firstFrom != "00:00:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = firstFrom;
                            string prevToTime = "00:10:00";
                            string prevFromTime = "00:00:00";
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            List<InsertWindTMLData> MissingList = new List<InsertWindTMLData>();
                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindRegen", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.avg_wind_speed = 0;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    MissingList.Add(addMissingUnit);
                                    //addSet.InsertRange(0, addMissingUnit);
                                    //addSet = addMissingUnit.Concat(addSet);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                                addSet.InsertRange(0, MissingList);
                            }
                            firstFrom = Convert.ToString(addSet[0].from_time);
                            if (firstFrom != "00:00:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();
                                MissingList.Clear();
                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "00:00:00-00:10:00";
                                addMissingUnit1.timestamp = LogTime + " 00:10:00";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "00:00:00";
                                addMissingUnit1.to_time = "00:10:00";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.avg_wind_speed = 0;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                MissingList.Add(addMissingUnit1);
                                addSet.InsertRange(0, MissingList);
                            }
                        }
                    }

                    if (lastSampleMissing)
                    {
                        string lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                        string lastTo = Convert.ToString(addSet[sampleCount - 1].to_time);
                        if (lastFrom != "23:50:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = "23:50:00";
                            string prevToTime = lastTo;
                            string prevFromTime = lastFrom;
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindSuzlonTMD", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.avg_wind_speed = 0;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    addSet.Add(addMissingUnit);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                            }

                            lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                            if (lastFrom != "23:50:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();

                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "23:50:00-23:59:59";
                                addMissingUnit1.timestamp = LogTime + " 23:59:59";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "23:50:00";
                                addMissingUnit1.to_time = "23:59:59";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.avg_wind_speed = 0;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                addSet.Add(addMissingUnit1);
                            }
                        }
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind TMR Validation SuccessFul,");
                    if (isMultiFiles)
                    {
                        TMLDataSet.AddRange(addSet);
                        return responseCode = (int)200;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(addSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        //insertWindTMLData type = 1 : Gamesa ; type = 2 : INOX ; type = 3 : Suzlon.
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=3";
                        using (var client = new HttpClient())
                        {
                            client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                            var response = await client.PostAsync(url, data);
                            string returnResponse = response.Content.ReadAsStringAsync().Result;
                            if (response.IsSuccessStatusCode)
                            {
                                if (returnResponse == "5")
                                {
                                    m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                                }
                                else
                                {
                                    m_ErrorLog.SetError(",Error in Calculation.");
                                }
                                return responseCode = (int)response.StatusCode;
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Wind TMR API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                                return responseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Suzlon TML Validation Failed,");
                }
            }
            return responseCode;
        }

        //InsertWindRejen
        private async Task<int> InsertWindRegen(string status, DataSet ds, string fileName, bool isMultiFiles)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            TMLType = 4;
            int responseCode = 400;
            bool isDateCorrect = false;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindTMLData> addSet = new List<InsertWindTMLData>();
                string previousFromTime = "00:00:00";
                string previousToTime = "00:00:00";
                string dataDate = "";
                int rowCount = ds.Tables[0].Rows.Count;
                int ColumnCount = ds.Tables[0].Columns.Count;
                int rowcount = 0;
                string LogTime = "";
                int loopCount = 0;
                int sampleCount = 0;

                string inputString = fileName;
                char separator = '_';
                string[] substrings = inputString.Split(separator);
                string fileNameNew = substrings[0];

                string wtgName = onm2equipmentName.ContainsKey(fileNameNew) ? onm2equipmentName[fileNameNew].ToString() : "";
                if (wtgName == "")
                {
                    //wtgName = fileNameNew;
                    m_ErrorLog.SetError(", ONM WTG not found as per master record.");
                    errorCount++;
                }

                int wtgId = equipmentId.ContainsKey(wtgName) ? Convert.ToInt32(equipmentId[wtgName]) : 0;

                if (wtgId == 0)
                {
                    m_ErrorLog.SetError(",Invalid WTG name.");
                    errorCount++;
                }

                string siteName = SiteByWtg.ContainsKey(wtgName) ? SiteByWtg[wtgName].ToString() : "";

                int siteId = siteNameId.ContainsKey(siteName) ? Convert.ToInt32(siteNameId[siteName]) : 0;

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindTMLData addUnit = new InsertWindTMLData();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;

                        if (rowNumber > 2)
                        {
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                        }
                        addUnit.file_name = fileName;

                        addUnit.onm_wtg = fileNameNew;

                        addUnit.WTGs = wtgName;

                        addUnit.wtg_id = wtgId;

                        addUnit.site = siteName;

                        addUnit.site_id = siteId;

                        //220727_0000
                        string file_time = dr["time"] is DBNull || string.IsNullOrEmpty((string)dr["time"]) ? "Nil" : Convert.ToString(dr["time"]);
                        string year = "20" + file_time.Substring(0, 2);
                        string month = file_time.Substring(2, 2);
                        string date = file_time.Substring(4, 2);
                        string fromtime = file_time.Substring(7, 2) + ":" + file_time.Substring(9, 2) + ":00";
                        string fullDate = year + "-" + month + "-" + date + " " + fromtime;

                        //Check if minute is 10, 20, 30, 40, 50, 00.
                        //ds.Tables[0].Rows[0]["Site"]
                        string[] fromArr = fromtime.Split(':');
                        int fromHrs = Convert.ToInt32(fromArr[0]);
                        int fromMinutes = Convert.ToInt32(fromArr[1]);
                        int reminder = fromMinutes % 10;
                        string finalFrom = "";
                        if (reminder > 0)
                        {
                            finalFrom = Convert.ToString(TimeSpan.Parse(previousFromTime).Add(TimeSpan.FromMinutes(10)));
                            fromtime = finalFrom;
                        }


                        //DateTime result = DateTime.ParseExact(file_time, "yyyyMMdd_HHmm", CultureInfo.InvariantCulture);
                        DateTime result = Convert.ToDateTime(fullDate);
                        string timeStamp = result.ToString("yyyy-MM-dd HH:mm:ss");

                        addUnit.timestamp = timeStamp;
                        if(rowNumber == 2)
                        {
                            isDateCorrect = importTMLDateValidationNew(TMLType, addUnit.site_id, Convert.ToDateTime(addUnit.timestamp));
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                        }

                        LogTime = Convert.ToDateTime(addUnit.timestamp).ToString("yyyy-MM-dd");

                        bool isdateEmpty = timeStamp == "" || string.IsNullOrEmpty((string)addUnit.timestamp) ? true : false;

                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToDateTime(timeStamp).ToString("dd-MMM-yy");

                        if (rowNumber == 2)
                        {
                            previousFromTime = fromtime;
                            dataDate = addUnit.date;
                        }
                        if (dataDate != addUnit.date)
                        {
                            previousFromTime = "00:00:00";
                            //m_ErrorLog.SetError(", Row <" + rowNumber + "> column <Time Stamp> : <" + dataDate + "> and Date <" + addUnit.date + "> missmatched");
                            //errorCount++;
                        }
                        addUnit.from_time = fromtime;
                        if (addUnit.from_time != "23:50:00")
                        {
                            addUnit.to_time = Convert.ToString(TimeSpan.Parse(fromtime).Add(TimeSpan.FromMinutes(10)));
                        }
                        else
                        {
                            addUnit.to_time = "23:59:59";
                        }

                        previousFromTime = addUnit.from_time;
                        previousToTime = addUnit.to_time;

                        bool isActivePowerEmpty = dr["active_power_avg"] is DBNull || string.IsNullOrEmpty((string)dr["active_power_avg"]);

                        if (!isActivePowerEmpty)
                        {
                            addUnit.avg_active_power = Convert.ToDouble(dr["active_power_avg"]);
                            addUnit.status_code = 0;
                            //Change to 1;
                        }

                        if (isActivePowerEmpty)
                        {
                            addUnit.status_code = 1;
                        }

                        addUnit.avg_wind_speed = dr["wind_speed_avg"] is DBNull || string.IsNullOrEmpty((string)dr["wind_speed_avg"]) ? 0 : Convert.ToDouble(dr["wind_speed_avg"]);

                        addUnit.operation_mode = dr["operation_mode"] is DBNull || string.IsNullOrEmpty((string)dr["operation_mode"]) ? 10000 : Convert.ToInt32(dr["operation_mode"]);

                        addUnit.low_wind_period = dr["low_wind_period_10m"] is DBNull || string.IsNullOrEmpty((string)dr["low_wind_period_10m"]) ? 10000 : Convert.ToInt32(dr["low_wind_period_10m"]);

                        addUnit.service = dr["service_10m"] is DBNull || string.IsNullOrEmpty((string)dr["service_10m"]) ? 10000 : Convert.ToInt32(dr["service_10m"]);

                        addUnit.visit = dr["visit_10m"] is DBNull || string.IsNullOrEmpty((string)dr["visit_10m"]) ? 10000 : Convert.ToInt32(dr["visit_10m"]);

                        addUnit.error = dr["error_10m"] is DBNull || string.IsNullOrEmpty((string)dr["error_10m"]) ? 10000 : Convert.ToInt32(dr["error_10m"]);

                        addUnit.operation = dr["operation_10m"] is DBNull || string.IsNullOrEmpty((string)dr["operation_10m"]) ? 10000 : Convert.ToInt32(dr["operation_10m"]);

                        addUnit.power_production = dr["power_production_10m"] is DBNull || string.IsNullOrEmpty((string)dr["power_production_10m"]) ? 10000 : Convert.ToInt32(dr["power_production_10m"]);

                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        
                        sampleCount++;

                        //Code to get the missing samples.
                        if (rowcount < rowCount - 1)
                        {
                            string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = Convert.ToDateTime(nextVariable).ToString("HH:mm:ss");
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(previousFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            if (addUnit.to_time != nextFrom)
                            {
                                string msg = "previous to time <" + addUnit.to_time + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindRegen", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(addUnit.from_time);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.avg_wind_speed = 0;
                                    addMissingUnit.operation_mode = 10000;
                                    addMissingUnit.low_wind_period = 10000;
                                    addMissingUnit.service = 10000;
                                    addMissingUnit.visit = 10000;
                                    addMissingUnit.error = 10000;
                                    addMissingUnit.operation = 10000;
                                    addMissingUnit.power_production = 10000;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    previousToTime = addMissingUnit.to_time;
                                    previousFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    addSet.Add(addMissingUnit);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindTMLData,");
                        string msg = ",Exception Occurred In Function: InsertWindTMLData: at rownumber <" + rowNumber + ">" + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindRegenTMD", msg);
                        errorCount++;
                    }

                    rowcount++;
                    loopCount++;
                }
                if (sampleCount != 144)
                {
                    bool lastSampleMissing = false;
                    bool firstSampleMissing = false;

                    if (Convert.ToString(addSet[sampleCount - 1].from_time) != "23:50:00")
                    {
                        lastSampleMissing = true;
                    }
                    if (Convert.ToString(addSet[0].from_time) != "00:00:00")
                    {
                        firstSampleMissing = true;
                    }

                    if (firstSampleMissing)
                    {
                        string firstFrom = Convert.ToString(addSet[0].from_time);
                        string firstTo = Convert.ToString(addSet[0].to_time);
                        if (firstFrom != "00:00:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = firstFrom;
                            string prevToTime = "00:10:00";
                            string prevFromTime = "00:00:00";
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            List<InsertWindTMLData> MissingList = new List<InsertWindTMLData>();
                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindRegen", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.avg_wind_speed = 0;
                                    addMissingUnit.operation_mode = 10000;
                                    addMissingUnit.low_wind_period = 10000;
                                    addMissingUnit.service = 10000;
                                    addMissingUnit.visit = 10000;
                                    addMissingUnit.error = 10000;
                                    addMissingUnit.operation = 10000;
                                    addMissingUnit.power_production = 10000;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    MissingList.Add(addMissingUnit);
                                    //addSet.InsertRange(0, addMissingUnit);
                                    //addSet = addMissingUnit.Concat(addSet);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                                addSet.InsertRange(0, MissingList);
                            }
                            firstFrom = Convert.ToString(addSet[0].from_time);
                            if (firstFrom != "00:00:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();
                                MissingList.Clear();
                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "00:00:00-00:10:00";
                                addMissingUnit1.timestamp = LogTime + " 00:10:00";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "00:00:00";
                                addMissingUnit1.to_time = "00:10:00";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.avg_wind_speed = 0;
                                addMissingUnit1.operation_mode = 10000;
                                addMissingUnit1.low_wind_period = 10000;
                                addMissingUnit1.service = 10000;
                                addMissingUnit1.visit = 10000;
                                addMissingUnit1.error = 10000;
                                addMissingUnit1.operation = 10000;
                                addMissingUnit1.power_production = 10000;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                MissingList.Add(addMissingUnit1);
                                addSet.InsertRange(0, MissingList);
                            }
                        }
                    }
                    if (lastSampleMissing)
                    {
                        string lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                        string lastTo = Convert.ToString(addSet[sampleCount - 1].to_time);
                        if (lastFrom != "23:50:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = "23:50:00";
                            string prevToTime = lastTo;
                            string prevFromTime = lastFrom;
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindRegen", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.avg_wind_speed = 0;
                                    addMissingUnit.operation_mode = 10000;
                                    addMissingUnit.low_wind_period = 10000;
                                    addMissingUnit.service = 10000;
                                    addMissingUnit.visit = 10000;
                                    addMissingUnit.error = 10000;
                                    addMissingUnit.operation = 10000;
                                    addMissingUnit.power_production = 10000;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    addSet.Add(addMissingUnit);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                            }

                            lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                            if (lastFrom != "23:50:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();

                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "23:50:00-23:59:59";
                                addMissingUnit1.timestamp = LogTime + " 23:59:59";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "23:50:00";
                                addMissingUnit1.to_time = "23:59:59";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.avg_wind_speed = 0;
                                addMissingUnit1.operation_mode = 10000;
                                addMissingUnit1.low_wind_period = 10000;
                                addMissingUnit1.service = 10000;
                                addMissingUnit1.visit = 10000;
                                addMissingUnit1.error = 10000;
                                addMissingUnit1.operation = 10000;
                                addMissingUnit1.power_production = 10000;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                addSet.Add(addMissingUnit1);
                            }
                        }
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Wind TML Data Validation SuccessFul,");
                    if (isMultiFiles)
                    {
                        TMLDataSet.AddRange(addSet);
                        return responseCode = (int)200;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(addSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=4";
                        using (var client = new HttpClient())
                        {
                            client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                            var response = await client.PostAsync(url, data);
                            string returnResponse = response.Content.ReadAsStringAsync().Result;
                            if (response.IsSuccessStatusCode)
                            {
                                m_ErrorLog.SetInformation(",Wind TML Data API SuccessFul,");

                            if (returnResponse == "5")
                            {
                                m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Error in Calculation.");
                            }
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind TML Data API Failure,: responseCode <" + (int)response.StatusCode + "> due to exception : " + returnResponse);

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                                return responseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind TML Data Validation Failed,");
                }
            }
            return responseCode;
        }

        //ImportWindInoxTMD
        Hashtable bdCodeTypeHash = new Hashtable();
        Hashtable bdCodeHash = new Hashtable();
        private async Task<int> ImportWindInoxTMD(string status, DataSet ds, string fileName, bool isMultiFiles)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            TMLType = 2;
            int responseCode = 400;
            bool isDateCorrect = false;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindTMLData> addSet = new List<InsertWindTMLData>();
                string previoustoTime = "00:10:00";
                string previousFromTime = "00:00:00";
                string dataDate = "";
                int columnCount = 0;
                int rowCount = ds.Tables[0].Rows.Count;
                int ColumnCount = ds.Tables[0].Columns.Count;
                int RowNo_variable = 0;
                int RowNo_timestamp = 0;
                int RowNo_activePower = 0;
                int RowNo_plcMax = 0;
                int RowNo_plcMin = 0;
                int RowNo_pc_validity = 0;
                int RowNo_windspeed = 0;
                bool lastDiscard = false;
                int subtractLast = 0;
                int finalResult = 0;
                string LogTime = "";
                string finalToTime = "";
                //TenMinLog_05_01.04.2023

                //KBs-10.xlsx
                //string fileNameNew = fileName.Substring(0, (fileName.Length - 5));

                //TenMinLog_KBS05_01.04.2023
                string inputString = fileName;
                //m_ErrorLog.SetInformation(",File Name : " + fileName);
                char separator = '_';
                string[] substrings = inputString.Split(separator);
                string fileNameNew = substrings[1];

                string wtgName = onm2equipmentName.ContainsKey(fileNameNew) ? onm2equipmentName[fileNameNew].ToString() : "";
                if (wtgName == "")
                {
                    //wtgName = fileNameNew;
                    m_ErrorLog.SetError(", ONM WTG not found as per master record.");
                    errorCount++;
                }
                int wtgId = equipmentId.ContainsKey(wtgName) ? Convert.ToInt32(equipmentId[wtgName]) : 0;
                if (wtgId == 0)
                {
                    m_ErrorLog.SetError(",Invalid WTG name.");
                    errorCount++;
                }
                string siteName = SiteByWtg.ContainsKey(wtgName) ? SiteByWtg[wtgName].ToString() : "";
                int siteId = siteNameId.ContainsKey(siteName) ? Convert.ToInt32(siteNameId[siteName]) : 0;

                bdCodeINOX(siteId);

                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    InsertWindTMLData addUnit = new InsertWindTMLData();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;

                        if(columnCount > 1)
                        {
                            if (isDateCorrect)
                            {
                                errorCount++;
                                continue;
                            }
                        }

                        //addUnit.timestamp = dc["Variable"];
                        if (columnCount == 0)
                        {
                            for (int row = 1; row < rowCount; row++)
                            {
                                try
                                {
                                    if (row == 1)
                                    {
                                        if(ds.Tables[0].Columns[ColumnCount-1].ToString() == "Source name")
                                        {
                                            lastDiscard = true;
                                        }
                                    }
                                    if (dc.ColumnName.ToString() == "Variable")
                                    {
                                        RowNo_variable = row; //0
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "Log time (Local)")
                                    {
                                        RowNo_timestamp = row; //1
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "Active power - AVE [kW]")
                                    {
                                        RowNo_activePower = row; //9
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "PLC state - MAX")
                                    {
                                        RowNo_plcMax = row; //149
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "PLC state - MIN")
                                    {
                                        RowNo_plcMin = row; //150
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "Power curve validity - MIN")
                                    {
                                        RowNo_pc_validity = row; //157
                                    }
                                    if (ds.Tables[0].Rows[row][columnCount].ToString() == "Wind speed - AVE [m/s]")
                                    {
                                        RowNo_windspeed = row; //200
                                    }
                                    finalResult = 1;
                                }
                                catch (Exception e)
                                {
                                    string msg = "Exception while getting row numbers of required rows of each column, due to : " + e.ToString();
                                    //ErrorLog(msg);
                                    LogError(user_id, 2, 4, "ImportWindInoxTMD", msg);
                                }
                            }
                            //string cellValue = ds.Tables[0].Rows[row][columnCount].ToString();
                        }
                        if (lastDiscard)
                        {
                            subtractLast = 1;
                        }
                        else
                        {
                            subtractLast = 0;
                        }
                        if (columnCount > 0 && columnCount < ColumnCount - subtractLast)
                        {
                            if (columnCount == 1)
                            {
                                int tempp = 0;
                            }
                            if (finalResult == 1)
                            {
                                if (columnCount > 1)
                                {
                                    if (isDateCorrect)
                                    {
                                        errorCount++;
                                        continue;
                                    }
                                }
                                addUnit.file_name = fileName;
                                addUnit.onm_wtg = fileNameNew;
                                addUnit.WTGs = wtgName;
                                addUnit.site_id = siteId;
                                addUnit.wtg_id = wtgId;
                                addUnit.site = siteName;

                                addUnit.variable = dc.ColumnName is DBNull || string.IsNullOrEmpty((string)dc.ColumnName) ? "Null" : dc.ColumnName.ToString();

                                string input = addUnit.variable;
                                char sep = '-';
                                string[] substr = input.Split(sep);
                                string toTime = "";
                                string fromTime = "";
                                if (substr.Length > 0)
                                {
                                    string[] fromArr = substr[0].Split(':');
                                    string[] toArr = substr[1].Split(':');
                                    fromTime = fromArr[0] + ":" + fromArr[1] + ":" + "00";
                                    toTime = toArr[0] + ":" + toArr[1] + ":" + "00";
                                }
                                else
                                {
                                    //ErrorLog("substr array is blank.");
                                    LogError(user_id, 2, 4, "ImportWindInoxTMD", "substr array is blank.");
                                }

                                if (toTime == "24:00:00")
                                {
                                    toTime = "23:59:00";
                                }

                                if (columnCount == 1)
                                {
                                    bool isdateEmpty = ds.Tables[0].Rows[RowNo_timestamp][columnCount] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_timestamp][columnCount]);
                                    if (isdateEmpty)
                                    {
                                        m_ErrorLog.SetError(", Log Time (Local) value is empty.");
                                        errorCount++;
                                    }
                                    isDateCorrect = importTMLDateValidationNew(TMLType, addUnit.site_id, Convert.ToDateTime(ds.Tables[0].Rows[RowNo_timestamp][columnCount]));
                                    if (isDateCorrect)
                                    {
                                        errorCount++;
                                        continue;
                                    }
                                    LogTime = isdateEmpty ? "Nil" : Convert.ToDateTime(ds.Tables[0].Rows[RowNo_timestamp][columnCount]).ToString("yyyy-MM-dd");
                                }
                                addUnit.timestamp = LogTime + " " + toTime;

                                bool isTimeEmpty = addUnit.timestamp == "" || addUnit.timestamp is DBNull || addUnit.timestamp == " " ? true : false;
                                addUnit.date = isTimeEmpty ? "Nil" : Convert.ToDateTime(addUnit.timestamp).ToString("dd-MMM-yy");

                                //check whether to time data needs to be converted into higher 10 min to lower 10 min ?
                                bool toNeedToCorrect = false;
                                bool isToHigher = true;

                                if (columnCount < ColumnCount - 2)
                                {
                                    string inPut = toTime;
                                    char seP = ':';
                                    string[] outPut = inPut.Split(seP);

                                    string inPutFrom = fromTime;
                                    char sePFrom = ':';
                                    string[] outPutFrom = inPutFrom.Split(sePFrom);
                                    if (outPut.Length > 0 && outPutFrom.Length > 0)
                                    {
                                        int Hrs = Convert.ToInt32(outPut[0]);
                                        int Min = Convert.ToInt32(outPut[1]);
                                        int HrsFrom = Convert.ToInt32(outPutFrom[0]);
                                        int MinFrom = Convert.ToInt32(outPutFrom[1]);
                                        int difference = Min - 10;
                                        if (difference != MinFrom)
                                        {
                                            toNeedToCorrect = true;
                                            if (difference < MinFrom)
                                            {
                                                isToHigher = true;
                                            }
                                            if (difference > MinFrom)
                                            {
                                                isToHigher = false;
                                            }
                                        }
                                        else
                                        {
                                            toNeedToCorrect = false;
                                        }
                                    }
                                }
                                if (toNeedToCorrect)
                                {
                                    if (toTime != "")
                                    {
                                        if (isToHigher)
                                        {
                                            //check if to time is 10, 20, 30, 40, 50, 00 if not then convert it into greater closest.
                                            //00:10:00
                                            string inputStrings = toTime;
                                            char sepre = ':';
                                            string[] output = inputStrings.Split(sepre);
                                            if (output.Length > 0)
                                            {
                                                int minute = Convert.ToInt32(output[1]);
                                                int remainder = minute % 10;
                                                int hour = Convert.ToInt32(output[0]);

                                                if (remainder > 0)
                                                {
                                                    minute = minute + (10 - remainder);
                                                    if (remainder < 9)
                                                    {
                                                        finalToTime = output[0] + ":" + minute.ToString() + ":" + "00";
                                                    }
                                                    else
                                                    {
                                                        if (remainder == 9)
                                                        {
                                                            if (hour <= 23)
                                                            {
                                                                if (hour == 23 && Convert.ToInt32(output[1]) > 55)
                                                                {
                                                                    finalToTime = hour.ToString() + ":" + output[1] + ":" + "00";
                                                                }
                                                                hour++;
                                                                finalToTime = hour + ":" + "00" + ":" + "00";
                                                            }
                                                            else
                                                            {
                                                                if (inputStrings == "24:00:00")
                                                                {
                                                                    finalToTime = "23:59:00";
                                                                }
                                                                finalToTime = inputStrings;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    finalToTime = inputStrings;
                                                }
                                            }
                                        }
                                        if (!isToHigher)
                                        {
                                            //check if to time is 10, 20, 30, 40, 50, 00 if not then convert it into smallest closest.
                                            //00:10:00
                                            string inputStrings = toTime;
                                            char sepre = ':';
                                            string[] output = inputStrings.Split(sepre);
                                            if (output.Length > 0)
                                            {
                                                int minute = Convert.ToInt32(output[1]);
                                                int remainder = minute % 10;
                                                int hour = Convert.ToInt32(output[0]);

                                                if (remainder > 0)
                                                {
                                                    minute = minute - (10 - remainder);
                                                    if (remainder < 9)
                                                    {
                                                        finalToTime = output[0] + ":" + minute.ToString() + ":" + "00";
                                                    }
                                                    else
                                                    {
                                                        if (remainder == 9)
                                                        {
                                                            if (hour <= 23)
                                                            {
                                                                if (hour == 23 && Convert.ToInt32(output[1]) > 55)
                                                                {
                                                                    finalToTime = hour.ToString() + ":" + output[1] + ":" + "00";
                                                                }
                                                                hour--;
                                                                finalToTime = hour + ":" + "00" + ":" + "00";
                                                            }
                                                            else
                                                            {
                                                                if (inputStrings == "24:00:00")
                                                                {
                                                                    finalToTime = "23:59:00";
                                                                }
                                                                finalToTime = inputStrings;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    finalToTime = inputStrings;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    finalToTime = toTime;
                                }
                                //Check if the from time is 10, 20, 30, 40, 50 if not convert it into the same.
                                string finalFrom = "";
                                if (fromTime != "" || finalToTime != "")
                                {
                                    string inputFrom = fromTime;
                                    char sepr = ':';
                                    string[] output = inputFrom.Split(sepr);
                                    if (output.Length > 0)
                                    {
                                        int hrs = Convert.ToInt32(output[0]);
                                        int mins = Convert.ToInt32(output[1]);
                                        int secs = Convert.ToInt32(output[2]);
                                        int remainder = mins % 10;
                                        if (remainder != 0)
                                        {
                                            TimeSpan actFrom = new TimeSpan();
                                            TimeSpan subFrom = new TimeSpan();
                                            actFrom = TimeSpan.Parse(finalToTime);
                                            if (columnCount == 1 && hrs == 00 && finalToTime == "00:20:00")
                                            {
                                                finalFrom = "00:10:00";
                                            }
                                            else if (columnCount >= 1 || (columnCount == 1 && finalToTime != "00:20:00") && finalFrom == "")
                                            {
                                                subFrom = actFrom.Subtract(TimeSpan.FromMinutes(10));
                                                finalFrom = subFrom.ToString();
                                                if (finalFrom != "")
                                                {
                                                    //if(finalFrom != previoustoTime)
                                                    //{
                                                    //    TimeSpan fromAsPrev = new TimeSpan();
                                                    //    fromAsPrev = TimeSpan.Parse(previoustoTime);
                                                    //    subFrom = fromAsPrev.Add(TimeSpan.FromMinutes(10));
                                                    //    finalFrom = subFrom.ToString();
                                                    //}
                                                }
                                            }
                                        }
                                        else
                                        {
                                            finalFrom = fromTime;
                                        }
                                    }
                                }
                                addUnit.from_time = finalFrom;
                                addUnit.to_time = finalToTime;
                                previoustoTime = addUnit.to_time;
                                previousFromTime = addUnit.from_time;

                                //checking if the active power is present or not 0 = Available 1 = Missing.

                                bool isActivePowerEmpty = string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_activePower][columnCount]) || ds.Tables[0].Rows[RowNo_activePower][columnCount] is DBNull;

                                if (!isActivePowerEmpty)
                                {
                                    addUnit.avg_active_power = Convert.ToDouble(ds.Tables[0].Rows[RowNo_activePower][columnCount]);
                                    addUnit.status_code = 0;

                                }

                                if (isActivePowerEmpty)
                                {
                                    addUnit.status_code = 1;
                                }

                                addUnit.PLC_max = ds.Tables[0].Rows[RowNo_plcMax][columnCount] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_plcMax][columnCount]) ? "Nill" : Convert.ToString(ds.Tables[0].Rows[RowNo_plcMax][columnCount]);
                                bool isPlcMax = addUnit.PLC_max is DBNull || string.IsNullOrEmpty((string)addUnit.PLC_max) ? false : true;

                                addUnit.PLC_min = ds.Tables[0].Rows[RowNo_plcMin][columnCount] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_plcMin][columnCount]) ? "Nill" : Convert.ToString(ds.Tables[0].Rows[RowNo_plcMin][columnCount]);
                                bool isPlcMin = addUnit.PLC_min is DBNull || string.IsNullOrEmpty((string)addUnit.PLC_min) ? false : true;

                                addUnit.PC_validity = ds.Tables[0].Rows[RowNo_pc_validity][columnCount] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_pc_validity][columnCount]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[RowNo_pc_validity][columnCount]);

                                addUnit.avg_wind_speed = ds.Tables[0].Rows[RowNo_windspeed][columnCount] is DBNull || string.IsNullOrEmpty((string)ds.Tables[0].Rows[RowNo_windspeed][columnCount]) ? 0 : Convert.ToDouble(ds.Tables[0].Rows[RowNo_windspeed][columnCount]);

                                //Calculation of PLC_state Code.
                                if (isPlcMax || isPlcMin)
                                {
                                    //condition 1
                                    if (addUnit.PLC_max == "(7) Production" && addUnit.PLC_min == "(7) Production" && (addUnit.PC_validity == 3 || addUnit.PC_validity == 2))
                                    {
                                        addUnit.plc_state_code = "7";
                                    }
                                    //condition 2
                                    else if (addUnit.PLC_max == "(7) Production" && addUnit.PLC_min == "(7) Production" && addUnit.PC_validity == 1)
                                    {
                                        addUnit.plc_state_code = "14";
                                    }
                                    //Condition 3
                                    else if (addUnit.PLC_max == "(7) Production" && addUnit.PLC_min != "(7) Production")
                                    {
                                        addUnit.plc_state_code = bdCodeHash.ContainsKey(addUnit.PLC_min) ? Convert.ToString(bdCodeHash[addUnit.PLC_min]) : "Nil";
                                    }
                                    else
                                    {
                                        addUnit.plc_state_code = bdCodeHash.ContainsKey(addUnit.PLC_max) ? Convert.ToString(bdCodeHash[addUnit.PLC_max]) : "Nil";
                                    }
                                }

                                //Calculating all bd

                                if (addUnit.status_code == 0)
                                {
                                    addUnit.all_bd = bdCodeTypeHash.ContainsKey(addUnit.plc_state_code) ? Convert.ToString(bdCodeTypeHash[addUnit.plc_state_code]) : "NC";
                                }

                                if (!(skipRow))
                                {
                                    addSet.Add(addUnit);
                                }
                                if (columnCount > 70)
                                {
                                    int trml = 0;
                                }

                                //Code to get the missing samples.
                                if (columnCount < ColumnCount - 2)
                                {
                                    string nextVariable = ds.Tables[0].Columns[columnCount + 1].ColumnName.ToString();
                                    string[] nextTime = nextVariable.Split(sep);

                                    string[] nextFromArr = nextTime[0].Split(':');
                                    string[] nextToArr = nextTime[1].Split(':');

                                    string nextFrom = nextFromArr[0] + ":" + nextFromArr[1] + ":00";
                                    string nextTo = nextToArr[0] + ":" + nextToArr[1] + ":00";

                                    string inputFrom = nextFrom;
                                    char sepr = ':';
                                    string[] output = inputFrom.Split(sepr);
                                    if (output.Length > 0)
                                    {
                                        int hrs = Convert.ToInt32(output[0]);
                                        int mins = Convert.ToInt32(output[1]);
                                        int seconds = Convert.ToInt32(output[2]);
                                        int remainder = mins % 10;
                                        if (remainder != 0)
                                        {
                                            TimeSpan nextfromtemp = new TimeSpan();
                                            nextfromtemp = TimeSpan.Parse(nextTo).Subtract(TimeSpan.FromMinutes(10));
                                            nextFrom = nextfromtemp.ToString();
                                        }

                                    }

                                    if (addUnit.to_time != nextFrom)
                                    {
                                        int insideCount = 0;
                                        string missingTo = "";
                                        string missingFrom = "";
                                        do
                                        {
                                            TimeSpan fromTimeSpan = new TimeSpan();
                                            TimeSpan nextFromFinal = new TimeSpan();
                                            TimeSpan nextToFinal = new TimeSpan();

                                            if (insideCount == 0)
                                            {
                                                fromTimeSpan = TimeSpan.Parse(addUnit.from_time);
                                            }
                                            if (insideCount > 0)
                                            {
                                                fromTimeSpan = TimeSpan.Parse(missingFrom);
                                            }

                                            nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                            nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                            InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                            addMissingUnit.file_name = fileName;
                                            addMissingUnit.onm_wtg = fileNameNew;
                                            addMissingUnit.WTGs = wtgName;
                                            addMissingUnit.site_id = siteId;
                                            addMissingUnit.wtg_id = wtgId;
                                            addMissingUnit.site = siteName;
                                            addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                            addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                            bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                            addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                            addMissingUnit.from_time = nextFromFinal.ToString();
                                            addMissingUnit.to_time = nextToFinal.ToString();
                                            addMissingUnit.status_code = 1;
                                            addMissingUnit.all_bd = "NC";
                                            addMissingUnit.avg_wind_speed = 0;
                                            missingTo = addMissingUnit.to_time;
                                            missingFrom = addMissingUnit.from_time;
                                            previoustoTime = addMissingUnit.to_time;

                                            addSet.Add(addMissingUnit);

                                            insideCount++;
                                        }
                                        while (missingTo != nextFrom);
                                    }
                                }
                            }
                        }

                        columnCount++;
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError(",File Column<" + rowNumber + ">" + e.Message + ": Function: InsertWindTMR,");
                        string msg = "Exception Occurred In Function: InsertWindTMR: " + e.ToString() + ", column count : " + columnCount;
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "ImportWindInoxTMD", msg);
                        errorCount++;
                    }
                }
                int sampleCount = addSet.Count();
                if (sampleCount != 144)
                {
                    bool lastSampleMissing = false;
                    bool firstSampleMissing = false;

                    if (Convert.ToString(addSet[sampleCount - 1].from_time) != "23:50:00")
                    {
                        lastSampleMissing = true;
                    }
                    if (Convert.ToString(addSet[0].from_time) != "00:00:00")
                    {
                        firstSampleMissing = true;
                    }

                    if (firstSampleMissing)
                    {
                        string firstFrom = Convert.ToString(addSet[0].from_time);
                        string firstTo = Convert.ToString(addSet[0].to_time);
                        if (firstFrom != "00:00:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = firstFrom;
                            string prevToTime = "00:10:00";
                            string prevFromTime = "00:00:00";
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            List<InsertWindTMLData> MissingList = new List<InsertWindTMLData>();
                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindRegen", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.all_bd = "NC";
                                    addMissingUnit.avg_wind_speed = 0;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    MissingList.Add(addMissingUnit);
                                    //addSet.InsertRange(0, addMissingUnit);
                                    //addSet = addMissingUnit.Concat(addSet);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                                addSet.InsertRange(0, MissingList);
                            }
                            firstFrom = Convert.ToString(addSet[0].from_time);
                            if (firstFrom != "00:00:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();
                                MissingList.Clear();
                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "00:00:00-00:10:00";
                                addMissingUnit1.timestamp = LogTime + " 00:10:00";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "00:00:00";
                                addMissingUnit1.to_time = "00:10:00";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.all_bd = "NC";
                                addMissingUnit1.avg_wind_speed = 0;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                MissingList.Add(addMissingUnit1);
                                addSet.InsertRange(0, MissingList);
                            }
                        }
                    }

                    if (lastSampleMissing)
                    {
                        string lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                        string lastTo = Convert.ToString(addSet[sampleCount - 1].to_time);
                        if (lastFrom != "23:50:00")
                        {
                            //string nextTime = ds.Tables[0].Rows[rowcount + 1]["time"].ToString();
                            //string nextVariable = nextTime.Substring(7, 2) + ":" + nextTime.Substring(9, 2) + ":00";

                            string nextFrom = "23:50:00";
                            string prevToTime = lastTo;
                            string prevFromTime = lastFrom;
                            string[] nextfromArr = nextFrom.Split(':');
                            int nextFromHrs = Convert.ToInt32(nextfromArr[0]);
                            int nextFromMinutes = Convert.ToInt32(nextfromArr[1]);
                            int nextReminder = nextFromMinutes % 10;
                            string nextFinalFrom = "";
                            //previousFromTime = lastFrom;
                            if (nextReminder > 0)
                            {
                                nextFinalFrom = Convert.ToString(TimeSpan.Parse(prevFromTime).Add(TimeSpan.FromMinutes(10)));
                                nextFrom = nextFinalFrom;
                            }

                            if (prevToTime != nextFrom)
                            {
                                string msg = "previous to time <" + prevToTime + "> next from time <" + nextFrom + ">.";
                                //InformationLog(msg);
                                LogInfo(user_id, 2, 4, "InsertWindInox", msg);
                                int insideCount = 0;
                                string missingTo = "";
                                string missingFrom = "";
                                do
                                {
                                    TimeSpan fromTimeSpan = new TimeSpan();
                                    TimeSpan nextFromFinal = new TimeSpan();
                                    TimeSpan nextToFinal = new TimeSpan();

                                    if (insideCount == 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(prevFromTime);
                                    }
                                    if (insideCount > 0)
                                    {
                                        fromTimeSpan = TimeSpan.Parse(missingFrom);
                                    }

                                    nextFromFinal = fromTimeSpan.Add(TimeSpan.FromMinutes(10));
                                    nextToFinal = nextFromFinal.Add(TimeSpan.FromMinutes(10));

                                    InsertWindTMLData addMissingUnit = new InsertWindTMLData();

                                    addMissingUnit.file_name = fileName;
                                    addMissingUnit.onm_wtg = fileNameNew;
                                    addMissingUnit.WTGs = wtgName;
                                    addMissingUnit.site_id = siteId;
                                    addMissingUnit.wtg_id = wtgId;
                                    addMissingUnit.site = siteName;
                                    addMissingUnit.variable = nextFromFinal.ToString() + "-" + nextToFinal.ToString();
                                    addMissingUnit.timestamp = LogTime + " " + nextToFinal.ToString();
                                    bool TimeEmpty = addMissingUnit.timestamp == "" || addMissingUnit.timestamp is DBNull || addMissingUnit.timestamp == " " ? true : false;
                                    addMissingUnit.date = TimeEmpty ? "Nil" : Convert.ToDateTime(addMissingUnit.timestamp).ToString("dd-MMM-yy");
                                    addMissingUnit.from_time = nextFromFinal.ToString();
                                    addMissingUnit.to_time = nextToFinal.ToString();
                                    addMissingUnit.status_code = 1;
                                    addMissingUnit.all_bd = "NC";
                                    addMissingUnit.avg_wind_speed = 0;
                                    missingTo = addMissingUnit.to_time;
                                    missingFrom = addMissingUnit.from_time;
                                    prevToTime = addMissingUnit.to_time;
                                    prevFromTime = addMissingUnit.from_time;
                                    sampleCount++;
                                    addSet.Add(addMissingUnit);

                                    insideCount++;
                                }
                                while (missingTo != nextFrom);
                            }

                            lastFrom = Convert.ToString(addSet[sampleCount - 1].from_time);
                            if (lastFrom != "23:50:00")
                            {
                                InsertWindTMLData addMissingUnit1 = new InsertWindTMLData();

                                addMissingUnit1.file_name = fileName;
                                addMissingUnit1.onm_wtg = fileNameNew;
                                addMissingUnit1.WTGs = wtgName;
                                addMissingUnit1.site_id = siteId;
                                addMissingUnit1.wtg_id = wtgId;
                                addMissingUnit1.site = siteName;
                                addMissingUnit1.variable = "23:50:00-23:59:59";
                                addMissingUnit1.timestamp = LogTime + " 23:59:59";
                                bool TimeEmpty1 = addMissingUnit1.timestamp == "" || addMissingUnit1.timestamp is DBNull || addMissingUnit1.timestamp == " " ? true : false;
                                addMissingUnit1.date = TimeEmpty1 ? "Nil" : Convert.ToDateTime(addMissingUnit1.timestamp).ToString("dd-MMM-yy");
                                addMissingUnit1.from_time = "23:50:00";
                                addMissingUnit1.to_time = "23:59:59";
                                addMissingUnit1.status_code = 1;
                                addMissingUnit1.all_bd = "NC";
                                addMissingUnit1.avg_wind_speed = 0;
                                prevToTime = addMissingUnit1.to_time;
                                prevFromTime = addMissingUnit1.from_time;
                                sampleCount++;
                                addSet.Add(addMissingUnit1);
                            }

                        }

                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind TMR Validation SuccessFul,");
                    if (isMultiFiles)
                    {
                        TMLDataSet.AddRange(addSet);
                        return responseCode = (int)200;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(addSet);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                    //insertWindTMLData type = 1 : Gamesa ; type = 2 : INOX ; type = 3 : Suzlon.
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindTMLData?type=2";
                    using (var client = new HttpClient())
                    {
                        client.Timeout = Timeout.InfiniteTimeSpan; // disable the HttpClient timeout
                        var response = await client.PostAsync(url, data);
                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            if (returnResponse == "5")
                            {
                                m_ErrorLog.SetInformation("TML_Data file imported successfully.");
                            }
                            else
                            {
                                m_ErrorLog.SetError(",Error in Calculation.");
                            }
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind TMR API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                                return responseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind INOX TML Validation Failed,");
                }
            }
            return responseCode;
        }

        //get bd_code_INOX into hashtable.
        public void bdCodeINOX(int site_id)
        {
            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindBdCodeINOX?site_id=" + site_id;
            var result = string.Empty;
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }

            bdCodeHash.Clear();
            bdCodeTypeHash.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                string code = (string)Convert.ToString(dr["code"]);//D
                bdCodeHash.Add((string)dr["plc_state"], code);
                bdCodeTypeHash.Add(code, dr["type"]);
            }
        }

        //InsertWindPowerCurve
        private async Task<int> InsertWindPowerCurve(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindPowerCurve> addSet = new List<InsertWindPowerCurve>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindPowerCurve addUnit = new InsertWindPowerCurve();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        if (addUnit.site_id == 0)
                        {
                            //m_ErrorLog.SetError("Site name <" + addUnit.site + "> fail to match with data in Location Master at row <" + rowNumber + ">.");
                            errorCount++;
                        }

                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.wind_speed = dr["Wind Speed (m/s)"] is DBNull || string.IsNullOrEmpty((string)dr["Wind Speed (m/s)"]) ? 0 : Convert.ToDouble(dr["Wind Speed (m/s)"]);

                        addUnit.active_power = dr["Active Power (kW)"] is DBNull || string.IsNullOrEmpty((string)dr["Active Power (kW)"]) ? 0 : Convert.ToDouble(dr["Active Power (kW)"]);

                        //errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindPowerCurve,");
                        string msg = ",Exception Occurred In Function: InsertWindPowerCurve: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindPowerCurve", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Power Curve Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindPowerCurve";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Power Curve API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Power Curve API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind TML Data Validation Failed,");
                }
            }
            return responseCode;
        }
        //InsertWindBDCodeGamesa
        private async Task<int> InsertWindBDCodeGamesa(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindBDCodesGamesa> addSet = new List<InsertWindBDCodesGamesa>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindBDCodesGamesa addUnit = new InsertWindBDCodesGamesa();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.codes = dr["Codes"] is DBNull || string.IsNullOrEmpty((string)dr["Codes"]) ? 0 : Convert.ToInt32(dr["Codes"]);

                        addUnit.description = dr["Description"] is DBNull || string.IsNullOrEmpty((string)dr["Description"]) ? "" : Convert.ToString(dr["Description"]);

                        addUnit.conditions = dr["Conditions"] is DBNull || string.IsNullOrEmpty((string)dr["Conditions"]) ? "" : Convert.ToString(dr["Conditions"]);

                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindBDCodeGamesa,");
                        string msg = ",Exception Occurred In Function: InsertWindBDCodeGamesa: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindBDCodeGamesa", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind BD Code Gamesa Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindBDCodeGamesa";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",BD code Gamesa API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",BD Code gamesa API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind BD Code Gamesa Validation Failed,");
                }
            }
            return responseCode;
        }

        //ImportWindBDCodeINOX
        private async Task<int> InsertWindBDCodeINOX(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<ImportWindBDCodeINOX> addSet = new List<ImportWindBDCodeINOX>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ImportWindBDCodeINOX addUnit = new ImportWindBDCodeINOX();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.plc_state = dr["PLC-State"] is DBNull || string.IsNullOrEmpty((string)dr["PLC-State"]) ? "Nil" : Convert.ToString(dr["PLC-State"]);

                        addUnit.code = dr["Code"] is DBNull || string.IsNullOrEmpty((string)dr["Code"]) ? "Nil" : Convert.ToString(dr["Code"]);

                        addUnit.type = dr["Type"] is DBNull || string.IsNullOrEmpty((string)dr["Type"]) ? "Nil" : Convert.ToString(dr["Type"]);

                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindBDCodeGamesa,");
                        string msg = ",Exception Occurred In Function: InsertWindBDCodeInox: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindBDCodeInox", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind BD Code INOX Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/ImportWindBDCodeINOX";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",BD code INOX API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",BD Code INOX API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind BD Code INOX Validation Failed,");
                }
            }
            return responseCode;
        }

        //InserttWindBDCodeREGEN
        private async Task<int> InsertWindBDCodeREGEN(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<InsertWindBDCodeREGEN> addSet = new List<InsertWindBDCodeREGEN>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindBDCodeREGEN addUnit = new InsertWindBDCodeREGEN();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Sites"] is DBNull || string.IsNullOrEmpty((string)dr["Sites"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.code = dr["Code"] is DBNull || string.IsNullOrEmpty((string)dr["Code"]) ? 100 : Convert.ToInt32(dr["Code"]);

                        if (addUnit.code != 100)
                        {
                            addUnit.operation_mode = dr["Operation Mode"] is DBNull || string.IsNullOrEmpty((string)dr["Operation Mode"]) ? "Nil" : Convert.ToString(dr["Operation Mode"]);
                        }
                        else
                        {
                            addUnit.operation_mode = "Nil";
                        }

                        addUnit.conditions = dr["Conditions"] is DBNull || string.IsNullOrEmpty((string)dr["Conditions"]) ? "Nil" : Convert.ToString(dr["Conditions"]);

                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindBDCodeREGEN,");
                        string msg = ",Exception Occurred In Function: InsertWindBDCodeREGEN: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindBDCodeRegen", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind BD Code REGEN Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindBDCodeREGEN";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        string returnResponse = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {


                            if (returnResponse == "0")
                            {
                                //m_ErrorLog.SetInformation(",BD code REGEN API SuccessFul,");
                                m_ErrorLog.SetError("Error in deleting old data.");
                                m_ErrorLog.SetError("Error in Inserting new Data.");
                            }
                            if (returnResponse == "1")
                            {
                                //m_ErrorLog.SetInformation(",BD code REGEN API SuccessFul,");
                                m_ErrorLog.SetInformation("Deleting old data successful");
                                m_ErrorLog.SetError("Error in Inserting new Data.");
                            }
                            if (returnResponse == "2")
                            {
                                m_ErrorLog.SetInformation("Data inserted Successfully");
                            }
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",BD Code REGEN API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind BD Code REGEN Validation Failed,");
                }
            }
            return responseCode;
        }

        //InsertWindSpeed_TMD
        private async Task<int> InsertWindSpeedTMD(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            string rowOneDate = "";

            if (ds.Tables.Count > 0)
            {
                List<InsertWindSpeedTMD> addSet = new List<InsertWindSpeedTMD>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertWindSpeedTMD addUnit = new InsertWindSpeedTMD();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        if (addUnit.site_id == 0)
                        {
                            //m_ErrorLog.SetError("Site name <" + addUnit.site + "> fail to match with data in Location Master at row <" + rowNumber + ">.");
                            errorCount++;
                        }

                        objImportBatch.importSiteId = addUnit.site_id;//C
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        if (rowNumber == 2)
                        {
                            rowOneDate = addUnit.date;
                        }
                        if (rowNumber > 2)
                        {
                            if (!(rowOneDate == addUnit.date))
                            {
                                m_ErrorLog.SetError(", Date " + addUnit.date + " mismatched with " + rowOneDate + " at <" + rowNumber + "> row ");
                            }
                        }
                        bool isTimeEmpty = false;
                        if (dr["Time"] is DBNull || string.IsNullOrEmpty((string)dr["Time"]))
                        {
                            isTimeEmpty = true;
                        }
                        else
                        {
                            isTimeEmpty = false;
                        }
                        if (isTimeEmpty)
                        {
                            m_ErrorLog.SetError(", Time column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }
                        else
                        {
                            addUnit.time = Convert.ToDateTime(dr["Time"]).ToString("HH:mm:ss");
                        }

                        addUnit.wind_speed = dr["Wind Farm WindSpeed"] is DBNull || string.IsNullOrEmpty((string)dr["Wind Farm WindSpeed"]) ? 0 : Convert.ToDouble(dr["Wind Farm WindSpeed"]);

                        //errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertWindSpeedTMD,");
                        string msg = ",Exception Occurred In Function: InsertWindSpeedTMD: " + e.ToString() + ",";
                        //ErrorLog(msg);
                        LogError(user_id, 2, 4, "InsertWindSpeedTMD", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Windspeed TMD Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindSpeedTMD";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Windspeed TMD API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Windspeed TMD API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }

                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Windspeed TMD Validation Failed,");
                }
            }
            return responseCode;
        }
        //ImportWindReferenceWtgs
        private async Task<int> ImportWindReferenceWtgs(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            string rowOneDate = "";

            if (ds.Tables.Count > 0)
            {
                List<ImportWindReferenceWtgs> addSet = new List<ImportWindReferenceWtgs>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ImportWindReferenceWtgs addUnit = new ImportWindReferenceWtgs();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        if (addUnit.site == "" || addUnit.site == null)
                        {
                            m_ErrorLog.SetError(", Site column of <" + rowNumber + "> row is empty");
                            errorCount++;
                            continue;
                        }

                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));

                        if (addUnit.site_id == 0)
                        {
                            //m_ErrorLog.SetError("Site name <" + addUnit.site + "> fail to match with data in Location Master at row <" + rowNumber + ">.");
                            errorCount++;
                        }

                        objImportBatch.importSiteId = addUnit.site_id;//C


                        addUnit.wtg = dr["WTGs"] is DBNull || string.IsNullOrEmpty((string)dr["WTGs"]) ? "" : Convert.ToString(dr["WTGs"]);
                        if (addUnit.wtg != "")
                        {
                            addUnit.wtg_id = equipmentId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(equipmentId[addUnit.wtg]) : 0;
                        }

                        addUnit.ref1 = dr["Ref.1"] is DBNull || string.IsNullOrEmpty((string)dr["Ref.1"]) ? "" : Convert.ToString(dr["Ref.1"]);
                        addUnit.ref2 = dr["Ref.2"] is DBNull || string.IsNullOrEmpty((string)dr["Ref.2"]) ? "" : Convert.ToString(dr["Ref.2"]);
                        addUnit.ref3 = dr["Ref.3"] is DBNull || string.IsNullOrEmpty((string)dr["Ref.3"]) ? "" : Convert.ToString(dr["Ref.3"]);

                        //errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: ImportWindReferenceWtgs,");
                        string msg = ",Exception Occurred In Function: ImportWindReferenceWtgs: " + e.ToString() + ",";
                        ErrorLog(msg);
                        LogError(user_id, 2, 4, "ImportWindReferenceWtgs", msg);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Reference WTGs Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/ImportWindReferenceWtgs";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Reference WTGs API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Reference WTGs API Failure,: responseCode <" + (int)response.StatusCode + ">");

                            //for solar 0, wind 1, other 2;
                            int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 2);
                            if (deleteStatus == 1)
                            {
                                m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                            }
                            else if (deleteStatus == 0)
                            {
                                m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(", File not uploaded");
                            }


                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Reference WTGs Validation Failed,");
                }
            }
            return responseCode;
        }

        private void LogInfo(int userId, int import_type, int module, string api_name, string Message)
        {
            //Level = 2;
            //module ppt=0, upload=1, cal_KPI=2, email=3
            //is_frontend = 0;
            string line = "";
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/LogInfo?userId=" + userId + "&import_type=" + import_type + "&module=" + module + "&api_name=" + api_name + "&message=" + Message + "&is_frontend=0";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    line = readStream.ReadToEnd().Trim();
                    //model = JsonConvert.DeserializeObject<LoginModel>(line);
                }
            }
            //System.IO.File.AppendAllText(@"C:\LogFile\test.txt", "*Validaion Information*" + Message + "\r\n");
        }
        private void LogError(int userId, int import_type, int module, string api_name, string Message)
        {
            //Level = 1;
            //module ppt=0, upload=1, cal_KPI=2, email=3
            //is_frontend = 0;
            string line = "";
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/LogError?userId=" + userId + "&import_type=" + import_type + "&module=" + module + "&api_name=" + api_name + "&message=" + Message + "&is_frontend=0";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    line = readStream.ReadToEnd().Trim();
                    //model = JsonConvert.DeserializeObject<LoginModel>(line);
                }
            }
            //System.IO.File.AppendAllText(@"C:\LogFile\test.txt", "*Validaion Information*" + Message + "\r\n");
        }

        public string getDateFormat(string sampleDate)
        {
            if (sampleDate.Contains("/"))
            {
                if (DateTime.TryParseExact(sampleDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return "dd/MM/yyyy HH:mm:ss";
                }
                else if (DateTime.TryParseExact(sampleDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return "yyyy/MM/dd HH:mm:ss";
                }
                else
                {
                    return "Invalid Slash separated date";
                }
            }
            else if (sampleDate.Contains("-"))
            {
                return "Contains Hyphen";
            }
            else
            {
                return "Invalid date format";
            }
        }

        //public bool importTMLDateValidation(int importType, int siteID, DateTime importDate)
        //{
        //    bool retValue = false;
        //    DateTime dtToday = DateTime.Now;
        //    string currentDate = dtToday.ToString("yyyy-MM-dd");
        //    string docDate = importDate.ToString("yyyy-MM-dd");
        //    bool SameDate = false;
        //    if (currentDate == docDate)
        //    {
        //        SameDate = true;
        //    }
        //    DateTime dtImportDate = Convert.ToDateTime(importDate);
        //    DateTime CrDate = Convert.ToDateTime(currentDate);
        //    TimeSpan dayDiff = CrDate - dtImportDate;
        //    TimeSpan dayDiff2 = dtImportDate - dtToday;
        //    int dayOfWeek = (int)dtToday.DayOfWeek;


        //    //for DayOfWeek function 
        //    //if it's not true that file-date is of previous day and today is from Tuesday-Friday
        //    //&& dayOfWeek > 1 && dayOfWeek < 6
        //    if (SameDate)
        //    {
        //        if(dayDiff.Days >= 0)
        //        {
        //            if (!(dayDiff.Days >= 0 && dayDiff.Days <= 5))
        //            {
        //                if (siteUserRole == "Admin")
        //                {
        //                    m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the admin user can import it.");
        //                    retValue = true;
        //                }
        //                else
        //                {
        //                    // file date is incorrect
        //                    m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the site user cannot import it.");
        //                    retValue = false;
        //                    //retValue = true;
        //                }

        //            }
        //            else
        //            {
        //                retValue = true;
        //            }
        //        }else if (dayDiff.Days < 0)
        //        {
        //            m_ErrorLog.SetError(",The import date <" + importDate + ">  is future date so cannot import it.");
        //            retValue = false;
        //        }
        //        //if (dayDiff2.Days > 0)
        //        //{
        //        //    m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is future date so cannot import it.");
        //        //    retValue = false;
        //        //}
        //        if (retValue == false)
        //        {
        //            //if date is within 5 days
        //        }
        //    }
        //    else
        //    {
        //        m_ErrorLog.SetError(",The import date <" + importDate + ">  is of future, so cannot import this.");
        //        retValue = false;
        //    }
        //    return retValue;
        //}
        public bool importTMLDateValidationNew(int importType, int siteID, DateTime importDate)
        {
            bool retValue = false;
            DateTime dtToday = DateTime.Now;
            string currentDate = dtToday.ToString("yyyy-MM-dd");
            string docDate = importDate.ToString("yyyy-MM-dd");
            bool SameDate = true;
            if (currentDate == docDate)
            {
                SameDate = false;
            }
            DateTime CrDate = Convert.ToDateTime(currentDate);
            DateTime dtImportDate = Convert.ToDateTime(importDate);
            TimeSpan dayDiff = CrDate - dtImportDate;

            int dayOfWeek = (int)dtToday.DayOfWeek;


            //for DayOfWeek function 
            //if it's not true that file-date is of previous day and today is from Tuesday-Friday
            //&& dayOfWeek > 1 && dayOfWeek < 6
            if (SameDate)
            {
                if (dayDiff.Days < 0)
                {
                    m_ErrorLog.SetError(",The import date <" + importDate + ">  is of future, so cannot import this.");
                    retValue = true;
                }
                else
                {
                    if (!(dayDiff.Days >= 0 && dayDiff.Days <= 5))
                    {
                        if (siteUserRole == "Admin")
                        {
                            m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the admin user can import it.");
                        }
                        else
                        {
                            // file date is incorrect
                            m_ErrorLog.SetError(",The import date <" + importDate + ">  is more than 5 days older but the site user cannot import it.");
                            retValue = true;
                        }

                    }
                    if (retValue == false)
                    {
                        //if date is within 5 days
                        //Check if the data is already import and/or Approved
                    }
                }
            }
            else
            {
                m_ErrorLog.SetError(",The import date <" + importDate + ">  is of future, so cannot import this.");
                retValue = true;
            }
            return retValue;
        }
    }
}