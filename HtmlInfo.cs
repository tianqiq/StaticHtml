using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace StaticHtml
{
    /// <summary>
    /// Html缓存信息
    /// </summary>
    public class HtmlInfo
    {
        /// <summary>
        /// 缓存时间
        /// </summary>
        public DateTime StoreTime { get; set; }
        
        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }
    }
}
