#region using
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
#endregion

namespace Joy.Common.Data
{
    public static class DbHelper
    {
        public static string SQLPARAMETERPREFIX = "";
        public static string BuildSqlStringParameterName(this Database db, string name)
        {
            string pname = name.Replace(".", "_");
            return DbHelper.SQLPARAMETERPREFIX + db.BuildParameterName(pname);
        }
        public static Type Convert2Type(this DbType type)
        {
            switch (type)
            {
                case DbType.Boolean: return typeof(bool);
                case DbType.Binary:
                case DbType.Byte: return typeof(byte);
                case DbType.Decimal:
                case DbType.Currency: return typeof(decimal);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset: return typeof(DateTime);
                case DbType.Double: return typeof(double);
                case DbType.Int16: return typeof(int);
                case DbType.Int32: return typeof(Int32);
                case DbType.Int64: return typeof(Int64);
                case DbType.Single: return typeof(Single);
                case DbType.UInt16: return typeof(UInt16);
                case DbType.UInt32: return typeof(UInt32);
                case DbType.UInt64: return typeof(UInt64);
                case DbType.VarNumeric: return typeof(decimal);
                case DbType.Time: return typeof(TimeSpan);
                case DbType.AnsiString:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Guid:
                case DbType.Xml:
                case DbType.AnsiStringFixedLength: return typeof(string);
                default: return typeof(string);
            }
        }
        public static DbType Convert2DbType(this Type type)
        {
            Dictionary<Type,DbType> typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            if (!typeMap.ContainsKey(type)) throw new System.Exception("Cant convert");
            else return typeMap[type];
        }

        public static DataTable ExecuteDataTable(Database db, DbTransaction trans, string sql, params string[] orderColumns)
        {
            return ExecuteDataTable(db, trans, sql, new PageObject(), new DbFilter[] { }, orderColumns);
        }
        public static DataTable ExecuteDataTable(Database db, DbTransaction trans,string table,DbColumn[] columns,params string[] orderColumns)
        {
            return ExecuteDataTable(db, trans, table, new PageObject(),columns, new DbFilter[] { }, orderColumns);
        }
        public static DataTable ExecuteDataTable(Database db, DbTransaction trans, string table,DbColumn[] columns,DbFilter[] filters, params string[] orderColumns)
        {
            return ExecuteDataTable(db, trans, table, new PageObject(), columns, filters, orderColumns);
        }
        public static DataTable ExecuteDataTable(Database db, DbTransaction trans, string table, PageObject page, DbColumn[] columns, DbFilter[] filters, params string[] orderColumns)
        {
            string execSql = string.Format("select {0} from {1}[...][---]", columns.BuildSqlColumn(), table);
            return ExecuteDataTable(db, trans, execSql, page, filters, orderColumns);
        }
        public static DataTable ExecuteDataTable(Database db, DbTransaction trans, string sql, DbFilter[] filters, params string[] orderColumns)
        {
            return ExecuteDataTable(db, trans, sql, new PageObject(), filters, orderColumns);
        }
        public static DataTable ExecuteDataTable(Database db, DbTransaction trans, string sql, PageObject page, DbFilter[] filters, params string[] orderColumns)
        {
            DbCommand cmd = db.GetSqlStringCommand(sql);
            filters.Fill2Command(db, cmd);
            string orderStr = (orderColumns != null && orderColumns.Length > 0) ? (" order by " + string.Join(",", orderColumns)) : string.Empty;
            string execSql = cmd.CommandText.Contains("[---]") ? cmd.CommandText.Replace("[---]", orderStr) : (cmd.CommandText + orderStr);
            cmd.CommandText = execSql;
            using (IDataReader rd = (trans == null) ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, trans))
            {
                DataTable dt = new DataTable();
                #region init Column
                for (int i = 0; i < rd.FieldCount; i++)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = rd.GetName(i);
                    col.DataType = rd.GetFieldType(i);
                    dt.Columns.Add(col);
                }
                #endregion
                int count = 0;
                bool hasPage=((page!=null) && (page.PageSize>0) && (page.PageIndex>0));
                int first =hasPage? (page.PageSize * (page.PageIndex - 1))+1: 0;
                int last = hasPage ? (first + page.PageSize - 1) : 0;
                #region Fill Datas
                while (rd.Read())
                {
                    count++;
                    if (hasPage && count < first) continue;
                    if (hasPage && count > last) break;
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < rd.FieldCount; i++) row[i] = rd[i];
                    dt.Rows.Add(row);
                }
                #endregion
                return dt;
            }
        }

        public static IDictionary ExecuteSingle(Database db, DbTransaction trans, string table, DbColumn[] columns,params string[] orderColumns)
        {
            return ExecuteSingle(db, trans, table, columns, new DbFilter[] { }, orderColumns);
        }
        public static IDictionary ExecuteSingle(Database db, DbTransaction trans, string sql, params string[] orderColumns)
        {
            return ExecuteSingle(db, trans, sql, new DbFilter[] { }, orderColumns);
        }
        
        public static IDictionary ExecuteSingle(Database db, DbTransaction trans, string table, DbColumn[] columns, DbFilter[] filters, params string[] orderColumns)
        {
            string execSql = string.Format("select {0} from {1}[...][---]", columns.BuildSqlColumn(), table);
            return ExecuteSingle(db, trans, execSql, filters, orderColumns);
        }
        public static IDictionary ExecuteSingle(Database db, DbTransaction trans, string sql, DbFilter[] filters, params string[] orderColumns)
        {
            DbCommand cmd = db.GetSqlStringCommand(sql);
            filters.Fill2Command(db, cmd);
            string orderStr = (orderColumns != null && orderColumns.Length > 0) ? (" order by " + string.Join(",", orderColumns)) : string.Empty;
            string execSql = cmd.CommandText.Contains("[---]") ? cmd.CommandText.Replace("[---]", orderStr) : (cmd.CommandText + orderStr);
            cmd.CommandText = execSql;
            using (IDataReader rd = (trans == null) ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, trans))
            {
                IDictionary dict = new Dictionary<string, object>();
                int count = 0;
                while (rd.Read())
                {
                    count++;
                    for (int i = 0; i < rd.FieldCount; i++) dict.Add(rd.GetName(i).ToUpper(), rd.IsDBNull(i) ? null : rd[i]);
                    break;
                }
                if (count == 0) return null;
                return dict;
            }
        }
        
        public static int ExecuteScalar(Database db, DbTransaction trans, string sql)
        {
            return ExecuteScalar(db, trans, sql, new DbFilter[] { });
        }
        public static int ExecuteScalar(Database db, DbTransaction trans, string sql, DbFilter[] filters)
        {
            DbCommand cmd = db.GetSqlStringCommand(sql);
            filters.Fill2Command(db, cmd);
            object dt =(trans==null)?db.ExecuteScalar(cmd):db.ExecuteScalar(cmd,trans);
            try
            {
                return int.Parse(dt.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static int ExecuteInsert(Database db, DbTransaction trans, string table, DbColumn[] columns)
        {
            DbCommand cmd = db.GetSqlStringCommand("insert");
            columns.Fill2InsertCommand(db, cmd, table);
            return (trans==null)?db.ExecuteNonQuery(cmd):db.ExecuteNonQuery(cmd,trans);
        }

        public static int ExecuteDelete(Database db, DbTransaction trans, string table, DbFilter[] filters)
        {
            string exeSql = string.Format("delete from {0}[...]", table);
            DbCommand cmd = db.GetSqlStringCommand(exeSql);
            filters.Fill2Command(db, cmd);
            return (trans==null)?db.ExecuteNonQuery(cmd):db.ExecuteNonQuery(cmd,trans);
        }

        public static int ExecuteUpdate(Database db, DbTransaction trans, string table, DbColumn[] columns, DbFilter[] filters)
        {
            DbCommand cmd = db.GetSqlStringCommand("update");
            columns.Fill2UpdateCommand(db, cmd, table);
            filters.Fill2Command(db, cmd);
            return (trans == null) ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, trans);
        }

        public static int ExecuteUpdate(Database db, DbTransaction trans, string sql, DbFilter[] filters,params object[] datas)
        {
            DbCommand cmd = db.GetSqlStringCommand(sql);
            if (datas != null) 
            {
                for (int i = 0; i < datas.Length;i++ )
                {
                    string pname = BuildSqlStringParameterName(db,string.Format("vx_{0}", i));
                    string strInSql=string.Format("[[{0}]]", i);
                    if (!sql.Contains(strInSql)) continue;
                    sql = sql.Replace(strInSql, pname);
                    db.AddInParameter(cmd, pname, datas[i].GetType().Convert2DbType(), datas[i]);
                }
                cmd.CommandText = sql;
            }
            filters.Fill2Command(db, cmd);
            return (trans == null) ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, trans);
        }

    
    }

}

