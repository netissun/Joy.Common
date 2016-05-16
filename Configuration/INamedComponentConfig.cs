
namespace Joy.Common.Configuration
{
    public interface INamedComponentConfig : IComponentConfig
    {
        string Name { get; set; }
    }
}
