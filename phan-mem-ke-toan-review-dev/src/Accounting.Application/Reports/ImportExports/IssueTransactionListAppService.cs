using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Accounting.Reports.GeneralDiaries;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.Catgories.ProductVouchers;
using Accounting.Vouchers;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.Formula.Functions;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class IssueTransactionListAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductAppService _productAppService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public IssueTransactionListAppService(
                        ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccSectionService accSectionService,
                        DepartmentService departmentService,
                        AccCaseService accCaseService,
                        FProductWorkService fProductWorkService,
                        AccPartnerService accPartnerService,
                        PartnerGroupService partnerGroupService,
                        AccPartnerAppService accPartnerAppService,
                        ProductGroupService productGroupService,
                        ProductService productService,
                        WarehouseService warehouseService,
                        ProductAppService productAppService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _fProductWorkService = fProductWorkService;
            _accPartnerService = accPartnerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerAppService = accPartnerAppService;
            _productGroupService = productGroupService;
            _productService = productService;
            _warehouseService = warehouseService;
            _productAppService = productAppService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.IssueTransactionListReportView)]
        public async Task<ReportResponseDto<IssueTransactionListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroup = productGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var caseCode = await _accCaseService.GetQueryableAsync();
            var lstCase = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var warehouse = await _warehouseService.GetQueryableAsync();
            var lstWarehouse = warehouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());

            var department = await _departmentService.GetQueryableAsync();
            var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstfProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            incurredData = (from p in incurredData
                            join pn in lstPartner on p.PartnerCode equals pn.Code into s
                            from pa in s.DefaultIfEmpty()
                            join g in lstProduct on p.ProductCode equals g.Code into m
                            from pro in m.DefaultIfEmpty()
                            select new IssueTransactionListDto
                            {
                                Sort = 2,
                                Bold = "K",
                                OrgCode = p.OrgCode,
                                Year = p.Year,
                                VoucherNumber = p.VoucherNumber,
                                VoucherDate = p.VoucherDate,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                ExchangeRate = p.ExchangeRate,
                                Quantity = p.Quantity,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                VoucherCode = p.VoucherCode,
                                FProductWorkCode = p.FProductWorkCode,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceSymbol = p.InvoiceSymbol,
                                InvoiceNumber = p.InvoiceNumber,
                                Price = p.Price,
                                PartnerCode = p.PartnerCode,
                                PartnerName = pa != null ? pa.Name : null,
                                UnitCode = p.UnitCode,
                                ProductGroupCode = pro != null ? pro.ProductGroupCode : null,
                                ProductCode = p.ProductCode,
                                ProductName = pro != null ? pro.Name : null,
                                TransWarehouseCode = p.TransWarehouseCode,
                                CreditAcc2 = p.CreditAcc2,
                                Note = p.ProductCode + " " + (pro != null ? pro.Name : null) + " " + p.Note,
                                VoucherGroup = p.VoucherGroup,
                                GroupCode = null,
                                CaseCode = p.CaseCode,
                                CaseName = p.CaseName,
                                WarehouseCode = p.WarehouseCode,
                                WarehouseName = p.WarehouseName,
                                AccName = p.AccName,
                                DepartmentCode = p.DepartmentCode,
                                DepartmentName = p.DepartmentName,
                                Description = p.Description
                            }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                incurredData = incurredData.Where(p => p.PartnerCode == dto.Parameters.PartnerCode).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.LstVoucherCode))
            {
                incurredData = incurredData.Where(p => p.VoucherCode == dto.Parameters.LstVoucherCode).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = (from p in incurredData
                                where GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) >= GetVoucherNumber(p.VoucherNumber)
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = null,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    Description = p.Description
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var partnerGroup = await _partnerGroupService.GetQueryableAsync();
                var lstPartnerGroup = partnerGroup.Where(p => p.Id == dto.Parameters.PartnerGroup).ToList();
                if (lstPartnerGroup.Count > 0)
                {
                    var code = lstPartnerGroup.FirstOrDefault().Code;
                    var partners = await _accPartnerAppService.GetListByPartnerGroupCode(code);
                    incurredData = (from p in incurredData
                                    join b in partners on p.PartnerCode equals b.Code
                                    select new IssueTransactionListDto
                                    {
                                        Sort = 2,
                                        Bold = "K",
                                        OrgCode = p.OrgCode,
                                        Year = p.Year,
                                        VoucherNumber = p.VoucherNumber,
                                        VoucherDate = p.VoucherDate,
                                        DebitAcc = p.DebitAcc,
                                        CreditAcc = p.CreditAcc,
                                        ExchangeRate = p.ExchangeRate,
                                        Quantity = p.Quantity,
                                        Amount = p.Amount,
                                        AmountCur = p.AmountCur,
                                        VoucherId = p.VoucherId,
                                        Id = p.Id,
                                        VoucherCode = p.VoucherCode,
                                        FProductWorkCode = p.FProductWorkCode,
                                        InvoiceDate = p.InvoiceDate,
                                        InvoiceSymbol = p.InvoiceSymbol,
                                        InvoiceNumber = p.InvoiceNumber,
                                        Price = p.Price,
                                        PartnerCode = p.PartnerCode,
                                        PartnerName = p.PartnerName,
                                        UnitCode = p.UnitCode,
                                        ProductGroupCode = p.ProductGroupCode,
                                        ProductCode = p.ProductCode,
                                        ProductName = p.ProductName,
                                        TransWarehouseCode = p.TransWarehouseCode,
                                        CreditAcc2 = p.CreditAcc2,
                                        Note = p.Note,
                                        VoucherGroup = p.VoucherGroup,
                                        GroupCode = null,
                                        CaseCode = p.CaseCode,
                                        CaseName = p.CaseName,
                                        Description = p.Description
                                    }).ToList();
                }

            }
   
                if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
                {
                    lstProductGroup = lstProductGroup.Where(p => p.Id == dto.Parameters.ProductGroupCode).ToList();
                    if (lstProductGroup.Count > 0)
                    {
                        var code = lstProductGroup.FirstOrDefault().Code;
                        var products = await _productAppService.GetListByProductGroupCode(code);
                        incurredData = (from p in incurredData
                                        join b in products on p.ProductCode equals b.Code
                                        select new IssueTransactionListDto
                                        {
                                            Sort = 2,
                                            Bold = "K",
                                            OrgCode = p.OrgCode,
                                            Year = p.Year,
                                            VoucherNumber = p.VoucherNumber,
                                            VoucherDate = p.VoucherDate,
                                            DebitAcc = p.DebitAcc,
                                            CreditAcc = p.CreditAcc,
                                            ExchangeRate = p.ExchangeRate,
                                            Quantity = p.Quantity,
                                            Amount = p.Amount,
                                            AmountCur = p.AmountCur,
                                            VoucherId = p.VoucherId,
                                            Id = p.Id,
                                            VoucherCode = p.VoucherCode,
                                            FProductWorkCode = p.FProductWorkCode,
                                            InvoiceDate = p.InvoiceDate,
                                            InvoiceSymbol = p.InvoiceSymbol,
                                            InvoiceNumber = p.InvoiceNumber,
                                            Price = p.Price,
                                            PartnerCode = p.PartnerCode,
                                            PartnerName = p.PartnerName,
                                            UnitCode = p.UnitCode,
                                            ProductGroupCode = b.Code,
                                            ProductCode = p.ProductCode,
                                            ProductName = p.ProductName,
                                            TransWarehouseCode = p.TransWarehouseCode,
                                            CreditAcc2 = p.CreditAcc2,
                                            Note = p.Note,
                                            VoucherGroup = p.VoucherGroup,
                                            GroupCode = null,
                                            CaseCode = p.CaseCode,
                                            CaseName = p.CaseName,
                                            ProductGroupName = b.Name,
                                            Description = p.Description
                                        }).ToList();
                    }

                }



                if (dto.Parameters.Sort == "1")
            {
                incurredData = (from p in incurredData

                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.ProductCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    Description = p.Description
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.VoucherDate,
                                       p.VoucherNumber,
                                       p.Amount,
                                       p.AmountCur,
                                       p.Description
                                   }
                                   by new
                                   {
                                       p.VoucherDate,
                                       p.VoucherNumber,
                                       p.Description
                                   } into gr
                                   select new
                                   {

                                       GroupCode = gr.Key.VoucherDate + gr.Key.VoucherNumber,
                                       Description = gr.Key.Description,
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur),
                                       VoucherDate = gr.Key.VoucherDate,
                                       VoucherNumber = gr.Key.VoucherNumber
                                   };
                incurredData = (from p in incurredData

                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    //VoucherNumber = p.VoucherNumber,
                                    //VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.VoucherDate + p.VoucherNumber,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    Description = p.Description
                                }).ToList();
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         VoucherDate = p.VoucherDate,
                                         VoucherNumber = p.VoucherNumber,
                                         Note = p.Description,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            // partner
            if (dto.Parameters.Sort == "2")
            {
                incurredData = (from p in incurredData

                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.PartnerCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    Description = p.Description
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.PartnerCode,
                                       p.PartnerName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.PartnerCode
                                   } into gr
                                   select new
                                   {

                                       ProductCode = gr.Key.PartnerCode,
                                       GroupCode = gr.Key.PartnerCode,
                                       GroupName = gr.Max(p => p.PartnerName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có đối tượng " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            //casecode
            if (dto.Parameters.Sort == "3")
            {
                incurredData = (from p in incurredData
                                join b in lstCase on p.CaseCode equals b.Code into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.CaseCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = ca != null ? ca.Name : null
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.CaseCode,
                                       p.CaseName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.PartnerCode
                                   } into gr
                                   select new
                                   {

                                       ProductCode = gr.Key.PartnerCode,
                                       GroupCode = gr.Key.PartnerCode,
                                       GroupName = gr.Max(p => p.CaseName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có vụ việc " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            // washouse
            if (dto.Parameters.Sort == "4")
            {
                incurredData = (from p in incurredData
                                join b in lstWarehouse on p.WarehouseCode equals b.Code into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.WarehouseCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = ca != null ? ca.Name : null
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.WarehouseCode,
                                       p.WarehouseName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.WarehouseCode
                                   } into gr
                                   select new
                                   {

                                       WarehouseCode = gr.Key.WarehouseCode,
                                       GroupCode = gr.Key.WarehouseCode,
                                       GroupName = gr.Max(p => p.WarehouseName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có kho " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            //acc
            if (dto.Parameters.Sort == "5")
            {
                incurredData = (from p in incurredData
                                join b in lstAccountSystem on p.CreditAcc equals b.AccCode into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.CreditAcc,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = p.WarehouseName,
                                    AccName = ca != null ? ca.AccName : null
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.CreditAcc,
                                       p.AccName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.CreditAcc
                                   } into gr
                                   select new
                                   {

                                       CreditAcc = gr.Key.CreditAcc,
                                       GroupCode = gr.Key.CreditAcc,
                                       GroupName = gr.Max(p => p.AccName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có tài khoản " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            //departmen
            if (dto.Parameters.Sort == "6")
            {
                incurredData = (from p in incurredData
                                join b in lstDepartment on p.DepartmentCode equals b.Code into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.DepartmentCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = p.WarehouseName,
                                    AccName = p.AccName,
                                    DepartmentCode = p.DepartmentCode,
                                    DepartmentName = ca != null ? ca.Name : null
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.DepartmentCode,
                                       p.DepartmentName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.DepartmentCode
                                   } into gr
                                   select new
                                   {

                                       DepartmentCode = gr.Key.DepartmentCode,
                                       GroupCode = gr.Key.DepartmentCode,
                                       GroupName = gr.Max(p => p.DepartmentName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có bộ phận " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }

            //ctsp
            if (dto.Parameters.Sort == "7")
            {
                incurredData = (from p in incurredData
                                join b in lstfProductWork on p.FProductWorkCode equals b.Code into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = p.WarehouseName,
                                    AccName = p.AccName,
                                    DepartmentCode = p.DepartmentCode,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = ca != null ? ca.Name : null
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.FProductWorkCode,
                                       p.FProductWorkName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.FProductWorkCode
                                   } into gr
                                   select new
                                   {

                                       FProductWorkCode = gr.Key.FProductWorkCode,
                                       GroupCode = gr.Key.FProductWorkCode,
                                       GroupName = gr.Max(p => p.FProductWorkName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có chứng từ sản phẩm " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            //productGroup
            if (dto.Parameters.Sort == "8")
            {
                incurredData = (from p in incurredData

                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.ProductGroupCode,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = p.WarehouseName,
                                    AccName = p.AccName,
                                    DepartmentCode = p.DepartmentCode,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    ProductGroupName = p.ProductGroupName
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.ProductGroupCode,
                                       p.ProductGroupName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.ProductGroupCode
                                   } into gr
                                   select new
                                   {

                                       ProductGroupCode = gr.Key.ProductGroupCode,
                                       GroupCode = gr.Key.ProductGroupCode,
                                       GroupName = gr.Max(p => p.ProductGroupName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }

            //filter accCode 
            if (!string.IsNullOrEmpty(dto.Parameters.AccCode))
            {
                incurredData = (from p in incurredData
                                where p.DebitAcc == dto.Parameters.AccCode
                                || p.CreditAcc == dto.Parameters.AccCode                     
                                join b in lstAccountSystem on p.DebitAcc equals b.AccCode into n
                                from ca in n.DefaultIfEmpty()
                                select new IssueTransactionListDto
                                {
                                    Sort = 2,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Year = p.Year,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherDate = p.VoucherDate,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    ExchangeRate = p.ExchangeRate,
                                    Quantity = p.Quantity,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    Price = p.Price,
                                    PartnerCode = p.PartnerCode,
                                    PartnerName = p.PartnerName,
                                    UnitCode = p.UnitCode,
                                    ProductGroupCode = p.ProductGroupCode,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    CreditAcc2 = p.CreditAcc2,
                                    Note = p.Note,
                                    VoucherGroup = p.VoucherGroup,
                                    GroupCode = p.CreditAcc,
                                    CaseCode = p.CaseCode,
                                    CaseName = p.CaseName,
                                    WarehouseCode = p.WarehouseCode,
                                    WarehouseName = p.WarehouseName,
                                    AccName = ca != null ? ca.AccName : null,
                                }).ToList();
                var sumAktGroup0 = from p in incurredData
                                   group new
                                   {
                                       p.CreditAcc,
                                       p.AccName,
                                       p.Amount,
                                       p.AmountCur
                                   }
                                   by new
                                   {
                                       p.CreditAcc
                                   } into gr
                                   select new
                                   {

                                       CreditAcc = gr.Key.CreditAcc,
                                       GroupCode = gr.Key.CreditAcc,
                                       GroupName = gr.Max(p => p.AccName),
                                       Amount = gr.Sum(p => p.Amount),
                                       AmountCur = gr.Sum(p => p.AmountCur)
                                   };
                var incurredDatas = (from p in sumAktGroup0
                                     select new IssueTransactionListDto
                                     {
                                         Sort = 0,
                                         Bold = "C",
                                         GroupCode = p.GroupCode,
                                         OrgCode = null,
                                         Note = p.GroupCode == null ? "Không có tài khoản " : p.GroupCode + " " + p.GroupName,
                                         Amount = p.Amount,
                                         AmountCur = p.AmountCur,


                                     }).ToList();
                incurredData.AddRange(incurredDatas);

            }
            var resul = incurredData.OrderBy(p => p.GroupCode)
                                    .ThenBy(p => p.Sort)
                                    .ThenBy(p => p.VoucherDate)
                                    .ToList();


            var reportResponse = new ReportResponseDto<IssueTransactionListDto>();
            reportResponse.Data = resul;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        [Authorize(ReportPermissions.IssueTransactionListReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dataSource = await CreateDataAsync(dto);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());
            var reportTemplate = await _reportTemplateService.GetByCodeAsync(dto.ReportTemplateCode);
            string fileTemplate = reportTemplate.FileTemplate.Replace(".xml", "");
            fileTemplate = fileTemplate + "_" + dto.VndNt;
            if (!File.Exists(GetFileTemplatePath(fileTemplate + ".xml")))
            {
                throw new Exception("Không tìm thấy mẫu in!");
            }
            var renderOption = new RenderOption()
            {
                DataSource = dataSource,
                TypePrint = dto.Type,
                TemplateFile = GetFileTemplatePath(fileTemplate + ".xml"),
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);
            var result = render.Execute();

            return new FileContentResult(result, MIMETYPE.GetContentType(dto.Type.ToLower()))
            {
                FileDownloadName = $"{fileTemplate}.{dto.Type}"
            };
        }
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private decimal GetVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (numbers.Length > 0)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }
        private async Task<List<IssueTransactionListDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Select(p => new IssueTransactionListDto()
            {
                OrgCode = p.OrgCode,
                Year = p.Year,
                VoucherNumber = p.VoucherNumber,
                VoucherDate = p.VoucherDate,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                ExchangeRate = p.ExchangeRate,
                Quantity = p.Quantity,
                Amount = p.Amount,
                AmountCur = p.AmountCur,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                FProductWorkCode = p.FProductWorkCode,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                Price = p.Price,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                PartnerName = null,
                UnitCode = p.UnitCode,
                CaseCode = p.CaseCode,
                CaseName = null,
                WarehouseCode = p.WarehouseCode,
                WarehouseName = null,
                //ProductGroupCode = p.ProductGroupCode,
                ProductCode = p.ProductCode,
                //ProductName = p.pro
                TransWarehouseCode = p.TransWarehouseCode,
                CreditAcc2 = p.CreditAcc2,
                Note = p.Note,
                VoucherGroup = p.VoucherGroup,
                AccName = null,
                DepartmentCode = p.DepartmentCode,
                DepartmentName = null,
                Description = p.Description

            }).Where(p => p.VoucherGroup == 2).ToList();


            return incurredData;
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehouseBook(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetWarehouseBookData(dic);
            return data;
        }


        private Dictionary<string, object> GetWarehouseBookParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            if (!string.IsNullOrEmpty(dto.EndVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.EndVoucherNumber, dto.EndVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.BeginVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.BeginVoucherNumber, dto.BeginVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            }
            //if (!string.IsNullOrEmpty(dto.PartnerCode))
            //{
            //    dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            //}
            if (!string.IsNullOrEmpty(dto.ProductGroupCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.CaseCode))
            {
                dic.Add(WarehouseBookParameterConst.CaseCode, dto.CaseCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(WarehouseBookParameterConst.SectionCode, dto.SectionCode);
            }

            if (!string.IsNullOrEmpty(dto.VoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.VoucherCode, dto.VoucherCode);
            }
            if (!string.IsNullOrEmpty(dto.Sort))
            {
                dic.Add(WarehouseBookParameterConst.Sort, dto.Sort);
            }



            return dic;
        }


        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        private async Task<dynamic> GetTenantSetting(string orgCode)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            var tenantSettings = await _tenantSettingService.GetBySettingTypeAsync(orgCode, TenantSettingType.Report);
            foreach (var setting in tenantSettings)
            {
                ((IDictionary<String, Object>)exo).Add(setting.Key, setting.Value);
            }
            return exo;
        }

        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private async Task<string> GetPartnerName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(code, orgCode);
            if (partner == null) return null;

            return partner.Name;
        }
        #endregion
    }
}

