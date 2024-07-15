using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Login.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly iLoginBS _loginBs;
        public LoginController(iLoginBS login)
        {
            _loginBs = login;
        }
        [Route("UserLogin")]
        [HttpGet]
       // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> UserLogin(string username, string password,bool isSSO, string device_id)
        {
           try
            {
              
                var data =await _loginBs.GetUserLogin(username, password, isSSO, device_id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                  
                return BadRequest(ex.Message);
            }
        }

        [Route("GetUserLoginFromDeviceId")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> GetUserLoginFromDeviceId(string device_id)
        {
            try
            {

                var data = await _loginBs.GetUserLoginFromDeviceId(device_id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //UpdateLoginLog
        [Route("UpdateLoginLog")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> UpdateLoginLog(int userID, string userRole)
        {
            try
            {

                var data = await _loginBs.UpdateLoginLog(userID, userRole);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateLoginStatus")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> UpdateLoginStatus(int userID)
        {
            try
            {

                var data = await _loginBs.UpdateLoginStatus(userID);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("DirectLogOut")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> DirectLogOut(int userID)
        {
            try
            {

                var data = await _loginBs.DirectLogOut(userID);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("WindUserRegistration")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> WindUserRegistration(string fname, string useremail, string role, string userpass )
        {
            try
            {

                var data = await _loginBs.WindUserRegistration(fname, useremail, role, userpass);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdatePassword")]
        [HttpGet]
        public async Task<IActionResult> UpdatePassword(int loginid, string updatepass)
        {
            try
            {
                var data = await _loginBs.UpdatePassword(loginid, updatepass);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeactivateUser")]
        [HttpGet]
        public async Task<IActionResult> DeactivateUser(int loginid)
        {
            try
            {
                var data = await _loginBs.DeactivateUser(loginid);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //ActivateUser
        [Route("ActivateUser")]
        [HttpGet]
        public async Task<IActionResult> ActivateUser(int loginid)
        {
            try
            {
                var data = await _loginBs.ActivateUser(loginid);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //DeleteUser
        [Route("DeleteUser")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int loginid)
        {
            try
            {
                var data = await _loginBs.DeleteUser(loginid);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindUserInformation")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> GetWindUserInformation(int login_id)
        {
            try
            {

                var data = await _loginBs.GetWindUserInformation(login_id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarUserInformation")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> GetSolarUserInformation(int login_id)
        {
            try
            {

                var data = await _loginBs.GetSolarUserInformation(login_id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetPageList")]
        [HttpGet]
        public async Task<IActionResult> GetPageList(int login_id, int site_type)
        {
            try
            {

                var data = await _loginBs.GetPageList(login_id, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindUserAccess")]
        [HttpGet]
        public async Task<IActionResult> GetWindUserAccess(int login_id,string role)
        {
            try
            {

                var data = await _loginBs.GetWindUserAccess(login_id, role);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetEmailAccess")]
        [HttpGet]
        public async Task<IActionResult> GetEmailAccess(int login_id, int site, int action, string notifications)
        {
            try
            {

                var data = await _loginBs.GetEmailAccess(login_id, site, action, notifications);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("SubmitUserAccess")]
        [HttpGet]
        public async Task<IActionResult> SubmitUserAccess(int login_id,string siteList,string pageList, string reportList, string site_type,int importapproval,int heatmap)
        {
            try
            {

                var data = await _loginBs.SubmitUserAccess(login_id, siteList, pageList, reportList, site_type, importapproval, heatmap);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //SubmitCloneUserAccess
        [Route("SubmitCloneUserAccess")]
        [HttpGet]
        public async Task<IActionResult> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access)
        {
            try
            {

                var data = await _loginBs.SubmitCloneUserAccess(login_id, site_type, page_type, identity, upload_access);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //GetUserLoginId
        [Route("GetUserLoginId")]
        [HttpGet]
        public async Task<IActionResult> GetUserLoginId(string username, string useremail)
        {
            try
            {

                var data = await _loginBs.GetUserLoginId(username, useremail);
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
                var data = await _loginBs.eQry(qry);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //EmailReportTimeChangeSetting
        [Route("EmailReportTimeChangeSetting")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> EmailReportTimeChangeSetting(string dailytime, string windweeklytime, string solarweeklytime, string windweekday, string solarweekday, string firstReminderTime, string secondReminderTime, string username, int user_id, string role)
        {
            try
            {

                var data = await _loginBs.EmailReportTimeChangeSetting(dailytime, windweeklytime, solarweeklytime, windweekday, solarweekday, firstReminderTime, secondReminderTime, username, user_id, role);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //EmailReportTimings
        [Route("GetEmailTime")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> EmailReportTimings()
        {
            try
            {

                var data = await _loginBs.EmailReportTimings();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomGroup")]
        [HttpGet]
        public async Task<IActionResult> GetCustomGroup(int login_id, int site_type,string groupPage)
        {
            try
            {
                var data = await _loginBs.GetCustomGroup(login_id, site_type, groupPage);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SubmitGroupBySite")]
        [HttpGet]
        public async Task<IActionResult> SubmitGroupBySite(int login_id,string reportgroup, string site_type)
        {
            try
            {
                var data = await _loginBs.SubmitGroupBySite(login_id, reportgroup, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCustomGroupAccess")]
        [HttpGet]
        public async Task<IActionResult> GetCustomGroupAccess(int login_id, int site_type)
        {
            try
            {
                var data = await _loginBs.GetCustomGroupAccess(login_id, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
		//COLUMN ACCESS CODE START

        [Route("AssignGroup")]
        [HttpGet]
        public async Task<IActionResult> AssignGroup(int login_id, string group_data)
        {
            try
            {

                var data = await _loginBs.AssignGroup(login_id, group_data);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        //COLUMN ACCESS CODE END 
    }
}
