using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Battery.Model.X;

namespace Battery.DAL.X
{
    public class ActiveAndPointDAL : DBUtility
    {
        /// <summary>
        /// 获取所有可用活动
        /// </summary>
        /// <returns></returns>
        public static List<Actives> GetAllActive()
        {
            string sql = string.Format(@"SELECT * FROM Actives ORDER BY ID DESC");
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {

                }))
                {
                    var list = multi.Read<Actives>().ToList();
                    return new List<Actives>(list);
                }
            }
        }

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="activename"></param>
        /// <param name="sTime"></param>
        /// <param name="eTime"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Tuple<int, List<Actives>> GetActivesList(int pageStart, int pageLength, string activename, string sTime, string eTime, int state = -100)
        {
            string sqlwhere = "";
            if (!string.IsNullOrEmpty(activename))
            {
                sqlwhere += " AND ActiveName LIKE '%" + activename + "%'";
            }
            if (!string.IsNullOrEmpty(sTime))
            {
                sqlwhere += " AND AddTime >= '" + sTime + "'";
            }
            if (!string.IsNullOrEmpty(eTime))
            {
                sqlwhere += " AND AddTime <= '" + eTime + "'";
            }
            if (state != -100)
            {
                sqlwhere += " AND [State]=" + state;
            }

            string sql = string.Format(@"
            WITH _a AS
            (
                SELECT *,CONVERT(VARCHAR(100), ContributeStartTime, 23) AS ContributeStartTimeT,CONVERT(VARCHAR(100), ContributeEndTime, 23) AS ContributeEndTimeT,
                CONVERT(VARCHAR(100), StartTime, 23) AS StartTimeT,CONVERT(VARCHAR(100), EndTime, 23) AS EndTimeT,CONVERT(VARCHAR(100), AddTime, 23) AS AddTimeT,
                (SELECT COUNT(1) FROM ActivesDocument WHERE ActiveID=Actives.ID) AS DocumentTotal,
                (SELECT COUNT(1) FROM ActivesPic WHERE ActiveID=Actives.ID) AS PicTotal,
                ROW_NUMBER() OVER(ORDER BY [State] DESC,AddTime DESC) AS RowID FROM Actives  WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Actives  WHERE 1=1 {0};
            SELECT ActiveID,(select RegID from UserDocument where ID=ActivesDocument.DocumentID) FROM ActivesDocument;
            SELECT ActiveID,(select RegID from UserPics where ID=ActivesPic.PicID) FROM ActivesPic;", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                }))
                {
                    var list = multi.Read<Actives>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    var listd = multi.Read<forUserCount>().ToList();
                    var listp = multi.Read<forUserCount>().ToList();

                    foreach (Actives item in list)
                    {
                        List<int> lint = new List<int>();
                        if (listd != null && listd.Count > 0)
                        {
                            List<forUserCount> gd = listd.FindAll(d => d.ActiveID == item.ID);
                            {
                                if (gd != null && gd.Count > 0)
                                {
                                    foreach (forUserCount fucitem in gd)
                                    {
                                        lint.Add(fucitem.RegID);
                                    }
                                }
                            }
                        }
                        if (listp != null && listp.Count > 0)
                        {
                            List<forUserCount> gp = listp.FindAll(p => p.ActiveID == item.ID);
                            {
                                if (gp != null && gp.Count > 0)
                                {
                                    foreach (forUserCount fucitem in gp)
                                    {
                                        lint.Add(fucitem.RegID);
                                    }
                                }
                            }
                        }
                        item.UsersTotal = lint.Distinct().ToList().Count;
                    }
                    return new Tuple<int, List<Actives>>(records, list);
                }
            }
        }

        /// <summary>
        /// 获取活动情况model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Actives GetActivesModel(int id)
        {
            string sql = @"SELECT *,CONVERT(VARCHAR(100), ContributeStartTime, 23) AS ContributeStartTimeT,CONVERT(VARCHAR(100), ContributeEndTime, 23) AS ContributeEndTimeT,
                CONVERT(VARCHAR(100), StartTime, 23) AS StartTimeT,CONVERT(VARCHAR(100), EndTime, 23) AS EndTimeT,CONVERT(VARCHAR(100), AddTime, 23) AS AddTimeT,
                (SELECT COUNT(1) FROM ActivesDocument WHERE ActiveID=@Id) AS DocumentTotal,
                (SELECT COUNT(1) FROM ActivesPic WHERE ActiveID=@Id) AS PicTotal FROM Actives WHERE ID=@Id;
                SELECT ActiveID,(select RegID from UserDocument where ID=ActivesDocument.DocumentID) FROM ActivesDocument WHERE ActiveID=@Id;
                SELECT ActiveID,(select RegID from UserPics where ID=ActivesPic.PicID) FROM ActivesPic WHERE ActiveID=@Id;
                ";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id
                }))
                {
                    var model = multi.Read<Actives>().FirstOrDefault();
                    var listd = multi.Read<forUserCount>().ToList();
                    var listp = multi.Read<forUserCount>().ToList();

                    List<int> lint = new List<int>();
                    if (listd != null && listd.Count > 0)
                    {
                        List<forUserCount> gd = listd.FindAll(d => d.ActiveID == model.ID);
                        {
                            if (gd != null && gd.Count > 0)
                            {
                                foreach (forUserCount fucitem in gd)
                                {
                                    lint.Add(fucitem.RegID);
                                }
                            }
                        }
                    }
                    if (listp != null && listp.Count > 0)
                    {
                        List<forUserCount> gp = listp.FindAll(p => p.ActiveID == model.ID);
                        {
                            if (gp != null && gp.Count > 0)
                            {
                                foreach (forUserCount fucitem in gp)
                                {
                                    lint.Add(fucitem.RegID);
                                }
                            }
                        }
                    }
                    if (model != null)
                    {
                        model.UsersTotal = lint.Distinct().ToList().Count;
                    }
                    else
                    {
                        Actives amode = new Actives();
                        amode.ID = 0;
                        amode.ActiveName = "";
                        amode.AddAdminID = 0;
                        amode.ContributeStartTime = DateTime.Now;
                        amode.ContributeEndTime = DateTime.Now;
                        amode.StartTime = DateTime.Now;
                        amode.EndTime = DateTime.Now;
                        amode.AddTime = DateTime.Now;
                        amode.State = 0;
                        amode.MaxDocument = 1;
                        amode.MaxPic = 1;
                        amode.Summary = "";
                        amode.Content = "";
                        amode.ContributeStartTimeT = "";
                        amode.ContributeEndTimeT = "";
                        amode.StartTimeT = "";
                        amode.EndTimeT = "";
                        amode.AddTimeT = "";
                        amode.DocumentTotal = 0;
                        amode.PicTotal = 0;
                        amode.UsersTotal = 0;
                        model = amode;
                    }
                    return model;
                }
            }
        }

        /// <summary>
        /// 添加 编辑 活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int SaveActive(Actives model)
        {
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add
                            DECLARE @newid INT;
                            SET @newid=0;  
                            IF EXISTS(SELECT 1 from Actives WHERE ActiveName=@ActiveName AND ID<>@ID)
                            BEGIN 
                                SELECT -2; 
                            END
                            ELSE 
                            BEGIN
                        ";
            if (model.ID > 0)
            {
                sql += @"   
                            UPDATE Actives SET ActiveName=@ActiveName,ContributeStartTime=@ContributeStartTime,ContributeEndTime=@ContributeEndTime
                            ,MaxDocument=@MaxDocument,MaxPic=@MaxPic,[Summary]=@Summary,
                            [Content]=@Content WHERE ID=@ID;
                            SET @newid = @ID;     
                        ";
            }
            else
            {
                sql += @"
                            INSERT INTO Actives(ActiveName,ContributeStartTime,ContributeEndTime,StartTime,EndTime,AddAdminID,AddTime,State,MaxDocument,
                            MaxPic,Summary,Content)
                            VALUES(@ActiveName,@ContributeStartTime,@ContributeEndTime,GETDATE(),GETDATE(),@AddAdminID,GETDATE(),2,@MaxDocument,@MaxPic,
                            @Summary,@Content);
                            SET @newid=@@IDENTITY;
                       ";
            }

            sql += @"
                            SELECT @newid; 
                          END
     
                        COMMIT TRAN #Proc_Add
                        END TRY
                        BEGIN CATCH
                            SELECT 0;
                            ROLLBACK TRAN #Proc_Add
                        END CATCH 
                        ";

            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<int>(sql, model).FirstOrDefault();
            }
        }

        public static int UpdateActivesState(int id, int state)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                string sql = @"
                            BEGIN TRY
                            DECLARE @h INT;
                            
                            UPDATE Actives SET [State] =@State WHERE ID=@Id;
                            SELECT 1;
                            
                            END TRY
                            BEGIN CATCH
                                SELECT 0;
                            END CATCH 
                            ";
                return conn.Execute(sql, new { ID = id, State = state });
            }
        }

        public static List<ShowUserListPicByActiveID> ShowUserListPicByActiveID(int activeID, int userID)
        {
            string sql = string.Format(@"SELECT * FROM UserPics u LEFT JOIN ActivesPic a ON u.ID=a.PicID WHERE a.ActiveID="+ activeID.ToString()+ " AND u.RegID="+ userID .ToString()+ " ORDER BY a.[State],a.[AddTime] DESC;");
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {

                }))
                {
                    var list = multi.Read<ShowUserListPicByActiveID>().ToList();
                    return new List<ShowUserListPicByActiveID>(list);
                }
            }
        }

        public static List<ShowUserListDocByActiveID> ShowUserListDocByActiveID(int activeID, int userID)
        {
            string sql = string.Format(@"SELECT * FROM UserDocument u LEFT JOIN ActivesDocument a ON u.ID=a.DocumentID WHERE a.ActiveID=" + activeID.ToString() + " AND u.RegID=" + userID.ToString() + " ORDER BY a.[State],a.[AddTime] DESC;");
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {

                }))
                {
                    var list = multi.Read<ShowUserListDocByActiveID>().ToList();
                    return new List<ShowUserListDocByActiveID>(list);
                }
            }
        }


    }
}
