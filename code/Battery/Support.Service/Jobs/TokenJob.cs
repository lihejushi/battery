using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Support.Service.Models;
using Support.Service.Tools;
using Dapper;
using System.Data;
using System.Threading;
using Senparc.Weixin.MP.Entities;

namespace Support.Service.Jobs
{
    public class TokenJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Logger logger = LogManager.GetLogger("TokenJob");
            try
            {
                object _model = context.JobDetail.JobDataMap["Model"];
                if (_model != null)
                {
                    while (true)
                    {
                        var model = (TokenConfig)_model;
                        if (model.TokenType == "client_credential")//获取AccessToken
                        {
                            var tokenResult = GetToken(model, "client_credential");
                            if(string.IsNullOrEmpty( tokenResult.access_token) == false)
                            {
                                UpdateToken(model.DbConnectionStr, model.AppId, tokenResult.access_token, "client_credential", tokenResult.expires_in, DateTime.Now.AddSeconds(tokenResult.expires_in), DateTime.Now);
                                break;
                            }
                            else
                            {
                                Thread.Sleep(2 * 1000);
                            }
                        }
                        else if (model.TokenType == "jsapi")//获取jsapi ticket
                        {
                            var tokenResult = GetTicketByAccessToken(model, "jsapi");
                            if (string.IsNullOrEmpty(tokenResult.ticket) == false)
                            {
                                UpdateToken(model.DbConnectionStr, model.AppId, tokenResult.ticket, "jsapi", tokenResult.expires_in, DateTime.Now.AddSeconds(tokenResult.expires_in), DateTime.Now);
                                break;
                            }
                            else
                            {
                                Thread.Sleep(2 * 1000);
                            }
                        }
                        else if (model.TokenType == "wx_card")//获取wx_card ticket
                        {
                            var tokenResult = GetTicketByAccessToken(model, "wx_card");
                            if (string.IsNullOrEmpty(tokenResult.ticket) == false)
                            {
                                UpdateToken(model.DbConnectionStr, model.AppId, tokenResult.ticket, "wx_card", tokenResult.expires_in, DateTime.Now.AddSeconds(tokenResult.expires_in), DateTime.Now);
                                break;
                            }
                            else
                            {
                                Thread.Sleep(2 * 1000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TokenJob：" + ex.Message);
            }
        }

        public static bool AccessTokenIsError(int code)
        {
            return code == 40001 || code == 40014 || code == 41001 || code == 42001;
        }

        public static ApiTicketResult GetTicketByAccessToken(TokenConfig model, string type = "jsapi")
        {
            Logger logger = LogManager.GetLogger("TokenJob");
            string accessToken = GetAccessToken(model.DbConnectionStr, model.AppId, "client_credential");
            if (string.IsNullOrEmpty(accessToken) == false)
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type={1}", accessToken, type);
                logger.Trace(url);
                string jsonResult = RequestUtility.HttpGet(url, Encoding.UTF8);
                logger.Trace(jsonResult);
                ApiTicketResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiTicketResult>(jsonResult);
                if (AccessTokenIsError(result.errcode))
                {
                    return GetTicketByAccessToken(model, type);
                }
                return result;
            }
            else
            {
                Thread.Sleep(2 * 1000);//若无有效的assesstoken则休眠2秒，再次执行
                return GetTicketByAccessToken(model, type);
            }
        }
        public static AccessTokenResult GetToken(TokenConfig model, string grant_type = "client_credential")
        {
            Logger logger = LogManager.GetLogger("TokenJob");
            try
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}",
                                        grant_type, model.AppId, model.AppSecretKey);
                logger.Trace(url);
                string jsonResult = RequestUtility.HttpGet(url, Encoding.UTF8);
                logger.Trace(jsonResult);
                AccessTokenResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenResult>(jsonResult);
                return result;
            }
            catch(Exception ex)
            {
                logger.Error("TokenJob：" + ex.Message);
                return GetToken(model, grant_type);
            }
        }
        public static bool UpdateToken(string connStr, string appId, string token, string tokenType,int expireIn,DateTime expireTime,DateTime updateTime)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                return conn.Execute(@"IF(NOT EXISTS(SELECT 1 FROM Sys_Wx_Center swc WHERE swc.AppId =@AppId AND swc.TokenType=@TokenType))
BEGIN
	INSERT INTO Sys_Wx_Center(AppId,Token,TokenType,ExpireIn,ExpireTime,UpdateTime)
	VALUES(@AppId,@Token,@TokenType,@ExpireIn,@ExpireTime,@UpdateTime);
END
ELSE
BEGIN
	UPDATE Sys_Wx_Center SET Token = @Token,ExpireIn = @ExpireIn,ExpireTime = @ExpireTime,UpdateTime = @UpdateTime WHERE AppId = @AppId AND TokenType = @TokenType;	
END;", new { AppId = appId, Token = token, tokenType = tokenType, ExpireIn = expireIn, ExpireTime = expireTime, UpdateTime = updateTime }) > 0;
            }
        }
        public static string GetAccessToken(string connStr, string appId, string tokenType)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                return conn.Query<string>(@"SELECT swc.Token FROM Sys_Wx_Center swc WHERE swc.AppId=@AppId AND swc.TokenType=@TokenType AND swc.ExpireTime > GETDATE();", new { AppId = appId, tokenType = tokenType }).FirstOrDefault();
            }
        }
    }
}
