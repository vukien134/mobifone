using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class CareerService : BaseDomainService<Career, string>
    {
        #region Fields
        private readonly IRepository<DefaultCareer, string> _defaultCareerRepository;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public CareerService(IRepository<Career, string> repository,
                IRepository<DefaultCareer, string> defaultCareerRepository,
                IStringLocalizer<AccountingResource> localizer
            ) 
            : base(repository)
        {
            _defaultCareerRepository = defaultCareerRepository;
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(Career entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistListAsync()
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any();
        }
        public async Task<Career> GetById(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<Career>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
        public async Task<List<DefaultCareer>> GetDefaultCareersAsync()
        {
            return await _defaultCareerRepository.GetListAsync();
        }
        public async Task<List<DefaultCareer>> GetByTenantTypeAsync(int? tenantType)
        {
            return await _defaultCareerRepository.GetListAsync(p => p.TenantType == tenantType);                                
        }
        public override async Task CheckDuplicate(Career entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Career, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
    }
}
