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
            for (var i = 0; i < req.Cookies.Count; i++)
            {
                System.Web.HttpCookie item = req.Cookies[i];
                var cookie = new Cookie(item.Name, item.Value, item.Path);
                cookie.Expires = item.Expires;
                request.CookieContainer.Add(req.Url,cookie);
            }
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
