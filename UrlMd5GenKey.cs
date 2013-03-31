using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticHtml
{
    /// <summary>
    /// 将HttpRequest生成为HttpRequest.RawUrl通过Md5的key   系统默认
    /// </summary>
    class UrlMd5GenKey:IGenKey
    {
        #region IGenKey 成员

        public string GenKey(System.Web.HttpRequest request)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(request.RawUrl,"MD5");
        }

        #endregion
    }
}
