using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace DGRA_V1.Controllers
{

    // [Authorize]
    [AllowAnonymous]
    public class ColunmAccessController : Controller
    {
        private IDapperRepository _idapperRepo;
        public ColunmAccessController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        public async Task<IActionResult> GetPageList(string type, string pageType )
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
    }
}
