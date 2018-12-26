using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Web
{
    public interface ISessionManager
    {
        T GetSession<T>(string name);
        object GetSession(string name);
        void SetSession(string name, object obj);
        void AddSession(string name, object obj);
        void DelSession(string name);
    }
}
