using System;
using System.Linq;
using XT.MVC.Core;

namespace XT.MVC.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IAppContext _appContext;
        private readonly IWorkContext _workContext;
        private readonly IThemeProvider _themeProvider;

        public ThemeContext(IWorkContext workContext,
            IAppContext appContext,
            IThemeProvider themeProvider)
        {
            this._workContext = workContext;
            this._appContext = appContext;
            this._themeProvider = themeProvider;
        }

        #region IThemeContext 成员

        public string WorkingTheme
        {
            //暂不开启主题
            get { return "Default"; }
        }

        #endregion
    }
}
