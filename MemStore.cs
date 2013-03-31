using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace StaticHtml
{
    /// <summary>
    /// 简单的通过IDictionary内存储存器
    /// </summary>
    public class MemStore : IStore
    {
        #region IStore 成员

        private IDictionary<string, string> Cache = new Dictionary<string, string>();
        private IDictionary<string, HtmlInfo> infos = new Dictionary<string, HtmlInfo>();
        public void Save(string key, string html)
        {
            Cache[key] = html;
            HtmlInfo info = null;
            if (infos.ContainsKey(key))
            {
                info = infos[key];
            }
            else
            {
                info = new HtmlInfo();
            }
            info.Size = html.Length;
            info.StoreTime = DateTime.Now;
            infos[key] = info;
        }

        public string Get(String key)
        {
            if (Cache.ContainsKey(key))
            {
                return Cache[key];
            }
            return null;
        }


        public HtmlInfo Query(string key)
        {
            HtmlInfo info = null;
            if (infos.ContainsKey(key))
            {
                info = infos[key];
            }
            return info;
        }
        #endregion
    }
}
