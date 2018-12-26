using Battery.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;

namespace Battery.DAL.Log
{
    public class SysLogDAL : DBUtility
    {
        /// <summary>
        /// 添加系统后台用户操作
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static int AddPersonLog(Person_Log log)
        {
            string addSql = "INSERT INTO Person_Log(PersonId,Url,ActionType,LogTime,LogIP,Memo) VALUES(@PersonId,@Url,@ActionType,@LogTime,@LogIP,@Memo);";
            using (SqlConnection conn = GetLogDb())
            {
                return conn.Execute(addSql, log);
            }
        }

        /// <summary>
        /// 添加系统后台用户操作
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static int AddUserLog(User_Log log)
        {
            string addSql = "INSERT INTO UserLogs(RegID,OperatCode,OPerate,OperateTime,OperateResult,OperateIP) VALUES(@RegID,@OperatCode,@OPerate,@OperateTime,@OperateResult,@OperateIP);";
            using (SqlConnection conn = GetLogDb())
            {
                return conn.Execute(addSql, log);
            }
        }
    }
}
