using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using AutomationAPI.Common;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Domain.DataAccess
{
    public class WMProjectDAL : BaseDAL, IWMProjectDAL
    {
        public bool Delete(List<WMProject> t)
        {
            throw new NotImplementedException();
        }

        public List<WMProject> GetList(Hashtable param)
        {
            return Convertor.TableToList<WMProject>(GetTable(param));
        }

        public DataTable GetTable(Hashtable param)
        {
            Hashtable ht = new Hashtable();
            ht.Add("CustProject", param["CustProject"] == null ? "ALL" : param["CustProject"]);
            ht.Add("MPN", param["MPN"] == null ? "ALL" : param["MPN"]);
            return ExcuteSQLList("SQL", ht, "getWMProject").Tables[0];
        }

        public bool Import(List<WMProject> t)
        {
            ExcuteSQLList("SQL", null, "deleteWMProject");

            var dt = Convertor.ToDataTable(t);
            dt.TableName = "WMProject";
            SQLBulkInsert(dt);
            return true;
        }

        public bool Insert(List<WMProject> t)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<WMProject> t)
        {
            throw new NotImplementedException();
        }
    }
}