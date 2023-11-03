using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.InventoryRecords
{
    public class CrudInventoryRecordDto : CruOrgBaseDto
    {
        public string VoucherCode { get; set; }
        public int? Year { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        //NGAY_DC
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? TransDate { get; set; }
        //DAI_DIEN1
        public string Representative1 { get; set; }
        public string Position1 { get; set; }
        //ONG_BA1
        public string OtherContact1 { get; set; }
        //DAI_DIEN2
        public string Representative2 { get; set; }
        public string Position2 { get; set; }
        //ONG_BA2
        public string OtherContact2 { get; set; }
        //DAI_DIEN2
        public string Representative3 { get; set; }
        public string Position3 { get; set; }
        //ONG_BA2
        public string OtherContact3 { get; set; }
        public string Description { get; set; }
        //T_TIEN_KT
        public decimal? TotalAuditAmount { get; set; }
        //T_TIEN_KK
        public decimal? TotalInventoryAmount { get; set; }
        //T_TIEN_THUA
        public decimal? TotalOverAmount { get; set; }
        //T_TIEN_THIEU
        public decimal? TotalShortAmount { get; set; }
        public List<CrudInventoryRecordDetailDto> InventoryRecordDetails { get; set; }
    }
}
