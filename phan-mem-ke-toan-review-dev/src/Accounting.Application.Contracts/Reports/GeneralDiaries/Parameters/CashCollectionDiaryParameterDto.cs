using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class CashCollectionDiaryParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string AccCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DebitCredit { get; set; }
        public string Acc1 { get; set; }
        public string Acc2 { get; set; }
        public string Acc3 { get; set; }
        public string Acc4 { get; set; }
        public string Acc5 { get; set; }
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
