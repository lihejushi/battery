using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;

namespace Battery.DAL.Sys
{
    public class ContentDAL : DBUtility
    {
        public static Tuple<int, List<Content>> GetList(int pageStart, int pageLength, string contentType, string title = "", int shopId = 0)
        {
            if (contentType == "4.1" && shopId == 0)
                throw new ArgumentException("参数无效", "shopId");

            string sql = string.Empty;

            switch (contentType)
            {
                case "2.1"://活动
                    sql = string.Format(@"
WITH _a AS
(
	SELECT [Id],[Name] AS Title,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowId
FROM [T_ComActivities] WHERE [OpenState]=1 AND [VerifyState]=1 {0} 
)
SELECT * FROM _a WHERE RowId BETWEEN @b AND @e;
SELECT COUNT(1) FROM [T_ComActivities] WHERE [OpenState]=1 AND [VerifyState]=1 {0}",
string.IsNullOrEmpty(title) == false ? " AND [Name] LIKE @Title" : "");
                    break;

                case "2.2"://资源   
                    sql = string.Format(@"
WITH _a AS
(
	SELECT [Id],[Name] AS Title,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowId
FROM [T_ComResources] WHERE [OpenState]=1 AND [VerifyState]=1 {0} 
)
SELECT * FROM _a WHERE RowId BETWEEN @b AND @e;
SELECT COUNT(1) FROM [T_ComResources] WHERE [OpenState]=1 AND [VerifyState]=1 {0}",
      string.IsNullOrEmpty(title) == false ? " AND [Name] LIKE @Title" : ""); 
                    break;
                default:
                    sql = "";
                    break;
            }

            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new { b = pageStart, e = pageStart + pageLength, ShopId = shopId, Title = "%" + title + "%" }))
                {
                    List<Content> list = multi.Read<Content>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();

                    return new Tuple<int, List<Content>>(records, list);
                }
            }
        }
    }
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}