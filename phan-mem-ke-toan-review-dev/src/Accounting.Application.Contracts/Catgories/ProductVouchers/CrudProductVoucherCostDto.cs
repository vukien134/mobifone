using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.ProductVouchers
{
    public class CrudProductVoucherCostDto : CruOrgBaseDto
    {
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string CostType { get; set; }
        public string DebitAcc { get; set; }
        public decimal? DebitExchange { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ContractCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CreditAcc { get; set; }
        public decimal? CreditExchange { get; set; }
        public string ClearingPartnerCode { get; set; }
        public string ClearingFProductWorkCode { get; set; }
        public string ClearingContractCode { get; set; }
        public string ClearingSectionCode { get; set; }
        public string ClearingWorkPlaceCode { get; set; }
        public string CaseCode { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
       
    }
}
