using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace StaticHtml
{
    /// <summary>
    /// StaticHtmlHttpModule
    /// </summary>
    public class HttpModule : IHttpModule
    {
        #region IHttpModule 成员

        private string log_path = null;

        private string Getpath()
        {
            if (log_path == null)
            {
                log_path = HttpContext.Current.Server.MapPath("~/") + "log.txt";
            }
            return log_path;
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="info"></param>
        private void Log(string info)
        {
            System.IO.File.AppendAllText(Getpath(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "  " + info + "\n");
        }

        public void Dispose()
        {

        }

        HtmlStaticCore core;
        public void Init(HttpApplication context)
        {
            try
            {
                core = new HtmlStaticCore(System.Configuration.ConfigurationManager.GetSection("staticHtml") as StaticHtmlSection);
                context.BeginRequest += new EventHandler(context_BeginRequest);
                Log("ini:success!");
            }
            catch (Exception e)
            {
                Log("ini:error!  " + e.ToString());
            }
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            var httpApplication = sender as HttpApplication;
            try
            {
                if (!core.IsSkip(httpApplication.Context.Request))
                {
                    core.Process(httpApplication.Context);
                }
            }
            catch (Exception ex)
            {
                Log("process:error!  " + httpApplication.Request.RawUrl + "  " + ex.ToString());
            }
        }



        #endregion
    }
}
