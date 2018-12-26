using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support.Service.Models
{
    public class ApiTicketResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }

        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string ticket { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime expires_time { get; set; }
    }
}
