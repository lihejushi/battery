using Battery.DAL.Log;
using Battery.Model.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
namespace Battery.Framework.Tools
{
    public class SmsHelper
    {
        private static ILogger logger = null;
        static SmsHelper()
        {
            logger = EngineContext.Current.Resolve<ILogger>("DefaultLogger");
        }
        /// <summary>
        /// 云通讯发送短信
        /// </summary>
        /// <param name="log"></param>
        /// <param name="templateId">模板id</param>
        /// <param name="deleteOther">是否删除同类型短信记录</param>
        /// <returns></returns>
        public static int SendVerifyCode(Sms_Log log, string templateId, bool deleteOther = false)
        {
            try
            {
                Sms_Log prevLog = SmsDAL.GetSms(log.MobileNo, log.SmsType);
                if (prevLog != null && prevLog.SendTime.AddSeconds(prevLog.Interval) >= DateTime.Now) return -1;//发送时间太短不能发送
                bool sendResult = true;
                //if (templateId == "211539")//兑换码
                //{
                //    sendResult = SendTemplateSMS(log.MobileNo, templateId, new string[] { log.VCode });
                //}
                //else
                //{
                //    sendResult = SendTemplateSMS(log.MobileNo, templateId, new string[] { log.VCode, "10" });
                //}

                #region 发送短信测试
                //测试环境用这里
                SmsDAL.AddSmsLog(log, deleteOther);
                return 1;
                //生产环境用这里
                //if (sendResult == false) return -2;//短信发送失败
                //else
                //{
                //    SmsDAL.AddSmsLog(log, deleteOther);
                //}
                //return 1;
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex, "短信发送失败 \r\n 错误信息:{0} \r\n 提交数据:{1}", ex.Message, Newtonsoft.Json.JsonConvert.SerializeObject(log));
            }
            return -99;
        }
     
        /// <summary>
        /// 云通讯发送短信
        /// </summary>
        /// <param name="log"></param>
        /// <param name="templateId">模板id</param>
        /// <param name="deleteOther">是否删除同类型短信记录</param>
        /// <returns></returns>
        public static int SendVerifyCode(Sms_Log log, string templateId, string[] param, bool deleteOther = false)
        {
            try
            {
                Sms_Log prevLog = SmsDAL.GetSms(log.MobileNo, log.SmsType);
                if (prevLog != null && prevLog.SendTime.AddSeconds(prevLog.Interval) >= DateTime.Now) return -1;//发送时间太短不能发送
                bool sendResult = SendTemplateSMS(log.MobileNo, templateId, param);;
                if (sendResult == false) return -2;//短信发送失败
                else
                {
                    SmsDAL.AddSmsLog(log, deleteOther);
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "短信发送失败 \r\n 错误信息:{0} \r\n 提交数据:{1}", ex.Message, Newtonsoft.Json.JsonConvert.SerializeObject(log));
            }
            return -99;
        }

        #region 云通讯短信接口

        /// <summary>
        /// 初始化短信发送接口
        /// </summary>
        /// <returns></returns>
        private static CCPRestSDK.CCPRestSDK CCPRestSDKInit(string appId="")
        {
            if (appId == "") appId = ConfigHelper.GetBranch("SmsAppId");
            CCPRestSDK.CCPRestSDK api = new CCPRestSDK.CCPRestSDK();
            bool isInit = api.init(ConfigHelper.GetBranch("SmsAddress"), ConfigHelper.GetBranch("SmsPort"));
            api.setAccount(ConfigHelper.GetBranch("SmsAccountSid"), ConfigHelper.GetBranch("SmsAccountToken"));
            api.setAppId(appId);

            if (isInit)
                return api;
            else
            {
                logger.Error(null, "{0}:{1}", "云通讯接口", "初始化短信发送接口失败");
                return null;
            }
        }

        /// <summary>
        /// 发送模板短信
        /// </summary>
        /// <param name="to">短信接收端手机号码集合，用英文逗号分开，每批发送的手机号数量不得超过100个</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="args">可选字段 内容数据，用于替换模板中{序号}</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static bool SendTemplateSMS(string to, string templateId, string[] args)
        {
            bool sendResult = false;
            CCPRestSDK.CCPRestSDK api = CCPRestSDKInit( ConfigHelper.GetBranch("SmsAppIdYz")); //CCPRestSDKInit(templateId== "210166"? ConfigHelper.GetBranch("SmsAppIdYz") : "");
            if (api != null)
            {
                Dictionary<string, object> reqData = api.SendTemplateSMS(to, templateId, args);
                if (reqData != null && reqData.ContainsKey("statusCode"))
                {
                    sendResult = reqData["statusCode"].Equals("000000");
                    if (sendResult == false)
                    {
                        logger.Error(null, "{0}:{1}", "云通讯接口返回结果", GetDictionaryData(reqData));
                    }
                }
                else
                {
                    logger.Error(null, "{0}:{1}", "云通讯接口返回结果", GetDictionaryData(reqData));
                }
            }

            return sendResult;
        }

        /// <summary>
        /// 发送语音验证码
        /// </summary>
        /// <param name="to">接收号码，被叫为座机时需要添加区号，如：01052823298；被叫为分机时分机号由‘-’隔开，如：01052823298-3627</param>
        /// <param name="verifyCode">验证码内容，为数字和英文字母，不区分大小写，长度4-8位</param>
        /// <param name="displayNum">显示主叫号码，显示权限由服务侧控制。</param>
        /// <param name="playTimes">循环播放次数，1－3次，默认播放3次。</param>
        /// <param name="respUrl">语音验证码状态通知回调地址（必须符合URL规范），云通讯平台将向该Url地址发送呼叫结果通知。</param>
        /// <param name="lang">语言类型。取值en（英文）、zh（中文），默认值zh。</param>
        /// <returns></returns>
        public static bool VoiceVerify(string to, string verifyCode, string displayNum = "", string playTimes = "3", string respUrl = "", string lang = "")
        {
            bool sendResult = false;
            CCPRestSDK.CCPRestSDK api = CCPRestSDKInit();

            if (api != null)
            {
                Dictionary<string, object> reqData = api.VoiceVerify(to, verifyCode, displayNum, playTimes, respUrl, lang);
                if (reqData != null && reqData.ContainsKey("statusCode"))
                {
                    sendResult = reqData["statusCode"].Equals("000000");
                    if (sendResult == false)
                    {
                        logger.Error(null, "{0}:{1}", "云通讯接口返回结果", GetDictionaryData(reqData));
                    }
                }
                else
                {
                    logger.Error(null, "{0}:{1}", "云通讯接口返回结果", GetDictionaryData(reqData));
                }
            }

            return sendResult;
        }

        /// <summary>
        /// 简单组装返回的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetDictionaryData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += GetDictionaryData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret;
        }

        #endregion 
    }
}
