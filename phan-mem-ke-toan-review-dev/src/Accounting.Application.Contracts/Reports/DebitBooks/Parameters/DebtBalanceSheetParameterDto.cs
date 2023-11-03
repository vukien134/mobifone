using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class DebtBalanceSheetParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerGroupCode { get; set; }
        public string PartnerGroupId { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string Type { get; set; } = "PS";
        public int Sort { get; set; } // 1. Theo mã đối tượng, 2. Theo tài khoản công nợ
        public bool CleaningFProductWork { get; set; } // Bù trừ ct, sp: 1. có bt, 2 không bt
        public DateTime? FromDate0
        {
            get
            {
                return this.FromDate;
            }
        }

        public DateTime? ToDate0
        {
            get
            {
                return this.ToDate;
            }
        }
    }
}
