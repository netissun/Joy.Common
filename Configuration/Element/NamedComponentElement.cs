using System.Configuration;

namespace Joy.Common.Configuration.Element
{
    public class NamedComponentElement : ComponentElement, INamedComponentConfig
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true, Options = ConfigurationPropertyOptions.IsKey)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set
            {
                this["name"] = value;
            }
        }

        public NamedComponentElement()
            : base()
        {
            this["name"] = string.Empty;
        }

    }
}
