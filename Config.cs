using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace StaticHtml
{
    /// <summary>
    /// web.config 配置文件读取
    /// </summary>
    public class StaticHtmlSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RuleElementCollection Rules
        {
            get { return (RuleElementCollection)base[""]; }
        }

        /// <summary>
        /// 直接跳过，不进行静态化的Url，正则表达式
        /// </summary>
        [ConfigurationProperty("skip")]
        public String Skip
        {
            get { return base["skip"] as string; }
        }
    }

    /// <summary>
    /// Rule集合
    /// </summary>
    public class RuleElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RuleElement).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "rule"; }
        }

        public RuleElement this[int i]
        {
            get
            {
                return (RuleElement)BaseGet(i);
            }
        }
    }

    /// <summary>
    /// Rule 标签
    /// </summary>
    public class RuleElement : ConfigurationElement
    {
        /// <summary>
        /// Rule 名
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        /// <summary>
        /// Http请求生成唯一Key
        /// </summary>
        [ConfigurationProperty("genKey")]
        public ParElement GenKey
        {
            get
            {
                return (ParElement)base["genKey"];
            }
        }

        /// <summary>
        /// Http请求匹配
        /// </summary>
        [ConfigurationProperty("patten", IsRequired = true)]
        public ParElement Patten
        {
            get
            {
                return (ParElement)base["patten"];
            }
        }

        /// <summary>
        /// 该缓存是否过期判断
        /// </summary>
        [ConfigurationProperty("expire")]
        public ParElement Expire
        {
            get
            {
                return (ParElement)base["expire"];
            }
        }

        /// <summary>
        /// 获取跟HttpRequest的Html文本
        /// </summary>
        [ConfigurationProperty("genHtml")]
        public ParElement GenHtml
        {
            get
            {
                return (ParElement)base["genHtml"];
            }
        }

        /// <summary>
        /// 存储Html缓存文本
        /// </summary>
        [ConfigurationProperty("store")]
        public ParElement Store
        {
            get
            {
                return (ParElement)base["store"];
            }
        }
    }

    /// <summary>
    /// 一个配置节点，用来读取patten，strore节点信息
    /// </summary>
    public class ParElement : ConfigurationElement
    {
        [ConfigurationProperty("type")]
        public string Type
        {
            get
            {
                return (string)base["type"];
            }
        }

        [ConfigurationProperty("pars")]
        public string Pars
        {
            get
            {
                return (string)base["pars"];
            }
        }


        public Dictionary<string, string> GetPars()
        {
            var str = this.Pars;
            var parsSplit = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var pars = new Dictionary<string, string>();
            foreach (var item in parsSplit)
            {
                var temp = item.Split('=');
                if (temp.Length == 2)
                {
                    pars.Add(temp[0], temp[1]);
                }
            }
            return pars;
        }

    }
}
