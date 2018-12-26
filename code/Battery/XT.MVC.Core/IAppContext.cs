using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Domain.Apps;

namespace XT.MVC.Core
{
    public interface IAppContext
    {
        IApp CurrentApp { get; }

        void Refresh(IApp app);
    }
}
