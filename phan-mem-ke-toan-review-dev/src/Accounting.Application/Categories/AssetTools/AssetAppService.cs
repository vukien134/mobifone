using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.AssetTools;
using Accounting.Catgories.Others.Departments;
using Accounting.Catgories.Reasons;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.VoucherNumbers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.AssetTools
{
    public class AssetAppService : AccountingAppService, IAssetAppService
    {
        #region Field
        private readonly AssetToolService _assetToolService;
        private readonly DefaultReasonService _defaultReasonService;
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AssetToolDetailDepreciationService _assetToolDetailDepreciationService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly AssetToolDepreciationService _assetToolDepreciationService;
        private readonly AssetToolAccountService _assetToolAccountService;
        private readonly AdjustDepreciationService _adjustDepreciationService;
        private readonly AdjustDepreciationDetailService _adjustDepreciationDetailService;
        private readonly ReasonService _reasonService;
        private readonly LedgerService _ledgerService;
        private readonly PurposeService _purposeService;
        private readonly DepartmentService _departmentService;
        private readonly CapitalService _capitalService;
        private readonly AccountSystemService _accountSystemService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly AccCaseService _accCaseService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherAppService _accVoucherAppService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccVoucherDetailService _accVoucherDetailService;        
        private readonly TenantSettingService _tenantSettingService;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public AssetAppService(AssetToolService assetToolService,
                                DefaultReasonService defaultReasonService,
                                AssetToolGroupService assetToolGroupService,
                                AssetToolDetailService assetToolDetailService,
                                AssetToolDetailDepreciationService assetToolDetailDepreciationService,
                                AssetToolStoppingDepreciationService assetToolStoppingDepreciationService,
                                AssetToolAccessoryService assetToolAccessoryService,
                                AssetToolDepreciationService assetToolDepreciationService,
                                AssetToolAccountService assetToolAccountService,
                                AdjustDepreciationService adjustDepreciationService,
                                AdjustDepreciationDetailService adjustDepreciationDetailService,
                                ReasonService reasonService,
                                LedgerService ledgerService,
                                PurposeService purposeService,
                                CapitalService capitalService,
                                AccountSystemService accountSystemService,
                                FProductWorkService fProductWorkService,
                                AccSectionService accSectionService,
                                AccCaseService accCaseService,
                                AccountingCacheManager accountingCacheManager,
                                WorkPlaceSevice workPlaceSevice,
                                DepartmentService departmentService,
                                AccVoucherService accVoucherService,
                                AccVoucherAppService accVoucherAppService,
                                AccPartnerService accPartnerService,
                                AccVoucherDetailService accVoucherDetailService,                                
                                TenantSettingService tenantSettingService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _assetToolService = assetToolService;
            _defaultReasonService = defaultReasonService;
            _assetToolGroupService = assetToolGroupService;
            _assetToolDetailService = assetToolDetailService;
            _assetToolDetailDepreciationService = assetToolDetailDepreciationService;
            _assetToolStoppingDepreciationService = assetToolStoppingDepreciationService;
            _assetToolAccessoryService = assetToolAccessoryService;
            _assetToolDepreciationService = assetToolDepreciationService;
            _assetToolAccountService = assetToolAccountService;
            _adjustDepreciationService = adjustDepreciationService;
            _adjustDepreciationDetailService = adjustDepreciationDetailService;
            _reasonService = reasonService;
            _ledgerService = ledgerService;
            _purposeService = purposeService;
            _capitalService = capitalService;
            _accountSystemService = accountSystemService;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
            _accCaseService = accCaseService;
            _accountingCacheManager = accountingCacheManager;
            _workPlaceSevice = workPlaceSevice;
            _departmentService = departmentService;
            _accVoucherService = accVoucherService;
            _accVoucherAppService = accVoucherAppService;
            _accPartnerService = accPartnerService;
            _accVoucherDetailService = accVoucherDetailService;            
            _tenantSettingService = tenantSettingService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.AssetManagerCreate)]
        public async Task<AssetToolDto> CreateAsync(CrudAssetToolDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Asset;
            var dataDetails = dto.AssetToolDetails.ToList();           
            var capitalData = await _capitalService.GetQueryableAsync();
            var departmentData = await _departmentService.GetQueryableAsync();
            foreach (var data in dataDetails)
            {
                if (data.CapitalCode.IsNullOrEmpty() == false)
                {
                    if (capitalData.Where(x => x.Code == data.CapitalCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Capital, ErrorCode.NotFoundEntity),
                            _localizer["Err:CapitalData"]);
                    }
                }
                if (data.DepartmentCode.IsNullOrEmpty() == false)
                {
                    if (departmentData.Where(x => x.Code == data.DepartmentCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.NotFoundEntity),
                            _localizer["Err:DepartmentData"]);
                    }
                }
            }
            var entity = ObjectMapper.Map<CrudAssetToolDto, AssetTool>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _assetToolService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AssetTool, AssetToolDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        [Authorize(AccountingPermissions.AssetManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _assetToolService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AssetToolCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetTool, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }            
            await _assetToolService.DeleteAsync(id);
            try
            {
                var assetToolDetails = await _assetToolDetailService.GetByAssetToolIdAsync(id);
                var assetToolAccessories = await _assetToolAccessoryService.GetByAssetToolIdAsync(id);
                var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(id);
                var assetToolDepreciations = await _assetToolDepreciationService.GetByAssetToolIdAsync(id);
                var assetToolAccounts = await _assetToolAccountService.GetByAssetToolIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (assetToolDetails != null)
                {
                    await _assetToolDetailService.DeleteManyAsync(assetToolDetails);
                }
                if (assetToolAccessories != null)
                {
                    await _assetToolAccessoryService.DeleteManyAsync(assetToolAccessories);
                }
                if (assetToolStoppingDepreciations != null)
                {
                    await _assetToolStoppingDepreciationService.DeleteManyAsync(assetToolStoppingDepreciations);
                }
                if (assetToolDepreciations != null)
                {
                    await _assetToolDepreciationService.DeleteManyAsync(assetToolDepreciations);
                }
                if (assetToolAccounts != null)
                {
                    await _assetToolAccountService.DeleteManyAsync(assetToolAccounts);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        [Authorize(AccountingPermissions.AssetManagerView)]
        public Task<PageResultDto<AssetToolDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.AssetManagerView)]
        public async Task<PageResultDto<AssetToolDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AssetToolDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetTool, AssetToolDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.AssetManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAssetToolDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Asset;
            dto.Id = id;
            var entity = await _assetToolService.GetAsync(id);
            var dataDetails = dto.AssetToolDetails.ToList();
            var capitalData = await _capitalService.GetQueryableAsync();
            var departmentData = await _departmentService.GetQueryableAsync();
            foreach (var data in dataDetails)
            {
                if (data.CapitalCode.IsNullOrEmpty() == false)
                {
                    if (capitalData.Where(x => x.Code == data.CapitalCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Capital, ErrorCode.NotFoundEntity),
                            _localizer["Err:CapitalData"]);
                    }
                }
                if (data.DepartmentCode.IsNullOrEmpty() == false)
                {
                    if (departmentData.Where(x => x.Code == data.DepartmentCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.NotFoundEntity),
                            _localizer["Err:DepartmentData"]);
                    }
                }
            }
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            try
            {
                var assetToolDetails = await _assetToolDetailService.GetByAssetToolIdAsync(id);
                var assetToolAccessories = await _assetToolAccessoryService.GetByAssetToolIdAsync(id);
                var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (assetToolDetails != null)
                {
                    await _assetToolDetailService.DeleteManyAsync(assetToolDetails);
                }
                if (assetToolAccessories != null)
                {
                    await _assetToolAccessoryService.DeleteManyAsync(assetToolAccessories);
                }
                if (assetToolStoppingDepreciations != null)
                {
                    await _assetToolStoppingDepreciationService.DeleteManyAsync(assetToolStoppingDepreciations);
                }

                await _assetToolService.UpdateAsync(entity);
                if (isChangeCode)
                {
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.AssetToolCode, dto.Code, oldCode, entity.OrgCode);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AssetToolDto> GetByIdAsync(string assetToolId)
        {
            var assetTool = await _assetToolService.GetAsync(assetToolId);
            return ObjectMapper.Map<AssetTool, AssetToolDto>(assetTool);
        }

        public async Task<List<AssetToolDetailDto>> GetAssetToolDetailAsync(string productId)
        {
            var assetToolDetails = await _assetToolDetailService.GetByAssetToolIdAsync(productId);
            var dtos = assetToolDetails.Select(p => ObjectMapper.Map<AssetToolDetail, AssetToolDetailDto>(p)).ToList();
            return dtos;
        }

        public async Task<List<AssetToolAccessoryDto>> GetAssetToolAccessoryAsync(string productId)
        {
            var assetToolAccessories = await _assetToolAccessoryService.GetByAssetToolIdAsync(productId);
            var dtos = assetToolAccessories.Select(p => ObjectMapper.Map<AssetToolAccessory, AssetToolAccessoryDto>(p)).ToList();
            return dtos;
        }

        public async Task<List<AssetToolStoppingDepreciationDto>> GetAssetToolStoppingDepreciationAsync(string productId)
        {
            var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(productId);
            var dtos = assetToolStoppingDepreciations.Select(p => ObjectMapper.Map<AssetToolStoppingDepreciation, AssetToolStoppingDepreciationDto>(p)).ToList();
            return dtos;
        }
        [Authorize(AccountingPermissions.AssetManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _assetToolService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AssetToolCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetTool, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _assetToolService.DeleteManyAsync(deleteIds);            
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<ResultDto> AssetToolAllocation(AssetToolAllocationDto dto)
        {
            var res = new ResultDto();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var priceDifference = decimal.Parse(AssetToolConst.PriceDifference.ToString());
            var assetTools = await _assetToolService.GetQueryableAsync();
            assetTools = assetTools.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.ReduceDate == null || p.ReduceDate > dto.FromDate
                                              && p.AssetOrTool == AssetToolConst.Asset);
            var dataAssetToolDetailDepreciation = new List<AssetToolDetailDepreciationDto>();
            var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetQueryableAsync();
            assetToolStoppingDepreciations = assetToolStoppingDepreciations.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var assetToolDetailDepreciation = await _assetToolDetailDepreciationService.GetQueryableAsync();
            var lstAssetToolDetailDepreciation = assetToolDetailDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            //Danh sách tài sản - công cụ cần tính
            var dataAssetTool = from a in assetTools
                                select new DataAssetToolDto
                                {
                                    Id = a.Id,
                                    OrgCode = a.OrgCode,
                                    Code = a.Code,
                                    ReduceDate = a.ReduceDate
                                };
            // Dữ liệu dừng khấu hao
            var stopDepreciation = from a in assetToolStoppingDepreciations
                                   join b in assetTools on a.AssetToolId equals b.Id
                                   select new DataStopDepreciationDto
                                   {
                                       AssetToolId = b.Id,
                                       BeginDate = a.BeginDate,
                                       EndDate = a.EndDate,
                                       Ord0 = a.Ord0
                                   };
            DateTime fromDate = dto.FromDate;
            DateTime toDate = dto.ToDate;
            var dataDate = new List<AssetToolAllocationDto>();
            dataDate.Add(new AssetToolAllocationDto
            {
                FromDate = Convert.ToDateTime(fromDate.Year + "-" + ((fromDate.Month < 10) ? "0" + fromDate.Month : fromDate.Month) + "-01"),
                ToDate = await LastDay(fromDate)
            });
            while (fromDate < toDate)
            {
                fromDate = await NextMonth(fromDate);
                if (fromDate < toDate)
                {
                    dataDate.Add(new AssetToolAllocationDto
                    {
                        FromDate = fromDate,
                        ToDate = await LastDay(fromDate)
                    });
                }
            }
            var iQAssetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
            var lstAssetToolDepreciation = iQAssetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            foreach (var item in dataDate)
            {
                var year = item.FromDate.Year;
                var month = item.FromDate.Month;
                var maxDayOfMonth = item.ToDate.Day;
                var assetToolDepreciationDele = await _assetToolDepreciationService.GetQueryableAsync();

                //Xóa dữ liệu đã tính theo tháng
                assetToolDepreciationDele = assetToolDepreciationDele.Where(p => p.Year == year
                                                                              && p.Month == month
                                                                              && p.OrgCode == _webHelper.GetCurrentOrgUnit());
                await _assetToolDepreciationService.DeleteManyAsync(assetToolDepreciationDele,true);
                //Update hạch toán theo tháng
                var assetToolAccount = await _assetToolAccountService.GetQueryableAsync();
                assetToolAccount = assetToolAccount.Where(p => p.Year == year
                                                            && p.Month == month
                                                            && p.OrgCode == _webHelper.GetCurrentOrgUnit());
                dataAssetTool = from a in dataAssetTool
                                join b in assetToolAccount on a.Id equals b.AssetToolId into ajb
                                from b in ajb.DefaultIfEmpty()
                                select new DataAssetToolDto
                                {
                                    Id = a.Id,
                                    OrgCode = a.OrgCode,
                                    Code = a.Code,
                                    ReduceDate = a.ReduceDate,
                                    DebitAcc = b != null ? b.DebitAcc : a.DebitAcc,
                                    CreditAcc = b != null ? b.CreditAcc : a.CreditAcc,
                                    PartnerCode = b != null ? b.PartnerCode : a.PartnerCode,
                                    FProductWorkCode = b != null ? b.FProductWorkCode : a.FProductWorkCode,
                                    SectionCode = b != null ? b.SectionCode : a.SectionCode,
                                    CaseCode = b != null ? b.PartnerCode : a.CaseCode,
                                    WorkPlaceCode = b != null ? b.PartnerCode : a.WorkPlaceCode,
                                };
                var lstDataAssetTool = dataAssetTool.ToList();
                //Chi tiết tài sản cần tính
                assetToolDetailDepreciation = await _assetToolDetailDepreciationService.GetQueryableAsync();
                lstAssetToolDetailDepreciation = assetToolDetailDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() 
                                                                                     && p.DepreciationBeginDate < item.FromDate).ToList();
                dataAssetToolDetailDepreciation = lstAssetToolDetailDepreciation
                            .GroupBy(x => new { x.AssetToolId, x.Ord0 })
                            // PARTITION BY ^^^^
                            .Select(c => c.OrderByDescending(o => o.DepreciationBeginDate).Select((v, i) => new { i, v }).ToList())
                            //                   ORDER BY ^^
                            .SelectMany(c => c)
                            .Select(c => new AssetToolDetailDepreciationDto
                            {
                                AssetToolId = c.v.AssetToolId,
                                Ord0 = c.v.Ord0,
                                DepreciationAmount = c.v.DepreciationAmount,
                                DepreciationBeginDate = c.v.DepreciationBeginDate,
                                RowNumber = c.i + 1
                            }).ToList();
                dataAssetToolDetailDepreciation = dataAssetToolDetailDepreciation.Where(p => p.RowNumber == 1).ToList();

                var assetToolDetail = await _assetToolDetailService.GetQueryableAsync();
                assetToolDetail = assetToolDetail.Where(p => p.IsCalculating == "C"
                                                          && p.BeginDate != null
                                                          && p.BeginDate <= item.ToDate
                                                          && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.AssetOrTool == dto.AssetOrTool);
                var reaSon = await _reasonService.GetQueryableAsync();
                reaSon = reaSon.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                bool isReasonExists = await _reasonService.IsExistListAsync(orgCode);
                if (!isReasonExists) 
                { 
                    var defaultReasons = await _defaultReasonService.GetListAsync();
                    var entities = defaultReasons.Select(p =>
                    {
                        var dto = new CrudReasonDto()
                        {
                            Code = p.Code,
                            Name = p.Name,
                            Id = this.GetNewObjectId(),
                            OrgCode = orgCode,
                            ReasonType = p.ReasonType
                        };
                        return ObjectMapper.Map<CrudReasonDto, Reason>(dto);
                    }).ToList();

                    await _reasonService.CreateManyAsync(entities, true);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                var dataAssetToolDetail = from a in assetToolDetail.ToList()
                                          join b in reaSon.ToList() on a.UpDownCode equals b.Code
                                          join c in dataAssetTool.ToList() on a.AssetToolId equals c.Id
                                          join d in dataAssetToolDetailDepreciation on new { a.AssetToolId, a.Ord0 } equals new {d.AssetToolId, d.Ord0} into ajd
                                          from d in ajd.DefaultIfEmpty()
                                          where a.IsCalculating == "C" && a.BeginDate != null && a.BeginDate <= item.ToDate
                                          select new DataAssetToolDetailDto
                                          {
                                              Id = a.Id,
                                              AssetToolId = a.AssetToolId,
                                              AssetOrTool = a.AssetOrTool,
                                              OrgCode = a.OrgCode,
                                              AssetToolCode = c.Code,
                                              Ord0 = a.Ord0,
                                              CapitalCode = a.CapitalCode,
                                              DepartmentCode = a.DepartmentCode,
                                              BeginDate = a.BeginDate,
                                              DepreciationBeginDate0 = a.BeginDate,
                                              UpDownDate = a.UpDownDate,
                                              ReduceDate = c.ReduceDate,
                                              OriginalPrice = a.OriginalPrice,
                                              Impoverishment = a.Impoverishment,
                                              CalculatingAmount = a.CalculatingAmount,
                                              DepreciationAmount = d != null ? d.DepreciationAmount : a.DepreciationAmount,
                                              DepreciationAmountT = d != null ? d.DepreciationAmount : a.DepreciationAmount,
                                              ReasonType = b.ReasonType,
                                              MonthNumber0 = a.MonthNumber0 ?? 0,
                                              MonthNumber = a.MonthNumber ?? 0,
                                              DepreciationAmount0 = a.DepreciationAmount,
                                              AmountRemaining = (a.OriginalPrice - a.Impoverishment) * (b.ReasonType == "T" ? 1 : -1),
                                              BeginDate0 = null,
                                              EndDate0 = null,
                                              IsStoppingDepreciation = "K",
                                              DayNum = 0
                                          };
                //Update lại giá trị khấu hao nếu có điều chỉnh
                var adjustDepreciation = await _adjustDepreciationService.GetQueryableAsync();
                adjustDepreciation = adjustDepreciation.Where(p => p.BeginDate < item.ToDate
                                                                && p.OrgCode == _webHelper.GetCurrentOrgUnit());
                var adjustDepreciationDetail = await _adjustDepreciationDetailService.GetQueryableAsync();
                adjustDepreciationDetail = adjustDepreciationDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

                var dataAdjustDepreciationDetail = from a in adjustDepreciation
                                                   join b in adjustDepreciationDetail on a.Id equals b.AdjustDepreciationId
                                                   join c in dataAssetTool on a.AssetToolCode equals c.Code
                                                   orderby b.AssetToolDetailId, a.BeginDate
                                                   select new
                                                   {
                                                       AssetToolDetailId = b.AssetToolDetailId,
                                                       DepreciationAmount = b.Amount
                                                   };
                dataAssetToolDetail = from a in dataAssetToolDetail
                                      join b in dataAdjustDepreciationDetail on a.Id equals b.AssetToolDetailId into ajb
                                      from b in ajb.DefaultIfEmpty()
                                      select new DataAssetToolDetailDto
                                      {
                                          Id = a.Id,
                                          AssetToolId = a.AssetToolId,
                                          AssetOrTool = a.AssetOrTool,
                                          OrgCode = a.OrgCode,
                                          AssetToolCode = a.AssetToolCode,
                                          Ord0 = a.Ord0,
                                          CapitalCode = a.CapitalCode,
                                          DepartmentCode = a.DepartmentCode,
                                          BeginDate = a.BeginDate,
                                          DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                          MonthNumber0 = a.MonthNumber0,
                                          MonthNumber = a.MonthNumber,
                                          UpDownDate = a.UpDownDate,
                                          ReduceDate = a.ReduceDate,
                                          OriginalPrice = a.OriginalPrice,
                                          Impoverishment = a.Impoverishment,
                                          CalculatingAmount = a.CalculatingAmount,
                                          DepreciationAmount = b != null ? b.DepreciationAmount : a.DepreciationAmount,
                                          DepreciationAmountT = a.DepreciationAmountT,
                                          ReasonType = a.ReasonType,
                                          DepreciationAmount0 = a.DepreciationAmount0,
                                          AmountRemaining = a.AmountRemaining,
                                          BeginDate0 = null,
                                          EndDate0 = null,
                                          IsStoppingDepreciation = "K",
                                          DayNum = 0
                                      };
                var lstDataAssetToolDetail = dataAssetToolDetail.ToList();
                //Cập nhật giá trị còn lại chi tiết theo dòng(có gắn theo mã nguồn vốn),+thay đổi ngày bắt đầu khấu hao lấy từ lstAssetToolDepreciation khai báo bên ngoài vòng lặp
                var assetToolDepreciation = lstAssetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                      && p.DepreciationDate < item.FromDate);
                var dataAssetToolDepreciation = from a in lstAssetToolDepreciation
                                                join b in lstDataAssetTool on a.AssetToolId equals b.Id
                                                where a.DepreciationBeginDate < item.FromDate
                                                group new { a } by new
                                                {
                                                    a.AssetToolId,
                                                    a.Ord0
                                                } into gr
                                                select new
                                                {
                                                    AssetToolId = gr.Key.AssetToolId,
                                                    Ord0 = gr.Key.Ord0,
                                                    DepreciationAmount = gr.Sum(p => p.a.DepreciationAmount),
                                                    MonthNumber0 = gr.Sum(p => p.a.DepreciationAmount != 0 ? 1 : 0)
                                                };
                var iDataAssetToolDetail = from a in lstDataAssetToolDetail
                                           join b in dataAssetToolDepreciation on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 } into ajb
                                           from b in ajb.DefaultIfEmpty()
                                           select new DataAssetToolDetailDto
                                           {
                                               Id = a.Id,
                                               AssetToolId = a.AssetToolId,
                                               AssetOrTool = a.AssetOrTool,
                                               OrgCode = a.OrgCode,
                                               AssetToolCode = a.AssetToolCode,
                                               Ord0 = a.Ord0,
                                               CapitalCode = a.CapitalCode,
                                               DepartmentCode = a.DepartmentCode,
                                               BeginDate = a.BeginDate < item.FromDate ? item.FromDate : a.BeginDate,
                                               MonthNumber0 = a.MonthNumber0 + b?.MonthNumber0 ?? 0,
                                               MonthNumber = a.MonthNumber,
                                               UpDownDate = a.UpDownDate,
                                               ReduceDate = a.ReduceDate,
                                               OriginalPrice = a.OriginalPrice,
                                               Impoverishment = a.Impoverishment,
                                               CalculatingAmount = a.CalculatingAmount,
                                               DepreciationAmount = a.DepreciationAmount,
                                               DepreciationAmountT = a.DepreciationAmountT,
                                               ReasonType = a.ReasonType,
                                               DepreciationAmount0 = b != null ? b.DepreciationAmount ?? 0 : 0,
                                               AmountRemaining = a.AmountRemaining - (b != null ? b.DepreciationAmount ?? 0 : 0),
                                               BeginDate0 = null,
                                               DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                               EndDate0 = null,
                                               IsStoppingDepreciation = "K",
                                               DayNum = 0
                                           };
                //lấy những giá trị còn lại != 0
                lstDataAssetToolDetail = iDataAssetToolDetail.Where(p => p.AmountRemaining != 0).ToList();
                //Cập nhật dữ liệu thông tin khai báo dừng khấu hao
                var dataStopDepreciation = from a in stopDepreciation
                                           where a.BeginDate <= item.ToDate && (a.EndDate == null || a.EndDate >= item.FromDate)
                                           orderby a.AssetToolId, a.Ord0
                                           select new
                                           {
                                               AssetToolId = a.AssetToolId,
                                               BeginDate = a.BeginDate,
                                               EndDate = a.EndDate
                                           };
                var lstDataStopDepreciation = dataStopDepreciation.ToList();
                iDataAssetToolDetail = from a in lstDataAssetToolDetail
                                       join b in lstDataStopDepreciation on a.AssetOrTool equals b.AssetToolId into ajb
                                       from b in ajb.DefaultIfEmpty()
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = a.DepreciationAmount,
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = (b != null && b.BeginDate != null) ? (b.BeginDate < item.FromDate ? item.FromDate : b.BeginDate) : a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = (b != null && b.EndDate != null) ? (b.EndDate == null || b.EndDate >= item.ToDate ? item.ToDate : b.EndDate) : a.EndDate0,
                                           IsStoppingDepreciation = b != null ? "C" : a.IsStoppingDepreciation,
                                           DayNum = a.DayNum,
                                       };
                //Cập nhật nếu có hoặc không quy tắc "Dừng khấu hao"
                //Lần 1
                iDataAssetToolDetail = from a in iDataAssetToolDetail
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = a.DepreciationAmount,
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = a.EndDate0,
                                           IsStoppingDepreciation = a.IsStoppingDepreciation,
                                           DayNum = (a.IsStoppingDepreciation == "K") ?
                                                   ((a.BeginDate != null && a.BeginDate.Value.Year == item.FromDate.Year && a.BeginDate.Value.Month == item.FromDate.Month && a.BeginDate.Value.Day > 1) ? a.BeginDate.Value.Day - 1 : 0) :
                                                   ((a.BeginDate0 != null && a.BeginDate0.Value.Year == a.BeginDate.Value.Year && a.BeginDate0.Value.Month == a.BeginDate.Value.Month && a.BeginDate0.Value.Day > a.BeginDate.Value.Day) ? a.BeginDate0.Value.Day - a.BeginDate.Value.Day : 0),
                                       };
                //Lần 2
                iDataAssetToolDetail = from a in iDataAssetToolDetail
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = a.DepreciationAmount,
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = a.EndDate0,
                                           IsStoppingDepreciation = a.IsStoppingDepreciation,
                                           DayNum = a.DayNum + ((a.IsStoppingDepreciation == "K") ?
                                                   ((a.ReduceDate != null && a.ReduceDate.Value.Year == item.ToDate.Year && a.ReduceDate.Value.Month == item.ToDate.Month && a.ReduceDate.Value.Day <= maxDayOfMonth) ? maxDayOfMonth - a.ReduceDate.Value.Day + 1 : 0) :
                                                   ((a.EndDate0 != null && a.EndDate0.Value.Year == item.ToDate.Year && a.EndDate0.Value.Month == item.ToDate.Month && a.EndDate0.Value.Day <= maxDayOfMonth) ? maxDayOfMonth - a.EndDate0.Value.Day : 0)),
                                       };
                //Lần 3
                iDataAssetToolDetail = from a in iDataAssetToolDetail
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = a.DepreciationAmount,
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = a.EndDate0,
                                           IsStoppingDepreciation = a.IsStoppingDepreciation,
                                           DayNum = (a.IsStoppingDepreciation == "K") ? maxDayOfMonth - a.DayNum : a.DayNum
                                       };
                //Lần 4
                iDataAssetToolDetail = from a in iDataAssetToolDetail
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = (a.DepreciationAmount * (a.ReasonType == "T" ? 1 : -1) * ((a.ReduceDate <= a.BeginDate) ? 0 : a.DayNum)) / maxDayOfMonth,
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = a.EndDate0,
                                           IsStoppingDepreciation = a.IsStoppingDepreciation,
                                           DayNum = (a.ReduceDate <= a.BeginDate) ? 0 : a.DayNum
                                       };
                //lấy những bút toán giá trị khấu hao != 0
                iDataAssetToolDetail = iDataAssetToolDetail.Where(p => p.DepreciationAmount != null && p.DepreciationAmount != 0);
                //cập nhật lại giá trị khấu hao cho chuẩn
                var lstAssetToolDetail = (from a in iDataAssetToolDetail
                                       select new DataAssetToolDetailDto
                                       {
                                           Id = a.Id,
                                           AssetToolId = a.AssetToolId,
                                           AssetOrTool = a.AssetOrTool,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           Ord0 = a.Ord0,
                                           CapitalCode = a.CapitalCode,
                                           DepartmentCode = a.DepartmentCode,
                                           BeginDate = a.BeginDate,
                                           MonthNumber0 = a.MonthNumber0,
                                           MonthNumber = a.MonthNumber,
                                           UpDownDate = a.UpDownDate,
                                           ReduceDate = a.ReduceDate,
                                           OriginalPrice = a.OriginalPrice,
                                           Impoverishment = a.Impoverishment,
                                           CalculatingAmount = a.CalculatingAmount,
                                           DepreciationAmount = (Abs(a.AmountRemaining) < Abs(a.DepreciationAmount) || (Abs(a.AmountRemaining) - Abs(a.DepreciationAmount) <= priceDifference) ? a.AmountRemaining : a.DepreciationAmount),
                                           DepreciationAmountT = a.DepreciationAmountT,
                                           ReasonType = a.ReasonType,
                                           DepreciationAmount0 = a.DepreciationAmount0,
                                           AmountRemaining = a.AmountRemaining,
                                           BeginDate0 = a.BeginDate0,
                                           DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                           EndDate0 = a.EndDate0,
                                           IsStoppingDepreciation = a.IsStoppingDepreciation,
                                           DayNum = a.DayNum
                                       }).ToList();
                // Xác định những mã cần tính nhiều mức khấu hao (tính lại)
                var assetToolDetailDepreciationDel = await _assetToolDetailDepreciationService.GetQueryableAsync();
                assetToolDetailDepreciationDel = assetToolDetailDepreciationDel.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                                  && p.AssetOrTool == dto.AssetOrTool
                                                                                  && p.DepreciationBeginDate >= item.FromDate
                                                                                  && p.DepreciationBeginDate <= item.ToDate);
                await _assetToolDetailDepreciationService.DeleteManyAsync(assetToolDetailDepreciationDel, true);

                var assetToolCal = (from a in lstAssetToolDetail
                                    group new { a } by new
                                   {
                                       a.AssetToolId
                                   } into gr
                                   where gr.Count() > 1
                                   select new DataAssetToolDetailDto
                                   {
                                       AssetToolId = gr.Key.AssetToolId,
                                       Ord0 = gr.Max(p => p.a.Ord0),
                                       Ord00 = gr.Max(p => p.a.DepreciationBeginDate0 <= item.FromDate ? p.a.Ord0 : ""),
                                       Recalculate = "K",
                                       RemainingMonth = 0,
                                       RemainingMonth0 = 0,
                                   }).ToList();
                if (assetToolCal.Count() > 0)
                {
                    foreach(var itemAssetToolCal in assetToolCal)
                    {
                        var dataB = lstAssetToolDetail.Where(p => p.BeginDate > item.FromDate 
                                                               && p.BeginDate < item.ToDate
                                                               && p.AssetToolId == itemAssetToolCal.AssetToolId).FirstOrDefault();
                        var dataC = lstAssetToolDetail.Where(p => p.AssetToolId == itemAssetToolCal.AssetToolId
                                                               && p.Ord0 == itemAssetToolCal.Ord0).FirstOrDefault();
                        var dataD = lstAssetToolDetail.Where(p => p.AssetToolId == itemAssetToolCal.AssetToolId
                                                               && p.Ord0 == itemAssetToolCal.Ord00).FirstOrDefault();
                        if (dataC != null)
                        {
                            itemAssetToolCal.Recalculate = dataB == null ? "K" : "C";
                            itemAssetToolCal.RemainingMonth = dataC.MonthNumber - dataC.MonthNumber0;
                            itemAssetToolCal.RemainingMonth0 = (dataD?.MonthNumber ?? 0) - (dataD?.MonthNumber0 ?? 0);
                        }
                    }

                    assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
                    assetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                                      && p.DepreciationBeginDate <= item.FromDate
                                                                                      && p.DepreciationAmount != 0);
                    var assetToolDepreciationWithties = assetToolDepreciation.ToList()
                                                        .Where(x => assetToolCal.Any(p => p.AssetToolId == x.AssetToolId))
                                                        .GroupBy(x => new { x.AssetToolId, x.Ord0 })
                                                        // PARTITION BY ^^^^
                                                        .Select(c => c.OrderByDescending(o => o.DepreciationBeginDate).ThenByDescending(x => x.Month)
                                                                      .Select((v, i) => new { i, v }).ToList())
                                                        //                   ORDER BY ^^
                                                        .SelectMany(c => c)
                                                        .Select(c => new AssetToolDetailDepreciationDto
                                                        {
                                                            AssetToolId = c.v.AssetToolId,
                                                            Ord0 = c.v.Ord0,
                                                            DepreciationAmount = c.v.DepreciationAmount ?? 0,
                                                            RowNumber = c.i + 1
                                                        }).ToList();

                    assetToolDepreciationWithties = assetToolDepreciationWithties.Where(p => p.RowNumber == 1).ToList();

                    assetToolDetailDepreciation = await _assetToolDetailDepreciationService.GetQueryableAsync();
                    assetToolDetailDepreciation = assetToolDetailDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                                      && p.DepreciationBeginDate <= item.FromDate);
                    dataAssetToolDetailDepreciation = assetToolDetailDepreciation.ToList()
                        .GroupBy(x => new { x.AssetToolId, x.Ord0 })
                        // PARTITION BY ^^^^
                        .Select(c => c.OrderByDescending(o => o.DepreciationBeginDate).Select((v, i) => new { i, v }).ToList())
                        //                   ORDER BY ^^
                        .SelectMany(c => c)
                        .Select(c => new AssetToolDetailDepreciationDto
                        {
                            AssetToolId = c.v.AssetToolId,
                            Ord0 = c.v.Ord0,
                            DepreciationAmount = c.v.DepreciationAmount,
                            DepreciationBeginDate = c.v.DepreciationBeginDate,
                            RowNumber = c.i + 1
                        }).ToList();

                    dataAssetToolDetailDepreciation = dataAssetToolDetailDepreciation.Where(p => p.RowNumber == 1).ToList();

                    var assetToolDetailCal = (from a in lstAssetToolDetail
                                             join b in assetToolCal on a.AssetToolId equals b.AssetToolId
                                             join c in assetToolDepreciationWithties
                                                    on new { a.AssetToolId, a.Ord0 } equals new { c.AssetToolId, c.Ord0 } into ajc
                                             from c in ajc.DefaultIfEmpty()
                                             join d in dataAssetToolDetailDepreciation
                                                    on new { a.AssetToolId, a.Ord0 } equals new { d.AssetToolId, d.Ord0 } into ajd
                                             from d in ajd.DefaultIfEmpty()
                                             select new DataAssetToolDetailDto
                                             {
                                                 AssetToolId = a.AssetToolId,
                                                 Ord0 = a.Ord0,
                                                 Recalculate = b.Recalculate,
                                                 RemainingMonth0 = b.RemainingMonth0,
                                                 RemainingMonth1 = b.RemainingMonth,
                                                 RemainingMonth = b.Recalculate == "K" ? b.RemainingMonth : a.MonthNumber - a.MonthNumber0,
                                                 ReduceDate = a.ReduceDate,
                                                 BeginDate = a.BeginDate,
                                                 DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                                 BeginDate0 = a.BeginDate,
                                                 EndDate0 = b.Recalculate == "K" ? item.ToDate : a.EndDate0,
                                                 ReasonType = a.ReasonType,
                                                 DayNum = a.DayNum,
                                                 AmountRemaining = a.AmountRemaining,
                                                 DepreciationAmount0 = a.DepreciationAmount,
                                                 DepreciationAmount = d == null ? a.DepreciationAmountT : d.DepreciationAmount,
                                                 DepreciationAmountT = c == null ? a.DepreciationAmount : c.DepreciationAmount,
                                             }).ToList();

                    var assetToolDetailGR = (from a in assetToolDetailCal
                                             where a.Recalculate == "C"
                                             group new { a } by new
                                             {
                                                 a.AssetToolId,
                                                 a.BeginDate
                                             } into gr
                                             select new
                                             {
                                                 AssetToolId = gr.Key.AssetToolId,
                                                 BeginDate = gr.Key.BeginDate,
                                             }).ToList();

                    var assetToolDetailAdd = new List<DataAssetToolDetailDto>();
                    foreach (var itemGR in assetToolDetailGR)
                    {
                        //Số tháng còn lại lấy theo lần điều chỉnh gần nhất
                        var itemTop1 = assetToolDetailCal.Where(p => p.AssetToolId == itemGR.AssetToolId && p.BeginDate > itemGR.BeginDate)
                                                          .OrderBy(p => p.BeginDate).ThenBy(p => p.Ord0).Select(p => 
                                                          new DataAssetToolDetailDto 
                                                          { 
                                                              BeginDate = p.BeginDate,
                                                          }).FirstOrDefault();
                        if (itemTop1 != null)
                        {
                            itemTop1.BeginDate = itemTop1.BeginDate.Value.AddDays(-1);
                            foreach (var itemUpdate in assetToolDetailCal)
                            {
                                if (itemUpdate.AssetToolId == itemGR.AssetToolId && itemUpdate.BeginDate == itemGR.BeginDate)
                                {
                                    itemUpdate.RemainingMonth = (itemUpdate.BeginDate0 == item.FromDate && itemUpdate.DepreciationBeginDate0 < item.FromDate)
                                                            ? itemUpdate.RemainingMonth0 : itemUpdate.RemainingMonth;
                                    itemUpdate.EndDate0 = itemTop1.BeginDate;
                                }
                            };
                        }

                        var assetToolDetailDepreciationCrudDto = from a in assetToolDetailCal
                                                                 where a.BeginDate == itemGR.BeginDate
                                                                    && a.EndDate0 != null && a.EndDate0 < item.ToDate
                                                                 select new CrudAssetToolDetailDepreciationDto
                                                                 {
                                                                     Id = GetNewObjectId(),
                                                                     OrgCode = _webHelper.GetCurrentOrgUnit(),
                                                                     AssetOrTool = dto.AssetOrTool,
                                                                     AssetToolId = a.AssetToolId,
                                                                     Ord0 = a.Ord0,
                                                                     DepreciationBeginDate = a.EndDate0.Value.AddDays(1),
                                                                     DepreciationAmount = a.RemainingMonth1 == 0 ? 0 
                                                                                          : Math.Round((a.AmountRemaining - Math.Round(((a.EndDate0 - a.BeginDate0).Value.Days + 1) * a.DepreciationAmount / maxDayOfMonth ?? 0, 6)) / a.RemainingMonth1 ?? 1, 6)
                                                                 };
                        var assetToolDetailDepreciationAdd = assetToolDetailDepreciationCrudDto.Select(p => ObjectMapper.Map<CrudAssetToolDetailDepreciationDto, AssetToolDetailDepreciation>(p));
                        await _assetToolDetailDepreciationService.CreateManyAsync(assetToolDetailDepreciationAdd, true);

                        // Quét những mã tăng phía sau chèn vào
                        DateTime? DateBefore = null;
                        var dataFor = (from a in assetToolDetailCal
                                      where a.AssetToolId == itemGR.AssetToolId && a.BeginDate > itemGR.BeginDate
                                      orderby a.BeginDate descending
                                      select new
                                      {
                                          BeginDate = a.BeginDate,
                                          RemainingMonth = a.RemainingMonth
                                      }).ToList();
                        foreach (var itemFor in dataFor)
                        {
                            assetToolDetailDepreciation = await _assetToolDetailDepreciationService.GetQueryableAsync();
                            assetToolDetailDepreciation = assetToolDetailDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                                              && p.DepreciationBeginDate <= itemFor.BeginDate);
                            dataAssetToolDetailDepreciation = assetToolDetailDepreciation.ToList()
                                                            .GroupBy(x => new { x.AssetToolId, x.Ord0 })
                                                            // PARTITION BY ^^^^
                                                            .Select(c => c.OrderByDescending(o => o.DepreciationBeginDate).Select((v, i) => new { i, v }).ToList())
                                                            //                   ORDER BY ^^
                                                            .SelectMany(c => c)
                                                            .Select(c => new AssetToolDetailDepreciationDto
                                                            {
                                                                AssetToolId = c.v.AssetToolId,
                                                                Ord0 = c.v.Ord0,
                                                                DepreciationAmount = c.v.DepreciationAmount,
                                                                DepreciationBeginDate = c.v.DepreciationBeginDate,
                                                                RowNumber = c.i + 1
                                                            }).ToList();
                            dataAssetToolDetailDepreciation = dataAssetToolDetailDepreciation.Where(p => p.RowNumber == 1).ToList();

                            assetToolDetailAdd.AddRange((from a in assetToolDetailCal
                                                     join b in dataAssetToolDetailDepreciation
                                                            on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 } into ajb
                                                     from b in ajb.DefaultIfEmpty()
                                                     where a.AssetToolId == itemGR.AssetToolId && a.BeginDate == itemGR.BeginDate
                                                     select new DataAssetToolDetailDto
                                                     {
                                                         AssetToolId = a.AssetToolId,
                                                         Ord0 = a.Ord0,
                                                         Recalculate = a.Recalculate,
                                                         RemainingMonth0 = a.RemainingMonth0,
                                                         RemainingMonth1 = a.RemainingMonth1,
                                                         RemainingMonth = itemFor.RemainingMonth,
                                                         ReduceDate = a.ReduceDate,
                                                         BeginDate = itemFor.BeginDate,
                                                         DepreciationBeginDate0 = a.DepreciationBeginDate0,
                                                         BeginDate0 = itemFor.BeginDate,
                                                         EndDate0 = DateBefore != null ? DateBefore.Value.AddDays(-1) : null,
                                                         ReasonType = a.ReasonType,
                                                         DayNum = a.DayNum,
                                                         AmountRemaining = a.AmountRemaining,
                                                         DepreciationAmount0 = a.DepreciationAmount0,
                                                         DepreciationAmount = b?.DepreciationAmount ?? a.DepreciationAmount,
                                                         DepreciationAmountT = a.DepreciationAmountT,
                                                     }).ToList());
                            DateBefore = itemFor.BeginDate;
                        }
                    }
                    assetToolDetailCal.AddRange(assetToolDetailAdd);
                    foreach (var itemUpdate in assetToolDetailCal)
                    {
                        if (itemUpdate.EndDate0 == null)
                        {
                            itemUpdate.EndDate0 = item.ToDate;
                        }
                        itemUpdate.EndDate0 = (itemUpdate.ReduceDate != null && itemUpdate.ReduceDate < itemUpdate.EndDate0) 
                                              ? itemUpdate.ReduceDate 
                                              : itemUpdate.EndDate0 ;

                        var dataStopDepreciationWT = stopDepreciation.ToList()
                                                            .GroupBy(x => new { x.AssetToolId})
                                                            // PARTITION BY ^^^^
                                                            .Select(c => c.OrderBy(o => o.AssetToolId).Select((v, i) => new { i, v }).ToList())
                                                            //                   ORDER BY ^^
                                                            .SelectMany(c => c)
                                                            .Select(c => new
                                                            {
                                                                AssetToolId = c.v.AssetToolId,
                                                                BeginDate0 = c.v.BeginDate,
                                                                EndDate0 = c.v.EndDate,
                                                                RowNumber = c.i + 1
                                                            }).ToList();
                        var itemStopDepreciationWT = dataStopDepreciationWT.Where(p => p.RowNumber == 1).FirstOrDefault();

                        if (itemStopDepreciationWT != null)
                        {
                            itemUpdate.BeginDate0 = itemStopDepreciationWT.BeginDate0 < itemUpdate.BeginDate0 ? itemUpdate.BeginDate0 : itemStopDepreciationWT.BeginDate0;
                            itemUpdate.EndDate0 = (itemStopDepreciationWT.EndDate0 == null || itemStopDepreciationWT.EndDate0 > itemUpdate.EndDate0)
                                                  ? itemUpdate.EndDate0 : itemStopDepreciationWT.EndDate0;
                        }
                        // tính lại khấu hao
                        itemUpdate.DayNum = (((itemUpdate.EndDate0 - itemUpdate.BeginDate0).Value.Days + 1) > 0)
                                            ? (itemUpdate.EndDate0 - itemUpdate.BeginDate0).Value.Days + 1 : 0;
                        itemUpdate.DepreciationAmount0 = Math.Round((itemUpdate.DepreciationAmount * itemUpdate.DayNum / maxDayOfMonth) ?? 0, 6);
                        // xử lý số lẻ
                        var fixAmount = decimal.Parse(await _tenantSettingService.GetValue("VHT_GT_FIX_TS_CC", _webHelper.GetCurrentOrgUnit()));
                        itemUpdate.DepreciationAmount0 = (Math.Abs(itemUpdate.AmountRemaining ?? 0) < Math.Abs(itemUpdate.DepreciationAmount0 ?? 0)
                                                            || Math.Abs(itemUpdate.AmountRemaining ?? 0) - Math.Abs(itemUpdate.DepreciationAmount0 ?? 0) <= fixAmount)
                                                         ? itemUpdate.AmountRemaining : itemUpdate.DepreciationAmount0;
                    }
                    foreach (var itemAssetToolDetail in lstAssetToolDetail)
                    {
                        var assetToolDetailCalGr = assetToolDetailCal.Where(p => p.AssetToolId == itemAssetToolDetail.AssetToolId
                                                                                && p.Ord0 == itemAssetToolDetail.Ord0).ToList();
                        if(assetToolDetailCalGr != null)
                        {
                            itemAssetToolDetail.DepreciationAmount = assetToolDetailCalGr.Sum(p => p.DepreciationAmount0);
                        }
                    }

                }
                //Chèn dữ liệu vào bảng Khấu hao
                var assetToolDepreciations = from a in lstAssetToolDetail
                                             join b in dataAssetTool on a.AssetToolId equals b.Id
                                             select new CrudAssetToolDepreciationDto
                                             {
                                                 Id = GetNewObjectId(),
                                                 AssetOrTool = dto.AssetOrTool,
                                                 AssetToolId = b.Id,
                                                 OrgCode = a.OrgCode,
                                                 Ord0 = a.Ord0,
                                                 Year = year,
                                                 Month = month,
                                                 DepreciationDate = item.ToDate,
                                                 UpDownDate = a.UpDownDate,
                                                 DepreciationBeginDate = a.BeginDate,
                                                 AssetToolCode = a.AssetToolCode,
                                                 DepartmentCode = a.DepartmentCode,
                                                 CapitalCode = a.CapitalCode,
                                                 DebitAcc = b.DebitAcc != null ? b.DebitAcc : "",
                                                 CreditAcc = b.CreditAcc != null ? b.CreditAcc : "",
                                                 PartnerCode = b.PartnerCode != null ? b.PartnerCode : "",
                                                 WorkPlaceCode = b.WorkPlaceCode != null ? b.WorkPlaceCode : "",
                                                 FProductWorkCode = b.FProductWorkCode != null ? b.FProductWorkCode : "",
                                                 SectionCode = b.SectionCode != null ? b.SectionCode : "",
                                                 CaseCode = b.CaseCode != null ? b.CaseCode : "",
                                                 DepreciationAmount = a.DepreciationAmount,
                                                 DepreciationUpAmount = a.DepreciationAmount > 0 ? a.DepreciationAmount : 0,
                                                 DepreciationDownAmount = a.DepreciationAmount < 0 ? a.DepreciationAmount : 0,
                                                 Edit = "K",
                                                 DepreciationEdit = "K",
                                                 Note = ""
                                             };
                var dataAssetToolDepreciations = assetToolDepreciations.Select(p => ObjectMapper.Map<CrudAssetToolDepreciationDto, AssetToolDepreciation>(p)).ToList();
                await _assetToolDepreciationService.CreateManyAsync(dataAssetToolDepreciations, true);
                foreach (var itd in dataAssetToolDepreciations)
                {
                    lstAssetToolDepreciation.Add(itd);
                }
            }
            await CreateVoucherAssetTool(dto);
            res.Ok = true;
            return res;
        }

        public async Task<ResultDto> CreateVoucherAssetTool(AssetToolAllocationDto dto)
        {
            var res = new ResultDto();
            var description = "";
            var voucherCode = "";
            if (dto.AssetOrTool == AssetToolConst.Asset)
            {
                description = "Khấu hao tài sản";
                voucherCode = "PTS";
            }
            else
            {
                description = "Phân bổ công cụ - dụng cụ";
                voucherCode = "PCC";
            }
            var curencyCode = await _tenantSettingService.GetValue("M_MA_NT0", _webHelper.GetCurrentOrgUnit());
            DateTime fromDate = dto.FromDate;
            DateTime toDate = dto.ToDate;
            var dataDate = new List<AssetToolAllocationDto>();
            dataDate.Add(new AssetToolAllocationDto
            {
                FromDate = Convert.ToDateTime(fromDate.Year + "-" + ((fromDate.Month < 10) ? "0" + fromDate.Month : fromDate.Month) + "-01"),
                ToDate = await LastDay(fromDate)
            });
            while (fromDate < toDate)
            {
                fromDate = await NextMonth(fromDate);
                if (fromDate < toDate)
                {
                    dataDate.Add(new AssetToolAllocationDto
                    {
                        FromDate = fromDate,
                        ToDate = await LastDay(fromDate)
                    });
                }
            }
            var dataAccVoucher = new List<CrudAccVoucherDto>();
            foreach (var item in dataDate)
            {
                var year = item.FromDate.Year;
                var month = item.FromDate.Month;
                // Xóa dữ liệu đã tính
                var ledgerDel = await _ledgerService.GetQueryableAsync();
                ledgerDel = ledgerDel.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == year
                                              && p.VoucherCode == voucherCode
                                              && p.VoucherDate.Month == month);
                await _ledgerService.DeleteManyAsync(ledgerDel);
                var accVoucherDel = await _accVoucherService.GetQueryableAsync();
                accVoucherDel = accVoucherDel.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == year
                                              && p.VoucherCode == voucherCode
                                              && p.VoucherDate.Month == month);
                var accVoucherDetailDel = await _accVoucherDetailService.GetQueryableAsync();
                accVoucherDetailDel = from a in accVoucherDel
                                      join b in accVoucherDetailDel on a.Id equals b.AccVoucherId
                                      select b;
                await _accVoucherDetailService.DeleteManyAsync(accVoucherDetailDel);
                await _accVoucherService.DeleteManyAsync(accVoucherDel);
                // Lấy dữ liệu chi tiết khấu hao ts
                var assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
                assetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == year
                                              && p.AssetOrTool == dto.AssetOrTool
                                              && p.Month == month
                                              && p.DebitAcc != null && p.DebitAcc != ""
                                              && p.CreditAcc != null && p.CreditAcc != "");
                var accPartner = await _accPartnerService.GetQueryableAsync();
                accPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                var dataAssetToolDepreciation = from a in assetToolDepreciation
                                                join b in accPartner on a.PartnerCode equals b.Code into ajb
                                                from b in ajb.DefaultIfEmpty()
                                                group new { a, b } by new
                                                {
                                                    a.PartnerCode,
                                                    a.DepartmentCode,
                                                    a.DebitAcc,
                                                    a.CreditAcc,
                                                    a.WorkPlaceCode,
                                                    a.SectionCode,
                                                    a.FProductWorkCode,
                                                    a.CaseCode,
                                                } into gr
                                                select new
                                                {
                                                    DepartmentCode = gr.Key.DepartmentCode ?? "",
                                                    DebitAcc = gr.Key.DebitAcc,
                                                    CreditAcc = gr.Key.CreditAcc,
                                                    PartnerCode = gr.Key.PartnerCode,
                                                    PartnerName = gr.Max(p => p.b != null ? p.b.Name : ""),
                                                    WorkPlaceCode = gr.Key.WorkPlaceCode,
                                                    SectionCode = gr.Key.SectionCode,
                                                    FProductWorkCode = gr.Key.FProductWorkCode,
                                                    CaseCode = gr.Key.CaseCode,
                                                    Amount = gr.Sum(p => p.a.DepreciationAmount),
                                                };
                var dataAssetToolDepreciationGroup = from a in dataAssetToolDepreciation
                                                     group new { a } by new { a.DepartmentCode } into gr
                                                     select new
                                                     {
                                                         TotalAmount = gr.Sum(p => p.a.Amount),
                                                         DepartmentCode = gr.Key.DepartmentCode
                                                     };
                var lstDataAssetToolDepreciationGroup = dataAssetToolDepreciationGroup.ToList();
                foreach (var itemDataAssetToolDepreciation in lstDataAssetToolDepreciationGroup)
                {
                    var accVoucherId = GetNewObjectId();
                    var accVoucher = new CrudAccVoucherDto
                    {
                        Id = accVoucherId,
                        OrgCode = _webHelper.GetCurrentOrgUnit(),
                        Year = year,
                        DepartmentCode = itemDataAssetToolDepreciation.DepartmentCode,
                        VoucherCode = voucherCode,
                        VoucherGroup = 3,
                        VoucherDate = item.ToDate,
                        Description = description + " Tháng " + ((month < 10) ? "0" + month : month),
                        CurrencyCode = curencyCode,
                        ExchangeRate = 1,
                        TotalAmountWithoutVat = itemDataAssetToolDepreciation.TotalAmount ?? 0,
                        TotalAmountCur = 0,
                        TotalAmount = itemDataAssetToolDepreciation.TotalAmount ?? 0,
                        Status = "1"
                    };
                    var dataAccVoucherDetail = from a in dataAssetToolDepreciation
                                               group new { a } by new
                                               {
                                                   a.PartnerCode,
                                                   a.DebitAcc,
                                                   a.CreditAcc,
                                                   a.WorkPlaceCode,
                                                   a.SectionCode,
                                                   a.FProductWorkCode,
                                                   a.CaseCode,
                                                   a.PartnerName
                                               } into gr
                                               select new CrudAccVoucherDetailDto
                                               {
                                                   Id = GetNewObjectId(),
                                                   AccVoucherId = accVoucherId,
                                                   OrgCode = _webHelper.GetCurrentOrgUnit(),
                                                   Year = year,
                                                   DebitAcc = gr.Key.DebitAcc,
                                                   CreditAcc = gr.Key.CreditAcc,
                                                   PartnerCode = gr.Key.PartnerCode,
                                                   PartnerName = gr.Key.PartnerName,
                                                   WorkPlaceCode = gr.Key.WorkPlaceCode,
                                                   SectionCode = gr.Key.SectionCode,
                                                   FProductWorkCode = gr.Key.FProductWorkCode,
                                                   CaseCode = gr.Key.CaseCode,
                                                   Note = description,
                                                   Amount = gr.Sum(p => p.a.Amount),
                                                   AmountCur = 0
                                               };
                    var crudAccvoucherDetailDto = dataAccVoucherDetail.ToList();
                    accVoucher.AccVoucherDetails = crudAccvoucherDetailDto;
                    dataAccVoucher.Add(accVoucher);
                }
            }
            await _accVoucherAppService.CreateListAsync(dataAccVoucher);
            res.Ok = true;
            return res;
        }

        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            var partnes = await _assetToolService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue, 
                                            AssetToolConst.Asset);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name,
                DataId = p.Id
            }).ToList();
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var year = _webHelper.GetCurrentYear();
            var assetToolGroup = await _assetToolGroupService.GetQueryableAsync();
            var lstAssetToolGroup = assetToolGroup.Where(p => p.OrgCode == orgCode);
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var unitOfWork = _unitOfWorkManager.Begin();
            var lstImport = await _excelService.ImportFileToList<ExcelAssetToolDto>(bytes, dto.WindowId);
            lstImport = lstImport.Where(p => p.VoucherNumber != null && p.VoucherNumber != "" && p.Code != null).ToList();
            var lstAccountSystem = await _accountingCacheManager.GetAccountSystemsAsync(year);
            int i = 2;
            foreach (var item in lstImport)
            {
                item.CalculatingMethod = "1";
                var errorMessage = "";
                var debitAcc = lstAccountSystem.Where(p => p.AccCode == item.DepreciationDebitAcc).FirstOrDefault();
                var creditAcc = lstAccountSystem.Where(p => p.AccCode == item.DepreciationCreditAcc).FirstOrDefault();
                bool isExistPartner = await _accPartnerService.IsExistCode(orgCode, item.PartnerCode);
                var fProductWork = await _accountingCacheManager.GetFProductWorkByCodeAsync(item.FProductWorkCode, orgCode);
                bool isExistsSection = await _accSectionService.IsExistCode(orgCode, item.SectionCode);
                bool isExistsWorkPlace = await _workPlaceSevice.IsExistCode(orgCode, item.WorkPlaceCode);
                bool isExistsAccCase = await _accCaseService.IsExistCode(orgCode, item.CaseCode);
                bool isExistsAssetTool = await _assetToolService.IsExistCode(orgCode, item.Code);
                bool isExistsAssetToolGroup = await _assetToolGroupService.IsExistCode(orgCode, item.AssetGroupCode);
                bool isExistsPurpose = await _purposeService.IsExistCode(orgCode, item.PurposeCode);
                bool isExistsDepartment = await _departmentService.IsExistCode(orgCode, item.DepartmentCode);
                bool isExistsCapital = await _capitalService.IsExistCode(orgCode, item.CapitalCode);
                var reason = (await _reasonService.GetQueryableAsync()).Where(p => p.OrgCode == orgCode && p.Code == item.UpDownCode).FirstOrDefault();
                bool isExistsReason = reason == null ? false : true;
                if (((item.AssetGroupCode ?? "") != "" && isExistsAssetToolGroup == false) || (item.AssetGroupCode ?? "") == "")
                {
                    errorMessage = $"Không có hoặc chưa nhập mã nhóm!";
                }
                else if ((item.Code ?? "") != "" && isExistsAssetTool == true)
                {
                    errorMessage = $"Mã tài sản {item.Code} đã tồn tại!";
                }
                else if ((item.Code ?? "") == "")
                {
                    errorMessage = $"Không có mã tài sản!";
                }
                else if ((item.PurposeCode ?? "") != "" && isExistsPurpose == false)
                {
                    errorMessage = $"Không tồn tại mã mục đích sử dụng!";
                }
                else if (((item.UpDownCode ?? "") != "" && isExistsReason == false) || (item.UpDownCode ?? "") == "")
                {
                    errorMessage = $"Không có hoặc không tồn tại mã tăng giảm!";
                }
                else if (((item.DepartmentCode ?? "") != "" && isExistsDepartment == false) || (item.DepartmentCode ?? "") == "")
                {
                    errorMessage = $"Không có hoặc không tồn tại mã bộ phận!";
                }
                else if (((item.CapitalCode ?? "") != "" && isExistsCapital == false) || (item.CapitalCode ?? "") == "")
                {
                    errorMessage = $"Không có hoặc không tồn tại mã nguồn vốn!";
                }
                else if (((item.DepreciationDebitAcc ?? "") != "" && debitAcc == null))
                {
                    errorMessage = $"Không có mã tài khoản KH/PB bên nợ!";
                }
                else if (((item.DepreciationCreditAcc ?? "") != "" && creditAcc == null))
                {
                    errorMessage = $"Không có mã tài khoản KH/PB bên có!";
                }
                else if (((item.PartnerCode ?? "") != "" && isExistPartner == false))
                {
                    errorMessage = $"Không có mã đối tượng KH/PB!";
                }
                else if (((item.WorkPlaceCode ?? "") != "" && isExistsWorkPlace == false))
                {
                    errorMessage = $"Không có mã phân xưởng KH/PB!";
                }
                else if (((item.FProductWorkCode ?? "") != "" && fProductWork == null))
                {
                    errorMessage = $"Không có mã công trình, sản phẩm KH/PB!";
                }
                else if (((item.SectionCode ?? "") != "" && isExistsSection == false))
                {
                    errorMessage = $"Không có mã khoản mục KH/PB!";
                }
                else if (((item.CaseCode ?? "") != "" && isExistsAccCase == false))
                {
                    errorMessage = $"Không có mã vụ việc KH/PB!";
                }
                else if (item.OriginalPrice - item.Impoverishment <= 0)
                {
                    errorMessage = $"Giá trị còn lại cần KH/PB phải > 0";
                }

                if (errorMessage != "")
                {
                    throw new Exception(errorMessage + " Lỗi tại dòng " + i);
                }
                i++;
                if (item.UpDownCode == (reason?.Code ?? "") && reason != null)
                {
                    item.Remaining = item.OriginalPrice - item.Impoverishment;
                    item.ReasonType = reason.ReasonType;
                }

            }
            var assetTool = (from a in lstImport
                             join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code into ajb
                             from b in ajb.DefaultIfEmpty()
                            group new { a, b } by new
                            {
                                a.AssetOrTool,
                                a.Code
                            } into gr
                            select new CrudAssetToolDto
                            {
                                Id = GetNewObjectId(),
                                AssetOrTool = gr.Key.AssetOrTool,
                                OrgCode = orgCode,
                                Year = year,
                                AssetGroupCode = gr.Max(p => p.a.AssetGroupCode),
                                AssetToolGroupId = gr.Max(p => p.b?.Id ?? null),
                                Code = gr.Key.Code,
                                AssetToolCard = gr.Max(p => p.a.AssetToolCard),
                                Name = gr.Max(p => p.a.Name),
                                UnitCode = gr.Max(p => p.a.UnitCode),
                                Country = gr.Max(p => p.a.Country),
                                ProductionYear = gr.Max(p => p.a.ProductionYear),
                                Wattage = gr.Max(p => p.a.Wattage),
                                Quantity = gr.Max(p => p.a.Quantity),
                                PurposeCode = gr.Max(p => p.a.PurposeCode),
                                DepreciationType = gr.Max(p => p.a.DepreciationType),
                                Note = gr.Max(p => p.a.Note),
                                CalculatingMethod = gr.Max(p => p.a.CalculatingMethod),
                                FollowDepreciation = gr.Max(p => p.a.FollowDepreciation),
                                OriginalPrice = gr.Sum(p => p.a.OriginalPrice),
                                Impoverishment = gr.Sum(p => p.a.Impoverishment),
                                Remaining = gr.Sum(p => p.a.Remaining),
                                DepreciationAmount0 = gr.Sum(p => p.a.CalculatingAmount),
                                DepreciationAmount = gr.Sum(p => p.a.DepreciationAmount),
                                DepreciationDebitAcc = gr.Max(p => p.a.DepreciationDebitAcc),
                                DepreciationCreditAcc = gr.Max(p => p.a.DepreciationCreditAcc),
                                PartnerCode = gr.Max(p => p.a.PartnerCode),
                                WorkPlaceCode = gr.Max(p => p.a.WorkPlaceCode),
                                FProductWorkCode = gr.Max(p => p.a.FProductWorkCode),
                                SectionCode = gr.Max(p => p.a.SectionCode),
                                CaseCode = gr.Max(p => p.a.CaseCode),
                            }).ToList();
            var assetToolEntity = assetTool.Select(p => ObjectMapper.Map<CrudAssetToolDto, AssetTool>(p)).ToList();
            var assetToolDetails = new List<AssetToolDetail>();
            foreach (var item in assetTool)
            {
                var lstDetail = lstImport.Where(p => p.Code == item.Code).ToList();
                int ord = 1;
                var assetToolDetail = from a in lstDetail
                                      join b in assetTool on a.Code equals b.Code
                                      select new CrudAssetToolDetailDto
                                      {
                                          Id = GetNewObjectId(),
                                          AssetOrTool = a.AssetOrTool,
                                          AssetToolId = b.Id,
                                          OrgCode = orgCode,
                                          Ord0 = "A" + ord++.ToString().PadLeft(9, '0'),
                                          Year = year,
                                          VoucherNumber = a.VoucherNumber,
                                          VoucherDate = a.VoucherDate,
                                          UpDownCode = a.UpDownCode,
                                          Number = a.Number,
                                          UpDownDate = a.UpDownDate,
                                          DepartmentCode = a.DepartmentCode,
                                          CapitalCode = a.CapitalCode,
                                          OriginalPrice = a.OriginalPrice,
                                          Impoverishment = a.Impoverishment,
                                          MonthNumber0 = a.MonthNumber0,
                                          IsCalculating = a.IsCalculating,
                                          Remaining = a.Remaining,
                                          BeginDate = a.BeginDate,
                                          CalculatingAmount = a.CalculatingAmount,
                                          MonthNumber = a.MonthNumber,
                                          DepreciationAmount = a.DepreciationAmount,
                                          Note = a.NoteDetail
                                      };
                assetToolDetails.AddRange(assetToolDetail.Select(p => ObjectMapper.Map<CrudAssetToolDetailDto, AssetToolDetail>(p)).ToList());
            }
            await _assetToolService.CreateManyAsync(assetToolEntity);
            await _assetToolDetailService.CreateManyAsync(assetToolDetails);
            var month = new List<MonthDto>();
            month.Add(new MonthDto {Month = 1, LastDate = DateTime.Parse(year + "-01-31")});
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 100 == 0))
            {
                month.Add(new MonthDto { Month = 2, LastDate = DateTime.Parse(year + "-02-29") });
            }
            else
            {
                month.Add(new MonthDto { Month = 2, LastDate = DateTime.Parse(year + "-02-28") });
            }
            month.Add(new MonthDto {Month = 3, LastDate = DateTime.Parse(year + "-03-31") });
            month.Add(new MonthDto {Month = 4, LastDate = DateTime.Parse(year + "-04-30") });
            month.Add(new MonthDto {Month = 5, LastDate = DateTime.Parse(year + "-05-31") });
            month.Add(new MonthDto {Month = 6, LastDate = DateTime.Parse(year + "-06-30") });
            month.Add(new MonthDto {Month = 7, LastDate = DateTime.Parse(year + "-07-31") });
            month.Add(new MonthDto {Month = 8, LastDate = DateTime.Parse(year + "-08-31") });
            month.Add(new MonthDto {Month = 9, LastDate = DateTime.Parse(year + "-09-30") });
            month.Add(new MonthDto {Month = 10, LastDate = DateTime.Parse(year + "-10-31") });
            month.Add(new MonthDto {Month = 11, LastDate = DateTime.Parse(year + "-11-30") });
            month.Add(new MonthDto {Month = 12, LastDate = DateTime.Parse(year + "-12-31") });

            foreach (var item in month)
            {
                var assetToolAccount = from a in assetTool
                                       where (a.DepreciationDebitAcc ?? "") != null || (a.DepreciationCreditAcc ?? "") != null
                                       select new CrudAssetToolAccountDto
                                       {
                                           Id = GetNewObjectId(),
                                           AssetOrTool = a.AssetOrTool,
                                           AssetToolId = a.Id,
                                           OrgCode = orgCode,
                                           Ord0 = "A" + item.Month.ToString().PadLeft(9, '0'),
                                           Year = year,
                                           Month = item.Month,
                                           DepreciationDate = item.LastDate,
                                           DebitAcc = a.DepreciationDebitAcc,
                                           CreditAcc = a.DepreciationCreditAcc,
                                           PartnerCode = a.PartnerCode,
                                           WorkPlaceCode = a.WorkPlaceCode,
                                           FProductWorkCode = a.FProductWorkCode,
                                           SectionCode = a.SectionCode,
                                           CaseCode = a.CaseCode
                                       };
                var assetToolAccountEntity = assetToolAccount.Select(p => ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(p)).ToList();
                await _assetToolAccountService.CreateManyAsync(assetToolAccountEntity);
            }
            await unitOfWork.CompleteAsync();
            return new UploadFileResponseDto() { Ok = true };
        }
        #region Private
        private async Task<IQueryable<AssetTool>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                          && p.AssetOrTool == AssetToolConst.Asset);
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("assetToolGroupId"))
                {
                    string value = item.Value.ToString();
                    queryable = queryable.Where(p => p.AssetToolGroupId == value);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }

        private async Task<DateTime> NextMonth(DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            month++;
            if (month == 13)
            {
                month = 1;
                year++;
            }
            return Convert.ToDateTime(year + "-" + ((month < 10) ? "0" + month : month) + "-01");
        }

        private async Task<DateTime> LastDay(DateTime date)
        {
            var month = date.Month;
            var day = date.Day;
            if (month == 2)
            {
                if (date.Year % 100 != 0 && date.Year % 4 == 0)
                {
                    day = 29;
                }
                else
                {
                    day = 28;
                }
            }
            else if ("135781012".Contains(month.ToString()))
            {
                day = 31;
            }
            else
            {
                day = 30;
            }
            return Convert.ToDateTime(date.Year + "-" + ((month < 10) ? "0" + month : month) + "-" + ((day < 10) ? "0" + day : day));
        }

        private decimal Abs(decimal? item)
        {
            if (item < 0)
            {
                return item * -1 ?? 0;
            }
            else
            {
                return item ?? 0;
            }
        }

        #endregion
    }
}
