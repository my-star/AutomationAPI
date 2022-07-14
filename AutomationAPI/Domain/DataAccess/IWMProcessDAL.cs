using AutomationAPI.Domain.Models;
using System.Collections;
using System.Collections.Generic;

namespace AutomationAPI.Domain.DataAccess
{
    public interface IWMProcessDAL:IBaseDAL<WMProcess>
    {
        List<WMProcess> GetTemplateList(Hashtable param);
    }
}