using AutomationAPI.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Service
{
    public interface IWipDataService
    {
        DataTable GetTable(string mpn);
        List<WipData> GetList(string mpn);
        List<WipData> GetList(Hashtable ht);
        int GetInnerOnhand(string mpn);
        int GetFGOnhand(string mpn);
    }
}