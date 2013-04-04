using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Sockets;

namespace StaticHtml
{
    /// <summary>
    /// 依靠发送Http请求生成HttpRequest的Html内容，理论上适用与所有站点包括mvc等经过urlrewrite的站点  系统默认
    /// </summary>
    public class WebRequestGenHtml : IGenHtml
    {
        #region IGenHtml 成员

        const String CONNECTION_CLOSE = "Connection: close\r\n";
        const String HEADERFORMAT = "{0}: {1}\r\n";

        public string GenHTML(HttpRequest req)
        {
            using (var client = new TcpClient())
            {
                client.Connect(req.Url.Host, req.Url.Port);
                using (var stream = client.GetStream())
                {
                    var _out = new StreamWriter(stream);
                    var _in = new StreamReader(stream);
                    _out.Write(String.Format("{0} {1} {2}\r\n", req.HttpMethod, req.RawUrl, req.ServerVariables["SERVER_PROTOCOL"]));
                    _out.Write(String.Format(HEADERFORMAT, "HOST", req.Url.Authority));
                    _out.Write(String.Format(HEADERFORMAT, HtmlStaticCore.SKIPMARKHEAD, 1));
                    foreach (String key in req.Headers.Keys)
                    {
                        var lowerKey = key.ToLower();
                        if (lowerKey != "connection" && lowerKey != "host")
                        {
                            var val = req.Headers[key];
                            _out.Write(String.Format(HEADERFORMAT, key, val));
                        }
                    }
                    _out.Write(CONNECTION_CLOSE);
                    _out.Write("\r\n");
                    _out.Flush();
                    SkipHeader(_in.BaseStream);
                    return _in.ReadToEnd();
                }
            }

        }

        /// <summary>
        /// 跳过响应Http头信息
        /// </summary>
        /// <param name="_in"></param>
        private void SkipHeader(Stream _in)
        {
            var buff = new byte[4];
            int len = 4;
            int state = 1;
            while (true)
            {
                _in.Read(buff, 0, len);
                for (int i = 0; i < len; i++)
                {
                    var one = buff[i];
                    switch (state)
                    {
                        case 1:
                            {
                                if (one == '\r')
                                {
                                    state = 2;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 2:
                            {
                                if (one == '\n')
                                {
                                    state = 3;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 3:
                            {
                                if (one == '\r')
                                {
                                    state = 4;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 4:
                            {
                                if (one == '\n')
                                {
                                    return;
                                }
                                else { state = 1; }
                                break;
                            }
                    }
                }
                len = 5 - state;
            }
        }

        /* asp.net bug。当cookie value里面有逗号会出错
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
        }*/

        #endregion
    }
}
