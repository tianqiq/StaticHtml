using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
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
            if (req.RawUrl.Contains(REFRESH) || Expire.IsExpire(req, info))
            {
                GenHtmlAndSave(context, key, info);
            }
            else
            {
                ResponceCache(context, key, info);
            }
        }

        /// <summary>
        /// 缓存没过期，直接输出缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="info"></param>
        private void ResponceCache(HttpContext context, string key, HtmlInfo info)
        {
            var req = context.Request;
            var rep = context.Response;
            string time = info.StoreTime.ToString("yyyy-MM-dd HH:mm:ss");
            rep.AppendHeader("Last-Modified", time);
            if (req.Headers["If-Modified-Since"] == time)
            {
                rep.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
            }
            else
            {
                rep.Write(Store.Get(key));
            }
            context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// 生成缓存保存，并输出
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="info"></param>
        private void GenHtmlAndSave(HttpContext context, string key, HtmlInfo info)
        {
            var req = context.Request;
            var rep = context.Response;
            if (!GlobalGenHtmlState.Contains(key))
            {
                try
                {
                    GlobalGenHtmlState.Add(key);
                    var html = GenHTML.GenHTML(req);
                    if (html != null)
                    {
                        Store.Save(key, html);
                        DateTime lastModifyed = info != null ? info.StoreTime : Store.Query(key).StoreTime;
                        rep.AppendHeader("Last-Modified", lastModifyed.ToString("yyyy-MM-dd HH:mm:ss"));
                        rep.Write(html);
                        context.ApplicationInstance.CompleteRequest();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    GlobalGenHtmlState.Remove(key);
                }
            }
        }

    }
}
