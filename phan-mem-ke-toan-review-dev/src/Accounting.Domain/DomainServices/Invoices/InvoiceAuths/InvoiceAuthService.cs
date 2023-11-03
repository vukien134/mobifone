using System;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Invoices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Invoices.InvoiceAuths
{
    public class InvoiceAuthService : BaseDomainService<InvoiceAuth, string>
    {
        private readonly LedgerService _ledgerService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly ProductVoucherService _productVoucherService;
        public InvoiceAuthService(IRepository<InvoiceAuth, string> repository,
                                LedgerService ledgerService,
                                WarehouseBookService warehouseBookService,
                                AccTaxDetailService accTaxDetailService,
                                VoucherExciseTaxService voucherExciseTaxService,
                                ProductVoucherService productVoucherService)
            : base(repository)
        {
            _ledgerService = ledgerService;
            _warehouseBookService = warehouseBookService;
            _accTaxDetailService = accTaxDetailService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _productVoucherService = productVoucherService;
        }
        public async Task<bool> IsExistCode(InvoiceAuth entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.InvoiceNumber == entity.InvoiceNumber
                                && p.Id != entity.Id &&  !string.IsNullOrEmpty(entity.InvoiceNumber));
        }
        public override async Task CheckDuplicate(InvoiceAuth entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceAuth, ErrorCode.Duplicate),
                        $"InvoiceAuth Code ['{entity.InvoiceNumber}'] already exist ");
            }
        }

        public async Task UpdateInvoiceNumberAsync(UpdateInvoiceNumberDto dto)
        {
            var invoiceAuth = await this.GetAsync(dto.Id);
            invoiceAuth.InvoiceSeries = dto.InvoiceSeries;
            invoiceAuth.InvoiceNumber = dto.InvoiceNumber;
            invoiceAuth.InvoiceId = Guid.Parse(dto.HDonID);
            invoiceAuth.InvoiceCodeId = Guid.Parse(dto.cttb_id);
            //invoiceAuth.ReservationCode = dto.ReservationCode;
            //invoiceAuth.ReservationCode = dto.TransactionID;
            invoiceAuth.InvoiceTemplate = (dto.TransactionID ?? "") == "" ? invoiceAuth.InvoiceTemplate : dto.TransactionID;
            invoiceAuth.Status = "Chờ ký";
            await this.UpdateAsync(invoiceAuth);

            // updata sổ cái
            var ledger = await _ledgerService.GetQueryableAsync();
            var lstLedger = ledger.Where(p => p.VoucherId == dto.Id).ToList();
            foreach (var itemLedger in lstLedger)
            {
                itemLedger.InvoiceNumber = dto.InvoiceNumber;
                await _ledgerService.UpdateAsync(itemLedger);
            }

            // updata sổ kho
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWarehouseBook = warehouseBook.Where(p => p.ProductVoucherId == dto.Id).ToList();
            foreach (var itemWarehouseBook in lstWarehouseBook)
            {
                itemWarehouseBook.InvoiceNumber = dto.InvoiceNumber;
                await _warehouseBookService.UpdateAsync(itemWarehouseBook);
            }

            // updata PS thuế
            var accTax = await _accTaxDetailService.GetQueryableAsync();
            var lstAccTax = accTax.Where(p => p.ProductVoucherId == dto.Id).ToList();
            foreach (var itemAccTax in lstAccTax)
            {
                itemAccTax.InvoiceNumber = dto.InvoiceNumber;
                await _accTaxDetailService.UpdateAsync(itemAccTax);
            }

            // updata PS tiêu thụ đb
            var voucherExciseTax = await _voucherExciseTaxService.GetQueryableAsync();
            var lstVoucherExciseTax = voucherExciseTax.Where(p => p.ProductVoucherId == dto.Id).ToList();
            foreach (var itemVoucherExciseTax in lstVoucherExciseTax)
            {
                itemVoucherExciseTax.InvoiceNumber = dto.InvoiceNumber;
                await _voucherExciseTaxService.UpdateAsync(itemVoucherExciseTax);
            }

            // updata phiếu HV
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.Id == dto.Id).ToList();
            foreach (var itemProductVoucher in lstProductVoucher)
            {
                itemProductVoucher.InvoiceNumber = dto.InvoiceNumber;
                await _productVoucherService.UpdateAsync(itemProductVoucher);
            }
        }
    }
}