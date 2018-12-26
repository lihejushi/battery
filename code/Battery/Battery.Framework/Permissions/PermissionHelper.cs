using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Infrastructure;

namespace Battery.Framework.Permissions
{
    public class PermissionHelper
    {
        private const string CacheKey = "CTMP.Permission";
        public static List<PermissionModel> GetPermission()
        {
            List<PermissionModel> list = new List<PermissionModel>();
            var cache = EngineContext.Current.Resolve<ICacheManager>();

            if (cache.IsSet(CacheKey))
            {
                list = cache.Get<List<PermissionModel>>(CacheKey);
            }
            else
            {
                ITypeFinder typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
                int i = 0;
                foreach (var item in typeFinder.GetAssemblies())
                {
                    var types = item.GetTypes().Where(m =>
                    {
                        return m.BaseType == typeof(SysBaseController) && m.IsClass == true;
                    }).ToList();
                    foreach (var type in types)
                    {
                        var methods = type.GetMethods().Where(m => m.IsConstructor == false && m.IsPublic == true);
                        foreach (var method in methods)
                        {
                            if (Attribute.IsDefined(method, typeof(PermissionAttribute)))
                            {
                                var attr = (PermissionAttribute)Attribute.GetCustomAttribute(method, typeof(PermissionAttribute));
                                string controllerName = string.IsNullOrEmpty(attr.ControllerName) ? type.Name.Replace("Controller", "") : attr.ControllerName;
                                string actionName = string.IsNullOrEmpty(attr.ActionName) ? method.Name : attr.ActionName;
                                if (list.Where(l => l.ControllerName == controllerName && l.ActionName == actionName).Count() == 0)
                                {
                                    list.Add(new PermissionModel()
                                    {
                                        ActionName = actionName,
                                        ControllerName = controllerName,
                                        Rank = attr.Rank,
                                        Memo = attr.Memo,
                                        Title = attr.Title
                                    }
                                    );
                                }


                            }
                        }
                    }
                }
                cache.Set(CacheKey, list, 1000 * 60 * 24);
            }
            return list;
        }
    }
}
