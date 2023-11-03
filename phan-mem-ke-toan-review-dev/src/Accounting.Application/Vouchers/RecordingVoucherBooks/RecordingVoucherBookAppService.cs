using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.RecordingVoucherBooks
{
    public class RecordingVoucherBookAppService : AccountingAppService,IRecordingVoucherBookAppService
    {
        #region Fields
        private readonly RecordingVoucherBookService _recordingService;
        private readonly LedgerService _ledgerService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public RecordingVoucherBookAppService(RecordingVoucherBookService recordingService,
                            LedgerService ledgerService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _recordingService = recordingService;
            _ledgerService = ledgerService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion

        [Authorize(AccountingPermissions.RecordingVoucherBookManagerCreate)]
        public async Task<RecordingVoucherBookDto> CreateAsync(CrudRecordingVoucherBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = ObjectMapper.Map<CrudRecordingVoucherBookDto, RecordingVoucherBook>(dto);
            var result = await _recordingService.CreateAsync(entity);
            // Update Chứng từ ghi sổ
            await UpdateRecordingVoucherBook(dto);
            return ObjectMapper.Map<RecordingVoucherBook, RecordingVoucherBookDto>(result);
        }

        [Authorize(AccountingPermissions.RecordingVoucherBookManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _recordingService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.RecordingVoucherBookManagerDelete)]
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

        [Authorize(AccountingPermissions.RecordingVoucherBookManagerView)]
        public async Task<PageResultDto<RecordingVoucherBookDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<RecordingVoucherBookDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<RecordingVoucherBookDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.VoucherDate).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<RecordingVoucherBook, RecordingVoucherBookDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.RecordingVoucherBookManagerUpdate)]
        public async Task UpdateAsync(string id, CrudRecordingVoucherBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = await _recordingService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _recordingService.UpdateAsync(entity);
            // Update Chứng từ ghi sổ
            await UpdateRecordingVoucherBook(dto);
        }        
        public async Task<RecordingVoucherBookDto> GetByIdAsync(string recordingId)
        {
            var entity = await _recordingService.GetAsync(recordingId);
            return ObjectMapper.Map<RecordingVoucherBook, RecordingVoucherBookDto>(entity);
        }
        
        #region Private
        private async Task<IQueryable<RecordingVoucherBook>> Filter(PageRequestDto dto)
        {
            var queryable = await _recordingService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));            

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }

        private async Task UpdateRecordingVoucherBook(CrudRecordingVoucherBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var ledgers = await _ledgerService.GetQueryableAsync();
            var lstLedger = ledgers.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                      && p.Year == _webHelper.GetCurrentYear()
                                      && p.VoucherDate >= dto.FromDate
                                      && p.VoucherDate <= dto.ToDate
                                      && (((dto.LstVoucherCode ?? "") == "") || dto.LstVoucherCode.Contains(p.VoucherCode))
                                      && (((dto.DebitAcc ?? "") == "") || p.DebitAcc.StartsWith(dto.DebitAcc))
                                      && (((dto.CreditAcc ?? "") == "") || p.CreditAcc.StartsWith(dto.CreditAcc))
                                      && (dto.TypeFilter == 1 || (p.RecordingVoucherNumber ?? "") == "")
                                      ).ToList();
            foreach (var item in lstLedger)
            {
                item.RecordingVoucherNumber = dto.VoucherNumber;
                await _ledgerService.UpdateAsync(item);
            }
        }

        #endregion
    }
}
