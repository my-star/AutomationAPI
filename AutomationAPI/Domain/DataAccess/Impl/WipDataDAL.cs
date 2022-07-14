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
    public class WipDataDAL : BaseDAL, IWipDataDAL
    {
        public List<WipData> GetList(string mpn)
        {
            return Convertor.TableToList<WipData>(GetTable(mpn));
        }
        public DataTable GetTable(string mpn)
        {
            Hashtable ht = new Hashtable();
            ht.Add("MPN", mpn);
            return ExcuteAS400List("SQL", ht, "getWIPData").Tables[0];
        }
        public DataTable GetMPNLotSize(string mpn)
        {
            Hashtable ht = new Hashtable();
            ht.Add("MPN", mpn);
            return ExcuteAS400List("SQL", ht, "getMPNLotSize").Tables[0];
        }
        public DataTable GetInnerOnhand(string mpn)
        {
            Hashtable ht = new Hashtable();
            if (mpn.Contains(","))
            {
                ht.Add("MPNList", mpn);
                return ExcuteAS400List("SQL", ht, "getInterOnhand2").Tables[0];
            }
            ht.Add("MPN", mpn);
            return ExcuteAS400List("SQL", ht, "getInterOnhand").Tables[0];
        }
        public DataTable GetFGOnhand(string mpn)
        {
            Hashtable ht = new Hashtable();
            if (mpn.Contains(","))
            {
                ht.Add("MPNList", mpn);
                return ExcuteAS400List("SQL", ht, "getFGOnhand2").Tables[0];
            }
            ht.Add("MPN", mpn);
            return ExcuteAS400List("SQL", ht, "getFGOnhand").Tables[0];
        }
        public DataTable GetTable(Hashtable ht)
        {
            return ExcuteAS400List("SQL", ht, "getWIPData2").Tables[0];
        }
        public List<WipData> GetList(Hashtable ht)
        {
            return Convertor.TableToList<WipData>(GetTable(ht));
        }
    }
}