using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;
using Quartz.Impl;
using TuoKe.Services.Tools;
using TuoKe.Services.Jobs;
using TuoKe.Services.Models;

namespace TuoKe.Services
{
    partial class TuoKeService : ServiceBase
    {
        ISchedulerFactory sf = null;
        IScheduler sched = null;
        Logger logger = null;
        public TuoKeService()
        {
            InitializeComponent();
            sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();
            sched.ResumeAll();
            logger = LogManager.GetLogger("Service");
        }

        protected override void OnStart(string[] args)
        {
            Logger logger = LogManager.GetLogger("Service");
            // TODO: 在此处添加代码以启动服务。
            //logger.Info("start");
            try
            {
                sched.Start();

                if (ConfigHelper.GetSynchronizationsProductInfoConfig() != null)
                {
                    var model = ConfigHelper.GetSynchronizationsProductInfoConfig();
                    if (string.IsNullOrEmpty(model.Exec) == false)
                    {
                        IJobDetail job = JobBuilder.Create<SynchronizationsProductInfoJob>()
                            .WithIdentity(model.GetHashCode().ToString(), "SynchronizationsProductInfo")
                            .UsingJobData(new JobDataMap(new Dictionary<string, SynchronizationsProductInfo>() { { "Model", model } }))
                            .Build();

                        ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity(model.GetHashCode().ToString(), "SynchronizationsProductInfo")
                            .WithCronSchedule(model.Exec)
                            .Build();
                        sched.ScheduleJob(job, trigger);
                    }
                }

                if (ConfigHelper.GetDealWithRequestFileConfig() != null)
                { 
                    var model = ConfigHelper.GetDealWithRequestFileConfig();
                    if (string.IsNullOrEmpty(model.Exec) == false)
                    {
                        IJobDetail job = JobBuilder.Create<DealWithRequestFileJob>()
                            .WithIdentity(model.GetHashCode().ToString(), "DealWithRequestFile")
                            .UsingJobData(new JobDataMap(new Dictionary<string, DealWithRequestFile>() { { "Model", model } }))
                            .Build();

                        ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity(model.GetHashCode().ToString(), "DealWithRequestFile")
                            .WithCronSchedule(model.Exec)
                            .Build();
                        sched.ScheduleJob(job, trigger);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Info("异常：" + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            if (sched != null)
                sched.Shutdown(true);
        }
    }
}
