using System.Collections.Generic;
using System.Web.Mvc;

namespace XT.MVC.Framework.Themes
{
    public class ThemeableRazorViewEngine : ThemeableBuildManagerViewEngine
    {
        public ThemeableRazorViewEngine()
        {
            #region AreaView

            AreaViewLocationFormats = new[]
                                          {
                                            //themes
                                            "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                            "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",
                                              
                                            //default
                                            "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Areas/{2}/Views/Shared/{0}.cshtml",

                                              //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"
                                          };

            AreaMasterLocationFormats = new[]
                                            {
                                                //themes
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",

                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml",

                                                //themes
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                //default
                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/Shared/{0}.cshtml"
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {
                                                     //themes
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",
                                                    
                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml",

                                                    //themes
                                                    "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                    //default
                                                    "~/Views/{1}/{0}.cshtml", 
                                                    "~/Views/Shared/{0}.cshtml"
                                                 };
            #endregion

            #region View

            ViewLocationFormats = new[]
                                      {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"
                                      };

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"

                                        };

            PartialViewLocationFormats = new[]
                                             {
                                                 //themes
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                //default
                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/Shared/{0}.cshtml"
                                             };
            #endregion

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
