using System.Configuration;

namespace Joy.Common.Configuration
{
    public interface IComponentConfig
    {
        string Type { get; set; }
        NameValueConfigurationCollection ComponentProperties { get; set; }
    }
}
