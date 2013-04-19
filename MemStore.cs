using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace StaticHtml
{
    /// <summary>
    /// 简单的通过IDictionary内存储存器
    /// </summary>
    public class MemStore : IStore
    {
        #region IStore 成员

        private IDictionary<string, Stream> Cache = new Dictionary<string, Stream>();
        private IDictionary<string, CacheInfo> infos = new Dictionary<string, CacheInfo>();
        public void Save(string key, Stream repInfo)
        {
            Cache[key] = repInfo;
            CacheInfo info = null;
            if (infos.ContainsKey(key))
            {
                info = infos[key];
            }
            else
            {
                info = new CacheInfo();
            }
            info.Size = repInfo.Length;
            info.StoreTime = DateTime.Now;
            infos[key] = info;
        }

        public Stream Get(String key)
        {
            Stream ret = null;
            if (Cache.ContainsKey(key))
            {
                ret = new MemoryStream((Cache[key] as MemoryStream).ToArray()); ;
                ret.Position = 0;
            }

            return ret;
        }


        public CacheInfo Query(string key)
        {
            CacheInfo info = null;
            if (infos.ContainsKey(key))
            {

                info = infos[key];
            }
            return info;
        }
        #endregion
    }
}
