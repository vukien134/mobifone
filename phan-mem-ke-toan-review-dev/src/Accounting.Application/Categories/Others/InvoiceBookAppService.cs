using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Others.InvoiceBooks;
using Accounting.Catgories.Others.Invoices;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class InvoiceBookAppService : AccountingAppService,IInvoiceBookAppService
    {
        #region Fields
        private readonly InvoiceBookService _invoiceBookService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public InvoiceBookAppService(InvoiceBookService invoiceService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _invoiceBookService = invoiceService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion

        [Authorize(AccountingPermissions.InvoiceBookManagerCreate)]
        public async Task<InvoiceBookDto> CreateAsync(CrudInvoiceBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudInvoiceBookDto, InvoiceBook>(dto);
            var result = await _invoiceBookService.CreateAsync(entity);
            return ObjectMapper.Map<InvoiceBook, InvoiceBookDto>(result);
        }

        [Authorize(AccountingPermissions.InvoiceBookManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _invoiceBookService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.InvoiceBookManagerDelete)]
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

        [Authorize(AccountingPermissions.InvoiceBookManagerView)]
        public Task<PageResultDto<InvoiceBookDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.InvoiceBookManagerView)]
        public async Task<PageResultDto<InvoiceBookDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InvoiceBookDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InvoiceBook, InvoiceBookDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.InvoiceBookManagerUpdate)]
        public async Task UpdateAsync(string id, CrudInvoiceBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _invoiceBookService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _invoiceBookService.UpdateAsync(entity);
        }
        public async Task<InvoiceBookDto> GetByIdAsync(string invoiceBookId)
        {
            var entity = await _invoiceBookService.GetAsync(invoiceBookId);
            return ObjectMapper.Map<InvoiceBook, InvoiceBookDto>(entity);
        }
        public async Task<List<InvoiceBookComboItemDto>> GetDataReference()
        {
            var queryable = await _invoiceBookService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes.Select(p => new InvoiceBookComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                InvoiceSerial = p.InvoiceSerial
            }).ToList();
        }
        #region Private
        private async Task<IQueryable<InvoiceBook>> Filter(PageRequestDto dto)
        {
            var queryable = await _invoiceBookService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());


            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.Code.Contains(dto.QuickSearch));
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }

        #endregion
    }
}
