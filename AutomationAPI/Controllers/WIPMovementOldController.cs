using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutomationAPI.Common;
using AutomationAPI.Domain.Models;
using AutomationAPI.Service;
using AutomationAPI.Service.Impl;

namespace AutomationAPI.Controllers
{
    public class WIPMovementOldController : BaseController
    {
        IWipMovementOldService _oldWip = new WipMovementOldService();
        private static string path = HttpRuntime.AppDomainAppPath.ToString() + @"OldWIP\";

        [HttpGet]
        [Route("api/WIP/OldWIPMovement/{hexfileName}")]
        public HttpResponseMessage GetWIPMovement(string hexfileName)
        {
            string fileName = Convertor.HexToString(hexfileName);
            return Download(fileName, path + fileName);
        }

        [HttpGet]
        [Route("api/WIP/GetOldWipName")]
        public IHttpActionResult GetOldWipName()
        {
            return Json(_oldWip.GetFileName());
        }

        [HttpDelete]
        [Route("api/WIP/DeleteOldWIP/{hexfileName}")]
        public IHttpActionResult DeleteOldWIP(string hexfileName)
        {
            string fileName = Convertor.HexToString(hexfileName);
            return Ok(_oldWip.Delete(path + fileName));
        }

        [HttpPost]
        [Route("api/WIP/DuelOldWip")]
        public IHttpActionResult DuelOldWip()
        {
            CalendarHelper calendar = new CalendarHelper(DateTime.Now.Year, DateTime.Now.Year);
            if (!calendar.IsWorkDate(DateTime.Now.Date.AddDays(-1)))
                return Ok();
            foreach (var f in _oldWip.GetFileName())
            {
                _oldWip.DuelWipData(path + f.FileName);
            }
            return Ok();
        }
    }
}