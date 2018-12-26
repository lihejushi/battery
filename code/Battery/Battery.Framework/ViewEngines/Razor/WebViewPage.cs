using Battery.Framework.AccessType;
using Battery.Framework.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;

namespace Battery.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IWorkContext _workContext;
        private IAppContext _appContext;
        private IAccessTypeHelper _accessType;

        public override void InitHelpers()
        {
            base.InitHelpers();
            _workContext = EngineContext.Current.Resolve<IWorkContext>();
            _appContext = EngineContext.Current.Resolve<IAppContext>();
            _accessType = EngineContext.Current.Resolve<IAccessTypeHelper>();
        }

        public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        {
            Action<TextWriter> action = delegate(TextWriter tw)
            {
                var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
                var tagBuilder = new TagBuilder("div");
                tagBuilder.MergeAttributes(htmlAttributes);

                var section = RenderSection(name, false);
                if (section != null)
                {
                    tw.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                    section.WriteTo(tw);
                    tw.Write(tagBuilder.ToString(TagRenderMode.EndTag));
                }
            };
            return new HelperResult(action);
        }

        public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            return IsSectionDefined(sectionName) ? RenderSection(sectionName) : defaultContent(new object());
        }

        //public override string Layout
        //{
        //    get
        //    {
        //        var layout = base.Layout;

        //        //if (!string.IsNullOrEmpty(layout))
        //        //{
        //        //    var filename = System.IO.Path.GetFileNameWithoutExtension(layout);
        //        //    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

        //        //    if (viewResult.View != null && viewResult.View is RazorView)
        //        //    {
        //        //        layout = (viewResult.View as RazorView).ViewPath;
        //        //    }
        //        //}

        //        return layout;
        //    }
        //    set
        //    {
        //        base.Layout = value;
        //    }
        //}

        public IWorkContext WorkContext
        {
            get { return _workContext; }
        }
        public IAppContext AppContext
        {
            get { return _appContext; }
        }
        public IAccessTypeHelper AccessType
        {
            get
            {
                return _accessType;
            }
        }
        public string FileDomain
        {
            get
            {
                return CDNConfig.FileDomain;
            }
        }
        public string BaseDomain
        {
            get
            {
                return CDNConfig.BaseDomain;
            }
        }
        public string ShopDomain
        {
            get
            {
                return XT.MVC.Core.Common.ConfigHelper.GetBranch("ShopDomain");
            }
        }
        public string MainDomain
        {
            get
            {
                return XT.MVC.Core.Common.ConfigHelper.GetBranch("AppDomain");
            }
        }

        public App CurrentApp { get { return (App)_appContext.CurrentApp; } }
        public User CurrentUser { get { return (User)_workContext.CurrentUser; } }
        public bool IsLogin { get { return _workContext.IsLogin; } }
        /// <summary>
        /// 是否处于测试
        /// </summary>
        public bool IsTest
        {
            get
            {
                return string.IsNullOrEmpty(ConfigHelper.GetBranch("Test.Enabled")) == false && ConfigHelper.GetBranch("Test.Enabled") == "1";
            }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
