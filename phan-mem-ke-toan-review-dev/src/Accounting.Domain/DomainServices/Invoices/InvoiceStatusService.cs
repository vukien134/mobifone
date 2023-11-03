using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Invoices;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Invoices
{
    public class InvoiceStatusService : BaseDomainService<InvoiceStatus, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public InvoiceStatusService(IRepository<InvoiceStatus, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(InvoiceStatus entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(InvoiceStatus entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceStatus, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<InvoiceStatus>> GetList(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<InvoiceStatus> GetByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public IQueryable<InvoiceStatus> GetQueryableQuickSearch(IQueryable<InvoiceStatus> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
    }
}
