using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace Battery.DAL.Sys
{
    public class PermissionDAL : DBUtility
    {
        #region 后台用户

        /// <summary>
        /// 通过用户名获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Sys_Person GetPersonByName(string userName)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Person>("SELECT * FROM Sys_Person WHERE [State]<>-2 AND UserName=@UserName", new { UserName = userName }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取后台用户列表
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static Tuple<int, List<Sys_Person>> GetPersonRequestList(int pageStart, int pageLength, string name = "", int state = -100, int roleId = 0)
        {
            string sqlwhere = "";
            if (name != "")
            {
                sqlwhere += " AND(sp.UserName LIKE '%" + name + "%' OR sp.TrueName LIKE '%" + name + "%' or MobilePhone LIKE '%" + name + "%')";
            }
            if (state != -100)
            {
                sqlwhere += " AND sp.[State]=" + state;
            }
            else
            {
                sqlwhere += " AND sp.[State]>=0";
            }
            if (roleId > 0)
            {
                sqlwhere += " AND sp.Id IN (SELECT spr.PersonId FROM Sys_Person_Role spr WHERE spr.RoleId=" + roleId + ")";
            }

            string sql = string.Format(@"WITH _a AS
            (
                SELECT *,ROW_NUMBER() OVER(ORDER BY sp.State DESC,sp.Id desc) AS RowID FROM Sys_Person sp WHERE 1=1 {0} 
            )
            SELECT _a.* FROM _a WHERE RowID BETWEEN @b AND @e;
            SELECT COUNT(1) FROM Sys_Person sp WHERE 1=1 {0};", sqlwhere);
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    b = pageStart,
                    e = pageStart + pageLength,
                }))
                {
                    var list = multi.Read<Sys_Person>().Select(p => { p.Password = ""; return p; }).ToList();
                    int records = multi.Read<int>().FirstOrDefault();
                    return new Tuple<int, List<Sys_Person>>(records, list);
                }
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="Id">PersonID</param>
        /// <param name="state">更新后状态</param>
        /// <returns></returns>
        public static int UpdatePersonState(int id, int state)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE Sys_Person SET [State] =@State WHERE Id=@Id", new { Id = id, State = state });
            }
        }

        /// <summary>
        /// 检查用户名是否重复
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userId">Id </param>
        /// <returns></returns>
        public static int CheckUserName(string userName, int userId)
        {
            string sqlStr = "DECLARE @Count INT;SELECT @Count=COUNT(1) FROM Sys_Person sp WHERE sp.UserName=@UserName AND id!=@Id;SELECT @Count;";
            using (SqlConnection conn = GetSqlConnection())
            {
                return int.Parse(conn.ExecuteScalar(sqlStr, new { UserName = userName, Id = userId }).ToString());
            }
        }
        /// <summary>
        /// 获取后台用户实体，包含有角色id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Sys_Person GetPersonModel(int personId)
        {
            string sql = @"SELECT * FROM Sys_Person sp WHERE sp.Id=@PersonId;
SELECT RoleId FROM Sys_Person_Role spr WHERE spr.PersonId = @PersonId;";
            using (SqlConnection conn = GetSqlConnection())
            {
                using (var multi = conn.QueryMultiple(sql, new
                {
                    PersonId = personId
                }))
                {
                    var model = multi.Read<Sys_Person>().FirstOrDefault();
                    int[] roleIds = multi.Read<int>().ToArray();
                    if (model != null)
                    {
                        model.RoleIds = roleIds;
                    }

                    return model;
                }
            }
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int SavePerson(Sys_Person model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.Id > 0)
            {
                strSql.Append("update #Sys_Person# set ");
                //strSql.Append("UserName=@UserName,");
                strSql.Append("TrueName=@TrueName,");
                if (string.IsNullOrEmpty(model.Password) == false) strSql.Append("Password=@Password,");
                strSql.Append("MobilePhone=@MobilePhone,");
                strSql.Append("Tel=@Tel,");
                strSql.Append("Address=@Address,");
                strSql.Append("Email=@Email,");
                strSql.Append("Memo=@Memo,");
                strSql.Append("State=@State");
                strSql.Append(" Where Id=@Id;");
            }
            else
            {
                strSql.AppendLine(@"IF(NOT EXISTS (SELECT 1 FROM Sys_Person sp WHERE sp.UserName=''))
BEGIN ");
                strSql.Append("insert into #Sys_Person#(");
                strSql.Append("UserName,TrueName,Password,MobilePhone,Tel,Address,Email,Memo,State)");
                strSql.Append(" values (");
                strSql.Append("@UserName,@TrueName,@Password,@MobilePhone,@Tel,@Address,@Email,@Memo,@State)");
                strSql.Append(";select @@identity;").AppendLine();
                strSql.AppendLine("END");
                strSql.AppendLine("ELSE");
                strSql.AppendLine("  SELECT 0");
            }
            using (SqlConnection conn = GetSqlConnection())
            {
                if (model.Id < 1)
                {
                    return int.Parse(conn.ExecuteScalar(GetSql(strSql.ToString()), model).ToString());
                }
                return conn.Execute(GetSql(strSql.ToString()), model);
            }
        }
        /// <summary>
        /// 设置人员角色
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public static bool SetRoleToPerson(int personId, int[] roleIds)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                int row = conn.Execute(GetSql("DELETE FROM #Sys_Person_Role# WHERE PersonId=@PersonId;"), new { PersonId = personId }, transaction, null, null);
                foreach (var roleId in roleIds)
                {
                    row += conn.Execute(GetSql("INSERT INTO Sys_Person_Role(RoleId,PersonId) VALUES(@RoleId,@PersonId);"), new { RoleId = roleId, PersonId = personId }, transaction, null, null);
                }
                if (row > 0) transaction.Commit();
                return row > 0;
            }
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionList"></param>
        /// <returns></returns>
        public static bool SetPermissionToRole(List<Sys_Role_Permission> permissionList)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                int row = conn.Execute(GetSql("DELETE FROM #Sys_Role_Permission# WHERE RoleId=@RoleId;"), new { RoleId = permissionList[0].RoleId }, transaction, null, null);
                foreach (var permission in permissionList)
                {
                    row += conn.Execute(GetSql("INSERT INTO #Sys_Role_Permission#(RoleId,Controller,[Action]) VALUES(@RoleId,@Controller,@Action)"), permission, transaction, null, null);
                }
                if (row > 0) transaction.Commit();
                return row > 0;
            }
        }

        /// <summary>
        /// 获取用户所有的权限
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static List<Sys_Role_Permission> GetPersonPermission(int personId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Role_Permission>(GetSql(@"SELECT DISTINCT srp.Controller,srp.[Action]
  FROM #Sys_Role_Permission# AS srp,#Sys_Person_Role# AS spr
WHERE spr.PersonId=@PersonId AND spr.RoleId=srp.RoleId"), new { PersonId = personId }).ToList();
            }
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static int EditPersonPW(int personId, string pd)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute("UPDATE Sys_Person SET [Password] = @Password WHERE Id=@Id", new { Id = personId, Password = pd });
            } 
        }

        #endregion

        #region 用户角色

        public static List<Sys_Role> GetRoleList()
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Role>(GetSql("SELECT * FROM #Sys_Role#")).ToList();
            }
        }
        public static Sys_Role GetRoleModel(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Role>(GetSql("SELECT * FROM #Sys_Role# WHERE Id=@RoleId"), new { RoleId = id }).FirstOrDefault();
            }
        }
        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int SaveRole(Sys_Role model)
        {
            string sql = @"
                           if not exists(select * from #Sys_Role# where Name=@Name)
                              begin
                                  set @rtval=1;
                                 INSERT INTO #Sys_Role#(Name,Memo,[State]) VALUES(@Name,@Memo,@State);
                              end
                           else 
                                 set @rtval=-1;";
            if (model.Id > 0)
            {
                sql = @"if not exists(select * from #Sys_Role# where Id<>@Id and Name=@Name )
                              begin
                                  set @rtval=1;
                                  UPDATE #Sys_Role# SET Name = @Name,Memo = @Memo WHERE Id=@Id;
                               end
                               else 
                                 set @rtval=-1;";
            }
            using (SqlConnection conn = GetSqlConnection())
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@rtval", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 4);
                p.AddDynamicParams(model);
                conn.Execute(GetSql(sql), p);
                return p.Get<int>("@rtval");
                // return conn.Execute(GetSql(sql), p) > 0;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool DeleteRole(int roleId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                int row = conn.Execute(GetSql("DELETE FROM #Sys_Role# WHERE Id=@RoleId;"), new { RoleId = roleId }, transaction, null, null);
                row += conn.Execute(GetSql("DELETE FROM #Sys_Person_Role# WHERE RoleId=@RoleId;"), new { RoleId = roleId }, transaction, null, null);
                row += conn.Execute(GetSql("DELETE FROM #Sys_Role_Permission# WHERE RoleId=@RoleId;"), new { RoleId = roleId }, transaction, null, null);
                if (row > 0) transaction.Commit();
                return row > 0;
            }
        }

        /// <summary>
        /// 获取角色对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<Sys_Role_Permission> GetPermissionFormRoleId(int roleId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Role_Permission>(GetSql("SELECT * FROM #Sys_Role_Permission# WHERE RoleId=@RoleId"), new { RoleId = roleId }).ToList();
            }
        }

        /// <summary>
        /// 获取后台用户对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<Sys_Role_Permission> GetPermissionFormPersonId(int personId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Role_Permission>(GetSql("SELECT * FROM #$Sys_Role_Permission# srp,#Sys_Person_Role# spr WHERE spr.RoleId=srp.RoleId AND spr.PersonId=@PersonId"), new { PersonId = personId }).ToList();
            }
        }

        #endregion

        #region 菜单

        /// <summary>
        /// 权限列表
        /// </summary>
        /// <returns></returns>
        public static List<Sys_Menu> GetMenuList()
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Menu>(GetSql("SELECT * FROM #Sys_Menu# sp ORDER BY SortNo ASC")).ToList();
            }
        }

        public static Sys_Menu GetMenu(int menuId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Menu>(GetSql("SELECT * FROM #Sys_Menu# sp WHERE Id=@MenuId"), new { MenuId = menuId }).FirstOrDefault();
            }
        }

        public static List<Sys_Menu> GetMenuList(int parId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Query<Sys_Menu>(GetSql("SELECT * FROM #Sys_Menu# sp WHERE ParentId=@ParentId ORDER BY SortNo ASC"), new { ParentId = parId }).ToList();
            }
        }

        public static bool SaveMenu(Sys_Menu model)
        {


            string sql = @"IF(EXISTS (SELECT 1 FROM Sys_Menu sm WHERE sm.Id=@ParentId OR @ParentId=0))
BEGIN
INSERT INTO #Sys_Menu#(Name,[Url],Icon,ViewPermission,ParentId,SortNo,Memo,[State])
VALUES(@Name,@Url,@Icon,@ViewPermission,@ParentId,@SortNo,@Memo,@State);
END";

            if (model.ParentId == 0)
            {
                sql = @"INSERT INTO #Sys_Menu#(Name,[Url],Icon,ViewPermission,ParentId,SortNo,Memo,[State])
VALUES(@Name,@Url,@Icon,@ViewPermission,@ParentId,@SortNo,@Memo,@State);";
            }

            if (model.Id > 0)
            {
                sql = @"UPDATE Sys_Menu
SET Name = @Name,[Url] = @Url,Icon = @Icon,ViewPermission = @ViewPermission,ParentId = @ParentId,SortNo = @SortNo,Memo = @Memo,[State] = @State WHERE Id=@Id";
            }
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(GetSql(sql), model) > 0;
            }
        }

        public static bool DeleteMenu(int menuId)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                return conn.Execute(GetSql(@"IF(NOT EXISTS (SELECT 1 FROM Sys_Menu sm WHERE sm.ParentId=@MenuId))
BEGIN
DELETE FROM Sys_Menu WHERE Id=@MenuId;
END"), new { MenuId = menuId }) > 0;
            }
        }

        #endregion
    }
}
