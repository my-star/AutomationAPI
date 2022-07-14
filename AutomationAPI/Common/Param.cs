using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAPI.Common
{
    /// <summary>
    /// SQL 参数类
    /// </summary>
    public class Param
    {
        /// <summary>
        /// 参数名
        /// </summary>
        private string name;
        /// <summary>
        /// 参数值
        /// </summary>
        private object paramValue;
        /// <summary>
        /// 参数最大长度
        /// </summary>
        private int maxLength;
        /// <summary>
        /// 参数类型
        /// </summary>
        private string direction;
        /// <summary>
        /// 数据库类型
        /// </summary>
        private string dbType;


        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public object ParamValue
        {
            set { paramValue = value; }
            get { return paramValue; }
        }

        public int MaxLength
        {
            set { maxLength = value; }
            get { return maxLength; }

        }

        public string Direction
        {
            set { direction = value; }
            get { return direction; }
        }

        public string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }
    }
}
