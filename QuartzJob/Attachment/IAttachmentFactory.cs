using Joy.Common.Configuration;

namespace Joy.Common.QuartzJob.Attachment
{
    public interface IAttachmentFactory : IConfigable
    {
        object GetAttachment();
    }
}
