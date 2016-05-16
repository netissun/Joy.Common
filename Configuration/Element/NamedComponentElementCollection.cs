using System.Configuration;

namespace Joy.Common.Configuration.Element
{
    public class NamedComponentElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new NamedComponentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as NamedComponentElement).Name;
        }

        public NamedComponentElementCollection()
            : base()
        {
        }

    }
}
