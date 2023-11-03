using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.InventoryRecords
{
    public class IRCreateVoucherDto
    {
        public string Id { get; set; }
        public string AdjustAcc { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime DateExport { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime DateImport { get; set; }
        public string DescriptionExport { get; set; }
        public string DescriptionImport { get; set; }
        public string PartnerCodeExport { get; set; }
        public string PartnerCodeImport { get; set; }
        public string VoucherCodeExport { get; set; }
        public string VoucherCodeImport { get; set; }
    }
}