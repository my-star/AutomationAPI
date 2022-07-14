using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace AutomationAPI.Common
{
    /// <summary>
    /// AS400 SQL工具类
    /// </summary>
    public static class AS400Tool
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 执行 SQL list
        /// </summary>
        /// <param name="conn">数据库连接串</param>
        /// <param name="sqlListId">SQL List ID</param>
        /// <param name="paras">输入参数</param>
        /// <returns></returns>
        public static DataSet ExeSqlList(string module, DbConnection conn, string sqlListId, Hashtable paras)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            List<SqlInfo> sqlInfoList = BuildSqlList(module, sqlListId);
            foreach (SqlInfo sqlInfo in sqlInfoList)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = (OleDbConnection)conn;
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), sqlInfo.CommandType);
                cmd.CommandText = sqlInfo.Text;
                cmd.CommandTimeout = 300;
                String paramStr ="";
                foreach (Param param in sqlInfo.ParamList)
                {
                    OleDbParameter oleDbPara = new OleDbParameter();
                    oleDbPara.ParameterName = param.Name;
                    oleDbPara.OleDbType = (OleDbType)Enum.Parse(typeof(OleDbType), param.DbType);
                    if (param.MaxLength != 0)
                    {
                        oleDbPara.Size = param.MaxLength;
                    }
                    if (param.Direction != null)
                    {
                        oleDbPara.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), param.Direction);
                    }
                    if (paras[param.Name] != null)
                    {
                        oleDbPara.Value = paras[param.Name];
                    }
                    else
                    {
                        oleDbPara.Value = param.ParamValue;
                    }
                    cmd.Parameters.Add(oleDbPara);
                    paramStr = paramStr + " " + param.Name + ":" + param.ParamValue;
                }

                _log.Info(cmd.CommandText + paramStr);

                if ("int" == sqlInfo.ReturnType)
                {
                    int exeResult = cmd.ExecuteNonQuery();
                    DataTable dataTable = new DataTable();
                    dataTable.Locale = CultureInfo.InvariantCulture;
                    dataTable.TableName = sqlInfo.TableName;
                    foreach (Param param in sqlInfo.ParamList)
                    {
                        if ("Output" == param.Direction)
                        {
                            dataTable.Columns.Add(param.Name, System.Type.GetType("System.String"));
                        }
                    }
                    dataTable.Columns.Add("exeResult", typeof(int));
                    DataRow dr = dataTable.NewRow();
                    dr["exeResult"] = exeResult;
                    foreach (Param param in sqlInfo.ParamList)
                    {
                        if ("Output" == param.Direction)
                        {
                            dr[param.Name] = cmd.Parameters[param.Name].Value;
                        }
                    }
                    dataTable.Rows.Add(dr);
                    dataSet.Tables.Add(dataTable);
                    _log.Info("exeResult:" + exeResult);
                }
                else if ("DataTable" == sqlInfo.ReturnType)
                {
                    using (OleDbDataAdapter adpt = new OleDbDataAdapter(cmd))
                    {
                        adpt.SelectCommand = cmd;
                        int exeResult = adpt.Fill(dataSet, sqlInfo.TableName);
                        _log.Info("exeResult:" + exeResult);
                    }

                }
                else if ("DataSet" == sqlInfo.ReturnType)
                {
                    using (OleDbDataAdapter adpt = new OleDbDataAdapter(cmd))
                    {
                        adpt.SelectCommand = cmd;
                        DataSet temp = new DataSet();
                        int exeResult = adpt.Fill(temp);
                        string[] tableNames = sqlInfo.TableName.Split(',');
                        for (int i = 0; i < temp.Tables.Count; i++)
                        {
                            DataTable tempTable = temp.Tables[i];
                            tempTable.TableName = tableNames[i];
                            dataSet.Tables.Add(tempTable);

                        }
                        _log.Info("exeResult:" + exeResult);
                    }

                }

            }

            return dataSet;
        }

        /// <summary>
        /// 读取xml配置SQL列表
        /// </summary>
        /// <param name="sqlListId">The SQL list identifier.</param>
        /// <returns>SQL列表</returns>
        public static List<SqlInfo> BuildSqlList(string module, string sqlListId)
        {
            XmlFile xmlFile = XmlFactory.getXmlFile(module);

            XmlNode node = xmlFile.GetOneNodeByID("Commands/SQLList", sqlListId);
            XmlNodeList sqlNodeList = XmlFile.GetChildNodeList(node, "Sql");
            List<SqlInfo> sqlInfoList = new List<SqlInfo>();
            if (sqlNodeList != null)
            {
                foreach (XmlNode sqlNode in sqlNodeList)
                {
                    SqlInfo sqlInfo = new SqlInfo();
                    sqlInfo.CommandType = XmlFile.GetNodeAttributes(sqlNode, "Type");
                    sqlInfo.TableName = XmlFile.GetNodeAttributes(sqlNode, "TableName");
                    sqlInfo.ReturnType = XmlFile.GetNodeAttributes(sqlNode, "ReturnType");
                    sqlInfo.Text = sqlNode.FirstChild.InnerText;
                    sqlInfo.ParamList.Clear();
                    sqlInfo.ParamList.AddRange(BuildParamList(sqlNode));
                    sqlInfoList.Add(sqlInfo);
                }
            }

            return sqlInfoList;

        }

        /// <summary>
        /// 读取 xml 的配置参数列表
        /// </summary>
        /// <param name="sqlNode">SQL 节点</param>
        /// <returns>参数列表</returns>
        public static List<Param> BuildParamList(XmlNode sqlNode)
        {
            XmlNodeList paramNodeList = XmlFile.GetChildNodeList(sqlNode, "Parameters/Parameter");
            List<Param> paramList = new List<Param>();
            foreach (XmlNode paramNode in paramNodeList)
            {
                Param param = new Param();
                param.DbType = XmlFile.GetChildNodeText(paramNode, "Type");
                param.Name = XmlFile.GetNodeAttributes(paramNode, "Name");
                int maxLength = 0;
                int.TryParse(XmlFile.GetChildNodeText(paramNode, "MaxLen"), out maxLength);
                param.MaxLength = maxLength;
                param.Direction = XmlFile.GetChildNodeText(paramNode, "Direction");
                param.ParamValue = XmlFile.GetChildNodeText(paramNode, "DefalultValue");
                paramList.Add(param);

            }
            return paramList;

        }
    }
}
