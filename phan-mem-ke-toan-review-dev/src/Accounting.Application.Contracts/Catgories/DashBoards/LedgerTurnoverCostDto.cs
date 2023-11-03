using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class LedgerTurnoverCostDto
    {
        public int Month { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Amount0 { get; set; }
    }
}
