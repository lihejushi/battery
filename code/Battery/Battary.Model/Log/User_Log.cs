using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Log
{
    public enum UserActionType
    {
        注册 = 0,
        登录 = 1,
        上传文档 = 2,
        上传照片 = 3,
        修改密码 = 4,
        找回密码 = 5,
        完善信息 = 6,
        删除文档 = 7

    }

    [Serializable]
    public class User_Log : BasicModel
    {
        public int ID { get; set; }
        public int RegID { get; set; }
        public string Url { get; set; }
        public UserActionType OperatCode { get; set; }
        public string OPerate { get; set; }
        public DateTime OperateTime { get; set; }
        public string OperateIP { get; set; }
        public int OperateResult { get; set; }
    }
}
