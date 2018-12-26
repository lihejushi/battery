using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Configuration;
using System.Data;

namespace Battery.Framework.Tools
{
    public class UploadFiles:System.Web.UI.Page
    {
        string upPath = "";
        int domin = 0;
        string result="";       
        string updir = "";
        DataTable redt = null;
        public  UploadFiles(string filePath)
        {
            System.Web.HttpContext.Current.Response.CacheControl = "no-cache";
            //redt = new DataTable();  //new List<Eastar.Model.help.AskAndAnswer.TS_Attachment>(); ;
            //redt.Columns.Add("Title");
            //redt.Columns.Add("Content", typeof(byte[]));

            upPath = ConfigurationManager.AppSettings[filePath];

            if (upPath.IndexOf("http://") != -1)
            {
                domin = 1;
                updir = upPath + "//";
            }
            else
                updir = Server.MapPath(upPath) + "\\";
        }

        public string UploadModth(int id=-1,string fileTrueName="")
        { 
            string files="\"filelist\":[";//记录具体的上传情况
            int filesret=0;

            if (id != -1)
                updir += id.ToString() + "\\";
      
            //上传动作
            if (System.Web.HttpContext.Current.Request.Files.Count > 0)
            {
                //////////////////流///////////////////


                ///////////////////////////////////////
                string filesin =  "成功";
                int signfileret = 1;
                string dn = DateTime.Now.ToString("yyyy-MM-dd");
                for (int j = 0; j < System.Web.HttpContext.Current.Request.Files.Count; j++)
                {
                    HttpPostedFile uploadFile = System.Web.HttpContext.Current.Request.Files[j];
                    string filename = "";
                    try
                    {                        
                        bool fc =checkFileSize(uploadFile);
                        if ( fc==false)     //检查附件大小
                        {
                            //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "", "alert(\"非常抱歉\", '" + checkFileSize() + "文件过大);", true);
                            filesin+="文件过大或者读取出错！";
                            signfileret=0;
                        }
                        else
                        {
                            if (uploadFile.ContentLength > 0)
                            {
                                //如果文件夹不存在则先创建     注释
                                if (domin != 1)
                                {
                                    
                                    updir = updir + dn + "\\";
                                    if (!Directory.Exists(updir))
                                    {
                                        Directory.CreateDirectory(updir);
                                    }
                                }

                                //处理文件名
                                //string extname = Path.GetExtension(uploadFile.FileName);
                                //string fullname = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();                        
                                //string filename = uploadFile.FileName;

                                if (string.IsNullOrEmpty(fileTrueName))
                                    filename = DateTime.Now.Ticks + "$" + System.IO.Path.GetFileName(uploadFile.FileName.Replace("$",""));
                                else
                                    filename = fileTrueName +"$"+ DateTime.Now.Ticks + System.IO.Path.GetExtension(uploadFile.FileName.Replace("$", ""));

                                //上传
                                //string filePath = Server.MapPath(updir) + @"\" + filename;
                                //uploadFile.SaveAs(filePath);
                                if (domin == 1)
                                {
                                    ////////////////流//////////////////
                                    int fileLength = uploadFile.ContentLength;
                                    byte[] filebyte = new byte[fileLength];
                                    Stream fileStream = uploadFile.InputStream;
                                    //fileStream.Read(filebyte, 0, fileLength);
                                    //DataRow dr = redt.NewRow();
                                    //dr["Title"] = filename;
                                    //dr["Content"] = filebyte;
                                    //redt.Rows.Add(dr);
                                    //Eastar.Model.help.AskAndAnswer.TS_Attachment ts = new Eastar.Model.help.AskAndAnswer.TS_Attachment();
                                    //ts.aTitle = filename;
                                    //ts.aContent = filebyte;
                                    //redt.Add(ts);

                                    //Session["table"] = redt;
                                    //////////////////////////////////
                                }
                                else
                                {
                                    uploadFile.SaveAs(string.Format("{0}\\{1}", updir, filename));
                                }  
                            }                             
                        }   
                    }
                    catch (Exception ex)
                    {
                        filesin=ex.ToString();
                        signfileret=0;
                        filesret++;
                    }
                    files += "{\"fileName\":\"\\\\" + dn +"\\\\"+ filename + "\",\"fileret\":\"" + signfileret.ToString() + "\",\"message\":\"" + filesin + "\"},";
                }
                files = files.Substring(0, files.Length - 1);
            }
            files+="]";
            result = "{\"files\":\"" + System.Web.HttpContext.Current.Request.Files.Count + "\",\"success\":\"" + (System.Web.HttpContext.Current.Request.Files.Count - filesret).ToString() + "\"," + files + "}";
            return result;

        }

        protected bool checkFileSize(HttpPostedFile f)
        {
            bool filer = true;
            string sizestring = ConfigurationManager.AppSettings["FileSize"];
            int size = 0;
            if (string.IsNullOrEmpty(sizestring))
            //size = int.Parse(ConfigurationManager.AppSettings["FileSize"]);
            {
                size = int.Parse(sizestring);
            }
            else
            {
                size = 31457280;//默认是30M
            }
            try
            {
                if (f.ContentLength > size)   //单位是字节B
                {
                    //name = System.IO.Path.GetFileName(f.FileName) + "  ";
                    filer = false;
                }
            }
            catch (Exception ex)
            {
                //System.Web.HttpContext.Current.Response.Write(ex.Message);
                filer = false;
            }
            return filer;
        }
    }
}
