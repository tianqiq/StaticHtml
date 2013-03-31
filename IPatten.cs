using System;
using System.Web;
using System.ComponentModel;

namespace StaticHtml
{
    public interface IPatten
    {
        /// <summary>
        /// 将HttpRequest判断是否匹配Rule规则
        /// </summary>
        /// <param name="request">HttpRequest请求</param>
        /// <returns>是否匹配</returns>
        bool IsPatten(HttpRequest request);
    }
}
