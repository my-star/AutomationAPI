using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationAPI.Common
{
    public class XmlFactory
    {
        public static Hashtable table = new Hashtable();

        public static XmlFile getXmlFile(string module)
        {
            if (table.Contains(module))
            {
                return (XmlFile)table[module];
            }
            else
            {
                XmlFile xmlFile = new XmlFile(module);
                table.Add(module, xmlFile);
                return xmlFile;
            }
        }
    }
}
