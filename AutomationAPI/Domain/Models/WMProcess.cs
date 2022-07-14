using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WMProcess
    {
        public long? Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string MPN { get; set; }
        public string ItemCode { get; set; }
        public string KeyProcess { get; set; }
        public int Level { get; set; }
        public int LT { get; set; }
        public int ProcessNo { get; set; }
        public string ProcessCode { get; set; }
        public int? OldProcessNo1 { get; set; }
        public string OldProcessCode1 { get; set; }
        public int? OldProcessNo2 { get; set; }
        public string OldProcessCode2 { get; set; }
        public int? OldProcessNo3 { get; set; }
        public string OldProcessCode3 { get; set; }
        public string Description { get; set; }
        public string ItemType { get; set; }
        public string RootPath { get; set; }
        public int? IsEnd { get; set; }
        public int QUANTITY { get; set; }
        public int KOTO { get; set; }
        public int ONHOLD { get; set; }
    }
}