using System;
namespace Accounting.Reports.Others
{
    public class BalanceTransferDto
    {
        public int? AccountOpeningBalance { get; set; }
        public int? ProductOpeningBalance { get; set; }
        public string LstDataForYear { get; set; }
    }
}

