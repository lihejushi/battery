using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XT.MVC.Core.Common;
using XT.MVC.Core.Encrypt;

namespace Battery.Framework.Tools
{

    /// <summary>
    ///  资源管理
    /// </summary>
    public class FileUpload
    {
        #region 上传文件
        /// <summary>
        ///  上传文件至服务器（重命名）
        /// </summary>
        /// <param name="fileName">原始文件名（带扩展名）</param>
        /// <param name="saveDire">存储路径（可多级：A\B\C）</param>
        /// <param name="buffer">文件流</param>
        /// <param name="isCoverFile">是否强制覆盖原文件</param>
        public static string UploadFile(string fileName, string saveDire, byte[] buffer, bool isCoverFile, string identity = null, string productCode = null)
        {
            string saveFileName = MD5Helper.Md5Hash32(buffer) + Path.GetExtension(fileName);
            return UploadFile(buffer, saveFileName, "Battery/"+saveDire, isCoverFile, identity, productCode);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename">原始文件名</param>
        /// <param name="buffer">文件流</param>
        /// <param name="md5key">md5key</param>
        /// <returns></returns>
        public static string UploadFile(string filename, byte[] buffer, string md5key, string identity = null, string productCode = null)
        {
            return "";
        }

        /// <summary>
        ///  上传文件至服务器（按指定名称）
        /// </summary>
        /// <param name="buffer">文件流</param>
        /// <param name="saveFileName">存储文件名（带扩展名）</param>
        /// <param name="saveDire">存储路径（可多级：A\B\C）</param>
        /// <param name="isCoverFile">是否强制覆盖原文件</param>
        public static string UploadFile(byte[] buffer, string saveFileName, string saveDire, bool isCoverFile, string identity = null, string productCode = null)
        {
            return UploadFile(saveFileName, saveDire, buffer, isCoverFile, null, identity, productCode);
        }
        /// <summary>
        ///  上传文件至服务器
        /// </summary>
        /// <param name="postUrl">服务器地址</param>
        /// <param name="saveFileName">存储文件名（带扩展名）</param>
        /// <param name="saveDire">存储路径（可多级：A\B\C）</param>
        /// <param name="buffer">文件流</param>
        /// <param name="IsCoverFile">是否强制覆盖原文件</param>
        /// <returns></returns>
        public static string UploadFile(string saveFileName, string saveDire, byte[] buffer, bool IsCoverFile, string postUrl = null, string identity = null, string productCode = null)
        {
            if (postUrl == null)
                postUrl = CDNConfig.FileServerUrl;

            string webName = "";
            int FileLengs = buffer.Length;
            //建立缓存（200K）
            int cacheLen = 1024 * 200;
            byte[] byteCache = new byte[cacheLen];

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                int Position = 0;
                int WriteLeng = ms.Read(byteCache, 0, cacheLen);
                while (WriteLeng > 0)
                {
                    webName = webUpload(postUrl, saveFileName, saveDire, byteCache, FileLengs, Position, WriteLeng, IsCoverFile, identity, productCode);
                    if (webName.IndexOf("$FileExists$") != -1)
                    {
                        webName = webName.Replace("$FileExists$", "");
                        break;
                    }
                    Position += WriteLeng;
                    WriteLeng = ms.Read(byteCache, 0, cacheLen);
                }
                ms.Close();
            }

            return webName;
        }
        /// <summary>
        ///  分段传递文件
        /// </summary>
        /// <param name="postUrl">请求地址</param>
        /// <param name="saveFileName">保存文件名（还扩展名）</param>
        /// <param name="saveDire">保存路径</param>
        /// <param name="buffer">文件流</param>
        /// <param name="FileLengs">文件总长度</param>
        /// <param name="Position">偏移量</param>
        /// <param name="WriteLeng">此次传递长度</param>
        /// <param name="IsCoverFile">是否强制覆盖原文件</param>
        /// <returns></returns>
        private static string webUpload(string postUrl, string saveFileName, string saveDire, byte[] buffer, int FileLengs, int Position, int WriteLeng, bool IsCoverFile, string identity = null, string productCode = null)
        {
            try
            {
                WebClient webClient = getWebClient(identity, productCode);
                webClient.Credentials = CredentialCache.DefaultCredentials;
                webClient.QueryString.Add("SaveFileName", saveFileName);
                webClient.QueryString.Add("SaveDire1", saveDire);
                if (IsCoverFile == true) //覆盖原文件
                    webClient.QueryString.Add("CoverFile", "true");
                webClient.QueryString.Add("FileLengs", FileLengs.ToString());
                webClient.QueryString.Add("BytePosition", Position.ToString());
                webClient.QueryString.Add("ByteWriteLeng", WriteLeng.ToString());

                byte[] responseArray = webClient.UploadData(postUrl, "POST", buffer);
                return System.Text.Encoding.ASCII.GetString(responseArray);
            }
            catch(Exception ex)
            {
                return "";
            }
        }


        /// <summary>
        /// 批量上传文件至本地文件夹，上传excel
        /// </summary>
        /// <returns>存储路径</returns>
        public static string UploadFileToCurrentDomain(string filename, byte[] buffer, string directory, string identity = null, string productCode = null)
        {
            string filePath = "";
            //扩展名
            string extName = Path.GetExtension(filename);
            //文件名（MD5值+扩展名）
            string fileMD5key = MD5Helper.Md5Hash32(buffer) + extName;
            //完整文件路径
            filePath = directory + "\\" + fileMD5key;
            //文件存在直接返回显示路径
            if (File.Exists(filePath)) return filePath;
            FileStream fStream = null;
            try
            {
                string newDire = Path.GetDirectoryName(filePath);
                //创建目录
                if (!Directory.Exists(newDire)) Directory.CreateDirectory(newDire);
                fStream = new FileStream(filePath, FileMode.OpenOrCreate);
                fStream.Write(buffer, 0, buffer.Length);
                fStream.Close();
                return filePath;
            }
            catch (Exception ex)
            {

                // XLog.XTrace.WriteLine("对象：{0} \r\n 消息：{1}\r\n堆栈：{2}", ex.Source, ex.Message, ex.StackTrace);
                return "";
            }
            finally
            {
                if (fStream != null) fStream.Close();
            }
        }
        #endregion

        #region 剪切图片
        /// <summary>
        ///  剪切图片
        /// </summary>
        /// <param name="SaveDire1">存储图片路径（可多级：A\B\C）</param>
        /// <param name="SaveDire2">原始图片路径（可多级：A\B\C）</param>
        /// <param name="PicPath">原始图片名（带扩展名）</param>
        /// <param name="PicExt">剪切后图片格式（.jpg/.png/.gif）</param>
        public static string CropPicture(string SaveDire1, string SaveDire2, string PicPath, string PicExt, int x, int y, int w, int h, int pw, int ph, int tw, int th, string postUrl = null, string identity = null, string productCode = null)
        {
            try
            {
                if (postUrl == null)
                    postUrl = CDNConfig.FileServerUrl;

                WebClient webClient = getWebClient(identity, productCode);
                webClient.Credentials = CredentialCache.DefaultCredentials;

                webClient.QueryString.Add("SaveDire1", "Battery/" + SaveDire1);
                webClient.QueryString.Add("SaveDire2", SaveDire2);
                webClient.QueryString.Add("PicPath", PicPath);

                webClient.QueryString.Add("x", x.ToString());
                webClient.QueryString.Add("y", y.ToString());
                webClient.QueryString.Add("w", w.ToString());
                webClient.QueryString.Add("h", h.ToString());
                webClient.QueryString.Add("pw", pw.ToString());
                webClient.QueryString.Add("ph", ph.ToString());
                webClient.QueryString.Add("tw", tw.ToString());
                webClient.QueryString.Add("th", th.ToString());
                webClient.QueryString.Add("ext", PicExt);

                System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();
                data.Add("CropImg", "CropImg");
                byte[] responseArray = webClient.UploadValues(postUrl, "POST", data);
                return System.Text.Encoding.ASCII.GetString(responseArray);
            }
            catch
            {
                return "Error";
            }
        }
        #endregion

        #region 文件移动
        /// <summary>
        ///  移动服务器文件
        /// </summary>
        /// <param name="SaveDire1">存储路径（可多级：A\B\C）</param>
        /// <param name="SaveDire2">原始路径（可多级：A\B\C）</param>
        /// <param name="SaveFileName">存储文件名（带扩展名）</param>
        /// <param name="SourceFileName">原始文件名（带扩展名）</param>
        /// <param name="ClearAllFile">移动前，是否清除目标路径下所有文件，包括子目录</param>
        /// <returns></returns>
        public static bool MoveFile(string SaveDire1, string SaveDire2, string SaveFileName, string SourceFileName, bool ClearAllFile, string postUrl = null, string identity = null, string productCode = null)
        {
            try
            {
                if (postUrl == null)
                    postUrl = CDNConfig.FileServerUrl;

                WebClient webClient = getWebClient(identity, productCode);
                webClient.Credentials = CredentialCache.DefaultCredentials;

                System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();

                data.Add("MoveFile", "MoveFile");
                data.Add("SaveDire1", SaveDire1);
                data.Add("SaveDire2", SaveDire2);
                data.Add("SaveFileName", SaveFileName);
                data.Add("SourceFileName", SourceFileName);
                if (ClearAllFile == true)
                    data.Add("ClearAllFile", "true");

                byte[] responseArray = webClient.UploadValues(postUrl, "POST", data);
                string responseTxt = System.Text.Encoding.ASCII.GetString(responseArray);
                return responseTxt == "Success";
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 删除文件
        public static bool DeleteFile(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if (attr == FileAttributes.Directory)
                {
                    if(Directory.Exists(path))
                        Directory.Delete(path, true);
                }
                else
                {
                    if(File.Exists(path))
                        File.Delete(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        static WebClient getWebClient(string identity = null, string productCode = null)
        {
            WebClient webClient = new WebClient();

            if (identity == null)
                identity = ConfigHelper.GetAppConfig("FileUploadIdentity");
            webClient.QueryString.Add("Identity", identity);

            if (productCode == null)
                productCode = ConfigHelper.GetAppConfig("XT.ProductCode");
            webClient.QueryString.Add("ProductCode", productCode);

            return webClient;
        }
    }
}
