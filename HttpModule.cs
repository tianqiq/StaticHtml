using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Diagnostics;

namespace StaticHtml
{
    /// <summary>
    /// StaticHtmlHttpModule
    /// </summary>
    public class HttpModule : IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {

        }

        private readonly static StaticHtmlSection htmlSection = System.Configuration.ConfigurationManager.GetSection("staticHtml") as StaticHtmlSection;

        HtmlStaticCore core;
        public void Init(HttpApplication context)
        {
            try
            {
                core = HtmlStaticCore.GetInstance(htmlSection);
                context.BeginRequest += new EventHandler(context_BeginRequest);
                LogHelp.Write("int success! ");
            }
            catch (Exception e)
            {
                LogHelp.Error("ini error! " + e.ToString());
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
                LogHelp.Warn("request process error " + httpApplication.Request.RawUrl + " " + ex.ToString());
            }
        }
        #endregion
    }
}
