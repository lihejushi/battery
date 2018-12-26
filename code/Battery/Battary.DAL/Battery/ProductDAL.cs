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

namespace Battery.DAL.Battery
{
    public class ProductDAL: DBUtility
    {
        /// <summary>
        /// 获取产品类型列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="typeCode">类型代码</param>
        /// <param name="voltage">电压</param>
        /// <param name="state">0:正常1：停用</param>
        /// <returns></returns>
        public static Tuple<int, List<ProductType>> GetProductTypeList(int pageStart, int pageLength, string typeCode, int voltage, int state )
        {
            string sqlwhere = "";
            if (typeCode != "")
            {
                sqlwhere += " AND ([TypeCode] LIKE '%"+typeCode+"%' ) ";
            }
            if (voltage != -1)
            {
                sqlwhere += " AND ([Voltage] = " + voltage.ToString() + ")";
            }
           
            if (state != -100)
            {
                sqlwhere += " AND [State]=" + state.ToString();
            }

            string sql = string.Format(@"WITH _a AS
            (
                SELECT ID,TypeName,TypeCode,CreateTime,CONVERT(varchar(100), CreateTime, 23) AS CreateTimeT,Voltage,State,ROW_NUMBER() OVER(ORDER BY [State] DESC,ID desc) AS RowID FROM ProductType WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM ProductType WHERE 1=1 {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                }))
                {
                    var list = multi.Read<ProductType>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<ProductType>>(records, list);
                }
            }
        }

        /// <summary>
        /// 获取产品类型model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProductType GetProductTypeModel(int id)
        {
            string sql = @"SELECT ID,TypeName,TypeCode,CreateTime,Remark,Length,Width,High,Weight,[State] FROM ProductType WHERE ID=" + id.ToString() + ";";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id
                }))
                {
                    var model = multi.Read<ProductType>().FirstOrDefault();
                    if (model == null)
                    {
                        ProductType amode = new ProductType();
                        amode.ID = 0;
                        model = amode;
                    }
                    return model;
                }
            }
        }

        /// <summary>
        /// 维护商品类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int EditProductType(ProductType model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.Append("UPDATE ProductType SET ");
                strSql.Append("TypeName=@TypeName,");
                strSql.Append("TypeCode=@TypeCode,");
                strSql.Append("CreateTime=GETDATE(),");
                strSql.Append("Remark=@Remark,");
                strSql.Append("Length=@Length,");
                strSql.Append("Width=@Width,");
                strSql.Append("High=@High,");
                strSql.Append("Weight=@Weight ");
                strSql.Append(" WHERE ID=@ID;");
            }
            else
            {
                strSql.AppendLine("IF(NOT EXISTS (SELECT 1 FROM ProductType WHERE TypeCode=@TypeCode))");
                strSql.AppendLine("BEGIN");
                strSql.Append("INSERT INTO ProductType(");
                strSql.Append("TypeName,TypeCode,CreateTime,Remark,Length,Width,High,Weight,State)");
                strSql.Append(" VALUES (");
                strSql.Append("@TypeName,@TypeCode,GETDATE(),@Remark,@Length,@Width,@High,@Weight,@State);");
                strSql.Append("SELECT @@identity;").AppendLine();
                strSql.AppendLine("END");
                strSql.AppendLine("ELSE");
                strSql.AppendLine("BEGIN");
                strSql.AppendLine("  SELECT -1;");
                strSql.AppendLine("END");
            }
            using (SqlConnection conn = GetSqlConnection())
            {
                if (model.ID < 1)
                {
                    return int.Parse(conn.ExecuteScalar(GetSql(strSql.ToString()), model).ToString());
                }
                return conn.Execute(GetSql(strSql.ToString()), model);
            }
        }

        //更改商品类型状态
        public static int SetTypeState(int id, int state)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE ProductType SET [State] =@State WHERE ID=@Id", new { Id = id, State = state });
            }
        }

        //下面是产品部分

        /// <summary>
        /// 获取产品类型列表
        /// </summary>
        /// <returns></returns>
        public static List<ProductType> GetProductTypeList()
        {

            string sql = string.Format("SELECT ID,TypeName FROM ProductType WHERE [State]=0;");
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    
                }))
                {
                    var list = multi.Read<ProductType>().ToList();
                    return list;
                }
            }
        }

        /// <summary>
        /// 获取生产厂家列表
        /// </summary>
        /// <returns></returns>
        public static List<Factorys> GetFactorysList()
        {

            string sql = string.Format("SELECT ID,Factory FROM Factorys WHERE [State]=0;");
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {

                }))
                {
                    var list = multi.Read<Factorys>().ToList();
                    return list;
                }
            }
        }


        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="num">产品编号</param>
        /// <param name="typeID">类型ID</param>
        /// <param name="voltage">电压</param>
        /// <param name="factoryID">厂家ID</param>
        /// <param name="state">0:正常1：停用</param>
        /// <returns></returns>
        public static Tuple<int, List<Products>> GetProductList(int pageStart, int pageLength, string num,int typeID, int voltage, int factoryID, int state)
        {
            string sqlwhere = "";
            if (num != "")
            {
                sqlwhere += " AND (p.Num LIKE '%" + num + "%') ";
            }
            if (typeID != -100)
            {
                sqlwhere += " AND (p.[TypeID] =" + typeID + ") ";
            }
            if (voltage != -100)
            {
                sqlwhere += " AND (p.[Voltage] = " + voltage.ToString() + ")";
            }

            if (factoryID != -100)
            {
                sqlwhere += " AND (p.[FactoryID] = " + factoryID.ToString() + ")";
            }

            if (state != -100)
            {
                sqlwhere += " AND p.[State]=" + state.ToString();
            }

            string sql = string.Format(@"WITH _a AS
            (
                SELECT p.ID,p.Num,(SELECT TypeName FROM ProductType WHERE ID=p.TypeID) AS TypeName,p.Voltage,(SELECT Factory FROM Factorys WHERE ID=p.FactoryID) AS Factory,CONVERT(varchar(100), p.CreateTime, 23) AS CreateTimeT,
                        (CASE p.State WHEN 0 THEN '出厂' WHEN 1 THEN '入店' WHEN 2 THEN '安装' WHEN 3 THEN '维修' WHEN 4 THEN '报废' WHEN 5 THEN '回收' END) AS StateT,
                        ROW_NUMBER() OVER(ORDER BY p.ID DESC) AS RowID FROM Products p WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Products p WHERE 1=1 {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                }))
                {
                    var list = multi.Read<Products>().ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<Products>>(records, list);
                }
            }
        }


        /// <summary>
        /// 获取产品列表只取头100条
        /// </summary>
        /// <param name="today"></param>
        /// <param name="userid">用户id</param>
        /// <param name="topcount">取头100条</param>
        /// <returns></returns>
        public static List<Products> GetProductListForScanAdd(DateTime today,int userid,int topcount)
        {
            string sql = string.Format(@"
                SELECT TOP "+topcount.ToString()+ @" p.ID,p.Num,(SELECT pt.TypeName FROM ProductType pt WHERE pt.ID=p.TypeID) AS TypeName,p.Voltage,(SELECT f.Factory FROM Factorys f WHERE f.ID=p.FactoryID) AS Factory,CONVERT(varchar(100), p.CreateTime, 23) AS CreateTimeT,
                        (CASE p.[State] WHEN 0 THEN '出厂' WHEN 1 THEN '入店' WHEN 2 THEN '安装' WHEN 3 THEN '维修' WHEN 4 THEN '报废' WHEN 5 THEN '回收' END) AS StateT 
                         FROM Products p WHERE p.[State]=0 AND DATEDIFF(d,CreateTime,@Today)=0 AND LibraryID=@Userid ORDER BY p.ID DESC;"
           );
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Today = today,
                    Userid = userid,
                }))
                {
                    var list = multi.Read<Products>().ToList();
                    return list;
                }
            }
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int ScanAddProduct(string productnum,string typecode,string voltage, string area,string factory,DateTime outtime,int userid)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.AppendLine("IF(NOT EXISTS (SELECT 1 FROM Products WHERE Num=@Productnum)) ");
            strSql.AppendLine("BEGIN ");
            strSql.AppendLine("     DECLARE @PID INT; ");
            strSql.Append("         INSERT INTO Products(");
            strSql.Append("                Num,TypeID,Voltage,Area,FactoryID,State,CreateTime,LibraryID,ActivationCode) ");
            strSql.Append("         VALUES (");
            strSql.Append("               @Productnum,(SELECT pt.ID FROM ProductType pt WHERE pt.TypeCode=@Typecode),('Voltage"+voltage+ "'),@Area,(SELECT f.ID FROM [Factorys] f WHERE f.FactoryNum=@Factory),0,@Outtime,@Userid,'111111');");
            strSql.AppendLine("     SET @PID= @@identity;");
            strSql.Append("         INSERT INTO ProductSend(ProductID,Num,SourcePlace,TargetPlace,[Type],Introduce,[Note],[Transport],TransportTime,TransportNum,AcceptTime,TransportStoreID,AcceptStoreID) VALUES(@PID,@Productnum,'','',0,'出厂','',0,GETDATE(),'',GETDATE(),0,0);");
            strSql.Append("         SELECT @@ROWCOUNT;").AppendLine();
            strSql.AppendLine("END ");
            strSql.AppendLine("ELSE ");
            strSql.AppendLine("BEGIN");
            strSql.AppendLine("    SELECT -1;");
            strSql.AppendLine("END ");

            using (SqlConnection conn = GetSqlConnection())
            {

                //using (var multi = conn.QueryMultiple(strSql.ToString(), new
                //{
                //    Productnum = productnum,
                //    Typecode = typecode,
                //    Area = area,
                //    Factory = factory,
                //    Outtime = outtime,
                //    Userid = userid
                //}))
                //{
                    return int.Parse(conn.ExecuteScalar(GetSql(strSql.ToString()), new { Productnum = productnum, Typecode = typecode, Area = area, Factory = factory, Outtime = outtime, Userid = userid }).ToString());
                //}

                //int records = multi.Read<int>().FirstOrDefault();
                //return conn.Execute(GetSql(strSql.ToString()),new { Productnum = productnum, Typecode = typecode, Area = area, Factory = factory, Outtime = outtime, Userid = userid });
            }
        }

        /// <summary>
        /// 获取产品基本信息model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Products ProductBase(int id)
        {
            string sql = @"SELECT p.ID,p.Num,p.TypeID,(SELECT t.TypeName FROM ProductType t WHERE t.ID=p.TypeID) AS TypeName, p.DeCode,p.Voltage,p.Area,(SELECT f.Factory FROM Factorys f WHERE f.ID=p.FactoryID) AS Factory,
                            p.UserArea,p.TypeFor,p.ActiveDate,p.ScrapDate,p.RealScrapDate,p.BackStoreID,p.BackStaffID,p.BackStaffNum,p.BackDate,p.Guarantee,p.BackStaffName,p.LibraryID,(SELECT s.UserName FROM Sys_Person s WHERE s.Id=p.LibraryID) AS LibraryName,
                            p.CreateTime,(CONVERT(varchar(100), p.CreateTime, 23)) AS CreateTimeT,p.[State],(CASE p.[State] WHEN 0 THEN '出厂' WHEN 1 THEN '入店' WHEN 2 THEN '安装' WHEN 3 THEN '维修' WHEN 4 THEN '报废' WHEN 5 THEN '回收' ELSE '其它' END) AS StateT  
                            FROM Products p WHERE p.ID=" + id.ToString() + ";";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id
                }))
                {
                    var model = multi.Read<Products>().FirstOrDefault();
                    if (model == null)
                    {
                        Products amode = new Products();
                        amode.ID = 0;
                        model = amode;
                    }
                    return model;
                }
            }
        }


        /// <summary>
        /// 获取产品调配信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<ProductSend> ProductSend(int id)
        {
            string sql = @"SELECT ID,ProductID,Num,SourcePlace,TargetPlace,Type,Introduce,Transport,TransportTime,TransportNum,AcceptTime,TransportStoreID,AcceptStoreID 
                            FROM ProductSend WHERE ProductID=" + id.ToString() + ";";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id
                }))
                {
                    List<ProductSend> l_ps = multi.Read<ProductSend>().ToList();
                    return l_ps;
                }
            }
        }

        /// <summary>
        /// 获取产品安装信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<ProductInstall> ProductInstall(int id)
        {
            string sql = @"SELECT ID,ProductID,Num,StoreID,StaffID,StaffNum,InstallDate,ActiveDate,InstallTimes,InstallCost,InstallOrUn,IsScrap,StaffName 
                            FROM ProductInstall WHERE ProductID=" + id.ToString() + ";";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id
                }))
                {
                    List<ProductInstall> l_ps = multi.Read<ProductInstall>().ToList();
                    return l_ps;
                }
            }
        }

        /// <summary>
        /// 获取用户资料model
        /// </summary>
        /// <param name="id">产品安装情况表ID</param>
        /// <returns></returns>
        public static ProductUserInfo GetUserInfo(int id)
        {
            string sql = @"SELECT p.ID,p.Num,p.ProductID,p.ProductInstallID,p.UserName,p.UserTel,p.UserSex,p.WorkUnit,p.Age,p.Career,p.Education,p.UseBike,p.KonwStyle,p.UserWeChat,p.UserQQ,p.UserEMail 
                            FROM UserInfo p WHERE p.ProductInstallID=" + id.ToString() + ";";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    ID = id 
                }))
                {
                    var model = multi.Read<ProductUserInfo>().FirstOrDefault();
                    if (model == null)
                    {
                        ProductUserInfo amode = new ProductUserInfo();
                        amode.ID = 0;
                        model = amode;
                    }
                    return model;
                }
            }
        }

        public static EndResult UserProduct(string Num,string UserName,string UserTel,string WorkUnit,int Age,string UseBike,string UserWeChat,string UserQQ,string UserEMail,string InstallCost,int UserSex, string Career, string Education,string KonwStyle,int UserId)
        {
            
            string sql = @"
                        BEGIN TRY 
                        DECLARE @TRANCOUNT INT; --commit,rollback只控制本存储过程  
                        SET @TRANCOUNT = @@TRANCOUNT;  
		                    IF (@TRANCOUNT=0) /*判断事务记数，根据情况确定使用保存点或者新建一个事务*/   
				                BEGIN TRAN #Proc_Add--当前事务点，rollback、commit都从这里开始   
		                    ELSE  
				                SAVE TRAN #Proc_Add
                        DECLARE @h INT;
                        DECLARE @time INT;
                        DECLARE @state INT;
                        DECLARE @newtime INT;
                        DECLARE @newintallid INT;
                        DECLARE @endstring INT;
                        SET @endstring=-1;
                        SELECT @time=pi.InstallTimes,@state=(SELECT p.State FROM Products p WHERE p.Num=@Num) FROM ProductInstall pi WHERE pi.Num=@Num;
                        IF(@time IS NULL)
                        BEGIN
                            SET @newtime=1;
                        END
                        ELSE
                        BEGIN
                            IF(@state=3 OR @state=5)
                            BEGIN
                                SET @newtime= @time+1;
                            END
                            ELSE
                            BEGIN
                               SET @endstring=@state;
                            END
                        END
                        IF(@endstring=-1)
                        BEGIN
                            INSERT INTO ProductInstall(ProductID,Num,StoreID,StaffID,StaffNum,InstallDate,ActiveDate,InstallTimes,InstallCost,InstallOrUn,IsScrap,StaffName) 
                            VALUES((SELECT p.ID FROM Products p WHERE p.Num=@Num),@Num,1,@UserId,@UserId,GETDATE(),GETDATE(),@newtime,@InstallCost,0,0,@UserId);
                            SET @newintallid = @@IDENTITY;                            

                            INSERT INTO UserInfo(ProductID,Num,ProductInstallID,UserName,UserTel,UserSex,WorkUnit,Age,Career,Education,UseBike,KonwStyle,UserWeChat,UserQQ,UserEMail) 
                            VALUES((SELECT p.ID FROM Products p WHERE p.Num=@Num),@Num,@newintallid,@UserName,@UserTel,@UserSex,@WorkUnit,@Age,@Career,@Education,@UseBike,@KonwStyle,@UserWeChat,@UserQQ,@UserEMail);                            

                            SELECT  ActivationCode,@endstring AS EndString FROM [Products] WHERE Num=@Num;                           
                        END
                        BEGIN
                            SELECT  'Error',@endstring AS EndString;  
                        END
                        COMMIT TRAN #Proc_Add
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRAN #Proc_Add;
                            SELECT 'Error' AS ActivationCode,-1 AS EndString;
                        END CATCH 
                        ";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    Num = Num,
                    UserName = UserName,
                    UserTel = UserTel,
                    WorkUnit = WorkUnit,
                    Age = Age,
                    UseBike = UseBike,
                    UserWeChat = UserWeChat,
                    UserQQ = UserQQ,
                    UserEMail = UserEMail,
                    InstallCost = InstallCost,
                    UserId = UserId,
                    UserSex = UserSex,
                    Career = Career,
                    Education = Education,
                    KonwStyle = KonwStyle
                }))
                {
                    var model = multi.Read<EndResult>().FirstOrDefault();
                    return model;
                }
            }
        }

    }
}
