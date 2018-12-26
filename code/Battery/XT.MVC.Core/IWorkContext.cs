using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Domain.Users;

namespace XT.MVC.Core
{
    public interface IWorkContext
    {
        bool IsLogin { get; }
        IUser CurrentUser { get; }

        void Refresh(IUser user);
    }
}
