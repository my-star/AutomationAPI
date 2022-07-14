using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Service
{
    public interface IBaseService<T> where T : class
    {
        bool Import(List<T> t);
        bool Delete(List<T> t);
        bool Insert(List<T> t);
        bool Update(List<T> t);
        List<T> GetList(Hashtable ht);
        DataTable GetTable(Hashtable ht);
    }
}