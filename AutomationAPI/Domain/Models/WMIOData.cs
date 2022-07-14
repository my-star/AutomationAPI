using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WMIOData
    { 
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime PlanDate { get; set; }
        public string Item { get; set; }
        public string Project { get; set; }
        public string FlexName { get; set; }
        public string DRI { get; set; }
        public decimal Quantity { get; set; }
    }
}