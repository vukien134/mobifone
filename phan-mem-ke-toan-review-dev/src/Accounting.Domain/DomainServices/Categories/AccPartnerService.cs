using Accounting.Categories.Partners;
using Accounting.Categories.Salaries;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class AccPartnerService : BaseDomainService<AccPartner, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly PartnerGroupService _partnerGroupService;
        #endregion
        public AccPartnerService(IRepository<AccPartner, string> repository,
                IStringLocalizer<AccountingResource> localizer,
                PartnerGroupService partnerGroupService
            ) : base(repository)
        {
            _localizer = localizer;
            _partnerGroupService = partnerGroupService;
        }
        public async Task<bool> IsExistCode(AccPartner entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode, string partnerCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == partnerCode);
        }
        public override async Task CheckDuplicate(AccPartner entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }

        public async Task CheckPartnerGroup(AccPartner entity)
        {
            if ((await _partnerGroupService.IsParentGroup(entity.PartnerGroupId)))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.Other), "Mã nhóm đối tượng là nhóm mẹ");
            }
        }

        public async Task<AccPartner> GetAccPartnerByCodeAsync(string Code, string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == Code && p.OrgCode == OrdCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<AccPartner>> GetAccPartnerAsync(string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == OrdCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccPartner>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, $"%{filterValue}%") || EF.Functions.ILike(p.Name, $"%{filterValue}%")))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public async Task<bool> IsExistPartnerGroupIdAsync(string groupId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.PartnerGroupId == groupId);
        }
        public async Task<List<AccPartner>> GetByParentIdAsync(string parentId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.PartnerGroupId.Equals(parentId));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccPartner>> GetByIdAsync(string parentId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id.Equals(parentId));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccPartner>> GetAllByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode);
            return queryable.ToList();
        }
    }
}
