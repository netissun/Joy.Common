using Joy.Common.Meta;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Joy.Common.Data
{
    public class DbColumn
    {
        public string ColumnName { get; set; }
        public DbType DbType { get; set; }
        public object Value { get; set; }
        public DbColumn(string columnName)
            : this(columnName, DbType.String, string.Empty)
        {
        }
        public DbColumn(string columnName, string value)
            : this(columnName, DbType.String, value)
        {
        }
        public DbColumn(string columnName, DbType dbType, object value)
        {
            this.ColumnName = columnName;
            this.DbType = dbType;
            this.Value = (value != null) ? ((!value.GetType().Equals(dbType.Convert2Type())) ? Convert.ChangeType(value, dbType.Convert2Type()) : value) : null;
        }
    }

    public class DbColumnCollection : ICollection<DbColumn>
    {
        private ConcurrentDictionary<string, DbColumn> cols = new ConcurrentDictionary<string, DbColumn>();
        public void Add(DbColumn item)
        {
            cols.TryAdd(item.ColumnName, item);
        }
        public DbColumnCollection Add(string columnName, DbType dbType, object value)
        {
            this.Add(new DbColumn(columnName, dbType, value));
            return this;
        }
        public DbColumnCollection Add(string columnName, string value)
        {
            this.Add(new DbColumn(columnName, value));
            return this;
        }
        public DbColumnCollection Add(string columnName)
        {
            this.Add(new DbColumn(columnName));
            return this;
        }
        public void Clear()
        {
            cols.Clear();
        }
        public bool Contains(DbColumn item)
        {
            return this.cols.ContainsKey(item.ColumnName);
        }
        public void CopyTo(DbColumn[] array, int arrayIndex)
        {
            if (arrayIndex > (array.Length -1 )) return;
            IList<DbColumn> ncols=this.cols.Values.ToList<DbColumn>();
            for (var i = 0; (i <= (array.Length - arrayIndex) && i < ncols.Count()); i++) array[i + arrayIndex] = ncols[i];
        }
        public int Count
        {
            get { return this.cols.Count();  }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(DbColumn item)
        {
            DbColumn it = null;
            return this.cols.TryRemove(item.ColumnName, out it);
        }
        public IEnumerator<DbColumn> GetEnumerator()
        {
            return this.cols.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cols.GetEnumerator();
        }
    }

    public static class DbColumnHelper
    {
        public static string BuildSqlColumn(this DbColumn[] columns)
        {
            return string.Join(",", columns.Select(z => z.ColumnName));
        }
        public static DbColumn[] Fill2UpdateCommand(this DbColumn[] columns, Database db, DbCommand cmd,string table)
        {
            string sql = string.Empty;
            for (int idx = 0; idx < columns.Length; idx++)
            {
                DbColumn c=columns[idx];
                string param = db.BuildSqlStringParameterName(string.Format("q_{0}", idx));
                db.AddInParameter(cmd, param, c.DbType, c.Value);
                sql+=string.Format(",{0}={1}",c.ColumnName,param);
            }
            cmd.CommandText = string.Format("update {0} set {1}", table, sql.Substring(1));
            return columns;
        }
        public static DbColumn[] Fill2InsertCommand(this DbColumn[] columns, Database db, DbCommand cmd, string table)
        {
            string csql = string.Empty;
            string vsql = string.Empty;
            for (int idx = 0; idx < columns.Length; idx++)
            {
                DbColumn c = columns[idx];
                string param = db.BuildSqlStringParameterName(string.Format("q_{0}", idx));
                db.AddInParameter(cmd, param, columns[idx].DbType, columns[idx].Value);
                csql += string.Format(",{0}", c.ColumnName);
                vsql += string.Format(",{0}", param);
            }
            cmd.CommandText = string.Format("insert into {0} ({1}) values ({2})", table, csql.Substring(1), vsql.Substring(1));
            return columns;
        }
        public static DbColumn[] ToDbColumnArray(this string columns)
        {
            string[] cols = columns.Split(',');
            DbColumn[] cs = new DbColumn[cols.Length];
            for (int i = 0; i < cols.Length; i++) cs[i] = new DbColumn(cols[i]);
            return cs;
        }
        public static DbColumn[] ToDbColumnArray(this KeyValue[] keyValues)
        {
            IList<DbColumn> cols = new List<DbColumn>();
            foreach (KeyValue kv in keyValues) cols.Add(new DbColumn(kv.Key, kv.Value??""));
            return cols.ToArray<DbColumn>();
        }
        public static DbColumn ToDbColumn(this KeyValue keyValue, DbType dbType=DbType.String)
        {
            return new DbColumn(keyValue.Key, dbType, keyValue.Value);
        }
        public static DbColumn[] ToDbColumnArray(this object obj)
        {
            List<DbColumn> columns = new List<DbColumn>();
            Type type = obj.GetType();
            PropertyInfo[] pros = type.GetProperties();
            foreach (PropertyInfo p in pros)
            {
                try
                {
                    DbType dbType = p.PropertyType.Convert2DbType();
                    columns.Add(new DbColumn(p.Name, dbType, p.GetValue(obj)));
                }
                catch
                {
                    continue;
                }
            }
            return columns.ToArray<DbColumn>();
        }
    }



}
