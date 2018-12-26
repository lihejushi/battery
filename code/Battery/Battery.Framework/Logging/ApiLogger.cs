using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;

namespace Battery.Framework.Logging
{
    public class ApiLogger : XT.MVC.Core.Logging.ILogger
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

        public void InsertLog(XT.MVC.Core.Domain.Logging.LogLevel logLevel, string fullMessage, params object[] args)
        {
            SetApiName(args);
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

        private string GetMessage(string msg, object[] args)
        {
            if (args == null || args.Length <= 1) return msg;
            else
            {
                return string.Format(msg, args.Skip(1).ToArray());
            }
        }

        private void SetApiName(object[] args)
        {
            LogManager.Configuration.Variables["ApiName"] = (args == null || args.Length == 0) ? "Common" : args[0].ToString();
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            SetApiName(args);
            logger.Debug(exception, GetMessage(message, args));
        }

        public void Information(string message)
        {
            SetApiName(new object[] { });
            logger.Info(message);
        }

        public void Warning(string message)
        {
            SetApiName(new object[] { });
            logger.Warn(message);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            SetApiName(args);
            logger.Error(exception, GetMessage(message, args));
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            SetApiName(args);
            logger.Fatal(exception, GetMessage(message, args));
        }

        #endregion
    }
}
