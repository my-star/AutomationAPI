using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Common
{
    public class CalendarHelper
    {
        ICalendarDAL _dal = new CalendarDAL();
        private List<Calendar> _cal = null;
        public CalendarHelper(int fromYear, int toYear)
        {
            _cal = new List<Calendar>();
            var dt = _dal.GetTable(fromYear, toYear);
            int m = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int year = int.Parse(dt.Rows[i]["MBDATE"].ToString());
                for (int month = 1; month <= 12; month++)
                {
                    var flags = dt.Rows[i]["MBMM" + month.ToString("D2")].ToString().Replace("Y", "").ToCharArray();
                    int day = 1;
                    foreach (var f in flags)
                        _cal.Add(new Calendar { Date = new DateTime(year, month, day++), WorkSeq = f == '-' ? m++ : m, IsWorkDay = f == '-' });
                }
            }
        }

        public DateTime GetWorkDate(int days)
        {
            var seq = _cal.Where(c => c.Date == DateTime.Now.Date).Select(c => c.WorkSeq).FirstOrDefault();
            return _cal.Where(c => c.WorkSeq == seq + days && c.IsWorkDay == true).Select(c => c.Date).FirstOrDefault();
        }
        public bool IsWorkDate(DateTime date)
        {
            return _cal.Any(c => c.Date == date && c.IsWorkDay == true);
        }
    }
}