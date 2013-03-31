using System;
using System.Web;
using System.ComponentModel;

namespace StaticHtml
{
    /// <summary>
    /// 生成html接口
    /// </summary>
    public interface IGenHtml
    {
        /// <summary>
        /// 根据HttpRequest生成该请求的Html
        /// </summary>
        /// <param name="req">HttpRequest请求</param>
        /// <returns>Html内容</returns>
        string GenHTML(HttpRequest req);
    }
}
