using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using XT.MVC.Framework.Controllers;

namespace Battery.Framework.Core.Controllers
{
    public class GobalController : BaseController
    {
        public ActionResult Index()
        {
            return Content("全局配置");
        }
    }
}
