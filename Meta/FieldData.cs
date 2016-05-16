
using System.Data;
namespace Joy.Common.Meta
{
    public class FieldData
    {
        public string[] Fields { get; set; }
        public string[][] Datas { get; set; }
        public override string ToString()
        {
            return string.Format("Fields:{0},Datas:{1}", this.Fields.Length, this.Datas.Length);
        }
    }

    public static class FieldDataHelper
    {
        public static FieldData ToFieldData(this DataTable dt)
        {
            FieldData fd = new FieldData();
            fd.Fields = new string[dt.Columns.Count];
            for (int i = 0; i < fd.Fields.Length; i++) fd.Fields[i] = dt.Columns[i].ColumnName.ToUpper();
            if (dt.Rows.Count > 0)
            {
                fd.Datas = new string[dt.Rows.Count][];
                for (int i = 0; i < fd.Datas.Length; i++)
                {
                    fd.Datas[i] = new string[fd.Fields.Length];
                    for (int j = 0; j < fd.Fields.Length; j++) fd.Datas[i][j] = dt.Rows[i].IsNull(j) ? "" : dt.Rows[i][j].ToString();
                }
            }
            else fd.Datas = new string[] []{ };
            return fd;
        }

        public static DataTable ToDataTable(this FieldData fd)
        {
            DataTable dt = new DataTable();
            foreach (string c in fd.Fields)  dt.Columns.Add(new DataColumn(c));
            if (fd.Datas!=null) foreach (string[] d in fd.Datas) dt.Rows.Add(d);
            return dt;
        }

    }

}
