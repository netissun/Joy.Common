using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using System.Collections.Specialized;
using Joy.Common.Configuration;

namespace Joy.Common.QuartzJob
{
    public interface IJobContainer : IConfigable
    {
        void SetQuartzProperties(NameValueCollection props);
        IDictionary<string, object> Attachments { get; set; }
        void AddJob(IJobDetail jobDetail, string cronString);
        void Init();
        void Start();
        void Stop();
    }
}
