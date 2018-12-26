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
    public class UserDAL : DBUtility
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Tuple<int, List<UserInfo>> GetUserList(int pageStart, int pageLength, string name,string identity,string school, string Career, int sex=-100, int state = -100)
        {
            string sqlwhere = "";
            if (name != "")
            {
                sqlwhere += " AND (r.UserName LIKE '%" + name + "%' OR u.UserRealName LIKE '%" + name + "%')";
            }
            if (identity != "")
            {
                sqlwhere += " AND (u.[Identity] = '" + identity + "')";
            }
            if (school != "")
            {
                sqlwhere += " AND (u.[WorkUnit] LIKE '%" + school + "%' OR u.[Graduation] LIKE '%" + school + "%')";
            }
            if (state != -100)
            {
                sqlwhere += " AND r.[State]=" + state;
            }
            if (sex != -100)
            {
                sqlwhere += " AND u.[Sex]=" + sex.ToString();
            }

            sqlwhere += " AND (u.[Career] LIKE '%" + Career+"%')";
            

            string sql = string.Format(@"WITH _a AS
            (
                SELECT r.ID,r.UserName,r.RegTime,r.[State],u.UserRealName,u.WorkUnit,u.Graduation,u.Occupation,u.Career,ROW_NUMBER() OVER(ORDER BY r.[State] DESC,r.Id desc) AS RowID FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE 1=1 {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                }))
                {
                    var list = multi.Read<UserInfo>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<UserInfo>>(records, list);
                }
            }
        }


        /// <summary>
        /// 获取提交照片的用户列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Tuple<int, List<UserInfo>> GetUserListPicByActiveID(int pageStart, int pageLength, string name, string identity, int activeID, int sex = -100)
        {
            string sqlwhere = "";
            if (name != "")
            {
                sqlwhere += " AND (r.UserName LIKE '%" + name + "%' OR u.UserRealName LIKE '%" + name + "%')";
            }
            if (identity != "")
            {
                sqlwhere += " AND (u.[Identity] = '" + identity + "')";
            }
            if (sex != -100)
            {
                sqlwhere += " AND u.[Sex]=" + sex.ToString();
            }

            string sql = string.Format(@"WITH _a AS
            (
                SELECT r.ID,r.UserName,r.RegTime,u.UserRealName,u.WorkUnit,u.Graduation,ROW_NUMBER() OVER(ORDER BY r.Id DESC) AS RowID FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE r.ID IN (SELECT DISTINCT up.RegID FROM UserPics up RIGHT JOIN ActivesPic ap ON ap.PicID=up.ID WHERE ap.ActiveID=@ActiveID) {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE r.ID IN (SELECT DISTINCT up.RegID FROM UserPics up RIGHT JOIN ActivesPic ap ON ap.PicID=up.ID WHERE ap.ActiveID=@ActiveID) {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                    ActiveID= activeID
                }))
                {
                    var list = multi.Read<UserInfo>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<UserInfo>>(records, list);
                }
            }
        }

        /// <summary>
        /// 获取提交文档的用户列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Tuple<int, List<UserInfo>> GetUserListDocByActiveID(int pageStart, int pageLength, string name, string identity, int activeID, int sex = -100)
        {
            string sqlwhere = "";
            if (name != "")
            {
                sqlwhere += " AND (r.UserName LIKE '%" + name + "%' OR u.UserRealName LIKE '%" + name + "%')";
            }
            if (identity != "")
            {
                sqlwhere += " AND (u.[Identity] = '" + identity + "')";
            }
            if (sex != -100)
            {
                sqlwhere += " AND u.[Sex]=" + sex.ToString();
            }

            string sql = string.Format(@"WITH _a AS
            (
                SELECT r.ID,r.UserName,r.RegTime,r.[State],u.UserRealName,u.WorkUnit,u.Graduation,u.Occupation,u.Career,ROW_NUMBER() OVER(ORDER BY r.Id DESC) AS RowID FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE r.ID IN (SELECT DISTINCT up.RegID FROM UserDocument up RIGHT JOIN ActivesDocument ap ON ap.DocumentID=up.ID WHERE ap.ActiveID=@ActiveID) {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE r.ID IN (SELECT DISTINCT up.RegID FROM UserDocument up RIGHT JOIN ActivesDocument ap ON ap.DocumentID=up.ID WHERE ap.ActiveID=@ActiveID) {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                    ActiveID = activeID
                }))
                {
                    var list = multi.Read<UserInfo>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<UserInfo>>(records, list);
                }
            }
        }

        //获取用户model
        public static Tuple<List<UserInfo>,List<UserPics>> GetUserModel(int Id)
        {
            string sql = @"SELECT r.*,u.*,CONVERT(VARCHAR(100), r.RegTime, 120) AS RegTimeT,CONVERT(VARCHAR(100), LastLoginTime, 120) AS LastLoginTimeT
            
            FROM Reg r INNER JOIN UserInfo u ON r.ID=u.RegID WHERE r.ID=@Id;
            SELECT * FROM UserPics WHERE RegID=@Id;
            ";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Id=Id,
                }))
                {
                    var list = multi.Read<UserInfo>().ToList();
                    var list1 = multi.Read<UserPics>().ToList();
                    return new Tuple<List<UserInfo>, List<UserPics>>(list, list1);
                }
            }
        }

        //更改用户状态
        public static int SetUserState(int id, int state,string Reason)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE Reg SET [State] =@State,LockReson=@Reason WHERE ID=@Id", new { Id = id, State = state, Reason= Reason });
            }
        }

        //更新用户登录信息
        public static int SetUserLogin(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE Reg SET [LastLoginTime] =GETDATE(),LoginCount=LoginCount+1 WHERE ID=@Id", new { Id = id});
            }
        }

        /// <summary>
        /// 根据用户ID获取文档
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Tuple<int, List<UserDocument>> GetDocumentByUser(int pageStart, int pageLength,string username, string documentname,int rid=-100,int aid=-100,int delete=0)
        {
            string sqlwhere = "";
            if (rid != -100)
            {
                sqlwhere += " AND (u.RegID =" + rid.ToString() + ")";
            }
            if (delete != -100)
            {
                sqlwhere += " AND (u.IsDelete =" + delete.ToString() + ")";
            }
            if (documentname != "")
            {
                sqlwhere += " AND (u.DocumentTitle LIKE '%" + documentname + "%')";
            }
            if (aid != -100)
            {
                sqlwhere += " AND (a.ActiveID =" + aid.ToString() + ")";
            }
            if (username != "")
            {
                sqlwhere += " AND (u.UserName LIKE '%" + username + "%')";
            }
            string sql = string.Format(@"WITH _a AS
            (
                SELECT u.ID,u.RegID,u.UserName,u.DocumentTitle,u.DocumentType,u.Summary,u.UpdateTime,CONVERT(VARCHAR(100), u.UpdateTime, 120) AS UpdateTimeT ,u.FilePath,u.FileName,u.Keys,ROW_NUMBER() OVER(ORDER BY u.RegID DESC,a.ActiveID DESC,u.ID ASC) AS RowID FROM RegUserDocument u LEFT JOIN ActivesDocument a ON u.ID=a.DocumentID WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM RegUserDocument u LEFT JOIN ActivesDocument a ON u.ID=a.DocumentID WHERE 1=1 {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                    Rid=rid,
                    ISDELETE= delete
                }))
                {
                    var list = multi.Read<UserDocument>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<UserDocument>>(records, list);
                }
            }
        }

        //更改用户文档状态
        public static int DeleteUserDocument(int id,string Reason)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE UserDocument SET [IsDelete] =1,DelReson=@Reason WHERE ID=@Id", new { Id = id, Reason = Reason });
            }
        }

        #region 这一部分是关于用户注册登录的
        /// <summary>
        /// 获取用户对象
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static Reg GetRegModel(string phone)
        {
            string sql = @"SELECT r.*,u.Avator FROM Reg r Left JOIN UserInfo u ON r.ID= u.RegID WHERE UserName=@UserName;";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    UserName = phone
                }))
                {
                    var model = multi.Read<Reg>().FirstOrDefault();

                    return model;
                }
            }
        }

        /// <summary>
        /// 获取用户对象
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static Reg GetRegModel(int Uid)
        {
            string sql = @"SELECT * FROM Reg WHERE ID=@Uid;";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Uid = Uid
                }))
                {
                    var model = multi.Read<Reg>().FirstOrDefault();

                    return model;
                }
            }
        }

        /// <summary>
        /// 获取完整用户信息对象
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(int Uid)
        {
            string sql = @"SELECT * FROM Reg r INNER JOIN UserInfo u ON r.ID = u.RegID WHERE r.ID=@Uid;";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Uid = Uid
                }))
                {
                    var model = multi.Read<UserInfo>().FirstOrDefault();

                    return model;
                }
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Regedit(Reg model,string code, int validMinutes = 10)
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
                            IF EXISTS(SELECT 1 from Reg WHERE UserName=@UserName)
                            BEGIN 
                                SELECT -2; 
                            END
                            ELSE 
                            BEGIN
                        ";           
                sql += @"
                            IF EXISTS (SELECT Id FROM Sms_Log WHERE MobileNo='"+model.UserName + @"' AND SmsType=1 AND VCode={0} AND DATEDIFF(Minute,SendTime,GETDATE()) BETWEEN 0 AND {1})
                            BEGIN
                                INSERT INTO Reg([UserName],[Password],[RegTime],[LastLoginTime],[LoginCount],[State],[LockReson])
                                VALUES(@UserName,@Password,@RegTime,@LastLoginTime,@LoginCount,@State,@LockReson);
                                SET @newid=@@IDENTITY;
                                INSERT INTO UserInfo([RegID]) VALUES(@newid);
                                SELECT @newid; 
                            END
                            ELSE
                            BEGIN
                                 SELECT -3;
                            END
                            
                       ";
            sql += @"
                            
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
                return conn.Query<int>(string.Format(sql, code, validMinutes),model).FirstOrDefault();
            }
        }

        //修改密码
        public static int ModifyPwd(int id, string pwd, string newpwd)
        {
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add
  
                            IF EXISTS(SELECT 1 from Reg WHERE ID="+id.ToString()+@" AND [Password]='"+pwd+@"')
                            BEGIN 
                                UPDATE Reg SET [Password]='"+newpwd+@"' WHERE ID="+id.ToString()+ @";
                                SELECT @@Rowcount;
                            END
                            ELSE 
                            BEGIN
                        ";
            sql += @"
                                SELECT -1; 
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
                return conn.Query<int>(sql).FirstOrDefault();
            }
        }

        //更改用户信息
        public static int EditUserInfo(UserInfo model)
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
                            IF EXISTS(SELECT 1 from UserInfo WHERE UserID=@UserID)
                            BEGIN 
                                UPDATE UserInfo SET [UserRealName]=@UserRealName,[Age]=@Age,[Sex]=@Sex,[Career]=@Career,[Identity]=@Identity,[WorkUnit]=@WorkUnit,[Education]=@Education,
                                [Graduation]=@Graduation,[Major]=@Major,[Specialties]=@Specialties,[Occupation]=@Occupation,[Avator]=@Avator,[BigPic]=@BigPic,[Achievement]=@Achievement,
                                [Birthday]=@Birthday,[Native]=@Native,[Province]=@Province,[City]=@City,[Town]=@Town,[QQ]=@QQ,[EMail]=@EMail,[Wechat]=@Wechat
                                WHERE UserID=@UserID;
                                SELECT @@Rowcount;
                            END
                            ELSE 
                            BEGIN
                        ";
            sql += @"
                                SELECT -1; 
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

        //找回密码
        public static int FindPWD(string username, string code, int validMinutes = 10)
        {
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add 
                            IF EXISTS(SELECT 1 from Reg WHERE UserName="+username+ @")
                            BEGIN 
                                IF EXISTS (SELECT Id FROM Sms_Log WHERE MobileNo='" + username + @"' AND SmsType=2 AND VCode='" + code + @"' AND DATEDIFF(Minute,SendTime,GETDATE()) BETWEEN 0 AND " + validMinutes.ToString() + @")
                                BEGIN
                                
                                    SELECT 1; 
                                END
                                ELSE
                                BEGIN
                                     SELECT -3;
                                END 
                            END
                            ELSE 
                            BEGIN
                        ";
            sql += @"
                            
                            
                       ";
            sql += @"
                            SELECT -2;
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
                return conn.Query<int>(sql).FirstOrDefault();
            }
        }


        //重置密码
        public static int ResetPWD(string username, string newpwd)
        {
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add
  
                            IF EXISTS(SELECT 1 from Reg WHERE UserName='" + username + @"')
                            BEGIN 
                                UPDATE Reg SET [Password]='" + newpwd + @"' WHERE UserName='" + username + @"';
                                SELECT @@Rowcount;
                            END
                            ELSE 
                            BEGIN
                        ";
            sql += @"
                                SELECT -1; 
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
                return conn.Query<int>(sql).FirstOrDefault();
            }
        }

        #endregion

        //编辑文档
        public static int UserEditDocument(UserDocument model)
        { 
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add
                            DECLARE @H INT;

                            IF(@ID>0)
                            BEGIN
                                IF EXISTS(SELECT 1 from UserDocument WHERE ID=@ID)
                                BEGIN 
                                    UPDATE UserDocument SET [DocumentTitle]=@DocumentTitle,[Summary]=@Summary,[FilePath]=@FilePath,[Keys]=@Keys,[FileName]=@FileName WHERE ID=@ID;
                                    SELECT @@Rowcount;
                                END
                                ElSE
                                BEGIN
                                    SELECT -1;
                                END
                            END                            
                            ELSE 
                            BEGIN
                                DECLARE @NewId INT;
                                INSERT INTO UserDocument(RegID,DocumentTitle,Summary,UpdateTime,FilePath,FileName,Keys,IsDelete) VALUES(@RegID,@DocumentTitle,@Summary,GETDATE(),@FilePath,@FileName,@Keys,0);
                                SET @NewId=@@identity;
                                INSERT INTO ActivesDocument(ActiveID,DocumentID,AddTime,[State]) VALUES(1,@@identity,GETDATE(),0);
                                SELECT @@Rowcount;
                        ";
            sql += @"
                                 
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


        //删除文档
        public static int DeleteDocument(int ID,int ActiveID)
        {
            string sql = @"
                            BEGIN TRY 
                           	DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                            SET @TRANCOUNT = @@TRANCOUNT;  
		                            IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                           BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                            ELSE  
				                            SAVE TRAN #Proc_Add
  
                            IF EXISTS(SELECT 1 from UserDocument WHERE ID=" + ID.ToString()+ @")
                            BEGIN 
                                DELETE UserDocument WHERE ID=" + ID.ToString()+ @";
                                DELETE ActivesDocument WHERE DocumentID="+ ID .ToString()+ " AND ActiveID="+ ActiveID.ToString()+@";
                                SELECT @@Rowcount;
                            END
                            ELSE 
                            BEGIN
                        ";
            sql += @"
                                SELECT -1; 
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
                return conn.Query<int>(sql).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取文档对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserDocument GetDocumentModelByID(int id)
        {
            string sql = @"SELECT * FROM UserDocument WHERE ID=@Id;";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Id = id
                }))
                {
                    var model = multi.Read<UserDocument>().FirstOrDefault();

                    return model;
                }
            }
        }


    }

}
