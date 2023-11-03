using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Invoices.InvoiceSuppliers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Invoice.InvoiceSuppliers
{
    public class InvoiceSupplierAppService : AccountingAppService, IInvoiceSupplierAppService
    {
        #region Field
        private readonly InvoiceSupplierService _invoiceSupplierService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        #endregion
        #region Ctor
        public InvoiceSupplierAppService(InvoiceSupplierService invoiceSupplierService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService)
        {
            _invoiceSupplierService = invoiceSupplierService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
        }
        #endregion
        [Authorize(AccountingPermissions.InvoiceSupplierManagerCreate)]
        public async Task<InvoiceSupplierDto> CreateAsync(CrudInvoiceSupplierDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudInvoiceSupplierDto, InvoiceSupplier>(dto);
            var result = await _invoiceSupplierService.CreateAsync(entity);
            return ObjectMapper.Map<InvoiceSupplier, InvoiceSupplierDto>(result);
        }
        [Authorize(AccountingPermissions.InvoiceSupplierManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _invoiceSupplierService.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.InvoiceSupplierManagerDelete)]
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
        [Authorize(AccountingPermissions.InvoiceSupplierManagerView)]
        public Task<PageResultDto<InvoiceSupplierDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.InvoiceSupplierManagerView)]
        public async Task<PageResultDto<InvoiceSupplierDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InvoiceSupplierDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InvoiceSupplier, InvoiceSupplierDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.InvoiceSupplierManagerUpdate)]
        public async Task UpdateAsync(string id, CrudInvoiceSupplierDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _invoiceSupplierService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _invoiceSupplierService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }

        public async Task<InvoiceSupplierDto> GetByIdAsync(string partnerId)
        {
            var partner = await _invoiceSupplierService.GetAsync(partnerId);
            return ObjectMapper.Map<InvoiceSupplier, InvoiceSupplierDto>(partner);
        }
        #region Private
        private async Task<IQueryable<InvoiceSupplier>> Filter(PageRequestDto dto)
        {
            var queryable = await _invoiceSupplierService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }


        #endregion
    }
}
