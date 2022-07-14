using System.Collections;
using System.Collections.Generic;
using System.Data;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Service.Impl
{
    public class WMProjectService : IBaseService<WMProject>, IWMProjectService
    {
        IWMProjectDAL _dal = new WMProjectDAL();
        public bool Delete(List<WMProject> t)
        {
            return _dal.Delete(t);
        }

        public List<WMProject> GetList(Hashtable param)
        {
            return _dal.GetList(param);
        }

        public DataTable GetTable(Hashtable param)
        {
            return _dal.GetTable(param);
        }

        public bool Import(List<WMProject> t)
        {
            return _dal.Import(t);
        }

        public bool Insert(List<WMProject> t)
        {
            return _dal.Insert(t);
        }

        public bool Update(List<WMProject> t)
        {
            return _dal.Update(t);
        }
    }
}