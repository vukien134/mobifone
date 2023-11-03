using Accounting.Categories.Others.InvoiceBooks;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class InvoiceBookService : BaseDomainService<InvoiceBook, string>
    {
        public InvoiceBookService(IRepository<InvoiceBook, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(InvoiceBook entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(InvoiceBook entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceBook, ErrorCode.Duplicate),
                        $"InvoiceBook Code ['{entity.Code}'] already exist ");
            }
        }
    }
}
