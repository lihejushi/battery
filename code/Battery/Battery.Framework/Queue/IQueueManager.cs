using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuoKe.Framework.Queue
{
    public interface IQueueManager
    {
        bool SendMessage(string topic, string message, int code = 1, string tag = null);
    }
}
