using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace XT.MVC.Framework.Results
{
    public class XTJsonResult : ActionResult
    {
        private Result _xtResult = new Result();

        private string _timeFormat = "yyyy-MM-dd HH:mm:ss";

        public string TimeFormat
        {
            get { return _timeFormat; }
            set { _timeFormat = value; }
        }

        public int Code
        {
            get { return _xtResult.Code; }
            set { _xtResult.Code = value; }
        }
        public string Message
        {
            get { return _xtResult.Message; }
            set { _xtResult.Message = value; }
        }
        public object Data
        {
            get { return _xtResult.Data; }
            set { _xtResult.Data = value; }
        }


        public XTJsonResult(int code, string message = "", object data = null, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            _xtResult.Code = code;
            _xtResult.Message = message;
            _xtResult.Data = data;

            _timeFormat = timeFormat;
        }
        public XTJsonResult(Result result, string timeFormat="yyyy-MM-dd HH:mm:ss")
        {
            _xtResult = result;
            _timeFormat = timeFormat;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            //参考 - http://stackoverflow.com/questions/15939944/jquery-post-json-fails-when-returning-null-from-asp-net-mvc

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;

            //this.Data = null;
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = TimeFormat;
            var serializedObject = JsonConvert.SerializeObject(_xtResult, Formatting.Indented, timeFormat);
            response.Write(serializedObject);
        }
    }
}
