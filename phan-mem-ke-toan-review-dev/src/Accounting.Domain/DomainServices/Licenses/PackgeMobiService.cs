using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Licenses;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Accounting.Windows;

namespace Accounting.DomainServices.Licenses
{
    public class PackgeMobiService : BaseDomainService<PackageMobi, string>
    {
        public PackgeMobiService(IRepository<PackageMobi, string> repository) : base(repository)
        {
        }

    }
}

