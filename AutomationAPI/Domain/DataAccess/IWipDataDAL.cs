using AutomationAPI.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.DataAccess
{
    public interface IWipDataDAL
    {
        DataTable GetTable(string mpn);
        DataTable GetTable(Hashtable ht);
        List<WipData> GetList(string mpn);
        List<WipData> GetList(Hashtable ht);

        DataTable GetMPNLotSize(string mpn);
        DataTable GetInnerOnhand(string mpn);
        DataTable GetFGOnhand(string mpn);
    }
}