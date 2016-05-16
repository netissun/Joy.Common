using Joy.Common.Configuration.Element;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joy.Common.QuartzJob.Configuration
{
    public class JobContainerElement : ComponentElement, IJobContainerConfig
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

        public JobContainerElement()
            : base()
        {
            this["type"] = "Nuts.QuartzJob.Impl.JobContainer,Nuts.QuartzJob";
            this["jobs"] = new NamedComponentElementCollection();
            this["attachments"] = new NamedComponentElementCollection();
        }

    }
}
