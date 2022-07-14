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
    public class WipMovementDAL : BaseDAL, IWipMovementDAL
    {
        public DataTable GetMPN()
        {
            return ExcuteSQLList("SQL", null, "getMPN").Tables[0];
        }

        public DataTable GetFinishGoods()
        {
            return ExcuteSQLList("SQL", null, "getFinishGoods").Tables[0];
        }
    }
}