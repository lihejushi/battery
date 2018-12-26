using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System;

namespace Battery.DAL.Sys
{
    public class WxCenterDAL : DBUtility
    {
        public static string Get(string appId, string type)
        {
            string sql = "SELECT swc.Token FROM Sys_Wx_Center swc WHERE swc.AppId=@AppId AND swc.TokenType=@TokenType AND swc.ExpireTime > GETDATE();";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<string>(sql, new { AppId = appId, tokenType = type }).FirstOrDefault();
            }
        }
        public static bool UpdateToken(string appId, string token, string tokenType, int expireIn, DateTime expireTime, DateTime updateTime)
        {
            string sqlStr = @"IF(NOT EXISTS(SELECT 1 FROM Sys_Wx_Center swc WHERE swc.AppId =@AppId AND swc.TokenType=@TokenType))
BEGIN
    INSERT INTO Sys_Wx_Center(AppId, Token, TokenType, ExpireIn, ExpireTime, UpdateTime)

    VALUES(@AppId, @Token, @TokenType, @ExpireIn, @ExpireTime, @UpdateTime);
            END
            ELSE
BEGIN
    UPDATE Sys_Wx_Center SET Token = @Token,ExpireIn = @ExpireIn,ExpireTime = @ExpireTime,UpdateTime = @UpdateTime WHERE AppId = @AppId AND TokenType = @TokenType;
            END; ";
            using (SqlConnection conn = GetSqlConnection())
            { 
                return conn.Execute(sqlStr, new { AppId = appId, Token = token, tokenType = tokenType, ExpireIn = expireIn, ExpireTime = expireTime, UpdateTime = updateTime }) > 0;
            }
        }
    }
}
