using System;
using System.Web;
using System.ComponentModel;

namespace StaticHtml
{
    /// <summary>
    /// 该缓存内容是否过期判断接口
    /// </summary>
    public interface IExpire
    {
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <param name="req">当前HttpReq</param>
        /// <param name="info">缓存信息</param>
        /// <returns>是否过期</returns>
        bool IsExpire(HttpRequest req, HtmlInfo info);
    }
}
