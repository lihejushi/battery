using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : Attribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Rank { get; set; }

        public PermissionAttribute(string title, string memo)
        {
            Title = title;
            Memo = memo;
        }

        public PermissionAttribute(string title)
        {
            Title = title;
            Memo = string.Empty;
        }

    }
    [Serializable]
    public class PermissionModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Rank { get; set; }
    }
}
