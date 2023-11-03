using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;

using Accounting.Exceptions;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class VoucherTypeService : BaseDomainService<VoucherType, string>
    {

        public VoucherTypeService(IRepository<VoucherType, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(VoucherType entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(VoucherType entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherType, ErrorCode.Duplicate),
                        $"VoucherType Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<List<VoucherType>> GetByVoucherTypeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<VoucherType> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }

}