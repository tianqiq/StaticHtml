using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;

namespace StaticHtml
{
    static class HttpParseHelp
    {
        /// <summary>
        /// 将流分析成Header集合和Body内存流
        /// </summary>
        /// <param name="_in"></param>
        /// <returns></returns>
        public static HttpInfo Parse(Stream _in)
        {
            var info = new HttpInfo();
            int position = 0;
            info.Headers = ParseHeader(_in, ref position);
            info.Content = ToMemStream(_in);
            return info;
        }

        /// <summary>
        /// 将流中Header分析出来
        /// </summary>
        /// <param name="_in"></param>
        /// <param name="headerEndPosition"></param>
        /// <returns></returns>
        public static IList<HttpInfo.Header> ParseHeader(Stream _in, ref int headerEndPosition)
        {
            var headers = new List<HttpInfo.Header>();
            var buff = new byte[4];
            int len = 4, totallen = 0, state = 1;
            var memStream = new MemoryStream();
            while (state != 5)
            {
                _in.Read(buff, 0, len);
                memStream.Write(buff, 0, len);
                headerEndPosition += len;
                totallen += len;
                for (int i = 0; i < len; i++)
                {
                    var one = buff[i];
                    switch (state)
                    {
                        case 1:
                            {
                                if (one == '\r')
                                {
                                    state = 2;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 2:
                            {
                                if (one == '\n')
                                {
                                    var header = ToHeader(memStream, totallen - len + i + 1);
                                    if (header != null)
                                    {
                                        headers.Add(header);
                                    }
                                    totallen = len - i - 1;
                                    memStream.Position = 0;
                                    memStream.Write(buff, i + 1, totallen);
                                    state = 3;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 3:
                            {
                                if (one == '\r')
                                {
                                    state = 4;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 4:
                            {
                                if (one == '\n')
                                {
                                    state = 5;
                                }
                                else { state = 1; }
                                break;
                            }
                    }
                }
                len = 5 - state;
            }
            return headers;
        }

        /// <summary>
        ///  将流中的一行，解析成Header
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static HttpInfo.Header ToHeader(MemoryStream data, int len)
        {
            var line = Encoding.Default.GetString(data.ToArray(), 0, len);
            HttpInfo.Header header = null;
            var pars = line.Split(new String[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
            if (pars.Length == 2)
            {
                header = new HttpInfo.Header() { Name = pars[0], Value = pars[1] };
            }
            return header;
        }

        /// <summary>
        /// 将流转换成内存流
        /// </summary>
        /// <param name="_in"></param>
        /// <returns></returns>
        public static Stream ToMemStream(Stream _in)
        {
            var data = new MemoryStream();
            var buff = new Byte[1024];
            var len = 0;
            while ((len = _in.Read(buff, 0, 1024)) != 0)
            {
                data.Write(buff, 0, len);
            }
            data.Position = 0;
            return data;
        }
        /*
        /// <summary>
        /// 跳过响应Http头信息
        /// </summary>
        /// <param name="_in"></param>
        public static IList<HttpInfo.Header> ParseHeader(Stream _in)
        {
            var headers = new List<HttpInfo.Header>();
            var reader = new StreamReader(_in);
            var line = String.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                var pars = line.Split(new String[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                if (pars.Length == 2)
                {
                    headers.Add(new HttpInfo.Header() { Name = pars[0], Value = pars[1] });
                }
            }
            return headers;
        }*/

        const string GZIPHEAD = "Content-Encoding: gzip";

        /// <summary>
        /// 将Http相应流（包括httpheader）转换成Gzip内存流，httpheader不转换
        /// </summary>
        /// <param name="_out"></param>
        /// <returns></returns>
        public static MemoryStream ToGzip(Stream _out)
        {
            var buff = new byte[4];
            int len = 4, state = 1;
            var memStream = new MemoryStream();
            bool gzip = false;
            while (state != 5)
            {
                _out.Read(buff, 0, len);
                memStream.Write(buff, 0, len);
                for (int i = 0; i < len; i++)
                {
                    var one = buff[i];
                    switch (state)
                    {
                        case 1:
                            {
                                if (one == '\r')
                                {
                                    state = 2;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 2:
                            {
                                if (one == '\n')
                                {
                                    state = 3;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 3:
                            {
                                if (one == '\r')
                                {
                                    state = 4;
                                }
                                else { state = 1; }
                                break;
                            }
                        case 4:
                            {
                                if (one == '\n')
                                {
                                    var position = memStream.Position;
                                    if (!new StreamReader(memStream).ReadToEnd().Contains(GZIPHEAD))
                                    {
                                        memStream.Position = position;
                                        CopyTo(_out, new GZipStream(memStream,CompressionMode.Compress));
                                    }
                                    else
                                    {
                                        CopyTo(_out, memStream);
                                    }
                                    state = 5;
                                }
                                else { state = 1; }
                                break;
                            }
                    }
                }
                len = 5 - state;
            }
            memStream.Position = 0;
            return memStream;
        }

        /// <summary>
        /// 将一个流复制到内一个流
        /// </summary>
        /// <param name="_out"></param>
        /// <param name="_in"></param>
        private static void CopyTo(Stream _out, Stream _in)
        {
            var buff = new byte[1024];
            int len = 0;
            while ((len = _out.Read(buff, 0, 1024)) != 0)
            {
                _in.Write(buff, 0, len);
            }
        }
    }
}
