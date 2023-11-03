using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.ImportExports.Parameters
{
    public class InventorySummaryBookParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string AccCode { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string ProductGroupCode { get; set; } = "";
        public string WarehouseCode { get; set; } = "";
        public string ProductLotCode { get; set; } = "";
        public string ProductOriginCode { get; set; } = "";
        public bool CheckWarehouse { get; set; } = true;
        public bool CheckProductLot { get; set; } = false;
        public bool CheckProductOrigin { get; set; } = false;
        public bool CheckAcc { get; set; } = false;
        public bool CheckTransfer { get; set; } = false;
        public string Type { get; set; } = "PS";
        public DateTime? FromDate0
        {
            get
            {
                return FromDate;
            }
        }

        public DateTime? ToDate0
        {
            get
            {
                return ToDate;
            }
        }
    }
}
