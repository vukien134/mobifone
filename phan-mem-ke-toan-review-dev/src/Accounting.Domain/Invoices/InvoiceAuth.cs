﻿using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Invoices
{
    public class InvoiceAuth : TenantOrgEntity
    {
        public string InvoiceType { get; set; }
        public Guid? InvoiceCodeId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? SubmmittedDate { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string InvoiceNote { get; set; }
        public int? AdjustmentType { get; set; }
        public Guid? OriginalInvoiceId { get; set; }
        public string AdditionalReferenceDes { get; set; }
        public DateTime? AdditionalReferenceDate { get; set; }
        public string BuyerDisplayName { get; set; }
        //ma_dt
        public string PartnerCode { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerAddressLine { get; set; }
        public string BuyerMobile { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankAccount { get; set; }
        public string BuyerBankName { get; set; }
        public string PaymentMethodName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerBankAccount { get; set; }        
        //trang_thai
        public string Status { get; set; }
        public string VoucherCode { get; set; }
        public DateTime? SignedDate { get; set; }
        //mau_hd
        public string InvoiceTemplate { get; set; }
        //so_benh_an
        public string OrderNumber { get; set; }
        //sovb
        public string DocumentNumber { get; set; }
        //ngayvb
        public DateTime? DocumentDate { get; set; }
        public string Note { get; set; }
        //so_hd_dc
        public string AdjustmentInvoiceNumber { get; set; }
        public string Signature { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public DateTime? DeliveryOrderDate { get; set; }
        public string DeliveryBy { get; set; }
        public string TransportationMethod { get; set; }
        public string FromWarehouseName { get; set; }
        public string ToWarehouseName { get; set; }
        public string ReservationCode { get; set; }
        public string BillCode { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string AmountByWord { get; set; }
        public string SupplierCode { get; set; }
        public string OtherDepartment { get; set; }
        public string BusinessCode { get; set; }
        public string CommandNumber { get; set; }
        public string Representative { get; set; }
        public string DepartmentCode { get; set; }
        public string PartnerName { get; set; }
        public DateTime? ImportDate { get; set; }
        public DateTime? ExportDate { get; set; }
        public int? Year { get; set; }
        public ICollection<InvoiceAuthDetail> InvoiceAuthDetails { get; set; }
    }
}
