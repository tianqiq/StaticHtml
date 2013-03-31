using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace StaticHtml
{
    /// <summary>
    /// 将HttpRequest生成唯一key接口
    /// </summary>
    public interface IGenKey
    {
        /// <summary>
        /// 根据HttpRequest生成唯一key
        /// </summary>
        /// <param name="request">HttpRequest请求</param>
        /// <returns>唯一key</returns>
        String GenKey(HttpRequest request);
    }
}
