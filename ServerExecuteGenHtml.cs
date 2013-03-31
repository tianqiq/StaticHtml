using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;

namespace StaticHtml
{
    /// <summary>
    /// 依靠Server.Execute生成HttpRequest的Html内容，只适用于未UrlRewrite的站点，比如asp.net mvc 就不能用
    /// </summary>
    public class ServerExecuteGenHtml : IGenHtml
    {
        #region IGenHtml 成员

        public string GenHTML(HttpRequest req)
        {
            var writer = new StringWriter();
            HttpContext.Current.Server.Execute(req.RawUrl, writer, true);
            return writer.ToString();
        }

        #endregion
    }
}
