using System;
using System.Linq;
using System.Threading.Tasks;
using Accounting.DomainServices.BaseServices;
using Accounting.Licenses;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Licenses
{
    public class RegLicenseInfoService : BaseDomainService<RegLicenseInfo, string>
    {
        public RegLicenseInfoService(IRepository<RegLicenseInfo, string> repository) : base(repository)
        {

        }
    }
}

