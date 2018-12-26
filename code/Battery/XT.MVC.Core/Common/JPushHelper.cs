using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;

namespace XT.MVC.Core.Common
{
    public class JPushHelper
    {
        public const string POSTURL_V3 = "https://api.jpush.cn/v3/push";
        public static bool Push(string title, string message, string link, string appKey, string masterSecret)
        {
            try
            {
                string result = WebRequest_Response(appKey, masterSecret, POSTURL_V3, Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        platform = "all",
                        audience = "all",
                        notification = new
                        {
                            android = new
                            {
                                alert = message,
                                title = title,
                                builder_id = 1,
                                extras = new
                                {
                                    link = link
                                }
                            },
                            ios = new
                            {
                                alert = message,
                                sound = "default",
                                badge = "+1",
                                extras = new
                                {
                                    link = link
                                }
                            }
                        },
                        //message = new
                        //{
                        //    msg_content = message,
                        //    title = title,
                        //    content_type = "json",
                        //    extras = new { link = link }
                        //}
                        options = new
                        {
                            apns_production = false
                        }
                    }
                    ));
                return string.IsNullOrEmpty(result) == false;
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>("DefaultLogger");
                logger.Error(ex, @"JPushHelper：
title:{0}
message:{1}
link:{2}
", title, message, link);
            }
            return false;
        }
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // trust any certificate!!!
            //  System.Console.WriteLine("Warning, trust any certificate");
            //为了通过证书验证，总是返回true
            return true;
        }

        private static string WebRequest_Response(string appKey, string masterSecret, string strUrl, string paramJson, string input_charset = "utf-8", int timeout = 120000)
        {
            string strResult;
            string appKey_masterSecret = appKey + ":" + masterSecret;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidate);//验证服务器证书回调自动验证  
            //设置HttpWebRequest基本信息
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            myReq.Timeout = timeout;
            myReq.Method = "post";
            myReq.ContentType = "application/json; charset=UTF-8";
            myReq.Headers[HttpRequestHeader.Authorization] = "Basic " + ToBase64(appKey_masterSecret);

            //参数不为空时
            byte[] bytesRequestData = Encoding.GetEncoding(input_charset).GetBytes(paramJson);

            //填充POST数据
            myReq.ContentLength = bytesRequestData.Length;
            Stream requestStream = myReq.GetRequestStream();
            requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
            requestStream.Close();

            //发送POST数据请求服务器
            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
            Stream myStream = HttpWResp.GetResponseStream();

            //从特定的编码中读取数据
            StreamReader sr = new StreamReader(myStream, Encoding.GetEncoding(input_charset));
            StringBuilder strBuilder = new StringBuilder();
            while (sr.Peek() != -1)
            {
                strBuilder.Append(sr.ReadLine());
            }
            strResult = strBuilder.ToString();
            myStream.Close();
            return strResult;
        }

        private static string ToBase64(string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            try
            {
                byte[] bytes = encoding.GetBytes(source);
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
