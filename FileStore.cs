using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Security;

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
            CreateCacheDir(_path);
        }

        private String _path="~/cacheHtml/";

        /// <summary>
        /// 相对于站点根目录的缓存目录
        /// </summary>
        public String Path
        {
            get { return _path; }
            set { _path = value; CreateCacheDir(_path); }
        }

        /// <summary>
        /// 映射并创建缓存文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void CreateCacheDir(string dir)
        {
            innerpath = HttpContext.Current.Server.MapPath(dir);
            DirectoryInfo dirInfo = new DirectoryInfo(innerpath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        /// <summary>
        /// 保存缓存文件
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <param name="html">html内容</param>
        public void Save(string key, string html)
        {
            GetRealPath();
            File.WriteAllText(innerpath + key, html);
        }

        /// <summary>
        /// 获取真实路径
        /// </summary>
        private void GetRealPath()
        {
            if (innerpath == null)
            {
                innerpath = HttpContext.Current.Server.MapPath(Path);
            }
        }

        /// <summary>
        /// 根据key获取缓存内容
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <returns>Html内容</returns>
        public string Get(string key)
        {
            GetRealPath();
            return File.ReadAllText(innerpath + key);
        }

        /// <summary>
        /// 根据key查询缓存信息
        /// </summary>
        /// <param name="key">HttpRequest唯一key</param>
        /// <returns>Html</returns>
        public HtmlInfo Query(string key)
        {
            GetRealPath();
            FileInfo fileinfo = new FileInfo((innerpath + key));
            if (fileinfo.Exists)
            {
                var info = new HtmlInfo();
                info.StoreTime = fileinfo.LastWriteTime;
                info.Size = fileinfo.Length;
                return info;
            }
            return null;
        }

        #endregion
    }
}
