using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class ConfigForwardYearService : BaseDomainService<ConfigForwardYear, string>
    {
        public ConfigForwardYearService(IRepository<ConfigForwardYear, string> repository) : base(repository)
        {
        }

    }
}

