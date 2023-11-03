using Accounting.Categories.Contracts;
using Accounting.Categories.Products;
using Accounting.Catgories.InventoryRecords;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class InventoryRecordService : BaseDomainService<InventoryRecord, string>
    {
        public InventoryRecordService(IRepository<InventoryRecord, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(InventoryRecord entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && (entity.VoucherNumber ?? "") != ""
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(CrudInventoryRecordDto entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && (entity.VoucherNumber ?? "") != ""
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(InventoryRecord entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InventoryRecord, ErrorCode.Duplicate),
                        $"InventoryRecord Code ['{entity.VoucherNumber}'] already exist ");
            }
        }
    }
}
