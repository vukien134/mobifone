using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DepartmentService : BaseDomainService<Department, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public DepartmentService(IRepository<Department, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(Department entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode,string departmentCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == departmentCode);
        }
        public override async Task CheckDuplicate(Department entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<Department> GetDepartmentByCodeAsync(string code, string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == OrdCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<Department>> GetListDepartmentAsync(string Code, string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == OrdCode);
            if (!string.IsNullOrEmpty(Code))
            {
                queryable = queryable.Where(p => p.Code.Equals(Code));
            }
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsParentGroup(string groupId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == groupId);
        }
    }
}
