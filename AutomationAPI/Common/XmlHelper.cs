using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutomationAPI.Common
{
    public class XmlHelper
    {
        private static XmlDocument _xmlData;

        /// <summary>
        /// 读取Xml文件SQLList.xml
        /// </summary>
        public static void InitXmlData()
        {
            if (_xmlData == null)
            {
                try
                {
                    string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);//MektecPort.Common.dll
                    string xmlFileName = Path.Combine(assemblyFolder, "SQLList.xml");

                    _xmlData = new XmlDocument();
                    _xmlData.Load(xmlFileName);
                }
                catch (Exception e)
                {
                    _xmlData = null;
                    throw e;
                }
            }
        }

        /// <summary>
        /// 获取指定的xml节点
        /// </summary>
        /// <param name="xPath">节点路径</param>
        /// <returns>指定的xml节点</returns>
        public static XmlNode GetOneNode(string xPath)
        {
            try
            {
                return _xmlData.SelectSingleNode(xPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取指定的xml节点List
        /// </summary>
        /// <param name="xPath">节点路径</param>
        /// <returns>xml节点List</returns>
        public static XmlNodeList GetNodeList(string xPath)
        {
            try
            {
                return _xmlData.SelectNodes(xPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取节点的子节点List
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="xPath">子节点路径</param>
        /// <returns>子节点List</returns>
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

        /// <summary>
        /// 获取节点的子节点
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="xPath">子节点路径</param>
        /// <returns>子节点</returns>
        public static XmlNode GetChildNode(XmlNode node, string xPath)
        {
            try
            {
                return node.SelectSingleNode(xPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取指定的子节点Text
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="xPath">子节点路径</param>
        /// <returns>子节点Text</returns>
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

        /// <summary>
        /// 获取指定的节点属性
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="xPath">属性名称</param>
        /// <returns>节点属性</returns>
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


        /// <summary>
        /// 获取指定的xml节点
        /// </summary>
        /// <param name="xPath">节点路径</param>
        ///<param name="id">节点ID</param>
        /// <returns>xml节点</returns>
        public static XmlNode GetOneNodeByID(string xPath, string id)
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
    }
}
