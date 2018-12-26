using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Domain.Apps;

namespace Battery.Framework.Domain
{
    public enum PlatformType
    {
        ChinaTelcom,
        Shop,//App
        Company
    }

    public class App : IApp
    {
        #region IApp 成员

        private int _AppId = 0;
        /// <summary>
        /// 应用id, app默认为0，商家默认为商家id
        /// </summary>
        public int AppId
        {
            get
            {
                return _AppId;
            }
            set
            {
                _AppId = value;
            }
        }

        #endregion

        /// <summary>
        /// 平台类型，app/微官网/店铺
        /// </summary>
        public PlatformType Platform { get; set; }

        public string AppName { get; set; }

        /// <summary>
        /// 是否是后台管理
        /// </summary>
        public bool IsAdmin { get; set; }

        public int MainAppId { get { return 0; } }
    }
}
