
using System.Collections;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Joy.Common.Meta
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public override bool Equals(object obj)
        {
            return obj.ToString().ToUpper().Equals(this.Key.ToUpper());
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public KeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public KeyValue()
            :this(string.Empty,string.Empty)
        {
        }
        public override string ToString()
        {
            return string.Format("{0}:{1}",this.Key,this.Value);
        }
    }
    public static class KeyValueHelper
    {
        public static KeyValue[] Fill(this KeyValue[] kvs, IDictionary datas)
        {
            int i=0;
            foreach (string key in datas.Keys)
            {
                if (i < kvs.Length)
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = key;
                    if (datas[key] == null) kv.Value = string.Empty;
                    else if (datas[key] is DateTime) kv.Value = ((DateTime)datas[key]).ToString("yyyy-MM-dd HH:mm:ss");
                    else kv.Value = datas[key].ToString();
                    kvs[i] = kv; 
                    i++;
                }
                else break;
            }
            return kvs;
        }
        public static KeyValue[] ToKeyValueArray(IDictionary datas)
        {
            if (datas == null) return new KeyValue[] { };
            KeyValue[] kvs = new KeyValue[datas.Keys.Count];
            int i = 0;
            foreach (string key in datas.Keys)
            {
                KeyValue kv = new KeyValue();
                kv.Key = key;
                if (datas[key] == null) kv.Value = string.Empty;
                else if (datas[key] is DateTime) kv.Value = ((DateTime)datas[key]).ToString("yyyy-MM-dd HH:mm:ss");
                else kv.Value = datas[key].ToString();
                kvs[i] = kv; 
                i++;
            }
            return kvs;
        }
        public static string RequiredCheck(this KeyValue[] keyValueArray, string requireKeys)
        {
            return RequiredCheck(keyValueArray.ToDictionary(), requireKeys);
        }
        public static string RequiredCheck(this IDictionary datas, string requireKeys)
        {
            string errors = string.Empty;
            string[] checkes = requireKeys.Split(',');
            if (checkes.Length == 0) return errors;
            foreach (string c in checkes) if (!datas.Contains(c.ToUpper())) errors += string.Format(",{0}", c);
            if (errors.Length > 0) errors = errors.Substring(1);
            return errors;
        }
        public static IDictionary ToDictionary(this KeyValue[] keyValueArray)
        {
            IDictionary dict=new Dictionary<string,string>();
            foreach(KeyValue kv in keyValueArray)
            {
                if (dict.Contains(kv.Key)) dict[kv.Key] = kv.Value;
                else dict.Add(kv.Key.ToUpper(), kv.Value);
            }
            return dict;
        }

    }

}
