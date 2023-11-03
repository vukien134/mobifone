using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class DataAdjustDepreciationDetailDto
    {
        public string AdjustDepreciationId { get; set; }
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public string AssetToolDetailId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DepreciationAmount { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string DepartmentCode { get; set; }
        public string UpDownCode { get; set; }
        //Mã nguồn vốn
        public string CapitalCode { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Impoverishment { get; set; }
        public decimal? MonthNumber0 { get; set; }
    }
}
