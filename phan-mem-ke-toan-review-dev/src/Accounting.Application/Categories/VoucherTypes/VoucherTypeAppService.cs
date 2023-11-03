using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Catgories.VoucherTypes;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Categories.VoucherTypes
{


    public class VoucherTypeAppService : AccountingAppService, IVoucherTypeAppService
    {
        #region Fields
        private readonly VoucherTypeService _voucherTypeService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;

        #endregion
        #region Ctor
        public VoucherTypeAppService(VoucherTypeService voucherTypeService,
                            LicenseBusiness licenseBusiness,
                            UserService userService,
                            WebHelper webHelper)
        {
            _voucherTypeService = voucherTypeService;
            _licenseBusiness = licenseBusiness;
            _userService = userService;
            _webHelper = webHelper;
        }
        #endregion


        [Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<VoucherTypeDto> CreateAsync(CrudVoucherTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CrudVoucherTypeDto, VoucherType>(dto);
            var result = await _voucherTypeService.CreateAsync(entity);
            return ObjectMapper.Map<VoucherType, VoucherTypeDto>(result);
        }



        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _voucherTypeService.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task<PageResultDto<VoucherTypeDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }

        public async Task<PageResultDto<VoucherTypeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<VoucherTypeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<VoucherType, VoucherTypeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }


        [Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudVoucherTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            var entity = await _voucherTypeService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _voucherTypeService.UpdateAsync(entity);
        }

        public async Task<VoucherTypeDto> GetByIdAsync(string caseId)
        {
            var accCase = await _voucherTypeService.GetAsync(caseId);
            return ObjectMapper.Map<VoucherType, VoucherTypeDto>(accCase);
        }

        #region Private
        private async Task<IQueryable<VoucherType>> Filter(PageRequestDto dto)
        {
            var queryable = await _voucherTypeService.GetQueryableAsync();

            return queryable;
        }
        #endregion
    }
}


