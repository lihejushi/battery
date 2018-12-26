using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace XT.MVC.Core.Web
{
    public interface ICookieManager
    {
        void SetCookie(string name, string value, string domain, string path, DateTime? expires);
        void SetCookie(string name, string value);
        void SetCookie(string name, string value, DateTime expires);
        void SetCookie(string name, string value, string domain);
        void SetCookie(string name, string value, string domain, DateTime expires);

        void ClearCookie(string name);
        string GetCookieValue(string name);
    }
}
