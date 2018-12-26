using System;

namespace XT.MVC.Core.Domain.Logging
{
    public partial class Log : BaseEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public int LogLevelId { get; set; }
        /// <summary>
        /// 日志描述
        /// </summary>
        public string ShortMessage { get; set; }
        /// <summary>
        /// 日志完整内容
        /// </summary>
        public string FullMessage { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 记录页面
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// 来源地址
        /// </summary>
        public string ReferrerUrl { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        public LogLevel LogLevel
        {
            get
            {
                return (LogLevel)this.LogLevelId;
            }
            set
            {
                this.LogLevelId = (int)value;
            }
        }
    }
}
