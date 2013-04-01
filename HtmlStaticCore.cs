using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
namespace StaticHtml
{
    /// <summary>
    /// HtmlStatic 核心
    /// </summary>
    public class HtmlStaticCore
    {
        /// <summary>
        /// Rule 集合
        /// </summary>
        public IList<Rule> rules { get; set; }

        /// <summary>
        /// 系统定义的不进行Html缓存的Url标识
        /// </summary>
        public const String SKIPMARKHEAD = "staticHtml";

        /// <summary>
        /// 直接跳过的Url正则表达式
        /// </summary>
        private Regex skipRegex;

        /// <summary>
        /// 判断是否调过
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Boolean IsSkip(HttpRequest req)
        {
            var skip = false;
            if (skipRegex != null)
            {
                skip = skipRegex.IsMatch(req.RawUrl);
            }
            return req.Headers[HtmlStaticCore.SKIPMARKHEAD] == "1" || skip;
        }

        /// <summary>
        /// 根据配置文件，初始化HtmlStaticCore
        /// </summary>
        /// <param name="config"></param>
        public HtmlStaticCore(StaticHtmlSection config)
        {
            if (!String.IsNullOrEmpty(config.Skip))
            {
                skipRegex = new Regex(config.Skip, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            rules = new List<Rule>();
            foreach (RuleElement rule in config.Rules)
            {
                rules.Add(ToRule(rule));
            }
        }

        #region 根据配置文件信息，初始化HtmlStaticCore
        private void SetPars(IDictionary<string, string> pars, object o)
        {
            var type = o.GetType();
            foreach (var item in pars)
            {
                var prop = type.GetProperty(item.Key, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                if (prop != null)
                {
                    prop.SetValue(o, item.Value, null);
                }
            }
        }

        private object GenObject(string type)
        {
            var splits = type.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            object ret = null;
            if (splits.Length == 2)
            {
                ret = Activator.CreateInstance(splits[1], splits[0]).Unwrap();
            }
            else if (splits.Length == 1)
            {
                ret = Activator.CreateInstance(null, splits[0]).Unwrap();
            }
            return ret;
        }

        private Rule ToRule(RuleElement ele)
        {
            Rule rule = new Rule();
            rule.Name = ele.Name;
            rule.Patten = ToPatten(ele.Patten);
            rule.GenKey = ToGenKey(ele.GenKey);
            rule.Expire = ToTimeExpire(ele.Expire);
            rule.Store = ToStorm(ele.Store);
            rule.GenHTML = ToGenHtml(ele.GenHtml);
            return rule;
        }

        private IPatten ToPatten(ParElement ele)
        {
            IPatten patten = null;
            if (ele == null || (patten = (GenObject(ele.Type) as IPatten)) == null)
            {
                patten = new RegexPatten();
            }
            SetPars(ele.GetPars(), patten);
            return patten;
        }

        private IGenKey ToGenKey(ParElement ele)
        {
            IGenKey genKey = null;
            if (ele == null || (genKey = (GenObject(ele.Type) as IGenKey)) == null)
            {
                genKey = new UrlMd5GenKey();
            }
            SetPars(ele.GetPars(), genKey);
            return genKey;
        }

        private IGenHtml ToGenHtml(ParElement ele)
        {
            IGenHtml genHtml = null;
            if (ele == null || (genHtml = (GenObject(ele.Type) as IGenHtml)) == null)
            {
                genHtml = new WebRequestGenHtml();
            }
            SetPars(ele.GetPars(), genHtml);
            return genHtml;
        }

        private IExpire ToTimeExpire(ParElement ele)
        {
            IExpire expire = null;
            if (ele == null || (expire = (GenObject(ele.Type) as IExpire)) == null)
            {
                expire = new TimeExpire();
            }
            SetPars(ele.GetPars(), expire);
            return expire;
        }

        private IStore ToStorm(ParElement ele)
        {
            IStore store = null;
            if (ele == null || (store = (GenObject(ele.Type) as IStore)) == null)
            {
                store = new FileStore();
            }
            SetPars(ele.GetPars(), store);
            return store;
        }
        #endregion


        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public HtmlStaticCore()
        {
            rules = new List<Rule>();
        }

        /// <summary>
        /// HtmlStatic入口
        /// </summary>
        /// <param name="context"HttpContext>HttpContext</param>
        public void Process(HttpContext context)
        {
            foreach (var item in rules)
            {
                var req = context.Request;
                var rep = context.Response;
                if (item.IsPatten(req))
                {
                    item.Apply(context);
                    break;
                }
            }
        }
    }
}
