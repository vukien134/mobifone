using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class FinancialSituationDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }

    }
}
