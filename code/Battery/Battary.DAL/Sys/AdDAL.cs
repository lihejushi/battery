using Battery.Model.Sys;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace Battery.DAL.Sys
{
    public class AdDAL : DBUtility
    {
        /// <summary>
        /// 编辑/添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool EditAd(Sys_Ad model)
        {
            string sql = "INSERT INTO [Sys_Ad]([AdType],[Title],[ImgUrl],[Desc],[NavJson],[Url]) VALUES (@AdType,@Title,@ImgUrl,@Desc,@NavJson,@Url)";
            if (model.Id > 0)
                sql = "UPDATE [Sys_Ad] SET [Title]=@Title,[ImgUrl]=@ImgUrl,[Desc]=@Desc,[NavJson]=@NavJson,[Url]=@Url WHERE [Id]=@Id";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(sql, model) > 0;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Del(int id)
        {
            string sql = "DELETE FROM [Sys_Ad] WHERE [Id]=@Id;";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(sql, new { Id = id }) > 0;
            }
        }

        /// <summary>
        /// 获取实体（根据类型获取）
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        public static Sys_Ad GetModelByAdType(int adType)
        {
            string sql = "SELECT * FROM [Sys_Ad] WHERE [AdType]=@AdType ORDER BY [Id] ASC;";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Ad>(sql, new { AdType = adType }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取实体（根据类型获取）
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        public static IEnumerable<Sys_Ad> GetListByAdType(int adType)
        {
            string sql = "SELECT * FROM [Sys_Ad] WHERE [AdType]=@AdType ORDER BY [Id] ASC;";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Ad>(sql, new { AdType = adType });
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Sys_Ad GetModel(int id)
        {
            string sql = "SELECT * FROM [Sys_Ad] WHERE [Id]=@Id;";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Ad>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageLength"></param>
        /// <param name="AdType">广告位位置  -1全部 1首页轮播图</param>
        /// <returns></returns>
        public static Tuple<int, List<Sys_Ad>> GetList(int pageStart, int pageLength, int AdType = -1)
        {
            string sqlwhere = "";
            if (AdType != -1)
            {
                sqlwhere = " AND AdType=" + AdType;
            }
            string sql = @"
WITH _a AS
(
	SELECT *,ROW_NUMBER() OVER(ORDER BY AdType DESC, [Id] DESC) AS RowId FROM [Sys_Ad] WHERE 1=1 {0}
)
SELECT * FROM _a WHERE RowId BETWEEN @b AND @e;
SELECT COUNT(1) FROM [Sys_Ad] WHERE 1=1 {0};";

            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(String.Format(sql, sqlwhere), new { b = pageStart, e = pageStart + pageLength }))
                {
                    List<Sys_Ad> list = multi.Read<Sys_Ad>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();

                    return new Tuple<int, List<Sys_Ad>>(records, list);
                }
            }
        }

        /// <summary>
        /// 广告类型
        /// </summary>
        /// <returns></returns>
        public static List<AdType> GetAdTypeList()
        {
            List<AdType> list = new List<AdType>();
            list.Add(new AdType { Key = 1, Value = "首页轮播图" });
            return list;
        }
    }
}
