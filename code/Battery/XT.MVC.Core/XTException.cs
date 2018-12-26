using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using XT.MVC.Core.Logging;

namespace XT.MVC.Core
{
    [Serializable]
    public class XTException : ApplicationException
    {
        //private static ILogger logger = LogManager.GetCurrentClassLogger();
        public XTException() { }
        public XTException(string message) : base(message) 
        {
            //logger.Error(message);
        }

        public XTException(string message, Exception inner) : base(message, inner) {
            //logger.Error(message, inner);
        }

        protected XTException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
