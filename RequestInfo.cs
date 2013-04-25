using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace StaticHtml
{
    public class RequestInfo
    {
        public String Host { get; set; }
        public int Port { get; set; }
        public String Path { get; set; }
        public NameValueCollection Headers { get; set; }
        public String Key { get; set; }
    }
}
