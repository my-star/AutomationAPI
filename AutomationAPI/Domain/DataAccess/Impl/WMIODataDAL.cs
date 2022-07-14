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
    public class WMIODataDAL : BaseDAL, IWMIODataDAL
    {
        public bool Delete(List<WMIOData> t)
        {
            throw new NotImplementedException();
        }

        public List<WMIOData> GetList(Hashtable param)
        {
            return Convertor.TableToList<WMIOData>(GetTable(param));
        }

        public DataTable GetTable(Hashtable param)
        {
            Hashtable ht = new Hashtable();
            ht.Add("Project", param["Project"] == null ? "ALL" : param["Project"]);
            ht.Add("PlanDateFrom", param["PlanDateFrom"] == null ? DBNull.Value : param["PlanDateFrom"]);
            ht.Add("PlanDateTo", param["PlanDateTo"] == null ? DBNull.Value : param["PlanDateTo"]);
            return ExcuteSQLList("SQL", ht, "getWMIOData").Tables[0];
        }

        public bool Import(List<WMIOData> t)
        {
            DateTime minDate = t.Min(x => x.PlanDate);
            Hashtable ht = new Hashtable();
            ht.Add("PlanDate", minDate);
            ht.Add("Project", "");
            ht.Add("FlexName", "");

            t.GroupBy(x => new { x.Project, x.FlexName }).Select(y => new { y.Key.Project,y.Key.FlexName }).ToList()
                .ForEach(pj =>
                    {
                        ht["Project"] = pj.Project;
                        ht["FlexName"] = pj.FlexName;
                        ExcuteSQLList("SQL", ht, "deleteWMIOData");

                    });

            var dt = Convertor.ToDataTable(t);
            dt.TableName = "WMIOData";
            SQLBulkInsert(dt);
            return true;
        }

        public bool Insert(List<WMIOData> t)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<WMIOData> t)
        {
            throw new NotImplementedException();
        }
    }
}