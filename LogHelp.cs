using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;

namespace StaticHtml
{
    static class LogHelp
    {

        readonly static TraceSwitch ts;

        static LogHelp()
        {
            var lister = new TextWriterTraceListener(HttpRuntime.AppDomainAppPath + "staticHtml_log.txt");
            lister.TraceOutputOptions = TraceOptions.DateTime;
            Trace.Listeners.Add(lister);
            Trace.AutoFlush = true;
            ts = new TraceSwitch("staticHtml", "staticHtml 日志开关", "2");
        }

        public static void Write(string message)
        {
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ": " + message);
        }

        public static void Info(string message)
        {
            if (ts.TraceInfo)
            {
                Trace.TraceInformation(message);
            }
        }

        public static void Warn(string message)
        {
            if (ts.TraceWarning)
            {
                Trace.TraceWarning(message);
            }
        }

        public static void Error(string message)
        {
            if (ts.TraceError)
            {
                Trace.TraceError(message);
            }
        }

    }
}
