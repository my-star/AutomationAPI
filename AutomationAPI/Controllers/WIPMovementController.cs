using AutomationAPI.Common;
using AutomationAPI.Domain.Models;
using AutomationAPI.Service;
using AutomationAPI.Service.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace AutomationAPI.Controllers
{
    public class WIPMovementController : BaseController
    {
        IWipMovementService _service = new WipMovementService();

        [HttpPost]
        [Route("api/WIP/FileUL")]
        public IHttpActionResult Upload()
        {
            string errMsg = _service.Upload(HttpContext.Current.Request.Files[0]);
            if (errMsg != "")
                return Content(HttpStatusCode.InternalServerError, errMsg);
            return Ok();
        }

        [HttpGet]
        [Route("api/WIP/WMProcess/{mpn}")]
        public IHttpActionResult GetWMProcess(string mpn)
        {
            var list = _service.GetWMProcess(mpn);
            if (list == null || list.Count == 0)
                return BadRequest();
            return Ok();
        }

        [HttpGet]
        [Route("api/WIP/WMProcessDL/{mpn}/{template?}")]
        public HttpResponseMessage WMProcessDL(string mpn, string template = null)
        {
            return Download(
                template + "Process_" + mpn + "_" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx",
                HttpContext.Current.Server.MapPath("~/Templates/WMProcess.xlsx"),
                _service.WMProcessDL(mpn, template));
        }

        [HttpGet]
        [Route("api/WIP/WMProjectDL")]
        public HttpResponseMessage WMProjectDL()
        {
            return Download(
                "Project_" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx", 
                HttpContext.Current.Server.MapPath("~/Templates/WMProject.xlsx"),
                _service.WMProjectDL());
        }

        [HttpGet]
        [Route("api/WIP/WMIODataDL")]
        public HttpResponseMessage WMProjectDL(string project, string flexname)
        {
            return Download(
                "IO_" + project + flexname + "_" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx",
                HttpContext.Current.Server.MapPath("~/Templates/WMProcess.xlsx"),
                _service.WMProjectDL(project, flexname));
        }

        [HttpGet]
        [Route("api/WIP/GetTemplate/{fileType}")]
        public HttpResponseMessage GetTemplate(int fileType)
        {
            string fileName = "";
            string template = "";
            switch (fileType)
            {
                case 1:
                    fileName = "Process_Template.xlsx";
                    template = HttpContext.Current.Server.MapPath("~/Templates/WMProcess.xlsx");
                    break;
                case 2:
                    fileName = "Project_Template.xlsx";
                    template = HttpContext.Current.Server.MapPath("~/Templates/WMProject.xlsx");
                    break;
                case 3:
                    fileName = "IO_Template.xlsx";
                    template = HttpContext.Current.Server.MapPath("~/Templates/WMIOData.xlsx");
                    break;
            }
            return Download(fileName, template);
        }

        [HttpGet]
        [Route("api/WIP/WIPMovement")]
        public HttpResponseMessage GetWIPMovement(string mpn, string debug = null)
        {
            return Download(
                "WIP Movement " + mpn + " " + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx",
                HttpContext.Current.Server.MapPath("~/Templates/WIPMovement.xlsx"),
                _service.GetWIPMovement(mpn, debug));
        }

        [HttpGet]
        [Route("api/WIP/GetMPN")]
        public IHttpActionResult GetMPN()
        {
            return Json(_service.GetMPN());
        }

        [HttpGet]
        [Route("api/WIP/GetFinishGoods")]
        public IHttpActionResult GetFinishGood()
        {
            return Json(_service.GetFinishGoods());
        }

        [HttpGet]
        [Route("api/WIP/GetProject")]
        public IHttpActionResult GetProject()
        {
            return Json(_service.GetProject());
        }

        [HttpGet]
        [Route("api/WIP/WIPMovement2")]
        public HttpResponseMessage GetWIPMovement2(string project)
        {
            return Download(
                "WIP Movement " + project + " " + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx",
                HttpContext.Current.Server.MapPath("~/Templates/WIPMovement.xlsx"),
                _service.GetWIPMovementByProject(project));
        }

        [HttpGet]
        [Route("api/WIP/WIPMovementDetail")]
        public HttpResponseMessage GetDetailByProject(string project)
        {
            return Download(
                "WIP Movement Deital " + project + " " + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx",
                HttpContext.Current.Server.MapPath("~/Templates/WIPMovwmentDetail.xlsx"),
                _service.GetDetailByProject(project));
        }
    }
}