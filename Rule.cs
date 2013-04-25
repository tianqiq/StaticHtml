using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
namespace StaticHtml
{
    /// <summary>
    /// Rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// 名字
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Html生存器
        /// </summary>
        public IGenHtml GenHTML { get; set; }

        /// <summary>
        /// Html缓存储存器
        /// </summary>
        public IStore Store { get; set; }

        /// <summary>
        /// HttpRequest匹配器
        /// </summary>
        public IPatten Patten { get; set; }

        /// <summary>
        /// Html缓存是否过期判断器
        /// </summary>
        public IExpire Expire { get; set; }

        /// <summary>
        /// HttpRequest生成唯一key生成器
        /// </summary>
        public IGenKey GenKey { get; set; }

        /// <summary>
        /// 判断HttpRequest 是否匹配该Rule
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <returns>是否品牌</returns>
        public bool IsPatten(HttpRequest request)
        {
            return Patten.IsPatten(request);
        }


        /// <summary>
        /// 强制刷新缓存标识
        /// </summary>
        public const String REFRESH = "__forced__refresh__";
        public const String REFRESHHEADER = "forced__refresh";

        /// <summary>
        /// 全局信息，标识该Request/key 是否正在生成html
        /// </summary>
        HashSet<string> GlobalGenHtmlState = new HashSet<string>();

        /// <summary>
        /// 应用该规则
        /// </summary>
        /// <param name="context">HttpContext</param>
        public void Apply(HttpContext context)
        {
            var req = context.Request;
            var key = GenKey.GenKey(req);
            var info = Store.Query(key);
            if (info != null)
            {
                info.Key = key;
            }
            if (req.RawUrl.Contains(REFRESH) || req.Headers[REFRESHHEADER] == "1" || Expire.IsExpire(req, info))
            {
                var reqInfo = ToRequestInfo(req);
                reqInfo.Key = key;
                System.Threading.ThreadPool.QueueUserWorkItem(ThreadStart, reqInfo);
            }
            else
            {
                ResponceCache(context, key, info);
            }
        }


        /// <summary>
        /// 供线程线程调用方法
        /// </summary>
        /// <param name="arg"></param>
        private void ThreadStart(Object arg)
        {
            var req = arg as RequestInfo;
            try
            {
                GenHtmlAndSave(req);
            }
            catch (Exception e)
            {
                LogHelp.Warn("getHtml error " + e);
            }
        }


        /// <summary>
        /// 输出Http流
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="info"></param>
        private void OutResponse(HttpRequest req, HttpResponse rep, HttpInfo info)
        {
            Stream _out = AcceptGzip(req, rep, info);
            OutHeaders(rep, info);
            OutBody(rep, _out);
        }

        /// <summary>
        /// 输出http响应body
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="_out"></param>
        private static void OutBody(HttpResponse rep, Stream _out)
        {
            var buff = new byte[2048];
            var len = 0;
            using (_out)
            {
                while ((len = _out.Read(buff, 0, 2048)) != 0)
                {
                    rep.OutputStream.Write(buff, 0, len);
                }
            }
        }

        /// <summary>
        /// 容许缓存并且转发的各个客户的http头
        /// </summary>
        HashSet<string> alowHeader = new HashSet<string>() { "Content-Type", "Vary", "Etag", "Location", "Charset" };

        /// <summary>
        /// 输出http响应header
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="info"></param>
        private void OutHeaders(HttpResponse rep, HttpInfo info)
        {
            foreach (var item in info.Headers)
            {
                if (alowHeader.Contains(item.Name))
                {
                    rep.AppendHeader(item.Name, item.Value.TrimEnd());
                }
            }
        }

        /// <summary>
        /// 根据请求头自动判断客户端是否支持gzip，如果不支持， 在服务器解压
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rep"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static Stream AcceptGzip(HttpRequest req, HttpResponse rep, HttpInfo info)
        {
            var acceptEncoding = req.Headers.Get("Accept-Encoding");
            Stream _out;
            if (acceptEncoding != null && acceptEncoding.Contains("gzip"))
            {
                _out = info.Content;
                rep.AppendHeader("Content-Encoding", "gzip");
            }
            else
            {
                _out = new GZipStream(info.Content, CompressionMode.Decompress);
            }
            return _out;
        }


        /// <summary>
        /// 缓存没过期，直接输出缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="info"></param>
        private void ResponceCache(HttpContext context, string key, CacheInfo info)
        {
            var req = context.Request;
            var rep = context.Response;
            string time = info.StoreTime.ToString("r");
            rep.AppendHeader("Last-Modified", time);
            if (req.Headers["If-Modified-Since"] == time)
            {
                rep.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
                LogHelp.Info("cache hit 304 " + req.RawUrl);
            }
            else
            {
                var httpInfo = new HttpInfo();
                var _stream = Store.Get(key);
                int headEndPosition = 0;
                httpInfo.Headers = HttpParseHelp.ParseHeader(_stream, ref headEndPosition);
                httpInfo.Content = _stream;
                OutResponse(req, rep, httpInfo);
                LogHelp.Info("cache hit html " + req.RawUrl);
            }
            context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// 将HttpRequest转换为RequestInfo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RequestInfo ToRequestInfo(HttpRequest request)
        {
            var info = new RequestInfo();
            info.Path = request.RawUrl;
            info.Host = request.Url.Host;
            info.Port = request.Url.Port;
            info.Headers = request.Headers;
            return info;
        }

        /// <summary>
        /// 生成缓存并保存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="info"></param>
        private void GenHtmlAndSave(RequestInfo info)
        {
            if (!GlobalGenHtmlState.Contains(info.Key))
            {
                try
                {
                    GlobalGenHtmlState.Add(info.Key);
                    var html = GenHTML.GenHTML(info);
                    if (html != null)
                    {
                        Store.Save(info.Key, html);
                    }
                    else
                    {
                        LogHelp.Warn("getHtml is null " + info.Path);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    GlobalGenHtmlState.Remove(info.Key);
                }
            }
        }

    }
}
