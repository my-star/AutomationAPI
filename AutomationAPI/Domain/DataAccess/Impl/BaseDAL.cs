using AutomationAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.Configuration;

namespace AutomationAPI.Domain.DataAccess.Impl
{

    public class BaseDAL
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string connStringMSSQL => WebConfigurationManager.ConnectionStrings["MSSQL.Automation"].ConnectionString.ToString();
        private string connStringAS400 => WebConfigurationManager.ConnectionStrings["AS400.MSRFLIB"].ConnectionString.ToString();

        /// <summary>
        /// xml文件中的SQL List 执行
        /// </summary>
        /// <param name="connString">连接串</param>
        /// <param name="paras">SQL参数</param>
        /// <param name="sqlId">SQL Id</param>
        /// <returns>数据Tables</returns>
        public DataSet ExcuteSQLList(string module, Hashtable paras, string sqlId)
        {
            SqlConnection conn = new SqlConnection();

            try
            {
                conn.ConnectionString = connStringMSSQL;
                conn.Open();
                return SqlTool.ExeSqlList(module, (DbConnection)conn, sqlId, paras);
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataSet ExcuteAS400List(string module, Hashtable paras, string sqlId)
        {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = connStringAS400;
            try
            {
                conn.Open();
                return AS400Tool.ExeSqlList(module, (DbConnection)conn, sqlId, paras);
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="connString">连接串</param>
        /// <param name="data">数据Table</param>
        public void SQLBulkInsert(DataTable data)
        {
            SqlConnection conn = new SqlConnection();

            try
            {
                conn.ConnectionString = connStringMSSQL;
                conn.Open();
                var columnList = GetFileds(conn, data.TableName);
                SqlBulkCopy sqlBulk = new SqlBulkCopy(conn);
                sqlBulk.DestinationTableName = data.TableName;
                foreach (DataColumn column in data.Columns)
                {
                    if (columnList.Contains(column.ColumnName))
                        sqlBulk.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                sqlBulk.WriteToServer(data);
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public List<string> GetFileds(SqlConnection conn, string tableName)
        {
            List<string> columnList = new List<string>();

            string[] restrictionValues = new string[4];
            restrictionValues[0] = null; // Catalog
            restrictionValues[1] = null; // Owner
            restrictionValues[2] = tableName; // Table
            restrictionValues[3] = null; // Column

            using (DataTable dt = conn.GetSchema(SqlClientMetaDataCollectionNames.Columns, restrictionValues))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    columnList.Add(dr["column_name"].ToString());
                }
            }
           
            return columnList;
        }
    }
}