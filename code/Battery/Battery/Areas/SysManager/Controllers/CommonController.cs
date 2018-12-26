using Battery.DAL.Sys;
using Battery.Framework;
using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Framework.Tools;
using Battery.Model.Battery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XT.MVC.Core.Common;
using XT.MVC.Framework.Results;

namespace Battery.Areas.SysManager.Controllers
{
    public class CommonController : SysBaseController
    {
        #region 参数
        /// <summary>
        /// 原始文件路径
        /// </summary>
        public string SourceSrc = "";   //源文件路径
        public string T_Width = "480";  //目标宽度
        public string T_Height = "600"; //目标高度
        public string SavePath = "";    //保存路径
        public string PicExt = "";   //文件格式
        public string ImgType = "";     //图片使用类型
        public int S_Width = 0;
        public int S_Height = 0;
        public double T_Scale = 0;

        public string FileJson = "";
        String fileTypes = "jpg,jpeg,png";
        #endregion

        public ActionResult NotHasAuthorize()
        {
            return View();
        }
        public ActionResult ManagerPageError()
        {
            return View();
        }

        #region 上传图片

        [HttpGet]
        public ActionResult Cropimg()
        {
            if (string.IsNullOrEmpty(Request["SavePath"]) == false)
                SavePath = Request["SavePath"];
            if (string.IsNullOrEmpty(Request["ext"]) == false)
                PicExt = Request["ext"];
            SourceSrc = Server.UrlDecode(Request["src"]);
            //计算缩放比例
            if (int.TryParse(Request["w"], out S_Width) && int.TryParse(Request["h"], out S_Height))
            {
                ViewData["T_Width"] = S_Width.ToString();
                ViewData["T_Height"] = S_Height.ToString();
                T_Scale = Convert.ToDouble(S_Width) / Convert.ToDouble(S_Height);
                ViewData["T_Scale"] = Math.Round(100 / T_Scale, 0);
                ViewData["T_Scale2"] = T_Scale;
                ViewData["Ext"] = PicExt;
            }
            ViewData["SavePath"] = SavePath;
            ImgType = Server.UrlDecode(Request["type"]);
            ViewData["FileDomain"] = CDNConfig.FileDomain;
            return View();
        }

        [HttpPost]
        public ActionResult UpImage()
        {
            HttpPostedFileBase file = Request.Files[0];
            if (file == null)
                return Content(JsonConvert.SerializeObject(new { code = -1, message = "请选择图片" }));
            if (file.ContentLength > 2 * 1024 * 1024)
                return Content(JsonConvert.SerializeObject(new { code = -2, message = "图片大小超过2M！" }));
            String fileName = file.FileName;
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1)) == -1)
                return Content(JsonConvert.SerializeObject(new { code = -3, message = "图片格式不符合规定！" }));
            int countBytes = file.ContentLength;
            byte[] buffer = new byte[countBytes];
            file.InputStream.Read(buffer, 0, countBytes);
            ////开始上传
            string SavePath = DateTime.Now.ToString("yyyyMMdd");
            string saveUrl = FileUpload.UploadFile(fileName, "large" + "/" + SavePath, buffer, false, null, null);

            if (string.IsNullOrEmpty(saveUrl) == true)
                return Content(JsonConvert.SerializeObject(new { code = 0, message = "上传失败！" }));
            else
                return Content(JsonConvert.SerializeObject(new { code = 1, message = "", data = saveUrl }));
        }

        [HttpPost]
        [ActionName("Cropimg")]
        public ActionResult ExecCropImg()
        {
            string PicPath = Convert.ToString(Request["p"]);
            int PointX = Convert.ToInt32(Request["x"]);
            int PointY = Convert.ToInt32(Request["y"]);
            int CutWidth = Convert.ToInt32(Request["w"]);
            int CutHeight = Convert.ToInt32(Request["h"]);
            int PicWidth = Convert.ToInt32(Request["pw"]);
            int PicHeight = Convert.ToInt32(Request["ph"]);
            int targetWidth = Convert.ToInt32(Request["tw"]);
            int targetHeight = Convert.ToInt32(Request["th"]);
            string picext = Request["ext"];

            if (string.IsNullOrEmpty(picext))
            {
                if (PicPath.LastIndexOf('.') >= 0)
                {
                    var _ext = PicPath.Substring(PicPath.LastIndexOf('.'), PicPath.Length - PicPath.LastIndexOf('.'));
                    if (_ext.ToLower() == ".png") picext = "png";
                }
                else
                {
                    picext = "jpg";
                }
            }
            //picext = "jpg";
            //处理存储路径
            string SavePath = Request["SavePath"];
            if (string.IsNullOrEmpty(SavePath) == true)
                SavePath = DateTime.Now.ToString("yyyyMMdd");
            string cropImgName = FileUpload.CropPicture(SavePath, "", PicPath, picext, PointX, PointY, CutWidth, CutHeight, PicWidth, PicHeight, targetWidth, targetHeight, null, null, null);
            if (cropImgName == "Error")
                return Content(JsonConvert.SerializeObject(new { code = 0, message = "图片保存失败" }));
            else
                return Content(JsonConvert.SerializeObject(new { code = 1, message = "", data = cropImgName }));
        }

        #endregion
        #region 模板
        /// <summary>
        /// 选择导航
        /// </summary>
        /// <param name="navType">0  显示全部链接类型， 1 显示（APP、自定义连接），2 显示（营业厅，自定义连接）</param>
        /// <param name="appId">当navType=2时，传入 大于0的appId 默认企业Id</param>
        /// <param name="needImg">1 需要上传图片，0 不上传图片</param>
        /// <param name="imgWidth">图片宽度</param>
        /// <param name="imgHeight">图片高度</param>
        /// <returns></returns>
        [SysManagerAuthorize(true)]
        public ActionResult SelectNav(int navType = 0, int appId = 0, int needImg = 0, int imgWidth = 600, int imgHeight = 450, string imgExt = "jpg")
        {
            ViewBag.AppId = appId;
            ViewBag.NavType = navType;
            ViewBag.NeedImg = needImg;
            ViewBag.ImgWidth = imgWidth;
            ViewBag.ImgHeight = imgHeight;
            ViewBag.ImgExt = imgExt == "png" ? "png" : "jpg";
            var scale = Convert.ToDouble(Convert.ToDouble(imgWidth) / Convert.ToDouble(imgHeight));

            if (scale < 1)
            {
                ViewBag.ShowImgHeight = 200;

                ViewBag.ShowImgWidth = Convert.ToInt32(Math.Round(200 * scale, 0));
            }
            else
            {
                ViewBag.ShowImgWidth = 200;
                ViewBag.ShowImgHeight = Convert.ToInt32(Math.Round(200 / scale, 0));
            }
            return View();
        }
        [SysManagerAuthorize(true)]
        public ActionResult ChooseContent(string contentType, int QYID = 0)
        {
            ViewBag.ShopId = QYID;
            ViewBag.ContentType = contentType;
            return View();
        }

        /// <summary>
        /// 连接内容
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="contentType"></param>
        /// <param name="searchTitle"></param>
        /// <returns></returns>
        [SysManagerAuthorize(true)]
        public ActionResult GetContentList(PagedModel pModel, string contentType, string searchTitle, int shopId = 0)
        {
            var data = ContentDAL.GetList(pModel.PageStart, pModel.PageLength, contentType, searchTitle, shopId);

            return new XTJsonResult(1, string.Empty, data);
        }
        #endregion

        /// <summary>
        /// 获取系统参数表里数据
        /// </summary>
        /// <returns></returns>
        public static List<Sys_Data> GetSys_DataList(string key = "",string typeCode="",int state = -100)
        {
            List<Sys_Data> l_sd = new List<Sys_Data>();
            l_sd = CommonDAL.GetSys_DataList(key, typeCode, state);
            return l_sd;
        }
    }
}
