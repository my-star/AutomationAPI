using AutomationAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Service
{
    public interface IWipMovementService
    {
        DataTable GetMPN();
        DataTable GetFinishGoods();
        string Upload(HttpPostedFile file);
        DataTable WMProjectDL();
        List<WMProcess> GetWMProcess(string mpn);
        DataSet WMProcessDL(string mpn, string template);
        DataTable WMProjectDL(string project, string flexname);
        DataTable GetWIPMovement(string mpn, string debug);
        List<string> GetProject();
        DataTable GetWIPMovementByProject(string project);
        DataTable GetDetailByProject(string project);
    }
}