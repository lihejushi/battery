using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;

namespace XT.MVC.Core.Logging
{
    public class DefaultLogger : ILogger
    {
        #region ILogger 成员
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        public bool IsEnabled(XT.MVC.Core.Domain.Logging.LogLevel level)
        {
            switch (level)
            {
                case XT.MVC.Core.Domain.Logging.LogLevel.Debug:
                    return logger.IsDebugEnabled;
                case XT.MVC.Core.Domain.Logging.LogLevel.Error:
                    return logger.IsErrorEnabled;
                case XT.MVC.Core.Domain.Logging.LogLevel.Fatal:
                    return logger.IsFatalEnabled;
                case XT.MVC.Core.Domain.Logging.LogLevel.Information:
                    return logger.IsInfoEnabled;
                case XT.MVC.Core.Domain.Logging.LogLevel.Warning:
                    return logger.IsWarnEnabled;
                default:
                    return false;
            }
        }

        public void InsertLog(XT.MVC.Core.Domain.Logging.LogLevel logLevel, string fullMessage , params object[] args)
        {
            switch (logLevel)
            {
                case XT.MVC.Core.Domain.Logging.LogLevel.Debug:
                    Debug(null, fullMessage, args);
                    break;
                case XT.MVC.Core.Domain.Logging.LogLevel.Error:
                    Error(null, fullMessage, args);
                    break;
                case XT.MVC.Core.Domain.Logging.LogLevel.Fatal:
                    Fatal(null, fullMessage, args);
                    break;
                case XT.MVC.Core.Domain.Logging.LogLevel.Information:
                    Information(fullMessage);
                    break;
                case XT.MVC.Core.Domain.Logging.LogLevel.Warning:
                    Warning(fullMessage);
                    break;
                default:
                    break;
            }
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            logger.Debug(exception, message, args);
        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            logger.Error(exception, message, args);
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            logger.Fatal(exception, message, args);
        }

        #endregion
    }
}
