using AutomationAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AutomationAPI.Service
{
    public interface IWipMovementOldService
    {
        List<WIPFileInfo> GetFileName();

        void DuelWipData(string fileName);

        string Delete(string fileName);
    }
}