using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationAPI.Domain.Models
{
    public class WIPFileInfo
    {
        public string FileName { get; set; }
        public string FileNameHex { get; set; }

        public DateTime LastWriteTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}