using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace StaticHtml
{
    public class SqlExpire : IExpire
    {


        private String _Sql;

        Regex valueReg = new Regex(@"GET\[(.+?)\]", RegexOptions.Compiled);

        public String Sql
        {
            get { return _Sql; }
            set
            {
                var matchs = valueReg.Matches(value);
                foreach (Match item in matchs)
                {
                    Values.Add(item.Groups[1].Value);
                }
                _Sql = value;
            }
        }

        private String ConnStr;

        public SqlExpire()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["SqlExpireConn"] != null)
            {
                ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlExpireConn"].ConnectionString;
            }
            else
            {
                LogHelp.Warn("SqlExpire config error: 没有数据库连接，staticHtml 会使用数据库连接：System.Configuration.ConfigurationManager.ConnectionStrings[\"SqlExpireConn\"]");
            }
        }



        private int _second = 2 * 600;

        public String Second
        {
            get { return _second.ToString(); }
            set
            {
                if (!Int32.TryParse(value, out _second))
                {
                    _second = 300;
                }
            }
        }

        /// <summary>
        /// 配置的sql里面的GET变量
        /// </summary>
        private IList<string> Values = new List<string>();


        /// <summary>
        /// 根据请求信息，生成实际的sql语句
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private String buildSql(System.Web.HttpRequest req, CacheInfo info)
        {
            String sql = Sql;
            foreach (var item in Values)
            {
                var value = req.QueryString[item];
                if (!String.IsNullOrEmpty(value))
                {
                    sql = sql.Replace("GET[" + item + "]", value);
                }
                else
                {
                    LogHelp.Warn("SqlExpire error:" + "配置了Get变量" + item + "，在Request.QueryString中确找不到");
                }
            }
            sql = sql.Replace("[STORETIME]", info.StoreTime.AddSeconds(3).ToString("yyyy-MM-dd HH:mm:ss"));
            sql = sql.Replace("[LENGTH]", info.Size.ToString());
            sql = sql.Replace("[KEY]", info.Key);
            return sql;
        }


        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetConn()
        {
            var conn = new SqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 根据请求及配置的sql语句，查询数据库是否有记录，如果有记录代表已过期，反之亦然
        /// </summary>
        /// <param name="req"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool innerSqlExpire(System.Web.HttpRequest req, CacheInfo info)
        {
            var sql = buildSql(req, info);
            var isExipre = false;
            if (!String.IsNullOrEmpty(ConnStr))
            {
                using (var conn = GetConn())
                {
                    var comm = conn.CreateCommand();
                    comm.CommandText = sql;
                    isExipre = comm.ExecuteReader().HasRows;
                }
            }
            return isExipre;
        }

        public bool IsExpire(System.Web.HttpRequest req, CacheInfo info)
        {
            return info == null || (DateTime.Now - info.StoreTime).TotalSeconds > _second || innerSqlExpire(req, info);
        }
    }
}
