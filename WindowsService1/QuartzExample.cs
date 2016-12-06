using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace WindowsService1
{
    class QuartzExample
    {
        private IScheduler sched = null;

        public void Start()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            sched = schedFact.GetScheduler();
            sched.Start();

            //{
            //    IJobDetail job = JobBuilder.Create<Noop_IJob>()
            //        .WithIdentity("myJob1", "group1")
            //        .Build();

            //    // Trigger the job to run now, and then every 40 seconds
            //    ITrigger trigger = TriggerBuilder.Create()
            //        .WithIdentity("myTrigger1", "group1")
            //        .StartNow()
            //        .WithSimpleSchedule(x => x
            //            .WithIntervalInSeconds(10)
            //            .RepeatForever())
            //        .Build();

            //    sched.ScheduleJob(job, trigger);
            //}
            
            {
                IJobDetail job = JobBuilder.Create<WebRequest>()
                    .WithIdentity("myJob2", "group1")
                    .Build();

                // Trigger the job to run now, and then every 40 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger2", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                sched.ScheduleJob(job, trigger);
            }


            //{
            //    IJobDetail job = JobBuilder.Create<Noop_BaseJob>()
            //        .WithIdentity("myJob3", "group1")
            //        .Build();

            //    // Trigger the job to run now, and then every 40 seconds
            //    ITrigger trigger = TriggerBuilder.Create()
            //        .WithIdentity("myTrigger3", "group1")
            //        .StartNow()
            //        .WithSimpleSchedule(x => x
            //            .WithIntervalInSeconds(10)
            //            .RepeatForever())
            //        .Build();

            //    sched.ScheduleJob(job, trigger);
            //}
            
        }

        public void Stop()
        {
            sched.Shutdown();
        }

        /*
         * We suggest a base class like the example below, but this is an example without it.
         * This would just specify the reporting transaction name as "Quartz"
         */
        class Noop_IJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                Debug.WriteLine("Just sleeping for a couple seconds");
                System.Threading.Thread.Sleep(2000);
                Debug.WriteLine("Sleep over");
            }
        }


        /*
         * Making a base class for the job to easily wrap all of them with the profile tracer logic.
         */ 

        class BaseJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                //Could name it by the inheriting job class name. This is definitely preferred if you have lots of different uses of the same job
                var tracer = StackifyLib.ProfileTracer.CreateAsOperation("Quartz " + this.GetType().Name);

                //could name it by the job name in quartz. This is fine if the job only exists once.
                //for use cases where you run the same job lots of times for different clients, for example, probably makes more sense to use the job class type so they all report the same.
                //var tracer = StackifyLib.ProfileTracer.CreateAsOperation("Quartz " + context.JobDetail.Name);

                tracer.Exec(() => ExecuteJob(context));
            }

            protected virtual void ExecuteJob(IJobExecutionContext context)
            {
                throw new NotImplementedException("Must implement in the inheriting class");
            }
        }

        class Noop_BaseJob : BaseJob
        {
            protected override void ExecuteJob(IJobExecutionContext context)
            {
                Debug.WriteLine("Just sleeping for a couple seconds");
                System.Threading.Thread.Sleep(2000);
                Debug.WriteLine("Sleep over");
            }
        }

        class WebRequest : BaseJob
        {
            protected override void ExecuteJob(IJobExecutionContext context)
            {
                Debug.WriteLine("This example is a quartz job running every once in a while");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString("https://www.microsoft.com/en-us/");
                }
            }
        }
    }
}
