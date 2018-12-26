using Battery.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace Battery.DAL.Sys
{
    public class DictDAL : DBUtility
    {
        /// <summary>
        /// 获取系统参数列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isSys"></param>
        /// <returns></returns>
//        public static Tuple<int, List<Sys_Dict>> GetDcitList(int pageIndex, int pageSize, int? isSys = 0)
//        {
//            string sqlwhere = "";
//            if (isSys.HasValue)
//            {
//                sqlwhere += " AND sp.IsSys = " + isSys;
//            }

//            string sql = string.Format(@"WITH _a AS
//            (
//                SELECT *,ROW_NUMBER() OVER(ORDER BY sp.GroupCode,sp.SortNo asc) AS RowID FROM Sys_Dict sp WHERE 1=1 {0} 
//            )
//            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
//            SELECT COUNT(1) FROM Sys_Dict sp WHERE 1=1 {0};", sqlwhere);
//            using (SqlConnection conn = GetSqlConnection())
//            {
//                using (var multi = conn.QueryMultiple(sql, new
//                {
//                    b = (pageIndex - 1) * pageSize + 1,
//                    e = pageIndex * pageSize,
//                }))
//                {
//                    var list = multi.Read<Sys_Dict>().ToList();
//                    int records = multi.Read<int>().FirstOrDefault();
//                    return new Tuple<int, List<Sys_Dict>>(records, list);
//                }
//            }
//        }

        public static Sys_Dict GetDict(int id)
        {
            string sql = "SELECT * FROM Sys_Dict sd WHERE sd.Id=@ParamId";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Dict>(sql, new { ParamId = id }).FirstOrDefault();
            }
        }
        public static Sys_Dict GetDict(string groupCode, string configCode)
        {
            string sql = "SELECT * FROM Sys_Dict sd WHERE sd.GroupCode=@GroupCode AND sd.ConfigCode=@ConfigCode";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Dict>(sql, new { GroupCode = groupCode, ConfigCode = configCode }).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public static string GetDictValue(string groupCode, string configCode)
        {
            string sql = "SELECT ConfigValue FROM Sys_Dict sd WHERE sd.GroupCode=@GroupCode AND sd.ConfigCode=@ConfigCode";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<string>(sql, new { GroupCode = groupCode, ConfigCode = configCode }).FirstOrDefault() ?? "";
            }
        }

        /// <summary>
        /// 获取分组参数
        /// </summary>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public static List<Sys_Dict> GetDicts(string groupCode)
        {
            string sql = "SELECT * FROM Sys_Dict sd WHERE sd.GroupCode=@GroupCode ORDER By sd.SortNo ASC";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Dict>(sql, new { GroupCode = groupCode }).ToList();
            }
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SaveDict(Sys_Dict model)
        {
            string sql = @"INSERT INTO Sys_Dict(GroupCode,ConfigCode,ConfigValue,SortNo,IsSys)
VALUES(@GroupCode ,@ConfigCode ,@ConfigValue ,@SortNo,@IsSys)";
            if (model.Id > 0)
            {
                sql = "UPDATE Sys_Dict SET GroupCode = @GroupCode,ConfigCode = @ConfigCode,ConfigValue = @ConfigValue,SortNo = @SortNo WHERE Id=@Id;";
            }
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(GetSql(sql), model) > 0;
            }
        }

        public static bool SaveDictByCode(Sys_Dict model)
        {
            string sql = @"IF(NOT EXISTS (SELECT 1 FROM Sys_Dict sd WHERE sd.GroupCode=@GroupCode AND sd.ConfigCode=@ConfigCode))
BEGIN
	INSERT INTO Sys_Dict(GroupCode,ConfigCode,ConfigValue,SortNo,IsSys) VALUES(@GroupCode,@ConfigCode,@ConfigValue,@SortNo,@IsSys);
END
ELSE
BEGIN
	UPDATE Sys_Dict SET ConfigValue = @ConfigValue,SortNo = @SortNo,IsSys = @IsSys WHERE GroupCode=@GroupCode AND ConfigCode=@ConfigCode;
END";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(GetSql(sql), model) > 0;
            }
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteDict(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("DELETE FROM Sys_Dict WHERE Id=@Id AND IsSys=0", new { Id = id }) > 0;
            }
        }

        public static List<NameValue> GetGobalConfig()
        {
            string sql = "SELECT sgc.ConfigKey AS NAME, sgc.ConfigValue AS Value FROM Sys_GlobalConfig sgc";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<NameValue>(sql).ToList();
            }
        }

        public static void InsertGobalConfig(string name, string value)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Execute("INSERT INTO Sys_GlobalConfig(ConfigKey,ConfigValue) VALUES(@ConfigKey,@ConfigValue)", new { ConfigKey = name, ConfigValue = value });
            }
        }
    }
}
