using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StaticHtml
{
    /// <summary>
    /// Http信息
    /// </summary>
    public class HttpInfo
    {
        /// <summary>
        /// Http响应流第一行
        /// </summary>
        public String FirstLine { get; set; }

        /// <summary>
        /// 头集合
        /// </summary>
        public IList<Header> Headers { get; set; }
        /// <summary>
        /// Body部分
        /// </summary>
        public Stream Content { get; set; }
        public class Header
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
