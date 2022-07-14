using System.Collections;
using System.Collections.Generic;
using System.Data;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Service.Impl
{
    public class WMIODataService : BaseService, IWMIODataService
    {
        IWMIODataDAL _dal = new WMIODataDAL();
        public bool Delete(List<WMIOData> t)
        {
            return _dal.Delete(t);
        }

        public List<WMIOData> GetList(Hashtable param)
        {
            return _dal.GetList(param);
        }

        public DataTable GetTable(Hashtable param)
        {
            return _dal.GetTable(param);
        }

        public bool Import(List<WMIOData> t)
        {
            return _dal.Import(t);
        }

        public bool Insert(List<WMIOData> t)
        {
            return _dal.Insert(t);
        }

        public bool Update(List<WMIOData> t)
        {
            return _dal.Update(t);
        }
    }
}