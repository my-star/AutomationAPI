using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WipMovement
    {
        public string Project { get; set; }
        public string Process { get; set; }
        public int LeadTime { get; set; }
        public decimal Yield { get; set; }
        public DateTime OutputDate { get; set; }
        public int? WIPPNL { get; set; }
        public int WIPPCS { get; set; }
        public int? OnholdQty { get; set; }
        public string WIPOutputYield { get; set; }
        public string WIPoutputCum { get; set; }
        public int PlanOutput { get; set; }
        public string PlanOutputCum { get; set; }
        public string CumDelta { get; set; }
        public string WIPOutput { get; set; }
        public string Item { get; set; }
        public string MPN { get; set; }
        public int KOTO { get; set; }
        public string ITEMTYPE { get; set; }
    }
}