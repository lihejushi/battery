using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Log
{
    [Serializable]
    public class Sms_Log : BasicModel
    {
        public int Id { get; set; }
        public string MobileNo { get; set; }
        public string SendContent { get; set; }
        public DateTime LogTime { get; set; }
        public DateTime SendTime { get; set; }
        public int State { get; set; }
        public int Interval { get; set; }
        /// <summary>
        /// 短信类型
        /// 1 注册
        /// 2 找回密码
        /// 3 审核通过通知
        /// 4 领取兑换码
        /// 5 兑换资源
        /// 6 发送兑换码
        /// 7 审核不通过通知
        /// </summary>
        public int SmsType { get; set; }
        public string VCode { get; set; }
    }
}
