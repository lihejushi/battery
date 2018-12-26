using AutoMapper;
using Battery.Areas.SysManager.Models;
using Battery.DAL.Sys;
using Battery.Framework;
using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Framework.Permissions;
using Battery.Model;
using Battery.Model.Log;
using Battery.Model.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Encrypt;
using XT.MVC.Core.Extensions;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;
using XT.MVC.Framework.Results;

namespace Battery.Areas.SysManager.Controllers
{
    public class SystemController : SysBaseController
    {
        #region 后台用户 
        /// <summary>
        /// 后台用户列表
        /// </summary>
        /// <returns></returns>
        [Permission("后台用户列表", ActionName = "PersonList", ControllerName = "System", Rank = 1)]
        [SysManagerAuthorize(true, "System|PersonList")]
        public ActionResult PersonList(PagedModel pModel, string cmd = "", string name = "", int state = -100)
        {
            InsertLog(ActionType.浏览, "浏览后台用户列表");

            int roleId = GetFormParaValue("RoleId").ToInt(0);
            ViewBag.RoleId = roleId;
            if (cmd == "GetList")
            {
                Tuple<int, List<Sys_Person>> data = PermissionDAL.GetPersonRequestList(pModel.PageStart, pModel.PageLength, name, state, roleId);
                return new XTJsonResult(1, string.Empty, data);
            }
            return View();
        }

        /// <summary>
        /// 设置后台用户状态
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [Permission("设置用户状态", ActionName = "PersonManager", Rank = 2)]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult SetPersonState(int personId, int state)
        {
            InsertLog(ActionType.变更状态, "更改后台用户【" + personId.ToString() + "】状态为【" + (state == -1 ? "删除" : (state == 1 ? "启用" : "禁用")) + "】");

            return new XTJsonResult(PermissionDAL.UpdatePersonState(personId, state) > 0 ? 1 : 0);
        }

        [HttpGet]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult EditPerson(int personId = 0)
        {
            var personModel = PermissionDAL.GetPersonModel(personId) ?? new Sys_Person() { RoleIds = new int[] { } };
            return View(personModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult EditPerson(SysPersonModel model, string Password2 = "")
        {
            if (!ModelState.IsValid)
            {
                return new XTJsonResult(-1, ModelState.Where(m => m.Value.Errors.Count > 0).First().Value.Errors[0].ErrorMessage);
            }
            else
            {
                Sys_Person person = Mapper.Map<SysPersonModel, Sys_Person>(model);
                //修改密码
                if (string.IsNullOrEmpty(person.Password) == false && person.Password != Password2)
                {
                    return new XTJsonResult(-1, "输入密码不一致");
                }
                if (string.IsNullOrEmpty(person.Password) == false)
                {
                    person.Password = new HashHelper().HashString(person.Password);
                }
                else
                {
                    person.Password = string.Empty;
                }
                int result = PermissionDAL.SavePerson(person);
                if (result > 0)
                    InsertLog(ActionType.编辑, "修改后台用户【" + model.Id.ToString() + "】信息");

                return new XTJsonResult(result > 0 ? 1 : 0, result > 0 ? "保存成功" : "保存失败");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult CreatePerson(SysPersonModel model, string Password2 = "")
        {
            if (!ModelState.IsValid)
            {
                return new XTJsonResult(-1, ModelState.Where(m => m.Value.Errors.Count > 0).First().Value.Errors[0].ErrorMessage);
            }
            else
            {
                
                Mapper.Initialize(x => x.CreateMap<SysPersonModel, Sys_Person>());
                Sys_Person person = Mapper.Map<SysPersonModel, Sys_Person>(model);
                //修改密码
                if (string.IsNullOrEmpty(person.Password) == false && person.Password != Password2)
                {
                    return new XTJsonResult(-1, "输入密码不一致");
                }
                person.Password = new HashHelper().HashString(person.Password);
                int result = PermissionDAL.SavePerson(person);
                if (result > 0)
                    InsertLog(ActionType.新增, "创建后台用户【" + model.UserName + "】信息");
                return new XTJsonResult(result > 0 ? 1 : 0, result > 0 ? "保存成功" : "保存失败");
            }
        }

        [SysManagerAuthorize(true)]
        public ActionResult CheckPersonName(string userName, int personId = 0)
        {
            return new XTJsonResult(PermissionDAL.CheckUserName(userName, personId) > 0 ? 1 : 0);
        }

        [HttpGet]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult SetRole(int personId)
        {
            IWebHelper webHelper = EngineContext.Current.Resolve<IWebHelper>();
            var person = PermissionDAL.GetPersonModel(personId);
            if (person == null) return Tip("未找到数据");
            ViewBag.PersonInfo = person;
            ViewBag.PersonId = personId;

            var roleList = PermissionDAL.GetRoleList();
            return View(roleList);
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|PersonManager")]
        public ActionResult SetRole(int personId, string roleIds)
        {
            int[] roles = roleIds.ToIntArray(new char[] { ',' });
            if (personId <= 0) return new XTJsonResult(0, "参数缺失");
            if (roles.Length == 0) return new XTJsonResult(0, "请选择角色");
            bool result = PermissionDAL.SetRoleToPerson(personId, roles);


            if (result)
                InsertLog(ActionType.审核, "设置后台用户【" + personId + "】为角色【" + roleIds + "】");

            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }

        #endregion

        #region 后台用户角色

        [Permission("角色列表")]
        [SysManagerAuthorize(true, "System|RoleList")]
        public ActionResult RoleList()
        {
            InsertLog(ActionType.浏览, "浏览角色列表");
            var roleList = PermissionDAL.GetRoleList();
            return View(roleList);
        }

        [Permission("角色管理", ActionName = "RoleManager")]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult EditRole(int roleId = 0)
        {
            var role = PermissionDAL.GetRoleModel(roleId);
            return View(role);
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult EditRole(int id, string name, string memo)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 16) return new XTJsonResult(0, "角色名称过长");

            int result = PermissionDAL.SaveRole(new Sys_Role()
            {
                Id = id,
                Name = name,
                Memo = memo,
                State = 1
            });
            if (result > 0)
            {
                InsertLog(id > 0 ? ActionType.编辑 : ActionType.新增, id > 0 ? "编辑角色" : "创建角色");
            }


            return new XTJsonResult(result, result.ToString());
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult CreateRole(string name, string memo)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 16) return new XTJsonResult(0, "角色名称过长");

            int result = PermissionDAL.SaveRole(new Sys_Role()
            {
                Id = 0,
                Name = name,
                Memo = memo,
                State = 1
            });
            //if (result > 0)
            //{
            //    InsertLog(id > 0 ? ActionType.编辑 : ActionType.新增, id > 0 ? "编辑角色" : "创建角色");
            //}
            return new XTJsonResult(result, result.ToString());
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult DeleteRole(int roleId)
        {
            bool result = PermissionDAL.DeleteRole(roleId);

            if (result)
            {
                InsertLog(ActionType.删除, "删除角色【" + roleId + "】");
            }

            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }

        #endregion

        #region 权限列表

        [SysManagerAuthorize(true, "System|PermissionList")]
        [Permission("权限列表")]
        public ActionResult PermissionList()
        {
            var permissionList = PermissionHelper.GetPermission(); //PermissionDAL.GetPermissionList();
            return View(permissionList);
        }

        [HttpGet]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult SetPermission(int roleId)
        {
            ViewBag.RoleId = roleId;
            var permissionList = PermissionHelper.GetPermission(); //PermissionDAL.GetPermissionList();
            ViewBag.PermissionList = PermissionDAL.GetPermissionFormRoleId(roleId);
            return View(permissionList);
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|RoleManager")]
        public ActionResult SetPermission(int roleId, string permissions)
        {
            List<string> permissionList = permissions.Split(',').ToList();
            List<Sys_Role_Permission> list = permissionList.Where(p => string.IsNullOrEmpty(p) == false && p.Split('|').Length == 2).Select(p => p.Split('|')).Select(p => new Sys_Role_Permission()
            {
                Action = p[1],
                Controller = p[0],
                RoleId = roleId
            }).ToList();
            if (list.Count == 0) return new XTJsonResult(0, "请选择权限");
            bool result = PermissionDAL.SetPermissionToRole(list);
            if (result)
            {
                InsertLog(ActionType.新增, "设置角色【" + roleId + "】权限");
            }
            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }

        #endregion

        #region 菜单

        [Permission("后台菜单列表")]
        [SysManagerAuthorize(true, "System|MenuList")]
        public ActionResult MenuList()
        {
            var menuList = PermissionDAL.GetMenuList();
            return View(menuList);
        }

        [HttpGet]
        [Permission("菜单管理", ActionName = "MenuManager")]
        [SysManagerAuthorize(true, "System|MenuManager")]
        public ActionResult EditMenu(int mId = 0, int parId = 0)
        {
            Sys_Menu menu = null;
            if (mId > 0)
            {
                menu = PermissionDAL.GetMenu(mId);
                if (menu != null) parId = menu.ParentId;
                else
                {
                    mId = 0;
                }
            }
            List<Sys_Menu> ParentList = PermissionDAL.GetMenuList(0);
            ParentList.Insert(0, new Sys_Menu()
            {
                Id = 0,
                Name = "主菜单"
            });

            ViewBag.ParMenuItem = ParentList.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString(), Selected = parId == p.Id });
            return View(menu);
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|MenuManager")]
        public ActionResult EditMenu(Sys_Menu model)
        {
            model.State = 1;
            bool result = PermissionDAL.SaveMenu(model);
            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }

        [HttpPost]
        [SysManagerAuthorize(true, "System|MenuManager")]
        public ActionResult CreateMenu(Sys_Menu model)
        {
            model.State = 1;
            bool result = PermissionDAL.SaveMenu(model);
            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }
        [HttpPost]
        [SysManagerAuthorize(true, "System|MenuManager")]
        public ActionResult DeleteMenu(int menuId)
        {
            bool result = PermissionDAL.DeleteMenu(menuId);
            return new XTJsonResult(result ? 1 : 0, result ? "执行成功" : "执行失败");
        }

        #endregion

        #region 系统设置  
        [Permission("系统参数管理", ActionName = "SystemSetting", Rank = 1)]
        [SysManagerAuthorize(true, "System|SystemSetting")]
        public ActionResult SystemSetting()
        {
            InsertLog(ActionType.浏览, "浏览系统设置");
            return View();
        }
        /// <summary>
        /// 保存系统字典
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SysManagerAuthorize(true, "System|SystemSetting")]
        [ValidateInput(false)]
        public ActionResult SaveSysDict(string data)
        {
            try
            {
                int successCount = 0;
                bool result = false;
                List<Sys_Dict> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Sys_Dict>>(data);

                foreach (var item in list)
                {
                    bool r = DictDAL.SaveDict(item);
                    successCount += (r ? 1 : 0);
                }

                result = successCount == list.Count;

                if (result)
                {
                    InsertLog(ActionType.编辑, "修改系统设置");
                }

                if (list.Count > 0)
                    return new XTJsonResult(result ? 1 : 0, (result ? "提交成功" : string.Format("{0} 项提交成功，{1} 项提交失败", successCount, list.Count - successCount)));
                else
                    return new XTJsonResult(0, "提交失败");
            }
            catch
            {
                return new XTJsonResult(0, "出现未知错误");
            }
        }



        #endregion

        #region 广告图
        [Permission("页面广告设置", ActionName = "AdManager", Rank = 2)]
        [SysManagerAuthorize(true, "System|AdManager")]
        public ActionResult AdList(PagedModel pModel, string cmd = "", int AdType = -1)
        {
            if (cmd == "GetList")
            {
                var data = AdDAL.GetList(pModel.PageStart, pModel.PageLength, AdType);
                return new XTJsonResult(1, string.Empty, data);
            }

            InsertLog(ActionType.浏览, "浏览页面广告列表");
            ViewBag.AdTypeList = AdDAL.GetAdTypeList();

            return View();
        }

        /// <summary>
        /// 编辑/添加
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        [HttpGet]
        [SysManagerAuthorize(true, "System|AdManager")]
        public ActionResult EditAd(int id = 0)
        {
            Sys_Ad model = AdDAL.GetModel(id) ?? new Sys_Ad();
            ViewBag.AdTypeList = AdDAL.GetAdTypeList();

            return View(model);
        }

        

        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SysManagerAuthorize(true, "System|AdManager")]
        public ActionResult DelAd(int id)
        {
            bool result = AdDAL.Del(id);

            if (result)
                InsertLog(ActionType.删除, string.Format("删除页面广告，Id：{0}", id));

            return new XTJsonResult(result ? 1 : 0, result ? "操作成功" : "操作失败");
        }
        #endregion

        public ActionResult RemoveCache()
        {
            if (Request.HttpMethod == "POST")
            {
                string removeType = GetFormParaValue("RemoveType");
                string key = GetFormParaValue("Key");
                ICacheManager cache = EngineContext.Current.Resolve<ICacheManager>("xt_cache_static");
                if (string.IsNullOrEmpty(key))
                {
                    cache.Clear();
                    return Content("已清除所有缓存数据");
                }
                else
                {
                    if (removeType == "2")
                    {
                        cache.RemoveByPattern(key);
                    }
                    else
                    {
                        cache.Remove(key);
                    }
                    return Content("已清除缓存【" + key + "】的数据");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetCacheValue(string key = "")
        {
            ICacheManager cache = EngineContext.Current.Resolve<ICacheManager>("xt_cache_static");
            if (string.IsNullOrEmpty(key))
            {
                return View(cache.Keys());
            }
            else
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Key = key, Value = cache.Get<object>(key) }, Formatting.Indented, timeFormat), "application/json", System.Text.Encoding.UTF8);
            }
        }
    }
}
