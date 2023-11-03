using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.Contracts
{
    public class CrudContractDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string PartnerCode { get; set; }
        public string ContractType { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? SignedDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? InvoiceDate { get; set; }
        public string Note { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        //Số lượng thực hiện
        public decimal TrxQuantity { get; set; }
        public decimal PriceCur { get; set; }
        public decimal Price { get; set; }
        public decimal TrxPriceCur { get; set; }
        public decimal TrxPrice { get; set; }
        public decimal AmountCur { get; set; }
        public decimal Amount { get; set; }
        public decimal TrxAmountCur { get; set; }
        public decimal TrxAmount { get; set; }
        public string NoteDetal { get; set; }
        public List<CrudContractDetailDto> ContractDetails { get; set; }
    }
}
