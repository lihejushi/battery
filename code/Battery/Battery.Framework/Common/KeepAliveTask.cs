using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using XT.MVC.Core;
using XT.MVC.Core.Services.Tasks;
using XT.MVC.Core.Web;

namespace Battery.Framework.Common
{
    public partial class KeepAliveTask : ITask
    {
        private readonly IAppContext _appContext;
        private readonly IWebHelper _webHelper;

        public KeepAliveTask(IAppContext appContext,IWebHelper webHelper)
        {
            this._appContext = appContext;
            this._webHelper = webHelper;
        }

        public void Execute()
        {
            string url = _webHelper.GetAppHost(false) + "keepalive/index";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
