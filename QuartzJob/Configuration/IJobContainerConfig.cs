using Joy.Common.Configuration;
using Joy.Common.Configuration.Element;
using System.Configuration;

namespace Joy.Common.QuartzJob.Configuration
{
    public interface IJobContainerConfig : IComponentConfig
    {
        NameValueConfigurationCollection QuartzProperties { get; set; }

        NamedComponentElementCollection Jobs { get; set; }

        NamedComponentElementCollection Attachments { get; set; }
    }
}
