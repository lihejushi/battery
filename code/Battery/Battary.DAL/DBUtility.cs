using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Battery.DAL
{
    public class DBUtility
    {
        protected static SqlConnection GetMainDb()
        {
            return GetSqlConnection("MainDb");
        }

        protected static SqlConnection GetLogDb()
        {
            return GetSqlConnection("MainDb");
        }
        protected static SqlConnection GetActivityDb()
        {
            return GetSqlConnection("ActivityDb");
        }
        /// <summary>
        /// 替换sql语句中的表名
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        internal static string GetSql(string strSql)
        {
            return System.Text.RegularExpressions.Regex.Replace(strSql, "(?<tableName>#([^#]*?)#)", "$1");
        }

        internal static string GetTable(string tableName)
        {
            return tableName;
        }
        protected static SqlConnection GetSqlConnection(string dbName = "MainDb")
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

                return GetConnection(connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected static SqlConnection GetConnection(string ConnectionStr)
        {
            SqlConnection _Connection = null;
            try
            {
                var conn = ConnectionStr;
                if (conn != null && string.IsNullOrEmpty(conn) == false)
                    _Connection = new SqlConnection(conn);


                _Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Connection;
        }

        /// <summary>
        /// 替换sql语句中的表名
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        //internal static string GetSql(string strSql)
        //{
        //    return System.Text.RegularExpressions.Regex.Replace(strSql, "(?<tableName>#([^#]*?)#)", "$1");
        //}

        //internal static string GetTable(string tableName)
        //{
        //    return tableName;
        //}
        protected static void AddInParam(SqlCommand cmd, string name, SqlDbType sqldbtype, object value)
        {
            SqlParameter dp = cmd.CreateParameter();
            dp.SqlDbType = sqldbtype;
            dp.Value = value;
            dp.ParameterName = name;
            dp.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dp);
        }
        protected static void AddOutParam(SqlCommand cmd, string name, SqlDbType sqldbtype, int size)
        {
            SqlParameter dp = cmd.CreateParameter();
            dp.SqlDbType = sqldbtype;
            dp.ParameterName = name;
            dp.Size = size;
            dp.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dp);
        }
        protected static SqlParameter AddInParam(string name, SqlDbType sqldbtype, object value)
        {
            SqlParameter dp = new SqlParameter();
            dp.SqlDbType = sqldbtype;
            dp.Value = value;
            dp.ParameterName = name;
            dp.Direction = ParameterDirection.Input;
            return dp;
        }
        protected static SqlParameter AddOutParam(string name, SqlDbType sqldbtype, int size)
        {
            SqlParameter dp = new SqlParameter();
            dp.SqlDbType = sqldbtype;
            dp.ParameterName = name;
            dp.Size = size;
            dp.Direction = ParameterDirection.Output;
            return dp;
        }
    }
}
