using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Others.PaymentTerms;
using Accounting.Catgories.Others.PaymentTerms;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.Others
{
    public class PaymentTermAppService : AccountingAppService, IPaymentTermAppService
    {
        #region Field
        private readonly PaymentTermService _paymentTermService;
        private readonly PaymentTermDetailService _paymentTermDetailService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public PaymentTermAppService(PaymentTermService accPartnerService,
                                PaymentTermDetailService paymentTermDetailService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper)
        {
            _paymentTermService = accPartnerService;
            _paymentTermDetailService = paymentTermDetailService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion

        [Authorize(AccountingPermissions.PaymentTermManagerCreate)]
        public async Task<PaymentTermDto> CreateAsync(CrudPaymentTermDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            if (dto.PaymentTermDetails.Count == 0)
            {
                throw new Exception("Bạn chưa nhập chi tiết điều kiện thanh toán vui lòng thử lại!");
            }
            var entity = ObjectMapper.Map<CrudPaymentTermDto, PaymentTerm>(dto);
            var result = await _paymentTermService.CreateAsync(entity);
            return ObjectMapper.Map<PaymentTerm, PaymentTermDto>(result);
        }

        [Authorize(AccountingPermissions.PaymentTermManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _paymentTermService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.PaymentTermManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }

        [Authorize(AccountingPermissions.PaymentTermManagerView)]
        public Task<PageResultDto<PaymentTermDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.PaymentTermManagerView)]
        public async Task<PageResultDto<PaymentTermDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<PaymentTermDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<PaymentTerm, PaymentTermDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<PaymentTermDetailDto>> GetListPaymentTermDetailAsync(string paymentTermId)
        {
            var paymentTermDetails = await _paymentTermDetailService.GetByPamentTermId(paymentTermId);
            var dtos = paymentTermDetails.Select(p => ObjectMapper.Map<PaymentTermDetail, PaymentTermDetailDto>(p)).ToList();
            return dtos;
        }

        [Authorize(AccountingPermissions.PaymentTermManagerUpdate)]
        public async Task UpdateAsync(string id, CrudPaymentTermDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _paymentTermService.GetAsync(id);
            if (dto.PaymentTermDetails.Count == 0)
            {
                throw new Exception("Bạn chưa nhập chi tiết điều kiện thanh toán vui lòng thử lại!");
            }
            ObjectMapper.Map(dto, entity);
            try
            {
                var details = await _paymentTermDetailService.GetByPamentTermId(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (details != null)
                {
                    await _paymentTermDetailService.DeleteManyAsync(details);
                }
                await _paymentTermService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<PaymentTermDto> GetByIdAsync(string paymentTermId)
        {
            var paymentTerms = await _paymentTermService.GetAsync(paymentTermId);
            return ObjectMapper.Map<PaymentTerm, PaymentTermDto>(paymentTerms);
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var queryable = await _paymentTermService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        #region Private
        private async Task<IQueryable<PaymentTerm>> Filter(PageRequestDto dto)
        {
            var queryable = await _paymentTermService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        #endregion
    }
}
