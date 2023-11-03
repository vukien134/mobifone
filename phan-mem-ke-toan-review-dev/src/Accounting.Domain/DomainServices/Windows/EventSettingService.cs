using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class EventSettingService : BaseDomainService<EventSetting, string>
    {
        public EventSettingService(IRepository<EventSetting, string> repository) : base(repository)
        {
        }
    }
}
