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
                if (htmlSection.Run == "on")
                {
                    core = HtmlStaticCore.GetInstance(htmlSection);
                    context.BeginRequest += new EventHandler(context_BeginRequest);
                    LogHelp.Info("int success! ");
                }
                else
                {
                    LogHelp.Warn("run off! 请在staticHtml节点中添加属性run=\"on\" on:启用 off:关闭");
                }
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
