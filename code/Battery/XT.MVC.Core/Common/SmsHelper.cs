using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace XT.MVC.Core.Helper
{
    public class SmsHelper
    {
        /// <summary>
        /// 初始化短信发送接口
        /// </summary>
        /// <returns></returns>
        private static CCPRestSDK.CCPRestSDK Init()
        {
            CCPRestSDK.CCPRestSDK api = new CCPRestSDK.CCPRestSDK();
            bool isInit = api.init(ConfigHelper.GetBranch("SmsAddress"), ConfigHelper.GetBranch("SmsPort"));
            api.setAccount(ConfigHelper.GetBranch("SmsAccountSid"), ConfigHelper.GetBranch("SmsAccountToken"));
            api.setAppId(ConfigHelper.GetBranch("SmsAppId"));

            if (isInit)
                return api;
            else
            {
                WriteLog("初始化短信发送接口失败");
                return null;
            }
        }

        /// <summary>
        /// 发送模板短信
        /// </summary>
        /// <param name="to">短信接收端手机号码集合，用英文逗号分开，每批发送的手机号数量不得超过100个</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="data">可选字段 内容数据，用于替换模板中{序号}</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static bool SendTemplateSMS(string to, string templateId, string[] data)
        {
            bool sendResult = false;
            CCPRestSDK.CCPRestSDK api = Init();

            if (api != null)
            {
                Dictionary<string, object> reqData = api.SendTemplateSMS(to, templateId, data);
                if (reqData != null && reqData.ContainsKey("statusCode"))
                {
                    sendResult = reqData["statusCode"].Equals("000000");
                    if (sendResult == false)
                    {
                        WriteLog(GetDictionaryData(reqData));
                    }
                }
                else
                    WriteLog(GetDictionaryData(reqData));
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
            CCPRestSDK.CCPRestSDK api = Init();

            if (api != null)
            {
                Dictionary<string, object> reqData = api.VoiceVerify(to, verifyCode, displayNum, playTimes, respUrl, lang);
                if (reqData != null && reqData.ContainsKey("statusCode"))
                {
                    sendResult = reqData["statusCode"].Equals("000000");
                    if (sendResult == false)
                    {
                        WriteLog(GetDictionaryData(reqData));
                    }
                }
                else
                    WriteLog(GetDictionaryData(reqData));
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

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        private static void WriteLog(string log)
        {
            string strFilePath = GetLogPath();
            System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Append);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
            sw.WriteLine(DateTime.Now.ToString() + "\t" + log);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取日志路径
        /// </summary>
        /// <returns>日志路径</returns>
        private static string GetLogPath()
        {
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            return System.IO.Path.GetDirectoryName(dllpath) + "\\smsLog.txt";
        }
    }
}
