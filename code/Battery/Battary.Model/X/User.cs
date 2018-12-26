using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.X
{
    public class User
    {
    }

    public class Reg: BasicModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime RegTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int LoginCount { get; set; }
        public int State { get; set; }
        public string LockReson { get; set; }
        public string Avator { get; set; }
    }

    public class UserInfo : Reg
    {
        public int UserID { get; set; }
        public string UserRealName { get; set; }
        public int RegID { get; set; }
        public int? Age { get; set; }
        public int? Sex { get; set; }
        public string Career { get; set; }
        public string Identity { get; set; }
        public string WorkUnit { get; set; }
        public string Education { get; set; }
        public string Graduation { get; set; }
        public string Major { get; set; }
        public string Specialties { get; set; }
        public string Occupation { get; set; }
        public DateTime? RegTimeT { get; set; }
        public DateTime? LastLoginTimeT { get; set; }
        public string Avator { get; set; }
        public string BigPic { get; set; }
        public string Achievement { get; set; }
        public string Birthday { get; set; }
        public string Native { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string QQ { get; set; }
        public string EMail { get; set; }
        public string Wechat { get; set; }
        public string CareerT { get; set; }
    }

    public class UserLogs : BasicModel
    {
        public int ID { get; set; }
        public int RegID { get; set; }
        public int OperatCode { get; set; }
        public string OPerate { get; set; }
        public DateTime OperateTime { get; set; }
        public int OperateResult { get; set; }
    }
}
