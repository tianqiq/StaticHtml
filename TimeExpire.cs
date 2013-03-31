using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace StaticHtml
{
    /// <summary>
    /// 根据时间判断Html缓存是否过期  系统默认
    /// </summary>
    public class TimeExpire : IExpire
    {
        #region IExpire 成员


        private int _second = 300;

        public String Second
        {
            get { return _second.ToString(); }
            set
            {
                if (!Int32.TryParse(value, out _second))
                {
                    _second = 300;
                }
            }
        }

        public bool IsExpire(HttpRequest req, HtmlInfo info)
        {
            return info == null || (DateTime.Now - info.StoreTime).TotalSeconds > _second;
        }

        #endregion
    }
}
