using System;
using Accounting.BaseDtos;
using Accounting.JsonConverters;
using NPOI.SS.Formula.Functions;

namespace Accounting.Catgories.Others
{
    public class CrudExchanRateDto : CruOrgBaseDto
    {
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherId { get; set; }
        public decimal? DebitExchangeRate { get; set; }
        public decimal? CreditExchangeRate { get; set; }
        public decimal? Amount { get; set; }
        public string NoteRefunds { get; set; }
        public string DebitAccRefunds { get; set; }
        public string CreditAccRefunds { get; set; }
        public string PartnerRefunds { get; set; }
        public string ContractRefunds { get; set; }
        public string FProductWorkRefunds { get; set; }
        public string SectionCodeRefunds { get; set; }
        public string WorkPlaceRefunds { get; set; }
        public decimal? AmountRefunds { get; set; }
        public string CurrencyCode { get; set; }
        public string VoucherKind { get; set; }
        public int IsHt2 { get; set; }
        public int IsPbVat { get; set; }
        public int IsPbCf { get; set; }
        public int ExchangeMethod { get; set; }
        public string Ord0 { get; set; }
        public decimal? ExchanRate { get; set; }
    }
}

