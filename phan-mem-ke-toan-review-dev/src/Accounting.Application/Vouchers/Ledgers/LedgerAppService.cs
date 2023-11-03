using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Accounting.Vouchers.Ledgers
{
    public class LedgerAppService : AccountingAppService, ILedgerAppService
    {
        #region Fields
        private readonly LedgerService _ledgerService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;

        #endregion
        #region Ctor
        public LedgerAppService(LedgerService ledgerService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper)
        {
            _ledgerService = ledgerService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<LedgerDto> CreateAsync(CrudLedgerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudLedgerDto, Ledger>(dto);
            var result = await _ledgerService.CreateAsync(entity);
            return ObjectMapper.Map<Ledger, LedgerDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _ledgerService.DeleteAsync(id);
        }

        public async Task<PageResultDto<LedgerDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<LedgerDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Ord0).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Ledger, LedgerDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudLedgerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _ledgerService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _ledgerService.UpdateAsync(entity);
        }
        #region Private
        private async Task<IQueryable<Ledger>> Filter(PageRequestDto dto)
        {
            var queryable = await _ledgerService.GetQueryableAsync();
            return queryable;
        }
        #endregion
    }
}
