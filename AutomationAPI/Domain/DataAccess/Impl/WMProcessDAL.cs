using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutomationAPI.Common;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Service.Impl
{
    public class WMProcessDAL : BaseDAL,IWMProcessDAL
    {
        public bool Delete(List<WMProcess> t)
        {
            throw new NotImplementedException();
        }

        public List<WMProcess> GetList(Hashtable param)
        {
            return Convertor.TableToList<WMProcess>(GetTable(param));
        }

        public DataTable GetTable(Hashtable param)
        {
            if (param.ContainsKey("MPNList"))
                return ExcuteSQLList("SQL", param, "getWMProcess2").Tables[0];
            Hashtable ht = new Hashtable();
            ht.Add("MPN", param["MPN"] == null ? "" : param["MPN"]);
            ht.Add("HMCD", param["HMCD"] == null ? "" : param["HMCD"]);
            return ExcuteSQLList("SQL", ht, "getWMProcess").Tables[0];
        }

        public bool Import(List<WMProcess> t)
        {
            Hashtable ht = new Hashtable();
            ht.Add("RootPath", "");
            List<string> mpnList = t.GroupBy(x => x.RootPath.Split('/')[0]).Select(y => y.Key).ToList();
            mpnList.ForEach(rootPath =>
            {
                ht["RootPath"] = rootPath.Split('/')[0];
                ExcuteSQLList("SQL", ht, "deleteWMProcess");

                var dt = Convertor.ToDataTable(t.Where(x => x.RootPath.Split('/')[0] == rootPath));
                dt.TableName = "WMProcess";
                SQLBulkInsert(dt);
            });
            return true;
        }

        public bool Insert(List<WMProcess> t)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<WMProcess> t)
        {
            throw new NotImplementedException();
        }
        public List<WMProcess> GetTemplateList(Hashtable param)
        {
            return Convertor.TableToList<WMProcess>(GetTemplateTable(param));
        }

        public DataTable GetTemplateTable(Hashtable param)
        {
            Hashtable ht = new Hashtable();
            ht.Add("MPN", param["MPN"] == null ? "ALL" : param["MPN"]);
            ht.Add("HMCD", param["HMCD"] == null ? "ALL" : param["HMCD"]);
            return ExcuteAS400List("SQL", ht, "getTemplateWMProcess").Tables[0];
        }
    }
}