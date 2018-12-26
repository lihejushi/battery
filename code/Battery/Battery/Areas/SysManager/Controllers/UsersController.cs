using Battery.DAL.Log;
using Battery.DAL.Sys;
using Battery.DAL.X;
using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Framework.Domain;
using Battery.Model.X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core.Encrypt;
using XT.MVC.Framework;
using XT.MVC.Framework.Results;
using Battery.Model.Log;
using Battery.Framework;
using Battery.Framework.Tools;

namespace Battery.Areas.SysManager.Controllers
{
    public class UsersController : SysBaseController
    {
        // GET: SysManager/Users
        public ActionResult Index()
        {
            return View();
        }

        [Permission("用户列表", ActionName = "UsersList", ControllerName = "Users", Rank = 1)]
        [SysManagerAuthorize(true, "Users|UsersList")]
        // GET: SysManager/Users
        public ActionResult UsersList(PagedModel pModel,  string name, string identity, string school, string career, int sex = -100, int state = -100, string cmd = "")
        {
            if (cmd == "GetList")
            {
                Tuple<int, List<UserInfo>> data = UserDAL.GetUserList(pModel.PageStart, pModel.PageLength, name, identity, school, career, sex, state);

                foreach (UserInfo ui in data.Item2)
                {
                    
                    if (ui.Career.Substring(0, 1) == "1")
                    {
                        ui.CareerT += "编剧；";
                    }
                    if (ui.Career.Substring(1, 1) == "1")
                    {
                        ui.CareerT += "演员；";
                    }
                }

                return new XTJsonResult(1, string.Empty, data);
            }
            return View();
        }

        [SysManagerAuthorize(true, "Users|UsersList")]
        public ActionResult UsersDetail(int id)
        {
            Tuple<List<UserInfo>, List<UserPics>> model = UserDAL.GetUserModel(id);
            if (model == null)
            {
                return Content("用户不存在");
            }
            ViewBag.PIC = model.Item2;
            return View(model.Item1[0]);
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Users|UsersList")]
        public ActionResult SetUserState(int uId, int state, string Reason)
        {
            InsertLog(ActionType.变更状态, "更改用户【" + uId.ToString() + "】状态为【" + (state == 1 ? "锁定" : "正常") + "】");
            int res = UserDAL.SetUserState(uId, state,Reason) > 0 ? 1 : 0;
            return new XTJsonResult(res);
        }

        [Permission("用户文档", ActionName = "UsersDocument", ControllerName = "Users", Rank = 1)]
        [SysManagerAuthorize(true, "Users|UsersList")]
        // GET: SysManager/Users
        public ActionResult UsersDocument(PagedModel pModel,string username, string documentname,int rid=-100,int aid=-100,int delete=0,string cmd = "")
        {

            List<Actives> l_ud = ActiveAndPointDAL.GetAllActive();
            ViewBag.ac = l_ud;
            ViewBag.rid = rid;
            if (cmd == "GetList")
            {
                Tuple<int, List<UserDocument>> data = UserDAL.GetDocumentByUser(pModel.PageStart, pModel.PageLength, username, documentname,rid, aid,delete);
                return new XTJsonResult(1, string.Empty, data);
            }
            return View();
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Users|UsersList")]
        public ActionResult DeleteUserDocument(int uId, int rid,string filepath, string Reason)
        {
            InsertLog(ActionType.删除, "删除用户【" + rid.ToString() + "】的文档【"+ uId + "】");
            int res = UserDAL.DeleteUserDocument(uId, Reason) > 0 ? 1 : 0;
            //下面删除文件
            if (res == 1)
            {
                bool dr = FileUpload.DeleteFile(FilePath+ filepath);
            }

            return new XTJsonResult(res);
        }
    }
}