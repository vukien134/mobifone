using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Vouchers;
using Accounting.Vouchers.VoucherNumbers;
using Accounting.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Accounting.Categories.VoucherPaymentBooks
{
    public class VoucherPaymentBookAppService : AccountingAppService, IVoucherPaymentBookAppService
    {
        #region Fields
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LedgerService _ledgerService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccPartnerService _accPartnerService;
        private readonly VoucherPaymentBeginningService _voucherPaymentBeginningService;
        private readonly VoucherPaymentBeginningDetailService _voucherPaymentBeginningDetailService;
        private readonly WindowService _windowService;
        private readonly ExcelService _excelService;
        #endregion
        #region Ctor
        public VoucherPaymentBookAppService(VoucherPaymentBookService voucherPaymentBookService,
                            DefaultVoucherCategoryService defaultVoucherCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            LedgerService ledgerService,
                            VoucherCategoryService voucherCategoryService,
                            AccPartnerService accPartnerService,
                            VoucherPaymentBeginningService voucherPaymentBeginningService,
                            VoucherPaymentBeginningDetailService voucherPaymentBeginningDetailService,
                            WindowService windowService,
                            ExcelService excelService
                            )
        {
            _voucherPaymentBookService = voucherPaymentBookService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _ledgerService = ledgerService;
            _voucherCategoryService = voucherCategoryService;
            _accPartnerService = accPartnerService;
            _voucherPaymentBeginningService = voucherPaymentBeginningService;
            _voucherPaymentBeginningDetailService = voucherPaymentBeginningDetailService;
            _windowService = windowService;
            _excelService = excelService;
        }
        #endregion

        //[Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<VoucherPaymentBookDto> CreateAsync(CrudVoucherPaymentBookDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(dto);
            var result = await _voucherPaymentBookService.CreateAsync(entity, true);
            return ObjectMapper.Map<VoucherPaymentBook, VoucherPaymentBookDto>(result);
        }

        //[Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _voucherPaymentBookService.DeleteAsync(id);
        }
        //[Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<VoucherPaymentBookDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        //[Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<VoucherPaymentBookDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<VoucherPaymentBookDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AccVoucherId).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<VoucherPaymentBook, VoucherPaymentBookDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<DataVoucherPaymentDto>> DataVoucherPaymentAsync(VoucherPaymentFilterDto dto)
        {
            var ledgers = await _ledgerService.GetQueryableAsync();
            ledgers = ledgers.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                      && (p.CreditAcc.StartsWith(dto.AccCode) || p.DebitAcc.StartsWith(dto.AccCode) || (dto.AccCode ?? "") == "")
                                      && p.Year == _webHelper.GetCurrentYear()
                                      && (p.VoucherDate >= dto.FromDate || dto.FromDate == null)
                                      && p.VoucherDate <= dto.ToDate);
            var accPartners = await _accPartnerService.GetQueryableAsync();
            accPartners = accPartners.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                               && (p.Code == dto.PartnerCode || dto.PartnerCode == null || dto.PartnerCode == ""));
            var voucherPaymentBooks0 = await _voucherPaymentBookService.GetQueryableAsync();
            voucherPaymentBooks0 = voucherPaymentBooks0.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var voucherPaymentBooks = voucherPaymentBooks0;
            voucherPaymentBooks = voucherPaymentBooks.Where(p => p.Year == _webHelper.GetCurrentYear()
                                                      && (p.PartnerCode == dto.PartnerCode || dto.PartnerCode == "")
                                                      && (p.AccCode.StartsWith(dto.AccCode) || (dto.AccCode ?? "") == "")
                                                      && (p.VoucherDate >= dto.FromDate || dto.FromDate == null)
                                                      && p.VoucherDate <= dto.ToDate);
            var voucherPaymentBeginnings = await _voucherPaymentBeginningService.GetQueryableAsync();
            voucherPaymentBeginnings = voucherPaymentBeginnings.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.Year == _webHelper.GetCurrentYear()
                                                      && (p.PartnerCode == dto.PartnerCode || dto.PartnerCode == null || dto.PartnerCode == ""));
            var voucherPaymentBeginningDetails = await _voucherPaymentBeginningDetailService.GetQueryableAsync();
            voucherPaymentBeginningDetails = voucherPaymentBeginningDetails.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var dataBetweenFromDateToDates = from v in voucherPaymentBooks
                                             join l in ledgers on v.DocumentId equals l.Ord0Extra into vjl
                                             from l in vjl.DefaultIfEmpty()
                                             join a in accPartners on v.PartnerCode equals a.Code into vja
                                             from a in vja.DefaultIfEmpty()
                                             where l.CreditAcc.StartsWith(dto.AccCode) || l.DebitAcc.StartsWith(dto.AccCode) || v.AccCode.StartsWith(dto.AccCode) || (dto.AccCode ?? "") == ""
                                             orderby v.DeadlinePayment
                                             group new { v, l, a } by new
                                             {
                                                 v.Id,
                                                 v.DocumentId,
                                                 v.VoucherDate,
                                                 v.VoucherNumber,
                                                 v.AccCode,
                                                 v.PartnerCode,
                                                 v.DeadlinePayment,
                                                 v.OrgCode,
                                                 v.AccVoucherId,
                                                 v.AccType,
                                                 a.Name,
                                                 a.Address,
                                                 a.Representative,
                                                 v.Times
                                             } into gr
                                             select new DataVoucherPaymentDto
                                             {
                                                 AccVoucherId = gr.Key.AccVoucherId,
                                                 DocumentId = gr.Key.DocumentId,
                                                 VoucherDate = gr.Key.VoucherDate,
                                                 VoucherNumber = gr.Key.VoucherNumber,
                                                 AccCode = gr.Key.AccCode,
                                                 PartnerCode = gr.Key.PartnerCode,
                                                 VatAmount = gr.Max(p => p.v.VatAmount),
                                                 DiscountAmount = gr.Max(p => p.v.DiscountAmount),
                                                 TotalAmount = gr.Max(p => p.v.TotalAmount),
                                                 AmountReceivable = gr.Max(p => p.v.AmountReceivable),
                                                 AmountReceived = gr.Max(p => p.v.AmountReceived),
                                                 DeadlinePayment = gr.Key.DeadlinePayment,
                                                 Times = gr.Key.Times,
                                                 AccType = gr.Key.AccType,
                                                 OrgCode = gr.Key.OrgCode,
                                                 PartnerName = gr.Key.Name,
                                                 Address = gr.Key.Address,
                                                 Representative = gr.Key.Representative,
                                                 AmountRemaining = gr.Max(p => p.v.AmountReceivable) - gr.Max(p => p.v.AmountReceived)
                                             };
            var dataBeginning = from a in voucherPaymentBeginnings
                                join b in voucherPaymentBeginningDetails on a.Id equals b.VoucherPaymentBeginningId
                                join c in accPartners on a.PartnerCode equals c.Code into ajc
                                from c in ajc.DefaultIfEmpty()
                                where a.VoucherDate < dto.FromDate
                                select new DataVoucherPaymentDto
                                {
                                    AccVoucherId = null,
                                    DocumentId = a.Id,
                                    VoucherDate = a.VoucherDate,
                                    VoucherNumber = a.VoucherNumber,
                                    AccCode = a.AccCode,
                                    PartnerCode = a.PartnerCode,
                                    Amount = a.TotalAmountWithoutVat,
                                    VatAmount = a.TotalAmountVat,
                                    DiscountAmount = a.TotalAmountDiscount,
                                    TotalAmount = a.TotalAmount,
                                    AmountReceivable = b.Amount,
                                    AmountReceived = 0,
                                    DeadlinePayment = b.DeadlinePayment,
                                    Times = b.Times,
                                    AccType = a.PaymentType,
                                    OrgCode = a.OrgCode,
                                    PartnerName = c.Name,
                                    Address = c.Address,
                                    Representative = c.Representative,
                                    AmountRemaining = null
                                };
            // các chứng từ thanh toán trong sổ thanh toán và đầu kỳ
            var voucherPayment = from s in Enumerable.Concat(dataBetweenFromDateToDates, dataBeginning)
                                 select s;
            var voucherPaymentBookGroup = from a in voucherPaymentBooks0
                                          group new { a } by new
                                          {
                                              a.DocumentId,
                                              a.AccCode,
                                              a.VoucherDate,
                                              a.VoucherNumber,
                                              a.PartnerCode,
                                              a.Times,
                                              a.OrgCode
                                          } into gr
                                          select new
                                          {
                                              DocumentId = gr.Key.DocumentId,
                                              VoucherDate = gr.Key.VoucherDate,
                                              VoucherNumber = gr.Key.VoucherNumber,
                                              AccCode = gr.Key.AccCode,
                                              OrgCode = gr.Key.OrgCode,
                                              PartnerCode = gr.Key.PartnerCode,
                                              Times = gr.Key.Times,
                                              AmountReceived = gr.Sum(p => p.a.AmountReceived)
                                          };


            voucherPayment = from a in voucherPayment
                             join b in voucherPaymentBookGroup on new {a.DocumentId, a.Times} equals new { b.DocumentId, b.Times } into ajb
                             from b in ajb.DefaultIfEmpty()
                             orderby a.AccVoucherId, a.VoucherNumber, a.Times
                             select new DataVoucherPaymentDto
                             {
                                 AccVoucherId = a.AccVoucherId,
                                 DocumentId = a.DocumentId,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = a.AmountReceivable,
                                 AmountReceived = (b != null) ? b.AmountReceived : 0,
                                 Amount = a.Amount,
                                 DeadlinePayment = a.DeadlinePayment,
                                 Times = a.Times,
                                 AccType = a.AccType,
                                 OrgCode = a.OrgCode,
                                 PartnerName = a.PartnerName,
                                 Address = a.Address,
                                 Representative = a.Representative,
                                 AmountRemaining = a.AmountReceivable - ((b != null) ? b.AmountReceived : 0)
                             };
            var data = voucherPayment.Where(p => p.AccVoucherId == null && p.AmountRemaining > 0
                                              && p.AccCode.StartsWith(dto.AccCode)).ToList();
            return data;
        }

        public async Task<List<DetailDataVoucherPaymentDto>> DetailVoucherPaymentAsync(DetailVoucherPaymentFilterDto dto)
        {
            var ledgers = await _ledgerService.GetQueryableAsync();
            ledgers = ledgers.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                      && p.Year == _webHelper.GetCurrentYear());
            var accPartners = await _accPartnerService.GetQueryableAsync();
            accPartners = accPartners.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                               && (p.Code == dto.PartnerCode || dto.PartnerCode == null || dto.PartnerCode == ""));
            var voucherCategories = await _voucherCategoryService.GetQueryableAsync();
            var lstVoucherCategories = voucherCategories.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            if (lstVoucherCategories.Count == 0)
            {
                var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();
                lstVoucherCategories = defaultVoucherCategory.Select(p => ObjectMapper.Map<DefaultVoucherCategory, VoucherCategory>(p)).ToList();
            }

            var voucherPaymentBooks = await _voucherPaymentBookService.GetQueryableAsync();
            voucherPaymentBooks = voucherPaymentBooks.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.Year == _webHelper.GetCurrentYear());
            var voucherPaymentBeginnings = await _voucherPaymentBeginningService.GetQueryableAsync();
            voucherPaymentBeginnings = voucherPaymentBeginnings.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.Year == _webHelper.GetCurrentYear());
            var voucherPaymentBeginningDetails = await _voucherPaymentBeginningDetailService.GetQueryableAsync();
            voucherPaymentBeginningDetails = voucherPaymentBeginningDetails.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (dto.AccCode.StartsWith("1"))
            {
                var voucherPaymentBookGroup = from a in voucherPaymentBooks
                                              group new { a } by new
                                              {
                                                  a.DocumentId,
                                                  a.Times
                                              } into gr
                                              select new
                                              {
                                                  DocumentId = gr.Key.DocumentId,
                                                  Times = gr.Key.Times,
                                                  AmountReceivable = gr.Sum(p => p.a.AmountReceivable)
                                              };
                var voucherPaymentBookGroup2 = from a in voucherPaymentBooks
                                               join b in voucherPaymentBookGroup on a.DocumentId equals b.DocumentId into ajb
                                               from b in ajb.DefaultIfEmpty()
                                               where a.Times == b.Times
                                               group new { a } by new
                                              {
                                                  a.AccVoucherId
                                              } into gr
                                              select new
                                              {
                                                  AccVoucherId = gr.Key.AccVoucherId,
                                                  Residual = gr.Sum(p => p.a.AmountReceivable) - gr.Max(p => p.a.AmountReceived)
                                              };

                var data1 = from a in ledgers.ToList()
                           join b in accPartners.ToList() on a.CreditPartnerCode equals b.Code
                           join c in lstVoucherCategories on a.VoucherCode equals c.Code
                           where a.CreditPartnerCode == dto.PartnerCode && a.CreditAcc == dto.AccCode && a.VoucherId != null && c.VoucherKind == "KT"
                                && !((from b in voucherPaymentBooks
                                     where b.PartnerCode == dto.PartnerCode && b.OrgCode == dto.OrgCode
                                           && b.AccVoucherId != null
                                     select b.AccVoucherId).Contains(a.VoucherId))
                           select new DetailDataVoucherPaymentDto
                           {
                               Id = a.VoucherId,
                               VoucherDate = a.VoucherDate,
                               VoucherNumber = a.VoucherNumber,
                               Amount = a.Amount,
                               PartnerCode = a.DebitPartnerCode,
                               PartnerName = b.Name,
                               Residual = 0
                           };

                var data2 = from a in ledgers.ToList()
                            join b in accPartners.ToList() on a.DebitPartnerCode equals b.Code
                            join c in voucherPaymentBookGroup2 on a.VoucherId equals c.AccVoucherId
                            where a.DebitPartnerCode == dto.PartnerCode && a.DebitAcc == dto.AccCode && a.VoucherId != null && c.Residual < 0
                            select new DetailDataVoucherPaymentDto
                            {
                                Id = a.VoucherId,
                                VoucherDate = a.VoucherDate,
                                VoucherNumber = a.VoucherNumber,
                                Amount = a.Amount,
                                PartnerCode = a.DebitPartnerCode,
                                PartnerName = b.Name,
                                Residual = (c.Residual < 0) ? c.Residual*(-1) : c.Residual
                            };

                // trường hợp này phiếu trả lại được coi là 1 phiếu kế toán thanh toán cho chứng từ
                var dataPTL = from a in ledgers
                            join b in accPartners on a.DebitPartnerCode equals b.Code
                            where a.DebitPartnerCode == dto.PartnerCode && a.DebitAcc == dto.AccCode && a.VoucherId != null && a.VoucherCode == "PTL"
                                 && !(from b in voucherPaymentBooks
                                      where b.PartnerCode == dto.PartnerCode && b.OrgCode == dto.OrgCode
                                            && b.AccVoucherId != null
                                      select b.AccVoucherId).Contains(a.VoucherId)
                            select new DetailDataVoucherPaymentDto
                            {
                                Id = a.VoucherId,
                                VoucherDate = a.VoucherDate,
                                VoucherNumber = a.VoucherNumber,
                                Amount = a.Amount,
                                PartnerCode = a.DebitPartnerCode,
                                PartnerName = b.Name + " Hàng bán trả lại",
                                Residual = 0
                            };
                var dataRes = from s in Enumerable.Concat(data1, data2)
                              orderby s.VoucherDate, s.VoucherNumber
                              select s;
                var data = dataRes.ToList();
                return data;

            }
            else
            {
                var voucherPaymentBookGroup = from a in voucherPaymentBooks
                                              group new { a } by new
                                              {
                                                  a.DocumentId,
                                                  a.Times
                                              } into gr
                                              select new
                                              {
                                                  DocumentId = gr.Key.DocumentId,
                                                  Times = gr.Key.Times,
                                                  AmountReceivable = gr.Sum(p => p.a.AmountReceivable)
                                              };
                var voucherPaymentBookGroup2 = from a in voucherPaymentBooks
                                               join b in voucherPaymentBookGroup on a.DocumentId equals b.DocumentId into ajb
                                               from b in ajb.DefaultIfEmpty()
                                               where a.Times == b.Times
                                               group new { a } by new
                                               {
                                                   a.AccVoucherId
                                               } into gr
                                               select new
                                               {
                                                   AccVoucherId = gr.Key.AccVoucherId,
                                                   Residual = gr.Sum(p => p.a.AmountReceivable) - gr.Max(p => p.a.AmountReceived)
                                               };

                var data1 = from a in ledgers.ToList()
                            join b in accPartners.ToList() on a.DebitPartnerCode equals b.Code
                            join c in lstVoucherCategories on a.VoucherCode equals c.Code
                            where a.DebitPartnerCode == dto.PartnerCode && a.DebitAcc == dto.AccCode && a.VoucherId != null && c.VoucherKind == "KT"
                                 && !(from b in voucherPaymentBooks
                                      where b.PartnerCode == dto.PartnerCode && b.OrgCode == dto.OrgCode
                                            && b.AccVoucherId != null
                                      select b.AccVoucherId).Contains(a.VoucherId)
                            select new DetailDataVoucherPaymentDto
                            {
                                Id = a.VoucherId,
                                VoucherDate = a.VoucherDate,
                                VoucherNumber = a.VoucherNumber,
                                Amount = a.Amount,
                                PartnerCode = a.CreditPartnerCode,
                                PartnerName = b.Name,
                                Residual = 0
                            };

                var data2 = from a in ledgers
                            join b in accPartners on a.CreditPartnerCode equals b.Code
                            join c in voucherPaymentBookGroup2 on a.VoucherId equals c.AccVoucherId
                            where a.CreditPartnerCode == dto.PartnerCode && a.CreditAcc == dto.AccCode && a.VoucherId != null && c.Residual < 0
                            select new DetailDataVoucherPaymentDto
                            {
                                Id = a.VoucherId,
                                VoucherDate = a.VoucherDate,
                                VoucherNumber = a.VoucherNumber,
                                Amount = a.Amount,
                                PartnerCode = a.CreditPartnerCode,
                                PartnerName = b.Name,
                                Residual = (c.Residual < 0) ? c.Residual * (-1) : c.Residual
                            };
                var dataRes = from s in Enumerable.Concat(data1, data2)
                              select s;
                var data = dataRes.ToList();
                return data;
            }
        }

        public async Task<ResultDto> PaymentAsync(ListPaymentFilterDto lstDto)
        {
            await _licenseBusiness.CheckExpired();
            var res = new ResultDto();
            var voucherPaymentBooks = await _voucherPaymentBookService.GetQueryableAsync();
            voucherPaymentBooks = voucherPaymentBooks.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.Year == _webHelper.GetCurrentYear());
            foreach(var dto in lstDto.Data)
            {
                var voucherPaymentBook = from a in voucherPaymentBooks
                                         where a.DocumentId == dto.DocumentId && a.Times == dto.Times
                                         group new { a } by new
                                         {
                                             a.DocumentId,
                                             a.VoucherDate,
                                             a.VoucherNumber,
                                             a.AccCode,
                                             a.PartnerCode,
                                             a.Amount,
                                             a.VatAmount,
                                             a.DiscountAmount,
                                             a.TotalAmount,
                                             a.DeadlinePayment,
                                             a.AmountReceivable,
                                             a.Times,
                                             a.AmountReceived,
                                             a.OrgCode,
                                             a.Year,
                                             a.AccVoucherId,
                                             a.AccType
                                         } into gr
                                         select new CrudVoucherPaymentBookDto
                                         {
                                             Id = GetNewObjectId(),
                                             DocumentId = gr.Key.DocumentId,
                                             VoucherDate = gr.Key.VoucherDate,
                                             VoucherNumber = gr.Key.VoucherNumber,
                                             AccCode = gr.Key.AccCode,
                                             PartnerCode = gr.Key.PartnerCode,
                                             Amount = gr.Key.Amount,
                                             VatAmount = gr.Key.VatAmount,
                                             DiscountAmount = gr.Key.DiscountAmount,
                                             TotalAmount = gr.Key.TotalAmount,
                                             DeadlinePayment = gr.Key.DeadlinePayment,
                                             AmountReceivable = gr.Key.AmountReceivable,
                                             Times = gr.Key.Times,
                                             AmountReceived = dto.Amount,
                                             OrgCode = gr.Key.OrgCode,
                                             Year = gr.Key.Year,
                                             AccVoucherId = dto.AccVoucherId,
                                             AccType = gr.Key.AccType
                                         };
                var data = voucherPaymentBook.First();
                await this.CreateAsync(data);
            }
            res.Ok = true;
            return res;
        }

        //[Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudVoucherPaymentBookDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _voucherPaymentBookService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _voucherPaymentBookService.UpdateAsync(entity);
        }
        
        public async Task<VoucherPaymentBookDto> GetByIdAsync(string voucherPaymentBookId)
        {
            var voucherPaymentBook = await _voucherPaymentBookService.GetAsync(voucherPaymentBookId);
            return ObjectMapper.Map<VoucherPaymentBook, VoucherPaymentBookDto>(voucherPaymentBook);
        }
        
        public async Task<WindowDto> GetWindowByVoucherCodeAsync(string voucherCode)
        {
            var window = await _windowService.GetQueryableAsync();
            var data = window.Where(x => x.VoucherCode == voucherCode).FirstOrDefault();
            return ObjectMapper.Map<Window, WindowDto>(data);
        }
        #region Private
        private async Task<IQueryable<VoucherPaymentBook>> Filter(PageRequestDto dto)
        {
            var queryable = await _voucherPaymentBookService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());


            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.AccCode.Contains(dto.QuickSearch) || p.PartnerCode.Contains(dto.QuickSearch));
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
