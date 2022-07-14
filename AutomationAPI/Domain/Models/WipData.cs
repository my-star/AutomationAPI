using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WipData
    { 
        public string MPN { get; set; }
        public string FullItemCode { get; set; }
        public int Level { get; set; }
        public int ProcessNo { get; set; }
        public string ProcessCode { get; set; }
        public int Quantity { get; set; }
        public int Koto { get; set; }
        public int Onhold { get; set; }

    }
}