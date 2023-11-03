using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class CutomerBalanceDetailBookParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerGroup { get; set; } = "";
        public string SectionCode { get; set; } = "";
        public string WorkPlaceCode { get; set; } = "";
        public string CurrencyCode { get; set; } = "";
        public string FProductWorkCode { get; set; } = "";
        public string WarehouseCode { get; set; } = "";
        public string ProductGroupCode { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string ProductLotCode { get; set; } = "";
        public string ProductOrigin { get; set; } = "";
        public string ContractCode { get; set; } = "";
        public int AccRank { get; set; }
        public string CleaningAcc { get; set; }
        public string CleaningFProductWork { get; set; }
        public int Incurred { get; set; }
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
