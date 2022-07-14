using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;

namespace AutomationAPI.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            _log.Error(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}