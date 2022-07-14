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
    public class WMProcessService : BaseService,IWMProcessService
    {
        IWMProcessDAL _dal = new WMProcessDAL();
        public bool Delete(List<WMProcess> t)
        {
            return _dal.Delete(t);
        }

        public List<WMProcess> GetList(Hashtable param)
        {
            return _dal.GetList(param);
        }

        public DataTable GetTable(Hashtable param)
        {
            return _dal.GetTable(param);
        }

        public bool Import(List<WMProcess> t)
        {
            return _dal.Import(t);
        }

        public bool Insert(List<WMProcess> t)
        {
            return _dal.Insert(t);
        }

        public bool Update(List<WMProcess> t)
        {
            return Update(t);
        }
        public List<WMProcess> GetTemplateList(Hashtable param)
        {
            return _dal.GetTemplateList(param);
        }
    }
}