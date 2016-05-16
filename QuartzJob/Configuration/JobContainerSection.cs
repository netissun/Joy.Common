using Joy.Common.Configuration;
using Joy.Common.Configuration.Element;
using System.Configuration;

namespace Joy.Common.QuartzJob.Configuration
{
    public class JobContainerSection : ComponentSection, IJobContainerConfig
    {

        [ConfigurationProperty("quartz", IsRequired = true, Options = ConfigurationPropertyOptions.IsRequired)]
        public NameValueConfigurationCollection QuartzProperties
        {
            get
            {
                return this["quartz"] as NameValueConfigurationCollection;
            }
            set
            {
                this["quartz"] = value;
            }
        }

        [ConfigurationProperty("jobs", IsRequired = true, Options = ConfigurationPropertyOptions.IsRequired)]
        public NamedComponentElementCollection Jobs
        {
            get
            {
                return this["jobs"] as NamedComponentElementCollection;
            }
            set
            {
                this["jobs"] = value;
            }
        }

        [ConfigurationProperty("attachments", IsRequired = false)]
        public NamedComponentElementCollection Attachments
        {
            get
            {
                return this["attachments"] as NamedComponentElementCollection;
            }
            set
            {
                this["attachments"] = value;
            }
        }

        public JobContainerSection()
            : base()
        {
            this["type"] = "Nuts.QuartzJob.Impl.JobContainer,Nuts.QuartzJob";
            this["jobs"] = new NamedComponentElementCollection();
            this["attachments"] = new NamedComponentElementCollection();
        }

    }
}
