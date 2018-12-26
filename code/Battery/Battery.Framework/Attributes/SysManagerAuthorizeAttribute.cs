using Battery.DAL.Sys;
using Battery.Framework.Domain;
using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Framework;
using XT.MVC.Framework.Results;

namespace Battery.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SysManagerAuthorizeAttribute :ActionFilterAttribute, IAuthorizationFilter
    {
        private List<string> _checkPermissions = new List<string>();
        private bool _isCheck = false;

        public bool IsCheck
        {
            get { return _isCheck; }
            set { _isCheck = value; }
        }
        public List<string> CheckPermissions
        {
            get { return _checkPermissions; }
            set { _checkPermissions = value; }
        }

        public SysManagerAuthorizeAttribute(bool isCheck, params string[] permissions)
        {
            IsCheck = isCheck;
            if (permissions != null && permissions.Length > 0) CheckPermissions = permissions.ToList();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;


            if (workContext == null || workContext.IsLogin == false ||  /*检查是否登陆*/
                appContext.Platform != PlatformType.ChinaTelcom || appContext.IsAdmin != true ||    /*检查登陆平台是否正确*/
                userContext.UserType != UserType.Sys    /*检查用户类型是否正确*/
                )
            {
                filterContext.Controller.ViewData["MenuList"] = null;
                base.OnActionExecuting(filterContext);
            }
            else
            {
                if (filterContext != null
                    && filterContext.HttpContext != null
                    && filterContext.IsChildAction == false
                    && filterContext.HttpContext.Request.IsAjaxRequest() == false)
                {
                    #region 获取菜单
                    List<Sys_Menu> allMenu = PermissionDAL.GetMenuList();
                    List<Sys_Role_Permission> allPermission = PermissionDAL.GetPersonPermission(userContext.UserId);


                    List<Sys_Menu> childList = allMenu.Where(m =>
                    {
                        string viewPermission = m.ViewPermission;
                        if (string.IsNullOrEmpty(viewPermission) || viewPermission == "MainMenu") return false;
                        else
                        {
                            if (viewPermission.Split('|').Count() != 2) return false;
                            else
                            {
                                string actionName = viewPermission.Split('|')[1];
                                string controllerName = viewPermission.Split('|')[0];
                                bool s = allPermission.Exists(per => per.Action.ToLower() == actionName.ToLower() && per.Controller.ToLower() == controllerName.ToLower());
                                return allPermission.Exists(per => per.Action.ToLower() == actionName.ToLower() && per.Controller.ToLower() == controllerName.ToLower());
                            }
                        }
                    }).ToList();

                    List<Sys_Menu> parentList = allMenu.Where(m => childList.Exists(menu => menu.ParentId == m.Id)).ToList();
                    foreach (var parent in parentList)
                    {
                        parent.ChildMenus = new List<Sys_Menu>();
                        parent.ChildMenus.AddRange(childList.Where(m => m.ParentId == parent.Id).ToArray());
                    }
                    filterContext.Controller.ViewData["MenuList"] = parentList;
                    #endregion
                }
                base.OnActionExecuting(filterContext);
            }
        }
        public virtual bool CheckAuthorize(AuthorizationContext filterContext)
        {

            //检查登录状态
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;


            if (workContext == null || workContext.IsLogin == false ||  /*检查是否登陆*/
                appContext.Platform != PlatformType.ChinaTelcom || appContext.IsAdmin != true ||    /*检查登陆平台是否正确*/
                userContext.UserType != UserType.Sys    /*检查用户类型是否正确*/
                ) return false; 
            //检查必要权限，若没有需要检查的权限则只检查登录状态
            if (CheckPermissions == null || CheckPermissions.Count == 0) return true;
            else
            {
                int existsPermission = 0;
                List<Sys_Role_Permission> allPermission = PermissionDAL.GetPersonPermission(userContext.UserId);
                foreach (var item in CheckPermissions.Where(m => string.IsNullOrEmpty(m) == false))
                {
                    string[] args = item.Split('|');
                    if (args.Count() == 2 && allPermission.Exists(m => m.Action.ToLower() == args[1].ToLower() && m.Controller.ToLower() == args[0].ToLower()))
                    {
                        existsPermission++;
                    }
                }


                return existsPermission == CheckPermissions.Count();
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsCheck == false)
                return;

            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");

            if (IsCheck == true && CheckAuthorize(filterContext) == false)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    Result result = new Result() { Code = -98, Message = "没有访问此接口的权限", Data = string.Empty };
                    filterContext.Result = new XTJsonResult(-98, "", string.Empty, "yyyy-MM-dd HH:mm:ss");
                }
                else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                {
                    filterContext.Result = new ContentResult() { Content = "没有访问此接口的权限", ContentEncoding = Encoding.UTF8 };
                }
                else
                {
                    //filterContext.Result = new RedirectResult(string.Format("~/Admin/Common/NotHasAuthorize?backurl={0}&accesstype={1}&open=self", System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl), "Admin"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Common",
                        action = "NotHasAuthorize",
                        backurl = filterContext.HttpContext.Request.RawUrl,
                        area = "SysManager"
                    }));
                }
            }
        }
    }
}
