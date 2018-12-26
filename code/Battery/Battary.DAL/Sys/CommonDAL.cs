using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Battery.Model.X;
using Battery.Model.Battery;

namespace Battery.DAL.Sys
{
    public class CommonDAL : DBUtility
    {
        public static List<Sys_Data> GetSys_DataList(string key, string typeCode, int state)
        {
            string sqlwhere = "";
            if (key != "")
            {
                sqlwhere += " AND ([Key] =" + typeCode + ") ";
            }
            if (typeCode != "")
            {
                sqlwhere += " AND ([TypeCode] = '" + typeCode + "')";
            }

            if (state != -100)
            {
                sqlwhere += " AND [State]=" + state.ToString();
            }

            string sql = string.Format("SELECT ID,[Name],[Key],[Value],[TypeCode],State FROM Sys_Data WHERE 1=1 {0} ORDER BY [Sort] ", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {                 
                }))
                {
                    var list = multi.Read<Sys_Data>().ToList();
                    return list;
                }
            }
        }
    }
}
