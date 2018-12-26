using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.X
{
    public class ActiveAndPoint
    {
    }

    public class Actives : BasicModel
    {
        public int ID { get; set; }
        public string ActiveName { get; set; }
        public DateTime ContributeStartTime { get; set; }
        public DateTime ContributeEndTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int AddAdminID { get; set; }
        public DateTime AddTime { get; set; }
        public int State { get; set; }
        public int MaxDocument { get; set; }
        public int MaxPic { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string ContributeStartTimeT { get; set; }
        public string ContributeEndTimeT { get; set; }
        public string StartTimeT { get; set; }
        public string EndTimeT { get; set; }
        public string AddTimeT { get; set; }
        public int DocumentTotal { get; set; }//共收到多少剧本
        public int PicTotal { get; set; }//共收到多少照片
        public int UsersTotal { get; set; }//共多少用户参与

    }

    public class ActivesPic : BasicModel
    {
        public int ID { get; set; }
        public int ActiveID { get; set; }
        public int PicID { get; set; }
        public DateTime AddTime { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime? PointTime { get; set; }
    }

    public class ActivesDocument : BasicModel
    {
        public int ID { get; set; }
        public int ActiveID { get; set; }
        public int DocumentID { get; set; }
        public DateTime AddTime { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime? PointTime { get; set; }
    }

    public class AdminActivesPic : BasicModel
    {
        public int ID { get; set; }
        public int ActivesPicID { get; set; }
        public int AdminID { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime PointTime { get; set; }
        public string Evaluate { get; set; }
    }

    public class AdminActivesDocument : BasicModel
    {
        public int ID { get; set; }
        public int ActivesDocumentID { get; set; }
        public int AdminID { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime PointTime { get; set; }
        public string Evaluate { get; set; }
    }

    public class forUserCount
    {
        public int ActiveID { get; set; }
        public int RegID { get; set; }
    }
}
