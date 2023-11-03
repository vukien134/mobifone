using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Contracts
{
    public class CrudContractDetailDto : CruOrgBaseDto
    {
        public string ContractId { get; set; }
        public string Ord0 { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal? Quantity { get; set; }
        //Số lượng thực hiện
        public decimal? TrxQuantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? TrxPriceCur { get; set; }
        public decimal? TrxPrice { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TrxAmountCur { get; set; }
        public decimal? TrxAmount { get; set; }
        public string Note { get; set; }
    }
}
