using Battery.Framework.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;

namespace Battery.Framework.Wen100Service
{
    public class Wen100Content
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }

    public class Wen100ListJson
    {
        /// <summary>
        /// 请求序号
        /// </summary>
        [JsonProperty(PropertyName = "sequenceNo")]
        public string SequenceNo { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        [JsonProperty(PropertyName = "totalRows")]
        public int TotalRows { get; set; }
        [JsonProperty(PropertyName = "contentList")]
        public List<Wen100Content> ContentList { get; set; }
    }

    public class Wen100
    {
        public static Wen100ListJson GetList(string keyWords, int currentPage = 1, int pageSize = 10, string sequenceNo = "")
        {
            if (string.IsNullOrEmpty(keyWords)) return new Wen100ListJson() { TotalRows = 0, SequenceNo = sequenceNo, ContentList = new List<Wen100Content>() };

            try
            {
                //keyWords = keyWords, Encoding.GetEncoding("GBK");
                string url = ConfigHelper.GetBranch("Wen100.SearchKngByKeyword");
                MemoryStream stream = new MemoryStream();
                var formDataBytes = Encoding.GetEncoding("GBK").GetBytes("{\"currentPage\":\"" + currentPage.ToString() + "\",\"pageSize\":\"" + pageSize.ToString() + "\",\"keywords\":\"" + keyWords + "\",\"sequenceNo\":\"" + sequenceNo + "\"}");
                stream.Write(formDataBytes, 0, formDataBytes.Length);
                stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置

                string jsonResult = RequestUtility.HttpPost(url, postStream: stream, timeOut: 3000, contentType: "application/json");
                return JsonConvert.DeserializeObject<Wen100ListJson>(jsonResult);
            }
            catch(Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>("ApiLogger").Error(ex, ex.Message, "Wen100_GetList");
                return new Wen100ListJson() { TotalRows = 0, SequenceNo = sequenceNo, ContentList = new List<Wen100Content>() };
            }
        }
        public static Wen100Content GetContent(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            try
            {
                string url = ConfigHelper.GetBranch("Wen100.SearchKngById");
                MemoryStream stream = new MemoryStream();
                var formDataBytes = Encoding.GetEncoding("GBK").GetBytes("{\"id\":\"" + id + "\"}");
                stream.Write(formDataBytes, 0, formDataBytes.Length);
                stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置

                string jsonResult = RequestUtility.HttpPost(url, postStream: stream, timeOut: 3000, contentType: "application/json");
                var r = JsonConvert.DeserializeObject<Wen100Content>(jsonResult);
                r.Id = id;
                return r;
            }
            catch(Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>("ApiLogger").Error(ex, ex.Message, "Wen100_GetList");
                return null;
            }
        }
    }
}
