using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.CategoryDeletes;
using Accounting.Categories.OrgUnits;
using Accounting.Catgories.CategoryDatas;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.VoucherCategories;
using Accounting.Catgories.VoucherCategoryDeletes;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Accounting.Categories.DeleteDatas
{
    public class DeleteDataAppService : AccountingAppService
    {
        #region Fields
        private readonly OrgUnitService _orgUnitService;
        private readonly UserService _userService;
        private readonly CategoryDeleteService _categoryDeleteService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly CreateAccVoucherBusiness _createAccVoucherBusiness;
        private readonly CreateProductVoucherBusiness _createProductVoucherBusiness;
        private readonly AccountSystemService _accountSystemService;
        private readonly DefaultAccountSystemService _defaultAccountSystemService;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly AccountingDb _accountingDb;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        private readonly WebHelper _webHelper;

        #endregion
        #region Ctor
        public DeleteDataAppService(OrgUnitService orgUnitService,
                            UserService userService,
                            CategoryDeleteService categoryDeleteService,
                            AccVoucherService accVoucherService,
                            ProductVoucherService productVoucherService,
                            FProductWorkService fProductWorkService,
                            YearCategoryService yearCategoryService,
                            VoucherCategoryService voucherCategoryService,
                            CreateAccVoucherBusiness createAccVoucherBusiness,
                            CreateProductVoucherBusiness createProductVoucherBusiness,
                            AccountSystemService accountSystemService,
                            DefaultAccountSystemService defaultAccountSystemService,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            LicenseBusiness licenseBusiness,
                            AccountingDb accountingDb,
                            AccountingCacheManager accountingCacheManager,
                            TenantExtendInfoService tenantExtendInfoService,
                            ICurrentTenant currentTenant,
                            WebHelper webHelper
                        )
        {
            _orgUnitService = orgUnitService;
            _userService = userService;
            _categoryDeleteService = categoryDeleteService;
            _yearCategoryService = yearCategoryService;
            _accVoucherService = accVoucherService;
            _productVoucherService = productVoucherService;
            _fProductWorkService = fProductWorkService;
            _voucherCategoryService = voucherCategoryService;
            _createAccVoucherBusiness = createAccVoucherBusiness;
            _createProductVoucherBusiness = createProductVoucherBusiness;
            _accountSystemService = accountSystemService;
            _defaultAccountSystemService = defaultAccountSystemService;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _licenseBusiness = licenseBusiness;
            _accountingDb = accountingDb;
            _accountingCacheManager = accountingCacheManager;
            _tenantExtendInfoService = tenantExtendInfoService;
            _currentTenant = currentTenant;
            _webHelper = webHelper;
        }
        #endregion
        #region Methods
        public async Task<List<VoucherCategoryDeleteDto>> GetVoucherCategoryDataAsync()
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var tenantType = await this.GetTenantType();
            var voucherCategory = await this.GetVoucherCategory(tenantType);
            var res = voucherCategory.Select(p => new VoucherCategoryDeleteDto 
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        VoucherKind = p.VoucherKind
                    }).ToList();
            return res;
        }

        public async Task<List<CategoryDataDto>> GetCategoryDataAsync()
        {
            var categoryData = await _categoryDeleteService.GetQueryableAsync();
            var tenantType = await this.GetTenantType();
            if (tenantType != 2)
            {
                var res = categoryData.Where(p => p.BusinessType == "DN").Select(p => ObjectMapper.Map<CategoryDelete, CategoryDataDto>(p)).ToList();
                return res;
            }
            else
            {
                var res = categoryData.Where(p => p.BusinessType == "HKD").Select(p => ObjectMapper.Map<CategoryDelete, CategoryDataDto>(p)).ToList();
                return res;
            }
        }

        public async Task<ResultDto> PostDeleteAsync(DeleteDataDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.LstVoucherCategories.Count > 0)
            {
                foreach (var item in dto.LstVoucherCategories)
                {
                    if (item.VoucherKind == "KT")
                    {
                        var accVoucher = await _accVoucherService.GetQueryableAsync();
                        accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() 
                                                        && p.VoucherCode == item.Code
                                                        && p.VoucherDate >= dto.FromDate
                                                        && p.VoucherDate <= dto.ToDate
                                                        );
                        var lstDelete = accVoucher.ToList();
                        foreach (var itemDelete in lstDelete)
                        {
                            await _createAccVoucherBusiness.DeleteAccVoucherAsync(itemDelete.Id);
                        }
                    }
                    else
                    {
                        var productVoucher = await _productVoucherService.GetQueryableAsync();
                        productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.VoucherCode == item.Code
                                                        && p.VoucherDate >= dto.FromDate
                                                        && p.VoucherDate <= dto.ToDate
                                                        );
                        var lstDelete = productVoucher.ToList();
                        foreach (var itemDelete in lstDelete)
                        {
                            await _createProductVoucherBusiness.DeleteProductVoucherAsync(itemDelete.Id);
                        }
                    }
                }
            }

            if (dto.LstCategories.Count > 0)
            {
                dto.LstCategories = dto.LstCategories.OrderByDescending(p => p.Type).ToList();
                foreach (var item in dto.LstCategories)
                {
                    switch (item.Type)
                    {
                        case 1: // (1. xét điều kiện không có phát sinh mới xóa)
                            var dict = new Dictionary<string, object>();
                            string sqlCheckData = $"select \"{item.FieldCode}\" from \"{item.TabName}\" WHERE \"OrgCode\"=@orgCode";
                            dict.Add("orgCode", _webHelper.GetCurrentOrgUnit());
                            var data = await _accountingDb.GetDataTableAsync(sqlCheckData, dict);
                            var dataRow = data.Rows;
                            for (int i = 0; i < dataRow.Count; i++)
                            {
                                string isUsing = await _linkCodeBusiness.IsCodeUsingString(item.RefFieldCode, dataRow[i][0].ToString(), _webHelper.GetCurrentOrgUnit());
                                if (isUsing != "")
                                {
                                    throw new AccountingException($"Dữ liệu của {item.Name} không thể xóa vì: {isUsing}");
                                }
                            }
                            string sqlDelete = $"delete from \"{item.TabName}\" WHERE \"OrgCode\"=@orgCode";
                            await _accountingDb.ExecuteSQLAsync(sqlDelete, dict);
                            break;
                        case 2: // (2. xóa theo năm không cần điều kiện)
                            var dict2 = new Dictionary<string, object>();
                            string sqlDeleteData2 = $"delete from \"{item.TabName}\" WHERE \"OrgCode\"=@orgCode and \"Year\"=@year";
                            dict2.Add("orgCode", _webHelper.GetCurrentOrgUnit());
                            dict2.Add("year", _webHelper.GetCurrentYear());
                            await _accountingDb.ExecuteSQLAsync(sqlDeleteData2, dict2);
                            break;
                        case 3: // (3. xóa theo điều kiện ConditionField = ConditionValue)
                            var dict3 = new Dictionary<string, object>();
                            string sqlDeleteData3 = $"delete from \"{item.TabName}\" WHERE \"{item.ConditionField}\" = @conditionValue and \"OrgCode\"=@orgCode and \"Year\"=@year";
                            dict3.Add("conditionValue", item.ConditionValue);
                            dict3.Add("orgCode", _webHelper.GetCurrentOrgUnit());
                            dict3.Add("year", _webHelper.GetCurrentYear());
                            await _accountingDb.ExecuteSQLAsync(sqlDeleteData3, dict3);
                            break;
                        case 4: // (4. dở dang đầu kỳ)
                            var productVoucher = await _productVoucherService.GetQueryableAsync();
                            var fProductWork = await _fProductWorkService.GetQueryableAsync();
                            fProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                            var dataDelete = productVoucher.Where(p => p.ProductVoucherDetails.Any(d => fProductWork.Any(f => f.FProductOrWork == item.ConditionValue && f.Code == d.FProductWorkCode))).ToList();
                            foreach (var itemDelete in dataDelete)
                            {
                                await _createProductVoucherBusiness.DeleteProductVoucherAsync(itemDelete.Id);
                            }
                            break;
                        case 5: // (5. danh mục tài khoản)
                            var year = _webHelper.GetCurrentYear();
                            var orgCode = _webHelper.GetCurrentOrgUnit();
                            var accountSystem = await _accountSystemService.GetQueryableAsync();
                            accountSystem = accountSystem.Where(p => p.OrgCode == orgCode && p.Year == year);
                            var lstAcc = string.Join(",", accountSystem.Select(p => p.AccCode).ToList());
                            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync("AccCode");
                            if (linkCodes.Count == 0) break;
                            var dictAcc = new Dictionary<string, object>();
                            foreach (var itemLinkCode in linkCodes)
                            {
                                string sql = "";
                                if (dictAcc.Count > 0) dictAcc.Clear();
                                if (!"BusinessCategory,Product,TaxCategory".Contains(itemLinkCode.RefTableName))
                                {
                                    sql = $"select \"{itemLinkCode.RefFieldCode}\" from \"{itemLinkCode.RefTableName}\" "
                                        + $"where strpos(\'@lstAcc\', \"{itemLinkCode.RefFieldCode}\") > 0 and \"OrgCode\"=@orgCode and \"Year\"=@year";
                                    dictAcc.Add("lstAcc", lstAcc);
                                    dictAcc.Add("orgCode", orgCode);
                                    dictAcc.Add("year", year);
                                }
                                else
                                {
                                    sql = $"select \"{itemLinkCode.RefFieldCode}\" from \"{itemLinkCode.RefTableName}\" "
                                        + $"where strpos(\'@lstAcc\', \"{itemLinkCode.RefFieldCode}\") > 0 and \"OrgCode\"=@orgCode";
                                    dictAcc.Add("lstAcc", lstAcc);
                                    dictAcc.Add("orgCode", orgCode);
                                }
                                var dataTable = await _accountingDb.GetDataTableAsync(sql, dictAcc);
                                if (dataTable.Rows.Count > 0)
                                throw new Exception($"Đã tồn tại tài khoản có phát sinh ở bảng {itemLinkCode.RefTableName}, không thể xóa danh mục tài khoản ở năm {year}");
                            }
                            await _accountSystemService.DeleteManyAsync(accountSystem,true);
                            break;
                        default:
                            break;
                    }
                }
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện xong!";
            return res;
        }
        #endregion
        #region Private Methods
        private async Task<int?> GetTenantType()
        {
            var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantExtendInfo == null) return null;
            return tenantExtendInfo.TenantType;
        }

        private async Task<List<VoucherCategory>> GetVoucherCategory(int? type)
        {
            var voucherCategory = await _voucherCategoryService.GetByVoucherCategoryAsync(_webHelper.GetCurrentOrgUnit());
            if (type == 2)
            {
                return voucherCategory.Where(p => p.TenantType == 2).ToList();
            }
            else
            {
                return voucherCategory.Where(p => p.TenantType == 1).ToList();
            }
        }
        #endregion
    }
}
