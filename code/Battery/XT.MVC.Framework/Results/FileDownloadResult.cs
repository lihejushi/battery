using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace XT.MVC.Framework.Results
{
    public class FileDownloadResult : ActionResult
    {
        public FileDownloadResult(string contentType, string fileDownloadName, byte[] file, string charset = "utf-8")
        {
            ContentType = contentType;
            FileDownloadName = fileDownloadName;
            File = file;
            Charset = charset;
        }

        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
        public byte[] File { get; set; }
        public string Charset { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Charset =  Charset ?? "utf-8";
            context.HttpContext.Response.ContentType = ContentType;
            context.HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding(context.HttpContext.Response.Charset);
            context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", FileDownloadName));
            context.HttpContext.Response.BinaryWrite(File);
            context.HttpContext.Response.End();
        }
    }
}
