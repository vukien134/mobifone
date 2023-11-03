using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class RecordingVoucherBookService : BaseDomainService<RecordingVoucherBook, string>
    {
        public RecordingVoucherBookService(IRepository<RecordingVoucherBook, string> repository) : base(repository)
        {
        }
    }
}
