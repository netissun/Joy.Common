using Joy.Common.Configuration.Element;
using Joy.Common.Object;
using Joy.Common.QuartzJob.Attachment;
using Quartz;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Joy.Common.QuartzJob.Configuration
{
    public class CfgManager
    {
        private static IJobDetail AddParameters(IJobDetail job, string parameters)
        {
            if (parameters != null && !parameters.Equals(string.Empty))
            {
                string[] ps = parameters.Split(';');
                foreach (string p in ps)
                {
                    string[] nv = p.Split(':');
                    if (nv.Length == 2) job.JobDataMap.Add(nv[0], nv[1]);
                }
            }
            return job;
        }

        private static IJobDetail AddAttachments(IJobContainer con, IJobDetail job, string atts)
        {
            if (atts != null && !atts.Equals(string.Empty))
            {
                string[] att = atts.Split(';');
                foreach (string at in att)
                {
                    string[] atm = at.Split(':');
                    if (atm.Length != 2) continue;
                    if (!con.Attachments.ContainsKey(atm[1])) continue;
                    job.JobDataMap.Add(atm[0], (con.Attachments[atm[1]] as IAttachmentFactory).GetAttachment());
                }
            }
            return job;
        }

        private static IJobDetail CreateJob(IJobContainer con, NamedComponentElement jobElement)
        {
            string cronString = (jobElement.ComponentProperties["trigger.cron"] != null) ? jobElement.ComponentProperties["trigger.cron"].Value : "* 0/1 * * * ?";
            IJobDetail jd = JobBuilder.Create(Type.GetType(jobElement.Type)).WithIdentity(jobElement.Name).Build();
            if (jobElement.ComponentProperties["job.parameters"] != null) AddParameters(jd, jobElement.ComponentProperties["job.parameters"].Value);
            if (jobElement.ComponentProperties["job.attachments"] != null) AddAttachments(con, jd, jobElement.ComponentProperties["job.attachments"].Value);
            return jd;
        }

        public static IJobContainer GetJobContainer(IJobContainerConfig cfg)
        {
            IJobContainer container = ObjectHelper.CreateObject<IJobContainer>(cfg.Type, cfg.ComponentProperties);
            NameValueCollection props = new NameValueCollection();
            foreach (NameValueConfigurationElement nv in cfg.QuartzProperties) props.Add(nv.Name, nv.Value);
            container.SetQuartzProperties(props);
            foreach (NamedComponentElement att in cfg.Attachments)
            {
                IAttachmentFactory acf = ObjectHelper.CreateObject<IAttachmentFactory>(att.Type, att.ComponentProperties);
                container.Attachments.Add(att.Name, acf);
            }
            foreach (NamedComponentElement nc in cfg.Jobs)
            {
                string cronString = (nc.ComponentProperties["trigger.cron"] != null) ? nc.ComponentProperties["trigger.cron"].Value : "* 0/1 * * * ?";
                IJobDetail jd = CreateJob(container, nc);
                container.AddJob(jd, cronString);
            }
            return container;
        }

        public static IJobContainer GetJobContainer(string section)
        {
            JobContainerSection sect = ConfigurationManager.GetSection(((section == null) || (section.Equals(string.Empty))) ? "jobContainer" : section) as JobContainerSection;
            return GetJobContainer(sect);
        }

        public static IJobContainer GetJobContainer()
        {
            return GetJobContainer("jobContainer");
        }

    }
}
