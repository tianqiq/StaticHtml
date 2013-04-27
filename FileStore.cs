using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Security;
using System.Collections.Generic;

namespace StaticHtml
{
    /// <summary>
    /// 文件方式储存Html缓存内容，系统默认
    /// </summary>
    public class FileStore : IStore
    {
        #region IStore 成员

        private String innerpath;

        public FileStore()
        {
            // CreateCacheDir(_path);
        }

        private String _path = "cacheHtml/";

        /// <summary>
        /// 相对于站点根目录的缓存目录
        /// </summary>
        public String Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// 映射并创建缓存文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void CreateCacheDir(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        /// <summary>
        /// 获取真实路径
        /// </summary>
        private void GetRealPath()
        {
            if (innerpath == null)
            {
                innerpath = HttpRuntime.AppDomainAppPath + Path;
                CreateCacheDir(innerpath);
            }
        }

        private HashSet<string> CreatedDir = new HashSet<string>();

        private string GetPath(string key)
        {
            var dir = key.Substring(0, 2);
            string path = innerpath + '/' + dir + '/';
            if (!CreatedDir.Contains(dir))
            {
                CreateCacheDir(path);
                CreatedDir.Add(dir);
            }
            return path + key;
        }

        /// <summary>
        /// 保存缓存文件
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <param name="html">html内容</param>
        public void Save(string key, Stream repInfo)
        {
            using (var write = File.Create(GetPath(key)))
            {
                var stream = repInfo as MemoryStream;
                stream.WriteTo(write);
                //write.Write(stream.ToArray(), 0, (int)stream.Length);
            }
        }

        /// <summary>
        /// 根据key获取缓存内容
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <returns>Html内容</returns>
        public Stream Get(string key)
        {
            GetRealPath();
            return File.OpenRead(GetPath(key));
        }

        /// <summary>
        /// 根据key查询缓存信息
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <returns>Html</returns>
        public CacheInfo Query(string key)
        {
            GetRealPath();
            FileInfo fileinfo = new FileInfo(GetPath(key));
            if (fileinfo.Exists)
            {
                var info = new CacheInfo();
                info.StoreTime = fileinfo.LastWriteTime;
                info.Size = fileinfo.Length;
                return info;
            }
            return null;
        }

        #endregion
    }
}
