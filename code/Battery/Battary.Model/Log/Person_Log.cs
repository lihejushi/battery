using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Log
{
    public enum ActionType
    {
        浏览 = 0,
        新增 = 1,
        编辑 = 2,
        删除 = 3,
        变更状态 = 4,
        审核 = 5,
        安装产品=6
    }

    [Serializable]
    public class Person_Log : BasicModel
    {
        public int LogId { get; set; }
        public int PersonId { get; set; }
        public string Url { get; set; }
        public ActionType ActionType { get; set; }
        public string Memo { get; set; }
        public DateTime LogTime { get; set; }
        public string LogIP { get; set; }
    }
}
