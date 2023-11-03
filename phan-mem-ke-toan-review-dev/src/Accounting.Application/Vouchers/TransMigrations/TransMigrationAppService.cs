using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts.AccOpeningBalances;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Careers;
using Accounting.Catgories.ProductOpeningBalances;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
using Accounting.Helpers;
using Accounting.Reports;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.ResetVoucherNumbers;
using Accounting.Vouchers.TransMigrations;
using Accounting.Vouchers.VoucherNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accounting.Vouchers.VoucherNumbers
{
    public class TransMigrationAppService : AccountingAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ProductService _productService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly LinkCodeService _linkCodeService;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly AccountingDb _accountingDb;
        private readonly ITenantRepository _tenantRepository;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        #endregion
        #region Ctor
        public TransMigrationAppService(VoucherNumberService voucherNumberService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            ProductVoucherService productVoucherService,
                            AccVoucherService accVoucherService,
                            IUnitOfWorkManager unitOfWorkManager,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ProductService productService,
                            FProductWorkService fProductWorkService,
                            LinkCodeService linkCodeService,
                            AccountSystemService accountSystemService,
                            AccOpeningBalanceService accOpeningBalanceService,
                            AccountingDb accountingDb,
                            ITenantRepository tenantRepository,
                            ProductOpeningBalanceService productOpeningBalanceService
                            )
        {
            _voucherNumberService = voucherNumberService;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _productService = productService;
            _fProductWorkService = fProductWorkService;
            _linkCodeService = linkCodeService;
            _accountSystemService = _accountSystemService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _accountingDb = accountingDb;
            _tenantRepository = tenantRepository;
            _productOpeningBalanceService = productOpeningBalanceService;
        }
        #endregion
        public async Task<ResultDto> TransAsync(TransMigrationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            voucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstdata = voucherCategory.ToList();
            var lstDataNew = new List<VoucherCategory>();
            DateTime bookClosingDate;
            ResultDto resultDto = new ResultDto();
            if (dto.IsCheck == 1)
            {
                var lstvoucherCategory = voucherCategory.Where(p => !string.IsNullOrEmpty(p.BookClosingDate.ToString())).ToList();
                if (lstvoucherCategory.Count > 0)
                {
                    bookClosingDate = (DateTime)voucherCategory.OrderBy(p => p.BookClosingDate).Where(p => p.BookClosingDate.ToString() != null).Select(p => p.BookClosingDate).FirstOrDefault();
                    if (dto.FieldCode != "AccCode" || bookClosingDate.Year >= _webHelper.GetCurrentYear())
                    {

                        resultDto.Ok = false;
                        resultDto.Message = "Hệ thống chứng từ đã có thời gian khoá từ ngày : " + bookClosingDate.ToString();
                        return resultDto;
                    }
                }
            }

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            int productT = lstProduct.Where(p => p.Code == dto.FieldCode && p.ProductType == "T").Count();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstFProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var fProductWorkS = fProductWork.Where(p => p.Code == dto.FieldCode && p.FProductOrWork == "S").ToList();
            if ((dto.FieldCode == "ProductCode" && productT > 0) || (dto.FieldCode == "FProductWorkCode"))
            {
                if (dto.FieldCode == "FProductWorkCode")
                {
                    dto.FieldCode = "FProductWorkCode";
                    resultDto = await Trans0Async(dto);
                }
                if (dto.FieldCode == "ProductCode")
                {
                    dto.FieldCode = "ProductCode";
                    resultDto = await Trans0Async(dto);
                }
            }

            resultDto = await Trans0Async(dto);

            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucer = await _productVoucherService.GetQueryableAsync();
            productVoucer = productVoucer.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());



            return resultDto;
        }
        public async Task<ResultDto> Trans0Async(TransMigrationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            voucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstdata = voucherCategory.ToList();
            var lstDataNew = new List<VoucherCategory>();
            DateTime bookClosingDate;
            ResultDto resultDto = new ResultDto();
            if (dto.IsCheck == 1)
            {
                var lstvoucherCategory = voucherCategory.Where(p => !string.IsNullOrEmpty(p.BookClosingDate.ToString())).ToList();
                if (lstvoucherCategory.Count > 0)
                {
                    bookClosingDate = (DateTime)voucherCategory.OrderBy(p => p.BookClosingDate).Where(p => p.BookClosingDate.ToString() != null).Select(p => p.BookClosingDate).FirstOrDefault();
                    if (dto.FieldCode != "AccCode" || bookClosingDate.Year >= _webHelper.GetCurrentYear())
                    {

                        resultDto.Ok = false;
                        resultDto.Message = "Hệ thống chứng từ đã có thời gian khoá từ ngày : " + bookClosingDate.ToString();
                        return resultDto;
                    }
                }
            }

            var linkcodes = await _linkCodeService.GetQueryableAsync();
            var lstLinkcodes = linkcodes.Where(p => p.FieldCode == dto.FieldCode).OrderBy(p => p.RefTableName).ToList();
            foreach (var item in lstLinkcodes)
            {
                if (item.RefTableName == "AccOpeningBalance" || item.RefTableName == "ProductOpeningBalance")
                {
                    if (item.RefTableName == "AccOpeningBalance")
                    {
                        if (dto.FieldCode == "AccCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                                    && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.AccCode == dto.OldValue
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == item1.PartnerCode
                                                                    && p.ContractCode == item1.ContractCode
                                                                    && p.FProductWorkCode == item1.FProductWorkCode
                                                                    && p.AccSectionCode == item1.AccSectionCode
                                                                    && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                  ).ToList();

                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                                && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                && p.AccCode == dto.NewValue
                                                                && p.CurrencyCode == item1.CurrencyCode
                                                                && p.PartnerCode == item1.PartnerCode
                                                                && p.ContractCode == item1.ContractCode
                                                                && p.FProductWorkCode == item1.FProductWorkCode
                                                                && p.AccSectionCode == item1.AccSectionCode
                                                                && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);
                            }



                        }
                        if (dto.FieldCode == "PartnerCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.PartnerCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == dto.NewValue
                                                                    && p.ContractCode == item1.ContractCode
                                                                    && p.FProductWorkCode == item1.FProductWorkCode
                                                                    && p.AccSectionCode == item1.AccSectionCode
                                                                    && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                    ).ToList();
                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                             && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                             && p.AccCode == item1.AccCode
                                                             && p.CurrencyCode == item1.CurrencyCode
                                                             && p.PartnerCode == item1.PartnerCode
                                                             && p.ContractCode == item1.ContractCode
                                                             && p.FProductWorkCode == item1.FProductWorkCode
                                                             && p.AccSectionCode == item1.AccSectionCode
                                                             && p.WorkPlaceCode == item1.WorkPlaceCode
                                                             ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);
                            }


                        }
                        if (dto.FieldCode == "ContractCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.ContractCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == item1.PartnerCode
                                                                    && p.ContractCode == dto.NewValue
                                                                    && p.FProductWorkCode == item1.FProductWorkCode
                                                                    && p.AccSectionCode == item1.AccSectionCode
                                                                    && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                    ).ToList();
                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                         && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                         && p.AccCode == item1.AccCode
                                                         && p.CurrencyCode == item1.CurrencyCode
                                                         && p.PartnerCode == item1.PartnerCode
                                                         && p.ContractCode == item1.ContractCode
                                                         && p.FProductWorkCode == item1.FProductWorkCode
                                                         && p.AccSectionCode == item1.AccSectionCode
                                                         && p.WorkPlaceCode == item1.WorkPlaceCode
                                                         ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);

                            }
                        }
                        if (dto.FieldCode == "FProductWorkCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.FProductWorkCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == item1.PartnerCode
                                                                    && p.ContractCode == item1.ContractCode
                                                                    && p.FProductWorkCode == dto.NewValue
                                                                    && p.AccSectionCode == item1.AccSectionCode
                                                                    && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                    ).ToList();
                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                          && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.AccCode == item1.AccCode
                                                          && p.CurrencyCode == item1.CurrencyCode
                                                          && p.PartnerCode == item1.PartnerCode
                                                          && p.ContractCode == item1.ContractCode
                                                          && p.FProductWorkCode == item1.FProductWorkCode
                                                          && p.AccSectionCode == item1.AccSectionCode
                                                          && p.WorkPlaceCode == item1.WorkPlaceCode
                                                          ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);

                            }
                        }
                        if (dto.FieldCode == "SectionCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccSectionCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == item1.PartnerCode
                                                                    && p.ContractCode == item1.ContractCode
                                                                    && p.FProductWorkCode == item1.FProductWorkCode
                                                                    && p.AccSectionCode == dto.NewValue
                                                                    && p.WorkPlaceCode == item1.WorkPlaceCode
                                                                    ).ToList();
                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                             && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                             && p.AccCode == item1.AccCode
                                                             && p.CurrencyCode == item1.CurrencyCode
                                                             && p.PartnerCode == item1.PartnerCode
                                                             && p.ContractCode == item1.ContractCode
                                                             && p.FProductWorkCode == item1.FProductWorkCode
                                                             && p.AccSectionCode == item1.AccSectionCode
                                                             && p.WorkPlaceCode == item1.WorkPlaceCode
                                                             ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);

                            }
                        }
                        if (dto.FieldCode == "WorkPlaceCode")
                        {
                            var accSysstem = await _accOpeningBalanceService.GetQueryableAsync();
                            var lstAccSysstem = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.WorkPlaceCode == dto.OldValue).ToList();
                            foreach (var item1 in lstAccSysstem)
                            {
                                var accCheck = accSysstem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.CurrencyCode == item1.CurrencyCode
                                                                    && p.PartnerCode == item1.PartnerCode
                                                                    && p.ContractCode == item1.ContractCode
                                                                    && p.FProductWorkCode == item1.FProductWorkCode
                                                                    && p.AccSectionCode == item1.AccSectionCode
                                                                    && p.WorkPlaceCode == dto.NewValue
                                                                    ).ToList();
                                var update = accSysstem.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                             && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                             && p.AccCode == item1.AccCode
                                                             && p.CurrencyCode == item1.CurrencyCode
                                                             && p.PartnerCode == item1.PartnerCode
                                                             && p.ContractCode == item1.ContractCode
                                                             && p.FProductWorkCode == item1.FProductWorkCode
                                                             && p.AccSectionCode == item1.AccSectionCode
                                                             && p.WorkPlaceCode == item1.WorkPlaceCode
                                                             ).ToList();
                                if (update.Count > 0)
                                {
                                    foreach (var item2 in update)
                                    {
                                        item2.Debit = item1.Debit;
                                        item2.DebitCur = item1.DebitCur;
                                        item2.Credit = item1.Credit;
                                        item2.CreditCur = item1.CreditCur;
                                        await _accOpeningBalanceService.UpdateAsync(item2);
                                    }



                                }
                                else
                                {
                                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                                    crudAccOpeningBalanceDto.Debit = item1.Debit;
                                    crudAccOpeningBalanceDto.DebitCur = item1.DebitCur;
                                    crudAccOpeningBalanceDto.Credit = item1.Credit;
                                    crudAccOpeningBalanceDto.CreditCur = item1.CreditCur;
                                    crudAccOpeningBalanceDto.AccCode = dto.NewValue;
                                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                                    crudAccOpeningBalanceDto.Year = _webHelper.GetCurrentYear();
                                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                                    var accOpeningBalance = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                                    await _accOpeningBalanceService.CreateAsync(accOpeningBalance);
                                }

                                await _accOpeningBalanceService.DeleteAsync(item1);

                            }
                        }



                    }
                    else
                    {
                        if (dto.FieldCode == "WarehouseCode")
                        {
                            var productOpeningBalances = await _productOpeningBalanceService.GetQueryableAsync();
                            var openingBalances = productOpeningBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.WarehouseCode == dto.OldValue).ToList();
                            foreach (var item1 in openingBalances)
                            {
                                var accCheck = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == dto.NewValue
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode

                                                                    ).ToList();
                                if (accCheck.Count > 0)
                                {
                                    var resulAcc = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == dto.NewValue
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode
                                                                    ).FirstOrDefault();
                                    item1.Quantity += resulAcc.Quantity;
                                    item1.Amount += resulAcc.Amount;
                                    item1.AmountCur += resulAcc.AmountCur;

                                    var update = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.ProductOriginCode == item1.ProductOriginCode
                                                                    && p.WarehouseCode == dto.NewValue

                                                                    ).ToList();
                                    foreach (var item2 in update)
                                    {
                                        item2.Quantity = item1.Quantity;
                                        item2.Amount = item1.Amount;
                                        item2.AmountCur = item1.Amount;
                                        await _productOpeningBalanceService.UpdateAsync(item2);
                                    }

                                }
                                else
                                {

                                }

                            }
                        }
                        if (dto.FieldCode == "ProductCode")
                        {
                            var productOpeningBalances = await _productOpeningBalanceService.GetQueryableAsync();
                            var openingBalances = productOpeningBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductCode == dto.OldValue).ToList();
                            foreach (var item1 in openingBalances)
                            {
                                var accCheck = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == item1.Year
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == dto.OldValue
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.ProductOriginCode == item1.ProductOriginCode
                                                                    && p.AccCode == item1.AccCode

                                                                    ).ToList();
                                if (accCheck.Count > 0)
                                {
                                    var resulAcc = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()

                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == dto.NewValue
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.Year == item1.Year
                                                                     && p.ProductOriginCode == item1.ProductOriginCode
                                                                    ).FirstOrDefault();
                                    if (resulAcc != null)
                                    {
                                        item1.Quantity += resulAcc.Quantity;
                                        item1.Amount += resulAcc.Amount;
                                        item1.AmountCur += resulAcc.AmountCur;
                                    }

                                    await _productOpeningBalanceService.DeleteAsync(item1);
                                    var update = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()

                                                                    && p.AccCode == item1.AccCode
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == dto.NewValue
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.Year == item1.Year
                                                                     && p.ProductOriginCode == item1.ProductOriginCode
                                                                    ).ToList();
                                    if (update.Count > 0)
                                    {
                                        foreach (var item2 in update)
                                        {
                                            item2.Quantity = item1.Quantity;
                                            item2.Amount = item1.Amount;
                                            item2.AmountCur = item1.Amount;
                                            await _productOpeningBalanceService.UpdateAsync(item2);
                                        }
                                    }
                                    else
                                    {
                                        CrudProductOpeningBalanceDto crudProductOpeningBalanceDto = new CrudProductOpeningBalanceDto();
                                        crudProductOpeningBalanceDto.ProductCode = dto.NewValue;
                                        crudProductOpeningBalanceDto.Quantity = item1.Quantity;
                                        crudProductOpeningBalanceDto.Amount = item1.Amount;
                                        crudProductOpeningBalanceDto.AmountCur = item1.AmountCur;
                                        crudProductOpeningBalanceDto.WarehouseCode = item1.WarehouseCode;
                                        crudProductOpeningBalanceDto.Year = item1.Year;
                                        crudProductOpeningBalanceDto.Price = item1.Price;
                                        crudProductOpeningBalanceDto.PriceCur = item1.PriceCur;
                                        crudProductOpeningBalanceDto.ProductLotCode = item1.ProductLotCode;
                                        crudProductOpeningBalanceDto.ProductOriginCode = item1.ProductOriginCode;
                                        crudProductOpeningBalanceDto.OrgCode = item1.OrgCode;
                                        crudProductOpeningBalanceDto.Ord0 = item1.Ord0;
                                        crudProductOpeningBalanceDto.Id = this.GetNewObjectId();

                                        var resul = ObjectMapper.Map<CrudProductOpeningBalanceDto, ProductOpeningBalance>(crudProductOpeningBalanceDto);
                                        await _productOpeningBalanceService.CreateAsync(resul);
                                    }



                                }


                            }
                        }
                        if (dto.FieldCode == "ProductLotCode")
                        {
                            var productOpeningBalances = await _productOpeningBalanceService.GetQueryableAsync();
                            var openingBalances = productOpeningBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.ProductLotCode == dto.OldValue).ToList();
                            foreach (var item1 in openingBalances)
                            {
                                var accCheck = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == dto.NewValue
                                                                    && p.ProductOriginCode == item1.ProductOriginCode
                                                                    && p.AccCode == item1.AccCode

                                                                    ).ToList();
                                if (accCheck.Count > 0)
                                {
                                    var resulAcc = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == dto.NewValue
                                                                    && p.AccCode == item1.AccCode
                                                                     && p.ProductOriginCode == item1.ProductOriginCode
                                                                    ).FirstOrDefault();
                                    item1.Quantity += resulAcc.Quantity;
                                    item1.Amount += resulAcc.Amount;
                                    item1.AmountCur += resulAcc.AmountCur;

                                    var update = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == dto.NewValue
                                                                    && p.AccCode == item1.AccCode
                                                                     && p.ProductOriginCode == item1.ProductOriginCode
                                                                    ).ToList();
                                    foreach (var item2 in update)
                                    {
                                        item2.Quantity = item1.Quantity;
                                        item2.Amount = item1.Amount;
                                        item2.AmountCur = item1.Amount;
                                        await _productOpeningBalanceService.UpdateAsync(item2);
                                    }

                                }
                                else
                                {

                                }

                            }
                        }
                        if (dto.FieldCode == "ProductOriginCode")
                        {
                            var productOpeningBalances = await _productOpeningBalanceService.GetQueryableAsync();
                            var openingBalances = productOpeningBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.ProductOriginCode == dto.OldValue).ToList();
                            foreach (var item1 in openingBalances)
                            {
                                var accCheck = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.ProductOriginCode == dto.NewValue
                                                                    && p.AccCode == item1.AccCode

                                                                    ).ToList();
                                if (accCheck.Count > 0)
                                {
                                    var resulAcc = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode
                                                                     && p.ProductOriginCode == dto.NewValue
                                                                    ).FirstOrDefault();
                                    item1.Quantity += resulAcc.Quantity;
                                    item1.Amount += resulAcc.Amount;
                                    item1.AmountCur += resulAcc.AmountCur;

                                    var update = openingBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.Year == _webHelper.GetCurrentYear()
                                                                    && p.AccCode == item1.AccCode
                                                                    && p.WarehouseCode == item1.WarehouseCode
                                                                    && p.ProductCode == item1.ProductCode
                                                                    && p.ProductLotCode == item1.ProductLotCode
                                                                    && p.AccCode == item1.AccCode
                                                                     && p.ProductOriginCode == dto.NewValue
                                                                    ).ToList();
                                    foreach (var item2 in update)
                                    {
                                        item2.Quantity = item1.Quantity;
                                        item2.Amount = item1.Amount;
                                        item2.AmountCur = item1.Amount;
                                        await _productOpeningBalanceService.UpdateAsync(item2);
                                    }

                                }
                                else
                                {

                                }

                            }
                        }
                        if (dto.FieldCode == "AccCode")
                        {

                            string sql2 = "UPDATE   " + "\"" + "ProductOpeningBalance" + "\"" + " SET " + "\"" + "AccCode" + "\"" + " = N'" + dto.NewValue + "' WHERE " + "\"" + "AccCode" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                            await _accountingDb.ExecuteSQLAsync(sql2);
                        }
                    }
                }
                else
                {
                    if (item.RefFieldCode == "ParentId")
                    {

                        var dt = new DataTable();
                        string sql = "Select *  from " + "\"" + item.RefTableName + "\"" + " where  " + "\"" + "Code" + "\"" + "=" + "'" + dto.NewValue + "'";
                        var strings = await _accountingDb.GetDataAsync(sql);

                        string sql2 = "UPDATE   " + "\"" + item.RefTableName + "\"" + " SET " + "\"" + "ParentId" + "\"" + " = N'" + strings + "' WHERE " + "\"" + "Code" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        await _accountingDb.ExecuteSQLAsync(sql2);

                    }
                    else
                    {
                        if (item.AttachYear == true)
                        {
                            string sql = "UPDATE   " + "\"" + item.RefTableName + "\"" + " SET " + "\"" + item.RefFieldCode + "\"" + " = N'" + dto.NewValue + "' WHERE " + "\"" + item.RefFieldCode + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' " + " AND " + "\"" + "Year" + "\"" + "='" + _webHelper.GetCurrentYear() + "' ";
                            await _accountingDb.ExecuteSQLAsync(sql);
                        }
                        else
                        {
                            string sql = "UPDATE   " + "\"" + item.RefTableName + "\"" + " SET " + "\"" + item.RefFieldCode + "\"" + " = N'" + dto.NewValue + "' WHERE " + "\"" + item.RefFieldCode + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                            await _accountingDb.ExecuteSQLAsync(sql);
                        }
                        //Update tên hv
                        if (dto.FieldCode == "ProductCode")
                        {
                            string sql1 = "Select *  from " + "\"" + "Product" + "\"" + " where  " + "\"" + "Code" + "\"" + "=" + "'" + dto.NewValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                            var resul = await _accountingDb.GetDataNameAsync(sql1);
                            string sql2 = "UPDATE   " + "\"" + "ProductVoucherDetail" + "\"" + " SET " + "\"" + "ProductName" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "ProductCode" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                            await _accountingDb.ExecuteSQLAsync(sql2);
                        }

                    }

                    //     if (dto.FieldCode == "CaseCode")
                    //{
                    //    string sql1 = "Select *  from " + "\"" + "AccCase" + "\"" + " where  " + "\"" + "Code" + "\"" + "=" + "'" + dto.NewValue + "'";
                    //    var resul = await _accountingDb.GetDataAsync(sql1);
                    //    string sql2 = "UPDATE   " + "\"" + "ProductVoucherDetail" + "\"" + " SET " + "\"" + "Name" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "Code" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                    //    await _accountingDb.ExecuteSQLAsync(sql2);
                    //}
                    if (dto.FieldCode == "PartnerCode")
                    {

                        string sql1 = "Select * from " + "\"" + "AccPartner" + "\"" + " where  " + "\"" + "Code" + "\"" + "=" + "'" + dto.NewValue + "'" + " AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        var resul = await _accountingDb.GetDataNameAsync(sql1);
                        string sql2 = "UPDATE   " + "\"" + "AccVoucher" + "\"" + " SET " + "\"" + "PartnerName0" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "PartnerCode0" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        await _accountingDb.ExecuteSQLAsync(sql2);
                        string sql3 = "UPDATE   " + "\"" + "AccVoucherDetail" + "\"" + " SET " + "\"" + "PartnerName" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "PartnerCode" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        await _accountingDb.ExecuteSQLAsync(sql3);

                        string sql4 = "UPDATE   " + "\"" + "ProductVoucher" + "\"" + " SET " + "\"" + "PartnerName0" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "PartnerCode0" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        await _accountingDb.ExecuteSQLAsync(sql4);

                        //string sql5 = "UPDATE   " + "\"" + "ProductVoucherDetail" + "\"" + " SET " + "\"" + "PartnerName" + "\"" + " = N'" + resul + "' WHERE " + "\"" + "PartnerCode" + "\"" + " = N'" + dto.OldValue + "' AND " + "\"" + "OrgCode" + "\"" + "='" + _webHelper.GetCurrentOrgUnit() + "' ";
                        //await _accountingDb.ExecuteSQLAsync(sql5);
                    }


                }
            }
            var res = new ResultDto();
            res.Ok = true;
            return res;
        }

        #region Private
        private List<FormularDto> GetFormular(string formular)
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i += 2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i + 1],
                    Math = lstData[i],
                });
            }
            return lst;
        }
        #endregion
    }
}
