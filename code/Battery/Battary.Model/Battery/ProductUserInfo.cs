using System;
using System.Collections.Generic;


namespace Battery.Model.Battery
{
    public class ProductUserInfo:BasicModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Num { get; set; }
        public int ProductInstallID { get; set; }
        public string UserName { get; set; }
        public string UserTel { get; set; }
        public int UserSex { get; set; }
        public string WorkUnit { get; set; }
        public int? Age { get; set; }
        public string Career { get; set; }
        public string Education { get; set; }
        public string UseBike { get; set; }
        public string KonwStyle { get; set; }
        public string UserWeChat { get; set; }
        public string UserQQ { get; set; }
        public string UserEMail { get; set; }
    }
}
