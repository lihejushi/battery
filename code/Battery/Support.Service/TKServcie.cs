using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Support.Service.Jobs;
using Support.Service.Models;
using Support.Service.Tools;

namespace Support.Service
{
    public partial class TKServcie : ServiceBase
    {
        ISchedulerFactory sf = null;
        IScheduler sched = null;
        Logger logger = null;
        public TKServcie()
        {
            InitializeComponent();

            sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();
            sched.ResumeAll();
            logger = LogManager.GetLogger("Service");
        }

        protected override void OnStart(string[] args)
        {
            sched.Start(); 
            //用户Token
            if (ConfigHelper.GetTokenConfigs() != null)
            {
                logger.Info("Token服务启动");
                var models = ConfigHelper.GetTokenConfigs();
                foreach (var model in models)
                {
                    if (string.IsNullOrEmpty(model.Exec) == false)
                    {
                        IJobDetail job = JobBuilder.Create<TokenJob>()
                            .WithIdentity(model.GetHashCode().ToString(), "TokenConfig")
                            .UsingJobData(new JobDataMap(new Dictionary<string, TokenConfig>() { { "Model", model } }))
                            .Build();
                        var trigger = (ICronTrigger)TriggerBuilder.Create()
                            .WithIdentity(model.GetHashCode().ToString(), "TokenConfig")
                            .WithCronSchedule(model.Exec)
                            //.WithCronSchedule(model.Exec, m => m.WithMisfireHandlingInstructionFireAndProceed()).StartAt(new DateTimeOffset(DateTime.Now.AddHours(-24)))
                            //.WithCronSchedule(model.Exec, m => m.WithMisfireHandlingInstructionIgnoreMisfires()).StartAt(new DateTimeOffset(DateTime.Now.AddHours(-24)))
                            .Build();
                        sched.ScheduleJob(job, trigger);
                    }
                }
            } 
        }

        protected override void OnStop()
        {
            if (sched != null)
                sched.Shutdown(true);
        }
    }
}
