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

namespace AutomationAPI.Domain.DataAccess
{
    public class CalendarDAL : BaseDAL, ICalendarDAL
    {
        public List<WipData> GetList(int fromYear, int toYear)
        {
            return Convertor.TableToList<WipData>(GetTable(fromYear, toYear));
        }
        public DataTable GetTable(int fromYear, int toYear)
        {
            Hashtable ht = new Hashtable();
            ht.Add("FromYear", fromYear);
            ht.Add("ToYear", toYear);
            return ExcuteAS400List("SQL", ht, "getCalendarDates").Tables[0];
        }
    }
}