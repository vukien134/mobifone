using Accounting.Categories.CostProductions;
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
    public class GroupCoefficientService : BaseDomainService<GroupCoefficient, string>
    {
        public GroupCoefficientService(IRepository<GroupCoefficient, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(GroupCoefficient entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(GroupCoefficient entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.GroupCoefficient, ErrorCode.Duplicate),
                        $"GroupCoefficient Code ['{entity.Code}'] already exist ");
            }
        }
    }
}
