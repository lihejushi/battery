using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuoKe.Services.Models
{
    public class SynchronizationsProductInfo
    {
        public string DbConnectionStr { get; set; }
        public string Exec { get; set; }//执行周期
        public int Interval { get; set; }//发送时间的间隔,单位：分钟
        public int TotelCount { get; set; }//总共发送几次
    }

    public class DealWithRequestFile
    {
        public string DbConnectionStr { get; set; }
        public string Exec { get; set; }//执行周期
        public string CountToFilePath { get; set; }//读取文件路径
        public string DesKey { get; set; }//KEY
        public string DesIV { get; set; }//KEY
    }


}
