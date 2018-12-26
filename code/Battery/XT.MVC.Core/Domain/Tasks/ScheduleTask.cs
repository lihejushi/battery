using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Domain.Tasks
{
    public class ScheduleTask : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 间隔周期
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 出现异常时是否停止
        /// </summary>
        public bool StopOnError { get; set; }
        /// <summary>
        /// 最后开始时间
        /// </summary>
        public DateTime? LastStartUtc { get; set; }
        /// <summary>
        /// 最后结束时间
        /// </summary>
        public DateTime? LastEndUtc { get; set; }
        /// <summary>
        /// 最后成功时间
        /// </summary>
        public DateTime? LastSuccessUtc { get; set; }
    }
}
