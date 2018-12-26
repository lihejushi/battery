using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.X
{
    public class UserUpload
    {
    }

    public class UserPics : BasicModel
    {
        public int ID { get; set; }
        public int RegID { get; set; }
        public int PicType { get; set; }
        public DateTime UpdateTime { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }

    public class UserDocument : BasicModel
    {
        public int ID { get; set; }
        public int RegID { get; set; }
        public string DocumentTitle { get; set; }
        public int DocumentType { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Summary { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Keys { get; set; }
        public DateTime UpdateTimeT { get; set; }
        public string UserName { get; set; }
        public int IsDelete { get; set; }
        public string DelReson { get; set; }

    }

    public class ShowUserListPicByActiveID: UserPics
    {
        public int PicID { get; set; }
        public DateTime AddTime { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime? PointTime { get; set; }
    }

    public class ShowUserListDocByActiveID : UserDocument
    {
        public int DocumentID { get; set; }
        public DateTime AddTime { get; set; }
        public int Point { get; set; }
        public int State { get; set; }
        public DateTime? PointTime { get; set; }
    }
}
