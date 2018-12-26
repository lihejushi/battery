using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using TuoKe.Services.Models;
using Dapper;
using static TuoKe.Services.Models.API_Notify_List;
using System.Data;
using static TuoKe.Services.Models.API_Notify_List_Push;
using NLog;

namespace TuoKe.Services.Tools
{
    public class Dal
    {
        
        //private SynchronizationsProductInfo model = ConfigHelper.GetSynchronizationsProductInfoConfig();
        #region 数据库连接
        protected static SqlConnection GetSqlConnection()
        {
            try
            {
                var model = ConfigHelper.GetSynchronizationsProductInfoConfig();
                string connection = model.DbConnectionStr;
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
        #endregion

        #region 推送
        public static List<API_Notify> GetNotify()
        {
            string sql = "SELECT Id,[KEY],IV,CompanyUrl,AllData,SendCount FROM API_Notify sd WHERE sd.State=0";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql))
                {
                    List<API_Notify> list = multi.Read<API_Notify>().ToList();
                    return list;
                }
            }
        }


        public static int AfterNotify(string sqlstring)
        {
            //string sql = @"
            //                BEGIN TRAN
            //                BEGIN TRY
            //            ";

            //sql += sqlstring;
            //sql += @"    
            //                SELECT @@ROWCOUNT;
            //                COMMIT TRAN
            //              END
            //              ELSE
            //              BEGIN
            //                     SELECT -1;
            //                     ROLLBACK TRAN 
            //              END
                           
            //            END TRY
            //            BEGIN CATCH
            //                SELECT 0;
            //                ROLLBACK TRAN 
            //            END CATCH 
            //            ";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(sqlstring);
            }
        }

        #endregion


        #region 对应接口2.4
        public static List<string> GetAllFile()
        {
            string sqlStr = "SELECT Batch FROM API_Accept_Log WHERE  DealWithCase=2 AND ErrorCode=0 AND [State]=0;";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<string>(sqlStr).ToList();
            }
        }


        public static T_Company GetCompanyModel(string rno)
        {
            string sqlStr = "SELECT top 1 * FROM T_Company WHERE  Id=(select top 1 CompanyId from T_ComResources where RNo=@RNo) AND VerifyState != -1";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<T_Company>(sqlStr, new { RNo = rno }).FirstOrDefault();
            }
        }


        //获取活动相关的信息
        public static List<API_Notify_List.T_CompanyMore> GetCompanyModelByActionNO(string anos)
        {
            string sqlStr = "SELECT c.Id,c.DesKey,c.DesIV,a.PushUrl,a.ANo FROM T_Company c,T_ComActivities a WHERE  c.Id=a.CompanyId AND c.VerifyState != -1 AND a.ANo IN (" + anos + ")";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<API_Notify_List.T_CompanyMore>(sqlStr).ToList();
            }
        }

        //批量写入资源领取Log
        public static int BatchAdd(List<API_Notify_List.ResourcesList> list_rc, string no)
        {
            Logger logger = LogManager.GetLogger("Service");
            int r = 1;
            //System.Data.SqlClient.SqlBulkCopy bulkCopy = new SqlBulkCopy(GetSqlConnection());
            //bulkCopy.DestinationTableName = "T_ResourceReceiveLog";

            DataTable dt = new DataTable();
            dt.Columns.Add("Id", Type.GetType("System.Int32"));
            dt.Columns.Add("RNo", Type.GetType("System.String"));
            dt.Columns.Add("ValidCode", Type.GetType("System.String"));
            dt.Columns.Add("ReceiveMobile", Type.GetType("System.String"));
            dt.Columns.Add("ReceiveTel", Type.GetType("System.String"));
            dt.Columns.Add("ReceiveName", Type.GetType("System.String"));
            dt.Columns.Add("ReceiveAddress", Type.GetType("System.String"));
            dt.Columns.Add("AddTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidState", Type.GetType("System.Int32"));
            dt.Columns.Add("Remark", Type.GetType("System.String"));
            dt.Columns.Add("ANo", Type.GetType("System.String"));
            dt.Columns.Add("OpenId", Type.GetType("System.String"));
            dt.Columns.Add("CustomContent", Type.GetType("System.String"));
            dt.Columns.Add("ProductName", Type.GetType("System.String"));
            dt.Columns.Add("ProductDesc", Type.GetType("System.String"));

            int times = 0; int timesPer = 0;
            if (list_rc.Count > 10000)
            {
                times = Convert.ToInt32(Math.Floor((double)(list_rc.Count / 10000)));
            }
            for (int t = 0; t < times + 1; t++)
            {
                if (t < times)
                {
                    timesPer = 10000;
                }
                else
                {
                    timesPer = list_rc.Count - 10000 * times;
                }
                Object thisLock = new Object();
                Parallel.For(0, timesPer, jj => {
                    lock (thisLock)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Id"] = 0;
                        dr["RNo"] = no;
                        dr["ValidCode"] = list_rc[t * 10000 + jj].tkContent.VerifyNo;
                        dr["ReceiveMobile"] = list_rc[t * 10000 + jj].bizContent.MobileNo;
                        dr["ReceiveTel"] = "";
                        dr["ReceiveName"] = "";
                        dr["ReceiveAddress"] = "";
                        dr["AddTime"] = DateTime.Now;
                        dr["ValidState"] = list_rc[t * 10000 + jj].bizContent.NodeCode;
                        dr["Remark"] = list_rc[t * 10000 + jj].bizContent.NodeMsg;
                        dr["ANo"] = list_rc[t * 10000 + jj].tkContent.ActiveNo;
                        dr["OpenId"] = "";
                        dr["CustomContent"] = "";
                        dr["ProductName"] = list_rc[t * 10000 + jj].bizContent.ProductName;
                        dr["ProductDesc"] = list_rc[t * 10000 + jj].bizContent.ProductDesc;

                        dt.Rows.Add(dr);
                    }

                });

                //for (int i = 0; i < timesPer; i++)
                //{
                //    DataRow dr = dt.NewRow();
                //    dr["Id"] = 0;
                //    dr["RNo"] = no;
                //    dr["ValidCode"] = list_rc[t * 10000 + i].tkContent.VerifyNo;
                //    dr["ReceiveMobile"] = list_rc[t * 10000 + i].bizContent.MobileNo;
                //    dr["ReceiveTel"] = "";
                //    dr["ReceiveName"] = "";
                //    dr["ReceiveAddress"] = "";
                //    dr["AddTime"] = DateTime.Now;
                //    dr["ValidState"] = list_rc[t * 10000 + i].bizContent.NodeCode;
                //    dr["Remark"] = list_rc[t * 10000 + i].bizContent.NodeMsg;
                //    dr["ANo"] = list_rc[t * 10000 + i].tkContent.ActiveNo;
                //    dr["OpenId"] = "";
                //    dr["CustomContent"] = "";
                //    dr["ProductName"] = list_rc[t * 10000 + i].bizContent.ProductName;
                //    dr["ProductDesc"] = list_rc[t * 10000 + i].bizContent.ProductDesc;
                //    dt.Rows.Add(dr);
                //}
                SqlConnection conn = GetSqlConnection();
                SqlTransaction sqlbulkTransaction = null; System.Data.SqlClient.SqlBulkCopy bulkCopy = null;
                //logger.Debug("1");
                try
                {
                    //logger.Debug("2");

                    if(conn.State== ConnectionState.Closed)
                        conn.Open();
                    //logger.Debug("3");
                    sqlbulkTransaction = conn.BeginTransaction();
                    bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, sqlbulkTransaction);//在插入数据的同时检查约束，如果发生错误调用sqlbulkTransaction事务
                    bulkCopy.DestinationTableName = "T_ResourceReceiveLog";

                    bulkCopy.WriteToServer(dt);
                    sqlbulkTransaction.Commit();
                    bulkCopy.Close();
                    conn.Close();
                    //logger.Debug("4");
                    if (t < times)
                    {
                        r = t * 10000;
                    }
                    else
                    {
                        r = t * 10000 + timesPer;
                    }
                    //logger.Debug("5");
                }
                catch (Exception ex)
                {
                    
                   // logger.Debug(ex.ToString());
                    sqlbulkTransaction.Rollback();
                }
                finally
                {
                    bulkCopy.Close();
                    conn.Close();
                }
            }

            return r;
        }


        //批量写入Api通知表
        public static int BatchAddNotify(List<API_Notify> an)
        {
            int r = 0;
            //System.Data.SqlClient.SqlBulkCopy bulkCopy = new SqlBulkCopy(GetSqlConnection());
            //bulkCopy.DestinationTableName = "T_ResourceReceiveLog";

            DataTable dt = new DataTable();
            dt.Columns.Add("Id", Type.GetType("System.Int32"));
            dt.Columns.Add("CompanyId", Type.GetType("System.String"));
            dt.Columns.Add("KEY", Type.GetType("System.String"));
            dt.Columns.Add("IV", Type.GetType("System.String"));
            dt.Columns.Add("CompanyUrl", Type.GetType("System.String"));
            dt.Columns.Add("AllData", Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("SendTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("SendCount", Type.GetType("System.Int32"));
            dt.Columns.Add("Reponse", Type.GetType("System.String"));
            dt.Columns.Add("ReponseTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("State", Type.GetType("System.Int32"));


            int times = 0; int timesPer = 0;
            if (an.Count > 5000)
            {
                times = Convert.ToInt32(Math.Floor((double)(an.Count / 5000)));
            }
            for (int t = 0; t < times + 1; t++)
            {
                if (t < times)
                {
                    timesPer = 5000;
                }
                else
                {
                    timesPer = an.Count - 5000 * times;
                }

                Object thisLock = new Object();
                Parallel.For(0, timesPer, jj1 =>
                {
                    lock (thisLock)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Id"] = 0;
                        dr["CompanyId"] = an[t * 5000 + jj1].CompanyId;
                        dr["KEY"] = an[t * 5000 + jj1].KEY;
                        dr["IV"] = an[t * 5000 + jj1].IV;
                        dr["CompanyUrl"] = an[t * 5000 + jj1].CompanyUrl;
                        dr["AllData"] = an[t * 5000 + jj1].AllData;
                        dr["CreateTime"] = DateTime.Now;
                        dr["SendTime"] = DateTime.Now;
                        dr["SendCount"] = 0;
                        dr["Reponse"] = "";
                        dr["ReponseTime"] = DBNull.Value;
                        dr["State"] = 0;

                        dt.Rows.Add(dr);
                    }
                });
                //for (int i = 0; i < timesPer; i++)
                //{
                //    DataRow dr = dt.NewRow();
                //    dr["Id"] = 0;
                //    dr["CompanyId"] = an[t * 5000 + i].CompanyId;
                //    dr["KEY"] = an[t * 5000 + i].KEY;
                //    dr["IV"] = an[t * 5000 + i].IV;
                //    dr["CompanyUrl"] = an[t * 5000 + i].CompanyUrl;
                //    dr["AllData"] = an[t * 5000 + i].AllData;
                //    dr["CreateTime"] = DateTime.Now;
                //    dr["SendTime"] = DateTime.Now;
                //    dr["SendCount"] = 0;
                //    dr["Reponse"] = "";
                //    dr["ReponseTime"]= DateTime.Now;
                //    dr["State"] = 0;
                //    dt.Rows.Add(dr);
                //}
                SqlConnection conn = conn = GetSqlConnection();
                SqlTransaction sqlbulkTransaction = null;
                System.Data.SqlClient.SqlBulkCopy bulkCopy = null;
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    sqlbulkTransaction = conn.BeginTransaction();
                    bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, sqlbulkTransaction);//在插入数据的同时检查约束，如果发生错误调用sqlbulkTransaction事务
                    bulkCopy.DestinationTableName = "API_Notify";
                    bulkCopy.WriteToServer(dt);
                    sqlbulkTransaction.Commit();
                    if (t < times)
                    {
                        r = t * 5000;
                    }
                    else
                    {
                        r = t * 5000 + timesPer;
                    }
                }
                catch (Exception ex)
                {
                    sqlbulkTransaction.Rollback();
                }
                finally
                {
                    bulkCopy.Close();
                    conn.Close();
                }
            }

            return r;

        }


        //获取批次详情
        public static API_Accept_Log GetAPI_Accept_LogModel(string batch)
        {
            string sql = @"select TOP 1 * FROM API_Accept_Log WHERE [State]=1 AND Batch='" + batch + "'; ";
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<API_Accept_Log>(sql).FirstOrDefault();
            }
        }


        public static int AddAPI_Accept_Log(API_Notify_List_Push.API_Accept_Log aal)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                string sql = @"
                              DECLARE @H INT;
                              DECLARE @R INT;
                              SELECT @H=COUNT(Batch) FROM API_Accept_Log WHERE Batch='" + aal.Batch + @"';
                              IF(@H<1)
                              BEGIN
                                INSERT INTO API_Accept_Log(Batch,AcceptTime,DealWithCase,ErrorCode,Count,DealWithTime,State,Remark,AcceptAllDate) VALUES(@Batch,@AcceptTime,@DealWithCase,@ErrorCode,@Count,@DealWithTime,@State,@Remark,@AcceptAllDate);
                                SET @R=@@ROWCOUNT;
                              END
                              ELSE
                              BEGIN
                                SET @R=-6;
                              END
                              SELECT @R;
                              ";
                return conn.Query<int>(sql, aal).FirstOrDefault();
            }
        }


        public static int UpdateAPI_Accept_Log(string batch, int errorCode, int state)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                string sql = "UPDATE API_Accept_Log SET ErrorCode=@ErrorCode,State=@State WHERE Batch =@Batch;";
                return conn.Execute(sql, new { @ErrorCode = errorCode, @State = state, @Batch = batch });
            }
        }
        #endregion
    }
}
