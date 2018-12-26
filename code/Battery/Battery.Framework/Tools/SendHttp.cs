using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battery.Framework.Tools
{
    //public class SendHttp
    //{
    #region http 带参数 请求
    public class MySoapClient_Param
    {
        private System.Net.HttpWebRequest m_Client = null;
        private Dictionary<string, string> xmlTemplate = new Dictionary<string, string>();

        /// <summary>  
        /// 默认构造函数  
        /// </summary>  
        public MySoapClient_Param(string requestUrl, Dictionary<string, string> dic)
        {
            xmlTemplate = dic;
            m_Client = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUrl);
            m_Client.Method = "POST";
            m_Client.Headers["SOAPAction"] = @"""""";               //双引号  
            m_Client.ContentType = "application/x-www-form-urlencoded";
            m_Client.Accept = "application/dime, multipart/related, text/*,application/soap+xml";
            m_Client.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 4.0.30319.42000)";
            m_Client.Headers["Cache-Control"] = "no-cache";
            m_Client.Headers["Pragma"] = "no-cache";
            //m_Client.Headers["X-Forwarded-For"] = "137.32.118.49";
        }
        public System.IO.StreamReader SendRequestByEncoding(Encoding encoding)
        {
            Dictionary<string, string> dic = xmlTemplate;
            try
            {
                //【发送】  
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                //byte[] data = Encoding.UTF8.GetBytes(System.Web.HttpUtility.UrlEncode(builder.ToString()));
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString().Replace("+", "%2B"));
                m_Client.ContentLength = data.Length;
                using (System.IO.Stream reqStream = m_Client.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

                ////【接收】
                System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)m_Client.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader reader = new System.IO.StreamReader(stream, encoding);
                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
    #endregion
    #region http请求 无参数请求
    public class MySoapClient
    {
        private System.Net.HttpWebRequest m_Client = null;
        private string xmlTemplate = string.Empty;

        /// <summary>  
        /// 默认构造函数  
        /// </summary>  
        public MySoapClient(string requestUrl, string requestXml)
        {
            xmlTemplate = requestXml;
            m_Client = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUrl);
            m_Client.Method = "POST";
            m_Client.Headers["SOAPAction"] = @"""""";               //双引号  
            m_Client.ContentType = "application/x-www-form-urlencoded";
            m_Client.Accept = "application/dime, multipart/related, text/*,application/soap+xml";
            m_Client.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 4.0.30319.42000)";
            m_Client.Headers["Cache-Control"] = "no-cache";
            m_Client.Headers["Pragma"] = "no-cache";
            //m_Client.Headers["X-Forwarded-For"] = "137.32.118.49";
        }
        public System.IO.StreamReader SendRequestByEncoding(Encoding encoding)
        {
            string sendXml = xmlTemplate;
            try
            {
                //【发送】   
                m_Client.ContentLength = Encoding.UTF8.GetBytes(sendXml).Length;
                System.IO.Stream sendStread = m_Client.GetRequestStream();
                byte[] sendData = new UTF8Encoding().GetBytes(sendXml);
                sendStread.Write(sendData, 0, sendData.Length);
                sendStread.Close();

                //StreamWriter myStreamWriter = new StreamWriter(sendStread, Encoding.GetEncoding("utf-8"));
                //myStreamWriter.Write(sendXml);
                //myStreamWriter.Close();
                //【接收】
                System.Net.HttpWebResponse myResponse = (System.Net.HttpWebResponse)m_Client.GetResponse();
                System.IO.StreamReader reader = new System.IO.StreamReader(myResponse.GetResponseStream(), encoding);
                //string content = reader.ReadToEnd();

                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
    #endregion
    //}
}
