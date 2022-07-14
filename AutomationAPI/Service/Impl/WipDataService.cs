using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using AutomationAPI.Common;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Service.Impl
{
    public class WipDataService : BaseService, IWipDataService
    {
        IWipDataDAL _dal = new WipDataDAL();

        public List<WipData> GetList(string mpn)
        {
            return _dal.GetList(mpn);
        }
        public DataTable GetTable(string mpn)
        {
            return _dal.GetTable(mpn);
        }
        public int GetInnerOnhand(string mpn)
        {
            var tbl = _dal.GetInnerOnhand(mpn);
            if (tbl == null || tbl.Rows.Count == 0)
                return 0;
            if (decimal.TryParse(tbl.Rows[0][0].ToString(), out decimal qty))
                return (int)qty;
            return 0;
        }
        public int GetFGOnhand(string mpn)
        {
            var tbl = _dal.GetFGOnhand(mpn);
            if (tbl == null || tbl.Rows.Count == 0)
                return 0;
            if (decimal.TryParse(tbl.Rows[0][0].ToString(), out decimal qty))
                return (int)qty;
            return 0;
        }
        public List<WipData> GetList(Hashtable ht)
        {
            return _dal.GetList(ht);
        }
    }
}