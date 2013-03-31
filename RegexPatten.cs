using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace StaticHtml
{
    /// <summary>
    /// 正则表达式匹配器 系统默认
    /// </summary>
    public class RegexPatten : IPatten
    {

        private Regex reg;

        #region IPatten 成员
        public String RegPatten
        {
            get { return reg.ToString(); }
            set
            {
                reg = new Regex(value, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        public bool IsPatten(HttpRequest request)
        {
            return reg.IsMatch(request.RawUrl);
        }
        #endregion
    }
}
