using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace XT.MVC.Framework
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Widget(this HtmlHelper helper, string widgetZone)
        {
            return helper.Action("WidgetsByZone", "Widget", new { widgetZone = widgetZone });
        }
        public static string Truncat(this HtmlHelper helper, string input, int length)
        {
            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + "...";
            }
        }
    }
}
