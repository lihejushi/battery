using Battery.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data;
namespace Battery.DAL.Log
{
    public class SmsDAL : DBUtility
    {
        /// <summary>
        /// 记录短信
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool AddSmsLog(Sms_Log model,bool deleteOther)
        {
            using (SqlConnection conn = GetLogDb())
            {
                string sql = "";
                if (deleteOther) sql = "DELETE FROM Sms_Log WHERE MobileNo=@MobileNo AND SmsType=@SmsType;";
                sql += @"INSERT INTO Sms_Log(MobileNo,SendContent,LogTime,SendTime,[State],Interval,SmsType,VCode)
    VALUES(@MobileNo,@SendContent,@LogTime,@SendTime,@State,@Interval,@SmsType,@VCode);";
                return conn.Execute(sql, model) > 0;
            }
        }
        /// <summary>
        /// 获取同类型下最近发送的短信
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="smsType"></param>
        /// <returns></returns>
        public static Sms_Log GetSms(string mobileNo, int smsType)
        {
            using (SqlConnection conn = GetLogDb())
            {
                return conn.Query<Sms_Log>(@"SELECT TOP 1 * FROM Sms_Log sl WHERE sl.MobileNo=@MobileNo AND sl.SmsType=@SmsType ORDER BY SendTime DESC;", new { MobileNo = mobileNo, SmsType = smsType }).FirstOrDefault();
            }
        }
    }
}
