using AutomationAPI.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Service
{
    public interface IWMProcessService:IBaseService<WMProcess>
    {
        List<WMProcess> GetTemplateList(Hashtable param);
    }
}