using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Support.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new WXServcie() 
            };
            ServiceBase.Run(ServicesToRun);
            //Support.Service.Tools.ConfigHelper.WXServiceConfig.GetConfig();
            //Support.Service.Tools.ConfigHelper.WXServiceConfig.SaveConfig(new Tools.ConfigHelper.WXServiceConfig()
            //{
            //    CheckConfig = new Models.Checking()
            //    {
            //        AreaCode = "371",
            //        BankCode = "14",
            //        BankShort = "WX",
            //        CheckExec = "0 10 1 * * ?",//每天1点10分执行
            //        DbConnectionStr = "server=.;uid=mob51;pwd=svn.mob51;database=CTMP_Log;",
            //        FtpIp = "",
            //        FtpPort = "",
            //        FtpUserName = "",
            //        FtpPassword = "",
            //        FtpLocalPath = "",
            //        FtpRemotePath = "",
            //        ServerPort = "10034",
            //        ServerIp = "137.32.44.25"
            //    },
            //    DbBackupConfigs = new List<Models.DbBackup>()
            //    {
            //        new Models.DbBackup(){
            //            ConnectionStr = "server=.;uid=mob51;pwd=svn.mob51;database=CTMP;",
            //            DbName = "CTMP",
            //            DiffBackupExec = "0 10 1,12,20 * * ?",//每天1、12、20点10分执行差异备份
            //            FullBackupExec = "0 10 1 * * 7"//周日1点10分执行完整备份
            //        },
            //        new Models.DbBackup(){
            //            ConnectionStr = "server=.;uid=mob51;pwd=svn.mob51;database=CTMP_Log;",
            //            DbName = "CTMP_Log",
            //            DiffBackupExec = "0 10 1,12,20 * * ?",//每天1、12、20点10分执行差异备份
            //            FullBackupExec = "0 10 2 * * 7"//周日2点10分执行完整备份
            //        },
            //        new Models.DbBackup(){
            //            ConnectionStr = "server=.;uid=mob51;pwd=svn.mob51;database=CTMP_Activity;",
            //            DbName = "CTMP_Activity",
            //            DiffBackupExec = "0 10 1,12,20 * * ?",//每天1、12、20点10分执行差异备份
            //            FullBackupExec = "0 10 3 * * 7"//周日3点10分执行完整备份
            //        }
            //    }
            //});
        }
    }
}
