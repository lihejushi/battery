using Support.File.Code;
using Support.File.Code.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Support.File
{
    public partial class uploadfile : System.Web.UI.Page
    {
        DefaultConfig defaultConfig = new DefaultConfig();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string ServiceName = Request["Code"] ?? "CTMP";
                if (string.IsNullOrEmpty(Request["CropImg"]) == false) //剪切图片
                    new FileService(FileServiceManage.GetService(ServiceName, defaultConfig)).CropImg(this.Context);
                else if (string.IsNullOrEmpty(Request["ConstrainImg"]) == false) //文件移动
                    new FileService(FileServiceManage.GetService(ServiceName, defaultConfig)).ConstrainImg(this.Context);
                else if (string.IsNullOrEmpty(Request["MoveFile"]) == false) //文件移动
                    new FileService(FileServiceManage.GetService(ServiceName, defaultConfig)).MoveFile(this.Context);
                else
                    new FileService(FileServiceManage.GetService(ServiceName, defaultConfig)).UploadFile(this.Context);
            }
            catch (Exception ex)
            {
                XLog.XTrace.WriteLine("{0} \r\n {1}", ex.Message, ex.StackTrace);
                Output("Error");
            }
        }
        private void Output(string state)
        {
            Response.Clear();
            Response.Write(state);
        }
    }//class end

    public class DefaultConfig : IFileServiceConfig
    {
        string FileRootPath = System.Configuration.ConfigurationManager.AppSettings["SaveFileRootPath"];


        #region IFileServiceConfig 成员
        /// <summary>
        /// 组装上传文件保存路径
        /// </summary>
        /// <param name="targetDir">目标文件夹</param>
        /// <param name="filename">文件</param>
        /// <returns>文件完整路径</returns>
        public string UploadFileConfig(string targetDir, string filename)
        {
            string targetPath = FileRootPath + targetDir + "\\" + filename;
            return targetPath;
        }
        /// <summary>
        /// 获取上传文件返回路径
        /// </summary>
        /// <param name="argument">相关参数</param>
        /// <returns>返回结果</returns>
        public string UploadFileReturnResult(string filePath, string targetDir, string filename)
        {
            return filePath.Remove(0, FileRootPath.Length - 1);
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="targetDir">保存文件夹</param>
        /// <param name="sourceDir">源文件文件夹</param>
        /// <param name="filename">文件名</param>
        /// <returns>{Item1:"源文件地址",Item2:"目标文件夹路径"}</returns>
        public Tuple<string, string> CropImgConfig(string targetDir, string sourceDir, string filename)
        {
            string filePath = FileRootPath + sourceDir + filename;
            targetDir = FileRootPath + targetDir + "\\";
            return new Tuple<string, string>(filePath, targetDir);
        }

        /// <summary>
        /// 文件保存路径
        /// </summary>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="targetDir">目标文件夹</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public string CropImgReturnResult(string filePath, string targetDir, string filename)
        {
            return filePath.Remove(0, FileRootPath.Length - 1);
        }

        public Tuple<string, string> MoveFile(string targetDir, string sourceDir, string SourceFileName, string SaveFileName)
        {
            string sourcePath = FileRootPath + sourceDir + "\\" + SourceFileName;
            string targetPath = FileRootPath + targetDir + "\\" + SaveFileName;
            return new Tuple<string, string>(sourcePath, targetPath);
        }

        #endregion
    }
}