using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticHtml
{
    /// <summary>
    /// 将HttpRequest生成为HttpRequest.RawUrl通过Md5的key   系统默认
    /// </summary>
    class UrlMd5GenKey : IGenKey
    {
        #region IGenKey 成员

        const string MARK1 = "&" + Rule.REFRESH;
        const string MARK2 = "?" + Rule.REFRESH;

        public string GenKey(System.Web.HttpRequest request)
        {
            var url = request.RawUrl.Replace(MARK1, "").Replace(MARK2, "");
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(url, "MD5");
        }

        #endregion
    }
}
