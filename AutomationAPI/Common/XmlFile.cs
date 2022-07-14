using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace AutomationAPI.Common
{
    public class XmlFile
    {
        private XmlDocument _xmlData;

        public XmlFile(string module)
        {
            try
            {
                string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);//MektecPort.Common.dll
                string xmlFileName = Path.Combine(assemblyFolder, module + "\\SQLList.xml");

                _xmlData = new XmlDocument();
                _xmlData.Load(xmlFileName);
            }
            catch (Exception e)
            {
                _xmlData = null;
                throw e;
            }
        }

        public XmlNode GetOneNodeByID(string xPath, string id)
        {
            try
            {
                XmlNode theNode = null;

                XmlNodeList nodeList = _xmlData.SelectNodes(xPath);
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["Id"].InnerText == id)
                    {
                        theNode = node;
                    }
                }
                return theNode;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static XmlNodeList GetChildNodeList(XmlNode node, string xPath)
        {
            try
            {
                return node.SelectNodes(xPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GetNodeAttributes(XmlNode node, string attributeName)
        {
            try
            {

                XmlAttribute attribute = node.Attributes[attributeName];
                if (attribute != null)
                    return attribute.InnerText;
                else
                    return "";

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GetChildNodeText(XmlNode node, string xPath)
        {
            try
            {
                XmlNode childNode = node.SelectSingleNode(xPath);
                if (childNode != null)
                    return childNode.InnerText;
                else
                    return null;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
