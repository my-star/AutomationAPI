using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WMProject
    { 
        public long? Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CustProject { get; set; }
        public string InterProject { get; set; }
        public string FPCItem { get; set; }
        public string SMTItem { get; set; }
        public string Customer { get; set; }
        public string Color { get; set; }
        public string MPN { get; set; }
    }
}