using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class Calendar
    {
        public DateTime Date { get; set; }
        public int WorkSeq { get; set; }
        public bool IsWorkDay { get; set; }

    }
}