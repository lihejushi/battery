using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Configuration;
namespace XT.MVC.Core.Domain.Widget
{
    public class WidgetSettings : ISettings
    {
        public WidgetSettings()
        {
            ActiveWidgetSystemNames = new List<string>();
        }

        /// <summary>
        /// 激活部件的系统名称
        /// </summary>
        public List<string> ActiveWidgetSystemNames { get; set; }
    }
}
