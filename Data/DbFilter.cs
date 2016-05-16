using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Joy.Common.Data
{

    public enum FilterType
    {
        Equals,
        NotEquals,
        MoreThan,
        MoreThanEquals,
        LessThan,
        LessThanEquals,
        Include,
        Exclude,
        FullRange,
        LeftRange,
        RightRange,
        IsNull,
        IsNotNull,
        In,
        NotIn,
        BeginWith,
        EndWith
    }
    public class DbFilter
    {
        public string ColumnName { get; set; }
        public FilterType FilterType { get; set; }
        public DbType DbType { get; set; }
        public object[] Value { get; set; }
        public bool WithValue { get; set; }
        public DbFilter(string columnName, FilterType type, DbType dbType, object[] value, bool withValue=false)
        {
            this.ColumnName = columnName;
            this.FilterType = type;
            this.DbType = dbType;
            this.Value = value;
            this.WithValue = withValue;
        }
        public DbFilter(string columnName, string value)
            :this(columnName,FilterType.Equals,DbType.String,new object[]{ value })
        {
        }
        public DbFilter(string columnName, DbType dbType, object value)
            :this(columnName,FilterType.Equals,dbType,new object[]{value})
        {
        }
        public DbFilter(string columnName, DbType dbType, string[] value)
        {
            this.ColumnName = columnName;
            this.DbType = dbType;

            #region length<1
            if (value==null || value.Length == 0) this.FilterType = FilterType.IsNull;
            #endregion

            #region length=1
            if (value.Length == 1) 
            { 
                this.FilterType = FilterType.Equals;
                this.Value = new object[] { Convert.ChangeType(value[0],dbType.Convert2Type()) };
            }
            #endregion

            #region length=2
            if (value.Length == 2 && !dbType.Equals(DbType.String) && !dbType.Equals(DbType.AnsiString) && !dbType.Equals(DbType.AnsiStringFixedLength))
            {
                if (value[0].Equals(string.Empty) && value[1].Equals(string.Empty))  this.FilterType = FilterType.IsNotNull;
                else if (value[0].Equals(string.Empty))
                {
                    this.FilterType = FilterType.LessThanEquals;
                    this.Value = new object[] { Convert.ChangeType(value[1], dbType.Convert2Type()) };
                }
                else if (value[1].Equals(string.Empty))
                {
                    this.FilterType = FilterType.MoreThanEquals;
                    this.Value = new object[] { Convert.ChangeType(value[0], dbType.Convert2Type()) };
                }
                else
                {
                    this.FilterType = FilterType.FullRange;
                    this.Value = new object[] { Convert.ChangeType(value[0], dbType.Convert2Type()), Convert.ChangeType(value[1], dbType.Convert2Type()) };
                }
            }
            else if (value.Length==2)
            {
                this.FilterType = FilterType.In;
                this.Value = new object[2] { value[0], value[1] };
            }
            #endregion

            #region length>2
            if (value.Length >2)
            {
                this.FilterType = FilterType.In;
                this.Value=new object[value.Length];
                for (int i = 0; i < this.Value.Length; i++) this.Value[i] = Convert.ChangeType(value[i], dbType.Convert2Type());
            }
            #endregion
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}", this.ColumnName, this.FilterType, this.DbType, this.Value);
        }

    }

    public class DbFilterCollection:ICollection<DbFilter>
    {
        private ConcurrentDictionary<string, DbFilter> fls = new ConcurrentDictionary<string, DbFilter>();
        public void Add(DbFilter item)
        {
            this.fls.TryAdd(item.ColumnName, item);
        }
        public DbFilterCollection Add(string columnName, FilterType filterType, DbType dbType, object[] values,bool withValue=false)
        {
            this.Add(new DbFilter(columnName, filterType, dbType, values, withValue));
            return this;
        }
        public DbFilterCollection Add(string columnName, DbType dbType, object value)
        {
            this.Add(new DbFilter(columnName, dbType, value));
            return this;
        }
        public DbFilterCollection Add(string columnName, string value)
        {
            this.Add(new DbFilter(columnName, value));
            return this;
        }
        public DbFilterCollection Add(string columnName, DbType dbType, string[] values)
        {
            this.Add(new DbFilter(columnName, dbType, values));
            return this;
        }
        public void Clear()
        {
            this.fls.Clear();
        }

        public bool Contains(DbFilter item)
        {
            return this.fls.ContainsKey(item.ColumnName);
        }

        public void CopyTo(DbFilter[] array, int arrayIndex)
        {
            if (arrayIndex > (array.Length - 1)) return;
            IList<DbFilter> ncols = this.fls.Values.ToList<DbFilter>();
            for (var i = 0; (i <= (array.Length - arrayIndex) && i < ncols.Count()); i++) array[i + arrayIndex] = ncols[i];
        }

        public int Count
        {
            get { return  this.fls.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false;  }
        }

        public bool Remove(DbFilter item)
        {
            DbFilter it;
            return this.fls.TryRemove(item.ColumnName,out it);
        }

        public IEnumerator<DbFilter> GetEnumerator()
        {
            return this.fls.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.fls.Values.GetEnumerator();
        }
    }

    public static class DbFilterHelper
    {
        public static DbFilter[] Fill2Command(this DbFilter[] filters, Database db,DbCommand cmd)
        {
            string condition = string.Empty;
            int idx=0;
            if (filters != null)
            {
                foreach (DbFilter f in filters)
                {
                    string c = string.Empty;
                    string tempc = string.Empty;
                    string wf = db.BuildSqlStringParameterName(string.Format("f_{0}", idx));
                    idx++;
                    switch (f.FilterType)
                    {
                        case FilterType.Equals:
                            c = string.Format("{0}={1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.NotEquals:
                            c = string.Format("{0}<>{1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.MoreThan:
                            c = string.Format("{0}>{1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.MoreThanEquals:
                            c = string.Format("{0}>={1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.LessThan:
                            c = string.Format("{0}<{1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.LessThanEquals:
                            c = string.Format("{0}<={1}", f.ColumnName, wf);
                            db.AddInParameter(cmd, wf, f.DbType, f.Value[0]);
                            break;
                        case FilterType.Include:
                            if (f.WithValue) c = string.Format("{0} like '%{1}%'", f.ColumnName, f.Value[0].ToString().Replace("'","''").Replace("%","[%]").Replace("[","[[]").Replace("]","[]]").Replace("_","[_]"));
                            else {
                                c = string.Format("{0} like '%'||{1}||'%'", f.ColumnName, wf);
                                db.AddInParameter(cmd, wf, f.DbType, string.Format("{0}", f.Value[0]));
                            }
                            break;
                        case FilterType.Exclude:
                            if (f.WithValue) c = string.Format("{0} not like '%{1}%'", f.ColumnName, f.Value[0].ToString().Replace("'", "''").Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]").Replace("_", "[_]"));
                            else
                            {
                                c = string.Format("{0} not like '%'||{1}||'%'", f.ColumnName, wf);
                                db.AddInParameter(cmd, wf, f.DbType, string.Format("{0}", f.Value[0]));
                            }
                            break;
                        case FilterType.BeginWith:
                            if (f.WithValue) c = string.Format("{0} like '{1}%'", f.ColumnName, f.Value[0].ToString().Replace("'", "''").Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]").Replace("_", "[_]"));
                            else
                            {
                                c = string.Format("{0} like {1}||'%'", f.ColumnName, wf);
                                db.AddInParameter(cmd, wf, f.DbType, string.Format("{0}", f.Value[0]));
                            }
                            break;
                        case FilterType.EndWith:
                            if (f.WithValue) c = string.Format("{0} like '%{1}", f.ColumnName, f.Value[0].ToString().Replace("'", "''").Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]").Replace("_", "[_]"));
                            else
                            {
                                c = string.Format("{0} like '%'||{1}", f.ColumnName, wf);
                                db.AddInParameter(cmd, wf, f.DbType, string.Format("{0}", f.Value[0]));
                            }
                            break;
                        case FilterType.IsNull:
                            c = string.Format("{0} is null", f.ColumnName);
                            break;
                        case FilterType.IsNotNull:
                            c = string.Format("{0} is not null", f.ColumnName);
                            break;
                        case FilterType.FullRange:
                            c = string.Format("{0}>={1} and {0}<={2}", f.ColumnName, wf + "_0", wf + "_1");
                            db.AddInParameter(cmd, wf + "_0", f.DbType, f.Value[0]);
                            db.AddInParameter(cmd, wf + "_1", f.DbType, f.Value[1]);
                            break;
                        case FilterType.LeftRange:
                            c = string.Format("{0}>={1} and {0}<{2}", f.ColumnName, wf + "_0", wf + "_1");
                            db.AddInParameter(cmd, wf + "_0", f.DbType, f.Value[0]);
                            db.AddInParameter(cmd, wf + "_1", f.DbType, f.Value[1]);
                            break;
                        case FilterType.RightRange:
                            c = string.Format("{0}>{1} and {0}<={2}", f.ColumnName, wf + "_0", wf + "_1");
                            db.AddInParameter(cmd, wf + "_0", f.DbType, f.Value[0]);
                            db.AddInParameter(cmd, wf + "_1", f.DbType, f.Value[1]);
                            break;
                        case FilterType.In:
                            if (f.Value.Length == 0) break;
                            for (int i = 0; i < f.Value.Length; i++)
                            {
                                tempc += string.Format(",{0}_{1}", wf, i);
                                db.AddInParameter(cmd, string.Format("{0}_{1}", wf, i), f.DbType, f.Value[i]);
                            }
                            c = tempc.Equals(string.Empty) ? string.Empty : string.Format("{0} in ({1})", f.ColumnName, tempc.Substring(1));
                            break;
                        case FilterType.NotIn:
                            if (f.Value.Length == 0) break;
                            for (int i = 0; i < f.Value.Length; i++)
                            {
                                tempc += string.Format(",{0}_{1}", wf, i);
                                db.AddInParameter(cmd, string.Format("{0}_{1}", wf, i), f.DbType);
                            }
                            c = tempc.Equals(string.Empty) ? string.Empty : string.Format("{0} not in ({1})", f.ColumnName, tempc.Substring(1));
                            break;
                    }
                    condition += c.Equals(string.Empty) ? string.Empty : string.Format(" and {0}", c);
                }
            }
            string whereSql=condition.Equals(string.Empty)?string.Empty:string.Format(" where {0}",condition.Substring(5));
            string execSql = cmd.CommandText;
            execSql=execSql.Contains("[...]")?execSql.Replace("[...]", whereSql):(execSql+whereSql);
            cmd.CommandText = execSql;
            return filters;
        }
        
    }

}
