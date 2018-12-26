using System;
using System.Collections.Generic;
using XT.MVC.Core;
using XT.MVC.Core.Domain.Logging;

namespace XT.MVC.Core.Logging
{
    public partial interface ILogger
    {
        bool IsEnabled(LogLevel level);

        void InsertLog(LogLevel logLevel, string fullMessage, params object[] args);

        void Debug(Exception exception, string message, params object[] args);
        void Information(string message);
        void Warning(string message);
        void Error(Exception exception, string message, params object[] args);
        void Fatal(Exception exception, string message, params object[] args);
    }
}
