using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.ImportExports
{
    public class InventorySummaryBookDto
    {
        public string Sort { get; set; }
        public string Css { get; set; }
        public string AddMenu { get; set; }
        public string TypeRow { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string Acc { get; set; }
        public string WarehouseCode { get; set; }
        public int RankProductGroup { get; set; }
        public string OrdProductGroup { get; set; }
        public string OrdGroup { get; set; }
        public string ProductCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string UnitCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductName { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ImportQuantity1 { get; set; }
        public decimal? ImportPrice1 { get; set; }
        public decimal? ImportAmount1 { get; set; }
        public decimal? ImportAmountCur1 { get; set; }
        public decimal? ImportQuantity2 { get; set; }
        public decimal? ImportPrice2 { get; set; }
        public decimal? ImportAmount2 { get; set; }
        public decimal? ImportAmountCur2 { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? AmountCur2 { get; set; }
        public string Specification { get; set; }
        public decimal? AvgBInventoryPrice { get; set; } // Giá bình quân tồn đầu
        public decimal? AvgEInventoryPrice { get; set; } // Giá bình quân tồn cuối
        public decimal? AvgImportPrice { get; set; } // Giá bình quân nhập
        public decimal? AvgExportPrice { get; set; } //  Giá bình quân xuất
        public decimal? QuantityMin { get; set; }
        public decimal? QuantityMax { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ExpiryDate { get; set; }
        public int OverdueDate { get; set; }
        public int ComingDate { get; set; }
        public int Year { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string PartnerName { get; set; }
        public string AttachPartner { get; set; }
        public string AttachProductCost { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherId { get; set; }
        public string VoucherOrd { get; set; }
        public string Ord0 { get; set; }
        public string Note { get; set; }
        public string ReciprocalAcc { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ToDate { get; set; }
        public string IncurredAcc { get; set; }
        public string Representative { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkName { get; set; }
        public string ProductNameHTML
        {
            get
            {
                return this.ProductName.Replace(" ", "&nbsp;");
            }
        }
    }
}
