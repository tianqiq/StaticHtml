using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

namespace StaticHtml
{
    /// <summary>
    /// 依靠发送Http请求生成HttpRequest的Html内容，理论上适用与所有站点  系统默认
    /// </summary>
    public class WebRequestGenHtml : IGenHtml
    {
        #region IGenHtml 成员

        public string GenHTML(HttpRequest req)
        {
            var url = req.Url.ToString();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add(HtmlStaticCore.SKIPMARKHEAD, "1");
            request.CookieContainer = new CookieContainer();
            foreach (System.Web.HttpCookie item in req.Cookies)
            {
                var cookie = new Cookie(item.Name, item.Value, item.Path, item.Domain);
                cookie.Expires = item.Expires;
                request.CookieContainer.Add(cookie);
            }
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
