using Battery.DAL.Log;
using Battery.DAL.Sys;
using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Framework.Domain;
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
using Battery.Model.Battery;
using Battery.DAL.Battery;
using Battery.Areas.SysManager.Models;
using AutoMapper;
using Battery.Model;

namespace Battery.Areas.SysManager.Controllers
{
    public class ProductController : SysBaseController
    {
        // GET: Battery/Product
        [Permission("产品管理", ActionName = "ProductList", ControllerName = "Product", Rank = 1)]
        [SysManagerAuthorize(true, "Product|ProductList")]
        public ActionResult ProductList(PagedModel pModel, string Num = "",int TypeID=-100, int Voltage = -100,int FactoryID=-100, int State = -100, string cmd = "")
        {
            List<Sys_Data> getSys_DataList = CommonController.GetSys_DataList("", "Voltage", 0);
            List<ProductType> get_ProductTypeList = ProductDAL.GetProductTypeList();
            List<Factorys> get_factory = ProductDAL.GetFactorysList();
            ViewBag.sd = getSys_DataList;
            ViewBag.gp = get_ProductTypeList;
            ViewBag.gf = get_factory;
            if (cmd == "GetList")
            {
                Tuple<int, List<Products>> data = ProductDAL.GetProductList(pModel.PageStart, pModel.PageLength, Num,TypeID, Voltage,FactoryID, State);

                return new XTJsonResult(1, string.Empty, data);
            }
            return View();

        }


        [Permission("产品类型管理", ActionName = "ProductTypeList", ControllerName = "Product", Rank = 1)]
        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult ProductTypeList(PagedModel pModel, string TypeCode="", int Voltage=-1, int state = -100, string cmd = "")
        {
            List<Sys_Data> GetSys_DataList = CommonController.GetSys_DataList("", "Voltage",0);
            if (cmd == "GetList")
            {
                Tuple<int, List<ProductType>> data = ProductDAL.GetProductTypeList(pModel.PageStart, pModel.PageLength, TypeCode, Voltage, state);

                return new XTJsonResult(1, string.Empty, data);
            }
            return View(GetSys_DataList);
        }

        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult ProductTypeDetail(int id)
        {
            ProductType model = ProductDAL.GetProductTypeModel(id);
            List<Sys_Data> GetSys_DataList = CommonController.GetSys_DataList("", "Voltage", 0);
            ViewBag.sd = GetSys_DataList;
            if (model == null)
            {
                return Content("类型不存在");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult EditProductType(ProductTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return new XTJsonResult(-1, ModelState.Where(m => m.Value.Errors.Count > 0).First().Value.Errors[0].ErrorMessage);
            }
            else
            {
                Mapper.Reset();
                Mapper.Initialize(x => x.CreateMap<ProductTypeModel, ProductType>());
                ProductType person = Mapper.Map<ProductTypeModel, ProductType>(model);
                
                int result = ProductDAL.EditProductType(person);
                if (result > 0)
                    InsertLog(ActionType.编辑, "修改产品类型【" + person.TypeCode + "】");

                return new XTJsonResult(result > 0 ? 1 : 0, result > 0 ? "保存成功" : "保存失败");
            }
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult SetTypeState(int id, int state)
        {
            InsertLog(ActionType.变更状态, "变更【" + id.ToString() + "】的商品状态为【" + state.ToString() + "】");
            int res = ProductDAL.SetTypeState(id, state) > 0 ? 1 : 0;

            return new XTJsonResult(res);
        }

        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult ScanAdd(string cmd = "")
        {
            if (cmd == "GetList")
            {
                List<Products> data = ProductDAL.GetProductListForScanAdd(DateTime.Now, CurrentUser.UserId, 100);

                return new XTJsonResult(1, string.Empty, data);
            }
            return View();
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult ScanCodeAdd(string productnum="")  //产品编码先拟定了一个临时的，如:P LWG001 36 450000 HN001 201811291011 0000000001    P代表产品，LWG001 是型号代码 36是电压 450000是地区代码 HN001 是厂家代码 201811291324 是出厂日期和时间 0000000001 是一个10位序列号
        {
            InsertLog(ActionType.新增, "扫码新增产品【" + productnum + "】");
            int res = 0;
            if (productnum != null&&productnum.Length==42)
            {
                string typecode = productnum.Substring(1,6);
                string voltage = productnum.Substring(7, 2);
                string area = productnum.Substring(9, 6);
                string factory = productnum.Substring(15, 5);
                string outtime = productnum.Substring(20, 12);

                DateTime ftime = new DateTime();
                string date = outtime.Substring(0, 4) + "/" + outtime.Substring(4, 2) + "/" + outtime.Substring(6, 2) + " " + outtime.Substring(8, 2) + ":" + outtime.Substring(10, 2);
                ftime = Convert.ToDateTime(date);
                res = ProductDAL.ScanAddProduct(productnum, typecode, voltage, area, factory, ftime, CurrentUser.UserId);
            }
            else
            {
                res = -2;
            }

            return new XTJsonResult(res);
        }

        
        public ActionResult ScanAddtest()
        {
            return View();
        }

        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult PeopleAdd(string cmd = "")
        {
            if (cmd == "GetList")
            {
                List<Products> data = ProductDAL.GetProductListForScanAdd(DateTime.Now, CurrentUser.UserId, 100);

                return new XTJsonResult(1, string.Empty, data);
            }
            return View();
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult PeopleCodeAdd(string productnum = "")  //产品编码先拟定了一个临时的，如:P LWG001 36 450000 HN001 201811291011 0000000001    P代表产品，LWG001 是型号代码 36是电压 450000是地区代码 HN001 是厂家代码 201811291324 是出厂日期和时间 0000000001 是一个10位序列号
        {
            InsertLog(ActionType.新增, "手动新增产品【" + productnum + "】");
            int res = 0;
            if (productnum != null && productnum.Length == 42)
            {
                string typecode = productnum.Substring(1, 6);
                string voltage = productnum.Substring(7, 2);
                string area = productnum.Substring(9, 6);
                string factory = productnum.Substring(15, 5);
                string outtime = productnum.Substring(20, 12);

                DateTime ftime = new DateTime();
                string date = outtime.Substring(0, 4) + "/" + outtime.Substring(4, 2) + "/" + outtime.Substring(6, 2) + " " + outtime.Substring(8, 2) + ":" + outtime.Substring(10, 2);
                ftime = Convert.ToDateTime(date);
                res = ProductDAL.ScanAddProduct(productnum, typecode, voltage, area, factory, ftime, CurrentUser.UserId);
            }
            else {
                res = -2;
            }

            return new XTJsonResult(res);
        }

        [SysManagerAuthorize(true, "Product|ProductList")]
        public ActionResult ProductDetail(int ID)
        {
            //ProductType model = ProductDAL.GetProductModel(id);

            //if (model == null)
            //{
            //    return Content("产品不存在");
            //}
            //return View(model);
            ViewBag.Id = ID;
            return View();
        }

        [SysManagerAuthorize(true, "Product|ProductTypeList")]
        public ActionResult ProductUserInfo(int ID)
        {
            ProductUserInfo data = ProductDAL.GetUserInfo(ID);
           return View();
        }

        [Permission("安装产品", ActionName = "UserProduct", ControllerName = "Product", Rank = 1)]
        [SysManagerAuthorize(true, "Product|UserProduct")]
        public ActionResult UserProduct()
        {
            return View();
        }

        [HttpPost]
        [SysManagerAuthorize(true, "Product|UserProduct")]
        public ActionResult UserProduct(string Num,string UserName,string UserTel,string WorkUnit,string Age,string UseBike,string UserWeChat,string UserQQ,string UserEMail,string InstallCost,int UserSex,string Career,string Education,string KonwStyle) 
        {
            InsertLog(ActionType.安装产品, "安装产品【" + Num + "】");
            EndResult res = new EndResult();

            res = ProductDAL.UserProduct(Num, UserName, UserTel, WorkUnit, Convert.ToInt32(Age), UseBike, UserWeChat, UserQQ, UserEMail, InstallCost,UserSex, Career,Education, KonwStyle,CurrentUser.UserId);

            string msg = "解锁码："+res.ActivationCode;
            if (res.ActivationCode == "Error")
            {
                switch (res.EndString)
                {
                    case 1:
                        msg = "系统检测到该产品有异常的安装记录，现该产品已彻底锁死，请联系管理员。";
                        break;
                    case 2:
                        msg = "系统检测到该产品已经处于使用状态，不能重复安装，请联系管理员。";
                        break;
                    case 4:
                        msg = "系统检测到该产品已报废，不能安装，如有疑问，请联系管理员。";
                        break;
                }
            }

            return new XTJsonResult(res.ActivationCode=="Error"?1:0, msg);
        }
    }
}