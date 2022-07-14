using System.Collections.Generic;

namespace AutomationAPI.Common
{
    /// <summary>
    /// SQL信息类
    /// </summary>
    public class SqlInfo
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        private string commandType;
        /// <summary>
        /// 数据表名称
        /// </summary>
        private string tableName;
        /// <summary>
        /// 执行语句
        /// </summary>
        private string text;
        /// <summary>
        /// 参数列表
        /// </summary>
        private List<Param> paramList = new List<Param>();
        /// <summary>
        /// 返回类型
        /// </summary>
        private string returnType;

        public string CommandType
        {
            set { commandType = value; }
            get { return commandType; }
        }

        public string TableName
        {
            set { tableName = value; }
            get { return tableName; }
        }

        public string Text
        {
            set { text = value; }
            get { return text; }
        }

        public List<Param> ParamList
        {
            get { return paramList; }
        }

        public string ReturnType
        {
            set { returnType = value; }
            get { return returnType; }
        }
    }
}
