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
            Trace.Listeners.Add(new TextWriterTraceListener(HttpRuntime.AppDomainAppPath + "staticHtml_log.txt"));
            Trace.AutoFlush = true;
            ts = new TraceSwitch("staticHtml", "staticHtml 日志开关", "2");
        }

        public static void Write(string message)
        {
            Trace.WriteLine(message);
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
                Trace.TraceInformation(message);
            }
        }

        public static void Error(string message)
        {
            if (ts.TraceError)
            {
                Trace.TraceInformation(message);
            }
        }

    }
}
