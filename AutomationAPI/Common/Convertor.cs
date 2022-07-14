using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace AutomationAPI.Common
{
    public class Convertor
    {
        public static List<T> TableToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            Type info = typeof(T);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T t = new T();
                foreach (var mi in info.GetMembers())
                {
                    if (mi.MemberType != MemberTypes.Property || !dt.Columns.Contains(mi.Name))
                        continue;
                    var p = info.GetProperty(mi.Name);
                    var obj = dt.Rows[i][mi.Name];
                    if (obj != DBNull.Value)
                    {
                        var type = Convert.ChangeType(obj, Nullable.GetUnderlyingType(p.PropertyType) == null
                            ? p.PropertyType 
                            : p.PropertyType.GetGenericArguments()[0]);
                        p.SetValue(t, type, null);
                    }
                        
                }
                list.Add(t);
            }
            return list;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            var rng = props.Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) == null ? p.PropertyType : p.PropertyType.GetGenericArguments()[0])).ToArray();
            dt.Columns.AddRange(rng);
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        public static int? ToIntNull(object obj)
        {
            int result;
            if (int.TryParse(obj.ToString().Trim(), out result))
                return result;
            else
                return null;
        }
        public static int ToInt(object obj)
        {
            int result;
            if (int.TryParse(obj.ToString().Trim(), out result))
                return result;
            else
                return 0;
        }
        public static decimal ToDecimal(object obj)
        {
            decimal result;
            if (decimal.TryParse(obj.ToString().Trim(), out result))
                return result;
            else
                return 0;
        }
        public static string ToHex(string str)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
        public static string HexToString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }
    }
}