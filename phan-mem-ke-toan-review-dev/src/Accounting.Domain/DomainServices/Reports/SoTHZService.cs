using System;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class SoTHZService : BaseDomainService<SoTHZ, string>
    {
        public SoTHZService(IRepository<SoTHZ, string> repository) : base(repository)
        {
        }
    }
}

