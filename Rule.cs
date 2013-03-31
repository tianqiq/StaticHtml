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
                var html = GenHTML.GenHTML(context.Request);
                if (html != null)
                {
                    context.Response.Write(html);
                    Store.Save(key, html);
                    context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                context.Response.Write(Store.Get(key));
                context.ApplicationInstance.CompleteRequest();
            }

        }

    }
}
