using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joy.Common.QuartzJob.Impl
{
    public class JobContainer : IJobContainer
    {
        private IDictionary<string, object> attachments = null;
        private TriggerBuilder triggerBuilder = null;
        private IDictionary<IJobDetail, ITrigger> jobs = null;
        private NameValueCollection quartzProps = null;
        private IScheduler scheduler = null;
        private bool isRunning = false;
        private ISchedulerFactory factory = null;

        public JobContainer()
        {
            triggerBuilder = TriggerBuilder.Create();
            attachments = new Dictionary<string, object>();
            jobs = new Dictionary<IJobDetail, ITrigger>();
        }

        public void SetQuartzProperties(NameValueCollection props)
        {
            this.quartzProps = props;
        }

        public void AddJob(IJobDetail jobDetail, string cronString)
        {
            ITrigger tg = triggerBuilder.WithCronSchedule(cronString).WithIdentity(Guid.NewGuid().ToString()).Build();
            jobs.Add(jobDetail, tg);
        }

        public void Init()
        {
            factory = new StdSchedulerFactory(this.quartzProps);
            scheduler = factory.GetScheduler();
            foreach (IJobDetail jd in jobs.Keys) scheduler.ScheduleJob(jd, jobs[jd]);
        }

        public void Start()
        {
            if (!isRunning)
            {
                scheduler.Start();
                isRunning = true;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                scheduler.Shutdown();
                isRunning = false;
                scheduler = null;
                factory = null;
            }
        }

        public void SetProperty(string name, string value)
        {

        }

        public IDictionary<string, object> Attachments
        {
            get
            {
                return this.attachments;
            }
            set
            {
                this.attachments = value;
            }
        }
    }
}
