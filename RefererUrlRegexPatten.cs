using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace StaticHtml
{
    class RefererUrlRegexPatten : IPatten
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
            var referUrl = request.Headers["Referer"];
            return referUrl != null && reg.IsMatch(referUrl);
        }
        #endregion
    }
}
