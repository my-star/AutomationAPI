using AutomationAPI.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.DataAccess
{
    public interface ICalendarDAL
    {
        DataTable GetTable(int fromYear, int toYear);
        List<WipData> GetList(int fromYear, int toYear);
    }
}