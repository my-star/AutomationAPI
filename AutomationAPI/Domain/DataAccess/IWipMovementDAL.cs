using AutomationAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.DataAccess
{
    public interface IWipMovementDAL
    {
        DataTable GetMPN();
        DataTable GetFinishGoods();
    }
}