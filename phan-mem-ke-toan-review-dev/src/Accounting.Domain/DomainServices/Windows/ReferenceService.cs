using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class ReferenceService : BaseDomainService<Reference, string>
    {
        public ReferenceService(IRepository<Reference, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(Reference entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(Reference entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tab, ErrorCode.Duplicate),
                        $"Reference Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<Reference> GetWithDetailAsync(string id)
        {
            var queryable = await this.GetRepository().WithDetailsAsync(p => p.ReferenceDetails);
            queryable = queryable.Where(p => p.Id.Equals(id));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);            
        }
    }
}
